using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Softweyr.Configuration;

namespace FileProcessorDemo
{
    public class StringToMailAddressArrayConverter : ITypeConverter
    {
        public object Convert(object source, System.Type targetType)
        {
            var strings = ((string) source).Split(new[] {',', ';'});
            var mailAddresses = strings.Select(str => new MailAddress(str));
            return mailAddresses.ToArray();
        }
    }
}