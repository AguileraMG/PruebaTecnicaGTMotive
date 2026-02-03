# Infrastructure Tests

Este proyecto contiene tests de infraestructura para el microservicio de alquiler de vehículos.

## ¿Qué son los Infrastructure Tests?

Los **Infrastructure Tests** prueban la capa de infraestructura con **dependencias reales** (sin mocks), pero en un entorno controlado de pruebas. A diferencia de los Unit Tests que mockean todas las dependencias, estos tests validan:

- ? Operaciones reales contra MongoDB
- ? Serialización/deserialización de entidades
- ? Configuración de servicios
- ? Repositorios con base de datos real

## Prerrequisitos

Antes de ejecutar estos tests, necesitas tener **MongoDB** corriendo localmente:

### Opción 1: Docker (Recomendado)

```bash
docker run -d -p 27017:27017 --name mongodb-test mongo:latest
```

### Opción 2: MongoDB Local

Instala MongoDB localmente y asegúrate de que esté corriendo en `localhost:27017`.

## Configuración

Los tests utilizan una base de datos de prueba separada:

- **Connection String**: `mongodb://localhost:27017`
- **Database Name**: `RentalTestDb`

La configuración está en: `appsettings.IntegrationTest.json`

## Estructura del Proyecto

```
InfrastructureTests/
??? Infrastructure/
?   ??? GenericInfrastructureTestServerFixture.cs   # Configura el TestServer
?   ??? InfrastructureTestBase.cs                   # Clase base para tests
?   ??? Startup.cs                                   # Configuración de servicios
?   ??? TestCollections.cs                           # Colecciones de tests
?   ??? TestServerCollectionFixture.cs               # Fixture de xUnit
??? Repositories/
?   ??? VehicleRepositoryTests.cs                    # Tests del VehicleRepository
?   ??? CustomerRepositoryTests.cs                   # Tests del CustomerRepository
?   ??? RentalRepositoryTests.cs                     # Tests del RentalRepository
??? appsettings.IntegrationTest.json                 # Configuración para tests
??? GlobalSuppressions.cs                            # Supresiones de análisis de código
```

## Ejecutar los Tests

### Desde Visual Studio

1. Asegúrate de que MongoDB esté corriendo
2. Abre **Test Explorer** (Test > Test Explorer)
3. Haz clic en "Run All" o ejecuta tests individuales

### Desde CLI

```bash
# Navegar al directorio del proyecto
cd test\infrastructure\GtMotive.Estimate.Microservice.InfrastructureTests

# Ejecutar todos los tests
dotnet test

# Ejecutar con más detalles
dotnet test --verbosity normal

# Ejecutar un test específico
dotnet test --filter "FullyQualifiedName~VehicleRepositoryTests"
```

## Tests Disponibles

### VehicleRepositoryTests

- `AddAsync_ShouldPersistVehicleToDatabase` - Verifica que un vehículo se persista correctamente
- `GetByLicensePlateAsync_ShouldReturnVehicle_WhenExists` - Busca vehículo por matrícula
- `GetByLicensePlateAsync_ShouldReturnNull_WhenNotExists` - Verifica que retorne null cuando no existe
- `UpdateAsync_ShouldModifyVehicleStatusInDatabase` - Actualiza el estado del vehículo
- `GetByStatusAsync_ShouldReturnOnlyVehiclesWithSpecifiedStatus` - Filtra vehículos por estado
- `GetAllAsync_ShouldReturnAllVehicles` - Obtiene todos los vehículos

### CustomerRepositoryTests

- `AddAsync_ShouldPersistCustomerToDatabase` - Verifica que un cliente se persista correctamente
- `GetByIdAsync_ShouldReturnNull_WhenNotExists` - Verifica que retorne null cuando no existe
- `UpdateAsync_ShouldModifyCustomerInDatabase` - Actualiza un cliente
- `GetAllAsync_ShouldReturnAllCustomers` - Obtiene todos los clientes
- `MultipleOperations_ShouldWorkCorrectly` - Verifica múltiples operaciones sobre el mismo cliente

### RentalRepositoryTests

- `AddAsyncShouldPersistRentalToDatabase` - Verifica que un alquiler se persista correctamente
- `GetByIdAsyncShouldReturnNullWhenNotExists` - Verifica que retorne null cuando no existe
- `UpdateAsyncShouldMarkRentalAsCompleted` - Marca un alquiler como completado
- `GetAllRentalsAsyncShouldReturnAllRentals` - Obtiene todos los alquileres
- `GetActiveRentalsAsyncShouldReturnOnlyActiveRentals` - Obtiene solo alquileres activos
- `GetActiveRentalByVehicleIdAsyncShouldReturnActiveRental` - Busca alquiler activo de un vehículo
- `GetActiveRentalsByCustomerIdAsyncShouldReturnCustomerActiveRentals` - Busca alquileres activos de un cliente

## Características Importantes

### Limpieza de Datos

Cada test implementa `IAsyncLifetime` de xUnit:

```csharp
public async Task InitializeAsync()
{
    // Limpia la base de datos antes de cada test
    var database = _mongoService.MongoClient.GetDatabase("RentalTestDb");
    await database.DropCollectionAsync("Vehicles");
}
```

Esto asegura que cada test comience con una base de datos limpia.

### Compartir el Fixture

Los tests utilizan `TestServerCollectionFixture` para compartir el TestServer entre tests, mejorando el rendimiento:

```csharp
[Collection(TestCollections.TestServer)]
public class VehicleRepositoryTests : InfrastructureTestBase
{
    // ...
}
```

## Diferencias con Unit Tests

| Aspecto | Unit Tests | Infrastructure Tests |
|---------|-----------|---------------------|
| **Dependencias** | Mockeadas con Moq | Reales (MongoDB, servicios) |
| **Velocidad** | Muy rápidos (< 1ms) | Más lentos (requieren BD) |
| **Alcance** | Lógica de negocio aislada | Integración con infraestructura |
| **Base de Datos** | No requieren | Requieren MongoDB real |
| **Propósito** | Validar reglas de negocio | Validar persistencia y repositorios |

## Troubleshooting

### Error: "Connection refused" o "No connection could be made"

**Problema**: MongoDB no está corriendo.

**Solución**:
```bash
# Iniciar MongoDB con Docker
docker start mongodb-test

# O si no existe el contenedor
docker run -d -p 27017:27017 --name mongodb-test mongo:latest
```

### Error: "Database name is invalid"

**Problema**: El nombre de la base de datos en `appsettings.IntegrationTest.json` es incorrecto.

**Solución**: Verifica que el archivo de configuración tenga:
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "RentalTestDb"
  }
}
```

### Tests fallan intermitentemente

**Problema**: Problemas de concurrencia o datos residuales.

**Solución**: Asegúrate de que cada test limpia su colección en `InitializeAsync()`.

## Buenas Prácticas

1. ? **Aislamiento**: Cada test debe ser independiente y limpiar sus datos
2. ? **Descriptivos**: Los nombres de los tests deben describir claramente qué se está probando
3. ? **AAA Pattern**: Arrange, Act, Assert - estructura clara en cada test
4. ? **Base de datos de prueba**: Nunca usar la base de datos de producción o desarrollo
5. ? **Validaciones completas**: Verificar todas las propiedades relevantes del resultado

## Próximos Pasos

Después de los Infrastructure Tests, deberías implementar:

1. **Functional Tests** (E2E/API Tests) - Prueban endpoints HTTP completos
2. **Performance Tests** - Validan tiempos de respuesta y escalabilidad
3. **Integration Tests** - Prueban flujos completos entre múltiples servicios

## Recursos Adicionales

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [MongoDB.Driver Documentation](https://mongodb.github.io/mongo-csharp-driver/)
- [ASP.NET Core TestServer](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)
