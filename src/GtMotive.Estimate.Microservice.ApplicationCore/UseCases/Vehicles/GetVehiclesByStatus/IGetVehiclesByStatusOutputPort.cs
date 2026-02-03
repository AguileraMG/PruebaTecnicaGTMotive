using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetVehiclesByStatus
{
    /// <summary>
    /// Output port for the get vehicles by status use case.
    /// </summary>
    public interface IGetVehiclesByStatusOutputPort
        : IOutputPortStandard<GetVehiclesByStatusOutput>
    {
    }
}
