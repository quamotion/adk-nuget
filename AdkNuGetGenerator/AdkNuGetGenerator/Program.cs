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
            var repository = await Repository.Load("https://dl.google.com/android/repository/repository-11.xml");
            var addons = await Repository.Load("https://dl.google.com/android/repository/addon.xml");

            // Merge them so we have one repository to work with.
            repository.Merge(addons);

            // Download the platform-tools, build-tools and usb_driver components.
            DirectoryInfo targetDirectory = new DirectoryInfo(Environment.CurrentDirectory);

            await PackageGenerator.GeneratePackages(repository.BuildTools, File.ReadAllText("adk-build-tools.nuspec"), targetDirectory, false, "windows");
            await PackageGenerator.GeneratePackages(repository.PlatformTools, File.ReadAllText("adk-platform-tools.nuspec"), targetDirectory, false, "windows");
            await PackageGenerator.GeneratePackages(repository.Extras.Where(e => e.Path == "usb_driver"), File.ReadAllText("adk-usb-driver.nuspec"), targetDirectory, false, "windows");
        }
    }
}
