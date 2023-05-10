using Remora.Rest.Core;

namespace Remora.Discord.Builders;

public static class OptionalExtensions
{
    /// <summary>
    /// Extracts the value from a <see cref="Optional{T}"/>, or returns a given value if the optional does not contain one.
    /// </summary>
    /// <param name="this">The optional.</param>
    /// <param name="value">The value to return if the optional does not contain a value.</param>
    /// <typeparam name="T">The type the optional contains.</typeparam>
    /// <returns>The pre-existing or given value.</returns>
    public static T ValueOr<T>(this Optional<T> @this, T value) => @this.HasValue ? @this.Value : value;

    /// <summary>
    /// Extracts the value from a <see cref="Optional{T}"/>, or returns a default, non-null value if the optional does not contain one.
    /// </summary>
    /// <param name="this">The optional.</param>
    /// <typeparam name="T">The type the optional contains.</typeparam>
    /// <returns>The pre-existing or generated value.</returns>
    public static T ValueOr<T>(this Optional<T> @this) where T : class, new() => @this.HasValue ? @this.Value : new();
}