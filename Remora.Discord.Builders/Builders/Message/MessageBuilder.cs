using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Builders.Errors;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Builders;

/// <summary>
/// A builder for creating messages. Use associated methods to configure the message.
/// </summary>
/// <param name="Content"></param>
/// <param name="Embeds"></param>
/// <param name="Mentions"></param>
/// <param name="Attachments"></param>
/// <param name="Components"></param>
/// <remarks>Builders are intended to be short-lived objects and not re-used.</remarks>
public record struct MessageBuilder
(
    Optional<string>                            Content         = default,
    Optional<Snowflake>                         ReplyMessageID  = default,
    Optional<IEnumerable<IEmbed>>               Embeds          = default,
    Optional<IAllowedMentions>                  Mentions        = default,
    Optional<IEnumerable<FileData>>             Attachments     = default,
    Optional<IEnumerable<IMessageComponent>>    Components      = default
);

/// <summary>
/// Extension methods for <see cref="MessageBuilder"/>.
/// </summary>
public static class MessageBuilderExtensions
{
    /// <summary>
    /// Appends content onto the given builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="content">The content.</param>
    /// <returns>A new builder with the set content.</returns>
    public static MessageBuilder WithContent(this MessageBuilder builder, string content) => builder with { Content = content };

    /// <summary>
    /// Appends a single embed to the given builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="embed">The embed to add.</param>
    /// <returns>A new builder with the set embed.</returns>
    public static MessageBuilder AddEmbed(this MessageBuilder builder, IEmbed embed)
    {
        var embeds = builder.Embeds.ValueOr(Enumerable.Empty<IEmbed>()).Append(embed);
        
        return builder with { Embeds = new(embeds) };
    }
    
    /// <summary>
    /// Appends multiple embeds to the given builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="embeds">The embeds to append.</param>
    /// <returns>A new builder with the set embeds.</returns>
    public static MessageBuilder AddEmbeds(this MessageBuilder builder, IEnumerable<IEmbed> embeds)
    { 
        var currentEmbeds = builder.Embeds.ValueOr(Enumerable.Empty<IEmbed>()).Concat(embeds);
        
        return builder with { Embeds = new(currentEmbeds.Concat(embeds)) };
    }

    /// <summary>
    /// Appends an attachment to the given builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="attachment">The attachment to append.</param>
    /// <returns>A new builder with the appended attachment.</returns>
    public static MessageBuilder AddAttachment(this MessageBuilder builder, FileData attachment)
    {
        var attachments = builder.Attachments.ValueOr(Enumerable.Empty<FileData>()).Append(attachment);
        
        return builder with { Attachments = new(attachments) };
    }

    /// <summary>
    /// Appends a single component to the given builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="component">The component to add.</param>
    /// <returns>A new builder with the set component.</returns>
    /// <remarks>This method generates a new action row per call.</remarks>
    public static MessageBuilder AddComponent(this MessageBuilder builder, IMessageComponent component)
    {
        var row = new ActionRowComponent(new[] { component });
        
        var components = builder.Components.ValueOr(Enumerable.Empty<IActionRowComponent>()).Append(row);
        
        return builder with { Components = new(components) };
    }
    
    /// <summary>
    /// Adds multiple components to the given builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="components">The components to add.</param>
    /// <returns>A new builder with the set components.</returns>
    /// <remarks>This method generates a new action row per call, to which the given components will be added..</remarks>
    public static MessageBuilder AddComponents(this MessageBuilder builder, IEnumerable<IMessageComponent> components)
    {
        var row = new ActionRowComponent(components.ToArray());
        
        var rows = builder.Components.ValueOr(Enumerable.Empty<IActionRowComponent>()).Append(row);
        
        return builder with { Components = new(rows) };
    }

    /// <summary>
    /// Sets the allowed mentions for the message.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="allowedMentions">The allowed mentions.</param>
    /// <returns>A new builder with the set allowed mentions.</returns>
    public static MessageBuilder WithAllowedMentions(this MessageBuilder builder, IAllowedMentions allowedMentions) => builder with { Mentions = new(allowedMentions) };

    public static Result Validate(this MessageBuilder builder)
    {
        if (builder.Content.IsDefined(out var content) && content.Length > 2000)
        {
            var over = content.Length - 2000;
            
            return Result.FromError(new ValidationError($"Message content cannot be longer than 2000 characters. (Got {over} too many)."));
        }
        
        if (builder.Embeds.IsDefined(out var embeds) && embeds.Count() > 10)
        {
            var over = embeds.Count() - 10;
            return Result.FromError(new ValidationError("Message cannot contain more than 10 embeds. (Got {over} too many)."));
        }
        
        if (builder.Attachments.IsDefined(out var attachments) && attachments.Count() > 10)
        {
            var over = attachments.Count() - 10;
            
            return Result.FromError(new ValidationError($"Message cannot contain more than 10 attachments. (Got {over} too many)."));
        }
        
        if (builder.Components.IsDefined(out var components) && components.Count() > 5)
        {
            var over = components.Count() - 5;
            return Result.FromError(new ValidationError($"Message cannot contain more than 5 components. (Got {over} too many)."));
        }
        
        return Result.FromSuccess();
    }
}