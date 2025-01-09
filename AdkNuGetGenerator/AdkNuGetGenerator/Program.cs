// <copyright file="Program.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

namespace AdkNuGetGenerator
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Generates NuGet packages for certain components of the Android SDK.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Runs the program.
        /// </summary>
        /// <param name="args">
        /// Command line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            // Load the normal & addon repositories.
            var repository = await Repository2.Load("https://dl.google.com/android/repository/repository2-1.xml");

            // Download the platform-tools, build-tools and usb_driver components.
            DirectoryInfo targetDirectory = new DirectoryInfo(Environment.CurrentDirectory);

            Version latestBuildToolsVersion = new Version(33, 0, 1);
            Version latestPlatformToolsVersion = new Version(33, 0, 3);
            Version latestUsbDriverVersion = new Version(12, 0, 0);

            var recentBuildTools = repository.BuildTools.Where(c => c.Revision.ToVersion() > latestBuildToolsVersion && !c.Revision.Preview).ToArray();
            var recentPlatformTools = repository.PlatformTools.Where(c => c.Revision.ToVersion() > latestPlatformToolsVersion && !c.Revision.Preview).ToArray();

            Console.WriteLine($"Processing {recentBuildTools.Length} build tool packages, {recentPlatformTools.Length} platform tool packages");

            string versionSuffix = string.Empty; // use things like -beta004 if you want to add NuGet version suffixes

            await PackageGenerator.GeneratePackages(
                recentBuildTools,
                File.ReadAllText("adk-build-tools.nuspec"),
                targetDirectory,
                false,
                versionSuffix);

            await PackageGenerator.GeneratePackages(
                recentPlatformTools,
                File.ReadAllText("adk-platform-tools.nuspec"),
                targetDirectory,
                false,
                versionSuffix);
        }
    }
}
