using System.ComponentModel;
using System.Globalization;
using System.Net.Mail;

namespace AnxiousAnt;

[TypeConverter(typeof(EmailAddressTypeConverter))]
partial struct EmailAddress
{
    internal sealed class EmailAddressTypeConverter : TypeConverter
    {
        private static readonly Type StringType = typeof(string);
        private static readonly Type MailAddressType = typeof(MailAddress);

        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
            sourceType == StringType || sourceType == MailAddressType ||
            base.CanConvertFrom(context, sourceType);

        /// <inheritdoc />
        public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType) =>
            destinationType == StringType || destinationType == MailAddressType ||
            base.CanConvertTo(context, destinationType);

        /// <inheritdoc />
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) =>
            value switch
            {
                string str => string.IsNullOrWhiteSpace(str) ? default : Parse(str),
                MailAddress mailAddress => new EmailAddress(mailAddress),
                _ => base.ConvertFrom(context, culture, value)
            };

        /// <inheritdoc />
        public override object? ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type destinationType)
        {
            if (value is not EmailAddress emailAddress)
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            if (destinationType == StringType)
            {
                return emailAddress.IsValid ? emailAddress.ToString() : null;
            }

            if (destinationType == MailAddressType)
            {
                return emailAddress.IsValid
                    ? new MailAddress(emailAddress.Address, emailAddress.DisplayName)
                    : null;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}