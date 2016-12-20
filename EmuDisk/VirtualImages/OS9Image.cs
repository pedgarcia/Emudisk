//-----------------------------------------------------------------------
// <copyright file="OS9Image.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// OS9 Disk Image support
    /// </summary>
    internal class OS9Image : DiskImageBase, IDiskImage
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OS9Image"/> class
        /// </summary>
        /// <param name="filename">Disk image filename</param>
        public OS9Image(string filename)
            : base(filename)
        {
            this.GetDiskInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OS9Image"/> class
        /// </summary>
        /// <param name="filename">Filename of disk image</param>
        /// <param name="tracks">Physical Cylinders</param>
        /// <param name="heads">Physical Sides</param>
        /// <param name="sectors">Physical Sectors</param>
        /// <param name="sectorSize">Physical Sector Size</param>
        public OS9Image(string filename, int tracks, int heads, int sectors, int sectorSize)
        {
            this.CreateDisk(filename, tracks, heads, sectors, sectorSize);
        }

        #endregion

        #region IDiskImage Members

        #region Public Properties

        /// <summary>
        /// Gets Disk Image Type
        /// </summary>
        public DiskImageType ImageType
        {
            get
            {
                return DiskImageType.OS9Image;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new virtual disk image
        /// </summary>
        /// <param name="filename">Filename of disk image</param>
        /// <param name="tracks">Physical Cylinders</param>
        /// <param name="heads">Physical Sides</param>
        /// <param name="sectors">Physical Sectors</param>
        /// <param name="sectorSize">Physical Sector Size</param>
        public void CreateDisk(string filename, int tracks, int heads, int sectors, int sectorSize)
        {
            this.PhysicalTracks = tracks;
            this.PhysicalHeads = heads;
            this.PhysicalSectors = sectors;
            this.PhysicalSectorSize = sectorSize;
            this.Create(filename, tracks * heads * sectors * sectorSize);
        }

        /// <summary>
        /// Refreshes the disk configuration
        /// </summary>
        public void GetDiskInfo()
        {
            this.PhysicalTracks = 35;
            this.PhysicalHeads = 1;
            this.PhysicalSectors = 18;
            this.PhysicalSectorSize = 256;

            LSN0 lsn0 = new LSN0(ReadBytes(0, 256));
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
                this.PhysicalHeads = ((lsn0.DiskFormat & 0x01) > 0) ? 2 : 1;
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
            return string.Format("CoCo OS9 {0}", MainForm.ResourceManager.GetString("DiskImage_DiskFormat", MainForm.CultureInfo));
        }

        #endregion
    }
}
