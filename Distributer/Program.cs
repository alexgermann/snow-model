using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Distributer
{
    class Program
    {
        private static readonly string SnowClass = "Snow";
        private static readonly string NoSnowClass = "NoSnow";

        private static readonly string Dataset = @"/Users/trevor-trou/Documents/Git/snow-model/Datasets/";
        private static readonly string Test = @"/Users/trevor-trou/Documents/Git/snow-model/Test/";
        private static readonly string Train = @"/Users/trevor-trou/Documents/Git/snow-model/Train/";
        private static readonly string Validation = @"/Users/trevor-trou/Documents/Git/snow-model/Validation/";

        private static readonly decimal TrainPercent = new decimal(0.7);
        private static readonly decimal TestPercent = new decimal(0.15);
        private static readonly decimal ValidationPercent = new decimal(0.15);

        static void Main(string[] args)
        {
            if (TrainPercent + TestPercent + ValidationPercent != 1)
                throw new ArgumentException("Percentages must sum to 1");

            var SnowImages = Directory.GetFiles(Dataset + SnowClass);
            var NoSnowImages = Directory.GetFiles(Dataset + NoSnowClass);

            var NumSnowImages = SnowImages.Length;
            var NumNoSnowImages = NoSnowImages.Length;

            Console.WriteLine("Total images: {0} snow, {1} no snow.", NumSnowImages, NumNoSnowImages);

            Random rng = new Random();
            var RandomSnowImages = SnowImages.OrderBy(x => rng.Next()).ToArray();
            var RandomNoSnowImages = NoSnowImages.OrderBy(x => rng.Next()).ToArray();

            var TestSnowSet = RandomSnowImages.Take(((int)(NumSnowImages * TestPercent)));
            var TrainSnowSet = RandomSnowImages.Take(((int)(NumSnowImages * TrainPercent)));
            var ValidationSnowSet = RandomSnowImages.Take(((int)(NumSnowImages * ValidationPercent)));

            Console.WriteLine("Snow Distribution: {0} test, {1} train, {2} validation",
                TestSnowSet.Count(),
                TrainSnowSet.Count(),
                ValidationSnowSet.Count());

            var TestNoSnowSet = RandomNoSnowImages.Take(((int)(NumNoSnowImages * TestPercent)));
            var TrainNoSnowSet = RandomNoSnowImages.Take(((int)(NumNoSnowImages * TrainPercent)));
            var ValidationNoSnowSet = RandomNoSnowImages.Take(((int)(NumNoSnowImages * ValidationPercent)));

            Console.WriteLine("No Snow Distribution: {0} test, {1} train, {2} validation",
                TestNoSnowSet.Count(),
                TrainNoSnowSet.Count(),
                ValidationNoSnowSet.Count());

            // Copy Test Snow
            foreach(var img in TestSnowSet)
            {
                File.Copy(Path.Combine(img), Path.Combine(Test, SnowClass), true);
            }

            // Copy Test No Snow
            foreach (var img in TestNoSnowSet)
            {
                File.Copy(Path.Combine(img), Path.Combine(Test, NoSnowClass), true);
            }

            // Copy Training Snow
            foreach (var img in TrainSnowSet)
            {
                File.Copy(Path.Combine(img), Path.Combine(Train, SnowClass), true);
            }

            // Copy Training No Snow
            foreach (var img in TrainNoSnowSet)
            {
                File.Copy(Path.Combine(img), Path.Combine(Train, NoSnowClass), true);
            }

            // Copy Validation Snow
            foreach (var img in ValidationSnowSet)
            {
                File.Copy(Path.Combine(img), Path.Combine(Validation, SnowClass), true);
            }

            // Copy Validation No Snow
            foreach (var img in ValidationNoSnowSet)
            {
                File.Copy(Path.Combine(img), Path.Combine(Validation, NoSnowClass), true);
            }
        }
    }
}
