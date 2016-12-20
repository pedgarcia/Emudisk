//-----------------------------------------------------------------------
// <copyright file="PartitionedVHDImage.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// Partitioned VHD Disk Image support
    /// </summary>
    internal class PartitionedVHDImage : DiskImageBase, IDiskImage
    {
        #region Private Properties

        /// <summary>
        /// Contains currently selected partition
        /// </summary>
        private int currentPartition;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionedVHDImage"/> class
        /// </summary>
        /// <param name="filename">Disk image filename</param>
        public PartitionedVHDImage(string filename)
            : base(filename)
        {
            this.GetDiskInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionedVHDImage"/> class
        /// </summary>
        /// <param name="filename">Disk image filename</param>
        /// <param name="rootPartitionSectors">Size of the Root OS9 partition in sectors</param>
        /// <param name="rgbdospartitions">Number of RGBDOS partitions</param>
        public PartitionedVHDImage(string filename, int rootPartitionSectors, int rgbdospartitions)
        {
            this.Partitions = rgbdospartitions + ((rootPartitionSectors > 0) ? 1 : 0);
            this.ImagePartitionOffset = rootPartitionSectors * 256;
            this.Create(filename, (rootPartitionSectors * 256) + (rgbdospartitions * 0x27600));
            this.PartitionOffset = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the current partition
        /// </summary>
        public int CurrentPartition
        {
            get 
            {
                return this.currentPartition; 
            }

            set
            {
                if (value > this.Partitions)
                {
                    return;
                }

                this.currentPartition = value;
                if (value == 0)
                {
                    this.PartitionOffset = 0;
                }
                else
                {
                    this.PartitionOffset = this.ImagePartitionOffset + ((value - 1) * 0x27600);
                }
            }
        }

        #endregion

        #region IDiskImage Members

        #region Public properties

        /// <summary>
        /// Gets Disk Image Type
        /// </summary>
        public DiskImageType ImageType
        {
            get
            {
                return DiskImageType.PartitionedVHDImage;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected partitions offset within the disk image
        /// </summary>
        public override int PartitionOffset { get; set; }

        /// <summary>
        /// Gets or sets the root OS9 partition's offset within the disk image. Beyond this offset
        /// are multiple RGBDOS drives
        /// </summary>
        public override int ImagePartitionOffset { get; set; }

        /// <summary>
        /// Gets or sets the number of partitions on the disk image
        /// </summary>
        public override int Partitions { get; set; }

        /// <summary>
        /// Gets a value indicating whether the disk image is partitioned (PartitionedVHDImage)
        /// </summary>
        public override bool IsPartitioned 
        { 
            get 
            { 
                return true; 
            } 
        }

        /// <summary>
        /// Gets a value indicating whether the disk image is a VHD or PartitionedVHD
        /// </summary>
        public override bool IsHD 
        { 
            get 
            { 
                return true; 
            } 
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Read a sector from the disk image
        /// </summary>
        /// <param name="track">Physical Cylinder</param>
        /// <param name="head">Physical Side</param>
        /// <param name="sector">Physical Sector</param>
        /// <returns>Array of bytes containing sector data</returns>
        public override byte[] ReadSector(int track, int head, int sector)
        {
            int offset = 0;
            if (this.PartitionOffset > this.ImagePartitionOffset - 1)
            {
                offset = this.PartitionOffset + (track * 1 * 18 * 256) + (head * 18 * 256) + ((sector - 1) * 256);
            }
            else
            {
                offset = this.PartitionOffset + (track * this.PhysicalHeads * this.PhysicalSectors * this.PhysicalSectorSize) + (head * this.PhysicalSectors * this.PhysicalSectorSize) + ((sector - 1) * this.PhysicalSectorSize);
            }

            return this.ReadBytes(offset, this.PhysicalSectorSize);
        }

        /// <summary>
        /// Write a sector to the disk image
        /// </summary>
        /// <param name="track">Physical Cylinder</param>
        /// <param name="head">Physical Side</param>
        /// <param name="sector">Physical Sector</param>
        /// <param name="sectorData">Array of bytes containing sector data</param>
        public override void WriteSector(int track, int head, int sector, byte[] sectorData)
        {
            int offset = 0;
            if (this.PartitionOffset > this.ImagePartitionOffset - 1)
            {
                offset = this.PartitionOffset + (track * 1 * 18 * 256) + (head * 18 * 256) + ((sector - 1) * 256);
            }
            else
            {
                offset = this.PartitionOffset + (track * this.PhysicalHeads * this.PhysicalSectors * this.PhysicalSectorSize) + (head * this.PhysicalSectors * this.PhysicalSectorSize) + ((sector - 1) * this.PhysicalSectorSize);
            }

            this.WriteBytes(offset, sectorData);
        }

        /// <summary>
        /// Create a new disk image and low-level format it
        /// </summary>
        /// <param name="filename">Disk image's Filename</param>
        /// <param name="tracks">Number of Cylinders</param>
        /// <param name="heads">Number of Sides</param>
        /// <param name="sectors">Number of Sectors per Track</param>
        /// <param name="sectorSize">Size of Sectors in bytes</param>
        public void CreateDisk(string filename, int tracks, int heads, int sectors, int sectorSize)
        {
            if (this.BaseStream == null)
            {
                return;
            }

            if (this.PartitionOffset > this.ImagePartitionOffset - 1)
            {
                this.WriteBytes(this.PartitionOffset, new byte[0x27600]);
            }
            else
            {
                this.WriteBytes(this.PartitionOffset, new byte[tracks * heads * sectors * sectorSize]);
            }
        }

        /// <summary>
        /// Refresh the disk's configuration
        /// </summary>
        public void GetDiskInfo()
        {
            this.PhysicalTracks = 35;
            this.PhysicalHeads = 1;
            this.PhysicalSectors = 18;
            this.PhysicalSectorSize = 256;
            this.Partitions = 0;
            this.ImagePartitionOffset = 0;
            this.PartitionOffset = 0;

            LSN0 lsn0 = new LSN0(this.ReadBytes(0, 256));
            int totalSectors = lsn0.TotalSectors;
            this.PhysicalTracks = lsn0.PathDescriptor.Cylinders;
            this.PhysicalHeads = lsn0.PathDescriptor.Sides;
            this.PhysicalSectors = lsn0.PathDescriptor.SectorsPerTrack;
            if (this.PhysicalSectors == 0)
            {
                this.PhysicalSectors = lsn0.SectorsPerTrack;
            }

            if (this.PhysicalHeads == 0)
            {
                if (lsn0.DiskFormat == 0x82)
                {
                    this.PhysicalHeads = 1;
                }
                else
                {
                    this.PhysicalHeads = ((lsn0.DiskFormat & 0x01) > 0) ? 2 : 1;
                }
            }

            if (totalSectors == 0 || this.PhysicalSectors == 0 || this.PhysicalHeads == 0)
            {
                goto NoOS9;
            }

            if (this.PhysicalTracks == 0)
            {
                this.PhysicalTracks = totalSectors / this.PhysicalSectors / this.PhysicalHeads;
            }

            if (totalSectors != this.PhysicalTracks * this.PhysicalHeads * this.PhysicalSectors)
            {
                goto NoOS9;
            }

            this.Partitions = 1;
            this.ImagePartitionOffset = totalSectors * this.PhysicalSectorSize;

        NoOS9:
            if ((this.Length - this.ImagePartitionOffset) % 0x27600 != 0)
            {
                goto NotValid;
            }

            this.Partitions += (int)((this.Length - this.ImagePartitionOffset) / 0x27600);

            this.PartitionOffset = 0;

            this.IsValidImage = true;
            return;

        NotValid:
            this.IsValidImage = false;
            this.Close();
        }

        #endregion

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Gets the Disk image display string
        /// </summary>
        /// <returns>Disk image display string</returns>
        public override string ToString()
        {
            return string.Format("CoCo Partitioned VHD {0}", MainForm.ResourceManager.GetString("DiskImage_DiskFormat", MainForm.CultureInfo));
        }

        #endregion
    }
}
