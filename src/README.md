# GtMotive Estimate Microservice - Vehicle Rental System

## ?? Índice

1. [Descripción General](#descripción-general)
2. [Arquitectura](#arquitectura)
3. [Tecnologías Utilizadas](#tecnologías-utilizadas)
4. [Estructura del Proyecto](#estructura-del-proyecto)
5. [Modelo de Dominio](#modelo-de-dominio)
6. [Casos de Uso](#casos-de-uso)
7. [API Endpoints](#api-endpoints)
8. [Base de Datos](#base-de-datos)
9. [Configuración y Despliegue](#configuración-y-despliegue)
10. [Testing](#testing)
11. [Guía de Desarrollo](#guía-de-desarrollo)

---

## ?? Descripción General

Sistema de gestión de alquiler de vehículos construido como microservicio siguiendo los principios de Clean Architecture y Domain-Driven Design (DDD). El sistema permite gestionar vehículos, clientes y alquileres con validaciones de negocio robustas.

### Características Principales

- ? **Gestión de Vehículos**: Registro y consulta de vehículos con validación de antigüedad (máximo 5 años)
- ? **Gestión de Clientes**: Registro de clientes con validación de datos únicos
- ? **Gestión de Alquileres**: Proceso completo de alquiler y devolución de vehículos
- ? **Validaciones de Negocio**: Reglas de dominio aplicadas consistentemente
- ? **API RESTful**: Endpoints bien documentados con Swagger
- ? **Persistencia MongoDB**: Base de datos NoSQL escalable
- ? **Contenedorización**: Docker y Docker Compose para despliegue

---

## ??? Arquitectura

El proyecto sigue **Clean Architecture** (también conocida como Onion Architecture), separando las responsabilidades en capas concéntricas:

```
???????????????????????????????????????????????????
?              Presentation Layer                 ?
?        (API Controllers, Presenters)            ?
???????????????????????????????????????????????????
?            Application Layer                    ?
?           (Use Cases, DTOs)                     ?
???????????????????????????????????????????????????
?              Domain Layer                       ?
?        (Entities, Business Rules)               ?
???????????????????????????????????????????????????
?           Infrastructure Layer                  ?
?      (Repositories, MongoDB, External)          ?
???????????????????????????????????????????????????
```

### Principios Aplicados

- **SOLID Principles**: Especialmente Dependency Inversion y Single Responsibility
- **Clean Architecture**: Independencia de frameworks y bases de datos
- **Domain-Driven Design**: Lógica de negocio en el dominio
- **CQRS Pattern**: Separación implícita entre comandos y consultas
- **Repository Pattern**: Abstracción de la capa de datos

---

## ?? Tecnologías Utilizadas

### Framework y Lenguaje
- **.NET 9.0**: Framework principal
- **C# 13.0**: Lenguaje de programación

### Base de Datos
- **MongoDB 2.19.0**: Base de datos NoSQL
- **MongoDB.Driver**: Driver oficial para .NET

### Testing
- **xUnit 2.9.2**: Framework de testing
- **Moq 4.18.1**: Librería de mocking
- **Bogus 35.6.5**: Generación de datos de prueba
- **FluentAssertions 7.0.0**: Assertions fluidas
- **coverlet.collector**: Cobertura de código

### Documentación
- **Swashbuckle.AspNetCore 6.4.0**: Generación de documentación Swagger/OpenAPI

### Containerización
- **Docker**: Contenedorización de la aplicación
- **Docker Compose**: Orquestación de contenedores

### Otras Librerías
- **MediatR 10.0.1**: Patrón mediator para CQRS
- **Polly 7.2.3**: Resiliencia y manejo de reintentos
- **Serilog**: Logging estructurado
- **Microsoft.ApplicationInsights**: Telemetría

---

## ?? Estructura del Proyecto

```
GtMotive.Estimate.Microservice/
?
??? src/
?   ??? GtMotive.Estimate.Microservice.Domain/
?   ?   ??? Entities/
?   ?       ??? Customer.cs
?   ?       ??? Vehicle.cs
?   ?       ??? Rental.cs
?   ?       ??? DomainException.cs
?   ?
?   ??? GtMotive.Estimate.Microservice.ApplicationCore/
?   ?   ??? UseCases/
?   ?   ?   ??? Customers/
?   ?   ?   ?   ??? CreateCustomer/
?   ?   ?   ?   ??? GetAllCustomers/
?   ?   ?   ??? Vehicles/
?   ?   ?   ?   ??? CreateVehicle/
?   ?   ?   ?   ??? GetVehiclesByStatus/
?   ?   ?   ??? Rentals/
?   ?   ?       ??? RentVehicle/
?   ?   ?       ??? ReturnVehicle/
?   ?   ?       ??? GetAllRentals/
?   ?   ?       ??? GetRentalByLicensePlate/
?   ?   ??? Repositories/
?   ?   ?   ??? ICustomerRepository.cs
?   ?   ?   ??? IVehicleRepository.cs
?   ?   ?   ??? IRentalRepository.cs
?   ?   ??? ApplicationConfiguration.cs
?   ?
?   ??? GtMotive.Estimate.Microservice.Infrastructure/
?   ?   ??? Repositories/
?   ?   ?   ??? CustomerRepository.cs
?   ?   ?   ??? VehicleRepository.cs
?   ?   ?   ??? RentalRepository.cs
?   ?   ??? MongoDb/
?   ?   ?   ??? MongoService.cs
?   ?   ??? InfrastructureConfiguration.cs
?   ?
?   ??? GtMotive.Estimate.Microservice.Api/
?   ?   ??? Controllers/
?   ?   ?   ??? CustomersController.cs
?   ?   ?   ??? VehiclesController.cs
?   ?   ?   ??? RentalsController.cs
?   ?   ??? UseCases/
?   ?   ?   ??? Customers/
?   ?   ?   ??? Vehicles/
?   ?   ?   ??? Rentals/
?   ?   ??? ApiConfiguration.cs
?   ?
?   ??? GtMotive.Estimate.Microservice.Host/
?       ??? Program.cs
?       ??? Dockerfile
?       ??? appsettings.json
?       ??? appsettings.Development.json
?
??? test/
?   ??? unit/
?   ?   ??? GtMotive.Estimate.Microservice.UnitTests/
?   ?       ??? ApplicationCore/
?   ?       ?   ??? Fakers/
?   ?       ?   ?   ??? EntityFakers.cs
?   ?       ?   ??? UseCases/
?   ?       ?       ??? Customers/
?   ?       ?       ??? Vehicles/
?   ?       ?       ??? Rentals/
?   ?       ??? README.md
?   ?
?   ??? infrastructure/
?   ?   ??? GtMotive.Estimate.Microservice.InfrastructureTests/
?   ?
?   ??? functional/
?       ??? GtMotive.Estimate.Microservice.FunctionalTests/
?
??? docker-compose.yml
??? docker-compose.override.yml
??? Directory.Build.targets
```

---

## ?? Modelo de Dominio

### Entidades Principales

#### Customer (Cliente)

Representa un cliente del sistema de alquiler.

**Propiedades:**
- `Id`: Identificador único
- `Name`: Nombre completo
- `Email`: Correo electrónico (único)
- `PhoneNumber`: Número de teléfono
- `DriverLicenseNumber`: Número de licencia de conducir (único)
- `HasActiveRental`: Indica si tiene un alquiler activo
- `CreatedAt`: Fecha de registro
- `UpdatedAt`: Fecha de última actualización

**Reglas de Negocio:**
- Un cliente no puede tener más de un alquiler activo simultáneamente
- El email debe ser único en el sistema
- El número de licencia de conducir debe ser único

**Métodos de Dominio:**
```csharp
bool CanRentVehicle()           // Verifica si puede alquilar
void MarkAsRenting()            // Marca como alquilando
void MarkAsNotRenting()         // Marca como sin alquiler activo
```

---

#### Vehicle (Vehículo)

Representa un vehículo de la flota.

**Propiedades:**
- `Id`: Identificador único
- `Brand`: Marca del vehículo
- `Model`: Modelo del vehículo
- `Year`: Año de fabricación
- `KilometersDriven`: Kilómetros recorridos
- `LicensePlate`: Matrícula (único)
- `Status`: Estado (Available, Rented, Retired)
- `CreatedAt`: Fecha de registro
- `UpdatedAt`: Fecha de última actualización

**Estados Posibles (VehicleStatus):**
- `Available`: Disponible para alquilar
- `Rented`: Actualmente alquilado
- `Retired`: Retirado de la flota

**Reglas de Negocio:**
- Solo se aceptan vehículos con máximo 5 años de antigüedad
- Los vehículos que excedan los 5 años se retiran automáticamente al devolverse
- No se puede alquilar un vehículo que no esté disponible
- El odómetro no puede retroceder

**Métodos de Dominio:**
```csharp
bool IsEligibleForFleet()       // Verifica si cumple edad máxima
bool IsAvailable()              // Verifica disponibilidad
void MarkAsRented()             // Marca como alquilado
void MarkAsAvailable()          // Marca como disponible
void MarkAsRetired()            // Marca como retirado
void AddKilometers(int km)      // Agrega kilómetros
void SetKilometers(int km)      // Establece lectura de odómetro
```

---

#### Rental (Alquiler)

Representa una transacción de alquiler de vehículo.

**Propiedades:**
- `Id`: Identificador único
- `VehicleId`: ID del vehículo alquilado
- `CustomerId`: ID del cliente
- `RentalDate`: Fecha de inicio del alquiler
- `ReturnDate`: Fecha de devolución (null si activo)
- `ExpectedReturnDate`: Fecha esperada de devolución
- `Status`: Estado (Active, Completed, Cancelled)
- `Notes`: Notas adicionales
- `CreatedAt`: Fecha de registro
- `UpdatedAt`: Fecha de última actualización

**Estados Posibles (RentalStatus):**
- `Active`: Alquiler activo
- `Completed`: Alquiler completado
- `Cancelled`: Alquiler cancelado

**Propiedades Calculadas:**
- `RentalDurationInDays`: Duración en días (si está completado)

**Reglas de Negocio:**
- Un alquiler solo puede completarse si está activo
- Un alquiler completado no puede cancelarse
- La fecha de devolución esperada debe ser futura

**Métodos de Dominio:**
```csharp
bool IsActive()                 // Verifica si está activo
bool IsOverdue()                // Verifica si está vencido
void CompleteRental()           // Completa el alquiler
void CancelRental()             // Cancela el alquiler
void AddNote(string note)       // Agrega una nota
```

---

## ?? Casos de Uso

### Gestión de Clientes

#### 1. CreateCustomer (Crear Cliente)

**Input:**
```csharp
{
  "name": "Juan Pérez",
  "email": "juan.perez@example.com",
  "phoneNumber": "+34 600 123 456",
  "driverLicenseNumber": "12345678A"
}
```

**Validaciones:**
- El email no debe existir
- El número de licencia no debe existir
- Todos los campos son obligatorios

**Output:**
```csharp
{
  "id": "guid",
  "name": "Juan Pérez",
  "email": "juan.perez@example.com",
  "phoneNumber": "+34 600 123 456",
  "driverLicenseNumber": "12345678A",
  "hasActiveRental": false,
  "createdAt": "2024-01-15T10:30:00Z"
}
```

---

#### 2. GetAllCustomers (Obtener Todos los Clientes)

**Input:**
```csharp
{
  "hasActiveRental": null  // Opcional: true, false, o null
}
```

**Output:**
```csharp
{
  "customers": [
    {
      "id": "guid",
      "name": "Juan Pérez",
      "email": "juan.perez@example.com",
      "phoneNumber": "+34 600 123 456",
      "driverLicenseNumber": "12345678A",
      "hasActiveRental": false,
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ],
  "totalCount": 1
}
```

---

### Gestión de Vehículos

#### 3. CreateVehicle (Crear Vehículo)

**Input:**
```csharp
{
  "brand": "Toyota",
  "model": "Corolla",
  "year": 2023,
  "licensePlate": "1234-ABC",
  "kilometersDriven": 5000
}
```

**Validaciones:**
- El año no debe exceder 5 años de antigüedad
- La matrícula debe ser única
- Los kilómetros no pueden ser negativos

**Output:**
```csharp
{
  "id": "guid",
  "brand": "Toyota",
  "model": "Corolla",
  "year": 2023,
  "licensePlate": "1234-ABC",
  "kilometersDriven": 5000,
  "status": "Available",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

---

#### 4. GetVehiclesByStatus (Obtener Vehículos por Estado)

**Input:**
```csharp
{
  "status": 0  // 0=Available, 1=Rented, 2=Retired
}
```

**Output:**
```csharp
{
  "vehicles": [
    {
      "id": "guid",
      "brand": "Toyota",
      "model": "Corolla",
      "year": 2023,
      "licensePlate": "1234-ABC",
      "kilometersDriven": 5000,
      "status": "Available",
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ],
  "totalCount": 1,
  "filterApplied": "Available"
}
```

---

### Gestión de Alquileres

#### 5. RentVehicle (Alquilar Vehículo)

**Input:**
```csharp
{
  "vehicleId": "guid",
  "customerId": "guid",
  "expectedReturnDate": "2024-01-22T10:30:00Z",
  "notes": "Cliente VIP"
}
```

**Validaciones:**
- El vehículo debe existir y estar disponible
- El vehículo debe ser elegible (menos de 5 años)
- El cliente debe existir
- El cliente no debe tener otro alquiler activo
- La fecha de devolución debe ser futura

**Proceso:**
1. Marca el vehículo como alquilado
2. Marca al cliente como con alquiler activo
3. Crea el registro de alquiler

**Output:**
```csharp
{
  "rentalId": "guid",
  "vehicleId": "guid",
  "vehicleBrand": "Toyota",
  "vehicleModel": "Corolla",
  "vehicleLicensePlate": "1234-ABC",
  "customerId": "guid",
  "customerName": "Juan Pérez",
  "rentalDate": "2024-01-15T10:30:00Z",
  "expectedReturnDate": "2024-01-22T10:30:00Z",
  "status": "Active",
  "notes": "Cliente VIP"
}
```

---

#### 6. ReturnVehicle (Devolver Vehículo)

**Input:**
```csharp
{
  "rentalId": "guid",
  "currentKilometers": 5500,
  "notes": "Todo correcto"
}
```

**Validaciones:**
- El alquiler debe existir y estar activo
- Los kilómetros no pueden ser menores al odómetro actual
- El vehículo debe existir
- El cliente debe existir

**Proceso:**
1. Completa el alquiler
2. Actualiza los kilómetros del vehículo
3. Marca al cliente como sin alquiler activo
4. Si el vehículo excede 5 años, lo retira; si no, lo marca como disponible

**Output:**
```csharp
{
  "rentalId": "guid",
  "vehicleId": "guid",
  "vehicleBrand": "Toyota",
  "vehicleModel": "Corolla",
  "vehicleLicensePlate": "1234-ABC",
  "customerId": "guid",
  "customerName": "Juan Pérez",
  "rentalDate": "2024-01-15T10:30:00Z",
  "returnDate": "2024-01-20T14:30:00Z",
  "expectedReturnDate": "2024-01-22T10:30:00Z",
  "status": "Completed",
  "kilometersDriven": 500,
  "vehicleStatus": "Available",
  "notes": "Todo correcto"
}
```

---

#### 7. GetAllRentals (Obtener Todos los Alquileres)

**Input:**
```csharp
{
  "status": null  // Opcional: 0=Active, 1=Completed, 2=Cancelled
}
```

**Output:**
```csharp
{
  "rentals": [
    {
      "rentalId": "guid",
      "vehicleId": "guid",
      "rentalDate": "2024-01-15T10:30:00Z",
      "returnDate": null,
      "expectedReturnDate": "2024-01-22T10:30:00Z",
      "status": "Active",
      "isOverdue": false
    }
  ],
  "totalCount": 1
}
```

---

#### 8. GetRentalByLicensePlate (Obtener Alquiler por Matrícula)

**Input:**
```csharp
{
  "licensePlate": "1234-ABC"
}
```

**Validaciones:**
- La matrícula no puede estar vacía
- El vehículo debe existir

**Output:**
```csharp
{
  "rentalId": "guid",
  "vehicleId": "guid",
  "vehicleBrand": "Toyota",
  "vehicleModel": "Corolla",
  "vehicleLicensePlate": "1234-ABC",
  "rentalDate": "2024-01-15T10:30:00Z",
  "returnDate": null,
  "expectedReturnDate": "2024-01-22T10:30:00Z",
  "status": "Active",
  "notes": "Cliente VIP"
}
```

---

## ?? API Endpoints

### Base URL
```
http://localhost:5000/api
https://localhost:5001/api
```

### Customers (Clientes)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/customers` | Crear nuevo cliente |
| GET | `/customers` | Obtener todos los clientes |
| GET | `/customers?hasActiveRental=true` | Filtrar clientes con alquiler activo |

### Vehicles (Vehículos)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/vehicles` | Crear nuevo vehículo |
| GET | `/vehicles?status=0` | Obtener vehículos por estado |

### Rentals (Alquileres)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/rentals/rent` | Alquilar un vehículo |
| POST | `/rentals/return` | Devolver un vehículo |
| GET | `/rentals` | Obtener todos los alquileres |
| GET | `/rentals?status=0` | Filtrar alquileres por estado |
| GET | `/rentals/by-license-plate/{licensePlate}` | Obtener alquiler activo por matrícula |

### Swagger UI

La documentación interactiva está disponible en:
- Desarrollo: `http://localhost:5000/swagger`
- Producción: `https://your-domain/swagger`

---

## ??? Base de Datos

### MongoDB

El sistema utiliza MongoDB como base de datos principal.

**Colecciones:**

1. **Customers**
   - Índices: `Email` (único), `DriverLicenseNumber` (único)

2. **Vehicles**
   - Índices: `LicensePlate` (único), `Status`

3. **Rentals**
   - Índices: `VehicleId`, `CustomerId`, `Status`

**Configuración de Conexión:**

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "GtMotiveRental"
  }
}
```

---

## ?? Configuración y Despliegue

### Requisitos Previos

- .NET 9.0 SDK
- Docker y Docker Compose
- MongoDB (si no se usa Docker)

### Configuración Local

1. **Clonar el repositorio:**
```bash
git clone https://github.com/AguileraMG/PruebaTecnicaRental.git
cd PruebaTecnicaRental/src
```

2. **Configurar appsettings:**

Edita `appsettings.Development.json`:
```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "GtMotiveRental"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

3. **Restaurar paquetes:**
```bash
dotnet restore
```

4. **Compilar:**
```bash
dotnet build
```

5. **Ejecutar:**
```bash
dotnet run --project GtMotive.Estimate.Microservice.Host
```

### Despliegue con Docker

1. **Construir imagen:**
```bash
docker build -t gtmotive-rental-api .
```

2. **Ejecutar con Docker Compose:**
```bash
docker-compose up -d
```

El archivo `docker-compose.yml` incluye:
- API en puerto 5000 (HTTP) y 5001 (HTTPS)
- MongoDB en puerto 27017

3. **Verificar servicios:**
```bash
docker-compose ps
```

4. **Ver logs:**
```bash
docker-compose logs -f
```

5. **Detener servicios:**
```bash
docker-compose down
```

### Variables de Entorno

| Variable | Descripción | Valor por Defecto |
|----------|-------------|-------------------|
| `ASPNETCORE_ENVIRONMENT` | Entorno de ejecución | Development |
| `MongoDb__ConnectionString` | Cadena de conexión MongoDB | mongodb://localhost:27017 |
| `MongoDb__DatabaseName` | Nombre de la base de datos | GtMotiveRental |

---

## ?? Testing

### Estructura de Tests

El proyecto incluye tres tipos de tests:

1. **Unit Tests** - Tests de lógica de negocio
2. **Infrastructure Tests** - Tests de infraestructura
3. **Functional Tests** - Tests end-to-end

### Tests Unitarios

Ubicación: `test/unit/GtMotive.Estimate.Microservice.UnitTests/`

**Cobertura:**
- ? 8 UseCases con 18 tests
- ? Generación de datos con Bogus
- ? Mocking con Moq
- ? Assertions con FluentAssertions

**Ejecutar tests:**
```bash
# Todos los tests
dotnet test

# Tests unitarios específicos
dotnet test test/unit/GtMotive.Estimate.Microservice.UnitTests/

# Con cobertura de código
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Tests específicos
dotnet test --filter "FullyQualifiedName~CreateCustomerUseCaseTests"
```

### Tests por Categoría

#### Customers (4 tests)
```bash
dotnet test --filter "FullyQualifiedName~Customers"
```

#### Vehicles (3 tests)
```bash
dotnet test --filter "FullyQualifiedName~Vehicles"
```

#### Rentals (11 tests)
```bash
dotnet test --filter "FullyQualifiedName~Rentals"
```

### Reportes de Cobertura

```bash
# Generar reporte HTML
dotnet test /p:CollectCoverage=true /p:CoverletOutput=TestResults/ /p:CoverletOutputFormat=opencover
reportgenerator -reports:TestResults/coverage.opencover.xml -targetdir:TestResults/html
```

---

## ????? Guía de Desarrollo

### Agregar un Nuevo Caso de Uso

1. **Crear Input/Output en ApplicationCore:**
```csharp
// UseCases/NewFeature/NewFeatureInput.cs
public sealed class NewFeatureInput : IUseCaseInput
{
    public string Property { get; set; }
}

// UseCases/NewFeature/NewFeatureOutput.cs
public sealed class NewFeatureOutput : IUseCaseOutput
{
    public string Result { get; set; }
}
```

2. **Crear Output Port:**
```csharp
// UseCases/NewFeature/INewFeatureOutputPort.cs
public interface INewFeatureOutputPort : IOutputPortStandard<NewFeatureOutput>, IOutputPortNotFound
{
}
```

3. **Implementar UseCase:**
```csharp
// UseCases/NewFeature/NewFeatureUseCase.cs
public sealed class NewFeatureUseCase : IUseCase<NewFeatureInput>
{
    private readonly INewFeatureOutputPort _outputPort;
    private readonly IRepository _repository;

    public NewFeatureUseCase(INewFeatureOutputPort outputPort, IRepository repository)
    {
        _outputPort = outputPort;
        _repository = repository;
    }

    public async Task ExecuteAsync(NewFeatureInput input, CancellationToken ct)
    {
        // Lógica del caso de uso
        var output = new NewFeatureOutput { Result = "Success" };
        _outputPort.StandardHandle(output);
    }
}
```

4. **Registrar en ApplicationConfiguration:**
```csharp
services.AddScoped<IUseCase<NewFeatureInput>, NewFeatureUseCase>();
```

5. **Crear Presenter en API:**
```csharp
// Api/UseCases/NewFeaturePresenter.cs
public sealed class NewFeaturePresenter : INewFeatureOutputPort, IWebApiPresenter
{
    public IActionResult ViewModel { get; private set; }

    public void StandardHandle(NewFeatureOutput output)
    {
        ViewModel = new OkObjectResult(output);
    }

    public void NotFoundHandle(string message)
    {
        ViewModel = new NotFoundObjectResult(message);
    }
}
```

6. **Crear Controller Endpoint:**
```csharp
[HttpPost("new-feature")]
public async Task<IActionResult> NewFeature(
    [FromBody] NewFeatureInput input,
    [FromServices] IUseCase<NewFeatureInput> useCase,
    [FromServices] NewFeaturePresenter presenter)
{
    await useCase.ExecuteAsync(input, HttpContext.RequestAborted);
    return presenter.ViewModel;
}
```

7. **Crear Tests Unitarios:**
```csharp
public class NewFeatureUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenValid_ShouldSucceed()
    {
        // Arrange
        var repositoryMock = new Mock<IRepository>();
        var outputPortMock = new Mock<INewFeatureOutputPort>();
        var sut = new NewFeatureUseCase(outputPortMock.Object, repositoryMock.Object);
        var input = new NewFeatureInput { Property = "test" };

        // Act
        await sut.ExecuteAsync(input, CancellationToken.None);

        // Assert
        outputPortMock.Verify(x => x.StandardHandle(It.IsAny<NewFeatureOutput>()), Times.Once);
    }
}
```

### Estándares de Código

#### Naming Conventions
- **Classes**: PascalCase (e.g., `CustomerRepository`)
- **Methods**: PascalCase (e.g., `GetAllAsync`)
- **Parameters**: camelCase (e.g., `customerId`)
- **Private fields**: _camelCase (e.g., `_repository`)

#### Async/Await
- Todos los métodos asíncronos deben terminar en `Async`
- Siempre usar `CancellationToken`
- Evitar `async void` (excepto event handlers)

#### Dependency Injection
- Usar constructor injection
- Validar parámetros con `ArgumentNullException.ThrowIfNull()`
- Registrar dependencias con el tiempo de vida apropiado (Scoped para UseCase)

#### Error Handling
- Usar `DomainException` para errores de negocio
- Usar Output Ports para comunicar errores (NotFoundHandle, ErrorHandle)
- No capturar excepciones genéricas

### Flujo de Trabajo Git

```bash
# Crear rama para nueva feature
git checkout -b feature/nueva-funcionalidad

# Hacer commits atómicos
git add .
git commit -m "feat: agregar nuevo caso de uso para..."

# Push a remoto
git push origin feature/nueva-funcionalidad

# Crear Pull Request en GitHub
```

### Convenciones de Commits

Seguir [Conventional Commits](https://www.conventionalcommits.org/):

- `feat:` Nueva funcionalidad
- `fix:` Corrección de bug
- `docs:` Cambios en documentación
- `test:` Agregar o modificar tests
- `refactor:` Refactorización de código
- `style:` Cambios de formato
- `chore:` Tareas de mantenimiento

---

## ?? Diagramas

### Flujo de Alquiler de Vehículo

```
???????????
? Cliente ?
???????????
     ?
     ? 1. POST /rentals/rent
     ?
??????????????????????
? RentalsController  ?
??????????????????????
         ?
         ? 2. ExecuteAsync(input)
         ?
????????????????????????
? RentVehicleUseCase   ?
????????????????????????
       ?
       ? 3. Validar vehículo
       ?
????????????????????????      ???????????????????
? VehicleRepository    ????????    MongoDB      ?
????????????????????????      ???????????????????
       ?
       ? 4. Validar cliente
       ?
????????????????????????
? CustomerRepository   ?
????????????????????????
       ?
       ? 5. Crear alquiler
       ?
????????????????????????
? RentalRepository     ?
????????????????????????
       ?
       ? 6. Actualizar estados
       ?
????????????????????????
? RentVehiclePresenter ?
????????????????????????
       ?
       ? 7. Response
       ?
???????????
? Cliente ?
???????????
```

### Arquitectura de Capas

```
?????????????????????????????????????????????????????????????
?                    Presentation Layer                      ?
?  ????????????????  ????????????????  ????????????????   ?
?  ? Controllers  ?  ?  Presenters  ?  ?   Filters    ?   ?
?  ????????????????  ????????????????  ????????????????   ?
?????????????????????????????????????????????????????????????
                            ?
                            ?
?????????????????????????????????????????????????????????????
?                   Application Layer                        ?
?  ????????????????  ????????????????  ????????????????   ?
?  ?  Use Cases   ?  ?    DTOs      ?  ? Output Ports ?   ?
?  ????????????????  ????????????????  ????????????????   ?
?????????????????????????????????????????????????????????????
                            ?
                            ?
?????????????????????????????????????????????????????????????
?                      Domain Layer                          ?
?  ????????????????  ????????????????  ????????????????   ?
?  ?   Entities   ?  ?    Rules     ?  ?  Exceptions  ?   ?
?  ????????????????  ????????????????  ????????????????   ?
?????????????????????????????????????????????????????????????
                            ?
                            ?
?????????????????????????????????????????????????????????????
?                  Infrastructure Layer                      ?
?  ????????????????  ????????????????  ????????????????   ?
?  ? Repositories ?  ?   MongoDB    ?  ?   External   ?   ?
?  ????????????????  ????????????????  ????????????????   ?
?????????????????????????????????????????????????????????????
```

---

## ?? Licencia

Este proyecto es una prueba técnica y está disponible únicamente para fines educativos y de evaluación.

---

## ?? Autor

**Manuel García**
- GitHub: [@AguileraMG](https://github.com/AguileraMG)
- Email: mgarciaa@example.com

---

## ?? Contribuciones

Este es un proyecto de prueba técnica. Para cualquier consulta o sugerencia, por favor abrir un issue en el repositorio.

---

## ?? Referencias

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [.NET 9 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [MongoDB .NET Driver](https://mongodb.github.io/mongo-csharp-driver/)
- [xUnit Documentation](https://xunit.net/)

---

## ?? Changelog

### Version 1.0.0 (2024-01-15)
- ? Implementación inicial del sistema
- ? CRUD completo de clientes, vehículos y alquileres
- ? Validaciones de negocio
- ? Tests unitarios (18 tests)
- ? Documentación Swagger
- ? Containerización con Docker
- ? Integración con MongoDB

---

**¡Gracias por revisar este proyecto!** ??
