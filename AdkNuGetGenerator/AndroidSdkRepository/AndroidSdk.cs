// <copyright file="AndroidSdk.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

using Microsoft.Build.Framework;
using System.Diagnostics;

namespace AndroidSdkRepository
{
    public class AndroidSdk : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// Gets or sets the version to download.
        /// </summary>
        public required string Version { get; set; }

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
        /// A <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task<DirectoryInfo> DownloadAndExtract(IArchiveContainer packageContainer, DirectoryInfo repositoryDirectory, bool overwrite)
        {
            // Create the directory into which to extract
            string directoryName = $"{packageContainer.Name}-{packageContainer.Revision}";

            // Figure out whether that directory already exists.
            DirectoryInfo? targetDirectory;
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

        /// <inheritdoc/>
        public override bool Execute()
        {
            var repository = Repository2.Load("https://dl.google.com/android/repository/repository2-1.xml").Result;

            // Download the platform-tools, build-tools and usb_driver components.
            DirectoryInfo targetDirectory = new DirectoryInfo(Environment.CurrentDirectory);

            Version latestBuildToolsVersion = new Version(this.Version);

            var recentBuildTool = repository.PlatformTools.Where(c => c.Revision!.ToVersion() == latestBuildToolsVersion).First();

            // Make sure the package is available locally
            Console.WriteLine($"Generating package {recentBuildTool.ToString()}");
            var dir = this.DownloadAndExtract(recentBuildTool, targetDirectory, true).Result;
            return true;
        }
    }
}
