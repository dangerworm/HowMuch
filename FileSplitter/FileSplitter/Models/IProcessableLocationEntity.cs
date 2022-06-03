﻿namespace FileSplitter.Models
{
    public interface IProcessableLocationEntity
    {
        public string[] Postcode { get; }
        
        public string PostcodeStart { get; }

        public string PostcodeFull { get; }
         
        public double Latitude { get; }

        public double Longitude { get; }

        public string[] AsStringArray();
    }
}