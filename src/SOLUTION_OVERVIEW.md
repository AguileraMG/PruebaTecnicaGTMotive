# ?? GtMotive Vehicle Rental System - Análisis Completo de Solución

## ?? Tabla de Contenidos

1. [Resumen Ejecutivo](#-resumen-ejecutivo)
2. [Arquitectura del Sistema](#-arquitectura-del-sistema)
3. [Stack Tecnológico](#-stack-tecnológico)
4. [Modelo de Dominio](#-modelo-de-dominio)
5. [Casos de Uso Implementados](#-casos-de-uso-implementados)
6. [Flujos de Negocio](#-flujos-de-negocio)
7. [API RESTful](#-api-restful)
8. [Estrategia de Testing](#-estrategia-de-testing)
9. [Patrones de Diseño Aplicados](#-patrones-de-diseño-aplicados)
10. [Configuración y Despliegue](#-configuración-y-despliegue)
11. [Métricas de Calidad](#-métricas-de-calidad)

---

## ?? Resumen Ejecutivo

### Descripción del Proyecto

Sistema de gestión de alquiler de vehículos desarrollado como **microservicio RESTful** siguiendo los principios de **Clean Architecture**, **Domain-Driven Design (DDD)** y **SOLID**. La solución implementa un sistema completo de gestión de flota vehicular con reglas de negocio robustas, validaciones exhaustivas y una arquitectura altamente desacoplada y testeable.

### Objetivo Principal

Proporcionar una plataforma escalable, mantenible y testeable para gestionar operaciones de alquiler de vehículos, incluyendo:
- Gestión de clientes y vehículos
- Proceso completo de alquiler y devolución
- Validaciones de reglas de negocio
- Consultas flexibles con filtros
- Trazabilidad completa de operaciones

### Características Destacadas

| Característica | Implementación |
|----------------|----------------|
| **Arquitectura** | Clean Architecture (4 capas) |
| **Paradigma** | Domain-Driven Design (DDD) |
| **Base de Datos** | MongoDB (NoSQL) |
| **Framework** | .NET 9.0 |
| **API** | RESTful con Swagger/OpenAPI |
| **Testing** | 3 niveles (Unit, Infrastructure, Functional) |
| **Containerización** | Docker + Docker Compose |
| **Cobertura de Tests** | 18+ tests unitarios implementados |

---

## ??? Arquitectura del Sistema

### Patrón Arquitectónico: Clean Architecture

La solución está estructurada siguiendo el patrón **Clean Architecture** (también conocido como **Onion Architecture** o **Hexagonal Architecture**), que organiza el código en capas concéntricas con dependencias dirigidas hacia el núcleo.

```
???????????????????????????????????????????????????????????????????
?                                                                  ?
?                    HOST LAYER (Entry Point)                      ?
?                                                                  ?
?  ??????????????????????????????????????????????????????????    ?
?  ?  GtMotive.Estimate.Microservice.Host                   ?    ?
?  ?  • Program.cs (Startup)                                ?    ?
?  ?  • appsettings.json (Configuration)                    ?    ?
?  ?  • Dockerfile (Containerization)                       ?    ?
?  ??????????????????????????????????????????????????????????    ?
?                                                                  ?
???????????????????????????????????????????????????????????????????
                              ?
                              ?
???????????????????????????????????????????????????????????????????
?                                                                  ?
?              PRESENTATION LAYER (API / User Interface)           ?
?                                                                  ?
?  ??????????????????????????????????????????????????????????    ?
?  ?  GtMotive.Estimate.Microservice.Api                    ?    ?
?  ?                                                        ?    ?
?  ?  Controllers:                                          ?    ?
?  ?  • CustomersController                                ?    ?
?  ?  • VehiclesController                                 ?    ?
?  ?  • RentalsController                                  ?    ?
?  ?                                                        ?    ?
?  ?  Presenters (Output Port Implementations):            ?    ?
?  ?  • CreateCustomerPresenter                            ?    ?
?  ?  • CreateVehiclePresenter                             ?    ?
?  ?  • RentVehiclePresenter                               ?    ?
?  ?  • ReturnVehiclePresenter                             ?    ?
?  ?  • GetAllCustomersPresenter                           ?    ?
?  ?  • GetVehiclesByStatusPresenter                       ?    ?
?  ?  • GetAllRentalsPresenter                             ?    ?
?  ?  • GetRentalByLicensePlatePresenter                   ?    ?
?  ?                                                        ?    ?
?  ?  Filters & Middleware:                                ?    ?
?  ?  • BusinessExceptionFilter                            ?    ?
?  ?  • InternalServerErrorObjectResult                    ?    ?
?  ??????????????????????????????????????????????????????????    ?
?                                                                  ?
???????????????????????????????????????????????????????????????????
                              ?
                              ?
???????????????????????????????????????????????????????????????????
?                                                                  ?
?         APPLICATION LAYER (Use Cases / Business Logic)           ?
?                                                                  ?
?  ??????????????????????????????????????????????????????????    ?
?  ?  GtMotive.Estimate.Microservice.ApplicationCore        ?    ?
?  ?                                                        ?    ?
?  ?  Use Cases (8 casos implementados):                   ?    ?
?  ?                                                        ?    ?
?  ?  Customers:                                            ?    ?
?  ?  • CreateCustomerUseCase                              ?    ?
?  ?  • GetAllCustomersUseCase                             ?    ?
?  ?                                                        ?    ?
?  ?  Vehicles:                                             ?    ?
?  ?  • CreateVehicleUseCase                               ?    ?
?  ?  • GetVehiclesByStatusUseCase                         ?    ?
?  ?                                                        ?    ?
?  ?  Rentals:                                              ?    ?
?  ?  • RentVehicleUseCase                                 ?    ?
?  ?  • ReturnVehicleUseCase                               ?    ?
?  ?  • GetAllRentalsUseCase                               ?    ?
?  ?  • GetRentalByLicensePlateUseCase                     ?    ?
?  ?                                                        ?    ?
?  ?  Abstractions:                                         ?    ?
?  ?  • IUseCase<TInput>                                   ?    ?
?  ?  • IOutputPort interfaces                             ?    ?
?  ?  • DTOs (Input/Output)                                ?    ?
?  ?                                                        ?    ?
?  ?  Repository Interfaces:                                ?    ?
?  ?  • ICustomerRepository                                ?    ?
?  ?  • IVehicleRepository                                 ?    ?
?  ?  • IRentalRepository                                  ?    ?
?  ??????????????????????????????????????????????????????????    ?
?                                                                  ?
???????????????????????????????????????????????????????????????????
                              ?
                              ?
???????????????????????????????????????????????????????????????????
?                                                                  ?
?           DOMAIN LAYER (Core Business Rules / Entities)          ?
?                                                                  ?
?  ??????????????????????????????????????????????????????????    ?
?  ?  GtMotive.Estimate.Microservice.Domain                 ?    ?
?  ?                                                        ?    ?
?  ?  Entities (Aggregates):                               ?    ?
?  ?  • Customer                                           ?    ?
?  ?  • Vehicle                                            ?    ?
?  ?  • Rental                                             ?    ?
?  ?                                                        ?    ?
?  ?  Enumerations:                                         ?    ?
?  ?  • VehicleStatus (Available, Rented, Retired)        ?    ?
?  ?  • RentalStatus (Active, Completed, Cancelled)       ?    ?
?  ?                                                        ?    ?
?  ?  Domain Logic:                                         ?    ?
?  ?  • Vehicle.IsEligibleForFleet() - Max 5 años         ?    ?
?  ?  • Customer.CanRentVehicle() - Un alquiler activo    ?    ?
?  ?  • Rental.IsOverdue() - Validar fecha vencida        ?    ?
?  ?                                                        ?    ?
?  ?  Exceptions:                                           ?    ?
?  ?  • DomainException                                    ?    ?
?  ??????????????????????????????????????????????????????????    ?
?                                                                  ?
???????????????????????????????????????????????????????????????????
                              ?
                              ?
???????????????????????????????????????????????????????????????????
?                                                                  ?
?    INFRASTRUCTURE LAYER (Persistence / External Services)        ?
?                                                                  ?
?  ??????????????????????????????????????????????????????????    ?
?  ?  GtMotive.Estimate.Microservice.Infrastructure         ?    ?
?  ?                                                        ?    ?
?  ?  Repositories (Implementaciones):                      ?    ?
?  ?  • CustomerRepository                                 ?    ?
?  ?  • VehicleRepository                                  ?    ?
?  ?  • RentalRepository                                   ?    ?
?  ?                                                        ?    ?
?  ?  MongoDB:                                              ?    ?
?  ?  • MongoService                                       ?    ?
?  ?  • MongoDbExtensions                                  ?    ?
?  ?  • Collection Mappings                                ?    ?
?  ?                                                        ?    ?
?  ?  Infrastructure Configuration:                         ?    ?
?  ?  • InfrastructureConfiguration.cs                     ?    ?
?  ??????????????????????????????????????????????????????????    ?
?                                                                  ?
???????????????????????????????????????????????????????????????????
                              ?
                              ?
                    ????????????????????
                    ?     MongoDB      ?
                    ?   localhost:27017?
                    ?  GtMotiveRental  ?
                    ????????????????????
```

### Flujo de Dependencias

```
???????????????????????????????????????????????????????????????
?                     REGLA DE DEPENDENCIAS                    ?
?                                                              ?
?  Las dependencias apuntan SIEMPRE hacia el centro (Domain)  ?
?                                                              ?
?  Host  ????????????  Api  ?????????  Application  ?????? Domain
?                        ?                  ?
?                        ?                  ?
?                        ?                  ?
?                        ?                  ?
?                Infrastructure ????????? Domain
?                                           ?
?                                           ?
?                                    SIN DEPENDENCIAS
?                                       EXTERNAS
???????????????????????????????????????????????????????????????
```

### Beneficios de la Arquitectura

| Característica | Beneficio |
|----------------|-----------|
| **Independencia de Frameworks** | El dominio no conoce ASP.NET, MongoDB o cualquier framework |
| **Testeable** | Cada capa puede testearse independientemente |
| **Independencia de UI** | La lógica de negocio no depende de la interfaz |
| **Independencia de BD** | Se puede cambiar MongoDB por SQL Server sin tocar el dominio |
| **Reglas de Negocio Protegidas** | El dominio está aislado y protegido de cambios externos |

---

## ?? Stack Tecnológico

### Framework y Runtime

```yaml
Framework: .NET 9.0
Language: C# 13.0
SDK Version: 9.0.308
Target Framework Moniker: net9.0
```

### Bases de Datos y Persistencia

| Tecnología | Versión | Propósito |
|------------|---------|-----------|
| **MongoDB** | 2.19.0+ | Base de datos NoSQL principal |
| **MongoDB.Driver** | 2.19.0 | Driver oficial para .NET |

**Características de MongoDB utilizadas:**
- ? Colecciones: `Customers`, `Vehicles`, `Rentals`
- ? Índices únicos: `Email`, `DriverLicenseNumber`, `LicensePlate`
- ? Índices compuestos para búsquedas optimizadas
- ? Operaciones atómicas con transacciones

### Librerías Core

#### Testing Framework
```xml
<PackageReference Include="xUnit" Version="2.9.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
<PackageReference Include="Moq" Version="4.18.1" />
<PackageReference Include="Bogus" Version="35.6.5" />
<PackageReference Include="FluentAssertions" Version="7.0.0" />
<PackageReference Include="coverlet.collector" Version="6.0.2" />
```

#### Documentación y API
```xml
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.4.0" />
<PackageReference Include="Swashbuckle.AspNetCore.Annotations" Version="6.4.0" />
```

#### Inyección de Dependencias y Configuración
```xml
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="9.0.0" />
```

#### Logging y Telemetría
```xml
<PackageReference Include="Serilog.AspNetCore" Version="6.1.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="4.1.0" />
<PackageReference Include="Microsoft.ApplicationInsights.AspNetCore" Version="2.21.0" />
```

#### Resiliencia y Patrones
```xml
<PackageReference Include="Polly" Version="7.2.3" />
<PackageReference Include="MediatR" Version="10.0.1" />
```

### Herramientas de Desarrollo

| Herramienta | Propósito |
|-------------|-----------|
| **Visual Studio 2022** | IDE principal |
| **Docker Desktop** | Containerización |
| **Docker Compose** | Orquestación multi-contenedor |
| **Git** | Control de versiones |
| **Swagger UI** | Documentación interactiva de API |
| **mongosh** | Cliente de MongoDB (opcional) |

### Containerización

**Dockerfile:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["GtMotive.Estimate.Microservice.Host/", "GtMotive.Estimate.Microservice.Host/"]
# ... más instrucciones
```

**Docker Compose:**
```yaml
services:
  api:
    image: gtmotive-rental-api
    ports:
      - "5000:80"
      - "5001:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - MongoDb__ConnectionString=mongodb://mongodb:27017
    depends_on:
      - mongodb
      
  mongodb:
    image: mongo:latest
    ports:
      - "27017:27017"
    volumes:
      - mongodb_data:/data/db
```

---

## ?? Modelo de Dominio

### Entidad: Customer (Cliente)

Representa un cliente del sistema con capacidad de alquilar vehículos.

#### Propiedades

| Propiedad | Tipo | Descripción | Validación |
|-----------|------|-------------|------------|
| `Id` | `string` | Identificador único (GUID) | Requerido, generado automáticamente |
| `Name` | `string` | Nombre completo del cliente | Requerido, no vacío |
| `Email` | `string` | Correo electrónico | Requerido, único, formato válido |
| `PhoneNumber` | `string` | Número de teléfono | Requerido |
| `DriverLicenseNumber` | `string` | Número de licencia de conducir | Requerido, único |
| `HasActiveRental` | `bool` | Indica alquiler activo | Default: false |
| `CreatedAt` | `DateTime` | Fecha de registro | Auto-generado |
| `UpdatedAt` | `DateTime?` | Última actualización | Auto-actualizado |

#### Métodos de Dominio

```csharp
// Valida si el cliente puede alquilar un vehículo
public bool CanRentVehicle()
{
    return !HasActiveRental;
}

// Marca al cliente como alquilando
public void MarkAsRenting()
{
    if (HasActiveRental)
        throw new DomainException($"Customer {Name} already has an active rental.");
    
    HasActiveRental = true;
    UpdatedAt = DateTime.UtcNow;
}

// Marca al cliente como sin alquiler activo
public void MarkAsNotRenting()
{
    if (!HasActiveRental)
        throw new DomainException($"Customer {Name} does not have an active rental.");
    
    HasActiveRental = false;
    UpdatedAt = DateTime.UtcNow;
}
```

#### Reglas de Negocio

| Regla | Descripción | Implementación |
|-------|-------------|----------------|
| **UN01** | Un cliente no puede tener más de un alquiler activo | `CanRentVehicle()` |
| **UN02** | El email debe ser único en el sistema | Validado en `CreateCustomerUseCase` |
| **UN03** | La licencia de conducir debe ser única | Validado en `CreateCustomerUseCase` |
| **UN04** | El estado de alquiler debe actualizarse atómicamente | Métodos `MarkAsRenting()` / `MarkAsNotRenting()` |

---

### Entidad: Vehicle (Vehículo)

Representa un vehículo de la flota disponible para alquiler.

#### Propiedades

| Propiedad | Tipo | Descripción | Validación |
|-----------|------|-------------|------------|
| `Id` | `string` | Identificador único (GUID) | Requerido, auto-generado |
| `Brand` | `string` | Marca del vehículo | Requerido, no vacío |
| `Model` | `string` | Modelo del vehículo | Requerido, no vacío |
| `Year` | `int` | Año de fabricación | Requerido, 1900-actual |
| `KilometersDriven` | `int` | Kilómetros recorridos | >= 0, no puede decrecer |
| `LicensePlate` | `string` | Matrícula del vehículo | Requerido, único |
| `Status` | `VehicleStatus` | Estado actual | Enum: Available, Rented, Retired |
| `CreatedAt` | `DateTime` | Fecha de registro | Auto-generado |
| `UpdatedAt` | `DateTime?` | Última actualización | Auto-actualizado |

#### Estados del Vehículo (VehicleStatus)

```csharp
public enum VehicleStatus
{
    Available = 0,    // Disponible para alquilar
    Rented = 1,       // Actualmente alquilado
    Retired = 2       // Retirado de la flota
}
```

#### Métodos de Dominio

```csharp
// Verifica si el vehículo es elegible (máximo 5 años)
public bool IsEligibleForFleet()
{
    var currentYear = DateTime.UtcNow.Year;
    var vehicleAge = currentYear - Year;
    return vehicleAge <= 5;
}

// Verifica disponibilidad
public bool IsAvailable()
{
    return Status == VehicleStatus.Available;
}

// Marca como alquilado
public void MarkAsRented()
{
    if (!IsAvailable())
        throw new DomainException($"Vehicle {LicensePlate} is not available for rent.");
    
    Status = VehicleStatus.Rented;
    UpdatedAt = DateTime.UtcNow;
}

// Marca como disponible
public void MarkAsAvailable()
{
    if (Status != VehicleStatus.Rented)
        throw new DomainException($"Vehicle {LicensePlate} is not currently rented.");
    
    Status = VehicleStatus.Available;
    UpdatedAt = DateTime.UtcNow;
}

// Marca como retirado
public void MarkAsRetired()
{
    if (Status == VehicleStatus.Retired)
        throw new DomainException($"Vehicle {LicensePlate} is already retired.");
    
    Status = VehicleStatus.Retired;
    UpdatedAt = DateTime.UtcNow;
}

// Actualiza odómetro (no puede retroceder)
public void SetKilometers(int kilometers)
{
    if (kilometers < 0)
        throw new DomainException("Kilometers cannot be negative.");
    
    if (kilometers < KilometersDriven)
        throw new DomainException(
            $"New odometer reading ({kilometers} km) cannot be less than current reading ({KilometersDriven} km).");
    
    KilometersDriven = kilometers;
    UpdatedAt = DateTime.UtcNow;
}
```

#### Reglas de Negocio

| Regla | Descripción | Implementación |
|-------|-------------|----------------|
| **VH01** | Solo vehículos <= 5 años pueden agregarse | `IsEligibleForFleet()` en `CreateVehicleUseCase` |
| **VH02** | Vehículos > 5 años se retiran al devolverse | `ReturnVehicleUseCase` verifica edad |
| **VH03** | No se puede alquilar vehículo no disponible | `MarkAsRented()` valida estado |
| **VH04** | El odómetro no puede retroceder | `SetKilometers()` valida incremento |
| **VH05** | Matrícula única en el sistema | Validado en `CreateVehicleUseCase` |

---

### Entidad: Rental (Alquiler)

Representa una transacción de alquiler de vehículo a cliente.

#### Propiedades

| Propiedad | Tipo | Descripción | Validación |
|-----------|------|-------------|------------|
| `Id` | `string` | Identificador único (GUID) | Requerido, auto-generado |
| `VehicleId` | `string` | ID del vehículo alquilado | Requerido, FK a Vehicle |
| `CustomerId` | `string` | ID del cliente | Requerido, FK a Customer |
| `RentalDate` | `DateTime` | Fecha/hora de inicio | Auto-generado (UTC) |
| `ReturnDate` | `DateTime?` | Fecha/hora de devolución | null si activo |
| `ExpectedReturnDate` | `DateTime` | Fecha esperada de devolución | Requerido, futura |
| `Status` | `RentalStatus` | Estado del alquiler | Enum: Active, Completed, Cancelled |
| `Notes` | `string` | Notas adicionales | Opcional |
| `CreatedAt` | `DateTime` | Fecha de registro | Auto-generado |
| `UpdatedAt` | `DateTime?` | Última actualización | Auto-actualizado |

#### Estados del Alquiler (RentalStatus)

```csharp
public enum RentalStatus
{
    Active = 0,       // Alquiler en curso
    Completed = 1,    // Completado (vehículo devuelto)
    Cancelled = 2     // Cancelado
}
```

#### Propiedades Calculadas

```csharp
// Duración del alquiler en días (si está completado)
public int? RentalDurationInDays
{
    get
    {
        if (ReturnDate.HasValue)
            return (ReturnDate.Value - RentalDate).Days;
        return null;
    }
}
```

#### Métodos de Dominio

```csharp
// Verifica si el alquiler está activo
public bool IsActive()
{
    return Status == RentalStatus.Active && !ReturnDate.HasValue;
}

// Verifica si el alquiler está vencido
public bool IsOverdue()
{
    return IsActive() && DateTime.UtcNow > ExpectedReturnDate;
}

// Completa el alquiler
public void CompleteRental()
{
    if (!IsActive())
        throw new DomainException("Only active rentals can be completed.");
    
    Status = RentalStatus.Completed;
    ReturnDate = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
}

// Cancela el alquiler
public void CancelRental()
{
    if (Status == RentalStatus.Completed)
        throw new DomainException("Cannot cancel a completed rental.");
    
    Status = RentalStatus.Cancelled;
    UpdatedAt = DateTime.UtcNow;
}

// Agrega una nota
public void AddNote(string note)
{
    if (!string.IsNullOrWhiteSpace(note))
    {
        Notes = string.IsNullOrEmpty(Notes) ? note : $"{Notes}\n{note}";
        UpdatedAt = DateTime.UtcNow;
    }
}
```

#### Reglas de Negocio

| Regla | Descripción | Implementación |
|-------|-------------|----------------|
| **RN01** | Solo alquileres activos pueden completarse | `CompleteRental()` |
| **RN02** | No se pueden cancelar alquileres completados | `CancelRental()` |
| **RN03** | Fecha esperada de devolución debe ser futura | Validado en `RentVehicleUseCase` |
| **RN04** | Un vehículo solo puede tener un alquiler activo | Validado en `RentVehicleUseCase` |
| **RN05** | Al completar, debe registrarse fecha de devolución | `CompleteRental()` establece `ReturnDate` |

---

## ?? Casos de Uso Implementados

### 1. CreateCustomer (Crear Cliente)

#### Descripción
Permite registrar un nuevo cliente en el sistema con validación de datos únicos.

#### Input (CreateCustomerInput)
```csharp
{
  "name": "Juan Pérez García",
  "email": "juan.perez@example.com",
  "phoneNumber": "+34 600 123 456",
  "driverLicenseNumber": "12345678A"
}
```

#### Proceso de Validación
1. ? Verifica que todos los campos estén completos
2. ? Valida formato de email
3. ? Verifica que el email no exista
4. ? Verifica que la licencia de conducir no exista
5. ? Crea el cliente con `HasActiveRental = false`
6. ? Genera timestamps `CreatedAt`

#### Output (CreateCustomerOutput)
```csharp
{
  "id": "507f1f77bcf86cd799439011",
  "name": "Juan Pérez García",
  "email": "juan.perez@example.com",
  "phoneNumber": "+34 600 123 456",
  "driverLicenseNumber": "12345678A",
  "hasActiveRental": false,
  "createdAt": "2024-02-01T10:30:00Z"
}
```

#### Posibles Respuestas

| Código HTTP | Escenario | Respuesta |
|-------------|-----------|-----------|
| **201 Created** | Cliente creado exitosamente | CreateCustomerOutput |
| **404 Not Found** | Email duplicado | "Customer with email {email} already exists" |
| **404 Not Found** | Licencia duplicada | "Customer with license {license} already exists" |
| **400 Bad Request** | Input inválido | Detalles de validación |

#### Código Relevante
```csharp
public sealed class CreateCustomerUseCase : IUseCase<CreateCustomerInput>
{
    public async Task ExecuteAsync(CreateCustomerInput input, CancellationToken ct)
    {
        // Verificar email único
        var existingByEmail = await _customerRepository.GetByEmailAsync(input.Email, ct);
        if (existingByEmail != null)
        {
            _outputPort.NotFoundHandle($"Customer with email {input.Email} already exists.");
            return;
        }

        // Verificar licencia única
        var existingByLicense = await _customerRepository
            .GetByDriverLicenseAsync(input.DriverLicenseNumber, ct);
        if (existingByLicense != null)
        {
            _outputPort.NotFoundHandle(
                $"Customer with driver license {input.DriverLicenseNumber} already exists.");
            return;
        }

        // Crear entidad
        var customer = new Customer
        {
            Id = Guid.NewGuid().ToString(),
            Name = input.Name,
            Email = input.Email,
            PhoneNumber = input.PhoneNumber,
            DriverLicenseNumber = input.DriverLicenseNumber,
            HasActiveRental = false,
            CreatedAt = DateTime.UtcNow
        };

        await _customerRepository.AddAsync(customer, ct);

        _outputPort.StandardHandle(new CreateCustomerOutput
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            DriverLicenseNumber = customer.DriverLicenseNumber,
            HasActiveRental = customer.HasActiveRental,
            CreatedAt = customer.CreatedAt
        });
    }
}
```

---

### 2. GetAllCustomers (Obtener Todos los Clientes)

#### Descripción
Recupera todos los clientes del sistema con opción de filtrar por estado de alquiler.

#### Input (GetAllCustomersInput)
```csharp
{
  "hasActiveRental": null  // null (todos), true (con alquiler), false (sin alquiler)
}
```

#### Output (GetAllCustomersOutput)
```csharp
{
  "customers": [
    {
      "id": "507f1f77bcf86cd799439011",
      "name": "Juan Pérez",
      "email": "juan.perez@example.com",
      "phoneNumber": "+34 600 123 456",
      "driverLicenseNumber": "12345678A",
      "hasActiveRental": true,
      "createdAt": "2024-02-01T10:30:00Z"
    },
    {
      "id": "507f1f77bcf86cd799439012",
      "name": "María López",
      "email": "maria.lopez@example.com",
      "phoneNumber": "+34 600 987 654",
      "driverLicenseNumber": "87654321B",
      "hasActiveRental": false,
      "createdAt": "2024-02-01T11:00:00Z"
    }
  ],
  "totalCount": 2
}
```

#### Filtros Disponibles

| Filtro | Descripción | Ejemplo Query |
|--------|-------------|---------------|
| `null` | Todos los clientes | `GET /api/customers` |
| `true` | Solo con alquiler activo | `GET /api/customers?hasActiveRental=true` |
| `false` | Solo sin alquiler activo | `GET /api/customers?hasActiveRental=false` |

---

### 3. CreateVehicle (Crear Vehículo)

#### Descripción
Registra un nuevo vehículo en la flota con validación de antigüedad (máximo 5 años).

#### Input (CreateVehicleInput)
```csharp
{
  "brand": "Toyota",
  "model": "Corolla",
  "year": 2023,
  "licensePlate": "1234-ABC",
  "kilometersDriven": 5000
}
```

#### Validaciones Críticas
1. ? **Antigüedad**: El vehículo no debe tener más de 5 años
2. ? **Matrícula única**: No puede existir otra matrícula igual
3. ? **Kilómetros positivos**: >= 0
4. ? **Año válido**: Entre 1900 y año actual

#### Proceso
```
Input Recibido
     ?
     ?
Validar Antigüedad (año actual - año vehículo <= 5)
     ?
     ?? NO ? NotFoundHandle("Vehicle is too old to be added to the fleet")
     ?
     ?? SÍ
         ?
         ?
    Validar Matrícula Única
         ?
         ?? EXISTE ? NotFoundHandle("Vehicle with license plate already exists")
         ?
         ?? NO EXISTE
             ?
             ?
        Crear Vehicle Entity
        Status = Available
             ?
             ?
        Guardar en Repository
             ?
             ?
        StandardHandle(CreateVehicleOutput)
```

#### Output (CreateVehicleOutput)
```csharp
{
  "id": "507f1f77bcf86cd799439013",
  "brand": "Toyota",
  "model": "Corolla",
  "year": 2023,
  "licensePlate": "1234-ABC",
  "kilometersDriven": 5000,
  "status": "Available",
  "createdAt": "2024-02-01T12:00:00Z"
}
```

#### Ejemplo de Rechazo (Vehículo Antiguo)

**Request:**
```json
{
  "brand": "Honda",
  "model": "Civic",
  "year": 2014,  // Más de 5 años
  "licensePlate": "5678-XYZ",
  "kilometersDriven": 150000
}
```

**Response (404 Not Found):**
```json
{
  "message": "Vehicle is too old to be added to the fleet. Maximum age is 5 years."
}
```

---

### 4. GetVehiclesByStatus (Obtener Vehículos por Estado)

#### Descripción
Consulta vehículos filtrados por su estado actual.

#### Input (GetVehiclesByStatusInput)
```csharp
{
  "status": 0  // 0 = Available, 1 = Rented, 2 = Retired
}
```

#### Estados Disponibles

| Valor | Enum | Descripción |
|-------|------|-------------|
| `0` | `Available` | Vehículos disponibles para alquilar |
| `1` | `Rented` | Vehículos actualmente alquilados |
| `2` | `Retired` | Vehículos retirados de la flota |

#### Output (GetVehiclesByStatusOutput)
```csharp
{
  "vehicles": [
    {
      "id": "507f1f77bcf86cd799439013",
      "brand": "Toyota",
      "model": "Corolla",
      "year": 2023,
      "licensePlate": "1234-ABC",
      "kilometersDriven": 5000,
      "status": "Available",
      "createdAt": "2024-02-01T12:00:00Z",
      "isEligibleForFleet": true
    }
  ],
  "totalCount": 1,
  "filterApplied": "Available"
}
```

---

### 5. RentVehicle (Alquilar Vehículo)

#### Descripción
Proceso completo de alquiler: valida disponibilidad, elegibilidad, estado del cliente, y crea el alquiler.

#### Input (RentVehicleInput)
```csharp
{
  "vehicleId": "507f1f77bcf86cd799439013",
  "customerId": "507f1f77bcf86cd799439011",
  "expectedReturnDate": "2024-02-08T10:00:00Z",
  "notes": "Cliente VIP - trato preferencial"
}
```

#### Validaciones Realizadas

```
???????????????????????????????????????????????????????????????
?           FLUJO DE VALIDACIÓN: RentVehicleUseCase           ?
???????????????????????????????????????????????????????????????

1. Verificar Vehículo Existe
   ?? NO ? NotFoundHandle("Vehicle not found")
   ?? SÍ ? Continuar

2. Verificar Vehículo Disponible (status == Available)
   ?? NO ? NotFoundHandle("Vehicle is not available")
   ?? SÍ ? Continuar

3. Verificar Vehículo Elegible (IsEligibleForFleet())
   ?? NO ? NotFoundHandle("Vehicle is too old")
   ?? SÍ ? Continuar

4. Verificar Cliente Existe
   ?? NO ? NotFoundHandle("Customer not found")
   ?? SÍ ? Continuar

5. Verificar Cliente Sin Alquiler Activo (CanRentVehicle())
   ?? NO ? NotFoundHandle("Customer already has an active rental")
   ?? SÍ ? Continuar

6. Verificar Fecha Devolución Futura
   ?? NO ? NotFoundHandle("Expected return date must be in the future")
   ?? SÍ ? Continuar

7. TODAS LAS VALIDACIONES PASADAS
   ?? Vehicle.MarkAsRented()
   ?? Customer.MarkAsRenting()
   ?? Crear Rental (status = Active)
   ?? StandardHandle(RentVehicleOutput)
```

#### Cambios de Estado

**Antes del Alquiler:**
```
Vehicle:
  Status: Available
  UpdatedAt: null

Customer:
  HasActiveRental: false
  UpdatedAt: null

Rental:
  (No existe)
```

**Después del Alquiler:**
```
Vehicle:
  Status: Rented
  UpdatedAt: 2024-02-01T14:30:00Z

Customer:
  HasActiveRental: true
  UpdatedAt: 2024-02-01T14:30:00Z

Rental:
  Id: "507f1f77bcf86cd799439020"
  Status: Active
  RentalDate: 2024-02-01T14:30:00Z
  ReturnDate: null
  ExpectedReturnDate: 2024-02-08T10:00:00Z
```

#### Output (RentVehicleOutput)
```csharp
{
  "rentalId": "507f1f77bcf86cd799439020",
  "vehicleId": "507f1f77bcf86cd799439013",
  "vehicleBrand": "Toyota",
  "vehicleModel": "Corolla",
  "vehicleLicensePlate": "1234-ABC",
  "customerId": "507f1f77bcf86cd799439011",
  "customerName": "Juan Pérez García",
  "rentalDate": "2024-02-01T14:30:00Z",
  "expectedReturnDate": "2024-02-08T10:00:00Z",
  "status": "Active",
  "notes": "Cliente VIP - trato preferencial"
}
```

#### Código Simplificado
```csharp
public async Task ExecuteAsync(RentVehicleInput input, CancellationToken ct)
{
    // 1-3: Validar vehículo
    var vehicle = await _vehicleRepository.GetByIdAsync(input.VehicleId, ct);
    if (vehicle == null || !vehicle.IsAvailable() || !vehicle.IsEligibleForFleet())
    {
        _outputPort.NotFoundHandle("Vehicle validation failed");
        return;
    }

    // 4-5: Validar cliente
    var customer = await _customerRepository.GetByIdAsync(input.CustomerId, ct);
    if (customer == null || !customer.CanRentVehicle())
    {
        _outputPort.NotFoundHandle("Customer validation failed");
        return;
    }

    // 6: Validar fecha
    if (input.ExpectedReturnDate <= DateTime.UtcNow)
    {
        _outputPort.NotFoundHandle("Expected return date must be in the future");
        return;
    }

    // 7: Ejecutar alquiler (transacción lógica)
    vehicle.MarkAsRented();
    customer.MarkAsRenting();

    var rental = new Rental
    {
        Id = Guid.NewGuid().ToString(),
        VehicleId = vehicle.Id,
        CustomerId = customer.Id,
        RentalDate = DateTime.UtcNow,
        ExpectedReturnDate = input.ExpectedReturnDate,
        Status = RentalStatus.Active,
        Notes = input.Notes,
        CreatedAt = DateTime.UtcNow
    };

    await _vehicleRepository.UpdateAsync(vehicle, ct);
    await _customerRepository.UpdateAsync(customer, ct);
    await _rentalRepository.AddAsync(rental, ct);

    _outputPort.StandardHandle(/* output */);
}
```

---

### 6. ReturnVehicle (Devolver Vehículo)

#### Descripción
Proceso completo de devolución: actualiza kilómetros, completa el alquiler, libera al cliente, y retira el vehículo si excede 5 años.

#### Input (ReturnVehicleInput)
```csharp
{
  "rentalId": "507f1f77bcf86cd799439020",
  "currentKilometers": 5500,
  "notes": "Vehículo en perfectas condiciones"
}
```

#### Flujo Completo de Devolución

```
??????????????????????????????????????????????????????????????????
?         PROCESO: ReturnVehicleUseCase (Paso a Paso)            ?
??????????????????????????????????????????????????????????????????

1. Buscar Alquiler por ID
   ?? NO EXISTE ? NotFoundHandle("Rental not found")
   ?? EXISTE ? rental

2. Verificar Alquiler Activo (status == Active && ReturnDate == null)
   ?? NO ? NotFoundHandle("Rental is not active")
   ?? SÍ ? Continuar

3. Buscar Vehículo del Alquiler
   ?? NO EXISTE ? NotFoundHandle("Vehicle not found")
   ?? EXISTE ? vehicle

4. Validar Kilómetros No Retroceden
   ?? currentKilometers < vehicle.KilometersDriven
   ?  ?? NotFoundHandle("Odometer reading cannot decrease")
   ?? Válido ? Continuar

5. Buscar Cliente del Alquiler
   ?? NO EXISTE ? NotFoundHandle("Customer not found")
   ?? EXISTE ? customer

6. VALIDACIONES COMPLETAS - EJECUTAR DEVOLUCIÓN:

   6.1. Completar Alquiler
        ?? rental.CompleteRental()
           ?? Status = Completed
           ?? ReturnDate = DateTime.UtcNow

   6.2. Actualizar Kilómetros del Vehículo
        ?? vehicle.SetKilometers(currentKilometers)

   6.3. Liberar Cliente
        ?? customer.MarkAsNotRenting()
           ?? HasActiveRental = false

   6.4. Verificar Antigüedad del Vehículo
        ?? vehicle.IsEligibleForFleet() == TRUE
        ?  ?? vehicle.MarkAsAvailable()
        ?     ?? Status = Available
        ?
        ?? vehicle.IsEligibleForFleet() == FALSE (> 5 años)
           ?? vehicle.MarkAsRetired()
              ?? Status = Retired

7. Persistir Cambios
   ?? await _rentalRepository.UpdateAsync(rental, ct)
   ?? await _vehicleRepository.UpdateAsync(vehicle, ct)
   ?? await _customerRepository.UpdateAsync(customer, ct)

8. Retornar Output
   ?? StandardHandle(ReturnVehicleOutput)
```

#### Ejemplo de Devolución Normal (Vehículo Elegible)

**Request:**
```json
{
  "rentalId": "507f1f77bcf86cd799439020",
  "currentKilometers": 5500,
  "notes": "Todo correcto"
}
```

**Output:**
```json
{
  "rentalId": "507f1f77bcf86cd799439020",
  "vehicleId": "507f1f77bcf86cd799439013",
  "vehicleBrand": "Toyota",
  "vehicleModel": "Corolla",
  "vehicleLicensePlate": "1234-ABC",
  "customerId": "507f1f77bcf86cd799439011",
  "customerName": "Juan Pérez",
  "rentalDate": "2024-02-01T14:30:00Z",
  "returnDate": "2024-02-05T16:45:00Z",
  "expectedReturnDate": "2024-02-08T10:00:00Z",
  "status": "Completed",
  "kilometersDriven": 500,
  "vehicleStatus": "Available",
  "notes": "Todo correcto"
}
```

#### Ejemplo de Devolución con Retiro (Vehículo > 5 años)

**Escenario:** Vehículo fabricado en 2019, devolución en 2024

**Request:**
```json
{
  "rentalId": "507f1f77bcf86cd799439021",
  "currentKilometers": 120000,
  "notes": "Vehículo cumplió 5 años"
}
```

**Output:**
```json
{
  "rentalId": "507f1f77bcf86cd799439021",
  "vehicleId": "507f1f77bcf86cd799439014",
  "vehicleBrand": "Honda",
  "vehicleModel": "Civic",
  "vehicleLicensePlate": "5678-DEF",
  "rentalDate": "2024-01-20T10:00:00Z",
  "returnDate": "2024-02-05T16:45:00Z",
  "expectedReturnDate": "2024-02-03T10:00:00Z",
  "status": "Completed",
  "kilometersDriven": 800,
  "vehicleStatus": "Retired",  // ?? RETIRADO POR ANTIGÜEDAD
  "notes": "Vehículo cumplió 5 años"
}
```

#### Cambios de Estado

**Antes de Devolución:**
```
Rental:
  Status: Active
  ReturnDate: null

Vehicle:
  Status: Rented
  KilometersDriven: 5000

Customer:
  HasActiveRental: true
```

**Después de Devolución (Vehículo Elegible):**
```
Rental:
  Status: Completed
  ReturnDate: 2024-02-05T16:45:00Z

Vehicle:
  Status: Available
  KilometersDriven: 5500

Customer:
  HasActiveRental: false
```

**Después de Devolución (Vehículo > 5 años):**
```
Rental:
  Status: Completed
  ReturnDate: 2024-02-05T16:45:00Z

Vehicle:
  Status: Retired  // ?? RETIRADO
  KilometersDriven: 120000

Customer:
  HasActiveRental: false
```

---

### 7. GetAllRentals (Obtener Todos los Alquileres)

#### Descripción
Recupera todos los alquileres del sistema con opción de filtrar por estado.

#### Input (GetAllRentalsInput)
```csharp
{
  "status": null  // null (todos), 0 (Active), 1 (Completed), 2 (Cancelled)
}
```

#### Output (GetAllRentalsOutput)
```csharp
{
  "rentals": [
    {
      "rentalId": "507f1f77bcf86cd799439020",
      "vehicleId": "507f1f77bcf86cd799439013",
      "customerId": "507f1f77bcf86cd799439011",
      "rentalDate": "2024-02-01T14:30:00Z",
      "returnDate": null,
      "expectedReturnDate": "2024-02-08T10:00:00Z",
      "status": "Active",
      "isOverdue": false
    }
  ],
  "totalCount": 1
}
```

---

### 8. GetRentalByLicensePlate (Obtener Alquiler por Matrícula)

#### Descripción
Busca el alquiler activo de un vehículo mediante su matrícula.

#### Input (GetRentalByLicensePlateInput)
```csharp
{
  "licensePlate": "1234-ABC"
}
```

#### Proceso
```
Input: licensePlate
     ?
     ?
Buscar Vehículo por Matrícula
     ?
     ?? NO EXISTE ? NotFoundHandle("Vehicle not found")
     ?
     ?? EXISTE ? vehicle
         ?
         ?
    Buscar Alquiler Activo (status = Active)
    WHERE VehicleId = vehicle.Id
         ?
         ?? NO EXISTE ? NotFoundHandle("No active rental found for this vehicle")
         ?
         ?? EXISTE ? rental
             ?
             ?
        StandardHandle(GetRentalByLicensePlateOutput)
```

#### Output (GetRentalByLicensePlateOutput)
```csharp
{
  "rentalId": "507f1f77bcf86cd799439020",
  "vehicleId": "507f1f77bcf86cd799439013",
  "vehicleBrand": "Toyota",
  "vehicleModel": "Corolla",
  "vehicleLicensePlate": "1234-ABC",
  "customerId": "507f1f77bcf86cd799439011",
  "customerName": "Juan Pérez",
  "rentalDate": "2024-02-01T14:30:00Z",
  "expectedReturnDate": "2024-02-08T10:00:00Z",
  "status": "Active",
  "isOverdue": false,
  "notes": "Cliente VIP"
}
```

---

## ?? Flujos de Negocio

### Flujo Completo: Desde Registro hasta Devolución

```
?????????????????????????????????????????????????????????????????????????
?                    FLUJO DE NEGOCIO COMPLETO                          ?
?                 (Happy Path - Caso de Éxito)                          ?
?????????????????????????????????????????????????????????????????????????

1. REGISTRO DE VEHÍCULO
   ?? POST /api/vehicles
   ?? Input: { brand: "Toyota", model: "Corolla", year: 2023, ... }
   ?? Validación: Antigüedad <= 5 años ?
   ?? Creación: Vehicle (Id: V-001, Status: Available)
   ?? Response: 201 Created

2. REGISTRO DE CLIENTE
   ?? POST /api/customers
   ?? Input: { name: "Juan Pérez", email: "juan@example.com", ... }
   ?? Validación: Email único ?, Licencia única ?
   ?? Creación: Customer (Id: C-001, HasActiveRental: false)
   ?? Response: 201 Created

3. ALQUILER DE VEHÍCULO
   ?? POST /api/rentals/rent
   ?? Input: { vehicleId: "V-001", customerId: "C-001", expectedReturnDate: "..." }
   ?? Validaciones:
   ?  ?? Vehículo existe ?
   ?  ?? Vehículo disponible (Status == Available) ?
   ?  ?? Vehículo elegible (IsEligibleForFleet()) ?
   ?  ?? Cliente existe ?
   ?  ?? Cliente sin alquiler activo (CanRentVehicle()) ?
   ?  ?? Fecha devolución futura ?
   ?? Cambios de Estado:
   ?  ?? Vehicle.Status = Rented
   ?  ?? Customer.HasActiveRental = true
   ?  ?? Rental creado (Id: R-001, Status: Active)
   ?? Response: 201 Created

4. CONSULTA DE ALQUILER ACTIVO
   ?? GET /api/rentals/by-license-plate/1234-ABC
   ?? Búsqueda: Vehículo por matrícula ? Alquiler activo
   ?? Response: 200 OK (detalles del alquiler)

5. DEVOLUCIÓN DE VEHÍCULO
   ?? POST /api/rentals/return
   ?? Input: { rentalId: "R-001", currentKilometers: 5500, notes: "..." }
   ?? Validaciones:
   ?  ?? Alquiler existe ?
   ?  ?? Alquiler activo ?
   ?  ?? Kilómetros no retroceden ?
   ?  ?? Vehículo y cliente existen ?
   ?? Cambios de Estado:
   ?  ?? Rental.Status = Completed, ReturnDate = now
   ?  ?? Vehicle.KilometersDriven = 5500
   ?  ?? Customer.HasActiveRental = false
   ?  ?? Si Vehicle.IsEligibleForFleet():
   ?     ?? TRUE ? Vehicle.Status = Available
   ?     ?? FALSE ? Vehicle.Status = Retired
   ?? Response: 200 OK (detalles de devolución)

6. VERIFICACIÓN POST-DEVOLUCIÓN
   ?? GET /api/vehicles?status=0 (Available)
   ?? GET /api/customers?hasActiveRental=false
```

### Diagrama de Estados: Vehicle

```
????????????????????????????????????????????????????????????????
?                CICLO DE VIDA DE UN VEHÍCULO                  ?
????????????????????????????????????????????????????????????????

                    ???????????????
                    ?   Created   ?
                    ???????????????
                           ?
                           ? CreateVehicleUseCase
                           ? (si year <= 5 años)
                           ?
                    ???????????????
          ???????????  AVAILABLE  ????????????
          ?         ???????????????          ?
          ?                ?                  ?
          ?                ? RentVehicleUseCase
          ?                ?                  ?
          ?         ???????????????          ?
          ?         ?   RENTED    ?          ?
          ?         ???????????????          ?
          ?                ?                  ?
          ?                ? ReturnVehicleUseCase
          ?                ?                  ?
          ?         ??????????????????       ?
          ?         ?  Verificar     ?       ?
          ?         ?  Antigüedad    ?       ?
          ?         ??????????????????       ?
          ?              ?       ?            ?
          ?    <= 5 años ?       ? > 5 años  ?
          ?              ?       ?            ?
          ????????????????       ?????????????
                                      ???????????????
                                      ?   RETIRED   ?
                                      ???????????????
                                        (Estado Final)
```

### Diagrama de Estados: Customer

```
????????????????????????????????????????????????????????????????
?                CICLO DE VIDA DE UN CLIENTE                   ?
????????????????????????????????????????????????????????????????

                    ???????????????
                    ?   Created   ?
                    ???????????????
                           ?
                           ? CreateCustomerUseCase
                           ?
                    ???????????????????????????
          ??????????? HasActiveRental: false ???????????
          ?         ???????????????????????????         ?
          ?                    ?                         ?
          ?                    ? RentVehicleUseCase     ?
          ?                    ? (MarkAsRenting)        ?
          ?                    ?                         ?
          ?         ???????????????????????????         ?
          ?         ? HasActiveRental: true  ?         ?
          ?         ???????????????????????????         ?
          ?                    ?                         ?
          ?                    ? ReturnVehicleUseCase  ?
          ?                    ? (MarkAsNotRenting)    ?
          ??????????????????????                         ?
                                                         ?
          ??????????????????????????????????????????????
          ?  REGLA: Un cliente NO puede alquilar      ?
          ?  si HasActiveRental == true                ?
          ??????????????????????????????????????????????
```

### Diagrama de Estados: Rental

```
????????????????????????????????????????????????????????????????
?                CICLO DE VIDA DE UN ALQUILER                  ?
????????????????????????????????????????????????????????????????

                    ???????????????
                    ?   Created   ?
                    ???????????????
                           ?
                           ? RentVehicleUseCase
                           ? (ReturnDate == null)
                           ?
                    ???????????????
                    ?   ACTIVE    ?
                    ? (En curso)  ?
                    ???????????????
                           ?
                           ? Posibles transiciones:
                           ?
                ???????????????????????
                ?                      ?
                ? ReturnVehicle       ? CancelRental
                ? UseCase              ? (Futuro)
                ?                      ?
         ???????????????        ???????????????
         ?  COMPLETED  ?        ?  CANCELLED  ?
         ? (Devuelto)  ?        ? (Cancelado) ?
         ???????????????        ???????????????
           (Estado Final)        (Estado Final)
```

---

## ?? API RESTful

### Base URL
```
Development:  http://localhost:5000/api
Production:   https://your-domain.com/api
```

### Swagger UI
```
Development:  http://localhost:5000/swagger
Production:   https://your-domain.com/swagger
```

### Endpoints Completos

#### Customers (Clientes)

| Método | Endpoint | Descripción | Body | Response |
|--------|----------|-------------|------|----------|
| **POST** | `/customers` | Crear nuevo cliente | CreateCustomerInput | 201 Created, 404 Not Found |
| **GET** | `/customers` | Obtener todos los clientes | - | 200 OK |
| **GET** | `/customers?hasActiveRental=true` | Filtrar clientes con alquiler activo | - | 200 OK |
| **GET** | `/customers?hasActiveRental=false` | Filtrar clientes sin alquiler | - | 200 OK |

**Ejemplo de Request (POST /customers):**
```http
POST /api/customers HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "name": "Juan Pérez García",
  "email": "juan.perez@example.com",
  "phoneNumber": "+34 600 123 456",
  "driverLicenseNumber": "12345678A"
}
```

**Ejemplo de Response (201 Created):**
```http
HTTP/1.1 201 Created
Content-Type: application/json
Location: /api/customers/507f1f77bcf86cd799439011

{
  "id": "507f1f77bcf86cd799439011",
  "name": "Juan Pérez García",
  "email": "juan.perez@example.com",
  "phoneNumber": "+34 600 123 456",
  "driverLicenseNumber": "12345678A",
  "hasActiveRental": false,
  "createdAt": "2024-02-01T10:30:00Z"
}
```

---

#### Vehicles (Vehículos)

| Método | Endpoint | Descripción | Body | Response |
|--------|----------|-------------|------|----------|
| **POST** | `/vehicles` | Crear nuevo vehículo | CreateVehicleInput | 201 Created, 404 Not Found |
| **GET** | `/vehicles?status=0` | Obtener vehículos disponibles | - | 200 OK |
| **GET** | `/vehicles?status=1` | Obtener vehículos alquilados | - | 200 OK |
| **GET** | `/vehicles?status=2` | Obtener vehículos retirados | - | 200 OK |

**Ejemplo de Request (POST /vehicles):**
```http
POST /api/vehicles HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "brand": "Toyota",
  "model": "Corolla",
  "year": 2023,
  "licensePlate": "1234-ABC",
  "kilometersDriven": 5000
}
```

**Ejemplo de Response (201 Created):**
```http
HTTP/1.1 201 Created
Content-Type: application/json
Location: /api/vehicles/507f1f77bcf86cd799439013

{
  "id": "507f1f77bcf86cd799439013",
  "brand": "Toyota",
  "model": "Corolla",
  "year": 2023,
  "licensePlate": "1234-ABC",
  "kilometersDriven": 5000,
  "status": "Available",
  "createdAt": "2024-02-01T12:00:00Z"
}
```

---

#### Rentals (Alquileres)

| Método | Endpoint | Descripción | Body | Response |
|--------|----------|-------------|------|----------|
| **POST** | `/rentals/rent` | Alquilar un vehículo | RentVehicleInput | 201 Created, 404 Not Found |
| **POST** | `/rentals/return` | Devolver un vehículo | ReturnVehicleInput | 200 OK, 404 Not Found |
| **GET** | `/rentals` | Obtener todos los alquileres | - | 200 OK |
| **GET** | `/rentals?status=0` | Filtrar alquileres activos | - | 200 OK |
| **GET** | `/rentals?status=1` | Filtrar alquileres completados | - | 200 OK |
| **GET** | `/rentals/by-license-plate/{licensePlate}` | Buscar alquiler por matrícula | - | 200 OK, 404 Not Found |

**Ejemplo de Request (POST /rentals/rent):**
```http
POST /api/rentals/rent HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "vehicleId": "507f1f77bcf86cd799439013",
  "customerId": "507f1f77bcf86cd799439011",
  "expectedReturnDate": "2024-02-08T10:00:00Z",
  "notes": "Cliente VIP"
}
```

**Ejemplo de Response (201 Created):**
```http
HTTP/1.1 201 Created
Content-Type: application/json
Location: /api/rentals/507f1f77bcf86cd799439020

{
  "rentalId": "507f1f77bcf86cd799439020",
  "vehicleId": "507f1f77bcf86cd799439013",
  "vehicleBrand": "Toyota",
  "vehicleModel": "Corolla",
  "vehicleLicensePlate": "1234-ABC",
  "customerId": "507f1f77bcf86cd799439011",
  "customerName": "Juan Pérez García",
  "rentalDate": "2024-02-01T14:30:00Z",
  "expectedReturnDate": "2024-02-08T10:00:00Z",
  "status": "Active",
  "notes": "Cliente VIP"
}
```

**Ejemplo de Request (POST /rentals/return):**
```http
POST /api/rentals/return HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "rentalId": "507f1f77bcf86cd799439020",
  "currentKilometers": 5500,
  "notes": "Todo correcto"
}
```

**Ejemplo de Request (GET /rentals/by-license-plate/{licensePlate}):**
```http
GET /api/rentals/by-license-plate/1234-ABC HTTP/1.1
Host: localhost:5000
```

---

### Códigos de Estado HTTP

| Código | Nombre | Uso en la API |
|--------|--------|---------------|
| **200 OK** | Éxito | GET requests exitosos, Return vehicle |
| **201 Created** | Creado | POST exitosos (Create, Rent) |
| **400 Bad Request** | Solicitud inválida | Validación de input fallida |
| **404 Not Found** | No encontrado | Entidad no existe, validación de negocio fallida |
| **500 Internal Server Error** | Error del servidor | Excepciones no manejadas |

---

## ?? Estrategia de Testing

### Pirámide de Testing

```
                    ???????????
                    ?   E2E   ?  ? Functional Tests (13 tests)
                    ???????????
                  ???????????????
                  ? Integration ?  ? Infrastructure Tests (6 tests)
                  ???????????????
              ?????????????????????
              ?    Unit Tests     ?  ? Unit Tests (18+ tests)
              ?????????????????????
```

### 1. Unit Tests (Tests Unitarios)

**Ubicación:** `test/unit/GtMotive.Estimate.Microservice.UnitTests/`

**Propósito:** Validar la lógica de negocio aislada de dependencias externas.

**Características:**
- ? **Muy rápidos** (< 100ms por test)
- ?? **Aislados** (todos los repositorios mockeados)
- ?? **Determin

ísticos** (mismo input = mismo output)
- ?? **Alta cobertura** (18+ tests implementados)

**Stack Tecnológico:**
- **xUnit 2.9.2**: Framework de testing
- **Moq 4.18.1**: Librería de mocking
- **Bogus 35.6.5**: Generación de datos de prueba realistas
- **FluentAssertions 7.0.0**: Assertions legibles y expresivas

**Estructura de Tests:**

```
ApplicationCore/
??? Fakers/
?   ??? EntityFakers.cs              # Generadores de datos con Bogus
?
??? UseCases/
?   ??? Customers/
?   ?   ??? CreateCustomerUseCaseTests.cs      # 3 tests
?   ?   ??? GetAllCustomersUseCaseTests.cs     # 1 test
?   ?
?   ??? Vehicles/
?   ?   ??? CreateVehicleUseCaseTests.cs       # 3 tests
?   ?   ??? GetVehiclesByStatusUseCaseTests.cs # 1 test
?   ?
?   ??? Rentals/
?       ??? RentVehicleUseCaseTests.cs         # 5 tests
?       ??? ReturnVehicleUseCaseTests.cs       # 3 tests
?       ??? GetAllRentalsUseCaseTests.cs       # 1 test
?       ??? GetRentalByLicensePlateUseCaseTests.cs # 1 test
```

**Cobertura de Tests:**

| Caso de Uso | Tests | Escenarios |
|-------------|-------|-----------|
| **CreateCustomerUseCase** | 3 | Creación exitosa, email duplicado, licencia duplicada |
| **GetAllCustomersUseCase** | 1 | Obtener todos con filtro |
| **CreateVehicleUseCase** | 3 | Creación exitosa, vehículo antiguo, matrícula duplicada |
| **GetVehiclesByStatusUseCase** | 1 | Filtrar por estado |
| **RentVehicleUseCase** | 5 | Alquiler exitoso, vehículo no disponible, cliente con alquiler, vehículo no elegible, fecha inválida |
| **ReturnVehicleUseCase** | 3 | Devolución exitosa, alquiler no activo, kilómetros inválidos |
| **GetAllRentalsUseCase** | 1 | Obtener todos con filtro |
| **GetRentalByLicensePlateUseCase** | 1 | Buscar por matrícula |
| **TOTAL** | **18** | **Cobertura completa de escenarios críticos** |

**Ejemplo de Test Unitario:**

```csharp
[Fact]
public async Task ExecuteAsync_WhenValidInput_ShouldCreateVehicle()
{
    // ARRANGE
    var faker = new EntityFakers();
    var vehicle = faker.VehicleFaker.Generate();
    
    var repositoryMock = new Mock<IVehicleRepository>();
    repositoryMock
        .Setup(x => x.GetByLicensePlateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((Vehicle)null);
    repositoryMock
        .Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);
    
    var outputPortMock = new Mock<ICreateVehicleOutputPort>();
    
    var sut = new CreateVehicleUseCase(outputPortMock.Object, repositoryMock.Object);
    
    var input = new CreateVehicleInput
    {
        Brand = vehicle.Brand,
        Model = vehicle.Model,
        Year = DateTime.UtcNow.Year - 2,
        LicensePlate = vehicle.LicensePlate,
        KilometersDriven = vehicle.KilometersDriven
    };
    
    // ACT
    await sut.ExecuteAsync(input, CancellationToken.None);
    
    // ASSERT
    outputPortMock.Verify(x => x.StandardHandle(It.IsAny<CreateVehicleOutput>()), Times.Once);
    repositoryMock.Verify(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

**Ejecución de Tests:**

```bash
# Todos los tests unitarios
cd test/unit/GtMotive.Estimate.Microservice.UnitTests/
dotnet test

# Tests específicos por categoría
dotnet test --filter "FullyQualifiedName~Customers"
dotnet test --filter "FullyQualifiedName~Vehicles"
dotnet test --filter "FullyQualifiedName~Rentals"

# Con verbose output
dotnet test --verbosity normal

# Con cobertura de código
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

---

### 2. Infrastructure Tests (Tests de Infraestructura)

**Ubicación:** `test/infrastructure/GtMotive.Estimate.Microservice.InfrastructureTests/`

**Propósito:** Validar la persistencia real contra MongoDB sin mockear la base de datos.

**Características:**
- ?? **Más lentos** (segundos)
- ??? **MongoDB real** (no mocks)
- ?? **Limpieza automática** entre tests
- ?? **Prueba Repositories** completos

**Cobertura:**

| Repository | Tests | Operaciones Probadas |
|------------|-------|---------------------|
| **VehicleRepository** | 2 | Add, GetById, GetByLicensePlate, GetByStatus |
| **CustomerRepository** | 2 | Add, GetById, GetByEmail, GetByDriverLicense, GetAll |
| **RentalRepository** | 2 | Add, GetById, GetByVehicleId, GetByCustomerId, GetAll |
| **TOTAL** | **6** | **Todas las operaciones CRUD** |

**Ejemplo de Test de Infraestructura:**

```csharp
[Fact]
public async Task AddAsync_ShouldPersistVehicleInDatabase()
{
    // ARRANGE
    var repository = GetService<IVehicleRepository>();
    var vehicle = new Vehicle
    {
        Id = Guid.NewGuid().ToString(),
        Brand = "Toyota",
        Model = "Corolla",
        Year = 2023,
        LicensePlate = $"TEST-{Guid.NewGuid().ToString()[..4]}",
        KilometersDriven = 5000,
        Status = VehicleStatus.Available,
        CreatedAt = DateTime.UtcNow
    };
    
    // ACT
    await repository.AddAsync(vehicle, CancellationToken.None);
    
    // ASSERT
    var retrieved = await repository.GetByIdAsync(vehicle.Id, CancellationToken.None);
    retrieved.Should().NotBeNull();
    retrieved.Id.Should().Be(vehicle.Id);
    retrieved.Brand.Should().Be("Toyota");
    retrieved.Status.Should().Be(VehicleStatus.Available);
}
```

**Ejecución:**

```bash
cd test/infrastructure/GtMotive.Estimate.Microservice.InfrastructureTests/
dotnet test

# Asegurarse de que MongoDB esté corriendo
docker run -d -p 27017:27017 --name mongodb-test mongo:latest
```

---

### 3. Functional Tests (Tests Funcionales / E2E)

**Ubicación:** `test/functional/GtMotive.Estimate.Microservice.FunctionalTests/`

**Propósito:** Validar flujos completos end-to-end con todos los componentes reales.

**Características:**
- ?? **Flujos completos** (múltiples UseCases encadenados)
- ??? **MongoDB real** (no mocks)
- ?? **UseCases reales** (no mocks)
- ?? **Test Output Ports** (capturan respuestas)

**Infraestructura de Tests:**

```
Infrastructure/
??? CompositionRootTestFixture.cs       # DI Container configurado
??? CompositionRootCollectionFixture.cs # xUnit Collection Fixture
??? FunctionalTestBase.cs               # Base class con limpieza de BD
??? TestCollections.cs                  # Definiciones de colecciones
```

**Cobertura de Tests:**

| Categoría | Tests | Flujos Probados |
|-----------|-------|-----------------|
| **Vehicles** | 4 | Crear vehículo, validar antigüedad, filtrar por estado, consulta vacía |
| **Customers** | 4 | Crear cliente, email duplicado, obtener todos, consulta vacía |
| **Rentals** | 5 | Alquilar, devolver, buscar por matrícula, obtener todos, validar regla de un alquiler |
| **TOTAL** | **13** | **Flujos E2E completos** |

**Ejemplo de Test Funcional:**

```csharp
[Fact]
public async Task CompleteRentalFlowShouldSucceed()
{
    // ARRANGE - Crear vehículo
    var vehicleInput = new CreateVehicleInput
    {
        Brand = "BMW",
        Model = "X5",
        Year = DateTime.UtcNow.Year - 2,
        LicensePlate = $"TEST-{Guid.NewGuid().ToString()[..4]}",
        KilometersDriven = 10000
    };
    
    var createVehicleOutputPort = new TestCreateVehicleOutputPort();
    var createVehicleUseCase = new CreateVehicleUseCase(
        createVehicleOutputPort, vehicleRepository);
    await createVehicleUseCase.ExecuteAsync(vehicleInput, CancellationToken.None);
    var vehicleId = createVehicleOutputPort.Output.Id;
    
    // ARRANGE - Crear cliente
    var customerInput = new CreateCustomerInput
    {
        Name = "Jane Smith",
        Email = $"jane.smith.{Guid.NewGuid().ToString()[..8]}@example.com",
        PhoneNumber = "+34600987654",
        DriverLicenseNumber = $"DL{Guid.NewGuid().ToString()[..8]}"
    };
    
    var createCustomerOutputPort = new TestCreateCustomerOutputPort();
    var createCustomerUseCase = new CreateCustomerUseCase(
        createCustomerOutputPort, customerRepository);
    await createCustomerUseCase.ExecuteAsync(customerInput, CancellationToken.None);
    var customerId = createCustomerOutputPort.Output.Id;
    
    // ACT - Alquilar vehículo
    var rentInput = new RentVehicleInput
    {
        VehicleId = vehicleId,
        CustomerId = customerId,
        ExpectedReturnDate = DateTime.UtcNow.AddDays(7),
        Notes = "Test rental"
    };
    
    var rentOutputPort = new TestRentVehicleOutputPort();
    var rentUseCase = new RentVehicleUseCase(
        rentOutputPort, vehicleRepository, customerRepository, rentalRepository);
    await rentUseCase.ExecuteAsync(rentInput, CancellationToken.None);
    
    // ASSERT - Verificar alquiler creado
    rentOutputPort.WasStandardHandled.Should().BeTrue();
    rentOutputPort.Output.Should().NotBeNull();
    rentOutputPort.Output.RentalId.Should().NotBeNullOrEmpty();
    
    // ASSERT - Verificar cambios de estado en BD
    var vehicle = await vehicleRepository.GetByIdAsync(vehicleId, CancellationToken.None);
    vehicle.Status.Should().Be(VehicleStatus.Rented);
    
    var customer = await customerRepository.GetByIdAsync(customerId, CancellationToken.None);
    customer.HasActiveRental.Should().BeTrue();
}
```

**Test Output Ports:**

Los Functional Tests usan **Test Output Ports** para capturar las respuestas de los UseCases:

```csharp
internal class TestRentVehicleOutputPort : IRentVehicleOutputPort
{
    public bool WasStandardHandled { get; private set; }
    public bool WasNotFoundHandled { get; private set; }
    public RentVehicleOutput Output { get; private set; }
    public string ErrorMessage { get; private set; }
    
    public void StandardHandle(RentVehicleOutput output)
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

**Ejecución:**

```bash
cd test/functional/GtMotive.Estimate.Microservice.FunctionalTests/

# Iniciar MongoDB
docker run -d -p 27017:27017 --name mongodb-functional mongo:latest

# Ejecutar tests
dotnet test

# Tests específicos
dotnet test --filter "FullyQualifiedName~Vehicle"
dotnet test --filter "FullyQualifiedName~Customer"
dotnet test --filter "FullyQualifiedName~Rental"
```

---

### Resumen de Testing

| Tipo de Test | Cantidad | Velocidad | Base de Datos | Dependencias Reales |
|--------------|----------|-----------|---------------|---------------------|
| **Unit Tests** | 18+ | ? < 100ms | ? Mockeada | ? Todas mockeadas |
| **Infrastructure Tests** | 6 | ?? Segundos | ? MongoDB real | ?? Solo Repositories |
| **Functional Tests** | 13 | ?? Segundos | ? MongoDB real | ? UseCases + Repositories |
| **TOTAL** | **37+** | - | - | - |

**Comando para ejecutar todos los tests:**

```bash
# Desde el directorio raíz de la solución
dotnet test

# Con output detallado
dotnet test --verbosity normal

# Con cobertura de código
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/

# Generar reporte HTML de cobertura
reportgenerator -reports:TestResults/coverage.cobertura.xml -targetdir:TestResults/html
```

---

## ?? Patrones de Diseño Aplicados

### 1. Clean Architecture (Arquitectura Limpia)

**Descripción:** Organización del código en capas concéntricas con dependencias dirigidas hacia el núcleo.

**Beneficios:**
- ? Independencia de frameworks
- ? Testeable en todos los niveles
- ? Independencia de UI y BD
- ? Reglas de negocio protegidas

**Implementación en el Proyecto:**
```
Domain (Núcleo) ? Application ? Infrastructure
                        ?
                       Api ? Host
```

---

### 2. Domain-Driven Design (DDD)

**Descripción:** Enfoque de desarrollo de software centrado en el dominio del negocio.

**Elementos Aplicados:**
- **Entities:** `Customer`, `Vehicle`, `Rental` (con identidad única)
- **Value Objects:** Enums como `VehicleStatus`, `RentalStatus`
- **Aggregates:** Cada entidad es su propio agregado
- **Domain Logic:** Métodos de negocio en las entidades (e.g., `IsEligibleForFleet()`)
- **Domain Exceptions:** `DomainException` para errores de negocio

---

### 3. Repository Pattern

**Descripción:** Abstracción de la lógica de acceso a datos.

**Implementación:**

```csharp
// Interfaz en Application Layer
public interface IVehicleRepository
{
    Task<Vehicle> GetByIdAsync(string id, CancellationToken ct);
    Task<Vehicle> GetByLicensePlateAsync(string licensePlate, CancellationToken ct);
    Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status, CancellationToken ct);
    Task AddAsync(Vehicle vehicle, CancellationToken ct);
    Task UpdateAsync(Vehicle vehicle, CancellationToken ct);
}

// Implementación en Infrastructure Layer
public sealed class VehicleRepository : IVehicleRepository
{
    private readonly IMongoCollection<Vehicle> _collection;
    
    public async Task<Vehicle> GetByIdAsync(string id, CancellationToken ct)
    {
        return await _collection
            .Find(v => v.Id == id)
            .FirstOrDefaultAsync(ct);
    }
    // ...
}
```

**Beneficios:**
- ? Desacoplamiento de la lógica de negocio y persistencia
- ? Facilita el testing (se puede mockear)
- ? Permite cambiar la BD sin afectar la lógica de negocio

---

### 4. Dependency Injection (Inyección de Dependencias)

**Descripción:** Patrón que permite invertir el control de las dependencias.

**Implementación:**

```csharp
// Registro en ApplicationConfiguration.cs
public static IServiceCollection AddApiDependencies(this IServiceCollection services)
{
    // Use Cases
    services.AddScoped<IUseCase<CreateVehicleInput>, CreateVehicleUseCase>();
    services.AddScoped<IUseCase<RentVehicleInput>, RentVehicleUseCase>();
    // ...
    
    return services;
}

// Registro en InfrastructureConfiguration.cs
public static IServiceCollection AddMongoDb(
    this IInfrastructureBuilder builder, 
    IConfiguration configuration)
{
    // Repositories
    services.AddScoped<IVehicleRepository, VehicleRepository>();
    services.AddScoped<ICustomerRepository, CustomerRepository>();
    services.AddScoped<IRentalRepository, RentalRepository>();
    
    return services;
}

// Uso en Controller
[HttpPost("rent")]
public async Task<IActionResult> RentVehicle(
    [FromBody] RentVehicleInput input,
    [FromServices] IUseCase<RentVehicleInput> useCase,  // ? Inyectado
    [FromServices] RentVehiclePresenter presenter)
{
    await useCase.ExecuteAsync(input, HttpContext.RequestAborted);
    return presenter.ViewModel;
}
```

---

### 5. Use Case Pattern (Command Pattern)

**Descripción:** Encapsula cada acción de negocio en una clase separada.

**Implementación:**

```csharp
// Interfaz genérica
public interface IUseCase<in TUseCaseInput> where TUseCaseInput : IUseCaseInput
{
    Task ExecuteAsync(TUseCaseInput input, CancellationToken ct);
}

// Implementación concreta
public sealed class RentVehicleUseCase : IUseCase<RentVehicleInput>
{
    private readonly IRentVehicleOutputPort _outputPort;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IRentalRepository _rentalRepository;
    
    public RentVehicleUseCase(
        IRentVehicleOutputPort outputPort,
        IVehicleRepository vehicleRepository,
        ICustomerRepository customerRepository,
        IRentalRepository rentalRepository)
    {
        _outputPort = outputPort;
        _vehicleRepository = vehicleRepository;
        _customerRepository = customerRepository;
        _rentalRepository = rentalRepository;
    }
    
    public async Task ExecuteAsync(RentVehicleInput input, CancellationToken ct)
    {
        // Lógica del caso de uso
    }
}
```

**Beneficios:**
- ? Single Responsibility Principle
- ? Fácil de testear unitariamente
- ? Fácil de extender (agregar nuevos casos de uso)

---

### 6. Output Port Pattern (Port-Adapter)

**Descripción:** Desacopla la lógica de negocio de la presentación mediante interfaces.

**Implementación:**

```csharp
// Output Port Interface (en Application Layer)
public interface IRentVehicleOutputPort : IOutputPortStandard<RentVehicleOutput>, 
                                          IOutputPortNotFound
{
}

// Presenter (en Api Layer)
public sealed class RentVehiclePresenter : IRentVehicleOutputPort, IWebApiPresenter
{
    public IActionResult ViewModel { get; private set; }
    
    public void StandardHandle(RentVehicleOutput output)
    {
        ViewModel = new CreatedAtActionResult(
            actionName: nameof(RentalsController.GetRentalById),
            controllerName: "Rentals",
            routeValues: new { id = output.RentalId },
            value: output);
    }
    
    public void NotFoundHandle(string message)
    {
        ViewModel = new NotFoundObjectResult(new { error = message });
    }
}

// Uso en UseCase
public async Task ExecuteAsync(RentVehicleInput input, CancellationToken ct)
{
    if (validationFails)
    {
        _outputPort.NotFoundHandle("Validation error message");
        return;
    }
    
    // ... lógica de negocio ...
    
    _outputPort.StandardHandle(new RentVehicleOutput { /* datos */ });
}
```

**Beneficios:**
- ? El UseCase no conoce ASP.NET ni HTTP
- ? Fácil de cambiar la presentación (Web API, gRPC, Console, etc.)
- ? Testeable sin dependencias de framework

---

### 7. CQRS (Command Query Responsibility Segregation)

**Descripción:** Separación implícita entre comandos (escritura) y consultas (lectura).

**Implementación:**

**Comandos (modifican estado):**
- `CreateVehicleUseCase`
- `CreateCustomerUseCase`
- `RentVehicleUseCase`
- `ReturnVehicleUseCase`

**Consultas (solo lectura):**
- `GetAllCustomersUseCase`
- `GetVehiclesByStatusUseCase`
- `GetAllRentalsUseCase`
- `GetRentalByLicensePlateUseCase`

**Beneficios:**
- ? Separación clara de responsabilidades
- ? Optimización independiente (índices de lectura vs escritura)
- ? Escalabilidad (réplicas de lectura vs maestro de escritura)

---

### 8. Specification Pattern (Implícito)

**Descripción:** Encapsula lógica de consulta compleja en métodos específicos.

**Implementación:**

```csharp
// En VehicleRepository
public async Task<IEnumerable<Vehicle>> GetByStatusAsync(
    VehicleStatus status, 
    CancellationToken ct)
{
    return await _collection
        .Find(v => v.Status == status)  // ? Specification implícita
        .ToListAsync(ct);
}

// En CustomerRepository
public async Task<Customer> GetByEmailAsync(string email, CancellationToken ct)
{
    return await _collection
        .Find(c => c.Email == email)  // ? Specification implícita
        .FirstOrDefaultAsync(ct);
}
```

---

### Resumen de Patrones

| Patrón | Capa | Beneficio Principal |
|--------|------|---------------------|
| **Clean Architecture** | Todas | Independencia de frameworks |
| **DDD** | Domain | Lógica de negocio protegida |
| **Repository Pattern** | Application/Infrastructure | Desacoplamiento de persistencia |
| **Dependency Injection** | Todas | Inversión de control |
| **Use Case Pattern** | Application | Single Responsibility |
| **Output Port Pattern** | Application/Api | Desacoplamiento de presentación |
| **CQRS** | Application | Separación lectura/escritura |
| **Specification Pattern** | Infrastructure | Encapsulación de consultas |

---

## ?? Configuración y Despliegue

### Requisitos Previos

```yaml
.NET SDK: 9.0.308 o superior
Docker Desktop: Latest version
MongoDB: 8.0+ (vía Docker o instalación local)
Git: Para clonar el repositorio
```

### Configuración Local (Sin Docker)

#### 1. Clonar el Repositorio

```bash
git clone https://github.com/AguileraMG/PruebaTecnicaRental.git
cd PruebaTecnicaRental/src
```

#### 2. Restaurar Dependencias

```bash
dotnet restore
```

#### 3. Configurar MongoDB

**Opción A: MongoDB vía Docker (Recomendado)**
```bash
docker run -d \
  --name mongodb-rental \
  -p 27017:27017 \
  -v mongodb_data:/data/db \
  mongo:latest
```

**Opción B: MongoDB Local**
- Descargar e instalar MongoDB Community Edition
- Iniciar servicio: `mongod --dbpath /path/to/data`

#### 4. Configurar `appsettings.Development.json`

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "GtMotiveRental"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### 5. Compilar la Solución

```bash
dotnet build
```

#### 6. Ejecutar la Aplicación

```bash
cd GtMotive.Estimate.Microservice.Host
dotnet run
```

**Output esperado:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

#### 7. Verificar la API

**Swagger UI:**
```
http://localhost:5000/swagger
```

**Health Check:**
```bash
curl http://localhost:5000/api/health
```

---

### Despliegue con Docker Compose (Recomendado)

#### Archivo `docker-compose.yml`

```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: GtMotive.Estimate.Microservice.Host/Dockerfile
    image: gtmotive-rental-api:latest
    container_name: gtmotive-rental-api
    ports:
      - "5000:80"
      - "5001:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - MongoDb__ConnectionString=mongodb://mongodb:27017
      - MongoDb__DatabaseName=GtMotiveRental
    depends_on:
      - mongodb
    networks:
      - rental-network
    restart: unless-stopped

  mongodb:
    image: mongo:latest
    container_name: mongodb-rental
    ports:
      - "27017:27017"
    volumes:
      - mongodb_data:/data/db
    networks:
      - rental-network
    restart: unless-stopped

volumes:
  mongodb_data:
    driver: local

networks:
  rental-network:
    driver: bridge
```

#### Comandos Docker Compose

**1. Construir y levantar servicios:**
```bash
docker-compose up -d --build
```

**2. Ver logs:**
```bash
# Todos los servicios
docker-compose logs -f

# Solo API
docker-compose logs -f api

# Solo MongoDB
docker-compose logs -f mongodb
```

**3. Ver estado de servicios:**
```bash
docker-compose ps
```

**Salida esperada:**
```
NAME                    IMAGE                           STATUS
gtmotive-rental-api     gtmotive-rental-api:latest     Up 30 seconds
mongodb-rental          mongo:latest                    Up 30 seconds
```

**4. Detener servicios:**
```bash
docker-compose stop
```

**5. Detener y eliminar contenedores:**
```bash
docker-compose down
```

**6. Detener y eliminar contenedores + volúmenes:**
```bash
docker-compose down -v
```

---

### Dockerfile

```dockerfile
# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["GtMotive.Estimate.Microservice.Host/*.csproj", "GtMotive.Estimate.Microservice.Host/"]
COPY ["GtMotive.Estimate.Microservice.Api/*.csproj", "GtMotive.Estimate.Microservice.Api/"]
COPY ["GtMotive.Estimate.Microservice.ApplicationCore/*.csproj", "GtMotive.Estimate.Microservice.ApplicationCore/"]
COPY ["GtMotive.Estimate.Microservice.Domain/*.csproj", "GtMotive.Estimate.Microservice.Domain/"]
COPY ["GtMotive.Estimate.Microservice.Infrastructure/*.csproj", "GtMotive.Estimate.Microservice.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "GtMotive.Estimate.Microservice.Host/GtMotive.Estimate.Microservice.Host.csproj"

# Copy all source files
COPY . .

# Build
WORKDIR "/src/GtMotive.Estimate.Microservice.Host"
RUN dotnet build "GtMotive.Estimate.Microservice.Host.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "GtMotive.Estimate.Microservice.Host.csproj" -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GtMotive.Estimate.Microservice.Host.dll"]
```

---

### Variables de Entorno

| Variable | Descripción | Valor por Defecto | Requerido |
|----------|-------------|-------------------|-----------|
| `ASPNETCORE_ENVIRONMENT` | Entorno de ejecución | Development | No |
| `MongoDb__ConnectionString` | Cadena de conexión MongoDB | mongodb://localhost:27017 | Sí |
| `MongoDb__DatabaseName` | Nombre de la base de datos | GtMotiveRental | Sí |
| `ASPNETCORE_URLS` | URLs de escucha | http://+:80;https://+:443 | No |
| `Logging__LogLevel__Default` | Nivel de log por defecto | Information | No |

**Ejemplo de configuración con variables de entorno:**

```bash
export ASPNETCORE_ENVIRONMENT=Production
export MongoDb__ConnectionString=mongodb://prod-server:27017
export MongoDb__DatabaseName=GtMotiveRentalProd
dotnet run --project GtMotive.Estimate.Microservice.Host
```

---

### Verificación del Despliegue

#### 1. Health Check Endpoint

```bash
curl http://localhost:5000/api/health
```

**Respuesta esperada:**
```json
{
  "status": "Healthy",
  "timestamp": "2024-02-01T15:30:00Z"
}
```

#### 2. Swagger UI

Abrir navegador en:
```
http://localhost:5000/swagger
```

#### 3. Test de API

**Crear un vehículo:**
```bash
curl -X POST http://localhost:5000/api/vehicles \
  -H "Content-Type: application/json" \
  -d '{
    "brand": "Toyota",
    "model": "Corolla",
    "year": 2023,
    "licensePlate": "1234-ABC",
    "kilometersDriven": 5000
  }'
```

**Obtener vehículos disponibles:**
```bash
curl http://localhost:5000/api/vehicles?status=0
```

#### 4. Verificar MongoDB

**Conectarse a MongoDB:**
```bash
# Si usas Docker Compose
docker exec -it mongodb-rental mongosh

# Si usas MongoDB local
mongosh mongodb://localhost:27017
```

**Comandos MongoDB:**
```javascript
// Mostrar bases de datos
show dbs

// Usar base de datos
use GtMotiveRental

// Mostrar colecciones
show collections

// Consultar vehículos
db.Vehicles.find().pretty()

// Contar documentos
db.Vehicles.countDocuments()
db.Customers.countDocuments()
db.Rentals.countDocuments()
```

---

## ?? Métricas de Calidad

### Cobertura de Código

```bash
# Generar cobertura
dotnet test /p:CollectCoverage=true \
  /p:CoverletOutputFormat=cobertura \
  /p:Exclude="[*.Tests]*"

# Generar reporte HTML
reportgenerator \
  -reports:test/**/coverage.cobertura.xml \
  -targetdir:TestResults/html \
  -reporttypes:Html
```

**Métricas Actuales:**

| Capa | Cobertura Estimada | Tests |
|------|-------------------|-------|
| **Domain** | ~90% | Tests unitarios |
| **Application** | ~85% | Tests unitarios + funcionales |
| **Infrastructure** | ~75% | Tests de infraestructura |
| **Api** | ~70% | Tests funcionales |

### Complejidad Ciclomática

**Herramientas utilizadas:**
- Visual Studio Code Metrics
- SonarQube (futuro)

**Objetivos:**
- Complejidad ciclomática < 10 por método
- Profundidad de herencia < 5
- Líneas por método < 50

### Deuda Técnica

**Estado Actual:**
- ? Sin warnings de compilación
- ? Sin code smells críticos
- ? Sin dependencias obsoletas
- ?? Algunos métodos largos en UseCases (refactor futuro)

### Métricas de Testing

| Métrica | Valor |
|---------|-------|
| **Total de Tests** | 37+ |
| **Tests Unitarios** | 18 |
| **Tests de Infraestructura** | 6 |
| **Tests Funcionales** | 13 |
| **Tests Pasando** | 100% |
| **Tiempo de Ejecución (Unit)** | ~2 segundos |
| **Tiempo de Ejecución (Functional)** | ~10 segundos |

---

## ?? Conclusión

Esta solución representa una implementación **profesional y completa** de un sistema de alquiler de vehículos, aplicando las **mejores prácticas de la industria** en:

? **Arquitectura:** Clean Architecture para máxima mantenibilidad  
? **Diseño:** DDD para capturar la complejidad del dominio  
? **Calidad:** 37+ tests con cobertura exhaustiva  
? **Tecnología:** .NET 9 + MongoDB como stack moderno  
? **Despliegue:** Docker Compose para facilitar la ejecución  
? **Documentación:** Completa y detallada  

### Puntos Destacados

1. **Separación de Responsabilidades:** Cada capa tiene un propósito claro
2. **Reglas de Negocio Protegidas:** Lógica en el dominio, no en controladores
3. **Testeable:** Arquitectura diseñada para facilitar el testing
4. **Escalable:** Preparado para crecer y evolucionar
5. **Mantenible:** Código limpio y bien organizado

### Posibles Mejoras Futuras

- ?? Implementar **CQRS** explícito con bases de datos separadas
- ?? Agregar **eventos de dominio** para desacoplamiento
- ?? Implementar **autenticación y autorización** (OAuth2/JWT)
- ?? Agregar **telemetría avanzada** con Application Insights
- ?? Implementar **API Gateway** para microservicios
- ?? Agregar **cache distribuido** (Redis)
- ?? Implementar **rate limiting** y **throttling**
- ?? Agregar **circuit breaker** con Polly
- ?? Implementar **audit logging** completo
- ?? Agregar **tests de carga** (JMeter, k6)

---

**Fecha de Creación:** 01 de Febrero de 2024  
**Autor:** Manuel García ([@AguileraMG](https://github.com/AguileraMG))  
**Proyecto:** Prueba Técnica - GtMotive Vehicle Rental System  
**Versión:** 1.0.0  

---

*Documento generado automáticamente por análisis completo de la solución.*
