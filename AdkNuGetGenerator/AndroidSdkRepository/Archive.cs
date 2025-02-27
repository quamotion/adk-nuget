// <copyright file="Archive.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

namespace AndroidSdkRepository
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Represents an individual archive that is part of the Android SDK.
    /// </summary>
    public class Archive
    {
        /// <summary>
        /// Gets or sets the size, in bytes, of the archive.
        /// </summary>
        public int Size
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the algorithm that was used to calculate the <see cref="Checksum"/>.
        /// Should always be <c>sha1</c>.
        /// </summary>
        public string? ChecksumType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the checksum of the package as a hex-encoded string.
        /// </summary>
        public string? Checksum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URL from which the package can be downloaded.
        /// </summary>
        public Uri? Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the operating system which the package targets.
        /// </summary>
        public string? HostOs
        {
            get;
            set;
        }

        /// <summary>
        /// Loads a set of <see cref="Archive"/> objects from an <c>archives</c> element.
        /// </summary>
        /// <param name="value">
        /// The element from which to load the <see cref="Archive"/> objects.
        /// </param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> which is used to resolve relative package URLs.
        /// </param>
        /// <returns>
        /// A list of <see cref="Archive"/> objects which can be retrieved from the
        /// <c>archives</c> element.
        /// </returns>
        public static IEnumerable<Archive> FromArchivesXElement(XElement value, Uri baseUri)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Name.LocalName != "archives")
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            foreach (var child in value.Elements())
            {
                yield return FromXElement(child, baseUri);
            }
        }

        /// <summary>
        /// Loads a <see cref="Archive"/> from an <c>archive</c> element.
        /// </summary>
        /// <param name="value">
        /// The element from which to load the <see cref="Archive"/>.
        /// </param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> which is used to resolve relative package URLs.
        /// </param>
        /// <returns>
        /// A new <see cref="Archive"/> object.
        /// </returns>
        public static Archive FromXElement(XElement value, Uri baseUri)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Name.LocalName != "archive")
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            var archive = new Archive()
            {
                HostOs = (string)value.Element(value.Name.Namespace + "host-os")!,
            };

            if (value.Element("complete") != null)
            {
                var complete = value.Element("complete")!;
                archive.Size = (int)complete.Element(value.Name.Namespace + "size")!;
                archive.ChecksumType = (string)complete.Element(value.Name.Namespace + "checksum")!.Attribute("type")!;
                archive.Checksum = (string)complete.Element(value.Name.Namespace + "checksum")!;
                archive.Url = new Uri((string)complete.Element(value.Name.Namespace + "url")!, UriKind.RelativeOrAbsolute);
            }
            else
            {
                archive.Size = (int)value.Element(value.Name.Namespace + "size")!;
                archive.ChecksumType = (string)value.Element(value.Name.Namespace + "checksum")!.Attribute("type")!;
                archive.Checksum = (string)value.Element(value.Name.Namespace + "checksum")!;
                archive.Url = new Uri((string)value.Element(value.Name.Namespace + "url")!, UriKind.RelativeOrAbsolute);
            }

            // Make the URL absolute if required.
            if (!archive.Url.IsAbsoluteUri)
            {
                archive.Url = new Uri(baseUri, archive.Url);
            }

            return archive;
        }

        /// <summary>
        /// Downloads the package and extracts it to a directory.
        /// </summary>
        /// <param name="repositoryDirectory">
        /// The directory to which to extract the package. A new sub folder will be created
        /// in this directory.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation, and returns the
        /// directory to which the package has been extracted.
        /// </returns>
        public async Task<DirectoryInfo> DownloadAndExtract(DirectoryInfo repositoryDirectory)
        {
            if (repositoryDirectory == null)
            {
                throw new ArgumentNullException(nameof(repositoryDirectory));
            }

            var targetDirectory = repositoryDirectory.CreateSubdirectory(this.HostOs!);

            // Temporary file to which to download the file.
            string? tempFile = null;

            try
            {
                tempFile = Path.GetTempFileName();
                HttpClient client = new HttpClient();
                using (Stream source = await client.GetStreamAsync(this.Url))
                using (Stream target = File.Open(tempFile, FileMode.Open, FileAccess.ReadWrite))
                using (var sha1 = SHA1.Create())
                {
                    await source.CopyToAsync(target);

                    target.Position = 0;

                    // Validate the SHA1 hash, if present
                    if (string.Equals(this.ChecksumType, "sha1", StringComparison.OrdinalIgnoreCase))
                    {
                        var actualHash = sha1.ComputeHash(target);
                        var expectedhash = StringToByteArray(this.Checksum!);

                        if (actualHash.Length != expectedhash.Length)
                        {
                            throw new Exception();
                        }

                        for (int i = 0; i < actualHash.Length; i++)
                        {
                            if (expectedhash[i] != actualHash[i])
                            {
                                throw new Exception();
                            }
                        }
                    }
                }

                // Extract to directory
                ZipFile.ExtractToDirectory(tempFile, targetDirectory.FullName);

                return targetDirectory;
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Url}";
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
