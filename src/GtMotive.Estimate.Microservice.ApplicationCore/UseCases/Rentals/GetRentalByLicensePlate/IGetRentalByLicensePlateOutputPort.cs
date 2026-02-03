using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetRentalByLicensePlate
{
    /// <summary>
    /// Output port for getting a rental by license plate.
    /// Supports: Success (200), Bad Request (400), and Not Found (404) responses.
    /// </summary>
    public interface IGetRentalByLicensePlateOutputPort
        : IOutputPortStandard<GetRentalByLicensePlateOutput>,
          IOutputPortBadRequest,
          IOutputPortNotFound
    {
    }
}
