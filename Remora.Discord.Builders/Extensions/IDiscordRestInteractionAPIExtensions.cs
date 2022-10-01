using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Builders;

public static class IDiscordRestInteractionAPIExtensions
{
    public static async Task<Result> CreateInteractionResponseAsync
    (
        this IDiscordRestInteractionAPI interactionAPI,
        Snowflake interactionID,
        string interactionToken,
        InteractionBuilder builder,
        bool validate = false,
        CancellationToken ct = default
    )
    {
        if (validate)
        {
            var validationResult = builder.Validate();
            
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
        }
        
        Optional<IReadOnlyList<IEmbed>> embeds = builder.Embeds.IsDefined(out var embedEnumerbale) ? new(embedEnumerbale.ToArray()) : default;
        Optional<IReadOnlyList<IMessageComponent>> components = builder.Components.IsDefined(out var componentsEnumerable) ? new(componentsEnumerable.ToArray()) : default;
        
        var data = new InteractionMessageCallbackData
        (
            Content: builder.Content,
            Embeds: embeds,
            Components: components,
            AllowedMentions: builder.Mentions,
            Flags: builder.IsEphemeral.IsDefined(out var ephemeral) && ephemeral ? MessageFlags.Ephemeral : default
        );
        
        return await interactionAPI.CreateInteractionResponseAsync
        (
            interactionID,
            interactionToken,
            new InteractionResponse(builder.Type, new(data)),
            attachments: builder.Attachments.IsDefined(out var attachmentsData) ? attachmentsData.ToArray() : default,
            ct: ct
        );
    }
    
    public static async Task<Result<IMessage>> EditOriginalInteractionResponseAsync
    (
        this IDiscordRestInteractionAPI interactionAPI,
        Snowflake applicationID,
        string interactionToken,
        InteractionBuilder builder,
        bool validate = false,
        CancellationToken ct = default
    )
    {
        if (validate)
        {
            var validationResult = builder.Validate();
            
            if (!validationResult.IsSuccess)
            {
                return Result<IMessage>.FromError(validationResult);
            }
        }
        
        Optional<IReadOnlyList<IEmbed>> embeds = builder.Embeds.IsDefined(out var embedEnumerbale) ? new(embedEnumerbale.ToArray()) : default;
        Optional<IReadOnlyList<IMessageComponent>> components = builder.Components.IsDefined(out var componentsEnumerable) ? new(componentsEnumerable.ToArray()) : default;
        
        return await interactionAPI.EditOriginalInteractionResponseAsync
        (
            applicationID,
            interactionToken,
            builder.Content,
            embeds,
            builder.Mentions,
            components,
            builder.Attachments.IsDefined(out var attachmentsData) ? attachmentsData.ToArray() : default,
            ct
        );
    }
    
    public static async Task<Result> CreateInteractionResponseAsync
    (
        this IDiscordRestInteractionAPI interactionAPI,
        Snowflake interactionID,
        string interactionToken,
        ModalBuilder builder,
        bool validate = false,
        CancellationToken ct = default
    )
    {
        if (validate)
        {
            var validationResult = builder.Validate();
            
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
        }

        var data = new InteractionModalCallbackData
        (
            Title: builder.Title.Value,
            CustomID: builder.CustomID.Value,
            Components: builder.Forms.Value.ToArray()
        );
        
        return await interactionAPI.CreateInteractionResponseAsync
        (
            interactionID,
            interactionToken,
            new InteractionResponse(InteractionCallbackType.Modal, new(data)),
            ct: ct
        );
    }

    public static async Task<Result<IMessage>> CreateFollowupMessageAsync
    (
        this IDiscordRestInteractionAPI interactionAPI,
        Snowflake applicationID,
        string interactionToken,
        InteractionBuilder builder,
        bool validate = false,
        CancellationToken ct = default
    )
    {
        if (validate)
        {
            var validationResult = builder.Validate();
            
            if (!validationResult.IsSuccess)
            {
                return Result<IMessage>.FromError(validationResult);
            }
        }
        
        Optional<IReadOnlyList<IEmbed>> embeds = builder.Embeds.IsDefined(out var embedEnumerbale) ? new(embedEnumerbale.ToArray()) : default;
        Optional<IReadOnlyList<IMessageComponent>> components = builder.Components.IsDefined(out var componentsEnumerable) ? new(componentsEnumerable.ToArray()) : default;
        
        return await interactionAPI.CreateFollowupMessageAsync
        (
            applicationID,
            interactionToken,
            builder.Content,
            default,
            embeds,
            builder.Mentions,
            components,
            builder.Attachments.IsDefined(out var attachmentsData) ? attachmentsData.ToArray() : default,
            builder.IsEphemeral.IsDefined(out var ephemeral) && ephemeral ? MessageFlags.Ephemeral : default,
            ct
        );
    }
    
    public static async Task<Result<IMessage>> EditFollowupMessageAsync
    (
        this IDiscordRestInteractionAPI interactionAPI,
        Snowflake applicationID,
        string interactionToken,
        Snowflake messageID,
        InteractionBuilder builder,
        bool validate = false,
        CancellationToken ct = default
    )
    {
        if (validate)
        {
            var validationResult = builder.Validate();
            
            if (!validationResult.IsSuccess)
            {
                return Result<IMessage>.FromError(validationResult);
            }
        }
        
        Optional<IReadOnlyList<IEmbed>> embeds = builder.Embeds.IsDefined(out var embedEnumerbale) ? new(embedEnumerbale.ToArray()) : default;
        Optional<IReadOnlyList<IMessageComponent>> components = builder.Components.IsDefined(out var componentsEnumerable) ? new(componentsEnumerable.ToArray()) : default;
        
        return await interactionAPI.EditFollowupMessageAsync
        (
            applicationID,
            interactionToken,
            messageID,
            builder.Content,
            embeds,
            builder.Mentions,
            components,
            builder.Attachments.IsDefined(out var attachmentsData) ? attachmentsData.ToArray() : default,
            ct
        );
    }
}