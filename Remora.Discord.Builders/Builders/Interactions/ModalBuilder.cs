using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Builders.Errors;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Builders;

public record struct ModalBuilder
(
    Optional<string>                            CustomID    = default,
    Optional<string>                            Title       = default,
    Optional<IEnumerable<ITextInputComponent>>  Forms       = default
);

public record struct TextInputBuilder
(
    Optional<string>    CustomID    = default,
    Optional<string>    Placeholder = default,
    Optional<string>    Label       = default,
    TextInputStyle      Style       = default,
    Optional<string>    Value       = default,
    Optional<int>       MinLength   = default,
    Optional<int>       MaxLength   = default,
    Optional<bool>      IsRequired  = default
);

public static class TextInputBuilderExtensions
{
    public static TextInputBuilder WithCustomID(this TextInputBuilder builder, string customID)
        => builder with { CustomID = customID };

    public static TextInputBuilder WithPlaceholder(this TextInputBuilder builder, string placeholder)
        => builder with { Placeholder = placeholder };

    public static TextInputBuilder WithLabel(this TextInputBuilder builder, string label)
        => builder with { Label = label };

    public static TextInputBuilder WithValue(this TextInputBuilder builder, string value)
        => builder with { Value = value };

    public static TextInputBuilder WithMinLength(this TextInputBuilder builder, int minLength)
        => builder with { MinLength = minLength };

    public static TextInputBuilder WithMaxLength(this TextInputBuilder builder, int maxLength)
        => builder with { MaxLength = maxLength };

    public static TextInputBuilder IsRequired(this TextInputBuilder builder, bool isRequired = true)
        => builder with { IsRequired = isRequired };

    public static ITextInputComponent Build(this TextInputBuilder builder)
        => new TextInputComponent(builder.CustomID.Value, builder.Style, builder.Label.Value, builder.MinLength, builder.MaxLength, builder.IsRequired, builder.Value, builder.Placeholder );
}

public static class ModalBuilderExtensions
{
    public static ModalBuilder WithCustomID(this ModalBuilder builder, string customID)
        => builder with { CustomID = customID };

    public static ModalBuilder AddForm(this ModalBuilder builder, TextInputBuilder form)
    {
        var newForms = builder.Forms.ValueOr(Enumerable.Empty<ITextInputComponent>()).Append(form.Build());

        return builder with { Forms = new(newForms) };
    }

    public static ModalBuilder AddForms(this ModalBuilder builder, IEnumerable<ITextInputComponent> forms)
    {
        var newForms = builder.Forms.ValueOr(Enumerable.Empty<ITextInputComponent>()).Concat(forms);

        return builder with { Forms = new(newForms.ToArray()) };
    }

    public static Result Validate(this ModalBuilder builder)
    {
        if (!builder.CustomID.IsDefined(out var customID) || customID.Length > 100)
        {
            return new ValidationError("CustomID must be set and less than 100 characters");
        }

        if (!builder.Title.IsDefined(out var title) || title.Length > 100)
        {
            return new ValidationError("Title must be set and less than 100 characters");
        }

        if (!builder.Forms.IsDefined(out var forms) || forms.Count() is 0)
        {
            return new ValidationError("At least one form must be set");
        }

        if (forms.Count() > 5)
        {
            return new ValidationError("You can only have 5 forms in a modal");
        }

        return Result.FromSuccess();
    }


}
