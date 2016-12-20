//-----------------------------------------------------------------------
// <copyright file="BaseDiskFormat.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal class BaseDiskFormat
    {
        #region Private Fields

        /// <summary>
        /// Contains the disk's volume label
        /// </summary>
        private string diskLabel;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the underlying disk image
        /// </summary>
        public IDiskImage DiskImage { get; set; }

        /// <summary>
        /// Gets or sets the logical number of cylinders
        /// </summary>
        public int LogicalTracks { get; set; }

        /// <summary>
        /// Gets or sets the logical number of sides
        /// </summary>
        public int LogicalHeads { get; set;  }
        
        /// <summary>
        /// Gets or sets the logical number of sectors
        /// </summary>
        public int LogicalSectors { get; set; }
        
        /// <summary>
        /// Gets or sets the logical size of sectors in bytes
        /// </summary>
        public int LogicalSectorSize { get; set; }

        /// <summary>
        /// Gets the total size of the virtual disk in number of bytes
        /// </summary>
        public int DiskSpace
        {
            get { return this.LogicalHeads * this.LogicalTracks * this.LogicalSectors * this.LogicalSectorSize; }
        }

        /// <summary>
        /// Gets or sets the volume label
        /// </summary>
        internal string BaseDiskLabel
        {
            get
            {
                return this.diskLabel;
            }

            set
            {
                this.diskLabel = value;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Not implemented, used in other classes to return a directory structure containing only subdirectories
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor</param>
        /// <returns>Directory class containing disk's subdirectory entries</returns>
        public virtual Directory GetDirectories(int lsn)
        {
            return null;
        }

        /// <summary>
        /// Not implemented, Create a new directory
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">New directory information</param>
        /// <returns>lsn of file descriptor of new directory</returns>
        public virtual int CreateDirectory(int lsn, DirectoryEntry entry)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented, Remove a directory
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for directory to be removed</param>
        public virtual void DeleteDirectory(int lsn, DirectoryEntry entry)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented, Rename a directory
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for directory to be renamed</param>
        /// <param name="newName">New directory name</param>
        public virtual void RenameDirectory(int lsn, DirectoryEntry entry, string newName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write a file's data to disk
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for file to be written</param>
        /// <param name="buffer">Byte array of file's data to be written</param>
        /// <returns>lsn of file descriptor of new file</returns>
        public virtual int PutFile(int lsn, DirectoryEntry entry, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a file from disk
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for file to be deleted</param>
        public virtual void DeleteFile(int lsn, DirectoryEntry entry)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replace a file on the disk
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for file to be replaced</param>
        /// <param name="buffer">Byte array of file's data to be written</param>
        public void ReplaceFile(int lsn, DirectoryEntry entry, byte[] buffer)
        {
            this.DeleteFile(lsn, entry);
            this.PutFile(lsn, entry, buffer);
        }

        #endregion
    }
}
