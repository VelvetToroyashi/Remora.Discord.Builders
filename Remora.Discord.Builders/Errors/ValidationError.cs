using Remora.Results;

namespace Remora.Discord.Builders.Errors;

public record ValidationError(string Message = "There was an error while validating.") : ResultError(Message);