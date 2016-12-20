//-----------------------------------------------------------------------
// <copyright file="OS9Format.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// OS9 virtual disk support
    /// </summary>
    internal class OS9Format : BaseDiskFormat, IDiskFormat
    {
        #region Private Fields

        /// <summary>
        /// Logical sector 0, contains disk configuration information
        /// </summary>
        private LSN0 lsn0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OS9Format"/> class.
        /// </summary>
        /// <param name="diskImage">Underlying virtual disk image class</param>
        public OS9Format(IDiskImage diskImage)
        {
            this.DiskImage = diskImage;
            this.ValidateOS9();
            this.BaseDiskLabel = this.lsn0.VolumeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OS9Format"/> class
        /// to create a new logical disk with specific parameters.
        /// </summary>
        /// <param name="diskImage">Underlying virtual disk image class</param>
        /// <param name="tracks">Cylinders per side</param>
        /// <param name="heads">Number of sides</param>
        /// <param name="sectors">Sectors per track</param>
        /// <param name="sectorSize">Size of sectors in bytes</param>
        public OS9Format(IDiskImage diskImage, int tracks, int heads, int sectors, int sectorSize)
        {
            this.DiskImage = diskImage;
            this.BaseDiskLabel = string.Empty;
            this.LogicalTracks = tracks;
            this.LogicalHeads = heads;
            this.LogicalSectors = sectors;
            this.LogicalSectorSize = sectorSize;
            this.FormatDisk();
            this.ValidateOS9();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the disk's Logical Sector 0
        /// </summary>
        public LSN0 Lsn0
        {
            get
            {
                return this.lsn0;
            }
        }

        #endregion

        #region IDiskFormat Members

        #region Public Properties

        /// <summary>
        /// Gets an enum value of which disk format this class supports
        /// </summary>
        public DiskFormatType DiskFormat 
        { 
            get 
            { 
                return DiskFormatType.OS9Format; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether the disk represented is in a valid format for this class
        /// </summary>
        public bool IsValidFormat
        {
            get 
            {
                return this.ValidateOS9(); 
            }
        }

        /// <summary>
        /// Gets the amount of available space in bytes remaining on this disk
        /// </summary>
        public int FreeSpace
        {
            get 
            {
                byte[] bitmap = this.GetBitmap();
                return this.GetFreeClusers(bitmap) * this.LogicalSectorSize * this.lsn0.ClusterSize;
            }
        }

        /// <summary>
        /// Gets or sets the disk's volume label
        /// </summary>
        public string DiskLabel
        {
            get
            {
                return this.BaseDiskLabel;
            }

            set
            {
                this.lsn0.VolumeName = value;
                this.BaseDiskLabel = this.lsn0.VolumeName;
                this.WriteLSN(0, (byte[])this.lsn0);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the directory structure on this disk
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor</param>
        /// <returns>Directory class containing disk's directory entries</returns>
        public Directory GetDirectory(int lsn)
        {
            if (lsn == 0)
            {
                lsn = this.lsn0.RootDirectory;
            }

            Directory directory = new Directory();
            directory.FormatType = DiskFormatType.OS9Format;
            directory.LSN = (uint)lsn;
            directory.FileDescriptor = new FileDescriptor(this.ReadLSN(lsn));

            int directoryIndex = 0;
            foreach (FileSegment segment in directory.FileDescriptor.SegmentList)
            {
                if (segment.LSN == 0)
                {
                    break;
                }

                byte[] buffer = this.ReadLSNs(segment.LSN, segment.Sectors);
                for (int i = 0; i < buffer.Length; i += 32)
                {
                    if (!(directoryIndex < directory.FileDescriptor.Size))
                    {
                        break;
                    }

                    // ignore deleted entries
                    if (buffer[i] == 0x00)
                    {
                        directoryIndex += 32;
                        continue;
                    }

                    DirectoryEntry entry = new DirectoryEntry(buffer.Subset(i, 32), DiskFormatType.OS9Format);
                    entry.Descriptor = new FileDescriptor(this.ReadLSN(entry.LSN));
                    directory.Add(entry);
                }
            }

            return directory;
        }

        /// <summary>
        /// Gets a directory structure containing only subdirectories
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor</param>
        /// <returns>Directory class containing disk's subdirectory entries</returns>
        public override Directory GetDirectories(int lsn)
        {
            Directory directory = this.GetDirectory(lsn);
            foreach (DirectoryEntry entry in directory)
            {
                if (!entry.IsDirectory || entry.Filename == ".." || entry.Filename == ".")
                {
                    directory.Remove(entry);
                }
            }

            return directory;
        }

        /// <summary>
        /// Create a new directory
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">New directory information</param>
        /// <returns>lsn of file descriptor of new directory</returns>
        public override int CreateDirectory(int lsn, DirectoryEntry entry)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove a directory
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for directory to be removed</param>
        public override void DeleteDirectory(int lsn, DirectoryEntry entry)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rename a directory
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for directory to be renamed</param>
        /// <param name="newName">New directory name</param>
        public override void RenameDirectory(int lsn, DirectoryEntry entry, string newName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a file's data
        /// </summary>
        /// <param name="entry">Directory information for data to be retrieved</param>
        /// <returns>Byte array containing the file's data</returns>
        public byte[] GetFile(DirectoryEntry entry)
        {
            byte[] buffer = new byte[entry.Size];

            int bufferIndex = 0;
            foreach (FileSegment segment in entry.SegmentList)
            {
                if (segment.LSN == 0)
                {
                    break;
                }

                if (bufferIndex + (segment.Sectors * this.LogicalSectorSize) > entry.Size)
                {
                    Array.Copy(this.ReadLSNs(segment.LSN, segment.Sectors), 0, buffer, bufferIndex, entry.Size - bufferIndex);
                }
                else
                {
                    Array.Copy(this.ReadLSNs(segment.LSN, segment.Sectors), 0, buffer, bufferIndex, segment.Sectors * this.LogicalSectorSize);
                }

                bufferIndex += segment.Sectors * this.LogicalSectorSize;
            }

            return buffer;
        }

        /// <summary>
        /// Write a file's data to disk
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for file to be written</param>
        /// <param name="buffer">Byte array of file's data to be written</param>
        /// <returns>lsn of file descriptor of new file</returns>
        public override int PutFile(int lsn, DirectoryEntry entry, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a file from disk
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for file to be deleted</param>
        public override void DeleteFile(int lsn, DirectoryEntry entry)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rename a file on disk
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor for parent directory</param>
        /// <param name="entry">Directory information for file to be renamed</param>
        /// <param name="newName">New file name</param>
        public void RenameFile(int lsn, DirectoryEntry entry, string newName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change a file's attributes
        /// </summary>
        /// <param name="entry">Directory information for file to be changed</param>
        public void SetFileAttr(DirectoryEntry entry)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// High level format a disk
        /// </summary>
        public void FormatDisk()
        {
            // Create Empty Disk
            byte fillByte = 0xe5;
            byte[] buffer;

            if (!(DiskImage is PhysicalDisk))
            {
                buffer = new byte[this.LogicalTracks * this.LogicalHeads * this.LogicalSectors * this.LogicalSectorSize].Initialize(fillByte);
                this.WriteLSNs(0, buffer);
            }
            else
            {
                buffer = new byte[this.LogicalSectors * this.LogicalSectorSize].Initialize(fillByte);

                FormatDiskForm fdf = new FormatDiskForm();
                ((PhysicalDisk)DiskImage).FormatChanged += new FormatChangedEventHandler(fdf.Update);

                for (int track = 0; track < this.LogicalTracks; track++)
                {
                    for (int head = 0; head < this.LogicalHeads; head++)
                    {
                        ((PhysicalDisk)DiskImage).WriteTrack(track, head, buffer);
                    }
                }

                fdf.Close();
            }

            // Create LSN0
            this.lsn0 = new LSN0();
            this.lsn0.TotalSectors = this.LogicalTracks * this.LogicalHeads * this.LogicalSectors;
            this.lsn0.TrackSize = (byte)this.LogicalSectors;
            this.lsn0.MapBytes = (ushort)((this.lsn0.TotalSectors + 7) / 8);
            this.lsn0.ClusterSize = 1;
            this.lsn0.Owner = 0;
            this.lsn0.Attributes = 0xff;
            Random rnd = new Random(DateTime.Now.Second);
            this.lsn0.DiskID = (ushort)rnd.Next(0xffff);
            if (this.DiskImage is PartitionedVHDImage || this.DiskImage is VHDImage)
            {
                this.lsn0.DiskFormat = 0x82;
            }
            else
            {
                this.lsn0.DiskFormat = (byte)(0x02 + (this.LogicalHeads - 1));
            }

            this.lsn0.BootStrap = 0;
            this.lsn0.BootStrapSize = 0;
            this.lsn0.CreatedDate = DateTime.Now;
            this.lsn0.VolumeName = string.Empty;
            this.lsn0.SectorSize = (ushort)this.LogicalSectorSize;
            this.lsn0.RootDirectory = 1 + ((this.lsn0.MapBytes + (this.LogicalSectorSize - 1)) / this.LogicalSectorSize);
            this.lsn0.PathDescriptor.DeviceType = 0x01;
            this.lsn0.PathDescriptor.DriveNumber = 0x00;
            this.lsn0.PathDescriptor.DeviceType = 0x20;
            this.lsn0.PathDescriptor.Density = 0x01;
            this.lsn0.PathDescriptor.Cylinders = (ushort)this.DiskImage.PhysicalTracks;
            this.lsn0.PathDescriptor.Sides = (byte)this.DiskImage.PhysicalHeads;
            this.lsn0.PathDescriptor.SectorsPerTrack = (ushort)this.DiskImage.PhysicalSectors;
            this.lsn0.PathDescriptor.SectorsPerTrack0 = this.lsn0.PathDescriptor.SectorsPerTrack;
            this.lsn0.PathDescriptor.Interleave = (byte)this.DiskImage.Interleave;
            if (this.DiskImage is PartitionedVHDImage || this.DiskImage is VHDImage)
            {
                this.lsn0.PathDescriptor.SegmentAllocationSize = 0x20;
            }
            else
            {
                this.lsn0.PathDescriptor.SegmentAllocationSize = 8;
            }

            this.WriteLSN(0, (byte[])this.lsn0);

            // Create Allocation Bitmap
            int bitmapBits = this.LogicalTracks * this.LogicalHeads * this.LogicalSectors;
            int bitmapLSNs = ((bitmapBits / 8) + this.LogicalSectorSize - 1) / this.LogicalSectorSize;
            int allocatedSectors = 1 + bitmapLSNs + 8;

            byte[] bitmap = new byte[bitmapLSNs * this.LogicalSectorSize].Initialize(0xff);
            for (int i = 0; i < this.lsn0.TotalSectors; i++)
            {
                this.DeallocateLSN(i, bitmap);
            }

            for (int i = 0; i < allocatedSectors; i++)
            {
                this.AllocateLSN(i, bitmap);
            }

            this.WriteLSNs(0, bitmap);

            // Create root directory file descriptor
            FileDescriptor rootDescriptor = new FileDescriptor(0xbf, 0, this.lsn0.PackedCreationDate, 2, 0x40, Util.CreatedDateBytes(this.lsn0.CreatedDate));
            rootDescriptor.SegmentList = new FileSegment[48];
            rootDescriptor.SegmentList[0] = new FileSegment(this.lsn0.RootDirectory + 1, 7);
            this.WriteLSN(this.lsn0.RootDirectory, (byte[])rootDescriptor);

            // Create root directory
            buffer = new byte[7 * this.LogicalSectorSize];
            Directory rootDirectory = new Directory();
            rootDirectory.FormatType = DiskFormatType.OS9Format;
            rootDirectory.Add(new DirectoryEntry("..", this.lsn0.RootDirectory));
            rootDirectory.Add(new DirectoryEntry(".", this.lsn0.RootDirectory));
            Array.Copy((byte[])rootDirectory, 0, buffer, 0, 0x40);
            this.WriteLSNs(this.lsn0.RootDirectory + 1, buffer);
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Read a sector from the disk
        /// </summary>
        /// <param name="lsn">Logical sector number to read</param>
        /// <returns>Array of bytes containing sector data</returns>
        private byte[] ReadLSN(int lsn)
        {
            if (lsn == 0)
            {
                return DiskImage.ReadSector(0, 0, 1);
            }

            int track = lsn / (LogicalSectors * LogicalHeads);
            int head = (lsn / LogicalSectors) % LogicalHeads;
            int sector = (lsn % LogicalSectors) + 1;

            return DiskImage.ReadSector(track, head, sector);
        }

        /// <summary>
        /// Read a series of sectors from the disk
        /// </summary>
        /// <param name="lsn">Logical sector number to begin read from</param>
        /// <param name="count">Number of sectors to read</param>
        /// <returns>Array of bytes containing sectors data</returns>
        private byte[] ReadLSNs(int lsn, int count)
        {
            byte[] buffer = new byte[count * this.LogicalSectorSize];

            for (int i = 0, bufferOffset = 0; i < count; i++, bufferOffset += this.LogicalSectorSize)
            {
                Array.Copy(this.ReadLSN(lsn + i), 0, buffer, bufferOffset, this.LogicalSectorSize);
            }

            return buffer;
        }

        /// <summary>
        /// Read a series of sectors from the disk
        /// </summary>
        /// <param name="track">Beginning cylinder</param>
        /// <param name="head">Beginning side</param>
        /// <param name="sector">Beginning sector</param>
        /// <param name="sectorCount">Number of consecutive sectors to read</param>
        /// <returns>Array of bytes read from the disk</returns>
        private byte[] ReadSectors(int track, int head, int sector, int sectorCount)
        {
            byte[] buffer = new byte[sectorCount * this.LogicalSectorSize];

            for (int i = 0; i < sectorCount; i++)
            {
                Array.Copy(this.DiskImage.ReadSector(track, head, sector), 0, buffer, i * this.LogicalSectorSize, this.LogicalSectorSize);
                sector++;
                if (sector > this.LogicalSectors)
                {
                    sector = 1;
                    head++;
                    if (head > this.LogicalHeads - 1)
                    {
                        head = 0;
                        track++;
                        if (track > this.LogicalTracks - 1)
                        {
                            track = 0;
                        }
                    }
                }
            }

            return buffer;
        }

        /// <summary>
        /// Write a sector to the disk
        /// </summary>
        /// <param name="lsn">Logical sector number to write</param>
        /// <param name="buffer">Array of bytes containing sector data</param>
        private void WriteLSN(int lsn, byte[] buffer)
        {
            if (buffer.Length < this.LogicalSectorSize)
            {
                byte[] newBuffer = new byte[this.LogicalSectorSize];
                Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
                buffer = newBuffer;
            }

            if (lsn == 0)
            {
                this.DiskImage.WriteSector(0, 0, 1, buffer);
            }
            else
            {
                int track = lsn / (this.LogicalSectors * this.LogicalHeads);
                int head = (lsn / this.LogicalSectors) % this.LogicalHeads;
                int sector = (lsn % this.LogicalSectors) + 1;

                this.DiskImage.WriteSector(track, head, sector, buffer);
            }
        }

        /// <summary>
        /// Write a series of sectors to the disk
        /// </summary>
        /// <param name="lsn">Logical sector number to begin write to</param>
        /// <param name="buffer">Array of bytes containing sectors data</param>
        private void WriteLSNs(int lsn, byte[] buffer)
        {
            if (buffer.Length % this.LogicalSectorSize != 0)
            {
                byte[] newBuffer = new byte[((buffer.Length / this.LogicalSectorSize) + 1) * this.LogicalSectorSize];
                Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
                buffer = newBuffer;
            }

            for (int i = 0, bufferOffset = 0; i < buffer.Length / this.LogicalSectorSize; i++, bufferOffset += this.LogicalSectorSize)
            {
                this.WriteLSN(lsn + i, buffer.Subset(bufferOffset, this.LogicalSectorSize));
            }
        }

        /// <summary>
        /// Get the Allocation Bitmap data
        /// </summary>
        /// <returns>Array of bytes representing the allocation bitmap</returns>
        private byte[] GetBitmap()
        {
            byte[] bitmap = new byte[this.lsn0.MapBytes].Initialize(0xff);
            int bitmapSectors = (this.lsn0.MapBytes + (this.LogicalSectorSize - 1)) / this.LogicalSectorSize;
            return this.ReadLSNs(1, bitmapSectors);
        }

        /// <summary>
        /// Allocate a sector in the bitmap
        /// </summary>
        /// <param name="lsn">Logical sector number to allocate</param>
        /// <param name="bitmap">Bitmap structure to modify</param>
        private void AllocateLSN(int lsn, byte[] bitmap)
        {
            byte mask;

            mask = (byte)(1 << (7 - (lsn % 9)));
            bitmap[lsn / 8] |= mask;
        }

        /// <summary>
        /// De-allocate a sector in the bitmap
        /// </summary>
        /// <param name="lsn">Logical sector number to de-allocate</param>
        /// <param name="bitmap">Bitmap structure to modify</param>
        private void DeallocateLSN(int lsn, byte[] bitmap)
        {
            byte mask;

            mask = (byte)(1 << (7 - (lsn % 9)));
            bitmap[lsn / 8] &= (byte)~mask;
        }

        /// <summary>
        /// Get a count of unused sectors
        /// </summary>
        /// <param name="bitmap">Disk's Bitmap structure</param>
        /// <returns>Count of available sectors</returns>
        private int GetFreeClusers(byte[] bitmap)
        {
            int freeLSNs = 0;

            for (int i = 0; i < this.lsn0.TotalSectors; i++)
            {
                byte b = bitmap[i / 8];
                b >>= 7 - (i % 8);
                freeLSNs += ~b & 1;
            }

            return freeLSNs;
        }

        /// <summary>
        /// Validate the disk's data is a valid OS9 format
        /// </summary>
        /// <returns>A value specifying the validation was successful or not</returns>
        private bool ValidateOS9()
        {
            this.lsn0 = this.ReadLSN(0);

            if (this.lsn0.DiskFormat == 0x82)
            {
                this.LogicalHeads = 1;
            }
            else
            {
                this.LogicalHeads = ((this.lsn0.DiskFormat & 0x01) > 0) ? 2 : 1;
            }

            this.LogicalSectors = this.lsn0.SectorsPerTrack;
            this.LogicalSectorSize = this.lsn0.SectorSize;
            if (this.LogicalSectorSize == 0)
            {
                this.LogicalSectorSize = 256;
            }

            if (this.lsn0.PathDescriptor.SegmentAllocationSize == 0)
            {
                this.lsn0.PathDescriptor.SegmentAllocationSize = 8;
            }

            if (this.lsn0.TotalSectors == 0 || this.LogicalSectors == 0)
            {
                return false;
            }

            this.LogicalTracks = this.lsn0.TotalSectors / this.LogicalSectors / this.LogicalHeads;

            int bitmapLSNs = (this.lsn0.MapBytes + (this.LogicalSectorSize - 1)) / this.LogicalSectorSize;
            if (1 + bitmapLSNs > this.lsn0.RootDirectory)
            {
                return false;
            }

            if (this.LogicalSectors != this.lsn0.TrackSize)
            {
                return false;
            }

            if (this.LogicalSectors == 0)
            {
                return false;
            }

            if (this.lsn0.TotalSectors % this.LogicalSectors > 0)
            {
                return false;
            }

            byte[] bitmap = this.GetBitmap();

            for (int i = 0; i < bitmapLSNs; i++)
            {
                byte b = bitmap[i / 8];
                byte mask = (byte)(1 << (7 - (i % 8)));
                if ((b & mask) == 0)
                {
                    return false;
                }
            }

            if (this.lsn0.TotalSectors != this.LogicalTracks * this.LogicalHeads * this.LogicalSectors)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
