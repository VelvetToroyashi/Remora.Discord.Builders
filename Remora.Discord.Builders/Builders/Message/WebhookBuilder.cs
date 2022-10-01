using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Builders.Errors;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Builders;

public record struct WebhookBuilder
(
    Optional<string>                            Content         = default,
    Optional<string>                            Username        = default,
    Optional<string>                            AvatarUrl       = default,
    Optional<IEnumerable<IEmbed>>               Embeds          = default,
    Optional<IAllowedMentions>                  AllowedMentions = default,
    Optional<Snowflake>                         ThreadID        = default,
    Optional<IEnumerable<IMessageComponent>>    Components      = default,
    Optional<IEnumerable<FileData>>             Attachments     = default,
    Optional<bool>                              IsEphemeral       = default,
    Optional<string>                            ThreadName      = default
);

public static class WebhookBuilderExtensions
{
    public static WebhookBuilder WithContent(this WebhookBuilder builder, string content)
    {
        return builder with { Content = content };
    }

    public static WebhookBuilder WithUsername(this WebhookBuilder builder, string username)
    {
        return builder with { Username = username };
    }
    
    public static WebhookBuilder WithAvatarUrl(this WebhookBuilder builder, string avatarUrl)
    {
        return builder with { AvatarUrl = avatarUrl };
    }
    
    public static WebhookBuilder AddEmbed(this WebhookBuilder builder, IEmbed embed)
    {
        var embeds = builder.Embeds.ValueOr(Enumerable.Empty<IEmbed>()).Append(embed);
        
        return builder with { Embeds = new(embeds) };
    }
    
    public static WebhookBuilder AddEmbeds(this WebhookBuilder builder, IEnumerable<IEmbed> embeds)
    {
        var newEmbeds = builder.Embeds.ValueOr(Enumerable.Empty<IEmbed>()).Concat(embeds);
        
        return builder with { Embeds = new(newEmbeds) };
    }
    
    public static WebhookBuilder WithAllowedMentions(this WebhookBuilder builder, IAllowedMentions allowedMentions)
    {
        return builder with { AllowedMentions = new(allowedMentions) };
    }
    
    public static WebhookBuilder WithThreadID(this WebhookBuilder builder, Snowflake threadID)
    {
        return builder with { ThreadID = new(threadID) };
    }
    
    public static WebhookBuilder AddComponent(this WebhookBuilder builder, IMessageComponent component)
    {
        var row = new ActionRowComponent(new[] { component });
        var components = builder.Components.ValueOr(Enumerable.Empty<IMessageComponent>()).Append(row);
        
        return builder with { Components = new(components) };
    }
    
    public static WebhookBuilder AddComponents(this WebhookBuilder builder, IEnumerable<IMessageComponent> components)
    {
        var row = new ActionRowComponent(components.ToArray());
        var newComponents = builder.Components.ValueOr(Enumerable.Empty<IMessageComponent>()).Append(row);
        
        return builder with { Components = new(newComponents) };
    }
    
    public static WebhookBuilder AddAttachment(this WebhookBuilder builder, FileData attachment)
    {
        var attachments = builder.Attachments.ValueOr(Enumerable.Empty<FileData>()).Append(attachment);
        
        return builder with { Attachments = new(attachments) };
    }
    
    public static WebhookBuilder AddAttachments(this WebhookBuilder builder, IEnumerable<FileData> attachments)
    {
        var newAttachments = builder.Attachments.ValueOr(Enumerable.Empty<FileData>()).Concat(attachments);
        
        return builder with { Attachments = new(newAttachments) };
    }
    
    public static WebhookBuilder AsEphemeral(this WebhookBuilder builder, bool ephemeral = true)
    {
        return builder with { IsEphemeral = new(ephemeral) };
    }
    
    public static WebhookBuilder WithThreadName(this WebhookBuilder builder, string threadName)
    {
        return builder with { ThreadName = threadName };
    }

    public static Result Validate(this WebhookBuilder builder)
    {
        if (builder.Content.IsDefined(out var content) && content.Length > 2000)
        {
            return new ValidationError("Content cannot be longer than 2000 characters.");
        }
        
        if (builder.Username.IsDefined(out var username) && username.Length > 32)
        {
            return new ValidationError("Username cannot be longer than 32 characters.");
        }
        
        if (builder.Embeds.IsDefined(out var embeds) && embeds.Count() > 10)
        {
            var over = embeds.Count() - 10;
            
            return new ValidationError($"Cannot send more than 10 embeds. (Got {over} too many)");
        }
        
        if (builder.Components.IsDefined(out var components) && components.Count() > 5)
        {
            var over = components.Count() - 5;
            
            return new ValidationError($"Cannot send more than 5 components. (Got {over} too many)");
        }
        
        if (builder.Attachments.IsDefined(out var attachments) && attachments.Count() > 10)
        {
            var over = attachments.Count() - 10;
            
            return new ValidationError($"Cannot send more than 10 attachments. (Got {over} too many)");
        }
        
        if (builder.ThreadName.IsDefined(out var threadName) && threadName.Length > 100)
        {
            return new ValidationError("Thread name cannot be longer than 100 characters.");
        }

        if (builder.ThreadID.IsDefined(out var threadID) && threadID.Value < Constants.DiscordEpoch)
        {
            return new ValidationError("Invalid thread ID (timestamped before Discord epoch, which is impossible.)");
        }

        return Result.FromSuccess();
    }
}