using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Builders.Errors;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Builders;

public static class IDiscordRestChannelAPIExtensions
{
    public static async Task<Result<IMessage>> CreateMessageAsync
    (
        this IDiscordRestChannelAPI api,
        Snowflake channelID,
        MessageBuilder builder, 
        bool validate = false,
        CancellationToken ct = default
    )
    {
        if (validate)
        {
            var validation = builder.Validate();

            if (!validation.IsSuccess)
            {
                return Result<IMessage>.FromError(validation);
            }
        }

        Optional<IReadOnlyList<IEmbed>> embeds = builder.Embeds.IsDefined(out var embedEnumerbale) ? new(embedEnumerbale.ToArray()) : default;
        Optional<IReadOnlyList<IMessageComponent>> components = builder.Components.IsDefined(out var componentsEnumerable) ? new(componentsEnumerable.ToArray()) : default;
        
        return await api.CreateMessageAsync
        (
            channelID: channelID,
            builder.Content,
            embeds: embeds,
            allowedMentions: builder.Mentions,
            messageReference: builder.ReplyMessageID.IsDefined(out var reply) ? new MessageReference(reply, FailIfNotExists: false) : default,
            components: components,
            attachments: builder.Attachments.IsDefined(out var attachments) ? attachments.Select(OneOf<FileData, IPartialAttachment>.FromT0).ToArray() : default,
            ct: ct
        );
    }
    
    public static async Task<Result<IMessage>> EditMessageAsync
    (
        this IDiscordRestChannelAPI api,
        Snowflake channelID,
        Snowflake messageID,
        MessageBuilder builder, 
        bool validate = false,
        CancellationToken ct = default
    )
    {
        if (validate)
        {
            var validation = builder.Validate();

            if (!validation.IsSuccess || builder.ReplyMessageID.IsDefined())
            {
                return Result<IMessage>.FromError(validation.Error ?? new ValidationError("Reply ID cannot be set when editing messages."));
            }
        }

        Optional<IReadOnlyList<IEmbed>> embeds = builder.Embeds.IsDefined(out var embedEnumerbale) ? new(embedEnumerbale.ToArray()) : default;
        Optional<IReadOnlyList<IMessageComponent>> components = builder.Components.IsDefined(out var componentsEnumerable) ? new(componentsEnumerable.ToArray()) : default;
        
        return await api.EditMessageAsync
        (
            channelID: channelID,
            messageID: messageID,
            builder.Content,
            embeds: embeds,
            allowedMentions: builder.Mentions,
            components: components,
            ct: ct
        );
    }
}