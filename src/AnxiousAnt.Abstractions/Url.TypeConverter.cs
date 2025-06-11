using System.ComponentModel;
using System.Globalization;

namespace AnxiousAnt;

[TypeConverter(typeof(UrlTypeConverter))]
partial class Url
{
    internal sealed class UrlTypeConverter : TypeConverter
    {
        private static readonly Type StringType = typeof(string);
        private static readonly Type UriType = typeof(Uri);

        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
            sourceType == StringType || sourceType == UriType || base.CanConvertFrom(context, sourceType);

        /// <inheritdoc />
        public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType) =>
            destinationType == StringType || destinationType == UriType || base.CanConvertTo(context, destinationType);

        /// <inheritdoc />
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) =>
            value switch
            {
                string str => string.IsNullOrWhiteSpace(str) ? null : Parse(str),
                Uri uri => new Url(uri),
                _ => base.ConvertFrom(context, culture, value)
            };

        /// <inheritdoc />
        public override object? ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type destinationType)
        {
            if (value is not Url url)
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            return destinationType == StringType
                ? url.ToString()
                : destinationType == UriType
                    ? url.ToUri()
                    : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}