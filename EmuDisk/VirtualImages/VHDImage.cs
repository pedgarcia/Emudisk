//-----------------------------------------------------------------------
// <copyright file="VHDImage.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// VHD Disk Image support
    /// </summary>
    internal class VHDImage : DiskImageBase, IDiskImage
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VHDImage"/> class
        /// </summary>
        /// <param name="filename">Disk image filename</param>
        public VHDImage(string filename)
            : base(filename)
        {
            this.GetDiskInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VHDImage"/> class
        /// </summary>
        /// <param name="filename">Filename of disk image</param>
        /// <param name="tracks">Physical Cylinders</param>
        /// <param name="heads">Physical Sides</param>
        /// <param name="sectors">Physical Sectors</param>
        /// <param name="sectorSize">Physical Sector Size</param>
        public VHDImage(string filename, int tracks, int heads, int sectors, int sectorSize)
        {
            this.CreateDisk(filename, heads, tracks, sectors, sectorSize);
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
                return DiskImageType.VHDImage;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the disk image is a VHD
        /// </summary>
        public override bool IsHD
        {
            get { return true; }
        }

        #endregion

        #region Public Methods

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
            this.PhysicalTracks = tracks;
            this.PhysicalHeads = heads;
            this.PhysicalSectors = sectors;
            this.PhysicalSectorSize = sectorSize;
            this.Create(filename, tracks * heads * sectors * sectorSize);
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
                goto NotValid;
            }

            if (this.PhysicalTracks == 0)
            {
                this.PhysicalTracks = totalSectors / this.PhysicalSectors / this.PhysicalHeads;
            }

            if (totalSectors != this.PhysicalTracks * this.PhysicalHeads * this.PhysicalSectors)
            {
                goto NotValid;
            }

            if (this.Length != totalSectors * this.PhysicalSectorSize)
            {
                if (this.Length < totalSectors * this.PhysicalSectorSize)
                {
                    this.WriteByte((totalSectors * this.PhysicalSectorSize) - 1, 0x00);
                }
                else
                {
                    goto NotValid;
                }
            }

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
            return string.Format("CoCo VHD {0}", MainForm.ResourceManager.GetString("DiskImage_DiskFormat", MainForm.CultureInfo));
        }

        #endregion
    }
}
