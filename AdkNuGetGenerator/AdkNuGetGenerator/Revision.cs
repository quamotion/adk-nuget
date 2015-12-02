// <copyright file="Revision.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

namespace AdkNuGetGenerator
{
    using System;
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
                Micro = value.Element(value.Name.Namespace + "micro") != null ? (int)value.Element(value.Name.Namespace + "micro") : -1
            };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Major}.{this.Minor}.{this.Micro}";
        }
    }
}
