namespace Upstart.Api.Models;

public record CreateUserApiRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    DateTime? DateOfBirth,
    string? SocialSecurityNumber,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? ZipCode,
    decimal? AnnualIncome,
    string? EmploymentStatus,
    int? CreditScore
);