//-----------------------------------------------------------------------
// <copyright file="IDiskFormat.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// Common interface for All Disk Formats.
    /// </summary>
    internal interface IDiskFormat
    {
        /// <summary>
        /// Gets or sets the underlying disk image
        /// </summary>
        IDiskImage DiskImage { get; set; }

        /// <summary>
        /// Gets an enum value of which disk format this class supports
        /// </summary>
        DiskFormatType DiskFormat { get; }

        /// <summary>
        /// Gets a value indicating whether the disk represented is in a valid format for this class
        /// </summary>
        bool IsValidFormat { get; }

        /// <summary>
        /// Gets the logical number of cylinders
        /// </summary>
        int LogicalTracks { get; }

        /// <summary>
        /// Gets the logical number of sides
        /// </summary>
        int LogicalHeads { get; }

        /// <summary>
        /// Gets the logical number of sectors
        /// </summary>
        int LogicalSectors { get; }

        /// <summary>
        /// Gets the logical size of sectors in bytes
        /// </summary>
        int LogicalSectorSize { get; }

        /// <summary>
        /// Gets the total size of the virtual disk in number of bytes
        /// </summary>
        int DiskSpace { get; }

        /// <summary>
        /// Gets the amount of available space in bytes remaining on this disk
        /// </summary>
        int FreeSpace { get; }

        /// <summary>
        /// Gets or sets the disk's volume label
        /// </summary>
        string DiskLabel { get; set; }

        /// <summary>
        /// Gets the directory structure on this disk
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor</param>
        /// <returns>Directory class containing disk's directory entries</returns>
        Directory GetDirectory(int lsn);

        /// <summary>
        /// Get a directory structure containing only subdirectories
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor</param>
        /// <returns>Directory class containing disk's subdirectory entries</returns>
        Directory GetDirectories(int lsn);

        /// <summary>
        /// Create a new directory
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">New directory information</param>
        /// <returns>lsn of file descriptor of new directory</returns>
        int CreateDirectory(int lsn, DirectoryEntry entry);

        /// <summary>
        /// Remove a directory
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for directory to be removed</param>
        void DeleteDirectory(int lsn, DirectoryEntry entry);

        /// <summary>
        /// Rename a directory
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for directory to be renamed</param>
        /// <param name="newName">New directory name</param>
        void RenameDirectory(int lsn, DirectoryEntry entry, string newName);

        /// <summary>
        /// Gets a file's data
        /// </summary>
        /// <param name="entry">Directory information for data to be retrieved</param>
        /// <returns>Byte array containing the file's data</returns>
        byte[] GetFile(DirectoryEntry entry);

        /// <summary>
        /// Write a file's data to disk
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for file to be written</param>
        /// <param name="buffer">Byte array of file's data to be written</param>
        /// <returns>lsn of file descriptor of new file</returns>
        int PutFile(int lsn, DirectoryEntry entry, byte[] buffer);

        /// <summary>
        /// Delete a file from disk
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for file to be deleted</param>
        void DeleteFile(int lsn, DirectoryEntry entry);

        /// <summary>
        /// Rename a file on disk
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for file to be renamed</param>
        /// <param name="newName">New file name</param>
        void RenameFile(int lsn, DirectoryEntry entry, string newName);

        /// <summary>
        /// Change a file's attributes
        /// </summary>
        /// <param name="entry">Directory information for file to be changed</param>
        void SetFileAttr(DirectoryEntry entry);

        /// <summary>
        /// Replace a file on the disk
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for file to be replaced</param>
        /// <param name="buffer">Byte array of file's data to be written</param>
        void ReplaceFile(int lsn, DirectoryEntry entry, byte[] buffer);

        /// <summary>
        /// High level format a disk
        /// </summary>
        void FormatDisk();
    }
}
