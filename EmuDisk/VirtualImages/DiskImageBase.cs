//-----------------------------------------------------------------------
// <copyright file="DiskImageBase.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// Disk Image Base
    /// </summary>
    internal class DiskImageBase : Stream
    {
        #region Private Fields

        /// <summary>
        /// File Stream used to access virtual disk image
        /// </summary>
        private FileStream baseStream;

        /// <summary>
        /// Filename of the currently opened virtual disk image
        /// </summary>
        private string filename;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskImageBase"/> class
        /// </summary>
        public DiskImageBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskImageBase"/> class
        /// </summary>
        /// <param name="filename">Disk image filename</param>
        public DiskImageBase(string filename)
        {
            this.filename = filename;

            try
            {
                this.baseStream = File.Open(this.filename, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            }
            catch (IOException)
            {
                this.filename = string.Empty;
                MessageBox.Show(string.Format(MainForm.ResourceManager.GetString("DiskImageBase_FileOpenError", MainForm.CultureInfo), this.filename), MainForm.ResourceManager.GetString("DiskImageBase_FileOpenErrorCaption", MainForm.CultureInfo), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskImageBase"/> class
        /// </summary>
        /// <param name="filename">Disk image filename</param>
        /// <param name="buffer">Array of bytes representing a new virtual disk image</param>
        public DiskImageBase(string filename, byte[] buffer)
        {
            this.Create(filename, buffer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskImageBase"/> class
        /// </summary>
        /// <param name="filename">Disk image filename</param>
        /// <param name="length">Size of new disk image in byes</param>
        public DiskImageBase(string filename, int length) : this(filename, new byte[length])
        {
        }

        #endregion

        #region IDiskImage Public Properties

        /// <summary>
        /// Gets the Filename of the currently opened virtual disk image
        /// </summary>
        public string Filename
        {
            get { return this.filename; }
        }

        /// <summary>
        /// Gets or sets size of disk image's header
        /// </summary>
        public int HeaderLength { get; set; }

        /// <summary>
        /// Gets or sets the number of cylinders
        /// </summary>
        public int PhysicalTracks { get; set; }

        /// <summary>
        /// Gets or sets the number of sides
        /// </summary>
        public int PhysicalHeads { get; set; }

        /// <summary>
        /// Gets or sets the number of sectors per track
        /// </summary>
        public int PhysicalSectors { get; set; }

        /// <summary>
        /// Gets or sets the size of a sector in bytes
        /// </summary>
        public int PhysicalSectorSize { get; set; }

        /// <summary>
        /// Gets or sets the sector interleave factor
        /// </summary>
        public int Interleave { get; set; }

        /// <summary>
        /// Gets or sets the currently selected partitions offset within the disk image
        /// </summary>
        public virtual int PartitionOffset
        {
            get 
            { 
                return 0; 
            }

            set 
            {
            }
        }

        /// <summary>
        /// Gets or sets the root OS9 partition's offset within the disk image. Beyond this offset
        /// are multiple RGBDOS drives
        /// </summary>
        public virtual int ImagePartitionOffset 
        { 
            get 
            { 
                return 0; 
            }
 
            set 
            {
            } 
        }

        /// <summary>
        /// Gets or sets the number of partitions on the disk image
        /// </summary>
        public virtual int Partitions 
        { 
            get 
            { 
                return 0; 
            }
 
            set 
            {
            } 
        }

        /// <summary>
        /// Gets a value indicating whether the disk image is partitioned (PartitionedVHDImage)
        /// </summary>
        public virtual bool IsPartitioned 
        { 
            get 
            { 
                return false; 
            } 
        }

        /// <summary>
        /// Gets a value indicating whether the disk image is a VHD or PartitionedVHD
        /// </summary>
        public virtual bool IsHD 
        { 
            get 
            { 
                return false; 
            } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data on the disk image is valid for the given disk image class
        /// </summary>
        public bool IsValidImage { get; set; }
        
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the stream can be read from
        /// </summary>
        public override bool CanRead
        {
            get 
            {
                if (this.baseStream != null && this.baseStream.CanRead)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the stream can seek to a new position
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                if (this.baseStream != null && this.baseStream.CanSeek)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the stream can be written to
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                if (this.baseStream != null && this.baseStream.CanWrite)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the length of the file stream
        /// </summary>
        public override long Length
        {
            get
            {
                if (this.baseStream != null)
                {
                    return this.baseStream.Length;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the current position within the file stream
        /// </summary>
        public override long Position
        {
            get
            {
                if (this.baseStream != null)
                {
                    return this.baseStream.Position;
                }

                return 0;
            }

            set
            {
                if (this.baseStream != null)
                {
                    this.baseStream.Position = value;
                }
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the File Stream used to access virtual disk image
        /// </summary>
        internal FileStream BaseStream
        {
            get
            {
                return this.baseStream;
            }
        }

        #endregion

        #region IDiskImage Public Methods

        /// <summary>
        /// Read a sector from the disk image
        /// </summary>
        /// <param name="track">Physical Cylinder</param>
        /// <param name="head">Physical Side</param>
        /// <param name="sector">Physical Sector</param>
        /// <returns>Array of bytes containing sector data</returns>
        public virtual byte[] ReadSector(int track, int head, int sector)
        {
            int offset = this.HeaderLength + (track * this.PhysicalHeads * this.PhysicalSectors * this.PhysicalSectorSize) + (head * this.PhysicalSectors * this.PhysicalSectorSize) + ((sector - 1) * this.PhysicalSectorSize);
            return this.ReadBytes(offset, this.PhysicalSectorSize);
        }

        /// <summary>
        /// Write a sector to the disk image
        /// </summary>
        /// <param name="track">Physical Cylinder</param>
        /// <param name="head">Physical Side</param>
        /// <param name="sector">Physical Sector</param>
        /// <param name="buffer">Array of bytes containing sector data</param>
        public virtual void WriteSector(int track, int head, int sector, byte[] buffer)
        {
            int offset = this.HeaderLength + (track * this.PhysicalHeads * this.PhysicalSectors * this.PhysicalSectorSize) + (head * this.PhysicalSectors * this.PhysicalSectorSize) + ((sector - 1) * this.PhysicalSectorSize);
            if (buffer.Length < this.PhysicalSectorSize)
            {
                byte[] newBuffer = new byte[this.PhysicalSectorSize];
                Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
                buffer = newBuffer;
            }

            this.WriteBytes(offset, buffer);
        }

        /// <summary>
        /// Format a sector on the disk image
        /// </summary>
        /// <param name="track">Physical Cylinder</param>
        /// <param name="head">Physical Side</param>
        /// <param name="sector">Physical Sector</param>
        /// <param name="fillByte">Byte pattern to use to fill the sector</param>
        public virtual void FormatSector(int track, int head, int sector, byte fillByte)
        {
            byte[] buffer = new byte[this.PhysicalSectorSize].Initialize(fillByte);
            this.WriteSector(track, head, sector, buffer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Read a byte from the disk image
        /// </summary>
        /// <param name="offset">Offset from beginning</param>
        /// <returns>A single byte</returns>
        public byte ReadByte(int offset)
        {
            if (this.CanRead && this.CanSeek)
            {
                this.Seek(offset);
                return (byte)this.baseStream.ReadByte();
            }

            return 0;
        }

        /// <summary>
        /// Read a series of bytes
        /// </summary>
        /// <param name="offset">Offset from beginning</param>
        /// <param name="length">Number of bytes to read</param>
        /// <returns>Array of bytes read</returns>
        public byte[] ReadBytes(int offset, int length)
        {
            byte[] buffer = new byte[length];
            if (this.CanRead && this.CanSeek)
            {
                this.Seek(offset);
                this.Read(buffer, 0, length);
                return buffer;
            }

            return null;
        }

        /// <summary>
        /// Write a byte to the disk image
        /// </summary>
        /// <param name="offset">Offset from beginning</param>
        /// <param name="value">Byte value to write</param>
        public void WriteByte(int offset, byte value)
        {
            if (this.CanWrite && this.CanSeek)
            {
                this.Seek(offset);
                this.baseStream.WriteByte(value);
                this.Flush();
            }
        }

        /// <summary>
        /// Write a series of bytes to the disk image
        /// </summary>
        /// <param name="offset">Offset from beginning</param>
        /// <param name="buffer">Array of bytes to write</param>
        public void WriteBytes(int offset, byte[] buffer)
        {
            if (this.CanWrite && this.CanSeek)
            {
                this.Seek(offset);
                this.Write(buffer, 0, buffer.Length);
                this.Flush();
            }
        }

        /// <summary>
        /// Close the file stream
        /// </summary>
        public new void Close()
        {
            if (this.baseStream != null)
            {
                this.baseStream.Close();
                this.baseStream.Dispose();
                this.baseStream = null;
            }

            base.Close();
        }

        /// <summary>
        /// Create a new virtual disk image
        /// </summary>
        /// <param name="filename">Disk image's Filename</param>
        /// <param name="length">Length of new disk image</param>
        public void Create(string filename, int length)
        {
            this.Create(filename, new byte[length]);
        }

        /// <summary>
        /// Create a new virtual disk image
        /// </summary>
        /// <param name="filename">Disk image's Filename</param>
        /// <param name="buffer">Array of bytes to write to the new disk image</param>
        public void Create(string filename, byte[] buffer)
        {
            if (this.baseStream != null)
            {
                this.Close();
            }

            this.filename = filename;

            try
            {
                this.baseStream = File.Open(this.filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                this.WriteBytes(0, buffer);
            }
            catch (IOException)
            {
                if (this.baseStream != null)
                {
                    this.baseStream.Close();
                    this.baseStream.Dispose();
                    this.baseStream = null;
                }

                this.filename = string.Empty;
                MessageBox.Show(string.Format(MainForm.ResourceManager.GetString("DiskImageBase_FileOpenError", MainForm.CultureInfo), this.filename), MainForm.ResourceManager.GetString("DiskImageBase_FileOpenErrorCaption", MainForm.CultureInfo), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Flush buffered data to file
        /// </summary>
        public override void Flush()
        {
            if (this.baseStream != null)
            {
                this.baseStream.Flush();
            }
        }

        /// <summary>
        /// Read from file stream
        /// </summary>
        /// <param name="buffer">Array of bytes to read to</param>
        /// <param name="offset">Offset within the array of bytes to begin reading to</param>
        /// <param name="count">Number of bytes to read</param>
        /// <returns>Number indicating how many bytes were read. -1 indicates EOF</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.baseStream != null && this.CanRead)
            {
                return this.baseStream.Read(buffer, offset, count);
            }

            return -1;
        }

        /// <summary>
        /// Seek to a specific position within the file stream
        /// </summary>
        /// <param name="offset">Position to seek to</param>
        /// <param name="origin">Origin to seek from, Beginning, Current, or End</param>
        /// <returns>New position within file stream, or -1 to indicate EOF</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (this.baseStream != null && this.CanSeek)
            {
                return this.baseStream.Seek(offset, origin);
            }

            return -1;
        }

        /// <summary>
        /// Seek to a specific position within the file stream from Beginning
        /// </summary>
        /// <param name="offset">Position to seek to</param>
        /// <returns>New position within file stream, or -1 to indicate EOF</returns>
        public long Seek(long offset)
        {
            return this.Seek(offset, SeekOrigin.Begin);
        }

        /// <summary>
        /// Set new file stream length, not implemented
        /// </summary>
        /// <param name="value">New length to set file stream to</param>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write a series of bytes to the file stream
        /// </summary>
        /// <param name="buffer">Array of bytes to write</param>
        /// <param name="offset">Offset within the array to being write from</param>
        /// <param name="count">Number of bytes to write</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.baseStream != null && this.CanWrite)
            {
                this.baseStream.Write(buffer, offset, count);
            }
        }

        #endregion
    }
}
