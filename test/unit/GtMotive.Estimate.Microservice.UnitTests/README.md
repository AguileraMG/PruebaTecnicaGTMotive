# Tests Unitarios - GtMotive Estimate Microservice

## ?? Resumen

Este proyecto contiene los tests unitarios para la capa de Application (UseCases) del microservicio de alquiler de vehículos. Los tests están diseñados siguiendo las mejores prácticas de testing en .NET, utilizando el patrón AAA (Arrange-Act-Assert) y generación de datos realistas con Bogus.

## ?? Objetivo

Garantizar la calidad y correcto funcionamiento de la lógica de negocio implementada en los casos de uso, validando:

- ? Flujos principales (happy paths)
- ? Manejo de errores y validaciones
- ? Interacciones con repositorios
- ? Salidas correctas a través de output ports
- ? Independencia de infraestructura (usando mocks)

## ?? Cobertura de Tests

### Resumen General

| Categoría | Casos de Uso | Tests | Estado |
|-----------|--------------|-------|--------|
| Customers | 2 | 4 | ? |
| Vehicles | 2 | 3 | ? |
| Rentals | 4 | 11 | ? |
| **Total** | **8** | **18** | ? |

## ?? Estructura de Tests

```
GtMotive.Estimate.Microservice.UnitTests/
?
??? ApplicationCore/
?   ?
?   ??? Fakers/
?   ?   ??? EntityFakers.cs                    # Generadores de datos con Bogus
?   ?
?   ??? UseCases/
?       ?
?       ??? Customers/
?       ?   ??? CreateCustomerUseCaseTests.cs
?       ?   ??? GetAllCustomersUseCaseTests.cs
?       ?
?       ??? Vehicles/
?       ?   ??? CreateVehicleUseCaseTests.cs
?       ?   ??? GetVehiclesByStatusUseCaseTests.cs
?       ?
?       ??? Rentals/
?           ??? RentVehicleUseCaseTests.cs
?           ??? ReturnVehicleUseCaseTests.cs
?           ??? GetAllRentalsUseCaseTests.cs
?           ??? GetRentalByLicensePlateUseCaseTests.cs
?
??? GtMotive.Estimate.Microservice.UnitTests.csproj
??? README.md
```

## ?? Tecnologías Utilizadas

### Frameworks y Librerías

| Paquete | Versión | Propósito |
|---------|---------|-----------|
| **xUnit** | 2.9.2 | Framework de testing principal |
| **Moq** | 4.18.1 | Mocking de dependencias |
| **Bogus** | 35.6.5 | Generación de datos de prueba realistas |
| **FluentAssertions** | 7.0.0 | Assertions expresivas y legibles |
| **Microsoft.NET.Test.Sdk** | 17.9.0 | SDK de testing de .NET |
| **coverlet.collector** | 3.1.2 | Recolección de cobertura de código |

## ?? Detalle de Tests por Módulo

### 1. Customer Tests (4 tests)

#### CreateCustomerUseCaseTests (2 tests)

**? `ExecuteAsync_WhenCustomerDoesNotExist_ShouldCreateCustomerSuccessfully`**
- **Descripción**: Verifica que se crea un cliente correctamente cuando no existe duplicado
- **Verifica**:
  - Llamada a `GetByEmailAsync` retorna null
  - Llamada a `AddAsync` se ejecuta una vez
  - Output port `StandardHandle` es llamado con el output correcto

**? `ExecuteAsync_WhenCustomerEmailExists_ShouldNotCreateCustomer`**
- **Descripción**: Verifica que no se crea un cliente si el email ya existe
- **Verifica**:
  - Llamada a `GetByEmailAsync` retorna cliente existente
  - `AddAsync` NO se ejecuta
  - Output port `NotFoundHandle` es llamado con mensaje de error

---

#### GetAllCustomersUseCaseTests (2 tests)

**? `ExecuteAsync_WhenCustomersExist_ShouldReturnAllCustomers`**
- **Descripción**: Verifica que retorna todos los clientes cuando existen
- **Verifica**:
  - Llamada a `GetAllAsync` se ejecuta una vez
  - Output contiene el número correcto de clientes (5)
  - Output port `StandardHandle` es llamado

**? `ExecuteAsync_WhenNoCustomersExist_ShouldReturnEmptyList`**
- **Descripción**: Verifica que retorna lista vacía cuando no hay clientes
- **Verifica**:
  - Llamada a `GetAllAsync` retorna lista vacía
  - Output `TotalCount` es 0
  - Output port `StandardHandle` es llamado

---

### 2. Vehicle Tests (3 tests)

#### CreateVehicleUseCaseTests (2 tests)

**? `ExecuteAsync_WhenVehicleIsEligible_ShouldCreateVehicleSuccessfully`**
- **Descripción**: Verifica que se crea un vehículo cuando cumple requisitos de antigüedad
- **Verifica**:
  - Vehículo tiene menos de 5 años
  - `AddAsync` se ejecuta una vez
  - Output port `StandardHandle` es llamado

**? `ExecuteAsync_WhenVehicleIsTooOld_ShouldNotCreateVehicle`**
- **Descripción**: Verifica que no se crea un vehículo si excede 5 años de antigüedad
- **Verifica**:
  - Vehículo tiene más de 5 años
  - `AddAsync` NO se ejecuta
  - Output port `NotFoundHandle` es llamado con mensaje de error

---

#### GetVehiclesByStatusUseCaseTests (2 tests)

**? `ExecuteAsync_WhenVehiclesExistWithStatus_ShouldReturnFilteredVehicles` (Theory)**
- **Descripción**: Verifica que retorna vehículos filtrados por estado
- **Parámetros**: Available, Rented, Retired
- **Verifica**:
  - Llamada a `GetVehiclesByStatusAsync` con estado correcto
  - Output contiene el número correcto de vehículos (3)
  - Output port `StandardHandle` es llamado

**? `ExecuteAsync_WhenNoVehiclesExistWithStatus_ShouldReturnEmptyList`**
- **Descripción**: Verifica que retorna lista vacía cuando no hay vehículos con ese estado
- **Verifica**:
  - Llamada a `GetVehiclesByStatusAsync` retorna lista vacía
  - Output `TotalCount` es 0
  - Output port `StandardHandle` es llamado

---

### 3. Rental Tests (11 tests)

#### RentVehicleUseCaseTests (4 tests)

**? `ExecuteAsync_WhenVehicleIsAvailableAndCustomerCanRent_ShouldCreateRentalSuccessfully`**
- **Descripción**: Verifica el flujo completo de alquiler exitoso
- **Verifica**:
  - Vehículo está disponible y es elegible (< 5 años)
  - Cliente no tiene alquiler activo
  - Se crean y actualizan todos los registros
  - Vehículo se marca como alquilado
  - Cliente se marca con alquiler activo
  - Output port `StandardHandle` es llamado

**? `ExecuteAsync_WhenVehicleNotFound_ShouldNotCreateRental`**
- **Descripción**: Verifica que no se crea alquiler si el vehículo no existe
- **Verifica**:
  - `GetByIdAsync` retorna null
  - `AddAsync` NO se ejecuta
  - Output port `NotFoundHandle` es llamado

**? `ExecuteAsync_WhenCustomerNotFound_ShouldNotCreateRental`**
- **Descripción**: Verifica que no se crea alquiler si el cliente no existe
- **Verifica**:
  - Vehículo existe
  - Cliente `GetByIdAsync` retorna null
  - `AddAsync` NO se ejecuta
  - Output port `NotFoundHandle` es llamado

**? `ExecuteAsync_WhenCustomerHasActiveRental_ShouldNotCreateRental`**
- **Descripción**: Verifica que no se permite alquilar si el cliente ya tiene un alquiler activo
- **Verifica**:
  - Vehículo y cliente existen
  - Cliente tiene `HasActiveRental = true`
  - `AddAsync` NO se ejecuta
  - Output port `NotFoundHandle` es llamado

---

#### ReturnVehicleUseCaseTests (3 tests)

**? `ExecuteAsync_WhenRentalIsActive_ShouldReturnVehicleSuccessfully`**
- **Descripción**: Verifica el flujo completo de devolución exitosa
- **Verifica**:
  - Alquiler está activo
  - Se actualiza el alquiler como completado
  - Se actualizan kilómetros del vehículo
  - Cliente se marca sin alquiler activo
  - Vehículo se marca como disponible (si cumple antigüedad)
  - Output port `StandardHandle` es llamado

**? `ExecuteAsync_WhenRentalNotFound_ShouldNotReturnVehicle`**
- **Descripción**: Verifica que no se procesa devolución si el alquiler no existe
- **Verifica**:
  - `GetByIdAsync` retorna null
  - No se actualizan registros
  - Output port `NotFoundHandle` es llamado

**? `ExecuteAsync_WhenRentalIsNotActive_ShouldNotReturnVehicle`**
- **Descripción**: Verifica que no se procesa devolución si el alquiler no está activo
- **Verifica**:
  - Alquiler tiene estado `Completed`
  - No se actualizan registros
  - Output port `NotFoundHandle` es llamado

---

#### GetAllRentalsUseCaseTests (2 tests)

**? `ExecuteAsync_WhenRentalsExist_ShouldReturnAllRentals`**
- **Descripción**: Verifica que retorna todos los alquileres cuando existen
- **Verifica**:
  - Llamada a `GetAllRentalsAsync` se ejecuta
  - Output contiene el número correcto de alquileres (5)
  - Output port `StandardHandle` es llamado

**? `ExecuteAsync_WhenNoRentalsExist_ShouldReturnEmptyList`**
- **Descripción**: Verifica que retorna lista vacía cuando no hay alquileres
- **Verifica**:
  - Llamada a `GetAllRentalsAsync` retorna lista vacía
  - Output `TotalCount` es 0
  - Output port `StandardHandle` es llamado

---

#### GetRentalByLicensePlateUseCaseTests (3 tests)

**? `ExecuteAsync_WhenVehicleExistsAndHasActiveRental_ShouldReturnRental`**
- **Descripción**: Verifica que retorna el alquiler activo por matrícula
- **Verifica**:
  - Vehículo existe
  - Alquiler activo existe para ese vehículo
  - Output contiene información completa del alquiler
  - Output port `StandardHandle` es llamado

**? `ExecuteAsync_WhenVehicleNotFound_ShouldReturnNotFound`**
- **Descripción**: Verifica que retorna error si el vehículo no existe
- **Verifica**:
  - `GetByLicensePlateAsync` retorna null
  - Output port `NotFoundHandle` es llamado

**? `ExecuteAsync_WhenVehicleHasNoActiveRental_ShouldReturnNotFound`**
- **Descripción**: Verifica que retorna error si el vehículo no tiene alquiler activo
- **Verifica**:
  - Vehículo existe
  - `GetActiveRentalByVehicleIdAsync` retorna null
  - Output port `NotFoundHandle` es llamado

---

## ?? Fakers - Generación de Datos con Bogus

El archivo `EntityFakers.cs` proporciona generadores de datos realistas:

### CustomerFaker
```csharp
- Id: GUID único
- Name: Nombres completos realistas (e.g., "John Smith")
- Email: Emails válidos (e.g., "john.smith@example.com")
- PhoneNumber: Números de teléfono con formato
- DriverLicenseNumber: Alfanumérico de 10 caracteres
- HasActiveRental: Booleano aleatorio
- CreatedAt: Fecha en los últimos 2 años
- UpdatedAt: Fecha reciente
```

### VehicleFaker
```csharp
- Id: GUID único
- Brand: Marcas reales de fabricantes (e.g., "Toyota", "Ford")
- Model: Modelos de vehículos (e.g., "Corolla", "Focus")
- Year: Entre año actual y hace 5 años
- KilometersDriven: Entre 0 y 200,000 km
- LicensePlate: Formato "ABC-1234"
- Status: Estado aleatorio (Available, Rented, Retired)
- CreatedAt: Fecha en el último año
- UpdatedAt: Fecha reciente
```

### RentalFaker
```csharp
- Id: GUID único
- VehicleId: GUID de vehículo
- CustomerId: GUID de cliente
- RentalDate: Fecha en los últimos 30 días
- ExpectedReturnDate: Entre 1 y 30 días después del inicio
- ReturnDate: Null si activo, fecha si completado
- Status: Estado aleatorio (Active, Completed, Cancelled)
- Notes: Frase generada automáticamente
- CreatedAt: Fecha en el último año
- UpdatedAt: Fecha reciente
```

**Ejemplo de Uso:**
```csharp
var customer = EntityFakers.CustomerFaker.Generate();
var customers = EntityFakers.CustomerFaker.Generate(10); // Generar 10 clientes

// Con reglas personalizadas
var availableVehicle = EntityFakers.VehicleFaker
    .RuleFor(v => v.Status, VehicleStatus.Available)
    .RuleFor(v => v.Year, DateTime.UtcNow.Year - 2)
    .Generate();
```

---

## ?? Patrones de Testing Implementados

### 1. AAA Pattern (Arrange-Act-Assert)

Todos los tests siguen esta estructura clara:

```csharp
[Fact]
public async Task ExecuteAsync_WhenCondition_ShouldExpectedBehavior()
{
    // Arrange: Preparar el entorno de prueba
    var input = new InputDto { /* ... */ };
    var mock = new Mock<IRepository>();
    mock.Setup(x => x.Method()).ReturnsAsync(expectedValue);
    var sut = new UseCase(mock.Object, outputPortMock.Object);
    
    // Act: Ejecutar la acción a probar
    await sut.ExecuteAsync(input, CancellationToken.None);
    
    // Assert: Verificar el resultado esperado
    mock.Verify(x => x.Method(), Times.Once);
    outputPortMock.Verify(x => x.Handle(It.IsAny<Output>()), Times.Once);
}
```

### 2. System Under Test (SUT)

Cada clase de test mantiene una instancia clara del objeto siendo probado:

```csharp
private readonly UseCase _sut;

public UseCaseTests()
{
    // Inicialización del SUT con sus dependencias mockeadas
    _sut = new UseCase(mock1.Object, mock2.Object);
}
```

### 3. Mocking con Moq

Simulación de dependencias para tests aislados:

```csharp
// Setup de comportamiento
_repositoryMock
    .Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(expectedEntity);

// Verificación de llamadas
_repositoryMock.Verify(
    x => x.AddAsync(It.IsAny<Entity>(), It.IsAny<CancellationToken>()), 
    Times.Once
);
```

### 4. Test Naming Convention

Formato: `MethodName_StateUnderTest_ExpectedBehavior`

**Ejemplos:**
- `ExecuteAsync_WhenCustomerExists_ShouldReturnCustomer`
- `ExecuteAsync_WhenVehicleNotFound_ShouldReturnNotFound`
- `ExecuteAsync_WhenDataIsValid_ShouldCreateSuccessfully`

### 5. Theory Tests

Para probar múltiples escenarios con los mismos datos:

```csharp
[Theory]
[InlineData(VehicleStatus.Available)]
[InlineData(VehicleStatus.Rented)]
[InlineData(VehicleStatus.Retired)]
public async Task ExecuteAsync_WithDifferentStatuses_ShouldFilter(VehicleStatus status)
{
    // Test se ejecuta 3 veces, una por cada estado
}
```

---

## ?? Cómo Ejecutar los Tests

### Desde Visual Studio

1. **Abrir Test Explorer:**
   - Menú: `Test` ? `Test Explorer`
   - Atajo: `Ctrl + E, T`

2. **Ejecutar todos los tests:**
   - Click en `Run All` en Test Explorer
   - Atajo: `Ctrl + R, A`

3. **Ejecutar tests específicos:**
   - Click derecho en un test o grupo
   - Seleccionar `Run`

4. **Debug de tests:**
   - Click derecho en el test
   - Seleccionar `Debug`

---

### Desde Línea de Comandos

#### Ejecutar Todos los Tests
```bash
dotnet test
```

#### Ejecutar Tests del Proyecto Específico
```bash
dotnet test test/unit/GtMotive.Estimate.Microservice.UnitTests/
```

#### Ejecutar Tests por Categoría
```bash
# Tests de Customers
dotnet test --filter "FullyQualifiedName~Customers"

# Tests de Vehicles
dotnet test --filter "FullyQualifiedName~Vehicles"

# Tests de Rentals
dotnet test --filter "FullyQualifiedName~Rentals"
```

#### Ejecutar Test Específico
```bash
dotnet test --filter "FullyQualifiedName~CreateCustomerUseCaseTests.ExecuteAsync_WhenCustomerDoesNotExist_ShouldCreateCustomerSuccessfully"
```

#### Ejecutar con Output Detallado
```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## ?? Cobertura de Código

### Generar Reporte de Cobertura

#### Formato básico
```bash
dotnet test /p:CollectCoverage=true
```

#### Con formato específico
```bash
# Formato Cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Formato OpenCover
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Múltiples formatos
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=\"opencover,cobertura\"
```

#### Generar Reporte HTML
```bash
# Instalar ReportGenerator (una vez)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Ejecutar tests con cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutput=TestResults/ /p:CoverletOutputFormat=opencover

# Generar reporte HTML
reportgenerator -reports:TestResults/coverage.opencover.xml -targetdir:TestResults/html -reporttypes:Html

# Abrir reporte
start TestResults/html/index.html  # Windows
open TestResults/html/index.html   # macOS
xdg-open TestResults/html/index.html  # Linux
```

### Configurar Umbrales de Cobertura

Agregar en `.csproj`:
```xml
<PropertyGroup>
    <CoverletOutputFormat>cobertura</CoverletOutputFormat>
    <Threshold>80</Threshold>
    <ThresholdType>line,branch,method</ThresholdType>
    <ThresholdStat>total</ThresholdStat>
</PropertyGroup>
```

---

## ?? Debugging de Tests

### En Visual Studio

1. **Establecer breakpoint** en el test
2. Click derecho en el test ? `Debug`
3. Usar F10/F11 para navegar

### Información de Debug

Agregar output para debugging:
```csharp
[Fact]
public async Task MyTest()
{
    var output = _testOutputHelper;
    output.WriteLine($"Testing with input: {input}");
    
    // Test code...
    
    output.WriteLine($"Result: {result}");
}
```

---

## ?? Mejores Prácticas

### ? DO (Hacer)

1. **Tests Independientes**: Cada test debe poder ejecutarse solo
2. **Tests Rápidos**: Los tests unitarios deben ser muy rápidos (< 100ms)
3. **Tests Determinísticos**: Mismo input = mismo output siempre
4. **Un Concepto por Test**: Cada test verifica un solo comportamiento
5. **Nombres Descriptivos**: El nombre debe explicar qué se prueba
6. **Arrange-Act-Assert**: Seguir el patrón AAA consistentemente
7. **Mock Externo**: Mockear todo lo que esté fuera del SUT
8. **Usar Bogus**: Para datos de prueba realistas y variados

### ? DON'T (No Hacer)

1. **Dependencias entre Tests**: Tests que dependen de otros tests
2. **Tests Lentos**: Tests que demoran más de 1 segundo
3. **Tests Frágiles**: Tests que fallan por razones no relacionadas
4. **Lógica Compleja en Tests**: Los tests deben ser simples
5. **Hardcodear Datos**: Usar datos mágicos sin contexto
6. **Múltiples Asserts No Relacionados**: Mezclar conceptos
7. **Testar Implementación**: Testar el contrato, no la implementación
8. **Ignorar Tests que Fallan**: Arreglar o eliminar, no ignorar

---

## ?? Recursos Adicionales

### Documentación Oficial

- [xUnit Documentation](https://xunit.net/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [Bogus Documentation](https://github.com/bchavez/Bogus)
- [FluentAssertions](https://fluentassertions.com/)
- [.NET Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

### Artículos Recomendados

- [AAA Pattern](https://medium.com/@pjbgf/title-testing-code-ocd-and-the-aaa-pattern-df453975ab80)
- [Test Naming Conventions](https://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html)
- [Mocking Best Practices](https://enterprisecraftsmanship.com/posts/when-to-mock/)

---

## ??? Solución de Problemas

### Tests no se descubren en Test Explorer

**Solución:**
1. Limpiar y reconstruir la solución
2. Cerrar y reabrir Visual Studio
3. Verificar que los packages están restaurados

### Tests fallan al ejecutar en batch pero pasan individualmente

**Posible causa:** Estado compartido entre tests

**Solución:**
- Asegurar que cada test limpia su estado
- No usar campos estáticos
- Verificar que los mocks se resetean

### Errores de referencia circular

**Solución:**
- Verificar que el proyecto de tests no referencia cosas innecesarias
- Asegurar que las dependencias son correctas

---

## ?? CI/CD Integration

### GitHub Actions

```yaml
name: Run Unit Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 9.0.x
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal /p:CollectCoverage=true
```

### Azure DevOps

```yaml
- task: DotNetCoreCLI@2
  displayName: 'Run Unit Tests'
  inputs:
    command: 'test'
    projects: '**/*UnitTests.csproj'
    arguments: '--configuration $(buildConfiguration) /p:CollectCoverage=true'
```

---

## ?? Notas Importantes

1. **Los tests NO usan base de datos real**: Todo está mockeado para velocidad y confiabilidad
2. **Los tests son determinísticos**: Aunque Bogus genera datos aleatorios, los mocks controlan el comportamiento
3. **Cobertura no es todo**: 100% cobertura no significa 100% calidad
4. **Tests como documentación**: Los tests son documentación viva del comportamiento esperado
5. **Refactoring seguro**: Con buena cobertura de tests, refactorizar es más seguro

---

## ?? Aprender Más

Para profundizar en testing:

1. **Libro**: "The Art of Unit Testing" by Roy Osherove
2. **Curso**: [Microsoft Testing Best Practices](https://docs.microsoft.com/en-us/learn/paths/test-aspnet-core-mvc-apps/)
3. **Video**: Clean Code - Uncle Bob (Testing)
4. **Blog**: Martin Fowler's Testing Articles

---

## ?? Contribuir

Si encuentras un bug o quieres agregar más tests:

1. Crear un issue describiendo el problema/mejora
2. Hacer fork del repositorio
3. Crear branch para tu feature: `git checkout -b test/nueva-feature`
4. Commit cambios: `git commit -m 'test: agregar tests para...'`
5. Push a branch: `git push origin test/nueva-feature`
6. Crear Pull Request

---

## ? Checklist para Nuevos Tests

Antes de crear un Pull Request con nuevos tests:

- [ ] El test tiene un nombre descriptivo
- [ ] Sigue el patrón AAA (Arrange-Act-Assert)
- [ ] Usa mocks para todas las dependencias externas
- [ ] Usa Bogus para generar datos de prueba
- [ ] El test es independiente (puede ejecutarse solo)
- [ ] El test es rápido (< 100ms)
- [ ] Incluye tanto casos de éxito como de error
- [ ] Verifica las llamadas a repositorios y output ports
- [ ] El código del test es simple y legible
- [ ] Todos los tests pasan

---

**¡Gracias por mantener la calidad del código con buenos tests!** ??

---

*Última actualización: 2024-01-15*
*Versión: 1.0.0*
