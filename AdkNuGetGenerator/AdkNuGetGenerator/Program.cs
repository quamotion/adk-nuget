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
            var repository = await Repository.Load("https://dl.google.com/android/repository/repository-12.xml");
            var addons = await Repository.Load("https://dl.google.com/android/repository/addon.xml");

            // Merge them so we have one repository to work with.
            repository.Merge(addons);

            // Download the platform-tools, build-tools and usb_driver components.
            DirectoryInfo targetDirectory = new DirectoryInfo(Environment.CurrentDirectory);

            Version latestBuildToolsVersion = new Version(25, 0, 1);
            Version latestPlatformToolsVersion = new Version(25, 0, 2);
            Version latestUsbDriverVersion = new Version(11, 0, 0);

            var recentBuildTools = repository.BuildTools.Where(c => c.Revision.ToVersion() > latestBuildToolsVersion && !c.Revision.Preview).ToArray();
            var recentPlatformTools = repository.PlatformTools.Where(c => c.Revision.ToVersion() > latestPlatformToolsVersion && !c.Revision.Preview).ToArray();
            var recentUsbDrivers = repository.Extras.Where(e => e.Path == "usb_driver").Where(c => c.Revision.ToVersion() > latestUsbDriverVersion && !c.Revision.Preview).ToArray();

            string versionSuffix = ".1-beta001"; // string.Empty; // use thinigs like -beta004 if you want to add NuGet version suffixes

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

            await PackageGenerator.GeneratePackages(
                recentUsbDrivers,
                File.ReadAllText("adk-usb-driver.nuspec"),
                targetDirectory,
                false,
                versionSuffix);
        }
    }
}
