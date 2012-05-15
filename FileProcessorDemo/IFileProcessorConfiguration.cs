using System.Net.Mail;
using Softweyr.Configuration;

namespace FileProcessorDemo
{
    public interface IFileProcessorConfiguration
    {
        [ConfigureUsingAppConfigAppSetting]
        [ConfigureUsingDefaultValue(20)]
        int NumberOfJobs { get; set; }

        [ConfigureUsingAppConfigAppSetting(Precedence.Application + 1)]
        [ConfigureUsingAppConfigAppSetting("WorkerThreads")]
        [ConfigureUsingDefaultValue(3)]
        int MaxDegreeOfParallelism { get; set; }

        [ConfigureUsingAppConfigAppSetting(typeof(StringToMailAddressArrayConverter))]
        MailAddress[] AlertRecipients { get; set; }

        [ConfigureUsingAppConfigAppSetting]
        string AlertFrom { get; set; }

        [ConfigureUsingAppConfigAppSetting("EmailSubject")]
        string AlertSubject { get; set; }

        [ConfigureUsingAppConfigAppSetting("EmailBody")]
        string AlertBody { get; set; }
    }
}