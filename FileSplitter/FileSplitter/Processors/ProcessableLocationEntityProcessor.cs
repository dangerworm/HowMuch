﻿using System;
using System.Collections.Generic;
using System.IO;
using FileSplitter.Models;

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

        protected static string TruncateCoordinates(double value)
        {
            return $"{Math.Truncate(value * 100) / 100:0.00}";
        }
    }
}
