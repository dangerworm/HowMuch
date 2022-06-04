using Csv;
using FileSplitter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            Console.Write("Counting lines in house price data file...");
            var reader = File.OpenText(InputPath);
            var numberOfLines = CountLines(reader);
            Console.WriteLine($"{numberOfLines:###,###,###,###}.");

            var divisor = 1000.0;
            double logStep;
            do
            {
                logStep = Math.Floor(numberOfLines / divisor);
                divisor *= 10;
            } while (logStep > 1000);

            const string message = "Processing price data...";
            Console.Write(message);

            var csvOptions = new CsvOptions { HeaderMode = HeaderMode.HeaderAbsent };
            var counter = 0;
            string line;
            while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
            {
                if (counter % logStep == 0)
                {
                    Console.CursorLeft = message.Length;
                    Console.Write($"line {counter:###,###,###,###} of {numberOfLines:###,###,###,###} ({counter * 100.0 / numberOfLines:F1}%)");
                }

                var pricePaidItem = CsvReader.ReadFromText(line, csvOptions)
                    .Select(x => new PricePaidData(x.Values.Skip(1).ToArray()))
                    .SingleOrDefault(x => x.RecordStatus != "Deleted" && x.PostcodeFull != string.Empty);

                counter++;

                if (pricePaidItem == null)
                {
                    continue;
                }

                AddLatLongData(pricePaidItem);

                if (pricePaidItem.Latitude == 0 && pricePaidItem.Longitude == 0)
                {
                    continue;
                }

                AppendToFiles(pricePaidItem);
            }
            
            Console.CursorLeft = message.Length;
            Console.WriteLine("done.                                                      ");

            reader.Dispose();

            Console.WriteLine();
        }

        private static int CountLines(StreamReader reader)
        {
            var numberOfLines = 0;
            while (reader.ReadLine() != null)
            {
                numberOfLines++;
            }

            reader.BaseStream.Position = 0;
            reader.DiscardBufferedData();

            return numberOfLines;
        }

        private void AddLatLongData(PricePaidData item)
        {
            if (!KeyedLocationData.ContainsKey(item.PostcodeStart))
            {
                return;
            }

            var location = KeyedLocationData[item.PostcodeStart].SingleOrDefault(x => x.PostcodeFull == item.PostcodeFull);
            if (location == null)
            {
                return;
            }

            item.Latitude = location.Latitude;
            item.Longitude = location.Longitude;
        }

        private void AppendToFiles(IProcessableLocationEntity item)
        {
            var csvColumns = item.AsStringArray();

            var postcodePath = Path.Combine(OutputDirectoryPath, $"{item.PostcodeStart}.csv");
            var writer = File.AppendText(postcodePath);
            CsvWriter.Write(writer, new string[csvColumns.Length], new[] { csvColumns }, ',', true);
            writer.Dispose();

            var coordinatesPath = Path.Combine(OutputDirectoryPath, 
                $"{TruncateDouble(item.Latitude, 2)}," +
                $"{TruncateDouble(item.Longitude, 2)}.csv");

            writer = File.AppendText(coordinatesPath);
            CsvWriter.Write(writer, new string[csvColumns.Length], new[] { csvColumns }, ',', true);
            writer.Dispose();
        }
    }
}
