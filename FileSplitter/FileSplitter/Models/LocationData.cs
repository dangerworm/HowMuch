using System.Collections.Generic;

namespace FileSplitter.Models
{
    public class LocationData : IProcessableLocationEntity
    {
        public LocationData(IReadOnlyList<string> locationData)
        {
            PostcodeFull = locationData[0];

            Latitude = double.TryParse(locationData[1], out var latitude)
                ? latitude
                : 0;

            Longitude = double.TryParse(locationData[2], out var longitude)
                ? longitude
                : 0;
        }

        public string PostcodeStart => Postcode[0];

        public string[] Postcode => PostcodeFull.Split(" ");

        public string PostcodeFull { get; }

        public double Latitude { get; }

        public double Longitude { get; }

        public string[] AsStringArray()
        {
            return Latitude == 0 || Longitude == 0
                ? new string[0]
                : new[] { PostcodeFull, $"{Latitude:F10}", $"{Longitude:F10}" };
        }
    }
}
