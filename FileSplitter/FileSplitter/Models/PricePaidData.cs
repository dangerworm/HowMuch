using System;
using System.Collections.Generic;

namespace FileSplitter.Models
{
    public class PricePaidData : IProcessableLocationEntity
    {
        public PricePaidData(IReadOnlyList<string> locationData)
        {
            if (int.TryParse(locationData[0], out var price))
            {
                Price = price;
            }

            if (DateTime.TryParse(locationData[1], out var dateOfTransfer))
            {
                DateOfTransfer = dateOfTransfer;
            }

            Postcode = locationData[2].Split(" ");

            PostcodeFull = locationData[2];

            PropertyType = locationData[3] switch
            {
                "F" => "Flats/Maisonettes",
                "D" => "Detached",
                "S" => "Semi-Detached",
                "T" => "Terraced",
                _ => "Other"
            };

            OldOrNew = locationData[4] switch
            {
                "Y" => "New Build",
                "N" => "Established",
                _ => "Unknown"
            };

            Duration = locationData[5] switch
            {
                "F" => "Freehold",
                "L" => "Leasehold",
                _ => "Unknown"
            };

            Paon = locationData[6];

            Saon = locationData[7];

            Street = locationData[8];

            Locality = locationData[9];

            TownOrCity = locationData[10];

            District = locationData[11];

            County = locationData[12];

            PpdCategoryType = locationData[13] switch
            {
                "A" => "Standard",
                "B" => "Additional",
                _ => "Unknown"
            };

            RecordStatus = locationData[14] switch
            {
                "A" => "Addition",
                "C" => "Change",
                "D" => "Delete",
                _ => "Unknown"
            };
        }

        public string PostcodeStart => Postcode[0];

        public int Price { get; }

        public DateTime DateOfTransfer { get; }

        public string[] Postcode { get; }

        public string PostcodeFull { get; }

        public string PropertyType { get; }

        public string OldOrNew { get; }

        public string Duration { get; }

        public string Paon { get; }

        public string Saon { get; }

        public string Street { get; }

        public string Locality { get; }

        public string TownOrCity { get; }

        public string District { get; }

        public string County { get; }

        public string PpdCategoryType { get; }

        public string RecordStatus { get; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string[] AsStringArray()
        {
            return new[]
            {
                $"{Price:#}",
                $"{DateOfTransfer:u}",
                PostcodeFull,
                PropertyType,
                OldOrNew,
                Duration,
                Paon,
                Saon,
                Street,
                Locality,
                TownOrCity,
                District,
                County,
                PpdCategoryType,
                RecordStatus,
                $"{Latitude:F10}",
                $"{Longitude:F10}"
            };
        }
    }
}
