using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetAllRentals
{
    /// <summary>
    /// Output port for getting all rentals.
    /// </summary>
    public interface IGetAllRentalsOutputPort : IOutputPortStandard<GetAllRentalsOutput>
    {
    }
}
