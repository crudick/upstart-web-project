namespace Upstart.Api.Models;

public record UpdatePollAnswerApiRequest(
    string AnswerText,
    int DisplayOrder
);