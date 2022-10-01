using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Builders;

public static class IDiscordRestWebhookAPIExtensions
{
    public static async Task<Result<IMessage?>> ExecuteWebhookAsync
    (
        this IDiscordRestWebhookAPI webhookAPI,
        Snowflake webhookID,
        string webhookToken,
        WebhookBuilder builder,
        Optional<bool> shouldWait = default,
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
        
        return await webhookAPI.ExecuteWebhookAsync
        (
            webhookID,
            webhookToken,
            shouldWait,
            builder.Content,
            builder.Username,
            builder.AvatarUrl,
            embeds: embeds,
            allowedMentions: builder.AllowedMentions,
            threadID: builder.ThreadID,
            components: components,
            attachments: builder.Attachments.IsDefined(out var attachments) ? attachments.Select(OneOf<FileData, IPartialAttachment>.FromT0).ToArray() : default,
            flags: builder.IsEphemeral.IsDefined(out var ephemeral) && ephemeral ? MessageFlags.Ephemeral : default,
            threadName: builder.ThreadName,
            ct: ct
        );
    }
    
    public static async Task<Result<IMessage>> EditWebhookAsync
    (
        this IDiscordRestWebhookAPI api,
        Snowflake webhookID,
        string webhookToken,
        Snowflake messageID,
        WebhookBuilder builder,
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
        
        return await api.EditWebhookMessageAsync
        (
            webhookID,
            webhookToken,
            messageID,
            builder.Content,
            embeds: embeds,
            allowedMentions: builder.AllowedMentions,
            components: components,
            attachments: builder.Attachments.IsDefined(out var attachments) ? attachments.Select(OneOf<FileData, IPartialAttachment>.FromT0).ToArray() : default,
            ct: ct
        );
    }
}