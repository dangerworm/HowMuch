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

            Console.Write("Loading input file...");
            var csvOptions = new CsvOptions
            {
                HeaderMode = HeaderMode.HeaderAbsent
            };

            EntityData = CsvReader.ReadFromStream(stream, csvOptions)
                .Select(x => new LocationData(x.Values.Skip(1).ToArray()))
                .Where(x => x.Latitude != 0 && x.Longitude != 0)
                .ToArray();
            Console.WriteLine("done.");

            stream.Dispose();

            KeyedLocationData = CreateLocationFiles();
        }
    }
}
