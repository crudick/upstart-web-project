using FluentValidation.Results;

namespace Upstart.Api.Extensions;

public static class ValidationExtensions
{
    public static Dictionary<string, string[]> ToDictionary(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x => x.ErrorMessage).ToArray()
            );
    }
}