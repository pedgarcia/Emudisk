//-----------------------------------------------------------------------
// <copyright file="JVCImage.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// JVC Disk Image support
    /// </summary>
    internal class JVCImage : DiskImageBase, IDiskImage
    {
        #region PrivateProperties

        /// <summary>
        /// Contains First Sector ID
        /// </summary>
        private byte firstSectorID;

        /// <summary>
        /// Contains Sector Attribute Flag
        /// </summary>
        private byte sectorAttributeFlag;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JVCImage"/> class
        /// </summary>
        /// <param name="filename">Disk image filename</param>
        public JVCImage(string filename)
            : base(filename)
        {
            this.GetDiskInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JVCImage"/> class
        /// </summary>
        /// <param name="filename">Filename of disk image</param>
        /// <param name="tracks">Physical Cylinders</param>
        /// <param name="heads">Physical Sides</param>
        /// <param name="sectors">Physical Sectors</param>
        /// <param name="sectorSize">Physical Sector Size</param>
        public JVCImage(string filename, int tracks, int heads, int sectors, int sectorSize)
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
                return DiskImageType.JVCImage;
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

            this.HeaderLength = 0;
            byte[] header = new byte[] { (byte)this.PhysicalSectors, (byte)this.PhysicalHeads, (byte)(this.PhysicalSectorSize / 128) };

            if (this.PhysicalSectors != 18)
            {
                this.HeaderLength = 1;
            }

            if (this.PhysicalHeads != 1)
            {
                this.HeaderLength = 2;
            }

            if (this.PhysicalSectorSize / 128 != 2)
            {
                this.HeaderLength = 3;
            }

            this.Create(filename, this.HeaderLength + (heads * tracks * sectors * sectorSize));
            if (this.HeaderLength > 0)
            {
                this.WriteBytes(0, header.Subset(0, this.HeaderLength));
            }
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
            this.Seek(0);
            if (this.HeaderLength > 0)
            {
                for (int i = 0; i < this.HeaderLength; i++)
                {
                    switch (i)
                    {
                        case 0:
                            this.PhysicalSectors = this.ReadByte();
                            if (this.PhysicalSectors < 1)
                            {
                                goto NotValid;
                            }

                            break;
                        case 1:
                            this.PhysicalHeads = this.ReadByte();
                            if (this.PhysicalHeads < 1)
                            {
                                goto NotValid;
                            }

                            break;
                        case 2:
                            this.PhysicalSectorSize = 128 << this.ReadByte();
                            if (this.PhysicalSectorSize < 0)
                            {
                                goto NotValid;
                            }

                            break;
                        case 3:
                            this.firstSectorID = (byte)this.ReadByte();
                            break;
                        case 4:
                            this.sectorAttributeFlag = (byte)this.ReadByte();
                            break;
                        default:
                            goto NotValid;
                    }
                }
            }

            this.PhysicalTracks = ((int)this.Length - this.HeaderLength) / (this.PhysicalSectors * this.PhysicalSectorSize) / this.PhysicalHeads;
            if (this.PhysicalTracks < 1)
            {
                goto NotValid;
            }

            if ((this.Length - this.HeaderLength) % this.PhysicalSectorSize != 0)
            {
                goto NotValid;
            }

            // Fix for non-full size images - Minimum 35 tracks
            if (this.PhysicalTracks < 35)
            {
                this.PhysicalTracks = 35;
                this.SetLength(this.HeaderLength + (this.PhysicalTracks * this.PhysicalHeads * this.PhysicalSectors * this.PhysicalSectorSize));
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
            return string.Format("CoCo JVC {0}", MainForm.ResourceManager.GetString("DiskImage_DiskFormat", MainForm.CultureInfo));
        }

        #endregion
    }
}
