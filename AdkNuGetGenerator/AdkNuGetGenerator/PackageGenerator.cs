// <copyright file="PackageGenerator.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

namespace AdkNuGetGenerator
{
    using NuGet;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Generates NuGet packages for Android SDK components.
    /// </summary>
    public static class PackageGenerator
    {
        /// <summary>
        /// Downloads one or more packages.
        /// </summary>
        /// <param name="packageContainer">
        /// The packageto downloaded.
        /// </param>
        /// <param name="repositoryDirectory">
        /// The directory to which to download the packages.
        /// </param>
        /// <param name="overwrite">
        /// If set to <see langword="true"/>, any existing directory will be deleted.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation
        /// </returns>
        public static async Task<DirectoryInfo> DownloadAndExtract(IArchiveContainer packageContainer, DirectoryInfo repositoryDirectory, bool overwrite)
        {
            // Create the directory into which to extract
            string directoryName = $"{packageContainer.Name}-{packageContainer.Revision}";

            // Figure out whether that directory already exists.
            DirectoryInfo targetDirectory;
            targetDirectory = repositoryDirectory.EnumerateDirectories(directoryName).SingleOrDefault();

            // If it does, proceed based on the value of the overwrite flag
            if (targetDirectory != null)
            {
                if (overwrite)
                {
                    // Delete the directory & proceed as usual.
                    targetDirectory.Delete(true);
                    targetDirectory = null;
                }
                else
                {
                    // Nothing left to do.
                    return targetDirectory;
                }
            }

            targetDirectory = repositoryDirectory.CreateSubdirectory(directoryName);

            foreach (var package in packageContainer.Archives)
            {
                Console.WriteLine($"Downloading package {package.Url} for {package.HostOs}");
                await package.DownloadAndExtract(targetDirectory);
            }

            return targetDirectory;
        }

        /// <summary>
        /// Geneates NuGet packages for Android components.
        /// </summary>
        /// <param name="packageContainers">
        /// The Android components for which to generate NuGet packages.
        /// </param>
        /// <param name="packageTemplate">
        /// The template for the <c>.nuspec</c> file.
        /// </param>
        /// <param name="targetDirectory">
        /// The directory to which to download the packages.
        /// </param>
        /// <param name="overwrite">
        /// If set to <see langword="true"/>, any existing directory will be deleted.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public static async Task GeneratePackages(IEnumerable<IArchiveContainer> packageContainers, string packageTemplate, DirectoryInfo targetDirectory, bool overwrite, string versionSuffix)
        {
            Dictionary<string, string> runtimes = new Dictionary<string, string>();
            runtimes.Add("win", "windows");
            runtimes.Add("linux", "linux");
            runtimes.Add("osx", "macosx");

            foreach (var package in packageContainers)
            {
                Console.WriteLine($"Generating package {package.ToString()}");

                // Make sure the package is available locally
                var dir = await DownloadAndExtract(package, targetDirectory, overwrite);

                // Generate the .nuspec file
                foreach (var runtime in runtimes)
                {
                    string packagePath = $"{dir.FullName}-{runtime.Key}.nuspec";

                    string nugetPackage = packageTemplate.Replace("{Version}", package.Revision.ToSematicVersion().ToString() + versionSuffix);

                    switch (runtime.Key)
                    {
                        case "win":
                            nugetPackage = nugetPackage.Replace("{PlatformSpecific}", string.Empty);
                            nugetPackage = nugetPackage.Replace("{Dependencies}", @"<dependency id=""runtime.win7-x64.vcruntime140"" version=""14.0.24406-r158"" /><dependency id=""runtime.win7-x86.vcruntime140"" version=""14.0.24406-r158"" />");
                            nugetPackage = nugetPackage.Replace("{LibPrefix}", string.Empty);
                            nugetPackage = nugetPackage.Replace("{LibExtension}", ".dll");
                            break;

                        case "linux":
                            nugetPackage = nugetPackage.Replace("{PlatformSpecific}", string.Empty);
                            nugetPackage = nugetPackage.Replace("{Dependencies}", string.Empty);
                            nugetPackage = nugetPackage.Replace("{LibPrefix}", "lib");
                            nugetPackage = nugetPackage.Replace("{LibExtension}", ".so");
                            break;

                        case "osx":
                            nugetPackage = nugetPackage.Replace("{PlatformSpecific}", string.Empty);
                            nugetPackage = nugetPackage.Replace("{Dependencies}", string.Empty);
                            nugetPackage = nugetPackage.Replace("{LibPrefix}", "lib");
                            nugetPackage = nugetPackage.Replace("{LibExtension}", ".dylib");
                            break;
                    }

                    nugetPackage = nugetPackage.Replace("{Dir}", dir.FullName);
                    nugetPackage = nugetPackage.Replace("{Runtime}", runtime.Key);
                    nugetPackage = nugetPackage.Replace("{OS}", runtime.Value);

                    File.WriteAllText(packagePath, nugetPackage);

                    PackageBuilder builder = new PackageBuilder(packagePath, null, false);
                    var packageOutputPath = Path.Combine(targetDirectory.FullName, $"{builder.Id}-{builder.Version}.nupkg");

                    using (Stream stream = File.Open(packageOutputPath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        builder.Save(stream);
                    }
                }
            }
        }
    }
}
