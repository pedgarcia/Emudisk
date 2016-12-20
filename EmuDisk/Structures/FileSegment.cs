//-----------------------------------------------------------------------
// <copyright file="FileSegment.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// OS9 File Segment
    /// </summary>
    internal class FileSegment
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSegment"/> class.
        /// </summary>
        public FileSegment() 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSegment"/> class.
        /// </summary>
        /// <param name="lsn">Starting Logical Sector Number</param>
        /// <param name="sectors">Number of Sectors</param>
        public FileSegment(int lsn, ushort sectors)
        {
            this.LSN = lsn;
            this.Sectors = sectors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSegment"/> class.
        /// </summary>
        /// <param name="segmentData">Array of bytes representing a File Segment</param>
        public FileSegment(byte[] segmentData)
        {
            if (segmentData.Length != 5)
            {
                return;
            }

            this.LSN = Util.Int24(segmentData.Subset(0, 3));
            this.Sectors = Util.UInt16(segmentData.Subset(3, 2));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Starting Logical Sector Number
        /// </summary>
        public int LSN { get; set; }

        /// <summary>
        /// Gets or sets the Number of Sectors
        /// </summary>
        public ushort Sectors { get; set; }

        #endregion

        #region Implicit Operators

        /// <summary>
        /// Convert a File Segment to an array of bytes
        /// </summary>
        /// <param name="fileSegment">File Segment</param>
        /// <returns>Array of bytes representing a File Segment</returns>
        public static implicit operator byte[](FileSegment fileSegment)
        {
            return fileSegment.GetBytes();
        }

        /// <summary>
        /// Convert an Array of bytes to a File Segment
        /// </summary>
        /// <param name="buffer">Array of bytes representing a File Segment</param>
        /// <returns>File Segment</returns>
        public static implicit operator FileSegment(byte[] buffer)
        {
            return new FileSegment(buffer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get an array of bytes representing a File Segment
        /// </summary>
        /// <returns>Array of bytes representing a File Segment</returns>
        public byte[] GetBytes()
        {
            byte[] data = new byte[5];
            Array.Copy(Util.Int24(this.LSN), 0, data, 0, 3);
            Array.Copy(Util.UInt16(this.Sectors), 0, data, 3, 2);
            return data;
        }

        #endregion
    }
}
