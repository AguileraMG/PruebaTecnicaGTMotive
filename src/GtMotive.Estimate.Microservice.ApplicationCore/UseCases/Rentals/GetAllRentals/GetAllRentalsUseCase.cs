using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetAllRentals
{
    /// <summary>
    /// Use case for getting all rentals with optional status filter.
    /// </summary>
    public sealed class GetAllRentalsUseCase(
        IGetAllRentalsOutputPort outputPort,
        IRentalRepository rentalRepository) : IUseCase<GetAllRentalsInput>
    {
        private readonly IGetAllRentalsOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
        private readonly IRentalRepository _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));

        /// <summary>
        /// Executes the use case to get all rentals.
        /// </summary>
        /// <param name="input">The input with optional status filter.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync(GetAllRentalsInput input, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(input);

            var rentals = await _rentalRepository.GetAllRentalsAsync(input.Status, ct);

            var rentalDtos = rentals.Select(v => new RentalDto
            {
                RentalId = v.Id,
                VehicleId = v.VehicleId,
                RentalDate = v.RentalDate,
                ReturnDate = v.ReturnDate,
                ExpectedReturnDate = v.ExpectedReturnDate,
                Status = v.Status.ToString(),
                IsOverdue = v.IsOverdue()
            }).ToList();

            var output = new GetAllRentalsOutput
            {
                Rentals = new Collection<RentalDto>(rentalDtos),
                TotalCount = rentalDtos.Count
            };

            _outputPort.StandardHandle(output);
        }
    }
}
