// <copyright file="Extra.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

namespace AdkNuGetGenerator
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Represents an extra (third party) component in the Android SDK.
    /// </summary>
    public class Extra : IArchiveContainer
    {
        /// <summary>
        /// Gets or sets the unique ID of the vendor that provides the extra.
        /// </summary>
        public string VendorId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an URL that describes the extra component.
        /// </summary>
        public string DescUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the SDK path to the extra component.
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return this.Path; }
        }

        /// <summary>
        /// Gets or sets a description of the extra component.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display name of the extra component.
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display name of the vendor that provides the extra component.
        /// </summary>
        public string DisplayVendor
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the revision of the extra component.
        /// </summary>
        public Revision Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the extra component is obsolete.
        /// </summary>
        public bool Obsolete
        {
            get;
            set;
        }

        /// <summary>
        /// Gets all archives that are available for this extra component.
        /// </summary>
        public Collection<Archive> Archives
        { get; } = new Collection<Archive>();

        /// <summary>
        /// Loads a <see cref="Extra"/> from a <c>extra</c> element.
        /// </summary>
        /// <param name="value">
        /// The element from which to load the <see cref="Extra"/>.
        /// </param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> which is used to resolve relative package URLs.
        /// </param>
        /// <returns>
        /// A new <see cref="Extra"/> object.
        /// </returns>
        public static Extra FromXElement(XElement value, Uri baseUri)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Name.LocalName != "extra")
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            var extra = new Extra()
            {
                VendorId = (string)value.Element(value.Name.Namespace + "vendor-id"),
                DescUrl = (string)value.Element(value.Name.Namespace + "desc-url"),
                Path = (string)value.Element(value.Name.Namespace + "path"),
                Description = (string)value.Element(value.Name.Namespace + "description"),
                DisplayName = (string)value.Element(value.Name.Namespace + "name-display"),
                DisplayVendor = (string)value.Element(value.Name.Namespace + "vendor-display"),
                Revision = Revision.FromXElement(value.Element(value.Name.Namespace + "revision")),
                Obsolete = value.Elements(value.Name.Namespace + "obsolete").Any()
            };

            foreach (var archive in Archive.FromArchivesXElement(value.Element(value.Name.Namespace + "archives"), baseUri))
            {
                extra.Archives.Add(archive);
            }

            return extra;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Path} {this.Revision}";
        }
    }
}
