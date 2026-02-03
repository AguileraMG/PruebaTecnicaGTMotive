# ?? Correcciones de Functional Tests - Documento de Referencia

## ? Errores Identificados y ? Correcciones

### **1. Nombres de Servicios (UseCases)**

Los UseCases **NO tienen interfaces** - se inyectan directamente por su clase.

| ? Incorrecto | ? Correcto |
|--------------|------------|
| `ICreateVehicleUseCase` | `CreateVehicleUseCase` |
| `ICreateCustomerUseCase` | `CreateCustomerUseCase` |
| `IGetVehiclesByStatusUseCase` | `GetVehiclesByStatusUseCase` |
| `IGetAllCustomersUseCase` | `GetAllCustomersUseCase` |
| `IRentVehicleUseCase` | `RentVehicleUseCase` |
| `IReturnVehicleUseCase` | `ReturnVehicleUseCase` |
| `IGetRentalByLicensePlateUseCase` | `GetRentalByLicensePlateUseCase` |
| `IGetAllRentalsUseCase` | `GetAllRentalsUseCase` |

---

### **2. Propiedades de Outputs**

#### CreateVehicleOutput
| ? Incorrecto | ? Correcto |
|--------------|------------|
| `Output.VehicleId` | `Output.Id` |

#### CreateCustomerOutput
| ? Incorrecto | ? Correcto |
|--------------|------------|
| `Output.CustomerId` | `Output.Id` |

#### GetRentalByLicensePlateOutput
| ? Incorrecto | ? Correcto |
|--------------|------------|
| `Output.CustomerId` | ? **No existe** |
| `Output.LicensePlate` | `Output.VehicleLicensePlate` |

---

### **3. ReturnVehicleInput - Propiedades Reales**

```csharp
public sealed class ReturnVehicleInput
{
    public string RentalId { get; set; }
    public DateTime ReturnDate { get; set; }        // No ActualReturnDate
    public int CurrentKilometers { get; set; }      // No FinalKilometers
    public string Notes { get; set; }
}
```

| ? Incorrecto | ? Correcto |
|--------------|------------|
| `ActualReturnDate` | `ReturnDate` |
| `FinalKilometers` | `CurrentKilometers` |

---

## ?? Cambios a Aplicar por Archivo

### **VehicleFunctionalTests.cs**

```csharp
// Línea ~45: Cambiar
var useCase = scope.ServiceProvider.GetRequiredService<ICreateVehicleUseCase>();
// Por:
var useCase = scope.ServiceProvider.GetRequiredService<CreateVehicleUseCase>();

// Línea ~57: Cambiar
var vehicleId = createVehicleOutputPort.Output.VehicleId;
// Por:
var vehicleId = createVehicleOutputPort.Output.Id;

// Línea ~102: Cambiar
var useCase = scope.ServiceProvider.GetRequiredService<IGetVehiclesByStatusUseCase>();
// Por:
var useCase = scope.ServiceProvider.GetRequiredService<GetVehiclesByStatusUseCase>();
```

### **CustomerFunctionalTests.cs**

```csharp
// Línea ~45: Cambiar
var useCase = scope.ServiceProvider.GetRequiredService<ICreateCustomerUseCase>();
// Por:
var useCase = scope.ServiceProvider.GetRequiredService<CreateCustomerUseCase>();

// Línea ~54: Cambiar
outputPort.Output.CustomerId.Should().NotBeNullOrEmpty();
// Por:
outputPort.Output.Id.Should().NotBeNullOrEmpty();

// Línea ~61: Cambiar
var savedCustomer = await repository.GetByIdAsync(outputPort.Output.CustomerId, CancellationToken.None);
// Por:
var savedCustomer = await repository.GetByIdAsync(outputPort.Output.Id, CancellationToken.None);

// Línea ~134: Cambiar
var getAllUseCase = scope.ServiceProvider.GetRequiredService<IGetAllCustomersUseCase>();
// Por:
var getAllUseCase = scope.ServiceProvider.GetRequiredService<GetAllCustomersUseCase>();

// Línea ~155: Cambiar
var useCase = scope.ServiceProvider.GetRequiredService<IGetAllCustomersUseCase>();
// Por:
var useCase = scope.ServiceProvider.GetRequiredService<GetAllCustomersUseCase>();
```

### **RentalFunctionalTests.cs**

```csharp
// Línea ~52: Cambiar
var createVehicleUseCase = scope.ServiceProvider.GetRequiredService<ICreateVehicleUseCase>();
// Por:
var createVehicleUseCase = scope.ServiceProvider.GetRequiredService<CreateVehicleUseCase>();

// Línea ~56: Cambiar
var vehicleId = createVehicleOutputPort.Output.VehicleId;
// Por:
var vehicleId = createVehicleOutputPort.Output.Id;

// Línea ~67: Cambiar
var createCustomerUseCase = scope.ServiceProvider.GetRequiredService<ICreateCustomerUseCase>();
// Por:
var createCustomerUseCase = scope.ServiceProvider.GetRequiredService<CreateCustomerUseCase>();

// Línea ~71: Cambiar
var customerId = createCustomerOutputPort.Output.CustomerId;
// Por:
var customerId = createCustomerOutputPort.Output.Id;

// Línea ~81: Cambiar
var rentUseCase = scope.ServiceProvider.GetRequiredService<IRentVehicleUseCase>();
// Por:
var rentUseCase = scope.ServiceProvider.GetRequiredService<RentVehicleUseCase>();

// Línea ~117-120: Cambiar
var returnInput = new ReturnVehicleInput
{
    RentalId = rentalId,
    ActualReturnDate = DateTime.UtcNow,
    FinalKilometers = 25000
};
// Por:
var returnInput = new ReturnVehicleInput
{
    RentalId = rentalId,
    ReturnDate = DateTime.UtcNow,
    CurrentKilometers = 25000
};

// Línea ~121: Cambiar
var returnUseCase = scope.ServiceProvider.GetRequiredService<IReturnVehicleUseCase>();
// Por:
var returnUseCase = scope.ServiceProvider.GetRequiredService<ReturnVehicleUseCase>();

// Línea ~163: Cambiar
var useCase = scope.ServiceProvider.GetRequiredService<IGetRentalByLicensePlateUseCase>();
// Por:
var useCase = scope.ServiceProvider.GetRequiredService<GetRentalByLicensePlateUseCase>();

// Línea ~172-173: ELIMINAR o COMENTAR (estas propiedades no existen)
outputPort.Output.CustomerId.Should().Be(customerId);
outputPort.Output.LicensePlate.Should().Be(licensePlate);
// Y AGREGAR:
outputPort.Output.VehicleLicensePlate.Should().Be(licensePlate);

// Línea ~191: Cambiar
var getAllUseCase = scope.ServiceProvider.GetRequiredService<IGetAllRentalsUseCase>();
// Por:
var getAllUseCase = scope.ServiceProvider.GetRequiredService<GetAllRentalsUseCase>();

// Línea ~224-225 y otros: Cambiar todas las referencias a ICreateVehicleUseCase
var createVehicleUseCase = scope.ServiceProvider.GetRequiredService<ICreateVehicleUseCase>();
// Por:
var createVehicleUseCase = scope.ServiceProvider.GetRequiredService<CreateVehicleUseCase>();

// Línea ~264-279 en SetupRentalAsync(): Cambiar todos los I* a clases concretas
// Línea ~268: VehicleId -> Id
var vehicleId = createVehicleOutputPort.Output.Id;

// Línea ~283: CustomerId -> Id  
var customerId = createCustomerOutputPort.Output.Id;
```

---

## ?? Resumen de Cambios

### Por Tipo de Error:

| Tipo de Error | Cantidad | Archivos Afectados |
|---------------|----------|-------------------|
| Interfaces inexistentes (`I*UseCase`) | ~15 | Todos |
| Propiedades incorrectas (`*Id` vs `Id`) | ~8 | Todos |
| `ReturnVehicleInput` propiedades | 2 | RentalFunctionalTests |
| `GetRentalByLicensePlateOutput` propiedades | 2 | RentalFunctionalTests |

### Archivos a Modificar:

1. ? **VehicleFunctionalTests.cs** - 3 cambios principales
2. ? **CustomerFunctionalTests.cs** - 5 cambios principales
3. ? **RentalFunctionalTests.cs** - 15+ cambios principales

---

## ?? Siguiente Paso

Aplicar todas estas correcciones a los 3 archivos de tests.

---

*Documento de referencia creado: 2024-02-01*
