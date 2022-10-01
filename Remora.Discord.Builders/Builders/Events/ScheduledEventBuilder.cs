using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Builders.Errors;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Builders;

public record struct ScheduledEventBuilder
(
    Optional<DateTimeOffset> StartTime = default,
    Optional<DateTimeOffset> EndTime = default,
    Optional<Snowflake> ChannelID = default,
    Optional<string> Location = default, 
    Optional<string> Description = default,
    GuildScheduledEventEntityType Type = GuildScheduledEventEntityType.StageInstance,
    Optional<GuildScheduledEventStatus> Status = default,
    Optional<Stream> Image = default,
    Optional<string> Reason = default
);

public static class ScheduledEventBuilderExtensions
{
    public static ScheduledEventBuilder WithStartTime(this ScheduledEventBuilder builder, DateTimeOffset startTime)
        => builder with { StartTime = startTime };

    public static ScheduledEventBuilder WithEndTime(this ScheduledEventBuilder builder, DateTimeOffset endTime)
        => builder with { EndTime = endTime };

    public static ScheduledEventBuilder WithChannelID(this ScheduledEventBuilder builder, Snowflake channelID)
        => builder with { ChannelID = channelID };

    public static ScheduledEventBuilder WithLocation(this ScheduledEventBuilder builder, string location)
        => builder with { Location = location };
    
    public static ScheduledEventBuilder WithStatus(this ScheduledEventBuilder builder, GuildScheduledEventStatus status)
        => builder with { Status = status };

    public static ScheduledEventBuilder WithDescription(this ScheduledEventBuilder builder, string description)
        => builder with { Description = description };

    public static ScheduledEventBuilder WithImage(this ScheduledEventBuilder builder, Stream image)
        => builder with { Image = image };

    public static ScheduledEventBuilder WithReason(this ScheduledEventBuilder builder, string reason)
        => builder with { Reason = reason };
    
    public static Result Validate(this ScheduledEventBuilder builder)
    {
        if (builder.StartTime == default)
        {
            return new ValidationError("Start time must be set.");
        }

        if ((builder.StartTime.HasValue && builder.EndTime.HasValue) && builder.StartTime.Value > builder.EndTime.Value)
        {
            return new ValidationError("End time must be after start time.");
        }
        
        if (builder.Type is GuildScheduledEventEntityType.External && !builder.Location.HasValue)
        {
            return new ValidationError("Location must be set for external events.");
        }

        return Result.FromSuccess();
    }
}