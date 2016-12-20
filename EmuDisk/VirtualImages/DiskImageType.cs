//-----------------------------------------------------------------------
// <copyright file="DiskImageType.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// Disk Image Type
    /// </summary>
    public enum DiskImageType
    {
        /// <summary>
        /// DMK Image support
        /// </summary>
        DMKImage,

        /// <summary>
        /// JVC Image support
        /// </summary>
        JVCImage,

        /// <summary>
        /// OS9 Image support
        /// </summary>
        OS9Image,

        /// <summary>
        /// VDK Image support
        /// </summary>
        VDKImage,

        /// <summary>
        /// VHD Image support
        /// </summary>
        VHDImage,

        /// <summary>
        /// Partitioned VHD Image support
        /// </summary>
        PartitionedVHDImage,

        /// <summary>
        /// Physical Floppy support
        /// </summary>
        PhysicalDisk
    }
}
