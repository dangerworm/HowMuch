using System.Collections.Generic;

namespace FileSplitter.Models
{
    public class LocationData : IProcessableLocationEntity
    {
        public LocationData(IReadOnlyList<string> locationData)
        {
            Postcode = locationData[0].Split(" ");
            
            PostcodeFull = locationData[0];

            Latitude = double.TryParse(locationData[1], out var latitude)
                ? latitude
                : 0;

            Longitude = double.TryParse(locationData[2], out var longitude)
                ? longitude
                : 0;
        }

        public string PostcodeStart => Postcode[0];

        public string[] Postcode { get; }

        public string PostcodeFull { get; }

        public double Latitude { get; }

        public double Longitude { get; }

        public string[] AsStringArray()
        {
            return Latitude == 0 || Longitude == 0
                ? new string[0]
                : new[] {$"{Postcode[0]} {Postcode[1]}", $"{Latitude:F10}", $"{Longitude:F10}"};
        }
    }
}
