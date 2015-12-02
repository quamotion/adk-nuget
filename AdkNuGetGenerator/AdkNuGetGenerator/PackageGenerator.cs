// <copyright file="PackageGenerator.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

namespace AdkNuGetGenerator
{
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
        /// <param name="targetDirectory">
        /// The directory to which to download the packages.
        /// </param>
        /// <param name="overwrite">
        /// If set to <see langword="true"/>, any existing directory will be deleted.
        /// </param>
        /// <param name="hostOs">
        /// The operating system for which to download the pacakges.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation
        /// </returns>
        public static async Task<DirectoryInfo> DownloadAndExtract(IArchiveContainer packageContainer, DirectoryInfo targetDirectory, bool overwrite, string hostOs)
        {
            var package = packageContainer.Archives.Single(p => p.HostOs == hostOs);
            return await package.DownloadAndExtract(targetDirectory, overwrite: false);
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
        /// <param name="hostOs">
        /// The operating system for which to download the pacakges.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public static async Task GeneratePackages(IEnumerable<IArchiveContainer> packageContainers, string packageTemplate, DirectoryInfo targetDirectory, bool overwrite, string hostOs)
        {
            foreach (var package in packageContainers)
            {
                // Make sure the package is available locally
                var dir = await DownloadAndExtract(package, targetDirectory, overwrite, hostOs);
                string packagePath = $"{dir.FullName}.nuspec";

                dir = dir.EnumerateDirectories().Single();

                // Generate the .nuspec file
                string nugetPackage = packageTemplate.Replace("{Version}", package.Revision.ToString());
                nugetPackage = nugetPackage.Replace("{Dir}", dir.FullName);

                File.WriteAllText(packagePath, nugetPackage);
            }
        }
    }
}
