using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication22
{
    using System.Collections;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.IO.Pipes;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

    class Program
    {
        private readonly Type BaseConfigurationRecordType;

        private readonly Type ClientConfigurationHostType;

        static void Main(string[] args)
        {
            var startTime = DateTime.Now;
            var program = new Program();
            var xDocument = program.GetCompleteConfigurationAsXDocument();
            var endTime = DateTime.Now;
            Console.WriteLine("Duration: {0}", endTime.Subtract(startTime));

            var xnm = new XmlNamespaceManager(new NameTable());
            var xElement = (IEnumerable)xDocument.XPathEvaluate("/configuration/appSettings/add/@key", xnm);
            foreach (XAttribute element in xElement)
            {
                Console.WriteLine(element.Value.ToString());
            }

            /*
            using (var sr = new StreamReader(program.GetCompleteConfigurationAsStream()))
            {
                Console.WriteLine(sr.ReadToEnd());
            } */

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private XDocument GetCompleteConfigurationAsXDocument()
        {
            return XDocument.Load(GetCompleteConfigurationAsStream());
        }

        private void WriteConfiguration(XmlWriter xmlWriter)
        {
            var filename = Path.GetTempFileName();
            try
            {
                this.SaveAs(filename);
                const int BufferSize = 1024;
                var buffer = new char[BufferSize];
                using (var fileStream = new StreamReader(filename))
                {
                    var readAmount = 0;
                    while ((readAmount = fileStream.Read(buffer, 0, BufferSize)) > 0)
                    {
                        xmlWriter.WriteRaw(buffer, 0, readAmount);
                    }
                }
            }
            finally
            {
                File.Delete(filename);
            }
        }

        private void WriteConfiguration(Stream stream)
        {
            var filename = Path.GetTempFileName();
            try
            {
                this.SaveAs(filename);
                const int BufferSize = 1024;
                var buffer = new byte[BufferSize];
                using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var readAmount = 0;
                    while ((readAmount = fileStream.Read(buffer, 0, BufferSize)) > 0)
                    {
                        stream.Write(buffer, 0, readAmount);
                    }
                }
            }
            finally
            {
                File.Delete(filename);
            }
        }

        private MemoryStream GetCompleteConfigurationAsStream()
        {
            // TODO: Read from the file stream instead of memory stream.
            var memoryStream = new MemoryStream();
            try
            {
                WriteConfiguration(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
            }

            return memoryStream;
        }

        public Program()
        {
            var configurationAssembly = typeof(Configuration).Assembly;
            this.BaseConfigurationRecordType = configurationAssembly.GetType("System.Configuration.BaseConfigurationRecord", true, false);
            this.ClientConfigurationHostType = configurationAssembly.GetType("System.Configuration.ClientConfigurationHost", true, false);
        }

        public void SaveAs(string filePath)
        {
            var configuration = this.GetCompleteConfiguration();
            var filename = filePath;
            var saveMode = ConfigurationSaveMode.Full;
            var forceUpdateAll = true;
            configuration.SaveAs(filename, saveMode, forceUpdateAll);
        }

        private Configuration GetCompleteConfiguration()
        {
            ForceConfigurationInitialization();
            var completeConfigurationRecord = GetCompleteConfigurationRecord();
            var configuration = CreateNewConfiguration();
            ReplaceConfigurationSectionRecords(configuration, completeConfigurationRecord);
            return configuration;
        }

        private void ReplaceConfigurationSectionRecords(Configuration configuration, object completeConfigurationRecord)
        {
            var fieldInfo = configuration.GetType().GetField("_configRoot", BindingFlags.NonPublic | BindingFlags.Instance);
            var configRoot = fieldInfo.GetValue(configuration);
            fieldInfo = configRoot.GetType().GetField(
                "_rootConfigRecord", BindingFlags.NonPublic | BindingFlags.Instance);
            var rootConfigRecord = fieldInfo.GetValue(configRoot);
            fieldInfo = BaseConfigurationRecordType.GetField("_sectionRecords", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(rootConfigRecord, fieldInfo.GetValue(completeConfigurationRecord));
        }

        private Configuration CreateNewConfiguration()
        {
            var configurationType = typeof(Configuration);
            var configurationConstructor =
                configurationType.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new[] { typeof(string), typeof(Type), typeof(object[]) },
                    null);
            return (Configuration)configurationConstructor.Invoke(
                new object[]
                    {
                        string.Empty, this.ClientConfigurationHostType, new object[3] { null, null, "MACHINE/EXE" } 
                    });
        }

        private object GetCompleteConfigurationRecord()
        {
            var clientConfigurationSystem = GetClientConfigurationSystem();
            // Get the complete configuration record.
            var fieldInfo = clientConfigurationSystem.GetType().GetField(
                "_completeConfigRecord", BindingFlags.NonPublic | BindingFlags.Instance);
            return fieldInfo.GetValue(clientConfigurationSystem);
        }

        private void ForceConfigurationInitialization()
        {
            var configurationManagerType = typeof(ConfigurationManager);

            // Force Initialization of the configuration system.
            var methodInfo = configurationManagerType.GetMethod(
                "PrepareConfigSystem", BindingFlags.NonPublic | BindingFlags.Static);
            methodInfo.Invoke(null, new object[] { });

            var clientConfigurationSystem = GetClientConfigurationSystem();

            // Force 
            methodInfo = clientConfigurationSystem.GetType().GetMethod(
                "EnsureInit", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(clientConfigurationSystem, new object[] { null });
        }

        private object GetClientConfigurationSystem()
        {
            var configurationManagerType = typeof(ConfigurationManager);

            // Get the configuration system.
            var fieldInfo = configurationManagerType.GetField(
                "s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);
            return fieldInfo.GetValue(null);
        }
    }
}
