//-----------------------------------------------------------------------
// <copyright file="RSDOSFormat.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;
    using System.Text;

    /// <summary>
    /// RS Dos virtual disk support
    /// </summary>
    internal class RSDOSFormat : BaseDiskFormat, IDiskFormat
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RSDOSFormat"/> class.
        /// </summary>
        /// <param name="diskImage">Underlying virtual disk image class</param>
        public RSDOSFormat(IDiskImage diskImage)
        {
            this.DiskImage = diskImage;

            byte[] sector = this.DiskImage.ReadSector(17, 0, 17);
            int e = 0;
            for (int i = 0; i < sector.Length; i++)
            {
                if (sector[i] == 0 || sector[i] == 0xff)
                {
                    e = i;
                    break;
                }
            }

            if (e > 0)
            {
                this.BaseDiskLabel = Encoding.ASCII.GetString(sector.Subset(0, e)).Trim();
            }

            this.LogicalTracks = 35;
            this.LogicalHeads = 1;
            this.LogicalSectors = 18;
            this.LogicalSectorSize = 256;

            if (!(this.DiskImage is PartitionedVHDImage))
            {
                this.LogicalTracks = this.DiskImage.PhysicalTracks;
                this.LogicalHeads = this.DiskImage.PhysicalHeads;
                this.LogicalSectors = this.DiskImage.PhysicalSectors;
                this.LogicalSectorSize = this.DiskImage.PhysicalSectorSize;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSDOSFormat"/> class
        /// to create a new logical disk with specific parameters.
        /// </summary>
        /// <param name="diskImage">Underlying virtual disk image class</param>
        /// <param name="tracks">Cylinders per side</param>
        /// <param name="heads">Number of sides</param>
        /// <param name="sectors">Sectors per track</param>
        /// <param name="sectorSize">Size of sectors in bytes</param>
        public RSDOSFormat(IDiskImage diskImage, int tracks, int heads, int sectors, int sectorSize)
        {
            this.DiskImage = diskImage;
            this.BaseDiskLabel = string.Empty;
            this.LogicalTracks = tracks;
            this.LogicalHeads = heads;
            this.LogicalSectors = sectors;
            this.LogicalSectorSize = sectorSize;
            this.FormatDisk();
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
                return DiskFormatType.RSDOSFormat; 
            } 
        }

        /// <summary>
        /// Gets a value indicating whether the disk represented is in a valid format for this class
        /// </summary>
        public bool IsValidFormat
        {
            get 
            { 
                return this.ValidateRSDOS(); 
            }
        }

        /// <summary>
        /// Gets the amount of available space in bytes remaining on this disk
        /// </summary>
        public int FreeSpace
        {
            get
            {
                byte[] granuleMap = this.DiskImage.ReadSector(17, 0, 2);

                int granuleCount = ((this.LogicalTracks * this.LogicalHeads) - 1) * 2;
                if (granuleCount > 0x79)
                {
                    granuleCount = 0x79;
                }

                int freeGranuleCount = 0;
                for (int i = 0; i < granuleCount; i++)
                {
                    if (granuleMap[i] == 0xff)
                    {
                        freeGranuleCount++;
                    }
                }

                return freeGranuleCount * 9 * this.LogicalSectorSize;
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
                this.BaseDiskLabel = value;
                if (BaseDiskLabel.Length > 255)
                {
                    this.BaseDiskLabel = this.BaseDiskLabel.Substring(0, 255);
                }

                byte[] sector = new byte[this.LogicalSectorSize].Initialize(0xff);
                Array.Copy(Encoding.ASCII.GetBytes(this.BaseDiskLabel), 0, sector, 0, Encoding.ASCII.GetByteCount(this.BaseDiskLabel));
                sector[BaseDiskLabel.Length] = 0x00;
                this.DiskImage.WriteSector(17, 0, 17, sector);
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
            byte[] granuleMap = this.DiskImage.ReadSector(17, 0, 2);

            Directory directory = new Directory(this.ReadSectors(17, 0, 3, 8), DiskFormatType.RSDOSFormat);
            foreach (DirectoryEntry entry in directory)
            {
                int granuleCount = 0;
                int granuleIndex = entry.FirstGranule;
                while (granuleMap[granuleIndex] < 80)
                {
                    granuleCount++;
                    granuleIndex = granuleMap[granuleIndex];
                }

                entry.Size = (uint)((granuleCount * (9 * 256)) + (((granuleMap[granuleIndex] & 0x0f) - 1) * 256) + ((entry.LastSectorBytesMSB << 8) + entry.LastSectorBytesLSB));
            }

            return directory;
        }

        /// <summary>
        /// Gets a file's data
        /// </summary>
        /// <param name="entry">Directory information for data to be retrieved</param>
        /// <returns>Byte array containing the file's data</returns>
        public byte[] GetFile(DirectoryEntry entry)
        {
            byte[] buffer = new byte[entry.Size];

            byte[] granuleMap = this.DiskImage.ReadSector(17, 0, 2);
            int granuleIndex = entry.FirstGranule;
            
            int bufferIndex = 0;
            CHS chs;
            while (granuleMap[granuleIndex] < 0x80)
            {
                chs = this.ConvertGranuleToCHS(granuleIndex);
                Array.Copy(this.ReadSectors(chs.Track, chs.Head, chs.Sector, 9), 0, buffer, bufferIndex, 0x900);
                bufferIndex += 0x900;
                granuleIndex = granuleMap[granuleIndex];
            }

            chs = this.ConvertGranuleToCHS(granuleIndex);
            Array.Copy(this.ReadSectors(chs.Track, chs.Head, chs.Sector, granuleMap[granuleIndex] & 0x3f), 0, buffer, bufferIndex, (((granuleMap[granuleIndex] & 0x3f) - 1) * this.LogicalSectorSize) + (entry.LastSectorBytesMSB << 8) + entry.LastSectorBytesLSB);

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
            byte fillByte = 0xff;
            if (!(this.DiskImage is PhysicalDisk))
            {
                byte[] buffer = new byte[this.LogicalTracks * this.LogicalHeads * this.LogicalSectors * this.LogicalSectorSize].Initialize(fillByte);
                this.WriteSectors(0, 0, 1, buffer);
            }
            else
            {
                byte[] trackBuffer = new byte[this.LogicalSectors * this.LogicalSectorSize].Initialize(fillByte);

                FormatDiskForm fdf = new FormatDiskForm();
                ((PhysicalDisk)DiskImage).FormatChanged += new FormatChangedEventHandler(fdf.Update);

                for (int track = 0; track < this.LogicalTracks; track++)
                {
                    for (int head = 0; head < this.LogicalHeads; head++)
                    {
                        ((PhysicalDisk)DiskImage).WriteTrack(track, head, trackBuffer);
                    }
                }

                fdf.Close();
            }

            this.DiskLabel = string.Empty;
        }

        #endregion

        #endregion

        #region Private Methods

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
        /// Write a series of sectors to the disk
        /// </summary>
        /// <param name="track">Beginning cylinder</param>
        /// <param name="head">Beginning side</param>
        /// <param name="sector">Beginning sector</param>
        /// <param name="buffer">Array of bytes to write</param>
        private void WriteSectors(int track, int head, int sector, byte[] buffer)
        {
            if (buffer.Length % this.LogicalSectorSize != 0)
            {
                byte[] newBuffer = new byte[((buffer.Length / this.LogicalSectorSize) + 1) * this.LogicalSectorSize];
                Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
                buffer = newBuffer;
            }

            for (int i = 0, bufferOffset = 0; i < buffer.Length / this.LogicalSectorSize; i++, bufferOffset += this.LogicalSectorSize)
            {
                this.DiskImage.WriteSector(track, head, sector, buffer.Subset(bufferOffset, this.LogicalSectorSize));
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
        }

        /// <summary>
        /// Validate the disk's data is a valid RS Dos format
        /// </summary>
        /// <returns>A value specifying the validation was successful or not</returns>
        private bool ValidateRSDOS()
        {
            byte[] granuleMap = this.DiskImage.ReadSector(17, 0, 2);
            int granuleCount = ((this.LogicalTracks * this.LogicalHeads) - 1) * 2;
            if (granuleCount > 0x79)
            {
                granuleCount = 0x79;
            }

            // Test for valid granule map
            for (int i = 0; i < granuleCount; i++)
            {
                if (granuleMap[i] != 0xff && (granuleMap[i] > granuleCount) && (granuleMap[i] > 0x80 && ((granuleMap[i] & 0x0f) < 0 || (granuleMap[i] & 0x0f) > 9)))
                {
                    return false;
                }
            }

            // Test for valid directory entries
            byte[] directorySectors = this.ReadSectors(17, 0, 3, 8);
            for (int i = 0; i < directorySectors.Length; i += 16)
            {
                if (directorySectors[i] == 0xff)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        if (directorySectors[i + j] != 0xff)
                        {
                            return false;
                        }
                    }
                }

                if (directorySectors[i] != 0xff && (directorySectors[i + 11] > 0x03 || (directorySectors[i + 12] != 00 && directorySectors[i + 12] != 0xff)))
                {
                    return false;
                }

                if (directorySectors[i] != 0xff && directorySectors[i] != 0xff && (!directorySectors.Subset(i, 11).IsASCII(false)))
                {
                    return false;
                }
            }

            bool directoryEmpty = true;
            for (int i = 0; i < 256; i++)
            {
                if (directorySectors[i] != 0)
                {
                    directoryEmpty = false;
                    break;
                }
            }

            return !directoryEmpty;
        }

        /// <summary>
        /// Convert RS Dos Granule value to a CHS value
        /// </summary>
        /// <param name="granule">RS Dos Granule</param>
        /// <returns>Cylinder, Side and Sector structure</returns>
        private CHS ConvertGranuleToCHS(int granule)
        {
            CHS chs = new CHS();
            chs.Track = granule / 2;
            if (chs.Track > this.LogicalTracks)
            {
                chs.Track = chs.Track - this.LogicalTracks;
                chs.Head = 1;
            }
            else
            {
                chs.Head = 0;
            }

            if (chs.Track > 16 && chs.Head == 0)
            {
                chs.Track++;
            }
            else
            {
                if (chs.Head > 0)
                {
                    chs.Track++;
                }
            }

            if (granule % 2 > 0)
            {
                chs.Sector = 10;
            }
            else
            {
                chs.Sector = 1;
            }

            return chs;
        }

        /// <summary>
        /// Structure representing a Cylinder, Side and Sector
        /// </summary>
        private struct CHS
        {
            /// <summary>
            /// Value representing a disk's Cylinder
            /// </summary>
            public int Track;

            /// <summary>
            /// Value representing a disk's Side
            /// </summary>
            public int Head;

            /// <summary>
            /// Value representing a disk's Sector
            /// </summary>
            public int Sector;
        }

        #endregion
    }
}
