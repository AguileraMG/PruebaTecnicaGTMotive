# 🚗 GtMotive Rental Microservice

[![.NET Version](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![MongoDB](https://img.shields.io/badge/MongoDB-7.0-47A248?logo=mongodb)](https://www.mongodb.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)

> Sistema de gestión de alquileres de vehículos construido con Clean Architecture y .NET 9

---

## 📋 Tabla de Contenidos

- [Descripción General](#-descripción-general)
- [Características](#-características)
- [Arquitectura](#-arquitectura)
- [Tecnologías](#-tecnologías)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación y Configuración](#-instalación-y-configuración)
- [Uso](#-uso)
- [Testing](#-testing)
- [API Endpoints](#-api-endpoints)
- [Dockerización](#-dockerización)
- [Configuración de Producción](#-configuración-de-producción)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Contribución](#-contribución)

---

## 🎯 Descripción General

**GtMotive Rental Microservice** es un sistema completo de gestión de alquileres de flotas de vehículos diseñado con principios de Clean Architecture. El sistema permite:

- 🚙 **Gestión de Vehículos**: Registro, consulta y administración de flotas
- 👤 **Gestión de Clientes**: Registro y validación de clientes con licencias de conducir
- 📝 **Gestión de Alquileres**: Proceso completo de alquiler y devolución de vehículos
- 🔍 **Consultas Avanzadas**: Filtrado por estado, matrícula, cliente, etc.
- ✅ **Validaciones de Negocio**: Reglas de dominio aplicadas en tiempo real

### Reglas de Negocio Principales

- ✅ Los vehículos deben tener máximo 5 años de antigüedad
- ✅ Un cliente solo puede tener un alquiler activo a la vez
- ✅ Los emails y números de licencia deben ser únicos
- ✅ Las matrículas de vehículos deben ser únicas
- ✅ Los vehículos solo pueden ser alquilados si están disponibles

---

## ✨ Características

### Funcionalidades Principales

- 📝 **Logging Estructurado**: Serilog integrado
- 📖 **Documentación API**: Swagger/OpenAPI integrado
- 🐳 **Dockerizado**: Listo para deploy en contenedores

### Características Técnicas

- ✅ **Clean Architecture**: Separación clara de responsabilidades
- ✅ **Domain-Driven Design**: Lógica de negocio en el dominio
- ✅ **CQRS Pattern**: Separación entre comandos y consultas
- ✅ **Repository Pattern**: Abstracción de persistencia
- ✅ **Rich Domain Model**: Entidades con comportamiento
- ✅ **Value Objects**: Inmutabilidad y validación
- ✅ **Factory Pattern**: Creación controlada de entidades

---

## 🏗️ Arquitectura

El proyecto sigue **Clean Architecture** (también conocida como Onion Architecture), con capas concéntricas de dependencias:

```
┌──────────────────────────────────────────────────┐
│          Presentation Layer (API)                │
│        Controllers, Presenters, DTOs             │
└──────────────────────────────────────────────────┘
                      ↓
┌──────────────────────────────────────────────────┐
│         Application Layer (Use Cases)            │
│     Business Logic, Orchestration, Ports         │
└──────────────────────────────────────────────────┘
                      ↓
┌──────────────────────────────────────────────────┐
│            Domain Layer (Entities)               │
│      Business Rules, Domain Events, VOs          │
└──────────────────────────────────────────────────┘
                      ↓
┌──────────────────────────────────────────────────┐
│     Infrastructure Layer (Persistence)           │
│    Repositories, MongoDB, External Services      │
└──────────────────────────────────────────────────┘
```

### Principios Aplicados

- **SOLID Principles**: Diseño orientado a principios
- **Dependency Inversion**: Las capas internas no dependen de las externas
- **Single Responsibility**: Cada clase tiene una única razón para cambiar
- **Open/Closed**: Abierto para extensión, cerrado para modificación
- **Interface Segregation**: Interfaces específicas y pequeñas

---

## 🛠️ Tecnologías

### Framework y Lenguaje

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| **.NET** | 9.0 | Framework principal |
| **C#** | 13.0 | Lenguaje de programación |
| **ASP.NET Core** | 9.0 | Web API framework |

### Base de Datos

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| **MongoDB** | 7.0+ | Base de datos NoSQL |
| **MongoDB.Driver** | 2.19.0 | Cliente .NET para MongoDB |

### Testing

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| **xUnit** | 2.9.2 | Framework de testing |
| **Moq** | 4.18.1 | Mocking library |
| **Bogus** | 35.6.5 | Generación de datos fake |
| **FluentAssertions** | 7.0.0 | Assertions fluidas |
| **coverlet** | - | Cobertura de código |

### Infraestructura y DevOps

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| **Docker** | Latest | Containerización |
| **Docker Compose** | Latest | Orquestación local |
| **Azure Key Vault** | Latest | Gestión de secretos |
| **Application Insights** | Latest | Telemetría y monitoring |

### Librerías Adicionales

| Tecnología | Propósito |
|-----------|-----------|
| **Serilog** | Logging estructurado |
| **Swashbuckle** | Documentación OpenAPI/Swagger |
---

## 📦 Requisitos Previos

### Software Requerido

- ✅ [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (versión 9.0.0 o superior)
- ✅ [Docker Desktop](https://www.docker.com/products/docker-desktop/) (para MongoDB y ejecución containerizada)
- ✅ [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.8+) o [VS Code](https://code.visualstudio.com/)
- ✅ [Git](https://git-scm.com/)

### Verificar Instalaciones

```bash
# Verificar .NET
dotnet --version
# Debe mostrar: 9.0.xxx

# Verificar Docker
docker --version
# Debe mostrar: Docker version 20.10.x o superior

# Verificar Docker Compose
docker-compose --version
# Debe mostrar: Docker Compose version 2.x.x o superior
```

---

## 🚀 Instalación y Configuración

### 1️⃣ Clonar el Repositorio

```bash
git clone https://github.com/AguileraMG/PruebaTecnicaGTMotive.git
cd PruebaTecnicaGTMotive
```

### 2️⃣ Configuración Local (sin Docker)

#### Paso 1: Iniciar MongoDB

```bash
docker run -d \
  --name mongodb-rental \
  -p 27017:27017 \
  mongo:7.0
```

#### Paso 2: Configurar appsettings

Edita `src/GtMotive.Estimate.Microservice.Host/appsettings.Development.json`:

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "EstimateDb"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### Paso 3: Restaurar paquetes y compilar

```bash
cd src
dotnet restore
dotnet build
```

#### Paso 4: Ejecutar la aplicación

```bash
cd GtMotive.Estimate.Microservice.Host
dotnet run
```

La API estará disponible en:
- 🌐 **HTTP**: http://localhost:7000
- 🔒 **HTTPS**: https://localhost:7001
- 📖 **Swagger**: http://localhost:7000/swagger

---

### 3️⃣ Configuración con Docker Compose (Recomendado)

#### Opción A: Desde Visual Studio 2022

1. Abrir `PruebaTecnica.sln` en Visual Studio 2022
2. En el menú desplegable de proyectos de inicio, seleccionar **"Docker Compose"**
3. Presionar **F5** o hacer clic en el botón ▶️
4. Visual Studio construirá las imágenes y ejecutará los contenedores automáticamente
5. Se abrirá el navegador en `http://localhost:5000/swagger`

#### Opción B: Desde la Terminal

```bash
# Desde la raíz del proyecto
docker-compose up -d

# Ver logs
docker-compose logs -f

# Verificar que los servicios están corriendo
docker-compose ps
```

La API estará disponible en:
- 🌐 **HTTP**: http://localhost:5000
- 📖 **Swagger**: http://localhost:5000/swagger
- 🗄️ **MongoDB**: localhost:27017

#### Detener los servicios

```bash
docker-compose down

# Detener y eliminar volúmenes (borra los datos)
docker-compose down -v
```

---

## 💻 Uso

### Acceder a la Documentación Swagger

Una vez que la aplicación esté corriendo, accede a:

```
http://localhost:5000/swagger
```

o

```
http://localhost:7000/swagger
```

Dependiendo de cómo hayas iniciado la aplicación.

### Flujo de Uso Típico

#### 1. Crear un Cliente

```bash
POST /api/customers
Content-Type: application/json

{
  "name": "Juan Pérez",
  "email": "juan.perez@example.com",
  "phoneNumber": "+34600123456",
  "driverLicenseNumber": "B12345678"
}
```

#### 2. Crear un Vehículo

```bash
POST /api/vehicles
Content-Type: application/json

{
  "brand": "Toyota",
  "model": "Camry",
  "year": 2022,
  "licensePlate": "ABC-1234",
  "kilometersDriven": 50000
}
```

#### 3. Alquilar un Vehículo

```bash
POST /api/rentals/rent
Content-Type: application/json

{
  "vehicleId": "vehicle-id-here",
  "customerId": "customer-id-here",
  "expectedReturnDate": "2024-12-31T23:59:59Z",
  "notes": "Alquiler de fin de semana"
}
```

#### 4. Consultar Alquiler por Matrícula

```bash
GET /api/rentals/by-license-plate/ABC-1234
```

#### 5. Devolver un Vehículo

```bash
POST /api/rentals/return
Content-Type: application/json

{
  "rentalId": "rental-id-here",
  "finalKilometers": 50500,
  "returnNotes": "Vehículo devuelto en buen estado"
}
```

---

## 🧪 Testing

El proyecto incluye **3 tipos de tests** con alta cobertura:

### Tipos de Tests

| Tipo | Proyecto | Tests | Cobertura |
|------|----------|-------|-----------|
| **Unit Tests** | `GtMotive.Estimate.Microservice.UnitTests` | 18 tests | Lógica de negocio |
| **Infrastructure Tests** | `GtMotive.Estimate.Microservice.InfrastructureTests` | 18 tests | Repositorios + MongoDB |
| **Functional Tests** | `GtMotive.Estimate.Microservice.FunctionalTests` | 13 tests | Flujos E2E completos |
| **TOTAL** | - | **49 tests** | **Alta cobertura** |

### 1️⃣ Unit Tests

Prueban la lógica de negocio de forma aislada usando mocks.

**Ejecutar:**

```bash
cd test/unit/GtMotive.Estimate.Microservice.UnitTests
dotnet test
```

**Cobertura:**
- ✅ 8 UseCases testeados
- ✅ Validaciones de dominio
- ✅ Reglas de negocio
- ✅ Casos edge y errores

**Tecnologías:**
- xUnit para estructura de tests
- Moq para mocking de repositorios
- Bogus para generación de datos fake
- FluentAssertions para assertions

### 2️⃣ Infrastructure Tests

Prueban la capa de persistencia con MongoDB real.

**Prerequisito:** MongoDB debe estar corriendo en `localhost:27017`

```bash
# Iniciar MongoDB
docker run -d -p 27017:27017 --name mongodb-test mongo:7.0

# Ejecutar tests
cd test/infrastructure/GtMotive.Estimate.Microservice.InfrastructureTests
dotnet test
```

**Cobertura:**
- ✅ CRUD completo de Vehicles, Customers, Rentals
- ✅ Queries complejas (filtros, búsquedas)
- ✅ Actualización de estados
- ✅ Relaciones entre entidades

### 3️⃣ Functional Tests (E2E)

Prueban flujos completos de negocio usando componentes reales.

**Prerequisito:** MongoDB debe estar corriendo en `localhost:27017`

```bash
# Iniciar MongoDB
docker run -d -p 27017:27017 --name mongodb-functional mongo:7.0

# Ejecutar tests
cd test/functional/GtMotive.Estimate.Microservice.FunctionalTests
dotnet test
```

**Cobertura:**
- ✅ Flujo completo de alquiler (crear vehículo → crear cliente → alquilar)
- ✅ Flujo de devolución de vehículo
- ✅ Validaciones de reglas de negocio
- ✅ Casos de error (conflictos, no encontrados)

### Ejecutar Todos los Tests

```bash
# Desde la raíz del proyecto
dotnet test

# Con cobertura de código
dotnet test --collect:"XPlat Code Coverage"
```

### Visualizar Resultados en Visual Studio

1. Abrir **Test Explorer**: `Test` → `Test Explorer` (Ctrl + E, T)
2. Hacer clic en **Run All** para ejecutar todos los tests
3. Ver resultados en tiempo real con indicadores ✅/❌

---

## 🔌 API Endpoints

### Customers (Clientes)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/customers` | Crear nuevo cliente |
| `GET` | `/api/customers` | Obtener todos los clientes |

### Vehicles (Vehículos)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/vehicles` | Crear nuevo vehículo |
| `GET` | `/api/vehicles/by-status/{status}` | Obtener vehículos por estado |

**Estados de Vehículo:**
- `Available` - Disponible para alquiler
- `Rented` - Actualmente alquilado
- `Maintenance` - En mantenimiento
- `Retired` - Retirado de la flota

### Rentals (Alquileres)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/rentals/rent` | Alquilar un vehículo |
| `POST` | `/api/rentals/return` | Devolver un vehículo |
| `GET` | `/api/rentals` | Obtener todos los alquileres |
| `GET` | `/api/rentals/by-license-plate/{plate}` | Obtener alquiler por matrícula |

---

## 🐳 Dockerización

### Dockerfile Multi-Stage

El proyecto incluye un Dockerfile optimizado con multi-stage build:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
FROM build AS publish
FROM base AS final
```

**Características:**
- ✅ Imágenes oficiales de Microsoft
- ✅ Multi-stage build (imagen final más pequeña)
- ✅ Soporte para Azure DevOps Packages
- ✅ Optimización de capas para mejor caché

### Docker Compose

El archivo `docker-compose.yml` orquesta la aplicación completa:

```yaml
services:
  gtmotive.estimate.microservice.host:
    # API Service
    ports:
      - "5000:8080"
    depends_on:
      - mongodb
    
  mongodb:
    # MongoDB Service
    image: mongo:7.0
    ports:
      - "27017:27017"
    volumes:
      - mongodb-data:/data/db
```

**Características:**
- ✅ API + MongoDB orquestados
- ✅ Volúmenes para persistencia de datos
- ✅ Red aislada entre servicios
- ✅ Variables de entorno configurables
- ✅ Soporte para Visual Studio 2022

### Comandos Docker Útiles

```bash
# Construir imagen
docker build -t gtmotive-rental-api -f GtMotive.Estimate.Microservice.Host/Dockerfile .

# Ejecutar contenedor
docker run -d -p 5000:8080 \
  -e MongoDb__ConnectionString=mongodb://host.docker.internal:27017 \
  -e MongoDb__DatabaseName=EstimateDb \
  --name rental-api \
  gtmotive-rental-api

# Ver logs
docker logs -f rental-api

# Detener y eliminar
docker stop rental-api
docker rm rental-api
```


## 📁 Estructura del Proyecto

```
PruebaTecnicaGTMotive/
│
├── src/                                           # Código fuente
│   ├── GtMotive.Estimate.Microservice.Domain/    # Capa de Dominio
│   │   └── Entities/                             # Entidades de dominio
│   │       ├── Customer.cs                       # 👤 Cliente
│   │       ├── Vehicle.cs                        # 🚗 Vehículo
│   │       ├── Rental.cs                         # 📝 Alquiler
│   │       └── DomainException.cs                # ⚠️ Excepciones
│   │
│   ├── GtMotive.Estimate.Microservice.ApplicationCore/  # Lógica de aplicación
│   │   ├── UseCases/                             # Casos de uso
│   │   │   ├── Customers/                        # 👤 Gestión de clientes
│   │   │   ├── Vehicles/                         # 🚗 Gestión de vehículos
│   │   │   └── Rentals/                          # 📝 Gestión de alquileres
│   │   └── Repositories/                         # Interfaces de repositorios
│   │
│   ├── GtMotive.Estimate.Microservice.Infrastructure/  # Infraestructura
│   │   ├── Repositories/                         # Implementación de repositorios
│   │   ├── MongoDb/                              # Configuración MongoDB
│   │   └── DependencyInjection/                  # Registro de servicios
│   │
│   ├── GtMotive.Estimate.Microservice.Api/       # Capa de presentación
│   │   ├── Controllers/                          # 🔌 API Controllers
│   │   ├── UseCases/                             # 🎨 Presenters
│   │   ├── Filters/                              # 🛡️ Exception filters
│   │   └── Authorization/                        # 🔐 Políticas de auth
│   │
│   └── GtMotive.Estimate.Microservice.Host/      # Host de la aplicación
│       ├── Program.cs                            # 🚀 Entry point
│       ├── Dockerfile                            # 🐳 Docker config
│       ├── appsettings.json                      # ⚙️ Configuración
│       └── DependencyInjection/                  # 📦 DI Configuration
│
├── test/                                          # Tests
│   ├── unit/                                      # Unit Tests
│   │   └── GtMotive.Estimate.Microservice.UnitTests/
│   │       ├── ApplicationCore/                   # Tests de UseCases
│   │       └── Fakers/                            # Generadores de datos
│   │
│   ├── infrastructure/                            # Infrastructure Tests
│   │   └── GtMotive.Estimate.Microservice.InfrastructureTests/
│   │       └── Repositories/                      # Tests de repositorios
│   │
│   └── functional/                                # Functional Tests (E2E)
│       └── GtMotive.Estimate.Microservice.FunctionalTests/
│           ├── Customers/                         # Tests de clientes
│           ├── Vehicles/                          # Tests de vehículos
│           └── Rentals/                           # Tests de alquileres
│
├── docker-compose.yml                             # 🐳 Orquestación Docker
├── docker-compose.override.yml                    # 🐳 Overrides locales
├── launchSettings.json                            # 🚀 Perfil Docker Compose
├── .dockerignore                                  # 🚫 Exclusiones Docker
├── Directory.Build.props                          # 📦 Props compartidas
├── Directory.Build.targets                        # 🎯 Targets compartidos
├── global.json                                    # 🌐 Versión del SDK
└── README.md                                      # 📖 Este archivo
```

### Convenciones de Nombres

- **Entidades**: PascalCase (ej: `Customer`, `Vehicle`)
- **UseCases**: PascalCase + Sufijo (ej: `CreateCustomerUseCase`)
- **Repositorios**: I + PascalCase + Repository (ej: `ICustomerRepository`)
- **Output Ports**: I + PascalCase + OutputPort (ej: `ICreateCustomerOutputPort`)
- **Tests**: PascalCase + Tests (ej: `CreateCustomerUseCaseTests`)

---


## 📊 Estadísticas del Proyecto

- **Líneas de Código**: ~15,000
- **Tests**: 49 (Unit + Infrastructure + Functional)
- **Cobertura**: >85%
- **Proyectos**: 8 (.NET Solutions)
- **Tecnologías**: 20+ (librerías y frameworks)

