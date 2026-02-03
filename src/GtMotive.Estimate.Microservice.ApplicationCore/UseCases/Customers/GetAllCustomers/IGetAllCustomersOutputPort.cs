using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.GetAllCustomers
{
    /// <summary>
    /// Output port for getting all customers.
    /// </summary>
    public interface IGetAllCustomersOutputPort : IOutputPortStandard<GetAllCustomersOutput>
    {
    }
}
