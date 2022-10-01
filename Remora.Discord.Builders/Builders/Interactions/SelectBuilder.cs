using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.Builders;

public record struct SelectBuilder
(
    Optional<string>                        CustomID = default,
    Optional<IEnumerable<ISelectOption>>    Options = default,
    Optional<string>                        Placeholder = default,
    Optional<int>                           MinValues = default,
    Optional<int>                           MaxValues = default,
    Optional<bool>                          IsDisabled = default
);

public static class SelectBuilderExtensions
{
    public static SelectBuilder WithCustomID(this SelectBuilder builder, string customID)
    {
        return builder with { CustomID = customID };
    }
    
    public static SelectBuilder AddOption(this SelectBuilder builder, ISelectOption option)
    {
        var newOptions = builder.Options.ValueOr(Enumerable.Empty<ISelectOption>()).Append(option);

        return builder with { Options = new(newOptions) };
    }
    
    public static SelectBuilder AddOptions(this SelectBuilder builder, IEnumerable<ISelectOption> options)
    {
        return builder with { Options = new(options) };
    }
    
    public static SelectBuilder WithPlaceholder(this SelectBuilder builder, string placeholder)
    {
        return builder with { Placeholder = placeholder };
    }
    
    public static SelectBuilder WithMinValues(this SelectBuilder builder, int minValues)
    {
        return builder with { MinValues = minValues };
    }
    
    public static SelectBuilder WithMaxValues(this SelectBuilder builder, int maxValues)
    {
        return builder with { MaxValues = maxValues };
    }
    
    public static SelectBuilder Disable(this SelectBuilder builder)
    {
        return builder with { IsDisabled = true };
    }
    
    public static SelectBuilder Enable(this SelectBuilder builder)
    {
        return builder with { IsDisabled = false };
    }
    
    public static SelectMenuComponent Build(this SelectBuilder builder)
    {
        return new(builder.CustomID.Value, builder.Options.Value.ToArray(), builder.Placeholder, builder.MinValues, builder.MaxValues, builder.IsDisabled);
    }
}