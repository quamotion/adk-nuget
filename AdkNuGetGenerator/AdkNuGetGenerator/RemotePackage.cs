// <copyright file="RemotePackage.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

namespace AdkNuGetGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class RemotePackage : IArchiveContainer
    {
        /// <summary>
        /// Gets the component name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the component revision (version).
        /// </summary>
        public Revision Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the component is obsolete.
        /// </summary>
        public bool Obsolete
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a list of archives that exist for the current component.
        /// </summary>
        public Collection<Archive> Archives
        { get; } = new Collection<Archive>();

        /// <summary>
        /// Loads a <see cref="Component"/> from a component element.
        /// </summary>
        /// <param name="value">
        /// The element from which to load the <see cref="Component"/>.
        /// </param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> which is used to resolve relative package URLs.
        /// </param>
        /// <returns>
        /// A new <see cref="Component"/> object.
        /// </returns>
        public static RemotePackage FromXElement(XElement value, Uri baseUri)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Name != "remotePackage")
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            var package = new RemotePackage()
            {
                Name = value.Element("display-name").Value,
                Revision = Revision.FromXElement(value.Element("revision")),
                Obsolete = value.Elements("obsolete").Any(),
            };

            foreach (var archive in Archive.FromArchivesXElement(value.Element("archives"), baseUri))
            {
                package.Archives.Add(archive);
            }

            return package;
        }

        /// <inheritdoc/>
        public override string ToString() => this.Name;
    }
}
