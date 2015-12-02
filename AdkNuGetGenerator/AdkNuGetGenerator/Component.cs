// <copyright file="Component.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

namespace AdkNuGetGenerator
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Represents an individual component of the Android SDK.
    /// </summary>
    public class Component
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Component"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the component.
        /// </param>
        public Component(string name)
        {
            this.Name = name;
        }

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
        /// <param name="name">
        /// The name of the component element.
        /// </param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> which is used to resolve relative package URLs.
        /// </param>
        /// <returns>
        /// A new <see cref="Component"/> object.
        /// </returns>
        public static Component FromXElement(XElement value, XName name, Uri baseUri)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Name != name)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            var buildTool = new Component(name.LocalName)
            {
                Revision = Revision.FromXElement(value.Element(name.Namespace + "revision")),
                Obsolete = value.Elements("obsolete").Any(),
            };

            foreach (var archive in Archive.FromArchivesXElement(value.Element(name.Namespace + "archives"), baseUri))
            {
                buildTool.Archives.Add(archive);
            }

            return buildTool;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Name} {this.Revision}";
        }
    }
}
