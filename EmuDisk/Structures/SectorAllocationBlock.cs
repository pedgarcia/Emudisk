//-----------------------------------------------------------------------
// <copyright file="SectorAllocationBlock.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// DragonDos Sector Allocation Block
    /// </summary>
    internal class SectorAllocationBlock
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SectorAllocationBlock"/> class
        /// </summary>
        public SectorAllocationBlock()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectorAllocationBlock"/> class
        /// </summary>
        /// <param name="lsn">Starting Logical Sector Number</param>
        /// <param name="sectors">Number of Sectors</param>
        public SectorAllocationBlock(ushort lsn, byte sectors)
        {
            this.LSN = lsn;
            this.Sectors = sectors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectorAllocationBlock"/> class
        /// </summary>
        /// <param name="buffer">Array of bytes representing a Sector Allocation Block</param>
        public SectorAllocationBlock(byte[] buffer)
        {
            if (buffer.Length != 3)
            {
                return;
            }

            this.LSN = Util.UInt16(buffer.Subset(0, 2));
            this.Sectors = buffer[2];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Starting Logical Sector Number
        /// </summary>
        public ushort LSN { get; set; }

        /// <summary>
        /// Gets or sets the Number of Sectors
        /// </summary>
        public byte Sectors { get; set; }

        #endregion

        #region Implicit Operators

        /// <summary>
        /// Implicit operator to convert a Sector Allocation Block to an array of bytes
        /// </summary>
        /// <param name="sectorAllocationBlock">Sector Allocation Block</param>
        /// <returns>Array of bytes</returns>
        public static implicit operator byte[](SectorAllocationBlock sectorAllocationBlock)
        {
            return sectorAllocationBlock.GetBytes();
        }

        /// <summary>
        /// Implicit operator to convert an array of bytes to a Sector Allocation Block
        /// </summary>
        /// <param name="buffer">Array of bytes</param>
        /// <returns>New Sector Allocation Block</returns>
        public static implicit operator SectorAllocationBlock(byte[] buffer)
        {
            return new SectorAllocationBlock(buffer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get an Array of bytes representing a Sector Allocation Block
        /// </summary>
        /// <returns>Array of bytes representing a Sector Allocation Block</returns>
        public byte[] GetBytes()
        {
            byte[] data = new byte[3];
            Array.Copy(Util.UInt16(this.LSN), 0, data, 0, 2);
            data[2] = this.Sectors;
            return data;
        }

        #endregion
    }
}
