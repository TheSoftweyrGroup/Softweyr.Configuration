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
            /* The hard bit */
            Configure.TheEnvironment.ImplicitlyAddConfigurationMethods();
            var configuration = Configure.Get<IFileProcessorConfiguration>();

            /* The bit you do all the time */
            var myFileProcessor = new FileProcessor(configuration);
            myFileProcessor.ProcessFile();
            Console.WriteLine("Press enter to close.");
            Console.ReadLine();
        }
    }
}
