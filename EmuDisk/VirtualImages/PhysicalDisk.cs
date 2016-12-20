//-----------------------------------------------------------------------
// <copyright file="PhysicalDisk.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// Delegate used to update formatting status
    /// </summary>
    /// <param name="sender">Sending object</param>
    /// <param name="e">Format Track Change event arguments</param>
    internal delegate void FormatChangedEventHandler(object sender, FormatTrackChangedEventArgs e);

    /// <summary>
    /// Physical Floppy Disk Support
    /// </summary>
    internal class PhysicalDisk : IDiskImage
    {
        #region FDRAWCMD.SYS Constants

        /// <summary>
        /// Minimum FDRAWCMD.SYS version
        /// </summary>
        private const uint FDRAWCMDVERSION = 0x01000109;

        /// <summary>
        /// FDRAWCMD.SYS Get Version command
        /// </summary>
        private const uint IOCTLFDRAWCMDGETVERSION = 0x0022e220;

        /// <summary>
        /// FDRAWCMD.SYS Seek command
        /// </summary>
        private const uint IOCTLFDCMDSEEK = 0x0022e03c;

        /// <summary>
        /// FDRAWCMD.SYS Read Data command
        /// </summary>
        private const uint IOCTLFDCMDREADDATA = 0x0022e01a;

        /// <summary>
        /// FDRAWCMD.SYS Write Data command
        /// </summary>
        private const uint IOCTLFDCMDWRITEDATA = 0x0022e015;

        /// <summary>
        /// FDRAWCMD.SYS Format Track command
        /// </summary>
        private const uint IOCTLFDCMDFORMATTRACK = 0x0022e034;

        /// <summary>
        /// FDRAWCMD.SYS Check for Disk Present command
        /// </summary>
        private const uint IOCTLFDCHECKDISK = 0x0022e45c;

        /// <summary>
        /// FDRAWCMD.SYS Set Data Rate command
        /// </summary>
        private const uint IOCTLFDSETDATARATE = 0x0022e410;

        /// <summary>
        /// FDRAWCMD.SYS MFM Option
        /// </summary>
        private const byte FDOPTIONMFM = 0x40;

        /// <summary>
        /// FDRAWCMD.SYS 500K Data Rate
        /// </summary>
        private const byte FDRATE500K = 0x00;

        /// <summary>
        /// FDRAWCMD.SYS 300K Data Rate
        /// </summary>
        private const byte FDRATE300K = 0x01;

        /// <summary>
        /// FDRAWCMD.SYS 250K Data Rate
        /// </summary>
        private const byte FDRATE250K = 0x02;

        /// <summary>
        /// FDRAWCMD.SYS 1M Data Rate
        /// </summary>
        private const byte FDRATE1M = 0x03;

        #endregion

        #region Private Fields

        /// <summary>
        /// Physical Drive Names
        /// </summary>
        private string[] drives = new string[] { "A:", "B:" };

        /// <summary>
        /// Gaps to use at various Sector Sizes
        /// </summary>
        private byte[] gap = new byte[] { 0x1b, 0x36, 0x54, 0x00, 0x74 };

        /// <summary>
        /// Physical Tracks
        /// </summary>
        private int tracks = 35;
        
        /// <summary>
        /// Physical Heads
        /// </summary>
        private int heads = 1;

        /// <summary>
        /// Physical Sectors
        /// </summary>
        private int sectors = 18;

        /// <summary>
        /// Physical Sector Size
        /// </summary>
        private int sectorsize = 256;

        /// <summary>
        /// Sector Interleave Factor
        /// </summary>
        private int interleave = 4;

        /// <summary>
        /// Data Rate
        /// </summary>
        private int datarate;

        /// <summary>
        /// Last Error encountered
        /// </summary>
        private int lasterr;

        /// <summary>
        /// Logical drive number
        /// </summary>
        private int drivenum;

        /// <summary>
        /// Handle to opened physical drive
        /// </summary>
        private SafeFileHandle handle;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalDisk"/> class
        /// </summary>
        /// <param name="drive">Drive Number</param>
        public PhysicalDisk(int drive)
        {
            this.drivenum = drive;
            this.OpenDisk(drive);
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Format track/head changed event handler
        /// </summary>
        public event FormatChangedEventHandler FormatChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the FDRAWCMD.SYS driver is installed and meets the minimum version
        /// </summary>
        public static bool DriverInstalled
        {
            get
            {
                uint dwRet = 0;
                int version = 0;
                uint paramSize = (uint)Marshal.SizeOf(version);
                IntPtr param = Marshal.AllocHGlobal((int)paramSize);

                SafeFileHandle handle = NativeMethods.CreateFile(@"\\.\fdrawcmd", NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE, 0, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
                NativeMethods.DeviceIoControl(handle, IOCTLFDRAWCMDGETVERSION, IntPtr.Zero, 0, param, paramSize, ref dwRet, IntPtr.Zero);

                version = Marshal.ReadInt32(param);
                Marshal.FreeHGlobal(param);
                param = IntPtr.Zero;

                if (version < FDRAWCMDVERSION)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets Disk Image Type
        /// </summary>
        public DiskImageType ImageType
        {
            get
            {
                return DiskImageType.PhysicalDisk;
            }
        }

        /// <summary>
        /// Gets or sets the track skew
        /// </summary>
        public int Skew { get; set; }

        #endregion

        #region Public IDiskImage Properties

        /// <summary>
        /// Gets Physical Drive Name
        /// </summary>
        public string Filename
        {
            get { return string.Format(MainForm.ResourceManager.GetString("PhysicalDisk_DriveName", MainForm.CultureInfo), this.drives[this.drivenum]); }
        }

        /// <summary>
        /// Gets or sets size of disk image's header
        /// </summary>
        public int HeaderLength
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
        /// Gets the number of cylinders
        /// </summary>
        public int PhysicalTracks
        {
            get
            {
                return this.tracks;
            }
        }

        /// <summary>
        /// Gets or sets the number of sides
        /// </summary>
        public int PhysicalHeads
        {
            get
            {
                return this.heads;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets the number of sectors per track
        /// </summary>
        public int PhysicalSectors
        {
            get
            {
                return this.sectors;
            }
        }

        /// <summary>
        /// Gets the size of a sector in bytes
        /// </summary>
        public int PhysicalSectorSize
        {
            get
            {
                return this.sectorsize;
            }
        }

        /// <summary>
        /// Gets or sets the sector interleave factor
        /// </summary>
        public int Interleave
        {
            get
            {
                return this.interleave;
            }

            set
            {
                this.interleave = value;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected partitions offset within the disk image
        /// </summary>
        public int PartitionOffset
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
        public int ImagePartitionOffset
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
        public int Partitions
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
        /// Gets or sets a value indicating whether the disk image is partitioned (PartitionedVHDImage)
        /// </summary>
        public bool IsPartitioned
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets a value indicating whether the disk image is a VHD or PartitionedVHD
        /// </summary>
        public bool IsHD
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data on the disk image is valid for the given disk image class
        /// </summary>
        public bool IsValidImage
        {
            get
            {
                return this.handle != null && !this.handle.IsInvalid;
            }

            set
            {
            }
        }

        #endregion

        #region Public IDiskImage Methods

        /// <summary>
        /// Read a sector from the disk image
        /// </summary>
        /// <param name="track">Physical Cylinder</param>
        /// <param name="head">Physical Side</param>
        /// <param name="sector">Physical Sector</param>
        /// <returns>Array of bytes containing sector data</returns>
        public byte[] ReadSector(int track, int head, int sector)
        {
            this.Seek(track, head);
            this.SetDataRate(this.datarate);
            byte[] b = new byte[this.sectorsize];
            FD_READ_WRITE_PARAMS rwParms = new FD_READ_WRITE_PARAMS() { Flags = FDOPTIONMFM, Phead = (byte)head, Cyl = (byte)track, Head = (byte)head, Sector = (byte)sector, Size = (byte)(this.sectorsize / 256), Eot = (byte)(sector + 1), Gap = this.gap[this.sectorsize / 256], Datalen = 0xff };

            this.DeviceIoControl(IOCTLFDCMDREADDATA, rwParms, ref b);

            return b;
        }

        /// <summary>
        /// Write a sector to the disk image
        /// </summary>
        /// <param name="track">Physical Cylinder</param>
        /// <param name="head">Physical Side</param>
        /// <param name="sector">Physical Sector</param>
        /// <param name="sectorData">Array of bytes containing sector data</param>
        public void WriteSector(int track, int head, int sector, byte[] sectorData)
        {
            this.Seek(track, head);
            this.SetDataRate(this.datarate);
            FD_READ_WRITE_PARAMS rwParms = new FD_READ_WRITE_PARAMS() { Flags = FDOPTIONMFM, Phead = (byte)head, Cyl = (byte)track, Head = (byte)head, Sector = (byte)sector, Size = (byte)(this.sectorsize / 256), Eot = (byte)(sector + 1), Gap = this.gap[this.sectorsize / 256], Datalen = 0xff };

            this.DeviceIoControl(IOCTLFDCMDWRITEDATA, rwParms, ref sectorData);
        }

        /// <summary>
        /// Format a sector on the disk image
        /// </summary>
        /// <param name="track">Physical Cylinder</param>
        /// <param name="head">Physical Side</param>
        /// <param name="sector">Physical Sector</param>
        /// <param name="fillByte">Byte pattern to use to fill the sector</param>
        public void FormatSector(int track, int head, int sector, byte fillByte)
        {
            byte[] buffer = new byte[this.sectorsize].Initialize(fillByte);
            this.WriteSector(track, head, sector, buffer);
        }

        /// <summary>
        /// Create a new disk image and low-level format it
        /// </summary>
        /// <param name="device">Physical Drive Device Name</param>
        /// <param name="tracks">Number of Cylinders</param>
        /// <param name="heads">Number of Sides</param>
        /// <param name="sectors">Number of Sectors per Track</param>
        /// <param name="sectorSize">Size of Sectors in bytes</param>
        public void CreateDisk(string device, int tracks, int heads, int sectors, int sectorSize)
        {
            this.tracks = tracks;
            this.heads = heads;
            this.sectors = sectors;
            this.sectorsize = sectorSize;
            this.datarate = 0;
            object outValue = null;

            this.handle = NativeMethods.CreateFile(device, NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE, 0, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
            if (this.handle.IsInvalid)
            {
                throw new DriveNotFoundException("PhysicalDisk_DriveNotFoundError", new string[] { this.drives[int.Parse(device.Substring(device.Length - 1))] });
            }

            if (!this.DeviceIoControl(IOCTLFDCHECKDISK, null, ref outValue))
            {
                throw new DiskNotPresentException();
            }

            this.SetDataRate(this.datarate);
            for (int track = 0; track < this.tracks; track++)
            {
                for (int head = 0; head < this.heads; head++)
                {
                    this.OnFormatChanged(track, head);
                    FD_FORMAT_PARAMS formatParams = new FD_FORMAT_PARAMS();
                    formatParams.Flags = FDOPTIONMFM;
                    formatParams.Phead = (byte)head;
                    formatParams.Size = (byte)(this.sectorsize / 256);
                    formatParams.Sectors = (byte)this.sectors;
                    formatParams.Gap = this.gap[this.sectorsize / 256];
                    formatParams.Fill = 0x00;
                    formatParams.Header = new FD_ID_HEADER[this.sectors];

                    for (int sector = 0, ph = 0; sector < this.sectors; sector++)
                    {
                        formatParams.Header[ph].Cyl = (byte)track;
                        formatParams.Header[ph].Head = (byte)head;
                        formatParams.Header[ph].Sector = (byte)(1 + (((sector + track) * (this.sectors - this.Skew)) % this.sectors));
                        formatParams.Header[ph].Size = (byte)(this.sectorsize / 256);
                        ph += this.interleave;
                        if (ph > this.sectors - 1)
                        {
                            ph -= this.sectors;
                        }

                        if (ph < this.interleave)
                        {
                            ph++;
                        }
                    }

                    this.Seek(track, head);
                    this.DeviceIoControl(IOCTLFDCMDFORMATTRACK, formatParams);
                }
            }
        }

        /// <summary>
        /// Refresh the disk's configuration
        /// </summary>
        public void GetDiskInfo()
        {
            this.Close();

            this.OpenDisk(this.drivenum);
        }

        /// <summary>
        /// Close the disk image
        /// </summary>
        public void Close()
        {
            if (this.handle != null)
            {
                NativeMethods.CloseHandle(this.handle);
            }

            this.handle = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Write an entire track
        /// </summary>
        /// <param name="track">Cylinder Number</param>
        /// <param name="head">Head Number</param>
        /// <param name="buffer">Array of bytes representing track data</param>
        public void WriteTrack(int track, int head, byte[] buffer)
        {
            this.Seek(track, head);
            this.OnFormatChanged(track, head);
            this.SetDataRate(this.datarate);
            FD_READ_WRITE_PARAMS rwParams = new FD_READ_WRITE_PARAMS() { Flags = FDOPTIONMFM, Phead = (byte)head, Cyl = (byte)track, Head = (byte)head, Sector = 0x01, Size = (byte)(this.PhysicalSectorSize / 256), Eot = (byte)(this.PhysicalSectors + 1), Gap = this.gap[this.PhysicalSectorSize / 256], Datalen = 0xff };

            this.DeviceIoControl(IOCTLFDCMDWRITEDATA, rwParams, ref buffer);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when a track/head is about to be formatted
        /// </summary>
        /// <param name="track">Physical Cylinder</param>
        /// <param name="head">Physical Head</param>
        protected virtual void OnFormatChanged(int track, int head)
        {
            if (this.FormatChanged != null)
            {
                this.FormatChanged(this, new FormatTrackChangedEventArgs(track, head));
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Opens and creates a handle to a Physical Drive
        /// </summary>
        /// <param name="drive">Drive Number</param>
        private void OpenDisk(int drive)
        {
            object outValue = null;

            this.handle = NativeMethods.CreateFile(@"\\.\fdraw" + drive, NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE, 0, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
            if (this.handle.IsInvalid)
            {
                throw new DriveNotFoundException("PhysicalDisk_DriveNotFoundError", new string[] { this.drives[drive] });
            }

            if (!this.DeviceIoControl(IOCTLFDCHECKDISK, null, ref outValue))
            {
                throw new DiskNotPresentException();
            }

            bool result = this.Seek(0, 0);
            if (!result)
            {
                throw new PhsycialDiskException("PhysicalDisk_SeekError", true);
            }

            // Find Data Rate and Sector Size
            result = false;
            for (int datarate = 3; datarate >= 0; datarate--)
            {
                this.datarate = (byte)datarate;
                byte size = 0x03;
                for (int sectorsize = 0; sectorsize < 4; sectorsize++)
                {
                    this.sectorsize = 128 << size;
                    this.ReadSector(0, 0, 1);
                    if (this.lasterr == 0)
                    {
                        result = true;
                        break;
                    }

                    size -= 1;
                }

                if (result)
                {
                    break;
                }
            }

            if (!result)
            {
                throw new DiskFormatException("PhysicalDisk_DiskFormatError", true);
            }

            // Find Sector Per Track
            result = this.Seek(0, 0);

            this.sectors = 18;
            for (int i = 0; i < 18; i++)
            {
                this.ReadSector(0, 0, i + 1);
                if (this.lasterr != 0)
                {
                    this.sectors = i - 1;
                    break;
                }
            }

            // Find Track Count
            if (this.Seek(0, 79))
            {
                this.tracks = 80;
            }
            else
            {
                if (this.Seek(0, 39))
                {
                    this.tracks = 40;
                }
                else
                {
                    if (this.Seek(0, 34))
                    {
                        this.tracks = 35;
                    }
                    else
                    {
                        throw new PhsycialDiskException("PhysicalDisk_TrackCountError", true);
                    }
                }
            }

            // Find Head Count
            if (this.Seek(1, 0))
            {
                this.heads = 2;
            }
            else
            {
                if (this.Seek(0, 0))
                {
                    this.heads = 1;
                }
                else
                {
                    throw new PhsycialDiskException("PhysicalDisk_HeadCountError", true);
                }
            }
        }

        /// <summary>
        /// Seek to specified Cylinder and Head
        /// </summary>
        /// <param name="track">Cylinder Number</param>
        /// <param name="head">Head Number</param>
        /// <returns>Operation Result</returns>
        private bool Seek(int track, int head)
        {
            object outValue = null;

            FD_SEEK_PARAMS seekParams = new FD_SEEK_PARAMS() { Cyl = (byte)track, Head = (byte)head };

            return this.DeviceIoControl(IOCTLFDCMDSEEK, seekParams, ref outValue);
        }

        /// <summary>
        /// Set Data Rate
        /// </summary>
        /// <param name="rate">Data Rate</param>
        /// <returns>Operation Result</returns>
        private bool SetDataRate(int rate)
        {
            return this.DeviceIoControl(IOCTLFDSETDATARATE, rate);
        }

        /// <summary>
        /// Performs low level Device Control of FDRAWCMD.SYS
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="inParams">Input Parameters</param>
        /// <returns>Operation Result</returns>
        private bool DeviceIoControl(uint command, object inParams)
        {
            object nullValue = null;
            return this.DeviceIoControl(command, inParams, ref nullValue);
        }

        /// <summary>
        /// Performs low level Device Control of FDRAWCMD.SYS
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="inParams">Input Parameters</param>
        /// <param name="outParams">Output Parameters - Array of bytes</param>
        /// <returns>Operation Result</returns>
        private bool DeviceIoControl(uint command, object inParams, ref byte[] outParams)
        {
            uint dwRet = 0;
            bool ret = false;

            IntPtr inParam = IntPtr.Zero;
            uint inParamSize = 0;
            if (inParams != null)
            {
                inParamSize = (uint)Marshal.SizeOf(inParams);
                inParam = Marshal.AllocHGlobal((int)inParamSize);
                if (inParams is int)
                {
                    Marshal.WriteInt32(inParam, (int)inParams);
                }
                else
                {
                    Marshal.StructureToPtr(inParams, inParam, true);
                }
            }

            IntPtr outParam = IntPtr.Zero;
            uint outParamSize = 0;
            if (outParams != null)
            {
                outParamSize = (uint)outParams.Length;
                outParam = Marshal.AllocHGlobal((int)outParamSize);
                Marshal.Copy(outParams, 0, outParam, (int)outParamSize);
            }

            ret = NativeMethods.DeviceIoControl(this.handle, command, inParam, inParamSize, outParam, outParamSize, ref dwRet, IntPtr.Zero);
            if (!ret)
            {
                this.lasterr = Marshal.GetLastWin32Error();
            }
            else
            {
                this.lasterr = 0;
            }

            if (inParam != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(inParam);
                inParam = IntPtr.Zero;
            }

            if (outParam != IntPtr.Zero)
            {
                Marshal.Copy(outParam, outParams, 0, outParams.Length);
                Marshal.FreeHGlobal(outParam);
                outParam = IntPtr.Zero;
            }

            return ret;
        }

        /// <summary>
        /// Performs low level Device Control of FDRAWCMD.SYS
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="inParams">Input Parameters</param>
        /// <param name="outParams">Output Parameters - structure</param>
        /// <returns>Operation Result</returns>
        private bool DeviceIoControl(uint command, object inParams, ref object outParams)
        {
            uint dwRet = 0;
            bool ret = false;

            IntPtr inParam = IntPtr.Zero;
            uint inParamSize = 0;
            if (inParams != null)
            {
                if (inParams is FD_FORMAT_PARAMS)
                {
                    inParamSize = (uint)((4 * ((FD_FORMAT_PARAMS)inParams).Header.Length) + 6);
                }
                else
                {
                    inParamSize = (uint)Marshal.SizeOf(inParams);
                }

                inParam = Marshal.AllocHGlobal((int)inParamSize);
                if (inParams is int)
                {
                    Marshal.WriteInt32(inParam, (int)inParams);
                }
                else
                {
                    Marshal.StructureToPtr(inParams, inParam, true);
                }
            }

            IntPtr outParam = IntPtr.Zero;
            uint outParamSize = 0;
            if (outParams != null)
            {
                outParamSize = (uint)Marshal.SizeOf(outParams);
                outParam = Marshal.AllocHGlobal((int)outParamSize);
                Marshal.StructureToPtr(outParams, outParam, false);
            }

            ret = NativeMethods.DeviceIoControl(this.handle, command, inParam, inParamSize, outParam, outParamSize, ref dwRet, IntPtr.Zero);
            if (!ret)
            {
                this.lasterr = Marshal.GetLastWin32Error();
            }
            else
            {
                this.lasterr = 0;
            }

            if (inParam != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(inParam);
                inParam = IntPtr.Zero;
            }

            if (outParam != IntPtr.Zero)
            {
                Marshal.PtrToStructure(outParam, outParams);
                Marshal.FreeHGlobal(outParam);
                outParam = IntPtr.Zero;
            }

            return ret;
        }

        #endregion

        #region FDRAWCMD.SYS Structures

        /// <summary>
        /// Seek Parameters structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct FD_SEEK_PARAMS
        {
            /// <summary>
            /// Cylinder to seek to
            /// </summary>
            public byte Cyl;

            /// <summary>
            /// Head to seek to
            /// </summary>
            public byte Head;
        }

        /// <summary>
        /// Read/Write Parameters structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct FD_READ_WRITE_PARAMS
        {
            /// <summary>
            /// Flags to read/write with - MT, MFM or SK
            /// </summary>
            public byte Flags;

            /// <summary>
            /// Physical Head
            /// </summary>
            public byte Phead;

            /// <summary>
            /// Cylinder, Head, Sector and Sector Size
            /// </summary>
            public byte Cyl, Head, Sector, Size;

            /// <summary>
            /// End of Track, Gap and Data Length
            /// </summary>
            public byte Eot, Gap, Datalen;
        }

        /// <summary>
        /// Format Parameters structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct FD_FORMAT_PARAMS
        {
            /// <summary>
            /// Flags to format with - MT, MFM or SK
            /// </summary>
            public byte Flags;

            /// <summary>
            /// Physical Head
            /// </summary>
            public byte Phead;

            /// <summary>
            /// Sector Size, Number of Sectors, Gap and Fill Byte
            /// </summary>
            public byte Size, Sectors, Gap, Fill;

            /// <summary>
            /// Series of Sector Headers to represent track
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray)]
            public FD_ID_HEADER[] Header;
        }

        /// <summary>
        /// Sector Header
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct FD_ID_HEADER
        {
            /// <summary>
            /// Cylinder, Head, Sector and Sector Size
            /// </summary>
            public byte Cyl, Head, Sector, Size;
        }

        #endregion
    }
}
