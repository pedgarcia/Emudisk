//-----------------------------------------------------------------------
// <copyright file="VDKImage.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// VDK Disk Image support
    /// </summary>
    internal class VDKImage : DiskImageBase, IDiskImage
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VDKImage"/> class
        /// </summary>
        /// <param name="filename">Disk image filename</param>
        public VDKImage(string filename)
            : base(filename)
        {
            this.GetDiskInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VDKImage"/> class
        /// </summary>
        /// <param name="filename">Filename of disk image</param>
        /// <param name="tracks">Physical Cylinders</param>
        /// <param name="heads">Physical Sides</param>
        /// <param name="sectors">Physical Sectors</param>
        /// <param name="sectorSize">Physical Sector Size</param>
        public VDKImage(string filename, int tracks, int heads, int sectors, int sectorSize)
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
                return DiskImageType.VDKImage;
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
            this.PhysicalSectorSize = sectors;

            this.HeaderLength = 12;
            byte[] header = new byte[] { (byte)'d', (byte)'k', 12, 0, 0x10, 0x10, 0, 0x10, (byte)this.PhysicalTracks, (byte)this.PhysicalHeads, 0, 0 };

            this.Create(filename, this.HeaderLength + (heads * tracks * sectors * sectorSize));
            this.WriteBytes(0, header);
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

            this.HeaderLength = (int)this.Length % 256;
            if (this.HeaderLength > 12)
            {
                goto NotValid;
            }

            this.Seek(0);
            if (this.ReadByte() != 'd' || this.ReadByte() != 'k')
            {
                goto NotValid;
            }

            ushort recordedHeaderSize = (ushort)(this.ReadByte() | (this.ReadByte() << 8));
            if (this.HeaderLength != recordedHeaderSize)
            {
                goto NotValid;
            }

            if (this.HeaderLength > 5 && this.ReadByte(5) != 0x10)
            {
                goto NotValid;
            }

            if (this.HeaderLength > 11 && this.ReadByte(11) != 0)
            {
                goto NotValid;
            }

            if (this.HeaderLength > 8)
            {
                this.PhysicalTracks = this.ReadByte(8);
            }

            if (this.HeaderLength > 9)
            {
                this.PhysicalHeads = this.ReadByte(9);
            }

            if (this.Length != this.HeaderLength + (this.PhysicalTracks * this.PhysicalHeads * this.PhysicalSectors * this.PhysicalSectorSize))
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
            return string.Format("CoCo VDK {0}", MainForm.ResourceManager.GetString("DiskImage_DiskFormat", MainForm.CultureInfo));
        }

        #endregion
    }
}
