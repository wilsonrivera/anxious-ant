using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace AnxiousAnt;

[StructLayout(LayoutKind.Auto)]
public readonly struct Optional<T> : IEquatable<Optional<T>>, IEquatable<T>
    where T : notnull
{
    private readonly T? _value;
    private readonly int _valueKind;

    public Optional()
    {
        Unsafe.SkipInit(out _value);
        _valueKind = Optional.EmptyValueKind;
    }

    public Optional(T? value)
    {
        _value = value;
        _valueKind = value is null ? Optional.NullValueKind : Optional.NotEmptyValueKind;
    }

    internal Optional(in Optional<Optional<T>> other)
    {
        if (other._valueKind < Optional.NotEmptyValueKind)
        {
            Unsafe.SkipInit(out _value);
            _valueKind = other._valueKind;
        }
        else
        {
            _value = other._value._value;
            _valueKind = other._value._valueKind;
        }
    }

    /// <summary>
    /// Represents an instance of <see cref="Optional{T}"/> with no value.
    /// </summary>
    public static Optional<T> None => new();

    /// <summary>
    /// Indicates whether the current instance of <see cref="Optional{T}"/> is in its default uninitialized state.
    /// </summary>
    [JsonIgnore]
    public bool IsDefault => _valueKind is Optional.UninitializedValueKind;

    /// <summary>
    /// Indicates whether the current instance of <see cref="Optional{T}"/> represents an empty state,
    /// either explicitly set to "none" or containing a <c>null</c> value.
    /// </summary>
    [JsonIgnore]
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(false, nameof(ValueRef))]
    public bool IsEmpty => _valueKind < Optional.NotEmptyValueKind;

    /// <summary>
    /// Gets a value indicating whether this instance of <see cref="Optional{T}"/> is either in the default
    /// uninitialized state or represents an empty value.
    /// </summary>
    [JsonIgnore]
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(false, nameof(ValueRef))]
    public bool IsDefaultOrEmpty => IsDefault || IsEmpty;

    /// <summary>
    /// Gets the underlying value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="Optional{T}"/> instance does not
    /// have a value.</exception>
    public T Value
    {
        get
        {
            EnsureInitializedAndHasValue();
            return _value;
        }
    }

    /// <summary>
    /// Gets an immutable reference to the underlying value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="Optional{T}"/> instance does not
    /// have a value.</exception>
    [JsonIgnore]
    public ref readonly T ValueRef
    {
        [UnscopedRef]
        get
        {
            EnsureInitializedAndHasValue();
            return ref _value!;
        }
    }

    internal int ValueKind => _valueKind;

    /// <inheritdoc />
    public override string ToString() => _valueKind switch
    {
        Optional.UninitializedValueKind => "Uninitialized",
        Optional.EmptyValueKind => "None",
        Optional.NullValueKind => "None",
        Optional.NotEmptyValueKind => $"Some({Value})",
        _ => throw new InvalidOperationException($"Optional is in an invalid state: {_valueKind}.")
    };

    /// <inheritdoc />
    public override int GetHashCode() =>
        _valueKind is Optional.NotEmptyValueKind
            ? EqualityComparer<T>.Default.GetHashCode(Value)
            : _valueKind;

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) => obj switch
    {
        null => _valueKind is Optional.NullValueKind,
        Optional<T> optional => Equals(optional),
        T value => Equals(value),
        _ => false
    };

    /// <inheritdoc />
    public bool Equals(Optional<T> other) =>
        _valueKind == other._valueKind &&
        (
            _valueKind < Optional.NotEmptyValueKind ||
            EqualityComparer<T?>.Default.Equals(_value, other._value)
        );

    /// <inheritdoc />
    public bool Equals(T? other) => other switch
    {
        null => _valueKind is Optional.NullValueKind,
        not null => _valueKind is Optional.NotEmptyValueKind && EqualityComparer<T>.Default.Equals(Value, other)
    };

    /// <summary>
    /// Attempts to retrieve the contained value in the current <see cref="Optional{T}"/> instance.
    /// </summary>
    /// <param name="result">When this method returns, contains the value stored in the <see cref="Optional{T}"/>
    /// instance if it is not empty or default; otherwise, the default value for the type of the parameter.
    /// This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="Optional{T}"/> instance contains a value; otherwise, <c>false</c>.
    /// </returns>
    public bool TryGetValue([MaybeNullWhen(false)] out T result)
    {
        result = default;
        if (_valueKind < Optional.NotEmptyValueKind || _value is null)
        {
            return false;
        }

        result = _value;
        return true;
    }

    /// <summary>
    /// Retrieves the value contained within the <see cref="Optional{T}"/> instance if present; otherwise, returns
    /// the default value of <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// The value of type <typeparamref name="T"/> if present; otherwise, the default value of
    /// <typeparamref name="T"/>.
    /// </returns>
    [Pure, ExcludeFromCodeCoverage]
    public T? ValueOrDefault() => ValueOr(default);

    /// <summary>
    /// Returns the contained value if the value is set; otherwise, returns the provided alternative value.
    /// </summary>
    /// <param name="alternative">The alternative value to return if the optional does not contain a value.</param>
    /// <returns>
    /// The contained value if present; otherwise, the specified alternative value.
    /// </returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(alternative))]
    public T? ValueOr(T? alternative) => _valueKind switch
    {
        Optional.NotEmptyValueKind => _value,
        _ => alternative,
    };

    /// <summary>
    /// Returns the current <see cref="Optional{T}"/> instance if it contains a value; otherwise, returns a
    /// new <see cref="Optional{T}"/> containing the specified alternative value.
    /// </summary>
    /// <param name="alternative">The alternative value to use if the current instance does not contain a value.</param>
    /// <returns>
    /// The current <see cref="Optional{T}"/> instance if it contains a value; otherwise, a new
    /// <see cref="Optional{T}"/> containing the specified alternative value.
    /// </returns>
    [Pure]
    public Optional<T> Or(T? alternative) => _valueKind switch
    {
        Optional.NotEmptyValueKind => this,
        _ => new Optional<T>(alternative)
    };

    /// <summary>
    /// Returns the current <see cref="Optional{T}"/> if it is not empty; otherwise, creates a new
    /// <see cref="Optional{T}"/> using the specified factory function.
    /// </summary>
    /// <param name="factory">The factory function to produce an alternative value if the current
    /// <see cref="Optional{T}"/> is empty or uninitialized.</param>
    /// <returns>
    /// A new <see cref="Optional{T}"/> containing the factory's value if the current instance is empty or
    /// uninitialized; otherwise, returns the current instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="factory"/> function is <c>null</c>.</exception>
    [Pure]
    public Optional<T> Or(Func<T?> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        if (_valueKind is Optional.NotEmptyValueKind)
        {
            return this;
        }

        var value = factory();
        return new Optional<T>(value);
    }

    /// <summary>
    /// Returns the current <see cref="Optional{T}"/> instance if it has a value; otherwise, asynchronously invokes
    /// the provided factory function to produce an alternative value.
    /// </summary>
    /// <param name="factory">A function that asynchronously produces an alternative value of type
    /// <typeparamref name="T"/> if the current instance is uninitialized or empty.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ValueTask{T}"/> representing the result of the operation: the current instance if it has a value,
    /// or a new instance containing the value returned by the factory function.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="factory"/> function is <c>null</c>.</exception>
    [Pure]
    public async ValueTask<Optional<T>> OrAsync(
        Func<CancellationToken, Task<T?>> factory,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(factory);
        if (_valueKind is Optional.NotEmptyValueKind)
        {
            return this;
        }

        var value = await factory(cancellationToken).ConfigureAwait(false);
        return new Optional<T>(value);
    }

    /// <summary>
    /// Returns the current instance if it contains a value; otherwise, returns the provided alternative.
    /// </summary>
    /// <param name="alternative">The alternative <see cref="Optional{T}"/> to return if the current instance is
    /// empty or uninitialized.</param>
    /// <returns>
    /// The current instance if it is not empty or the provided alternative.
    /// </returns>
    [Pure]
    public Optional<T> OrElse(Optional<T> alternative) =>
        _valueKind switch
        {
            Optional.NotEmptyValueKind => this,
            _ => alternative
        };

    /// <summary>
    /// Returns the current instance if it is not empty; otherwise, invokes the specified factory function
    /// to generate an alternative instance.
    /// </summary>
    /// <param name="factory">A function that returns an alternative <see cref="Optional{T}"/> instance when the
    /// current instance is empty.</param>
    /// <returns>
    /// The current instance if it is not empty; otherwise, the result of invoking the specified factory function.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="factory"/> function is <c>null</c>.</exception>
    [Pure]
    public Optional<T> OrElse(Func<Optional<T>> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        return _valueKind switch
        {
            Optional.NotEmptyValueKind => this,
            _ => factory()
        };
    }

    /// <summary>
    /// Returns the current instance if it contains a value, or asynchronously evaluates the provided factory
    /// function to produce an alternative value.
    /// </summary>
    /// <param name="factory">A function that produces an alternative <see cref="Optional{T}"/> asynchronously when the
    /// current instance is empty or uninitialized.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ValueTask{T}"/> that represents the result of the operation. It is the current instance if
    /// it contains a value, or the result of the factory function if the current instance is empty or uninitialized.
    /// </returns>
    [Pure]
    public ValueTask<Optional<T>> OrElseAsync(
        Func<CancellationToken, Task<Optional<T>>> factory,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(factory);

        return _valueKind is Optional.NotEmptyValueKind
            ? ValueTask.FromResult(this)
            : new ValueTask<Optional<T>>(factory(cancellationToken));
    }

    /// <summary>
    /// Filters the current <see cref="Optional{T}"/> instance based on the provided condition.
    /// If the condition is <c>true</c> and the current instance is not empty or default, the instance is returned;
    /// otherwise, an empty <see cref="Optional{T}"/> is returned.
    /// </summary>
    /// <param name="condition">A boolean value that determines whether the filter condition is met.</param>
    /// <returns>
    /// The current <see cref="Optional{T}"/> instance if the condition is true and it is not empty or default;
    /// otherwise, an empty <see cref="Optional{T}"/>.
    /// </returns>
    [Pure]
    public Optional<T> If(bool condition) =>
        !IsDefaultOrEmpty && condition ? this : None;

    /// <summary>
    /// Filters the current <see cref="Optional{T}"/> instance based on the provided predicate.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>An <see cref="Optional{T}"/> containing the current value if the predicate
    /// evaluates to <c>true</c>; otherwise, an empty <see cref="Optional{T}"/>.</returns>
    [Pure]
    public Optional<T> If(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return !IsDefaultOrEmpty && predicate(_value!) ? this : None;
    }

#pragma warning disable CS8763 // A method marked [DoesNotReturn] should not return.
    [StackTraceHidden, DoesNotReturn, MemberNotNull(nameof(_value)), MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureInitializedAndHasValue()
    {
        switch (_valueKind)
        {
            case Optional.UninitializedValueKind:
                throw new InvalidOperationException("Optional value has not been initialized.");
            case Optional.EmptyValueKind:
            case Optional.NullValueKind:
                throw new InvalidOperationException("Optional has no value.");
            case Optional.NotEmptyValueKind:
                Debug.Assert(_value is not null);
                break;
            default:
                throw new InvalidOperationException($"Optional is in an invalid state: {_valueKind}.");
        }
    }
#pragma warning restore CS8763 // A method marked [DoesNotReturn] should not return.

    /// <summary>
    /// Compares two <see cref="Optional{T}"/> instances for equality.
    /// </summary>
    /// <param name="left">The first <see cref="Optional{T}"/> instance to compare.</param>
    /// <param name="right">The second <see cref="Optional{T}"/> instance to compare.</param>
    /// <returns>
    /// <c>true</c> if the instances are equal; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool operator ==(in Optional<T> left, in Optional<T> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Optional{T}"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first operand to compare.</param>
    /// <param name="right">The second operand to compare.</param>
    /// <returns>
    /// <c>true</c> if the instances are not equal; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool operator !=(in Optional<T> left, in Optional<T> right) => !left.Equals(right);

    /// <summary>
    /// Determines whether the value contained in the <see cref="Optional{T}"/> instance
    /// is equal to the specified value.
    /// </summary>
    /// <param name="left">The <see cref="Optional{T}"/> instance being compared.</param>
    /// <param name="right">The value to compare against the optional's value.</param>
    /// <returns>
    /// <c>true</c> if the optional contains a value that is equal to <paramref name="right"/>; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool operator ==(in Optional<T> left, T? right) => left.Equals(right);

    /// <summary>
    /// Determines whether the value contained in the <see cref="Optional{T}"/> instance is not equal to the
    /// specified value.
    /// </summary>
    /// <param name="optional">The <see cref="Optional{T}"/> instance being compared.</param>
    /// <param name="value">The value to compare against the optional's value.</param>
    /// <returns>
    /// <c>true</c> if the optional does not contain a value equal to <paramref name="value"/>; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool operator !=(in Optional<T> optional, T? value) => !optional.Equals(value);

    /// <summary>
    /// Determines whether a specified <see cref="Optional{T}"/> instance is default or empty.
    /// </summary>
    /// <param name="optional">The <see cref="Optional{T}"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="Optional{T}"/> instance is default or empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !(in Optional<T> optional) => optional.IsDefaultOrEmpty;

    /// <summary>
    /// Determines whether the <see cref="Optional{T}"/> contains a non-empty value when evaluated in a boolean
    /// expression.
    /// </summary>
    /// <param name="optional">The <see cref="Optional{T}"/> instance being evaluated.</param>
    /// <returns>
    /// <c>true</c> if the optional is not the default and has a value; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator true(in Optional<T> optional) => !optional.IsDefaultOrEmpty;

    /// <summary>
    /// Determines whether the <see cref="Optional{T}"/> should be treated as <see langword="false"/>
    /// when evaluated in a boolean expression.
    /// </summary>
    /// <param name="result">The <see cref="Optional{T}"/> instance being evaluated.</param>
    /// <returns>
    /// <c>true</c> if the optional is default or empty; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public static bool operator false(in Optional<T> result) => !result;

    /// <summary>
    /// Performs a logical AND operation between two <see cref="Optional{T}"/> instances, evaluating to
    /// <c>true</c> only if both are non-empty.
    /// </summary>
    /// <param name="left">The first <see cref="Optional{T}"/> instance.</param>
    /// <param name="right">The second <see cref="Optional{T}"/> instance.</param>
    /// <returns>
    /// <c>true</c> if both optionals are not default and contain values; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator &(in Optional<T> left, in Optional<T> right) =>
        !left.IsDefaultOrEmpty && !right.IsDefaultOrEmpty;

    /// <summary>
    /// Returns the value of the <see cref="Optional{T}"/> if present; otherwise, returns the specified fallback value.
    /// </summary>
    /// <param name="left">The <see cref="Optional{T}"/> instance to evaluate.</param>
    /// <param name="right">The fallback value to return if <paramref name="left"/> is empty.</param>
    /// <returns>
    /// The value of <c>true</c> if it contains one; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    [return: NotNullIfNotNull(nameof(right))]
    public static T? operator |(in Optional<T> left, T? right) => left.ValueOr(right);

    /// <summary>
    /// Selects the first non-empty <see cref="Optional{T}"/> in a logical OR operation between two
    /// <see cref="Optional{T}"/>s.
    /// </summary>
    /// <param name="left">The primary <see cref="Optional{T}"/>.</param>
    /// <param name="right">The alternative <see cref="Optional{T}"/>.</param>
    /// <returns>
    /// The first non-empty <see cref="Optional{T}"/>.
    /// </returns>
    public static Optional<T> operator |(in Optional<T> left, in Optional<T> right) =>
        !left.IsDefaultOrEmpty ? left : right;
}