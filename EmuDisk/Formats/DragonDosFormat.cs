//-----------------------------------------------------------------------
// <copyright file="DragonDosFormat.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// <para>
    /// Dragon Dos virtual disk support
    /// </para>
    /// <para>
    /// 3.  Disk Layout
    /// ===============
    /// </para>
    /// <para>
    ///     MFM Double Density
    ///      40 or 80 tracks (0 - 39 or 0 - 79)
    ///       1 or  2 sides
    ///      18 sectors per track (1 - 18)
    ///     256 bytes per sector
    ///     2:1 interleave
    /// </para>
    /// <para>
    /// Drive numbering: Dragon DOS refers to drives 1-4 (physical units 0-3),
    /// drive 0 can also be used to refer to drive 1.
    /// </para>
    /// <para>
    /// Double sided disks are usually considered as having 36 sectors per
    /// track rather than referring to side numbers.
    /// </para>
    /// <para>
    /// Logical Sector Numbers (LSNs) are used to refer to a sector relative to the
    /// first sector on the disk.  LSN 0x000 == Track 0, Side 0, Sector 1.
    /// </para>
    /// <para>
    /// 40 Tracks, 1 Side,  18 Sectors:
    /// </para>
    /// <para>
    ///   LSN 0x000  Track 0 Side 0 Sector 1
    ///   LSN 0x012  Track 1 Side 0 Sector 1
    /// </para>
    /// <para>
    /// 40 Tracks, 2 Sides, 36 Sectors:
    /// </para>
    /// <para>
    ///   LSN 0x000  Track 0 Side 0 Dragon Sector  1 Physical Sector 1
    ///   LSN 0x012  Track 0 Side 1 Dragon Sector 19 Physical Sector 1
    ///   LSN 0x024  Track 1 Side 0 Dragon Sector  1 Physical Sector 1
    /// </para>
    /// <para>
    /// 4.  Directory Track
    /// ===================
    /// </para>
    /// <para>
    /// Track 20 contains the Directory information
    /// </para>
    /// <para>
    /// Sectors 1 and 2 hold the sector bitmap
    /// </para>
    /// <para>
    /// Track 20 Sector 1 identifies the disk format as follows:
    /// Offset
    /// 0xfc    Tracks on disk
    /// 0xfd    Sectors per track (36 indicates double sided / 18 secs per track)
    /// 0xfe    One's complement of offset 0xfc
    /// 0xff    One's complement of offset 0xfd
    /// </para>
    /// <para>
    /// Sectors 3 - 18 hold directory entries.
    /// Each directory entry is 25 bytes long - 160 entries max numbered 0 - 159.
    /// Each sector contains 10 directory entries as follows:
    /// 0x00 - 0x18 Directory Entry  1
    /// 0x19 - 0x31 Directory Entry  2
    /// 0x32 - 0x4a Directory Entry  3
    /// 0x4b - 0x63 Directory Entry  4
    /// 0x64 - 0x7c Directory Entry  5
    /// 0x7d - 0x95 Directory Entry  6
    /// 0x96 - 0xae Directory Entry  7
    /// 0xaf - 0xc7 Directory Entry  8
    /// 0xc8 - 0xe0 Directory Entry  9
    /// 0xe1 - 0xf9 Directory Entry 10
    /// 0xfa - 0xff 6 unused bytes - usually 0x00 - used by some programs to
    ///         store long disk labels
    /// </para>
    /// <para>
    /// 4.1  Format of Sector Bitmap
    /// ----------------------------
    /// </para>
    /// <para>
    /// The sector bitmap is split across sectors 1 and 2 of track 20 
    /// Sector 1:
    /// 0x00 - 0xb3 Bitmap for LSNs 0x000 - 0x59f
    /// 0xb4 - 0xfb Unused - 0x00 - used by DosPlus for label and something else
    /// 0xfc - 0xff Disk format information (see above)
    /// </para>
    /// <para>
    /// Sector 2:
    /// 0x00 - 0xb3 Bitmap for LSNs 0x5a0 - 0xb3f (80 Track, DS only)
    /// 0xb4 - 0xff Unused - 0x00
    /// </para>
    /// <para>
    /// Each bit in the sector bitmap represents a single logical sector number -
    /// 0 = used, 1 = free
    /// </para>
    /// <para>
    /// LSN
    /// 0x000   Sector 1 Byte 0x00 Bit 0
    /// 0x007   Sector 1 Byte 0x00 Bit 7
    /// 0x008   Sector 1 Byte 0x01 Bit 0
    /// 0x59f   Sector 1 Byte 0xb3 Bit 7
    /// 0x5a0   Sector 2 Byte 0x00 Bit 0
    /// 0xb3f   Sector 2 Byte 0xb3 Bit 7
    /// </para>
    /// <para>
    /// 4.2  Directory entry format
    /// ---------------------------
    /// </para>
    /// <para>
    /// 0x00        flag byte
    ///     bit 7   Deleted - this entry may be reused
    ///     bit 6   Unused
    ///     bit 5   Continued - byte at offset 0x18 gives next entry number
    ///     bit 4   Unused
    ///     bit 3   End of Directory - no further entries need to be scanned
    ///     bit 2   Unused
    ///     bit 1   Protect Flag - file should not be overwritten
    ///     bit 0   Continuation Entry - this entry is a Continuation Block
    /// </para>
    /// <para>
    /// 0x01 - 0x17 File Header Block or Continuation Block
    /// </para>
    /// <para>
    /// 0x18    [flag byte bit 5 == 0]
    ///         Bytes used in last sector (0x00 == 256 bytes)
    ///         [flag byte bit 5 == 1]
    ///         Next directory entry num (0-159)
    /// </para>
    /// <para>
    /// File Header block:  (flag byte bit 0 == 0)
    /// </para>
    /// <para>
    /// 0x01 - 0x08 filename (padded with 0x00)
    /// 0x09 - 0x0b extension (padded with 0x00)
    /// 0x0c - 0x0e Sector Allocation Block 1
    /// 0x0f - 0x11 Sector Allocation Block 2
    /// 0x12 - 0x14 Sector Allocation Block 3
    /// 0x15 - 0x17 Sector Allocation Block 4
    /// </para>
    /// <para>
    /// Continuation block:  (flag byte bit 0 == 1)
    /// </para>
    /// <para>
    /// 0x01 - 0x03 Sector Allocation Block 1
    /// 0x04 - 0x06 Sector Allocation Block 2
    /// 0x07 - 0x09 Sector Allocation Block 3
    /// 0x0a - 0x0c Sector Allocation Block 4
    /// 0x0d - 0x0f Sector Allocation Block 5
    /// 0x10 - 0x12 Sector Allocation Block 6
    /// 0x13 - 0x15 Sector Allocation Block 7
    /// 0x16 : 0x17 Unused
    /// </para>
    /// <para>
    /// Sector Allocation Block format:
    /// </para>
    /// <para>
    /// 0x00 : 0x01 Logical Sector Number of first sector in this block
    /// 0x02        Count of contiguous sectors in this block
    /// </para>
    /// <para>
    /// 5.  Bootable disks
    /// ==================
    /// </para>
    /// <para>
    /// The characters 'OS' at the offsets 0 and 1 of LSN 2 (Track 0, sector 3)
    /// signify the disk is bootable.
    /// </para>
    /// <para>
    /// The DragonDos BOOT command checks for this signature returning BT error
    /// if not found.  Otherwise, Sectors 3-18 of track 0 are loaded into memory
    /// at address $2600 and execution begins at $2602.
    /// </para>
    /// <para>
    /// Note that the operation of BOOT under the original DragonDos does not set the
    /// default drive number to that passed to the BOOT command but DosPlus does.
    /// </para>
    /// </summary>
    internal class DragonDosFormat : BaseDiskFormat, IDiskFormat
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DragonDosFormat"/> class.
        /// </summary>
        /// <param name="diskImage">Underlying virtual disk image class</param>
        public DragonDosFormat(IDiskImage diskImage)
        {
            this.DiskImage = diskImage;

            this.LogicalTracks = 40;
            this.LogicalHeads = 1;
            this.LogicalSectors = 18;
            this.LogicalSectorSize = 256;

            if (!(this.DiskImage is PartitionedVHDImage))
            {
                this.LogicalTracks = DiskImage.PhysicalTracks;
                this.LogicalHeads = DiskImage.PhysicalHeads;
                this.LogicalSectors = DiskImage.PhysicalSectors;
                this.LogicalSectorSize = DiskImage.PhysicalSectorSize;
            }

            this.ValidateDragonDos();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DragonDosFormat"/> class
        /// to create a new logical disk with specific parameters.
        /// </summary>
        /// <param name="diskImage">Underlying virtual disk image class</param>
        /// <param name="tracks">Cylinders per side</param>
        /// <param name="heads">Number of sides</param>
        /// <param name="sectors">Sectors per track</param>
        /// <param name="sectorSize">Size of sectors in bytes</param>
        public DragonDosFormat(IDiskImage diskImage, int tracks, int heads, int sectors, int sectorSize)
        {
            this.DiskImage = diskImage;

            this.LogicalTracks = tracks;
            this.LogicalHeads = heads;
            this.LogicalSectors = sectors;
            this.LogicalSectorSize = sectorSize;
            this.FormatDisk();

            this.ValidateDragonDos();
        }

        #endregion

        #region IDiskFormat Members

        /// <summary>
        /// Gets an enum value of which disk format this class supports
        /// </summary>
        public DiskFormatType DiskFormat 
        { 
            get 
            { 
                return DiskFormatType.DragonDosFormat; 
            } 
        }

        /// <summary>
        /// Gets a value indicating whether the disk represented is in a valid format for this class
        /// </summary>
        public bool IsValidFormat
        {
            get 
            { 
                return this.ValidateDragonDos(); 
            }
        }

        /// <summary>
        /// Gets the amount of available space in bytes remaining on this disk
        /// </summary>
        public int FreeSpace
        {
            get 
            {
                return this.GetFreeSectors(this.DiskImage.ReadSector(20, 0, 1), this.DiskImage.ReadSector(20, 1, 1));
            }
        }

        /// <summary>
        /// Gets or sets the disk's volume label
        /// </summary>
        public string DiskLabel
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        /// <summary>
        /// Returns the directory structure on this disk
        /// </summary>
        /// <param name="lsn">Used to specify which sector holds the file descriptor</param>
        /// <returns>Directory class containing disk's directory entries</returns>
        public Directory GetDirectory(int lsn)
        {
            Directory directory = new Directory(this.ReadSectors(20, 0, 3, 16), DiskFormatType.DragonDosFormat);

            return directory;
        }

        /// <summary>
        /// Gets a file's data
        /// </summary>
        /// <param name="entry">Directory information for data to be retrieved</param>
        /// <returns>Byte array containing the file's data</returns>
        public byte[] GetFile(DirectoryEntry entry)
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
                byte[] trackBuffer = new byte[LogicalSectors * LogicalSectorSize].Initialize(fillByte);

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

            Directory directory = new Directory();
            this.WriteSectors(20, 0, 3, directory);
            this.WriteSectors(16, 0, 3, directory);

            byte[] bitmap0 = new byte[256];
            for (int i = 0; i < (40 * this.LogicalHeads * this.LogicalSectors); i++)
            {
                this.DeallocateSector(i, bitmap0);
            }

            for (int i = 1; i < 18; i++)
            {
                this.AllocateSector(16, 0, i, bitmap0);
                this.AllocateSector(20, 0, i, bitmap0);
            }

            bitmap0[0xfc] = (byte)this.LogicalTracks;
            bitmap0[0xfd] = (byte)(this.LogicalSectors * this.LogicalHeads);
            bitmap0[0xfe] = (byte)~bitmap0[0xfc];
            bitmap0[0xff] = (byte)~bitmap0[0xfd];

            this.DiskImage.WriteSector(20, 0, 1, bitmap0);
            this.DiskImage.WriteSector(16, 0, 1, bitmap0);

            if (this.LogicalTracks > 40)
            {
                byte[] bitmap1 = new byte[256];
                Array.Copy(new byte[0xb4].Initialize(0xff), 0, bitmap1, 0, 0xb4);

                this.DiskImage.WriteSector(20, 0, 2, bitmap1);
                this.DiskImage.WriteSector(16, 0, 2, bitmap1);
            }

            this.DiskLabel = string.Empty;
        }

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
        /// Allocate a sector in the bitmap
        /// </summary>
        /// <param name="track">Specified cylinder</param>
        /// <param name="head">Specified side</param>
        /// <param name="sector">Specified sector</param>
        /// <param name="map">Bitmap structure to modify</param>
        private void AllocateSector(int track, int head, int sector, byte[] map)
        {
            this.AllocateSector(this.ConvertCHStoLSN(track, head, sector), map);
        }

        /// <summary>
        /// Allocate a sector in the bitmap
        /// </summary>
        /// <param name="lsn">Logical sector number to allocate</param>
        /// <param name="map">Bitmap structure to modify</param>
        private void AllocateSector(int lsn, byte[] map)
        {
            byte mask = (byte)(1 << (lsn % 8));
            map[lsn / 8] &= (byte)~mask;
        }

        /// <summary>
        /// De-allocate a sector in the bitmap
        /// </summary>
        /// <param name="track">Specified cylinder</param>
        /// <param name="head">Specified side</param>
        /// <param name="sector">Specified sector</param>
        /// <param name="map">Bitmap structure to modify</param>
        private void DeallocateSector(int track, int head, int sector, byte[] map)
        {
            this.DeallocateSector(this.ConvertCHStoLSN(track, head, sector), map);
        }

        /// <summary>
        /// De-allocate a sector in the bitmap
        /// </summary>
        /// <param name="lsn">Logical sector number to de-allocate</param>
        /// <param name="map">Bitmap structure to modify</param>
        private void DeallocateSector(int lsn, byte[] map)
        {
            byte mask = (byte)(1 << (lsn % 8));
            map[lsn / 8] |= mask;
        }

        /// <summary>
        /// Get a count of unused sectors
        /// </summary>
        /// <param name="bitmap0">Bitmap for side 0 of disk</param>
        /// <param name="bitmap1">Bitmap for side 1 of disk</param>
        /// <returns>Count of available sectors</returns>
        private int GetFreeSectors(byte[] bitmap0, byte[] bitmap1)
        {
            int freeLSNs = 0;

            for (int i = 0; i < 40 * this.LogicalHeads * this.LogicalSectors; i++)
            {
                byte b = bitmap0[i / 8];
                b >>= i % 8;
                freeLSNs += ~b & 1;
            }

            if (this.LogicalTracks == 80)
            {
                for (int i = 0; i < 40 * this.LogicalHeads * this.LogicalSectors; i++)
                {
                    byte b = bitmap1[i / 8];
                    b >>= i % 8;
                    freeLSNs += ~b & 1;
                }
            }

            return freeLSNs;
        }
        
        /// <summary>
        /// Convert a Cylinder, Side, Sector structure to a Logical Sector Number
        /// </summary>
        /// <param name="track">Specified cylinder</param>
        /// <param name="head">Specified side</param>
        /// <param name="sector">Specified sector</param>
        /// <returns>Logical Sector Number</returns>
        private int ConvertCHStoLSN(int track, int head, int sector)
        {
            return (((track * this.LogicalHeads) + head) * this.LogicalSectors) + sector - 1;
        }
        
        /// <summary>
        /// Validate the disk's data is a valid DragonDos format
        /// </summary>
        /// <returns>A value specifying the validation was successful or not</returns>
        private bool ValidateDragonDos()
        {
            byte[] bitmap0 = this.DiskImage.ReadSector(20, 0, 1);
            this.LogicalTracks = bitmap0[0xfc];
            this.LogicalSectors = bitmap0[0xfd];
            this.LogicalHeads = 1;
            this.LogicalSectorSize = 256;
            if (this.LogicalSectors == 36)
            {
                this.LogicalSectors = 18;
                this.LogicalHeads = 2;
            }

            if (bitmap0[0xfe] != ~bitmap0[0xfc] || bitmap0[0xff] != ~bitmap0[0xfd])
            {
                return false;
            }

            if (this.LogicalTracks != 40 && this.LogicalTracks != 80)
            {
                return false;
            }

            if (this.LogicalSectors != 18)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
