namespace Upstart.Api.Models;

public record CreatePollAnswerApiRequest(
    int PollId,
    string AnswerText,
    int DisplayOrder
);