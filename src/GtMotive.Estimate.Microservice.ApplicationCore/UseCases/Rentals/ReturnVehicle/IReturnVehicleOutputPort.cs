using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.ReturnVehicle
{
    /// <summary>
    /// Output port for the return vehicle use case.
    /// Supports: Success (200 OK), Bad Request (400), Not Found (404), and Conflict (409) responses.
    /// </summary>
    public interface IReturnVehicleOutputPort
        : IOutputPortStandard<ReturnVehicleOutput>,
          IOutputPortBadRequest,
          IOutputPortNotFound,
          IOutputPortConflict
    {
    }
}
