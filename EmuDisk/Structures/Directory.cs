//-----------------------------------------------------------------------
// <copyright file="Directory.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal class Directory : ICollection
    {
        #region Private Fields

        /// <summary>
        /// Contains a Collection of directory entries
        /// </summary>
        private List<DirectoryEntry> entries = new List<DirectoryEntry>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Directory"/> class.
        /// This is the default constructor.
        /// </summary>
        public Directory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Directory"/> class
        /// from an array of bytes.
        /// </summary>
        /// <param name="buffer">Array of bytes containing the directory information</param>
        /// <param name="diskFormatType">Disk Format to use when interpreting the Array of bytes</param>
        public Directory(byte[] buffer, DiskFormatType diskFormatType)
        {
            this.FormatType = diskFormatType;
            this.entries = new List<DirectoryEntry>();
            int offset = 0;

            switch (diskFormatType)
            {
                case DiskFormatType.DragonDosFormat:
                    List<byte[]> entries = new List<byte[]>();

                    while (offset < buffer.Length)
                    {
                        if ((buffer[offset] & 0x08) == 0x08)
                        {
                            break;
                        }

                        entries.Add(buffer.Subset(offset, 0x19));
                        offset += 19;
                        if ((offset + 6) % 100 == 0)
                        {
                            offset += 6;
                        }
                    }

                    byte[][] blocks = entries.ToArray();

                    foreach (byte[] block in blocks)
                    {
                        DirectoryEntry entry = new DirectoryEntry(block, this.FormatType);
                        this.entries.Add(entry);
                        if (entry.NextDirectoryEntry != 0)
                        {
                            entry.PutDragonDosExtBytes(blocks[entry.NextDirectoryEntry]);
                        }
                    }

                    break;
                case DiskFormatType.OS9Format:
                case DiskFormatType.RSDOSFormat:
                    while (offset < buffer.Length)
                    {
                        this.entries.Add(new DirectoryEntry(buffer.Subset(offset, 32), this.FormatType));
                        offset += 32;
                    }

                    break;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Directory's File Descriptor
        /// </summary>
        public FileDescriptor FileDescriptor { get; set; }

        /// <summary>
        /// Gets or sets the Directory's parent Logical Sector Number
        /// </summary>
        public uint LSN { get; set; }

        /// <summary>
        /// Gets or sets the directory's disk format type
        /// </summary>
        public DiskFormatType FormatType { get; set; }

        #region ICollection Properties

        /// <summary>
        /// Gets the count of directory entries in this directory
        /// </summary>
        public int Count 
        { 
            get 
            { 
                return this.entries.Count; 
            } 
        }

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized 
        { 
            get { throw new NotImplementedException(); } 
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        public object SyncRoot 
        { 
            get { throw new NotImplementedException(); } 
        }

        #endregion

        /// <summary>
        /// Gets a specified directory entry
        /// </summary>
        /// <param name="key">Filename of the directory entry</param>
        /// <returns>Individual directory entry</returns>
        public DirectoryEntry this[string key]
        {
            get
            {
                foreach (DirectoryEntry directoryEntry in this.entries)
                {
                    // File and directory names are case-insensitive
                    if (directoryEntry.Filename.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        return directoryEntry;
                    }
                }

                return null;
            }
        }

        #endregion

        #region Implicit Operators

        /// <summary>
        /// Implicit operator converting a directory to an array of bytes
        /// </summary>
        /// <param name="directory">Directory to convert from</param>
        /// <returns>Array of bytes representing the directory</returns>
        public static implicit operator byte[](Directory directory)
        {
            return directory.GetBytes();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a directory entry to the collection
        /// </summary>
        /// <param name="directoryEntry">Directory entry to be added</param>
        public void Add(DirectoryEntry directoryEntry)
        {
            this.entries.Add(directoryEntry);
        }

        /// <summary>
        /// Removes a directory entry from the collection
        /// </summary>
        /// <param name="directoryEntry">Directory entry to be removed</param>
        public void Remove(DirectoryEntry directoryEntry)
        {
            foreach (DirectoryEntry entry in this.entries)
            {
                if (entry.Filename == directoryEntry.Filename)
                {
                    this.entries.Remove(entry);
                }
            }
        }

        /// <summary>
        /// Check for an existing entry matching the requested filename, case insensitive
        /// </summary>
        /// <param name="fileName">Filename to check for</param>
        /// <returns>True if the collection contains a directory entry with the requested filename</returns>
        public bool Contains(string fileName)
        {
            foreach (DirectoryEntry entry in this.entries)
            {
                if (entry.Filename.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection. (Inherited from IEnumerable.)
        /// </summary>
        /// <returns>Enumerator that iterates through a collection</returns>
        public IEnumerator GetEnumerator()
        {
            return this.entries.GetEnumerator();
        }

        /// <summary>
        /// Returns an array of bytes representing the directory
        /// </summary>
        /// <returns>Array of bytes representing the directory</returns>
        public byte[] GetBytes()
        {
            byte[] data = null;
            int offset = 0, count = 0;
            
            switch (this.FormatType)
            {
                case DiskFormatType.DragonDosFormat:
                    data = new byte[16 * 256];

                    for (int i = 0; i < 160; i++)
                    {
                        Array.Copy(new byte[25], 0, data, offset, 25);
                        data[offset] = 0x89;
                        offset += 25;
                        if ((offset + 6) % 0x100 == 0)
                        {
                            offset += 6;
                        }
                    }

                    offset = 0;
                    foreach (DirectoryEntry entry in this.entries)
                    {
                        if (entry.NeedsExtension)
                        {
                            entry.NextDirectoryEntry = (byte)(count + 1);
                            entry.AttrByte |= 0x20;
                        }
                        else
                        {
                            entry.BytesUsedInLastSector = (byte)(entry.Size % 256);
                            entry.AttrByte &= 0xDF;
                        }

                        Array.Copy((byte[])entry, 0, data, offset, 0x19);
                        offset += 25;
                        if ((offset + 6) % 0x100 == 0)
                        {
                            offset += 6;
                        }

                        count++;

                        if (entry.NeedsExtension)
                        {
                            Array.Copy(entry.GetDragonDosExtBytes(), 0, data, offset, 0x19);
                            offset += 25;
                            if ((offset + 6) % 0x100 == 0)
                            {
                                offset += 6;
                            }

                            count++;
                        }
                    }

                    break;
                case DiskFormatType.OS9Format:
                case DiskFormatType.RSDOSFormat:
                    data = new byte[this.entries.Count * 32];

                    offset = 0;
                    foreach (DirectoryEntry directoryEntry in this.entries)
                    {
                        Array.Copy(directoryEntry.GetBytes(), 0, data, offset, 32);
                        offset += 32;
                    }

                    break;
            }

            return data;
        }

        #region ICollection Methods

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">New collection to copy to</param>
        /// <param name="index">Beginning index to start copying from</param>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion
    }
}
