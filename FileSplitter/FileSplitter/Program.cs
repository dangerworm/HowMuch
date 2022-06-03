﻿using FileSplitter.Processors;

namespace FileSplitter
{
    public class Program
    {
        private const string InputDirectory = "Input";
        private const string LocationOutputDirectory = "LocationOutput";
        private const string HouseDataOutputDirectory = "HouseDataOutput";

        static void Main(string[] args)
        {
            // ukpostcodes.csv taken from https://www.freemaptools.com/download-uk-postcode-lat-lng.htm

            var latLongProcessor = new PostcodeLatLongProcessor(InputDirectory, "ukpostcodes.csv", LocationOutputDirectory);
            latLongProcessor.Process();

            var pricePaidProcessor = new PricePaidProcessor(InputDirectory, "pp-2022.csv", HouseDataOutputDirectory, latLongProcessor.KeyedLocationData);
            pricePaidProcessor.Process();
        }
    }
}