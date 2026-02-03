# ?? Guía de Tests Funcionales - GtMotive Rental Microservice

## ?? Índice
1. [¿Qué son los Tests Funcionales?](#qué-son-los-tests-funcionales)
2. [Arquitectura de Tests](#arquitectura-de-tests)
3. [Estructura del Proyecto](#estructura-del-proyecto)
4. [Prerequisitos](#prerequisitos)
5. [Cómo Ejecutar los Tests](#cómo-ejecutar-los-tests)
6. [Tests Implementados](#tests-implementados)
7. [Patrón de Implementación](#patrón-de-implementación)
8. [Troubleshooting](#troubleshooting)
9. [Mejores Prácticas](#mejores-prácticas)

---

## ?? ¿Qué son los Tests Funcionales?

Los **Tests Funcionales** (también conocidos como **E2E** o **Integration Tests**) son pruebas que validan **flujos completos** de la aplicación, utilizando componentes reales en lugar de mocks.

### Comparación con Otros Tipos de Tests

| Característica | Unit Tests | Infrastructure Tests | Functional Tests |
|----------------|-----------|---------------------|------------------|
| **Alcance** | Clase aislada | Repositorios + BD | UseCase + Repositorios + BD |
| **Dependencies** | Todo mockeado | Solo BD real | BD real + UseCases reales |
| **Velocidad** | ? Muy rápido (ms) | ?? Lento (segundos) | ?? Lento (segundos) |
| **Propósito** | Validar lógica de negocio | Validar persistencia | Validar flujos E2E |
| **Configuración** | Simple | Requiere BD | Requiere BD + DI Container |
| **Ejemplo** | `Vehicle.IsEligibleForFleet()` | `VehicleRepository.AddAsync()` | Crear vehículo ? Alquilarlo ? Verificar estado |

### ¿Cuándo usar Tests Funcionales?

? **Úsalos cuando:**
- Necesitas validar que **múltiples componentes funcionan juntos**
- Quieres probar **flujos de negocio completos** (ej: crear vehículo ? alquilar ? devolver)
- Necesitas verificar que la **integración entre capas** funciona correctamente
- Quieres detectar problemas de **configuración del DI container**

? **No los uses cuando:**
- Solo necesitas validar lógica de negocio aislada (usa Unit Tests)
- Solo necesitas probar la persistencia (usa Infrastructure Tests)
- El test sería muy lento y no aporta valor adicional

---

## ??? Arquitectura de Tests

### Diagrama de Componentes

```
???????????????????????????????????????????????????????????????
?                  Functional Test Project                      ?
?                                                               ?
?  ????????????????????????????????????????????????????????  ?
?  ?  Test Classes                                         ?  ?
?  ?  - VehicleFunctionalTests                            ?  ?
?  ?  - CustomerFunctionalTests                           ?  ?
?  ?  - RentalFunctionalTests                             ?  ?
?  ????????????????????????????????????????????????????????  ?
?                          ? usa                               ?
?  ????????????????????????????????????????????????????????  ?
?  ?  FunctionalTestBase                                   ?  ?
?  ?  - Limpia BD antes de cada test                      ?  ?
?  ?  - Proporciona acceso al Fixture                     ?  ?
?  ????????????????????????????????????????????????????????  ?
?                          ? usa                               ?
?  ????????????????????????????????????????????????????????  ?
?  ?  CompositionRootTestFixture                          ?  ?
?  ?  - Configura DI Container                            ?  ?
?  ?  - Carga appsettings.json                            ?  ?
?  ?  - Registra servicios reales                         ?  ?
?  ????????????????????????????????????????????????????????  ?
?                                                               ?
???????????????????????????????????????????????????????????????
                          ? interactúa con
        ????????????????????????????????????????????
        ?         MongoDB (Real Database)          ?
        ?         localhost:27017                  ?
        ?         RentalFunctionalTestDb           ?
        ????????????????????????????????????????????
                          ?
                          ? usa
        ????????????????????????????????????????????
        ?     UseCases & Repositories (Reales)     ?
        ?     - CreateVehicleUseCase               ?
        ?     - VehicleRepository                  ?
        ?     - CustomerRepository                 ?
        ?     - RentalRepository                   ?
        ????????????????????????????????????????????
```

### Flujo de Ejecución de un Test

```
1. [xUnit] ? Crea CompositionRootTestFixture (una vez por colección)
                    ?
2. [Fixture] ? Configura DI Container con servicios reales
                    ?
3. [Test] ? Hereda de FunctionalTestBase
                    ?
4. [FunctionalTestBase.InitializeAsync()] ? Limpia BD
                    ?
5. [Test.Arrange] ? Crea TestOutputPort y UseCases
                    ?
6. [Test.Act] ? Ejecuta UseCase.ExecuteAsync()
                    ?
7. [UseCase] ? Usa Repository real ? MongoDB real
                    ?
8. [Test.Assert] ? Verifica OutputPort y BD
                    ?
9. [Test.Complete] ? Limpia BD para siguiente test
```

---

## ?? Estructura del Proyecto

```
GtMotive.Estimate.Microservice.FunctionalTests/
?
??? ?? Infrastructure/                           # Infraestructura base
?   ??? CompositionRootTestFixture.cs           # ? DI Container
?   ??? CompositionRootCollectionFixture.cs     # xUnit Collection
?   ??? FunctionalTestBase.cs                   # ? Clase base para tests
?   ??? TestCollections.cs                      # Definición de colecciones
?
??? ?? Vehicles/                                 # Tests de Vehículos
?   ??? VehicleFunctionalTests.cs               # 4 tests
?       ??? CreateVehicleCompleteFlowShouldSucceed
?       ??? CreateVehicleTooOldShouldFail
?       ??? CreateMultipleVehiclesAndFilterByStatusShouldWork
?       ??? GetVehiclesByStatusWithNoMatchesShouldReturnEmpty
?
??? ?? Customers/                                # Tests de Clientes
?   ??? CustomerFunctionalTests.cs              # 4 tests
?   ?   ??? CreateCustomerCompleteFlowShouldSucceed
?   ?   ??? CreateCustomerWithDuplicateEmailShouldFail
?   ?   ??? CreateMultipleCustomersAndGetAllShouldWork
?   ?   ??? GetAllCustomersWhenNoneExistShouldReturnEmpty
?   ?
?   ??? TestCreateCustomerOutputPort.cs         # Test Output Ports
?       ??? TestCreateCustomerOutputPort
?       ??? TestGetAllCustomersOutputPort
?
??? ?? Rentals/                                  # Tests de Alquileres
?   ??? RentalFunctionalTests.cs                # 5 tests + helper
?       ??? CompleteRentalFlowShouldSucceed
?       ??? CompleteReturnVehicleFlowShouldSucceed
?       ??? GetRentalByLicensePlateShouldReturnActiveRental
?       ??? GetAllRentalsShouldReturnAllActiveRentals
?       ??? CustomerWithActiveRentalCannotRentAgain
?
??? appsettings.json                             # ? Configuración de MongoDB
??? GlobalSuppressions.cs                        # Supresiones de warnings
??? README.md                                    # Documentación técnica
??? FIXES_APPLIED.md                             # Registro de correcciones
??? GUIA_TESTS_FUNCIONALES.md                   # ?? Esta guía
```

### Archivos Clave

#### 1. **CompositionRootTestFixture.cs**
Configura el DI Container con todos los servicios reales.

```csharp
public sealed class CompositionRootTestFixture : IDisposable, IAsyncLifetime
{
    private readonly ServiceProvider _serviceProvider;
    public IConfiguration Configuration { get; }
    
    public CompositionRootTestFixture()
    {
        // Carga configuración desde appsettings.json
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
            
        var services = new ServiceCollection();
        
        // Registra servicios reales
        services.AddApiDependencies();      // UseCases
        services.AddBaseInfrastructure(true)
                .AddMongoDb(Configuration); // Repositorios + MongoDB
        
        _serviceProvider = services.BuildServiceProvider();
    }
}
```

#### 2. **FunctionalTestBase.cs**
Clase base que limpia la BD antes de cada test.

```csharp
public abstract class FunctionalTestBase(CompositionRootTestFixture fixture) 
    : IAsyncLifetime
{
    protected CompositionRootTestFixture Fixture { get; } = fixture;
    
    public async Task InitializeAsync()
    {
        // ?? Limpia BD antes de cada test
        await CleanDatabaseAsync();
    }
    
    private async Task CleanDatabaseAsync()
    {
        // Elimina colecciones: Vehicles, Customers, Rentals
    }
}
```

#### 3. **appsettings.json**
Configuración de MongoDB para tests.

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "RentalFunctionalTestDb"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

---

## ? Prerequisitos

### 1. Software Requerido

| Software | Versión | Propósito |
|----------|---------|-----------|
| **.NET SDK** | 9.0 | Runtime de la aplicación |
| **Docker Desktop** | Latest | Para ejecutar MongoDB |
| **MongoDB** | 8.0+ | Base de datos (via Docker) |
| **Visual Studio** | 2022+ | IDE (opcional, también funciona con VS Code) |

### 2. Verificar Instalaciones

```bash
# Verificar .NET
dotnet --version
# Debe mostrar: 9.0.xxx

# Verificar Docker
docker --version
# Debe mostrar: Docker version 20.10.x o superior

# Verificar que Docker está corriendo
docker ps
# No debe mostrar error
```

### 3. Iniciar MongoDB

**Opción A: Primera vez (crear contenedor)**
```bash
docker run -d \
  --name mongodb-functional \
  -p 27017:27017 \
  mongo:latest
```

**Opción B: Contenedor ya existe (iniciar)**
```bash
docker start mongodb-functional
```

**Verificar que MongoDB está corriendo:**
```bash
docker ps | grep mongodb-functional
```

Deberías ver algo como:
```
CONTAINER ID   IMAGE          STATUS         PORTS                      NAMES
a1b2c3d4e5f6   mongo:latest   Up 2 seconds   0.0.0.0:27017->27017/tcp   mongodb-functional
```

### 4. Verificar Conexión a MongoDB (Opcional)

```bash
# Si tienes mongosh instalado
mongosh mongodb://localhost:27017

# Dentro de mongosh:
show dbs
use RentalFunctionalTestDb
show collections
```

---

## ?? Cómo Ejecutar los Tests

### Método 1: Visual Studio (Recomendado)

#### Paso 1: Abrir Test Explorer
- **Menú**: `Test` ? `Test Explorer` (o `Ctrl + E, T`)

#### Paso 2: Ejecutar Tests
- **Todos los tests**: Click derecho en "Functional Tests" ? `Run`
- **Tests específicos**: Click derecho en el test ? `Run`
- **Por categoría**: 
  - Click derecho en `VehicleFunctionalTests` ? `Run`
  - Click derecho en `CustomerFunctionalTests` ? `Run`
  - Click derecho en `RentalFunctionalTests` ? `Run`

#### Paso 3: Ver Resultados
- ? Verde = Test pasó
- ? Rojo = Test falló (ver Output para detalles)
- ? Amarillo = Test en ejecución

### Método 2: Línea de Comandos

#### Navegar al Proyecto
```bash
cd C:\Users\mgarciaa\Desktop\PruebaTecnica\test\functional\GtMotive.Estimate.Microservice.FunctionalTests
```

#### Ejecutar Todos los Tests
```bash
dotnet test
```

**Salida esperada:**
```
Starting test execution, please wait...
A total of 13 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    13, Skipped:     0, Total:    13, Duration: 5s
```

#### Ejecutar con Verbose Output
```bash
dotnet test --verbosity normal
```

#### Ejecutar Tests Específicos por Filtro

**Solo tests de Vehicles:**
```bash
dotnet test --filter "FullyQualifiedName~Vehicle"
```

**Solo tests de Customers:**
```bash
dotnet test --filter "FullyQualifiedName~Customer"
```

**Solo tests de Rentals:**
```bash
dotnet test --filter "FullyQualifiedName~Rental"
```

**Un test específico:**
```bash
dotnet test --filter "FullyQualifiedName~CreateVehicleCompleteFlowShouldSucceed"
```

#### Ejecutar con Logger
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Método 3: Rider (JetBrains)

1. Abrir ventana de **Unit Tests** (`Alt + 8`)
2. Click derecho en `GtMotive.Estimate.Microservice.FunctionalTests`
3. Seleccionar `Run Unit Tests`

### Método 4: VS Code

```bash
# Instalar extensión C# Dev Kit si no la tienes

# En terminal integrado:
cd test/functional/GtMotive.Estimate.Microservice.FunctionalTests
dotnet test
```

---

## ?? Tests Implementados

### ?? VehicleFunctionalTests (4 tests)

#### 1. `CreateVehicleCompleteFlowShouldSucceed`
**Propósito:** Valida que se puede crear un vehículo correctamente.

**Flujo:**
1. Crea un vehículo con datos válidos
2. Ejecuta `CreateVehicleUseCase`
3. Verifica que el output port fue llamado correctamente
4. Verifica que el vehículo se guardó en MongoDB

**Assertions:**
```csharp
outputPort.WasStandardHandled.Should().BeTrue();
outputPort.Output.Id.Should().NotBeNullOrEmpty();
outputPort.Output.Status.Should().Be(VehicleStatus.Available.ToString());

// Verificar en BD
var savedVehicle = await repository.GetByIdAsync(outputPort.Output.Id);
savedVehicle.Should().NotBeNull();
savedVehicle.Brand.Should().Be("Toyota");
```

---

#### 2. `CreateVehicleTooOldShouldFail`
**Propósito:** Valida que no se pueden crear vehículos demasiado viejos.

**Regla de Negocio:** Los vehículos deben tener máximo 5 años de antigüedad.

**Flujo:**
1. Intenta crear un vehículo de hace 10 años
2. Ejecuta `CreateVehicleUseCase`
3. Verifica que el output port maneje el error

**Assertions:**
```csharp
outputPort.WasNotFoundHandled.Should().BeTrue();
outputPort.WasStandardHandled.Should().BeFalse();
outputPort.ErrorMessage.Should().Contain("too old");
```

---

#### 3. `CreateMultipleVehiclesAndFilterByStatusShouldWork`
**Propósito:** Valida el flujo de crear múltiples vehículos y filtrar por estado.

**Flujo:**
1. Crea 3 vehículos (todos `Available`)
2. Ejecuta `GetVehiclesByStatusUseCase` con filtro `Available`
3. Verifica que retorna exactamente 3 vehículos

**Assertions:**
```csharp
getByStatusOutputPort.Output.Vehicles.Should().HaveCount(3);
getByStatusOutputPort.Output.Vehicles.Should().AllSatisfy(v =>
    v.Status.Should().Be(VehicleStatus.Available.ToString()));
```

---

#### 4. `GetVehiclesByStatusWithNoMatchesShouldReturnEmpty`
**Propósito:** Valida que una consulta sin resultados retorna lista vacía.

**Flujo:**
1. No crea ningún vehículo
2. Ejecuta `GetVehiclesByStatusUseCase` con filtro `Retired`
3. Verifica que retorna lista vacía

**Assertions:**
```csharp
outputPort.Output.Vehicles.Should().BeEmpty();
outputPort.Output.TotalCount.Should().Be(0);
```

---

### ?? CustomerFunctionalTests (4 tests)

#### 1. `CreateCustomerCompleteFlowShouldSucceed`
**Propósito:** Valida que se puede crear un cliente correctamente.

**Flujo:**
1. Crea un cliente con datos válidos (email único)
2. Ejecuta `CreateCustomerUseCase`
3. Verifica en output port y en BD

**Datos de Prueba:**
```csharp
var input = new CreateCustomerInput
{
    Name = "John Doe",
    Email = $"john.doe.{Guid.NewGuid()}@example.com", // Email único
    PhoneNumber = "+34600123456",
    DriverLicenseNumber = $"DL{Guid.NewGuid()}"
};
```

---

#### 2. `CreateCustomerWithDuplicateEmailShouldFail`
**Propósito:** Valida que no se pueden crear clientes con email duplicado.

**Regla de Negocio:** Los emails deben ser únicos.

**Flujo:**
1. Crea primer cliente con email `duplicate@example.com`
2. Intenta crear segundo cliente con el mismo email
3. Verifica que el segundo intento falla

**Assertions:**
```csharp
outputPort2.WasNotFoundHandled.Should().BeTrue();
outputPort2.ErrorMessage.Should().Contain("already exists");
```

---

#### 3. `CreateMultipleCustomersAndGetAllShouldWork`
**Propósito:** Valida el flujo de crear múltiples clientes y obtener todos.

**Flujo:**
1. Crea 5 clientes
2. Ejecuta `GetAllCustomersUseCase`
3. Verifica que retorna exactamente 5 clientes

---

#### 4. `GetAllCustomersWhenNoneExistShouldReturnEmpty`
**Propósito:** Valida que una consulta sin datos retorna lista vacía.

---

### ?? RentalFunctionalTests (5 tests + helper)

#### 1. `CompleteRentalFlowShouldSucceed`
**Propósito:** Valida el flujo completo de alquiler.

**Flujo Completo:**
```
1. Crear Vehículo (BMW X5)
        ?
2. Crear Cliente (Jane Smith)
        ?
3. Alquilar Vehículo
        ?
4. Verificar:
   - ? Rental creado
   - ? Vehículo.Status = Rented
   - ? Cliente.HasActiveRental = true
```

**Código del Test:**
```csharp
// 1. Crear vehículo
var createVehicleOutputPort = new TestCreateVehicleOutputPort();
var createVehicleUseCase = new CreateVehicleUseCase(createVehicleOutputPort, vehicleRepository);
await createVehicleUseCase.ExecuteAsync(vehicleInput, CancellationToken.None);
var vehicleId = createVehicleOutputPort.Output.Id;

// 2. Crear cliente
var createCustomerOutputPort = new TestCreateCustomerOutputPort();
var createCustomerUseCase = new CreateCustomerUseCase(createCustomerOutputPort, customerRepository);
await createCustomerUseCase.ExecuteAsync(customerInput, CancellationToken.None);
var customerId = createCustomerOutputPort.Output.Id;

// 3. Alquilar vehículo
var rentInput = new RentVehicleInput
{
    VehicleId = vehicleId,
    CustomerId = customerId,
    ExpectedReturnDate = DateTime.UtcNow.AddDays(7)
};
var rentOutputPort = new TestRentVehicleOutputPort();
var rentUseCase = new RentVehicleUseCase(rentOutputPort, vehicleRepository, customerRepository, rentalRepository);
await rentUseCase.ExecuteAsync(rentInput, CancellationToken.None);

// 4. Assertions
rentOutputPort.WasStandardHandled.Should().BeTrue();

var vehicle = await vehicleRepository.GetByIdAsync(vehicleId, CancellationToken.None);
vehicle.Status.Should().Be(VehicleStatus.Rented);

var customer = await customerRepository.GetByIdAsync(customerId, CancellationToken.None);
customer.HasActiveRental.Should().BeTrue();
```

---

#### 2. `CompleteReturnVehicleFlowShouldSucceed`
**Propósito:** Valida el flujo completo de devolución de vehículo.

**Flujo:**
```
1. Setup: Crear vehículo, cliente y alquiler (helper)
        ?
2. Devolver Vehículo
        ?
3. Verificar:
   - ? Rental.Status = Completed
   - ? Rental.ReturnDate != null
   - ? Vehículo.KilometersDriven actualizado
   - ? Cliente.HasActiveRental = false
```

---

#### 3. `GetRentalByLicensePlateShouldReturnActiveRental`
**Propósito:** Valida que se puede buscar un alquiler por matrícula.

**Flujo:**
1. Setup: Crear alquiler activo
2. Buscar por matrícula: `GetRentalByLicensePlateUseCase`
3. Verifica que retorna el alquiler correcto

---

#### 4. `GetAllRentalsShouldReturnAllActiveRentals`
**Propósito:** Valida que se pueden obtener todos los alquileres.

**Flujo:**
1. Crea 3 alquileres
2. Ejecuta `GetAllRentalsUseCase`
3. Verifica que retorna exactamente 3 alquileres

---

#### 5. `CustomerWithActiveRentalCannotRentAgain`
**Propósito:** Valida regla de negocio crítica.

**Regla de Negocio:** Un cliente no puede alquilar dos vehículos simultáneamente.

**Flujo:**
```
1. Cliente alquila Vehículo A (exitoso)
        ?
2. Mismo cliente intenta alquilar Vehículo B
        ?
3. Debe fallar con: "already has an active rental"
```

**Assertions:**
```csharp
rentOutputPort.WasNotFoundHandled.Should().BeTrue();
rentOutputPort.ErrorMessage.Should().Contain("already has an active rental");
```

---

### ??? Helper Method: `SetupRentalAsync`

**Propósito:** Crear un alquiler completo para reutilizar en tests.

**Retorna:**
```csharp
(string VehicleId, string CustomerId, string RentalId, string LicensePlate)
```

**Uso:**
```csharp
var (vehicleId, customerId, rentalId, licensePlate) = await SetupRentalAsync(scope);
```

---

## ?? Patrón de Implementación

### Estructura de un Test Funcional

```csharp
[Fact]
public async Task MyFunctionalTest()
{
    // ???????????????????????????????????????????????????????
    // ARRANGE - Configurar el test
    // ???????????????????????????????????????????????????????
    
    // 1?? Obtener el scope del DI Container
    using var scope = CompositionRootTestFixtureExtensions.CreateScope(Fixture);
    
    // 2?? Obtener repositorios del DI
    var repository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
    
    // 3?? Crear Test Output Port
    var outputPort = new TestCreateVehicleOutputPort();
    
    // 4?? Instanciar UseCase manualmente (inyectando output port)
    var useCase = new CreateVehicleUseCase(outputPort, repository);
    
    // 5?? Preparar el input
    var input = new CreateVehicleInput
    {
        Brand = "Toyota",
        Model = "Camry",
        Year = DateTime.UtcNow.Year - 2,
        LicensePlate = $"TEST-{Guid.NewGuid().ToString()[..4]}",
        KilometersDriven = 50000
    };
    
    // ???????????????????????????????????????????????????????
    // ACT - Ejecutar el UseCase
    // ???????????????????????????????????????????????????????
    
    // ?? IMPORTANTE: Solo 2 parámetros (input, CancellationToken)
    await useCase.ExecuteAsync(input, CancellationToken.None);
    
    // ???????????????????????????????????????????????????????
    // ASSERT - Verificar resultados
    // ???????????????????????????????????????????????????????
    
    // 6?? Verificar que el output port fue llamado correctamente
    outputPort.WasStandardHandled.Should().BeTrue();
    outputPort.Output.Should().NotBeNull();
    outputPort.Output.Id.Should().NotBeNullOrEmpty();
    
    // 7?? Verificar en la base de datos (opcional pero recomendado)
    var savedVehicle = await repository.GetByIdAsync(
        outputPort.Output.Id, 
        CancellationToken.None
    );
    savedVehicle.Should().NotBeNull();
    savedVehicle.Brand.Should().Be("Toyota");
    savedVehicle.Status.Should().Be(VehicleStatus.Available);
}
```

### Componentes Clave

#### 1. Test Output Ports

**¿Qué son?**  
Implementaciones de prueba de las interfaces `IOutputPort` que capturan las respuestas de los UseCases.

**Ejemplo:**
```csharp
internal class TestCreateVehicleOutputPort : ICreateVehicleOutputPort
{
    // Flags para tracking
    public bool WasStandardHandled { get; private set; }
    public bool WasNotFoundHandled { get; private set; }
    
    // Datos capturados
    public CreateVehicleOutput Output { get; private set; }
    public string ErrorMessage { get; private set; }
    
    // Implementación de interfaz
    public void StandardHandle(CreateVehicleOutput output)
    {
        WasStandardHandled = true;
        Output = output;
    }
    
    public void NotFoundHandle(string message)
    {
        WasNotFoundHandled = true;
        ErrorMessage = message;
    }
}
```

**¿Por qué se necesitan?**  
Los UseCases en este proyecto usan el patrón **Port-Adapter**:
- En producción, usan **Presenters** (ej: `CreateVehiclePresenter`)
- En tests, usamos **Test Output Ports** para capturar las respuestas

#### 2. Instanciación Manual de UseCases

**? Incorrecto:**
```csharp
// No funciona porque el DI inyectaría el Presenter real
var useCase = scope.ServiceProvider.GetRequiredService<CreateVehicleUseCase>();
```

**? Correcto:**
```csharp
// Instanciación manual con Test Output Port
var outputPort = new TestCreateVehicleOutputPort();
var repository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
var useCase = new CreateVehicleUseCase(outputPort, repository);
```

#### 3. Firma de ExecuteAsync

**?? IMPORTANTE:** El método `ExecuteAsync` **solo toma 2 parámetros**:

```csharp
// Firma en IUseCase.cs
Task ExecuteAsync(TUseCaseInput input, CancellationToken ct);
```

**? Incorrecto (3 parámetros):**
```csharp
await useCase.ExecuteAsync(input, outputPort, CancellationToken.None);
```

**? Correcto (2 parámetros):**
```csharp
await useCase.ExecuteAsync(input, CancellationToken.None);
```

El output port ya está inyectado vía constructor.

---

## ?? Troubleshooting

### Error: "Connection refused to MongoDB"

**Síntoma:**
```
MongoConnectionException: A timeout occurred after 30000ms selecting a server
```

**Causa:** MongoDB no está corriendo.

**Solución:**
```bash
# Verificar si el contenedor existe
docker ps -a | grep mongodb-functional

# Si existe pero está detenido
docker start mongodb-functional

# Si no existe, crearlo
docker run -d --name mongodb-functional -p 27017:27017 mongo:latest

# Verificar que está corriendo
docker ps | grep mongodb-functional
```

---

### Error: "Cannot resolve service"

**Síntoma:**
```
System.InvalidOperationException: Unable to resolve service for type 'IVehicleRepository'
```

**Causa:** El servicio no está registrado en el DI Container.

**Solución:**  
Verificar que `CompositionRootTestFixture.ConfigureServices()` registra todos los servicios:

```csharp
private void ConfigureServices(IServiceCollection services)
{
    services.AddApiDependencies();  // ? Registra UseCases
    services.AddLogging();
    services.AddBaseInfrastructure(true)
            .AddMongoDb(Configuration);  // ? Registra Repositorios
}
```

---

### Error: "Database name is null"

**Síntoma:**
```
ArgumentNullException: Value cannot be null. (Parameter 'name')
```

**Causa:** Falta configuración de MongoDB en `appsettings.json`.

**Solución:**  
Verificar que `appsettings.json` tiene:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "RentalFunctionalTestDb"
  }
}
```

---

### Tests Fallan Aleatoriamente

**Síntoma:**  
Los tests pasan individualmente pero fallan cuando se ejecutan todos juntos.

**Causa:** Datos residuales entre tests.

**Solución:**  
Verificar que `FunctionalTestBase.CleanDatabaseAsync()` limpia todas las colecciones:

```csharp
private async Task CleanDatabaseAsync()
{
    // ...
    await database.DropCollectionAsync("Vehicles");
    await database.DropCollectionAsync("Customers");
    await database.DropCollectionAsync("Rentals");
}
```

---

### Error: "ExecuteAsync no toma 3 argumentos"

**Síntoma:**
```
CS1501: No overload for method 'ExecuteAsync' takes 3 arguments
```

**Causa:** Intentas pasar el output port como parámetro.

**Solución:**  
El output port se inyecta vía constructor:

```csharp
// ? Incorrecto
await useCase.ExecuteAsync(input, outputPort, CancellationToken.None);

// ? Correcto
var outputPort = new TestOutputPort();
var useCase = new MyUseCase(outputPort, repository);
await useCase.ExecuteAsync(input, CancellationToken.None);
```

---

### Port 27017 Already in Use

**Síntoma:**
```
docker: Error response from daemon: Ports are not available: listen tcp 0.0.0.0:27017: bind: address already in use
```

**Causa:** Ya hay otro MongoDB corriendo en el puerto 27017.

**Solución:**

**Opción A: Usar el MongoDB existente**
```bash
# No hagas nada, los tests usarán el MongoDB que ya está corriendo
```

**Opción B: Detener el MongoDB existente**
```bash
# Buscar el contenedor
docker ps | grep mongo

# Detener el contenedor
docker stop <container_id>
```

**Opción C: Usar puerto diferente**  
Modificar `appsettings.json`:
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27018",
    "DatabaseName": "RentalFunctionalTestDb"
  }
}
```

Y crear contenedor en puerto diferente:
```bash
docker run -d --name mongodb-functional -p 27018:27017 mongo:latest
```

---

### Tests Muy Lentos

**Síntoma:**  
Los tests tardan más de 1 minuto en ejecutarse.

**Causas Posibles:**
1. MongoDB no está optimizado
2. Limpieza de BD muy lenta
3. Muchos datos de prueba

**Soluciones:**

**1. Optimizar limpieza de BD:**
```csharp
// En lugar de DropCollectionAsync, usar DeleteMany
await collection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
```

**2. Reducir datos de prueba:**
```csharp
// En lugar de crear 100 vehículos, crear solo 3
for (var i = 0; i < 3; i++) // En lugar de 100
{
    // ...
}
```

**3. Ejecutar tests en paralelo (con precaución):**
```csharp
[Collection("Parallel")] // Nueva colección
public class VehicleFunctionalTests { }
```

?? **Nota:** Ejecutar tests en paralelo puede causar conflictos si comparten datos.

---

## ?? Mejores Prácticas

### 1. Nombres de Tests Descriptivos

? **Bueno:**
```csharp
public async Task CreateVehicleCompleteFlowShouldSucceed()
public async Task CustomerWithActiveRentalCannotRentAgain()
```

? **Malo:**
```csharp
public async Task Test1()
public async Task VehicleTest()
```

**Patrón recomendado:**  
`[AcciónOCondición][Contexto]Should[ResultadoEsperado]`

---

### 2. Arrange-Act-Assert Claros

```csharp
[Fact]
public async Task MyTest()
{
    // ???????????????????????????????????????????????????????
    // ARRANGE
    // ???????????????????????????????????????????????????????
    var input = new CreateVehicleInput { /* ... */ };
    var useCase = new CreateVehicleUseCase(outputPort, repository);
    
    // ???????????????????????????????????????????????????????
    // ACT
    // ???????????????????????????????????????????????????????
    await useCase.ExecuteAsync(input, CancellationToken.None);
    
    // ???????????????????????????????????????????????????????
    // ASSERT
    // ???????????????????????????????????????????????????????
    outputPort.WasStandardHandled.Should().BeTrue();
}
```

---

### 3. Datos Únicos en Cada Test

? **Bueno:**
```csharp
var email = $"customer.{Guid.NewGuid().ToString()[..8]}@example.com";
var licensePlate = $"TEST-{Guid.NewGuid().ToString()[..4]}";
```

? **Malo:**
```csharp
var email = "test@example.com"; // Puede causar colisiones
var licensePlate = "TEST-1234"; // Hardcoded
```

---

### 4. Verificar en BD Cuando Sea Importante

```csharp
// ACT
await useCase.ExecuteAsync(input, CancellationToken.None);

// ASSERT - Output Port
outputPort.WasStandardHandled.Should().BeTrue();

// ASSERT - BD (verificación adicional importante)
var savedEntity = await repository.GetByIdAsync(outputPort.Output.Id, CancellationToken.None);
savedEntity.Should().NotBeNull();
savedEntity.Status.Should().Be(ExpectedStatus);
```

---

### 5. Usar Helpers para Flujos Complejos

**En lugar de duplicar código:**
```csharp
// ? Duplicar en cada test
var vehicle = await CreateVehicle();
var customer = await CreateCustomer();
var rental = await CreateRental(vehicle.Id, customer.Id);
```

**Usar helper method:**
```csharp
// ? Helper reutilizable
var (vehicleId, customerId, rentalId, licensePlate) = await SetupRentalAsync(scope);
```

---

### 6. Tests Independientes

? **Bueno:** Cada test crea sus propios datos.

```csharp
[Fact]
public async Task Test1()
{
    var vehicle = await CreateVehicle("Toyota");
    // ...
}

[Fact]
public async Task Test2()
{
    var vehicle = await CreateVehicle("Honda"); // Independiente
    // ...
}
```

? **Malo:** Tests dependen de datos de otros tests.

```csharp
[Fact]
public async Task Test1()
{
    var vehicle = await CreateVehicle("Toyota");
    _sharedVehicleId = vehicle.Id; // ? Compartir estado
}

[Fact]
public async Task Test2()
{
    var vehicle = await repository.GetByIdAsync(_sharedVehicleId); // ? Depende de Test1
    // ...
}
```

---

### 7. Assertions Específicas

? **Bueno:**
```csharp
outputPort.ErrorMessage.Should().Contain("already has an active rental");
vehicle.KilometersDriven.Should().Be(25000);
customers.Should().HaveCount(5);
```

? **Malo:**
```csharp
outputPort.ErrorMessage.Should().NotBeNullOrEmpty(); // Muy genérico
vehicle.KilometersDriven.Should().BeGreaterThan(0); // Poco específico
customers.Should().NotBeEmpty(); // No verifica cantidad exacta
```

---

### 8. Documentar Tests Complejos

```csharp
/// <summary>
/// Verifies the complete rental flow: create vehicle, create customer, rent vehicle.
/// 
/// This test validates:
/// 1. Vehicle creation with valid data
/// 2. Customer creation with valid data
/// 3. Rental creation linking vehicle and customer
/// 4. Vehicle status changes to Rented
/// 5. Customer HasActiveRental flag is set to true
/// </summary>
[Fact]
public async Task CompleteRentalFlowShouldSucceed()
{
    // ...
}
```

---

## ?? Resumen de Tests

| Categoría | Tests | Cobertura |
|-----------|-------|-----------|
| **Vehicles** | 4 | Crear, Validar edad, Filtrar por estado, Query vacío |
| **Customers** | 4 | Crear, Email duplicado, Obtener todos, Query vacío |
| **Rentals** | 5 | Alquilar, Devolver, Buscar por matrícula, Obtener todos, Validar regla de negocio |
| **TOTAL** | **13** | **Flujos E2E completos** |

---

## ?? Checklist de Ejecución

Antes de ejecutar los tests, verificar:

- [ ] MongoDB está corriendo: `docker ps | grep mongodb-functional`
- [ ] Proyecto compila sin errores: `dotnet build`
- [ ] Configuración correcta en `appsettings.json`
- [ ] No hay otros tests ejecutándose en paralelo
- [ ] Puerto 27017 está disponible

Para ejecutar:

- [ ] Abrir Test Explorer en Visual Studio
- [ ] Click derecho en `GtMotive.Estimate.Microservice.FunctionalTests`
- [ ] Seleccionar `Run`
- [ ] Esperar ~5-10 segundos
- [ ] Verificar que todos los tests están en ? verde

---

## ?? Recursos Adicionales

### Documentación del Proyecto

- `README.md` - Documentación técnica completa
- `FIXES_APPLIED.md` - Registro de correcciones aplicadas
- `CORRECTIONS_REFERENCE.md` - Referencia de errores corregidos
- `FINAL_STATUS.md` - Estado final de implementación

### Arquitectura del Proyecto

- **Clean Architecture** con separación de capas
- **CQRS** (Command Query Responsibility Segregation)
- **Port-Adapter Pattern** para UseCases
- **Repository Pattern** para persistencia
- **Dependency Injection** con Microsoft.Extensions.DependencyInjection

### Tecnologías Utilizadas

| Tecnología | Propósito |
|-----------|-----------|
| **xUnit** | Framework de testing |
| **FluentAssertions** | Assertions legibles |
| **MongoDB.Driver** | Cliente de MongoDB |
| **Microsoft.Extensions.DependencyInjection** | Inyección de dependencias |
| **Microsoft.AspNetCore.Mvc.Testing** | Testing de API (infraestructura) |

---

## ?? Conceptos Clave

### ¿Qué es un Functional Test?

Un test que valida un **flujo completo** de la aplicación, usando componentes reales.

### ¿Cuándo usar Functional Tests?

- Para validar que **múltiples componentes funcionan juntos**
- Para detectar problemas de **integración entre capas**
- Para verificar **flujos de negocio críticos**

### ¿Cuál es la diferencia con Integration Tests?

En este proyecto, **Functional Tests** e **Integration Tests** son similares, pero:

- **Infrastructure Tests**: Prueban solo capa de persistencia (Repositories + BD)
- **Functional Tests**: Prueban flujo completo (UseCases + Repositories + BD)

### ¿Por qué usar MongoDB real?

- Para detectar problemas de persistencia
- Para validar queries complejas
- Para probar con datos reales (no mocks)

---

## ? Conclusión

Los **Functional Tests** son una parte esencial de la estrategia de testing porque:

1. ? Validan **flujos E2E completos**
2. ? Detectan problemas de **integración**
3. ? Proporcionan **confianza** en el sistema
4. ? Sirven como **documentación** de los flujos de negocio

**Estado Actual:**
- ? 13 tests implementados
- ? Cobertura de Vehicles, Customers y Rentals
- ? Compilación exitosa sin errores
- ? Listo para ejecutar

---

## ?? Próximos Pasos

1. **Ejecutar los tests** siguiendo esta guía
2. **Añadir más tests** para casos edge
3. **Medir cobertura** de código con `dotnet test --collect:"XPlat Code Coverage"`
4. **Integrar en CI/CD** (GitHub Actions, Azure DevOps)
5. **Automatizar ejecución** en cada PR

---

*Guía creada: 2024-02-01*  
*Proyecto: GtMotive Rental Microservice*  
*Tests Funcionales: 13 tests implementados*  
*Estado: ? Listo para producción*

---

## ?? Soporte

Si encuentras problemas:

1. **Revisar Troubleshooting** en esta guía
2. **Verificar logs** de los tests
3. **Verificar logs** de MongoDB: `docker logs mongodb-functional`
4. **Consultar documentación** adicional en archivos README

---

**¡Feliz Testing! ??**
