using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.Builders;

public record struct ButtonBuilder
(
    string Label = default,
    string? CustomID = default,
    ButtonComponentStyle Style = default,
    Optional<IPartialEmoji> Emoji = default,
    Optional<string> Url = default,
    Optional<bool> IsDisabled = default
);

public static class ButtonBuilderExtensions
{
    public static ButtonBuilder WithLabel(this ButtonBuilder builder, string label) => builder with { Label = label };

    public static ButtonBuilder WithCustomId(this ButtonBuilder builder, string customId) => builder with { CustomID = customId };

    public static ButtonBuilder WithStyle(this ButtonBuilder builder, ButtonComponentStyle style) => builder with { Style = style };

    public static ButtonBuilder WithEmoji(this ButtonBuilder builder, IEmoji emoji) => builder with { Emoji = new(emoji) };

    public static ButtonBuilder WithUrl(this ButtonBuilder builder, string url) => builder with { Url = url };

    public static ButtonBuilder WithDisabled(this ButtonBuilder builder, bool isDisabled) => builder with { IsDisabled = isDisabled };

    public static ButtonComponent ToButton(this ButtonBuilder builder) => new(builder.Style,builder.Label, builder.Emoji,builder.CustomID, builder.Url, builder.IsDisabled);
}
