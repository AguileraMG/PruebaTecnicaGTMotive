using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.CreateCustomer;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.GetAllCustomers;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Customers
{
    // Test Output Ports
    internal class TestCreateCustomerOutputPort : ICreateCustomerOutputPort
    {
        public bool WasStandardHandled { get; private set; }

        public bool WasConflictHandled { get; private set; }

        public CreateCustomerOutput Output { get; private set; }

        public string ErrorMessage { get; private set; }

        public void StandardHandle(CreateCustomerOutput output)
        {
            WasStandardHandled = true;
            Output = output;
        }

        public void ConflictHandle(string message)
        {
            WasConflictHandled = true;
            ErrorMessage = message;
        }
    }

    internal class TestGetAllCustomersOutputPort : IGetAllCustomersOutputPort
    {
        public bool WasStandardHandled { get; private set; }

        public GetAllCustomersOutput Output { get; private set; }

        public void StandardHandle(GetAllCustomersOutput output)
        {
            WasStandardHandled = true;
            Output = output;
        }
    }
}
