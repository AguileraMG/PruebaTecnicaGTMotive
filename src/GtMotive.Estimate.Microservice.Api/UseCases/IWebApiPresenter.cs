using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases
{
    /// <summary>
    /// Base interface for Web API presenters that transform use case outputs into HTTP responses.
    /// Provides common functionality for handling HTTP responses and Problem Details (RFC 7807).
    /// </summary>
    public interface IWebApiPresenter
    {
        /// <summary>
        /// Gets the action result to be returned by the controller.
        /// </summary>
        IActionResult ActionResult { get; }

        /// <summary>
        /// Gets the current trace identifier for the request.
        /// Used for correlation and troubleshooting.
        /// </summary>
        /// <returns>The trace identifier from Activity or empty string.</returns>
        string GetTraceId() => System.Diagnostics.Activity.Current?.Id ?? string.Empty;

        /// <summary>
        /// Creates a standardized Problem Details response following RFC 7807.
        /// </summary>
        /// <param name="type">URI reference that identifies the problem type.</param>
        /// <param name="title">Short, human-readable summary of the problem.</param>
        /// <param name="status">HTTP status code for this problem.</param>
        /// <param name="detail">Human-readable explanation specific to this occurrence.</param>
        /// <returns>An ObjectResult configured with Problem Details.</returns>
        ObjectResult CreateProblemDetails(string type, string title, int status, string detail)
        {
            return new ObjectResult(new
            {
                type,
                title,
                status,
                detail,
                traceId = GetTraceId()
            })
            {
                StatusCode = status
            };
        }

        /// <summary>
        /// Creates a standardized Not Found (404) Problem Details response.
        /// Use when a required resource (entity) is not found in the system.
        /// </summary>
        /// <param name="detail">Specific explanation of what was not found.</param>
        /// <returns>An ObjectResult with 404 status code.</returns>
        ObjectResult CreateNotFoundProblem(string detail)
        {
            return CreateProblemDetails(
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: detail);
        }

        /// <summary>
        /// Creates a standardized Bad Request (400) Problem Details response.
        /// Use when input validation fails or data is malformed/empty.
        /// </summary>
        /// <param name="detail">Specific explanation of the validation failure.</param>
        /// <returns>An ObjectResult with 400 status code.</returns>
        ObjectResult CreateBadRequestProblem(string detail)
        {
            return CreateProblemDetails(
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title: "Bad Request",
                status: StatusCodes.Status400BadRequest,
                detail: detail);
        }

        /// <summary>
        /// Creates a standardized Conflict (409) Problem Details response.
        /// Use when a business rule is violated or there's a conflict with current state.
        /// </summary>
        /// <param name="detail">Specific explanation of the business rule violation.</param>
        /// <returns>An ObjectResult with 409 status code.</returns>
        ObjectResult CreateConflictProblem(string detail)
        {
            return CreateProblemDetails(
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                title: "Conflict",
                status: StatusCodes.Status409Conflict,
                detail: detail);
        }

        /// <summary>
        /// Creates a standardized OK (200) response with content.
        /// </summary>
        /// <param name="content">The content to return in the response.</param>
        /// <returns>An OkObjectResult with the provided content.</returns>
        OkObjectResult CreateOkResponse(object content)
        {
            return new OkObjectResult(content);
        }

        /// <summary>
        /// Creates a standardized Created (201) response with content.
        /// </summary>
        /// <param name="content">The content to return in the response.</param>
        /// <returns>An ObjectResult with 201 status code.</returns>
        ObjectResult CreateCreatedResponse(object content)
        {
            return new ObjectResult(content)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
    }
}
