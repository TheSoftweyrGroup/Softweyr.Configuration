using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileProcessorDemo
{
    internal class FileProcessor
    {
        private readonly IFileProcessorConfiguration configuration;

        public FileProcessor(IFileProcessorConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ProcessFile()
        {
            try
            {
                var jobs = new List<Action>();
                for (var i = 0; i < this.configuration.NumberOfJobs; i++)
                {
                    var innerId = i;
                    jobs.Add(() => this.DoWork(innerId));
                }

                var options = new ParallelOptions { MaxDegreeOfParallelism = this.configuration.MaxDegreeOfParallelism };
                Parallel.Invoke(options, jobs.ToArray());
                throw new InvalidOperationException("Something bad happened.");
            }
            catch (InvalidOperationException ex)
            {
                SendEmail(ex);
            }
        }

        private void SendEmail(Exception exception)
        {
            Console.WriteLine("From: {0}", configuration.AlertFrom);
            Console.WriteLine("To: {0}", string.Join(";", configuration.AlertRecipients.Select(address => address.Address)));
            Console.WriteLine("Subject: {0}", configuration.AlertSubject);
            Console.Write("Body: ");
            Console.WriteLine(configuration.AlertBody, exception);
        }

        public void DoWork(int jobId)
        {
            Console.WriteLine("Doing job {0} on thread {1}", jobId, Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(1000);
        }
    }
}