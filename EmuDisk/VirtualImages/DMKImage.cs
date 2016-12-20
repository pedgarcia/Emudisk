//-----------------------------------------------------------------------
// <copyright file="DMKImage.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// VHD Disk Image support
    /// </summary>
    internal class DMKImage : DiskImageBase, IDiskImage
    {
        #region Private Fields

        /// <summary>
        /// Raw Track Size
        /// </summary>
        private int trackSize;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DMKImage"/> class
        /// </summary>
        /// <param name="filename">Disk image filename</param>
        public DMKImage(string filename)
            : base(filename)
        {
            this.GetDiskInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DMKImage"/> class
        /// </summary>
        /// <param name="filename">Filename of disk image</param>
        /// <param name="tracks">Physical Cylinders</param>
        /// <param name="heads">Physical Sides</param>
        /// <param name="sectors">Physical Sectors</param>
        /// <param name="sectorSize">Physical Sector Size</param>
        /// <param name="interleave">Sector Interleave Factor</param>
        public DMKImage(string filename, int tracks, int heads, int sectors, int sectorSize, int interleave)
        {
            this.Interleave = interleave;
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
                return DiskImageType.DMKImage;
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
            int trackoffset = this.HeaderLength + (track * this.PhysicalHeads * this.trackSize) + (head * this.trackSize);

            bool doneSearching = false;
            int sectorsFound = 0;

            byte[] buffer = null;

            while (!doneSearching)
            {
                for (int i = 0; i < 128; i += 2)
                {
                    int idamOffset = ((this.ReadByte(trackoffset + i + 1) << 8) + this.ReadByte(trackoffset + i)) & 0x7fff;
                    int curtrack = this.ReadByte(trackoffset + idamOffset + 1);
                    int curhead = this.ReadByte(trackoffset + idamOffset + 2);
                    int cursector = this.ReadByte(trackoffset + idamOffset + 3);
                    if (curtrack == track && curhead == head && cursector == sector)
                    {
                        buffer = this.ReadBytes(trackoffset + idamOffset + 45, this.PhysicalSectorSize);
                        doneSearching = true;
                        break;
                    }

                    sectorsFound++;
                    if (sectorsFound > this.PhysicalSectors)
                    {
                        break;
                    }
                }
            }

            return buffer;
        }

        /// <summary>
        /// Write a sector to the disk image
        /// </summary>
        /// <param name="track">Physical Cylinder</param>
        /// <param name="head">Physical Side</param>
        /// <param name="sector">Physical Sector</param>
        /// <param name="buffer">Array of bytes containing sector data</param>
        public override void WriteSector(int track, int head, int sector, byte[] buffer)
        {
            int trackoffset = this.HeaderLength + (track * this.PhysicalHeads * this.trackSize) + (head * this.trackSize);

            bool doneSearching = false;
            int sectorsFound = 0;
            byte[] crc = Crc16.ComputeChecksumBytes(0xE295, buffer);

            while (!doneSearching)
            {
                for (int i = 0; i < 128; i += 2)
                {
                    int idamOffset = ((this.ReadByte(trackoffset + i + 1) << 8) + this.ReadByte(trackoffset + i)) & 0x7fff;
                    int curtrack = this.ReadByte(trackoffset + idamOffset + 1);
                    int curhead = this.ReadByte(trackoffset + idamOffset + 2);
                    int cursector = this.ReadByte(trackoffset + idamOffset + 3);
                    if (curtrack == track && curhead == head && cursector == sector)
                    {
                        this.WriteBytes(trackoffset + idamOffset + 45, buffer);
                        this.WriteByte(trackoffset + idamOffset + 45 + this.PhysicalSectorSize, crc[1]);
                        this.WriteByte(trackoffset + idamOffset + 45 + this.PhysicalSectorSize + 1, crc[0]);
                        doneSearching = true;
                        break;
                    }

                    sectorsFound++;
                    if (sectorsFound > this.PhysicalSectors)
                    {
                        break;
                    }
                }
            }
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
            this.PhysicalTracks = tracks;
            this.PhysicalHeads = heads;
            this.PhysicalSectors = sectors;
            this.PhysicalSectorSize = sectors;

            this.HeaderLength = 16;
            byte[] header = new byte[] { 0, (byte)this.PhysicalTracks, (byte)(this.trackSize & 0xff), (byte)(this.trackSize >> 8), (byte)(((this.PhysicalHeads > 1) ? 0 : 1) << 4), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            this.Create(filename, this.HeaderLength + (this.PhysicalTracks * this.PhysicalHeads * this.trackSize));
            this.WriteBytes(0, header);
            this.LowLevelFormat();
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
            this.trackSize = 0x1900;

            this.HeaderLength = (int)this.Length % 256;
            if (this.HeaderLength > 16)
            {
                goto NotValid;
            }

            this.Seek(0);
            this.PhysicalTracks = this.ReadByte(1);
            this.PhysicalHeads = this.ReadByte(4) >> 4 == 0 ? 2 : 1;
            this.trackSize = (this.ReadByte(3) << 8) | this.ReadByte(2);

            if (this.Length != this.HeaderLength + (this.PhysicalTracks * this.PhysicalHeads * this.trackSize))
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
            return string.Format("CoCo DMK {0}", MainForm.ResourceManager.GetString("DiskImage_DiskFormat", MainForm.CultureInfo));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create DMK basic disk structure on disk image
        /// </summary>
        private void LowLevelFormat()
        {
            // Prepare blank sector data
            byte[] sectorData = new byte[this.PhysicalSectorSize];
            byte[] sectorCRC = Crc16.ComputeChecksumBytes(0xe295, sectorData);

            for (int track = 0; track < this.PhysicalTracks; track++)
            {
                for (int head = 0; head < this.PhysicalHeads; head++)
                {
                    byte[] trackData = new byte[this.trackSize];

                    // First, create the IDAM table
                    byte[] idamtable = new byte[128];
                    for (int i = 0, sectorOffset = 0x80ab; i < this.PhysicalSectors; i++, sectorOffset += 338)
                    {
                        idamtable[i * 2] = (byte)(sectorOffset & 0xff);
                        idamtable[(i * 2) + 1] = (byte)(sectorOffset >> 8);
                    }

                    Array.Copy(idamtable, 0, trackData, 0, idamtable.Length);

                    // Now lets make the track
                    Array.Copy(new byte[32].Initialize(0x4e), 0, trackData, idamtable.Length, 32);

                    // Lets create each sector
                    int interleavedSector = 1;
                    for (int sector = 1; sector < this.PhysicalSectors; sector++)
                    {
                        // Sector Preamble part
                        byte[] sectorPreamble = new byte[56].Initialize(0x4e);
                        Array.Copy(new byte[0x08], 0, sectorPreamble, 0, 0x08);
                        Array.Copy(new byte[0x03].Initialize(0xa1), 0, sectorPreamble, 0x08, 3);

                        byte[] sectorControl = new byte[] { 0xfe, (byte)track, (byte)head, (byte)sector, 0x01 };
                        Array.Copy(sectorControl, 0, sectorPreamble, 0x0b, 5);
                        byte[] sectorControlCRC = Crc16.ComputeChecksumBytes(0xcdb4, sectorControl);
                        sectorPreamble[0x10] = sectorControlCRC[1];
                        sectorPreamble[0x11] = sectorControlCRC[0];
                        Array.Copy(new byte[12], 0, sectorPreamble, 0x28, 12);
                        Array.Copy(new byte[3].Initialize(0xa1), 0, sectorPreamble, 0x34, 3);
                        sectorPreamble[0x37] = 0xfb;

                        // Sector Post-amble part
                        byte[] sectorPostamble = new byte[26].Initialize(0x4e);
                        sectorPostamble[0] = sectorCRC[1];
                        sectorPostamble[1] = sectorCRC[0];

                        // Write the parts to the track data
                        int trackOffset = idamtable.Length + ((interleavedSector - 1) * 338) + 32;
                        Array.Copy(sectorPreamble, 0, trackData, trackOffset, sectorPreamble.Length);
                        Array.Copy(sectorData, 0, trackData, trackOffset + 56, sectorData.Length);
                        Array.Copy(sectorPostamble, 0, trackData, trackOffset + 312, sectorPostamble.Length);

                        // Calculate next interleaved sector position
                        interleavedSector += this.Interleave;
                        if (interleavedSector > this.PhysicalSectors)
                        {
                            interleavedSector -= this.PhysicalSectors;
                        }

                        if (interleavedSector < this.Interleave)
                        {
                            interleavedSector++;
                        }
                    }

                    // Write out the track
                    this.WriteBytes(this.HeaderLength + (track * this.PhysicalHeads * this.trackSize) + (head * this.trackSize), trackData);
                }
            }
        }

        #endregion
    }
}
