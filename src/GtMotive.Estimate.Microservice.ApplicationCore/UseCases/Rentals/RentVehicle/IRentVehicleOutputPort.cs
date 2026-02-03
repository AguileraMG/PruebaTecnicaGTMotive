using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.RentVehicle
{
    /// <summary>
    /// Output port for the rent vehicle use case.
    /// Supports: Success (201 Created), Bad Request (400), Not Found (404), and Conflict (409) responses.
    /// </summary>
    public interface IRentVehicleOutputPort
        : IOutputPortStandard<RentVehicleOutput>,
          IOutputPortBadRequest,
          IOutputPortNotFound,
          IOutputPortConflict
    {
    }
}
