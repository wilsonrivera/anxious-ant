// Adapted from https://github.com/tmenier/Flurl/blob/93f5668dc2438c677a9e3384d2460b6de9f99c02/src/Flurl/QueryParamCollection.cs#L135

using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;

using AnxiousAnt.Collections;
using AnxiousAnt.Text;

namespace AnxiousAnt.Http;

/// <summary>
/// Represents a URL query as a collection of name/value pairs. Insertion order is preserved.
/// </summary>
[DebuggerDisplay("{ToString(),raw}")]
public sealed partial class QueryParamCollection : IReadOnlyKeyValueCollection<string?>
{
    private static readonly SearchValues<char> QuestionMarkSearchValues = SearchValues.Create('?');
    private static readonly string?[] EmptyStrings = [];

    private KeyValueCollection<QueryParamValue>? _values;
    private bool _changed;
    private string? _toString;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParamCollection"/> class.
    /// </summary>
    /// <param name="queryString">Optional query string to parse.</param>
    public QueryParamCollection(string? queryString = null)
    {
        _toString = queryString;
        _values = string.IsNullOrWhiteSpace(queryString) ? null : ParseQueryString(queryString);
    }

    private QueryParamCollection(QueryParamCollection other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (other._values?.Count is not > 0)
        {
            return;
        }

        _values = new KeyValueCollection<QueryParamValue>(other._values.Count, true);
        _values.AddRange(other._values);
    }

    /// <inheritdoc />
    public int Count => _values?.Count ?? 0;

    /// <inheritdoc />
    public KeyValuePair<string, string?> this[int index]
    {
        get
        {
            if (_values is null)
            {
                throw new IndexOutOfRangeException(
                    "Index was out of range. Must be non-negative and less than the size of the collection."
                );
            }

            var result = _values[index];
            return new KeyValuePair<string, string?>(result.Key, result.Value.Value);
        }
    }

    /// <summary>
    /// Retrieves the first value associated with the specified key or a default value if the key is not found.
    /// </summary>
    /// <param name="key">The key for which to retrieve the value.</param>
    [ExcludeFromCodeCoverage]
    public string? this[string key] => FirstOrDefault(key);

    /// <inheritdoc />
    public override string ToString()
    {
        if (_toString is not null && !_changed)
        {
            return _toString;
        }

        if (_values?.Count is not > 0)
        {
            return string.Empty;
        }

        var sb = StringBuilderPool.Rent();
        var isFirstParam = true;
        foreach ((string key, QueryParamValue value) in _values)
        {
            if (!isFirstParam)
            {
                sb.Append('&');
            }

            isFirstParam = false;
            sb.Append(EncodeIllegalCharacters(key));
            if (value.Value is null)
            {
                continue;
            }

            sb.Append('=');
            sb.Append(value.EncodedValue);
        }

        _changed = false;
        return _toString = StringBuilderPool.ToStringAndReturn(sb);
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, string?>> GetEnumerator() =>
        _values?
            .Select(x => new KeyValuePair<string, string?>(x.Key, x.Value.Value))
            .GetEnumerator()
        ?? Enumerable.Empty<KeyValuePair<string, string?>>().GetEnumerator();

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public string? FirstOrDefault(string key) => _values?.FirstOrDefault(key).Value;

    /// <inheritdoc />
    public bool TryGetFirst(string key, out string? value)
    {
        value = null;
        if (_values is null || !_values.TryGetFirst(key, out var result))
        {
            return false;
        }

        value = result.Value;
        return true;
    }

    /// <inheritdoc />
    public IEnumerable<string?> GetAll(string key) =>
        _values?.GetAll(key).Select(x => x.Value) ?? EmptyStrings;

    /// <inheritdoc />
    public bool Contains(string key) => _values?.Contains(key) ?? false;

    /// <summary>
    /// Creates a copy of this <see cref="QueryParamCollection"/>.
    /// </summary>
    /// <returns>
    /// The created <see cref="QueryParamCollection"/>.
    /// </returns>
    [Pure]
    public QueryParamCollection Clone() => new(this);

    /// <summary>
    /// Adds a query parameter.
    /// </summary>
    /// <param name="key">Key of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="nullValueHandling">Describes how to handle null values.</param>
    public void Add(string key, string? value, NullValueHandling nullValueHandling = NullValueHandling.Remove)
    {
        switch ((value, nullValueHandling))
        {
            case (null, NullValueHandling.Remove):
                _values?.Remove(key);
                _changed = true;
                break;
            case (null, NullValueHandling.Ignore):
                break;
            default:
                _values ??= new KeyValueCollection<QueryParamValue>(true);
                _values.Add(key, new QueryParamValue(value, false));
                _changed = true;
                break;
        }
    }

    /// <summary>
    /// Replaces existing query parameter(s) or adds to the end.
    /// </summary>
    /// <param name="key">Key of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="nullValueHandling">Describes how to handle null values.</param>
    public void AddOrReplace(string key, string? value, NullValueHandling nullValueHandling = NullValueHandling.Remove)
    {
        if (!Contains(key))
        {
            Add(key, value, nullValueHandling);
            return;
        }

        var replaced = false;
        ImmutableArray<KeyValuePair<string, QueryParamValue>> old = [.._values!];
        _values!.Clear();

        foreach (var item in old)
        {
            if (!item.Key.OrdinalEquals(key))
            {
                _values.Add(item);
                _changed = true;
                continue;
            }

            if (replaced)
            {
                continue;
            }

            switch ((value, nullValueHandling))
            {
                case (null, NullValueHandling.Ignore):
                    _values.Add(item);
                    _changed = true;
                    break;
                case (null, NullValueHandling.Remove):
                    break;
                default:
                    replaced = true;
                    Add(key, value, nullValueHandling);
                    break;
            }
        }
    }

    /// <summary>
    /// Removes all query parameters of the given name.
    /// </summary>
    /// <param name="key">The key to remove from the collection.</param>
    /// <returns>
    /// <c>true</c> if an entry with the specified key was successfully removed; otherwise, <c>false</c>.
    /// </returns>
    public bool Remove(string key)
    {
        if (_values?.Count is not > 0 || !_values.Remove(key))
        {
            return false;
        }

        _changed = true;
        return true;
    }

    /// <summary>
    /// Clears all query parameters from this collection.
    /// </summary>
    public void Clear()
    {
        _values?.Clear();
        _changed = _values?.Count is > 0;
    }

    /// <summary>
    /// Sorts the query parameter alphabetically.
    /// </summary>
    public void Sort()
    {
        _values?.Sort(ValueComparer.Instance);
        _changed = _values?.Count is > 0;
    }

    private static KeyValueCollection<QueryParamValue>? ParseQueryString(string queryString)
    {
        var querySpan = queryString.AsSpan();
        if (querySpan.TrimStart('?').IsWhiteSpace())
        {
            return null;
        }

        var ampersandCount = ReadOnlySpanExtensions.Count(querySpan, '&');
        using var splitOwner = SpanOwner<Range>.Allocate(ampersandCount + 1);

        var splitDestination = splitOwner.Span;
        var splitCount = querySpan.Split(splitDestination, '&', StringSplitOptions.RemoveEmptyEntries);
        var result = new KeyValueCollection<QueryParamValue>(ampersandCount, true);
        for (var i = 0; i < splitCount; i++)
        {
            var range = splitDestination[i];
            var segmentSpan = querySpan[range];
            var indexOfFirstNonQuestionMarkChar = segmentSpan.IndexOfAnyExcept(QuestionMarkSearchValues);
            range = GetRangeWithOffset(range, indexOfFirstNonQuestionMarkChar);

            SplitKeyValue(queryString[range], out var key, out var value);
            result.Add(key, new QueryParamValue(value, true));
        }

        return result;
    }

    private static Range GetRangeWithOffset(in Range range, int offset) => offset <= 0
        ? range
        : new Range(new Index(range.Start.Value + offset, range.Start.IsFromEnd), range.End);

    private static void SplitKeyValue(string segment, out string key, out string? value)
    {
        key = string.Empty;
        value = null;

        var segmentSpan = segment.AsSpan();
        var separatorCount = ReadOnlySpanExtensions.Count(segmentSpan, '=');
        if (separatorCount == 0)
        {
            key = segment;
            return;
        }

        using var splitOwner = SpanOwner<Range>.Allocate(2);
        var splitDestination = splitOwner.Span;
        _ = segmentSpan.Split(splitDestination, '=');

        key = segment[splitDestination[0]];
        value = segment[splitDestination[1]];
    }

    [GeneratedRegex("(.*?)((%[0-9A-Fa-f]{2})|$)")]
    private static partial Regex IllegalCharactersRegex();

    // Adapted from https://github.com/tmenier/Flurl/blob/93f5668dc2438c677a9e3384d2460b6de9f99c02/src/Flurl/Url.cs#L737

    /// <summary>
    /// URL-encodes characters in a string that are neither reserved nor unreserved. Avoids encoding reserved
    /// characters such as '/' and '?'. Avoids encoding '%' if it begins a %-hex-hex sequence (i.e.
    /// avoids double-encoding).
    /// </summary>
    /// <param name="s">The string to encode.</param>
    /// <param name="encodeSpaceAsPlus">If true, spaces will be encoded as + signs. Otherwise, they'll be encoded as %20.</param>
    /// <returns>
    /// The encoded URL.
    /// </returns>
    private static string EncodeIllegalCharacters(string s, bool encodeSpaceAsPlus = false)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        if (encodeSpaceAsPlus)
        {
            s = s.Replace(" ", "+");
        }

        // Uri.EscapeUriString mostly does what we want - encodes illegal characters only - but it has a quirk
        // in that % isn't illegal if it's the start of a %-encoded sequence https://stackoverflow.com/a/47636037/62600

        // no % characters, so avoid the regex overhead
        if (!s.OrdinalContains("%"))
        {
#pragma warning disable SYSLIB0013 // EscapeUriString is deprecated
            return Uri.EscapeUriString(s);
#pragma warning restore SYSLIB0013
        }

        // pick out all %-hex-hex matches and avoid double-encoding
        return IllegalCharactersRegex().Replace(s, c =>
        {
            var a = c.Groups[1].Value; // group 1 is a sequence with no %-encoding - encode illegal characters
            var b = c.Groups[2].Value; // group 2 is a valid 3-character %-encoded sequence - leave it alone!
#pragma warning disable SYSLIB0013 // EscapeUriString is deprecated
            return $"{Uri.EscapeUriString(a)}{b}";
#pragma warning restore SYSLIB0013
        });
    }

    /// <summary>
    /// Describes how to handle null values in query parameters.
    /// </summary>
    public enum NullValueHandling
    {
        /// <summary>
        /// Set as name without value in query string.
        /// </summary>
        NameOnly,

        /// <summary>
        /// Don't add to query string, remove any existing value.
        /// </summary>
        Remove,

        /// <summary>
        /// Don't add to query string, but leave any existing value unchanged.
        /// </summary>
        Ignore
    }

    private sealed class ValueComparer : IComparer<KeyValuePair<string, QueryParamValue>>
    {
        public static ValueComparer Instance { get; } = new();

        public int Compare(KeyValuePair<string, QueryParamValue> x, KeyValuePair<string, QueryParamValue> y) =>
            string.Compare(x.Key, y.Key, StringComparison.OrdinalIgnoreCase);
    }

    private readonly struct QueryParamValue
    {
        public QueryParamValue(string? value, bool isEncoded)
        {
            if (value is null)
            {
                return;
            }

            if (isEncoded)
            {
                EncodedValue = value;
                Value = Decode(value, true);
            }
            else
            {
                Value = value;
                EncodedValue = Encode(value, true);
            }
        }

        public string? Value { get; }

        public string? EncodedValue { get; }

        // Adapted from https://github.com/tmenier/Flurl/blob/93f5668dc2438c677a9e3384d2460b6de9f99c02/src/Flurl/Url.cs#L711

        /// <summary>
        /// URL-encodes a string, including reserved characters such as '/' and '?'.
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <param name="encodeSpaceAsPlus">If true, spaces will be encoded as + signs. Otherwise, they'll be encoded as %20.</param>
        /// <returns>
        /// The encoded URL.
        /// </returns>
        private static string Encode(string s, bool encodeSpaceAsPlus = false)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            const int maxUrlLength = 65519;
            if (s.Length > maxUrlLength)
            {
                // Uri.EscapeDataString is going to throw because the string is "too long", so break it
                // into pieces and concat them
                var parts = new string[(int)Math.Ceiling((double)s.Length / maxUrlLength)];
                for (var i = 0; i < parts.Length; i++)
                {
                    var start = i * maxUrlLength;
                    var len = Math.Min(maxUrlLength, s.Length - start);
                    parts[i] = Uri.EscapeDataString(s.AsSpan(start, len));
                }

                s = string.Concat(parts);
            }
            else
            {
                s = Uri.EscapeDataString(s);
            }

            return encodeSpaceAsPlus ? s.Replace("%20", "+") : s;
        }

        // Adapted from https://github.com/tmenier/Flurl/blob/93f5668dc2438c677a9e3384d2460b6de9f99c02/src/Flurl/Url.cs#L690

        /// <summary>
        /// Decodes a URL-encoded string.
        /// </summary>
        /// <param name="s">The URL-encoded string.</param>
        /// <param name="interpretPlusAsSpace">If true, any '+' character will be decoded to a space.</param>
        /// <returns></returns>
        private static string Decode(string s, bool interpretPlusAsSpace) =>
            string.IsNullOrEmpty(s)
                ? s
                : Uri.UnescapeDataString(interpretPlusAsSpace ? s.Replace("+", " ") : s);
    }
}