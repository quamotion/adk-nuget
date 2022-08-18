// <copyright file="Repository2.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

namespace AdkNuGetGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Represents an Android SDK repository, using the version 2 file format.
    /// </summary>
    public class Repository2
    {
        private static readonly XNamespace RepositoryNamespace = "http://schemas.android.com/sdk/android/repo/repository2/01";

        /// <summary>
        /// Gets a list of all build tools that are part of this repository.
        /// </summary>
        public List<RemotePackage> BuildTools
        { get; } = new List<RemotePackage>();

        /// <summary>
        /// Gets a list of all components that are part of this repository.
        /// </summary>
        public List<RemotePackage> PlatformTools
        { get; } = new List<RemotePackage>();

        private XNamespace Namespace
        {
            get;
            set;
        }

        /// <summary>
        /// Loads a repository from a specific URL.
        /// </summary>
        /// <param name="url">
        /// The URL at which the repository exists.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public static async Task<Repository2> Load(string url)
        {
            XDocument document;
            HttpClient client = new HttpClient();

            using (Stream stream = await client.GetStreamAsync(url))
            {
                document = XDocument.Load(stream);
            }

            var repository = Repository2.FromXElement(document.Root, new Uri(url));
            return repository;
        }

        /// <summary>
        /// Loads a <see cref="Repository"/> from a repository element.
        /// </summary>
        /// <param name="value">
        /// The element from which to load the <see cref="Repository"/>.
        /// </param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> which is used to resolve relative package URLs.
        /// </param>
        /// <returns>
        /// A new <see cref="Repository"/> object.
        /// </returns>
        public static Repository2 FromXElement(XElement value, Uri baseUri)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Name.Namespace != Repository2.RepositoryNamespace && value.Name.LocalName != "sdk-repository")
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            var repository = new Repository2()
            {
                Namespace = value.Name.Namespace
            };

            repository.LoadComponents(value, repository.BuildTools, "build-tools", baseUri);
            repository.LoadComponents(value, repository.PlatformTools, "platform-tools", baseUri);

            return repository;
        }

        private void LoadComponents(XElement value, List<RemotePackage> collection, string path, Uri baseUri)
        {
            XName name = XName.Get("remotePackage");

            foreach (var remotePackage in value.Elements(name))
            {
                var paths = ((string)remotePackage.Attribute("path")).Split(";", StringSplitOptions.RemoveEmptyEntries);

                if (paths.Contains(path))
                {
                    collection.Add(RemotePackage.FromXElement(remotePackage, baseUri));
                }
            }
        }
    }
}
