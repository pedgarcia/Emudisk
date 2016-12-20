//-----------------------------------------------------------------------
// <copyright file="IDiskImage.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// Common interface for All Disk Images.
    /// </summary>
    internal interface IDiskImage
    {
        /// <summary>
        /// Gets Disk Image Type
        /// </summary>
        DiskImageType ImageType { get; }

        /// <summary>
        /// Gets disk image's Filename
        /// </summary>
        string Filename { get; }

        /// <summary>
        /// Gets or sets size of disk image's header
        /// </summary>
        int HeaderLength { get; set; }

        /// <summary>
        /// Gets the number of cylinders
        /// </summary>
        int PhysicalTracks { get; }

        /// <summary>
        /// Gets or sets the number of sides
        /// </summary>
        int PhysicalHeads { get; set; }
        
        /// <summary>
        /// Gets the number of sectors per track
        /// </summary>
        int PhysicalSectors { get; }
        
        /// <summary>
        /// Gets the size of a sector in bytes
        /// </summary>
        int PhysicalSectorSize { get; }

        /// <summary>
        /// Gets or sets the sector interleave factor
        /// </summary>
        int Interleave { get; set; }

        /// <summary>
        /// Gets or sets the currently selected partitions offset within the disk image
        /// </summary>
        int PartitionOffset { get; set; }

        /// <summary>
        /// Gets or sets the root OS9 partition's offset within the disk image. Beyond this offset
        /// are multiple RGBDOS drives
        /// </summary>
        int ImagePartitionOffset { get; set; }

        /// <summary>
        /// Gets or sets the number of partitions on the disk image
        /// </summary>
        int Partitions { get; set; }

        /// <summary>
        /// Gets a value indicating whether the disk image is partitioned (PartitionedVHDImage)
        /// </summary>
        bool IsPartitioned { get; }

        /// <summary>
        /// Gets a value indicating whether the disk image is a VHD or PartitionedVHD
        /// </summary>
        bool IsHD { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the data on the disk image is valid for the given disk image class
        /// </summary>
        bool IsValidImage { get; set; }

        /// <summary>
        /// Read a sector from the disk image
        /// </summary>
        /// <param name="track">Physical Cylinder</param>
        /// <param name="head">Physical Side</param>
        /// <param name="sector">Physical Sector</param>
        /// <returns>Array of bytes containing sector data</returns>
        byte[] ReadSector(int track, int head, int sector);

        /// <summary>
        /// Write a sector to the disk image
        /// </summary>
        /// <param name="track">Physical Cylinder</param>
        /// <param name="head">Physical Side</param>
        /// <param name="sector">Physical Sector</param>
        /// <param name="sectorData">Array of bytes containing sector data</param>
        void WriteSector(int track, int head, int sector, byte[] sectorData);

        /// <summary>
        /// Format a sector on the disk image
        /// </summary>
        /// <param name="track">Physical Cylinder</param>
        /// <param name="head">Physical Side</param>
        /// <param name="sector">Physical Sector</param>
        /// <param name="fillByte">Byte pattern to use to fill the sector</param>
        void FormatSector(int track, int head, int sector, byte fillByte);

        /// <summary>
        /// Create a new disk image and low-level format it
        /// </summary>
        /// <param name="filename">Disk image's Filename</param>
        /// <param name="tracks">Number of Cylinders</param>
        /// <param name="heads">Number of Sides</param>
        /// <param name="sectors">Number of Sectors per Track</param>
        /// <param name="sectorSize">Size of Sectors in bytes</param>
        void CreateDisk(string filename, int tracks, int heads, int sectors, int sectorSize);

        /// <summary>
        /// Refresh the disk's configuration
        /// </summary>
        void GetDiskInfo();

        /// <summary>
        /// Close the disk image
        /// </summary>
        void Close();
    }
}
