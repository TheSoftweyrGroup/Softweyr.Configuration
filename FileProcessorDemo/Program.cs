using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Softweyr.Configuration;

namespace FileProcessorDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configure what methods of configuration we're going to use.
            Configure.TheEnvironment.AddConfigurationMethod<DefaultConfigurationMethodProvider>();
            Configure.TheEnvironment.AddConfigurationMethod<AppConfigConfigurationMethodProvider>();

            var configuration = Configure.Get<IFileProcessorConfiguration>();
            var myFileProcessor = new FileProcessor(configuration);
            myFileProcessor.ProcessFile();
            Console.WriteLine("Press enter to close.");
            Console.ReadLine();
        }
    }
}
