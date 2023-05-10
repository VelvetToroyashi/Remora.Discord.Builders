using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Builders;

public static class IDiscordRestScheduledEventAPIExtensions
{
    public static async Task<Result<IGuildScheduledEvent>> CreateGuildScheduledEventAsync
    (
        this IDiscordRestGuildScheduledEventAPI scheduledEventAPI,
        Snowflake guildID,
        string name,
        ScheduledEventBuilder builder,
        bool validate = false,
        CancellationToken ct = default
    )
        => await scheduledEventAPI.CreateGuildScheduledEventAsync
        (
            guildID,
            builder.ChannelID,
            builder.Location.IsDefined(out var location) ? new GuildScheduledEventEntityMetadata(location) : default,
            name,
            GuildScheduledEventPrivacyLevel.GuildOnly,
            builder.StartTime.ValueOr(DateTimeOffset.UtcNow),
            builder.EndTime,
            builder.Description,
            builder.Type,
            builder.Image,
            builder.Reason,
            ct
        );

    public static async Task<Result<IGuildScheduledEvent>> ModifyGuildScheduledEventAsync
    (
        this IDiscordRestGuildScheduledEventAPI scheduledEventAPI,
        Snowflake guildID,
        Snowflake eventID,
        string name,
        ScheduledEventBuilder builder,
        bool validate = false,
        CancellationToken ct = default
    )
        => await scheduledEventAPI.ModifyGuildScheduledEventAsync
        (
            guildID,
            eventID,
            builder.ChannelID,
            builder.Location.IsDefined(out var location) ? new GuildScheduledEventEntityMetadata(location) : default,
            name,
            GuildScheduledEventPrivacyLevel.GuildOnly,
            builder.StartTime,
            builder.EndTime,
            builder.Description,
            builder.Type,
            builder.Status,
            builder.Image,
            builder.Reason,
            ct
        );
}