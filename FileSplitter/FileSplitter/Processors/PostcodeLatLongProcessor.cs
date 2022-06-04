using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Csv;
using FileSplitter.Models;

namespace FileSplitter.Processors
{
    public class PostcodeLatLongProcessor : ProcessableLocationEntityProcessor<LocationData>
    {
        public PostcodeLatLongProcessor(string inputDirectory, string inputFile, string outputDirectory)
            : base(inputDirectory, inputFile, outputDirectory)
        {
        }

        public IDictionary<string, LocationData[]> KeyedLocationData;

        public void Process()
        {
            var stream = File.OpenRead(InputPath);
            var csvOptions = new CsvOptions
            {
                HeaderMode = HeaderMode.HeaderAbsent
            };
            
            Console.Write("Processing location data...");
            EntityData = CsvReader.ReadFromStream(stream, csvOptions)
                .Select(x => new LocationData(x.Values.Skip(1).ToArray()))
                .Where(x => x.Latitude != 0 && x.Longitude != 0)
                .ToArray();
            Console.WriteLine("done.");

            stream.Dispose();

            KeyedLocationData = CreateLocationFiles();
        }

        private IDictionary<string, LocationData[]> CreateLocationFiles()
        {
            Console.WriteLine();
            var keyedLocationData = CreateOutputFiles(x => x.PostcodeStart, "postcodes");
            Console.Write("done.");

            Console.WriteLine();
            CreateOutputFiles(x => $"{TruncateCoordinates(x.Latitude)},{TruncateCoordinates(x.Longitude)}", "coordinates");
            Console.Write("done.");

            Console.WriteLine();

            return keyedLocationData;
        }

        private IDictionary<string, LocationData[]> CreateOutputFiles(Func<LocationData, string> groupFunction, string outputType, string filenamePrefix = "")
        {
            var message = $"Processing {outputType}...";
            Console.Write(message);

            var keyedLocationData = EntityData
                .GroupBy(groupFunction)
                .ToDictionary(x => $"{filenamePrefix}{x.Key}", y => y.ToArray());

            var locationDataList = keyedLocationData.ToList();

            var percentageStep = Math.Floor(locationDataList.Count / 100.0);
            var counter = 0;
            foreach (var pair in locationDataList)
            {
                if (counter++ % percentageStep == 0)
                {
                    Console.CursorLeft = message.Length;
                    Console.Write($"{counter * 100 / locationDataList.Count}%");
                }

                CreateOutputFile(pair);
            }

            Console.CursorLeft = message.Length;

            return keyedLocationData;
        }

        private void CreateOutputFile(KeyValuePair<string, LocationData[]> pair)
        {
            var (filename, locations) = pair;

            var outputPath = Path.Combine(OutputDirectoryPath, $"{filename}.csv");
            var writer = File.CreateText(outputPath);

            var locationData = locations.Select(x => x.AsStringArray()).ToArray();
            
            CsvWriter.Write(writer, new string[locationData.First().Length], locationData, ',', true);

            writer.Dispose();
        }
    }
}
