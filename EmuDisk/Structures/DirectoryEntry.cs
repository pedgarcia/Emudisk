//-----------------------------------------------------------------------
// <copyright file="DirectoryEntry.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Directory Entry Class
    /// </summary>
    [DebuggerDisplay("{Filename}")]
    internal class DirectoryEntry
    {
        #region Private Fields

        /// <summary>
        /// True if entry has been marked as deleted
        /// </summary>
        private bool deleted;

        /// <summary>
        /// True if entry has been marked as protected
        /// </summary>
        private bool fileProtected;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryEntry"/> class.
        /// This is the default constructor.
        /// </summary>
        public DirectoryEntry() 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryEntry"/> class.
        /// Basic entry
        /// </summary>
        /// <param name="filename">Filename of new directory entry</param>
        /// <param name="size">Size of file</param>
        /// <param name="isDirectory">Is a directory</param>
        public DirectoryEntry(string filename, uint size, bool isDirectory)
        {
            this.Filename = filename;
            this.Size = size;
            this.IsDirectory = isDirectory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryEntry"/> class.
        /// RSDOS Specific constructor
        /// </summary>
        /// <param name="filename">Filename of new directory entry</param>
        /// <param name="size">Size of file</param>
        /// <param name="isDirectory">Is a directory</param>
        /// <param name="fileType">RSDOS File Type</param>
        /// <param name="asciiFlag">RSDOS ASCII Flag</param>
        public DirectoryEntry(string filename, uint size, bool isDirectory, byte fileType, byte asciiFlag) :
            this(filename, size, isDirectory, fileType, asciiFlag, 0, 0, 0) 
        { 
            this.FormatType = DiskFormatType.RSDOSFormat; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryEntry"/> class.
        /// RSDOS Specific constructor
        /// </summary>
        /// <param name="filename">Filename of new directory entry</param>
        /// <param name="size">Size of file</param>
        /// <param name="isDirectory">Is a directory</param>
        /// <param name="fileType">RSDOS File Type</param>
        /// <param name="asciiFlag">RSDOS ASCII Flag</param>
        /// <param name="firstGranule">First granule of file</param>
        /// <param name="lastSectorBytesMSB">MSB of bytes used in last sector</param>
        /// <param name="lastSectorBytesLSB">LSB of bytes used in last sector</param>
        public DirectoryEntry(string filename, uint size, bool isDirectory, byte fileType, byte asciiFlag, byte firstGranule, byte lastSectorBytesMSB, byte lastSectorBytesLSB) :
            this(filename, size, isDirectory)
        {
            this.FileType = fileType;
            this.AsciiFlag = asciiFlag;
            this.FirstGranule = firstGranule;
            this.LastSectorBytesMSB = lastSectorBytesMSB;
            this.LastSectorBytesLSB = lastSectorBytesLSB;
            this.FormatType = DiskFormatType.RSDOSFormat;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryEntry"/> class.
        /// OS9 Specific constructor
        /// </summary>
        /// <param name="filename">Filename of new directory entry</param>
        /// <param name="size">Size of file</param>
        /// <param name="isDirectory">Is a directory</param>
        /// <param name="lsn">Logical Sector Number of entry's File Descriptor</param>
        /// <param name="fileAttribute">File's Attributes</param>
        /// <param name="owner">File Owner Number</param>
        /// <param name="modifiedDateBytes">Array of bytes representing entry's Modified Date</param>
        /// <param name="linkCount">Link Count</param>
        /// <param name="createdDateBytes">Array of bytes representing entry's Creation Date></param>
        public DirectoryEntry(string filename, uint size, bool isDirectory, int lsn, byte fileAttribute, ushort owner, byte[] modifiedDateBytes, byte linkCount, byte[] createdDateBytes) :
            this(filename, size, isDirectory)
        {
            this.LSN = lsn;
            this.AttrByte = fileAttribute;
            this.Owner = owner;
            this.ModifiedDateBytes = modifiedDateBytes;
            this.LinkCount = linkCount;
            this.CreatedDateBytes = createdDateBytes;
            this.SegmentList = new FileSegment[48];
            this.FormatType = DiskFormatType.OS9Format;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryEntry"/> class.
        /// OS9 Specific constructor
        /// </summary>
        /// <param name="filename">Filename of new directory entry</param>
        /// <param name="lsn">Logical Sector Number of entry's File Descriptor</param>
        public DirectoryEntry(string filename, int lsn)
        {
            this.FormatType = DiskFormatType.OS9Format;
            this.Filename = filename;
            this.LSN = lsn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryEntry"/> class
        /// from an Array of bytes.
        /// </summary>
        /// <param name="directoryEntryBytes">Array of bytes containing directory entry information</param>
        /// <param name="diskFormatType">Type of Disk Format</param>
        public DirectoryEntry(byte[] directoryEntryBytes, DiskFormatType diskFormatType)
        {
            this.FormatType = diskFormatType;

            string filename, ext;

            switch (diskFormatType)
            {
                case DiskFormatType.OS9Format:
                    if (directoryEntryBytes[0] == 0x00)
                    {
                        this.Deleted = true;
                        this.Filename = "?" + Util.GetString(directoryEntryBytes.Subset(1, 29));
                    }
                    else
                    {
                        this.Deleted = false;
                        this.Filename = Util.GetString(directoryEntryBytes.Subset(0, 29));
                    }

                    this.LSN = Util.Int24(directoryEntryBytes.Subset(29, 3));
                    this.SegmentList = new FileSegment[48];
                    break;
                case DiskFormatType.DragonDosFormat:
                    this.AttrByte = directoryEntryBytes[0];
                    this.Deleted = (directoryEntryBytes[0] & 0x80) == 0x80;
                    this.Protected = (directoryEntryBytes[0] & 0x02) == 0x02;

                    filename = Util.GetString(directoryEntryBytes.Subset(1, 8));
                    ext = Util.GetString(directoryEntryBytes.Subset(9, 3));
                    this.Filename = string.Format("{0}.{1}", filename, ext);
                    this.SectorAllocationBlocks = new SectorAllocationBlock[11];

                    for (int i = 0; i < 4; i++)
                    {
                        this.SectorAllocationBlocks[i] = new SectorAllocationBlock(directoryEntryBytes.Subset(12 + (i * 3), 3));
                    }

                    if ((directoryEntryBytes[0] & 0x20) == 0x20)
                    {
                        this.NextDirectoryEntry = directoryEntryBytes[0x18];
                    }
                    else
                    {
                        this.BytesUsedInLastSector = directoryEntryBytes[0x18];
                    }

                    int size = 0;
                    for (int i = 0; i < 11; i++)
                    {
                        if (this.SectorAllocationBlocks[i].Sectors != 0)
                        {
                            size += this.SectorAllocationBlocks[i].Sectors * 256;
                        }
                    }

                    this.Size = (uint)size;

                    if (this.BytesUsedInLastSector != 0)
                    {
                        size -= 0x100 - this.BytesUsedInLastSector;
                    }

                break;
                case DiskFormatType.RSDOSFormat:
                    if (directoryEntryBytes[0] == 0x00)
                    {
                        this.Deleted = true;
                        filename = "?" + Util.GetString(directoryEntryBytes.Subset(1, 8)).Trim();
                    }
                    else
                    {
                        Deleted = false;
                        filename = Util.GetString(directoryEntryBytes.Subset(0, 8)).Trim();
                    }

                    ext = Util.GetString(directoryEntryBytes.Subset(8, 3)).Trim();
                    this.Filename = string.Format("{0}.{1}", filename, ext);
                    this.FileType = directoryEntryBytes[0x0b];
                    this.AsciiFlag = directoryEntryBytes[0x0c];
                    this.FirstGranule = directoryEntryBytes[0x0d];
                    this.LastSectorBytesMSB = directoryEntryBytes[0x0e];
                    this.LastSectorBytesLSB = directoryEntryBytes[0x0f];
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryEntry"/> class
        /// from an Array of bytes.
        /// OS9 Specific.
        /// </summary>
        /// <param name="directoryEntryBytes">Array of bytes containing directory entry information</param>
        /// <param name="descriptor">File Descriptor</param>
        public DirectoryEntry(byte[] directoryEntryBytes, FileDescriptor descriptor) :
            this(directoryEntryBytes, DiskFormatType.OS9Format)
        {
            this.Descriptor = descriptor;
        }

        #endregion
        
        #region Public Properties

        #region Common Properties

        /// <summary>
        /// Gets or sets Entry's Filename
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets Entry's size
        /// </summary>
        public uint Size 
        {
            get
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                return this.Descriptor.Size;
            }

            set
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                this.Descriptor.Size = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether entry is a directory
        /// </summary>
        public bool IsDirectory 
        {
            get 
            {
                if (this.FormatType != DiskFormatType.OS9Format)
                {
                    return false;
                }

                if ((this.AttrByte & 0x80) == 0x80)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            set 
            {
                if (this.FormatType == DiskFormatType.OS9Format)
                {
                    if (value)
                    {
                        this.AttrByte |= 0x80;
                    }
                    else
                    {
                        this.AttrByte &= 0x7f;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets or sets Entry's Disk Format type
        /// </summary>
        public DiskFormatType FormatType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entry is marked deleted
        /// </summary>
        public bool Deleted 
        {
            get 
            { 
                return this.deleted; 
            }

            set
            {
                this.deleted = value;
                if (this.FormatType == DiskFormatType.DragonDosFormat)
                {
                    if (value)
                    {
                        this.AttrByte |= 0x80;
                    }
                    else
                    {
                        this.AttrByte &= 0x7f;
                    }
                }
            }
        }

        #endregion

        #region RSDOS Properties

        /// <summary>
        /// Gets or sets RSDOS File Type
        /// RSDOS Directory Entry Offset 0x0B
        /// </summary>
        public byte FileType { get; set; }
        
        /// <summary>
        /// Gets or sets RSDOS ASCII Flag
        /// RSDOS Directory Entry Offset 0x0C
        /// </summary>
        public byte AsciiFlag { get; set; }
        
        /// <summary>
        /// Gets or sets RSDOS First Granule
        /// RSDOS Directory Entry Offset 0x0D
        /// </summary>
        public byte FirstGranule { get; set; }
        
        /// <summary>
        /// Gets or sets MSB of bytes used in last sector
        /// RSDOS Directory Entry Offset 0x0E
        /// </summary>
        public byte LastSectorBytesMSB { get; set; }
        
        /// <summary>
        /// Gets or sets LSB of bytes used in last sector
        /// RSDOS Directory Entry Offset 0x0F
        /// </summary>
        public byte LastSectorBytesLSB { get; set; }
        
        /// <summary>
        /// Gets or sets Number of bytes used in last sector
        /// </summary>
        public ushort LastSectorBytes 
        {
            get 
            {
                return Util.UInt16(new byte[] { this.LastSectorBytesMSB, this.LastSectorBytesLSB }); 
            }

            set
            {
                this.LastSectorBytesMSB = (byte)(value >> 8);
                this.LastSectorBytesLSB = (byte)(value & 0xff);
            }
        }

        #endregion

        #region OS9 Properties

        /// <summary>
        /// Gets or sets Logical Sector Number of entry's File Descriptor
        /// </summary>
        public int LSN { get; set; }
        
        /// <summary>
        /// Gets or sets entry's attribute byte
        /// OS9 FileDescriptor Offset 0x00
        /// </summary>
        public byte AttrByte
        { 
            get 
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                return this.Descriptor.AttrByte;
            }

            set
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                this.Descriptor.AttrByte = value;
                switch (this.FormatType)
                {
                    case DiskFormatType.OS9Format:
                        this.IsDirectory = (value & 0x80) == 0x80;
                        break;
                    case DiskFormatType.DragonDosFormat:
                        this.Deleted = (value & 0x80) == 0x80;
                        this.Protected = (value & 0x02) == 0x02;
                        break;
                }
            }
        }                  
        
        /// <summary>
        /// Gets or sets Owner Number
        /// OS9 FileDescriptor Offset 0x01-0x02
        /// </summary>
        public ushort Owner 
        {
            get
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                return this.Descriptor.Owner;
            }

            set
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                this.Descriptor.Owner = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the array of bytes representing the Modified Date
        /// OS9 FileDescriptor Offset 0x03-0x07
        /// </summary>
        public byte[] ModifiedDateBytes 
        { 
            get 
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                return this.Descriptor.ModifiedDateBytes; 
            }

            set 
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                this.Descriptor.ModifiedDateBytes = value;
            } 
        }
        
        /// <summary>
        /// Gets or sets the entry's Link Count
        /// OS9 FileDescriptor Offset 0x08
        /// </summary>
        public byte LinkCount 
        {
            get
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                return this.Descriptor.LinkCount;
            }

            set
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                this.Descriptor.LinkCount = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the array of bytes representing the Creation Date
        /// OS9 FileDescriptor Offset 0x0D-0x0F
        /// </summary>
        public byte[] CreatedDateBytes 
        {
            get 
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                return this.Descriptor.CreatedDateBytes; 
            }

            set 
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                this.Descriptor.CreatedDateBytes = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the entry's File Segment List
        /// OS9 FileDescriptor Offset 0x10-0xFF
        /// </summary>
        public FileSegment[] SegmentList 
        {
            get
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                return this.Descriptor.SegmentList;
            }

            set
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                this.Descriptor.SegmentList = value;
            }
        }

        /// <summary>
        /// Gets or sets the entry's modified date
        /// </summary>
        public DateTime ModifiedDate
        {
            get 
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                return this.Descriptor.ModifiedDate;
            }

            set 
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                this.Descriptor.ModifiedDate = value; 
            }
        }

        /// <summary>
        /// Gets or sets the entry's creation date
        /// </summary>
        public DateTime CreatedDate
        {
            get 
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                return this.Descriptor.CreatedDate;
            }

            set 
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                this.Descriptor.CreatedDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the entry's File Descriptor
        /// </summary>
        public FileDescriptor Descriptor
        {
            get
            {
                if (this.Descriptor == null)
                {
                    this.Descriptor = new FileDescriptor();
                }

                return this.Descriptor;
            }

            set
            {
                this.Descriptor = value;
                this.IsDirectory = (value.AttrByte & 0x80) == 0x80;
            }
        }

        #endregion

        #region DragonDos Properties

        /// <summary>
        /// Gets or sets the entry's Sector Allocation Blocks
        /// </summary>
        public SectorAllocationBlock[] SectorAllocationBlocks { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether entry is marked protected
        /// </summary>
        public bool Protected
        {
            get 
            { 
                return this.fileProtected; 
            }

            set
            {
                this.fileProtected = value;
                if (this.FormatType == DiskFormatType.DragonDosFormat)
                {
                    if (value)
                    {
                        this.AttrByte |= 0x02;
                    }
                    else
                    {
                        this.AttrByte &= 0xfd;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the number of bytes used in the last sector
        /// 0 = 256
        /// </summary>
        public byte BytesUsedInLastSector { get; set; }

        /// <summary>
        /// Gets or sets the Directory Index of next directory entry containing more Sector Allocation Blocks
        /// </summary>
        public byte NextDirectoryEntry { get; set; }

        /// <summary>
        /// Gets a value indicating whether this entry requires another directory entry containing Sector Allocation Blocks
        /// </summary>
        public bool NeedsExtension
        {
            get
            {
                if (this.SectorAllocationBlocks != null && this.SectorAllocationBlocks.Length > 4 && this.SectorAllocationBlocks[4].LSN != 0)
                {
                    return true;
                }

                return false;
            }
        }

        #endregion

        #endregion

        #region Implicit Operators

        /// <summary>
        /// Implicit operator to convert a Directory Entry to an Array of bytes
        /// </summary>
        /// <param name="entry">Directory Entry</param>
        /// <returns>Array of bytes representing a directory entry</returns>
        public static implicit operator byte[](DirectoryEntry entry)
        {
            return entry.GetBytes();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get an array of bytes representing an OS9 directory entry
        /// </summary>
        /// <returns>Array of bytes representing an OS9 directory entry</returns>
        public byte[] GetOS9Bytes()
        {
            byte[] data = new byte[32];
            
            int length = this.Filename.Length;
            if (length > 29)
            {
                length = 29;
            }

            byte[] filenamebytes = Encoding.ASCII.GetBytes(this.Filename);
            if (this.Deleted)
            {
                filenamebytes[0] = 0;
            }

            Array.Copy(filenamebytes, 0, data, 0, length);

            data[29] = (byte)(this.LSN >> 16);
            data[30] = (byte)((this.LSN >> 8) & 0xff);
            data[31] = (byte)(this.LSN & 0xff);

            return data;
        }

        /// <summary>
        /// Gets an array of bytes representing an RSDOS directory entry
        /// </summary>
        /// <returns>Array of bytes representing an RSDOS directory entry</returns>
        public byte[] GetRSDOSBytes()
        {
            string[] filenameparts = this.Filename.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            string filename = filenameparts[0];
            string ext = string.Empty;
            if (filenameparts.Length > 1)
            {
                ext = filenameparts[1];
            }

            if (filename.Length > 8)
            {
                filename = filename.Substring(0, 8);
            }

            filename = filename.PadRight(8, ' ');
            if (ext.Length > 3)
            {
                ext = ext.Substring(0, 3);
            }

            ext = ext.PadRight(3, ' ');

            byte[] filenamebytes = Encoding.ASCII.GetBytes(filename);
            if (this.Deleted)
            {
                filenamebytes[0] = 0;
            }

            byte[] extbytes = Encoding.ASCII.GetBytes(ext);
            byte[] data = new byte[32];
            Array.Copy(filenamebytes, 0, data, 0, 8);
            Array.Copy(extbytes, 8, data, 0, 3);
            data[0x0b] = this.FileType;
            data[0x0c] = this.AsciiFlag;
            data[0x0d] = this.FirstGranule;
            data[0x0e] = this.LastSectorBytesMSB;
            data[0x0f] = this.LastSectorBytesLSB;

            return data;
        }

        /// <summary>
        /// Gets an array of bytes representing a DragonDos directory entry
        /// </summary>
        /// <returns>Array of bytes representing an DragonDos directory entry</returns>
        public byte[] GetDragonDosBytes()
        {
            byte[] buffer = new byte[0x19];

            buffer[0] = this.AttrByte;

            string[] filenameparts = this.Filename.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            string filename = filenameparts[0];
            string ext = string.Empty;
            if (filenameparts.Length > 1)
            {
                ext = filenameparts[1];
            }

            if (filename.Length > 8)
            {
                filename = filename.Substring(0, 8);
            }

            if (ext.Length > 3)
            {
                ext = ext.Substring(0, 3);
            }

            byte[] filenamebytes = Encoding.ASCII.GetBytes(filename);
            if (this.Deleted)
            {
                filenamebytes[0] = 0;
            }

            byte[] extbytes = Encoding.ASCII.GetBytes(ext);

            Array.Copy(filenamebytes, 0, buffer, 1, filenamebytes.Length);
            Array.Copy(extbytes, 0, buffer, 9, ext.Length);

            for (int i = 0; i < 4; i++)
            {
                if (this.SectorAllocationBlocks[i].LSN == 0)
                {
                    break;
                }
                else
                {
                    Array.Copy((byte[])this.SectorAllocationBlocks[i], 0, buffer, 12 + (i * 3), 3);
                }
            }

            if ((this.AttrByte & 0x20) == 0x20)
            {
                buffer[0x18] = this.NextDirectoryEntry;
            }
            else
            {
                buffer[0x18] = this.BytesUsedInLastSector;
            }

            return buffer;
        }

        /// <summary>
        /// Gets an array of bytes representing a DragonDos additional Sector Allocation Blocks
        /// </summary>
        /// <returns>Array of bytes representing a DragonDos additional Sector Allocation Blocks</returns>
        public byte[] GetDragonDosExtBytes()
        {
            byte[] buffer = new byte[0x19];

            buffer[0] = 1;
            if (this.Deleted)
            {
                buffer[0] |= 0x80;
            }

            if (this.Protected)
            {
                buffer[0] |= 0x02;
            }

            for (int i = 0; i < 7; i++)
            {
                if (this.SectorAllocationBlocks[4 + i].LSN == 0)
                {
                    break;
                }
                else
                {
                    Array.Copy((byte[])this.SectorAllocationBlocks[4 + i], 0, buffer, 1 + (i * 3), 3);
                }
            }

            buffer[0x18] = this.BytesUsedInLastSector;

            return buffer;
        }

        /// <summary>
        /// Puts an array of additional Sector Allocation Blocks
        /// </summary>
        /// <param name="buffer">Array of bytes representing a DragonDos additional Sector Allocation Blocks</param>
        public void PutDragonDosExtBytes(byte[] buffer)
        {
            for (int i = 0; i < 7; i++)
            {
                this.SectorAllocationBlocks[4 + i] = new SectorAllocationBlock(buffer.Subset(1 + (i * 3), 3));
            }

            int size = 0;
            for (int i = 0; i < 11; i++)
            {
                if (this.SectorAllocationBlocks[i].Sectors != 0)
                {
                    size += this.SectorAllocationBlocks[i].Sectors * 256;
                }
            }

            if (this.BytesUsedInLastSector != 0)
            {
                size -= 0x100 - this.BytesUsedInLastSector;
            }

            this.Size = (uint)size;
        }

        /// <summary>
        /// Gets an array of bytes representing the directory entry
        /// </summary>
        /// <returns>Array of bytes representing the directory entry</returns>
        public byte[] GetBytes()
        {
            switch (this.FormatType)
            {
                case DiskFormatType.OS9Format:
                    return this.GetOS9Bytes();
                case DiskFormatType.DragonDosFormat:
                    return this.GetDragonDosBytes();
                case DiskFormatType.RSDOSFormat:
                    return this.GetRSDOSBytes();
            }

            return null;
        }

        #endregion
    }
}
