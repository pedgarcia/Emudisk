//-----------------------------------------------------------------------
// <copyright file="Util.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal class Util
    {
        /// <summary>
        /// Convert Array of bytes to a 32 bit unsigned integer
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>New 32 bit unsigned integer</returns>
        public static uint UInt32(byte[] bytes)
        {
            uint value = 0;
            if (bytes.Length > 0)
            {
                int pos = bytes.Length - 1;
                int count = 0;
                while (pos >= 0 && count < 4)
                {
                    value |= (uint)(bytes[pos--] << (count++ * 8));
                }
            }

            return value;
        }

        /// <summary>
        /// Convert Array of bytes to a Big Endian 32 bit unsigned integer
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>Big Endian 32 bit unsigned integer</returns>
        public static uint LEUint32(byte[] bytes)
        {
            uint value = 0;
            if (bytes.Length > 0)
            {
                int pos = 0;
                int count = 0;
                while (pos < bytes.Length && count < 4)
                {
                    value |= (uint)(bytes[pos++] << (count++ * 8));
                }
            }

            return value;
        }

        /// <summary>
        /// Convert Array of bytes to a 24 bit integer
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>24 bit integer</returns>
        public static int Int24(byte[] bytes)
        {
            int value = 0;
            if (bytes.Length > 0)
            {
                int pos = bytes.Length - 1;
                int count = 0;
                while (pos >= 0 && count < 3)
                {
                    value |= bytes[pos--] << (count++ * 8);
                }
            }   

            return value;
        }

        /// <summary>
        /// Convert Array of bytes to a Big Endian 24 bit integer
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>Big Endian 24 bit integer</returns>
        public static int LEInt24(byte[] bytes)
        {
            int value = 0;
            if (bytes.Length > 0)
            {
                int pos = 0;
                int count = 0;
                while (pos < bytes.Length && count < 3)
                {
                    value |= bytes[pos++] << (count++ * 8);
                }
            }

            return value;
        }

        /// <summary>
        /// Convert an array of bytes to a 16 bit unsigned integer
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>New 16 bit unsigned integer</returns>
        public static ushort UInt16(byte[] bytes)
        {
            ushort value = 0;
            if (bytes.Length > 0)
            {
                int pos = bytes.Length - 1;
                int count = 0;
                while (pos >= 0 && count < 2)
                {
                    value |= (ushort)(bytes[pos--] << (count++ * 8));
                }
            }

            return value;
        }

        /// <summary>
        /// Converts an array of bytes to a Big Endian 16 bit unsigned integer
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>Big Endian 16 bit unsigned integer</returns>
        public static ushort LEUInt16(byte[] bytes)
        {
            ushort value = 0;
            if (bytes.Length > 0)
            {
                int pos = 0;
                int count = 0;
                while (pos < bytes.Length && count < 2)
                {
                    value |= (ushort)(bytes[pos++] << (count++ * 8));
                }
            }

            return value;
        }

        /// <summary>
        /// Convert a 32 bit unsigned integer to 4 bytes
        /// </summary>
        /// <param name="value">32 bit unsigned integer</param>
        /// <returns>Array of bytes</returns>
        public static byte[] UInt32(uint value)
        {
            byte[] data = new byte[4];
            data[0] = (byte)(value >> 24);
            data[1] = (byte)((value >> 16) & 0xff);
            data[2] = (byte)((value >> 8) & 0xff);
            data[3] = (byte)(value & 0xff);
            return data;
        }

        /// <summary>
        /// Convert a 24 bit integer to 3 bytes
        /// </summary>
        /// <param name="value">24 bit integer</param>
        /// <returns>Array of bytes</returns>
        public static byte[] Int24(int value)
        {
            byte[] data = new byte[3];
            data[0] = (byte)(value >> 16);
            data[1] = (byte)((value >> 8) & 0xff);
            data[2] = (byte)(value & 0xff);
            return data;
        }

        /// <summary>
        /// Convert a 16 bit unsigned integer to 2 bytes
        /// </summary>
        /// <param name="value">16 bit unsigned integer</param>
        /// <returns>Array of bytes</returns>
        public static byte[] UInt16(ushort value)
        {
            byte[] data = new byte[2];
            data[0] = (byte)(value >> 8);
            data[1] = (byte)(value & 0xff);
            return data;
        }

        /// <summary>
        /// Convert an 5 byte array to a DateTime
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>DateTime structure</returns>
        public static DateTime ModifiedDate(byte[] bytes)
        {
            DateTime date = new DateTime();
            if (bytes == null || bytes.Length != 5)
            {
                return date;
            }

            try
            {
                date = new DateTime(1900 + bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], 0);
                if (date.Year < 1970)
                {
                    date.AddYears(100);
                }
            }
            catch
            {
                if (bytes[0] == 0 || bytes[1] == 0)
                {
                    date = DateTime.Now;
                }
                else
                {
                    try
                    {
                        date = new DateTime(2000 + bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], 0);
                    }
                    catch
                    {
                        date = DateTime.Now;
                    }
                }
            }

            return date;
        }

        /// <summary>
        /// Convert an 3 byte array to a DateTime
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>DateTime structure</returns>
        public static DateTime CreatedDate(byte[] bytes)
        {
            DateTime date = new DateTime();
            if (bytes == null || bytes.Length != 3)
            {
                return date;
            }

            try
            {
                date = new DateTime(1900 + bytes[0], bytes[1], bytes[2], 0, 0, 0);
                if (date.Year < 1970)
                {
                    date.AddYears(100);
                }
            }
            catch
            {
                if (bytes[0] == 0 || bytes[1] == 0)
                {
                    date = DateTime.Now;
                }
                else
                {
                    try
                    {
                        date = new DateTime(2000 + bytes[0], bytes[1], bytes[2], 0, 0, 0);
                    }
                    catch
                    {
                        date = DateTime.Now;
                    }
                }
            }

            return date;
        }

        /// <summary>
        /// Convert a DateTime structure to a 5 byte array
        /// </summary>
        /// <param name="date">DateTime structure</param>
        /// <returns>5-Byte array</returns>
        public static byte[] ModifiedDateBytes(DateTime date)
        {
            return new byte[] { (byte)(date.Year - 1900), (byte)date.Month, (byte)date.Day, (byte)date.Hour, (byte)date.Minute };
        }

        /// <summary>
        /// Convert a DateTime structure to a 3 byte array
        /// </summary>
        /// <param name="date">DateTime structure</param>
        /// <returns>3-Byte array</returns>
        public static byte[] CreatedDateBytes(DateTime date)
        {
            return new byte[] { (byte)(date.Year - 1900), (byte)date.Month, (byte)date.Day };
        }

        /// <summary>
        /// Gets a zero terminated string from an array of bytes
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>String value</returns>
        public static string GetString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0)
                {
                    break;
                }

                sb.Append((char)bytes[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a high-bit terminated string from an array of bytes
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>String value</returns>
        public static string GetHighBitString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                if ((bytes[i] & 0x80) == 0x80)
                {
                    sb.Append((char)(bytes[i] & 0x7f));
                    break;
                }

                sb.Append((char)bytes[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// <para>
        /// Truncate a path to fit within a certain number of characters 
        /// by replacing path components with ellipses.
        /// </para>
        /// <para>
        /// This solution is provided by CodeProject and GotDotNet C# expert
        /// Richard Deeming.
        /// </para>
        /// </summary>
        /// <param name="longName">Long file name</param>
        /// <param name="maxLen">Maximum length</param>
        /// <returns>Truncated file name</returns>
        public static string GetShortDisplayName(string longName, int maxLen)
        {
            StringBuilder pszOut = new StringBuilder(maxLen + maxLen + 2);  // for safety

            if (NativeMethods.PathCompactPathEx(pszOut, longName, maxLen, 0))
            {
                return pszOut.ToString();
            }
            else
            {
                return longName;
            }
        }
    }
}
