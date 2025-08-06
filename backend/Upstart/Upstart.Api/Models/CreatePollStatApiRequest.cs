namespace Upstart.Api.Models;

public record CreatePollStatApiRequest(
    int PollId,
    int PollAnswerId,
    int UserId
);