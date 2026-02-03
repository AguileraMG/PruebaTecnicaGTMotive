using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.CreateCustomer
{
    /// <summary>
    /// Output port for the create customer use case.
    /// Supports: Success (201 Created) and Conflict (409) responses.
    /// </summary>
    public interface ICreateCustomerOutputPort
        : IOutputPortStandard<CreateCustomerOutput>,
          IOutputPortConflict
    {
    }
}
