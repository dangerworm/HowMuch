using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Csv;
using FileSplitter.Models;

namespace FileSplitter.Processors
{
    public class PricePaidProcessor : ProcessableLocationEntityProcessor<PricePaidData>
    {
        public PricePaidProcessor(string inputDirectory, string inputFile, string outputDirectory, IDictionary<string, LocationData[]> keyedLocationData)
            : base(inputDirectory, inputFile, outputDirectory)
        {
            KeyedLocationData = keyedLocationData;
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
                .Select(x => new PricePaidData(x.Values.Skip(1).ToArray()))
                .ToArray();
            Console.WriteLine("done.");

            stream.Dispose();

            Console.Write("Adding lat/long information to entities...");
            AddLatLongData();
            Console.WriteLine("done.");

            CreateLocationFiles();
        }

        private void AddLatLongData()
        {
            foreach (var entity in EntityData)
            {
                if (!KeyedLocationData.ContainsKey(entity.PostcodeStart) || 
                    KeyedLocationData[entity.PostcodeStart].All(x => x.PostcodeFull != entity.PostcodeFull)) 
                    continue;

                var location = KeyedLocationData[entity.PostcodeStart].Single(x => x.PostcodeFull == entity.PostcodeFull);

                entity.Latitude = location.Latitude;
                entity.Longitude = location.Longitude;
            }
        }
    }
}
