﻿//-----------------------------------------------------------------------
// <copyright file="LSN0.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;
    using System.Text;

    /// <summary>
    /// OS9 Logical Sector 0
    /// </summary>
    internal class LSN0
    {
        #region Private Fields
        
        /// <summary>
        /// Contains the packed Creation Date
        /// </summary>
        private byte[] createdDateBytes;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LSN0"/> class
        /// </summary>
        public LSN0()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSN0"/> class
        /// </summary>
        /// <param name="buffer">Array of bytes representing a Logical Sector 0</param>
        public LSN0(byte[] buffer)
        {
            this.TotalSectors = Util.Int24(buffer.Subset(0x00, 3));
            this.TrackSize = buffer[0x03];
            this.MapBytes = Util.UInt16(buffer.Subset(0x04, 2));
            this.ClusterSize = Util.UInt16(buffer.Subset(0x06, 2));
            this.RootDirectory = Util.Int24(buffer.Subset(0x08, 3));
            this.Owner = Util.UInt16(buffer.Subset(0x0b, 2));
            this.Attributes = buffer[0x0d];
            this.DiskID = Util.UInt16(buffer.Subset(0x0e, 2));
            this.DiskFormat = buffer[0x10];
            this.SectorsPerTrack = Util.UInt16(buffer.Subset(0x11, 2));
            this.Reserved = Util.UInt16(buffer.Subset(0x13, 2));
            this.BootStrap = Util.Int24(buffer.Subset(0x15, 3));
            this.BootStrapSize = Util.UInt16(buffer.Subset(0x18, 2));
            this.PackedCreationDate = buffer.Subset(0x1a, 5);
            this.VolumeNameBytes = buffer.Subset(0x1f, 32);
            this.PathDescriptor = new PathDescriptor(buffer.Subset(0x3f, 19));
            this.SectorSize = buffer[0x68];
        }

        #endregion

        #region Public Properties
        
        /// <summary>
        /// Gets or sets Number of sectors on disk
        /// DD.TOT - offset $00-$02
        /// </summary>
        public int TotalSectors { get; set; }

        /// <summary>
        /// Gets or sets Track size (in sectors)
        /// DD.TKS - offset $03
        /// </summary>
        public byte TrackSize { get; set; }

        /// <summary>
        /// Gets or sets Number of bytes in the allocation bit map
        /// DD.MAP - offset $04-$05
        /// </summary>
        public ushort MapBytes { get; set; }

        /// <summary>
        /// Gets or sets Number of sectors per cluster
        /// DD.BIT - offset $06-$07
        /// </summary>
        public ushort ClusterSize { get; set; }

        /// <summary>
        /// Gets or sets Starting sector of the root directory
        /// DD.DIR - offset $08-$0A
        /// </summary>
        public int RootDirectory { get; set; }

        /// <summary>
        /// Gets or sets Owner’s user number
        /// DD.OWN - offset $0B-$0C
        /// </summary>
        public ushort Owner { get; set; }

        /// <summary>
        /// Gets or sets Disk attributes
        /// DD.ATT - offset $0D
        /// </summary>
        public byte Attributes { get; set; }

        /// <summary>
        /// Gets or sets Disk identification (for internal use)
        /// DD.DSK - offset $0E-$0F
        /// </summary>
        public ushort DiskID { get; set; }

        /// <summary>
        /// Gets or sets Disk format, density, number of sides
        /// DD.FMT - offset $10
        /// </summary>
        public byte DiskFormat { get; set; }

        /// <summary>
        /// Gets or sets Number of sectors per track
        /// DD.SPT - offset $11-$12
        /// </summary>
        public ushort SectorsPerTrack { get; set; }

        /// <summary>
        /// Gets or sets Reserved for future use
        /// DD.RES - offset $13-$14
        /// </summary>
        public ushort Reserved { get; set; }

        /// <summary>
        /// Gets or sets Starting sector of the bootstrap file
        /// DD.BT  - offset $15-$17
        /// </summary>
        public int BootStrap { get; set; }

        /// <summary>
        /// Gets or sets Size of the bootstrap file (in bytes)
        /// DD.BSZ - offset $18-$19
        /// </summary>
        public ushort BootStrapSize { get; set; }

        /// <summary>
        /// Gets or sets Time of creation (Y:M:D:H:M)
        /// offset $1A-$1E
        /// </summary>
        public byte[] PackedCreationDate
        {
            get 
            { 
                return this.createdDateBytes;
            }

            set
            {
                if (value == null || value.Length != 5)
                {
                    return;
                }

                this.createdDateBytes = value;
            }
        }

        /// <summary>
        /// Gets or sets Volume name in which the last character has the most significant bit set
        /// DD.NAM - offset $1F-$3E
        /// </summary>
        public byte[] VolumeNameBytes { get; set; }

        /// <summary>
        /// Gets or sets Path Descriptor
        /// DD.OPT bytes $3F-$52
        /// </summary>
        public PathDescriptor PathDescriptor { get; set; }

        /// <summary>
        /// Gets or sets the Sector Size
        /// LSN0 byte $68
        /// </summary>
        public ushort SectorSize { get; set; }

        /// <summary>
        /// Gets or sets the Volume Name
        /// </summary>
        public string VolumeName
        {
            get 
            {
                return Util.GetHighBitString(this.VolumeNameBytes);
            }

            set
            {
                if (value.Length > 32)
                {
                    value = value.Substring(0, 32);
                }

                this.VolumeNameBytes = Encoding.ASCII.GetBytes(value);
                this.VolumeNameBytes[value.Length - 1] |= 0x80;
            }
        }

        /// <summary>
        /// Gets or sets the Creation Date
        /// </summary>
        public DateTime CreatedDate
        {
            get 
            {
                return Util.ModifiedDate(this.PackedCreationDate); 
            }

            set 
            {
                this.PackedCreationDate = Util.ModifiedDateBytes(value); 
            }
        }
        
        #endregion    
 
        #region Implicit Operators

        /// <summary>
        /// Implicit operator to convert a Logical Sector 0 to an array of bytes
        /// </summary>
        /// <param name="lsn0">Logical Sector 0</param>
        /// <returns>Array of bytes</returns>
        public static implicit operator byte[](LSN0 lsn0)
        {
            return lsn0.GetBytes();
        }

        /// <summary>
        /// Implicit operator to convert an array of bytes to a Logical Sector 0
        /// </summary>
        /// <param name="buffer">Array of bytes</param>
        /// <returns>Logical Sector 0</returns>
        public static implicit operator LSN0(byte[] buffer)
        {
            return new LSN0(buffer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get an Array of bytes representing a Logical Sector 0
        /// </summary>
        /// <returns>Array of bytes representing a Logical Sector 0</returns>
        public byte[] GetBytes()
        {
            byte[] buffer = new byte[256];
            Array.Copy(Util.Int24(this.TotalSectors), 0, buffer, 0x00, 3);
            buffer[0x03] = this.TrackSize;
            Array.Copy(Util.UInt16(this.MapBytes), 0, buffer, 0x04, 2);
            Array.Copy(Util.UInt16(this.ClusterSize), 0, buffer, 0x06, 2);
            Array.Copy(Util.Int24(this.RootDirectory), 0, buffer, 0x08, 3);
            Array.Copy(Util.UInt16(this.Owner), 0, buffer, 0x0b, 2);
            buffer[0x0d] = this.Attributes;
            Array.Copy(Util.UInt16(this.DiskID), 0, buffer, 0x0e, 2);
            buffer[0x10] = this.DiskFormat;
            Array.Copy(Util.UInt16(this.SectorsPerTrack), 0, buffer, 0x11, 2);
            Array.Copy(Util.UInt16(this.Reserved), 0, buffer, 0x13, 2);
            Array.Copy(Util.Int24(this.BootStrap), 0, buffer, 0x15, 3);
            Array.Copy(Util.UInt16(this.BootStrapSize), 0, buffer, 0x18, 2);
            Array.Copy(this.PackedCreationDate, 0, buffer, 0x1a, 5);
            Array.Copy(this.VolumeNameBytes, 0, buffer, 0x1f, this.VolumeNameBytes.Length);
            Array.Copy((byte[])this.PathDescriptor, 0, buffer, 0x3f, 19);
            Array.Copy(Util.UInt16(this.SectorSize), 0, buffer, 0x68, 2);

            return buffer;
        }

        #endregion
    }
}
