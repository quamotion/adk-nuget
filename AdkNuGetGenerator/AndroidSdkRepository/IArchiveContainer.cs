// <copyright file="IArchiveContainer.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

namespace AndroidSdkRepository
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// A common interface for all components that contain archives.
    /// </summary>
    public interface IArchiveContainer
    {
        /// <summary>
        /// Gets a list of archives that exist for the current component.
        /// </summary>
        Collection<Archive> Archives
        { get; }

        /// <summary>
        /// Gets the revision of the component.
        /// </summary>
        Revision? Revision
        {
            get;
        }

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        string? Name
        {
            get;
        }
    }
}
