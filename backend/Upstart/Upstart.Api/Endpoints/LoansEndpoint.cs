using AutoMapper;
using FluentValidation;
using Upstart.Api.Models;
using Upstart.Application.Services;

namespace Upstart.Api.Endpoints;

public static class LoansEndpoint
{
    public static void MapLoansEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/loans").WithTags("Loans");

        group.MapPost("/", CreateLoan)
            .WithName("CreateLoan")
            .WithSummary("Create a new loan")
            .Produces(201)
            .Produces(400);
    }

    private static async Task<IResult> CreateLoan(CreateLoanApiRequest request, LoanService loanService, IValidator<CreateLoanApiRequest> validator, IMapper mapper)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.ToDictionary());
        }

        var serviceRequest = mapper.Map<CreateLoanRequest>(request);

        var result = await loanService.CreateLoanAsync(serviceRequest);
        return Results.Created($"/api/loans/{result.Id}", result);
    }
}