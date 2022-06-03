using Csv;
using FileSplitter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileSplitter.Processors
{
    public class ProcessableLocationEntityProcessor<T> where T : IProcessableLocationEntity
    {
        protected readonly string InputPath;
        protected readonly string OutputDirectoryPath;

        protected IReadOnlyCollection<T> EntityData;

        public ProcessableLocationEntityProcessor(string inputDirectory, string inputFile, string outputDirectory)
        {
            InputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, inputDirectory, inputFile);
            OutputDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, outputDirectory);

            if (!Directory.Exists(OutputDirectoryPath))
            {
                Directory.CreateDirectory(OutputDirectoryPath);
            }
        }

        protected IDictionary<string, T[]> CreateLocationFiles()
        {
            Console.Write("Processing postcodes...");
            var keyedLocationData = CreateOutputFiles(x => x.PostcodeStart, string.Empty);
            Console.WriteLine("done.");

            Console.Write("Processing latitudes...");
            CreateOutputFiles(x => x.Latitude.ToString("F2"), "N");
            Console.WriteLine("done.");

            Console.Write("Processing longitudes...");
            CreateOutputFiles(x => x.Longitude.ToString("F2"), "E");
            Console.WriteLine("done.");

            return keyedLocationData;
        }

        private IDictionary<string, T[]> CreateOutputFiles(Func<T, string> groupFunction, string filenamePrefix)
        {
            var keyedLocationData = EntityData
                .GroupBy(groupFunction)
                .ToDictionary(x => $"{filenamePrefix}{x.Key}", y => y.ToArray());

            keyedLocationData
                .ToList()
                .ForEach(CreateOutputFile);

            return keyedLocationData;
        }

        private void CreateOutputFile(KeyValuePair<string, T[]> pair)
        {
            var (filename, locations) = pair;

            var outputPath = Path.Combine(OutputDirectoryPath, $"{filename}.csv");
            var textWriter = File.CreateText(outputPath);

            var locationData = locations.Select(x => x.AsStringArray()).ToArray();
            
            CsvWriter.Write(textWriter, new string[locationData.First().Length], locationData, ',', true);

            textWriter.Dispose();
        }
    }
}
