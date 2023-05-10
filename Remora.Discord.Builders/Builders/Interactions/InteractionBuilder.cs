using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Builders.Errors;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Builders;

public record struct InteractionBuilder
(
    Optional<string>                                            Content     = default,
    Optional<IEnumerable<IEmbed>>                               Embeds      = default,
    Optional<IAllowedMentions>                                  Mentions    = default,
    Optional<bool>                                              IsEphemeral = default,
    Optional<IEnumerable<IMessageComponent>>                    Components  = default,
    Optional<IEnumerable<OneOf<FileData, IPartialAttachment>>>  Attachments = default,
    InteractionCallbackType                                     Type        = InteractionCallbackType.ChannelMessageWithSource
);

public static class InteractionBuilderExtensions
{
    public static InteractionBuilder WithContent(this InteractionBuilder builder, string content) => builder with { Content = content };

    public static InteractionBuilder AddEmbed(this InteractionBuilder builder, IEmbed embed)
    {
        var embeds = builder.Embeds.ValueOr(Enumerable.Empty<IEmbed>()).Append(embed);

        return builder with { Embeds = new(embeds) };
    }

    public static InteractionBuilder AddEmbeds(this InteractionBuilder builder, IEnumerable<IEmbed> embeds)
    {
        var newEmbeds = builder.Embeds.ValueOr(Enumerable.Empty<IEmbed>()).Concat(embeds);

        return builder with { Embeds = new(newEmbeds) };
    }

    public static InteractionBuilder WithMentions(this InteractionBuilder builder, IAllowedMentions mentions) => builder with { Mentions = new(mentions) };

    public static InteractionBuilder AsEphemeral(this InteractionBuilder builder, bool isEphemeral = true) => builder with { IsEphemeral = isEphemeral };

    public static InteractionBuilder AddComponent(this InteractionBuilder builder, IMessageComponent component)
    {
        var row = new ActionRowComponent(new[] { component });

        var components = builder.Components.ValueOr(Enumerable.Empty<IMessageComponent>()).Append(row);

        return builder with { Components = new(components) };
    }

    public static InteractionBuilder AddComponents(this InteractionBuilder builder, IEnumerable<IMessageComponent> components)
    {
        var row = new ActionRowComponent(components.ToArray());

        var newComponents = builder.Components.ValueOr(Enumerable.Empty<IMessageComponent>()).Append(row);

        return builder with { Components = new(components) };
    }

    public static InteractionBuilder AddAttachment(this InteractionBuilder builder, Stream data, string fileName)
    {
        var attachments = builder.Attachments.ValueOr(Enumerable.Empty<OneOf<FileData, IPartialAttachment>>()).Append(new FileData(fileName, data, null));

        return builder with { Attachments = new(attachments) };
    }

    public static InteractionBuilder AddAttachment(this InteractionBuilder builder, IPartialAttachment attachment)
    {
        var attachments = builder.Attachments.ValueOr(Enumerable.Empty<OneOf<FileData, IPartialAttachment>>()).Append(OneOf<FileData, IPartialAttachment>.FromT1(attachment));

        return builder with { Attachments = new(attachments) };
    }

    public static InteractionBuilder AddAttachments(this InteractionBuilder builder, IEnumerable<FileData> attachments)
    {
        var newAttachments = builder.Attachments.ValueOr(Enumerable.Empty<OneOf<FileData, IPartialAttachment>>()).Concat(attachments.Select(OneOf<FileData, IPartialAttachment>.FromT0));

        return builder with { Attachments = new(newAttachments) };
    }

    public static InteractionBuilder WithType(this InteractionBuilder builder, InteractionCallbackType type) => builder with { Type = type };

    public static Result Validate (this InteractionBuilder builder)
    {
        if (builder.Type is InteractionCallbackType.Pong or InteractionCallbackType.Modal)
        {
            return new ValidationError($"Builders should not be used for PONG response types. For modals, please use {nameof(ModalBuilder)} instead.");
        }

        if (builder.Type is InteractionCallbackType.ChannelMessageWithSource)
        {
            if (!builder.Content.IsDefined() && !builder.Embeds.IsDefined() && !builder.Components.IsDefined())
            {
                return new ValidationError($"You must provide at least one of content, embeds or components when using ChannelMessageWithSource.");
            }
        }

        if (builder.Content.IsDefined(out var content) && content.Length > 2000)
        {
            var over = content.Length - 2000;
            return new ValidationError($"Content must be 2000 characters or less. (Got {over} over).");
        }

        if (builder.Embeds.IsDefined(out var embeds) && embeds.Count() > 10)
        {
            var over = embeds.Count() - 10;

            return new ValidationError($"You can only provide up to 10 embeds. (Got {over} over).");
        }

        if (builder.Components.IsDefined(out var components) && components.Count() > 5)
        {
            var over = components.Count() - 5;

            return new ValidationError($"You can only provide up to 5 components. (Got {over} over).");
        }

        return Result.FromSuccess();
    }
}
