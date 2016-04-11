// <copyright file="Revision.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

namespace AdkNuGetGenerator
{
    using NuGet;
    using System;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Represents a version number.
    /// </summary>
    public class Revision
    {
        /// <summary>
        /// Gets or sets the major version number.
        /// </summary>
        public int Major
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minor version number.
        /// </summary>
        public int Minor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the micro version number.
        /// </summary>
        public int Micro
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the component is in preview (that is, this
        /// is not a publicly released version of the component).
        /// </summary>
        public bool Preview
        {
            get;
            set;
        }

        /// <summary>
        /// Loads a <see cref="Revision"/> from a <c>revision</c> element.
        /// </summary>
        /// <param name="value">
        /// The element from which to load the <see cref="Revision"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="Revision"/> object.
        /// </returns>
        public static Revision FromXElement(XElement value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Name.LocalName != "revision")
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            return new Revision()
            {
                Major = (int)value.Element(value.Name.Namespace + "major"),
                Minor = value.Element(value.Name.Namespace + "minor") != null ? (int)value.Element(value.Name.Namespace + "minor") : -1,
                Micro = value.Element(value.Name.Namespace + "micro") != null ? (int)value.Element(value.Name.Namespace + "micro") : -1,
                Preview = value.Elements(value.Name.Namespace + "preview").Any(),
            };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.Micro > 0)
            {
                return $"{this.Major}.{this.Minor}.{this.Micro}";
            }
            else if (this.Minor > 0)
            {
                return $"{this.Major}.{this.Minor}";
            }
            else
            {
                return $"{this.Major}";
            }
        }

        public Version ToVersion()
        {
            return new Version(this.Major, this.Minor > 0 ? this.Minor : 0, this.Micro > 0 ? this.Micro : 0);
        }

        public SemanticVersion ToSematicVersion()
        {
            return new SemanticVersion(this.Major, this.Minor > 0 ? this.Minor : 0, this.Micro > 0 ? this.Micro : 0, null);
        }
    }
}
