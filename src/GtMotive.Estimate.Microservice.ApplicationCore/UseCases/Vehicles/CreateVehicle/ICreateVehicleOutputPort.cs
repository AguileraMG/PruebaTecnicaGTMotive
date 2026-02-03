using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle
{
    /// <summary>
    /// Output port for the create vehicle use case.
    /// Supports: Success (201 Created), Bad Request (400), and Conflict (409) responses.
    /// </summary>
    public interface ICreateVehicleOutputPort
        : IOutputPortStandard<CreateVehicleOutput>,
          IOutputPortBadRequest,
          IOutputPortConflict
    {
    }
}
