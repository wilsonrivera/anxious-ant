namespace AnxiousAnt;

/// <summary>
/// Represents maybe a value, maybe not.
/// <br/>
/// It contains a <see cref="bool"/> indicating if the value is there and, if so, the value itself.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public readonly struct Maybe<TValue> : IEquatable<Maybe<TValue>>
{
    private readonly TValue _value;

    private Maybe(TValue value)
    {
        HasValue = true;
        _value = value;
    }

    /// <summary>
    /// Represents a reusable result to be used when no value is there: using this saves memory allocations.
    /// </summary>
    public static readonly Maybe<TValue> None = default;

    /// <summary>
    /// Indicates if the value is there.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; }

    /// <summary>
    /// If the value is there (you can check <see cref="HasValue"/> to know that) the actual value is returned,
    /// otherwise an <see cref="InvalidOperationException"/> will be thrown.
    /// </summary>
    public TValue Value
    {
        get
        {
            if (HasValue)
            {
                return _value;
            }

            throw new InvalidOperationException("A value is not available for this instance");
        }
    }

    /// <summary>
    /// Creates a new <see cref="Maybe{TValue}"/> instance for a successful case by providing
    /// the <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value of type <typeparamref name="TValue"/> to use.</param>
    /// <returns>
    /// The newly created <see cref="Maybe{TValue}"/> instance.
    /// </returns>
    public static Maybe<TValue> FromValue(TValue value) => new(value);

    /// <inheritdoc />
    public override string ToString() =>
        HasValue ? Value?.ToString() ?? string.Empty : string.Empty;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(HasValue, _value);

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        obj is Maybe<TValue> other && Equals(other);

    /// <inheritdoc/>
    public bool Equals(Maybe<TValue> other)
    {
        if (!HasValue)
        {
            return !other.HasValue;
        }

        return other.HasValue &&
               (
                   (ReferenceEquals(null, Value) && ReferenceEquals(null, other.Value)) ||
                   (
                       !ReferenceEquals(null, Value) &&
                       !ReferenceEquals(null, other.Value) &&
                       EqualityComparer<TValue>.Default.Equals(Value, other.Value)
                   )
               );
    }

    /// <summary>
    /// Implements an implicit conversion from any type of value to a <see cref="Maybe{TValue}"/> instance with
    /// that value.
    /// </summary>
    /// <param name="value">The value to convert to a <see cref="Maybe{TValue}"/> instance.</param>
    public static implicit operator Maybe<TValue>(TValue value) => FromValue(value);

    /// <summary>
    /// Returns <see cref="Maybe{TValue}"/> or, if <see cref="Maybe{TValue}.HasValue"/> is <see langword="false"/>,
    /// throws an <see cref="InvalidOperationException"/> exception instead.
    /// </summary>
    /// <param name="maybe">The <see cref="Maybe{TValue}"/> instance.</param>
    public static implicit operator TValue(Maybe<TValue> maybe) => maybe.Value;

    /// <summary>
    /// Determines whether two specified <see cref="Maybe{TValue}"/> instances have the same value.
    /// </summary>
    /// <param name="left">The first <see cref="Maybe{TValue}"/> to compare.</param>
    /// <param name="right">The second <see cref="Maybe{TValue}"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the values of the two instances are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(Maybe<TValue> left, Maybe<TValue> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two specified <see cref="Maybe{TValue}"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="Maybe{TValue}"/> to compare.</param>
    /// <param name="right">The second <see cref="Maybe{TValue}"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the two specified <see cref="Maybe{TValue}"/> instances are not equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(Maybe<TValue> left, Maybe<TValue> right) => !(left == right);
}