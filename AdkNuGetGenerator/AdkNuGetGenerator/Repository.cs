// <copyright file="Repository.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

namespace AdkNuGetGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Represents an Android SDK repository.
    /// </summary>
    public class Repository
    {
        private static readonly XNamespace RepositoryNamespace = "http://schemas.android.com/sdk/android/repository/11";
        private static readonly XNamespace AddonNamespace = "http://schemas.android.com/sdk/android/addon/7";

        /// <summary>
        /// Gets a list of all build tools that are part of this repository.
        /// </summary>
        public List<Component> BuildTools
        { get; } = new List<Component>();

        /// <summary>
        /// Gets a list of all components that are part of this repository.
        /// </summary>
        public List<Component> PlatformTools
        { get; } = new List<Component>();

        /// <summary>
        /// Gets a list of all extras that are part of this repository.
        /// </summary>
        public List<Extra> Extras
        { get; } = new List<Extra>();

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
        public static async Task<Repository> Load(string url)
        {
            XDocument document;
            HttpClient client = new HttpClient();

            using (Stream stream = await client.GetStreamAsync(url))
            {
                document = XDocument.Load(stream);
            }

            var repository = Repository.FromXElement(document.Root, new Uri(url));
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
        public static Repository FromXElement(XElement value, Uri baseUri)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if ((value.Name.Namespace != Repository.RepositoryNamespace && value.Name.LocalName != "sdk-repository")
                && (value.Name.Namespace != Repository.AddonNamespace && value.Name.LocalName != "sdk-addon"))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            var repository = new Repository()
            {
                Namespace = value.Name.Namespace
            };

            repository.LoadComponents(value, repository.BuildTools, repository.Namespace + "build-tool", baseUri);
            repository.LoadComponents(value, repository.PlatformTools, repository.Namespace + "platform-tool", baseUri);
            repository.LoadExtras(value, repository.Extras, repository.Namespace, baseUri);

            return repository;
        }

        /// <summary>
        /// Merges another repository with this repository.
        /// </summary>
        /// <param name="other">
        /// The repository to merge into this repository.
        /// </param>
        public void Merge(Repository other)
        {
            this.BuildTools.AddRange(other.BuildTools);
            this.Extras.AddRange(other.Extras);
            this.PlatformTools.AddRange(other.PlatformTools);
        }

        private void LoadComponents(XElement value, List<Component> collection, XName name, Uri baseUri)
        {
            foreach (var component in value.Elements(name))
            {
                collection.Add(Component.FromXElement(component, name, baseUri));
            }
        }

        private void LoadExtras(XElement value, List<Extra> collection, XNamespace ns, Uri baseUri)
        {
            foreach (var extra in value.Elements(ns + "extra"))
            {
                collection.Add(Extra.FromXElement(extra, baseUri));
            }
        }
    }
}
