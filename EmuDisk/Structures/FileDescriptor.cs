//-----------------------------------------------------------------------
// <copyright file="FileDescriptor.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// OS9 File Descriptor
    /// </summary>
    internal class FileDescriptor
    {
        #region Private Fields

        /// <summary>
        /// Contains an array of bytes representing the entry's modified date
        /// </summary>
        private byte[] modifiedDateBytes;

        /// <summary>
        /// Contains an array of bytes representing the entry's creation date
        /// </summary>
        private byte[] createdDateBytes;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDescriptor"/> class.
        /// </summary>
        public FileDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDescriptor"/> class.
        /// </summary>
        /// <param name="fileAttribute">File's Attributes</param>
        /// <param name="owner">Owner Number</param>
        /// <param name="modifiedDateBytes">Array of bytes representing modified date</param>
        /// <param name="linkCount">Link Count</param>
        /// <param name="size">File Size</param>
        /// <param name="createdDateBytes">Array of bytes representing creation date</param>
        public FileDescriptor(byte fileAttribute, ushort owner, byte[] modifiedDateBytes, byte linkCount, uint size, byte[] createdDateBytes)
        {
            this.AttrByte = fileAttribute;
            this.Owner = owner;
            this.ModifiedDateBytes = modifiedDateBytes;
            this.LinkCount = linkCount;
            this.Size = size;
            this.CreatedDateBytes = createdDateBytes;
            this.SegmentList = new FileSegment[48];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDescriptor"/> class.
        /// </summary>
        /// <param name="fileAttribute">File's Attributes</param>
        /// <param name="owner">Owner Number</param>
        /// <param name="modifiedDateBytes">Array of bytes representing modified date</param>
        /// <param name="linkCount">Link Count</param>
        /// <param name="size">File Size</param>
        /// <param name="createdDateBytes">Array of bytes representing creation date</param>
        /// <param name="segmentList">File Segment List</param>
        public FileDescriptor(byte fileAttribute, ushort owner, byte[] modifiedDateBytes, byte linkCount, uint size, byte[] createdDateBytes, FileSegment[] segmentList) :
            this(fileAttribute, owner, modifiedDateBytes, linkCount, size, createdDateBytes)
        {
            this.SegmentList = segmentList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDescriptor"/> class
        /// from an array of bytes.
        /// </summary>
        /// <param name="fileDescriptorBytes">Array of bytes representing the File Descriptor</param>
        public FileDescriptor(byte[] fileDescriptorBytes)
        {
            this.AttrByte = fileDescriptorBytes[0];
            this.Owner = Util.UInt16(fileDescriptorBytes.Subset(1, 2));
            this.ModifiedDateBytes = fileDescriptorBytes.Subset(3, 5);
            this.LinkCount = fileDescriptorBytes[8];
            this.Size = Util.UInt32(fileDescriptorBytes.Subset(9, 4));
            this.CreatedDateBytes = fileDescriptorBytes.Subset(0x0d, 3);

            for (int i = 0; i < 48; i++)
            {
                this.SegmentList[i] = new FileSegment(fileDescriptorBytes.Subset(0x10 + (i * 5), 5));
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets entry's attribute byte
        /// OS9 FileDescriptor Offset 0x00
        /// </summary>
        public byte AttrByte { get; set; }

        /// <summary>
        /// Gets or sets Owner Number
        /// OS9 FileDescriptor Offset 0x01-0x02
        /// </summary>
        public ushort Owner { get; set; }

        /// <summary>
        /// Gets or sets the array of bytes representing the Modified Date
        /// OS9 FileDescriptor Offset 0x03-0x07
        /// </summary>
        public byte[] ModifiedDateBytes
        {
            get 
            { 
                return this.modifiedDateBytes; 
            }

            set
            {
                if (value == null || value.Length != 5)
                {
                    return;
                }

                this.modifiedDateBytes = value;
            }
        }

        /// <summary>
        /// Gets or sets the entry's Link Count
        /// OS9 FileDescriptor Offset 0x08
        /// </summary>
        public byte LinkCount { get; set; }

        /// <summary>
        /// Gets or sets Entry's size
        /// OS9 FileDescriptor Offset 0x09-0x0C
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// Gets or sets the array of bytes representing the Creation Date
        /// OS9 FileDescriptor Offset 0x0D-0x0F
        /// </summary>
        public byte[] CreatedDateBytes
        {
            get 
            { 
                return this.createdDateBytes; 
            }

            set
            {
                if (value == null || value.Length != 3)
                {
                    return;
                }

                this.createdDateBytes = value;
            }
        }

        /// <summary>
        /// Gets or sets the entry's File Segment List
        /// OS9 FileDescriptor Offset 0x10-0xFF
        /// </summary>
        public FileSegment[] SegmentList { get; set; }

        /// <summary>
        /// Gets or sets the entry's modified date
        /// </summary>
        public DateTime ModifiedDate
        {
            get 
            { 
                return Util.ModifiedDate(this.ModifiedDateBytes); 
            }

            set 
            { 
                this.ModifiedDateBytes = Util.ModifiedDateBytes(value); 
            }
        }

        /// <summary>
        /// Gets or sets the entry's creation date
        /// </summary>
        public DateTime CreatedDate
        {
            get 
            { 
                return Util.CreatedDate(this.CreatedDateBytes); 
            }

            set 
            { 
                this.CreatedDateBytes = Util.CreatedDateBytes(value); 
            }
        }

        #endregion

        #region Implicit Operators

        /// <summary>
        /// Implicit operator to convert a File Descriptor to an Array of bytes
        /// </summary>
        /// <param name="fileDescriptor">File Descriptor</param>
        /// <returns>Array of bytes representing a File Descriptor</returns>
        public static implicit operator byte[](FileDescriptor fileDescriptor)
        {
            return fileDescriptor.GetBytes();
        }

        /// <summary>
        /// Implicit operator to convert a Array of bytes to an File Descriptor
        /// </summary>
        /// <param name="buffer">Array of bytes representing a File Descriptor</param>
        /// <returns>File Descriptor</returns>
        public static implicit operator FileDescriptor(byte[] buffer)
        {
            return new FileDescriptor(buffer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets an array of bytes representing the file descriptor
        /// </summary>
        /// <returns>Array of bytes representing the file descriptor</returns>
        public byte[] GetBytes()
        {
            byte[] data = new byte[256];

            data[0] = this.AttrByte;
            Array.Copy(Util.UInt16(this.Owner), 0, data, 1, 2);
            Array.Copy(this.ModifiedDateBytes, 0, data, 3, 5);
            data[8] = this.LinkCount;
            Array.Copy(Util.UInt32(this.Size), 0, data, 9, 4);
            Array.Copy(this.CreatedDateBytes, 0, data, 0x0d, 3);
            for (int i = 0; i < 48; i++)
            {
                Array.Copy(this.SegmentList[i].GetBytes(), 0, data, 0x10 + (i * 5), 5);
            }

            return data;
        }

        #endregion
    }
}
