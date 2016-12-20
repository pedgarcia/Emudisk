//-----------------------------------------------------------------------
// <copyright file="Crc16CalculatorStream.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// Performs CRC16 calculation on bytes passed through the base stream.
    /// </summary>
    internal class Crc16CalculatorStream : System.IO.Stream, System.IDisposable
    {
        /// <summary>
        /// Default length limit to indicate no length limit
        /// </summary>
        private static readonly long UnsetLengthLimit = -99;

        /// <summary>
        /// Contains the base stream object to pipe data through
        /// </summary>
        private System.IO.Stream innerStream;

        /// <summary>
        /// Contains the base crc class
        /// </summary>
        private Crc16 crc16;

        /// <summary>
        /// Contains the number of bytes to pass through the stream to process
        /// </summary>
        private long lengthLimit = -99;

        /// <summary>
        /// Contains the flag to leave the base stream open once done calculating
        /// </summary>
        private bool leaveOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc16CalculatorStream"/> class.
        /// </summary>
        /// <param name="stream">Base stream to perform operations on</param>
        public Crc16CalculatorStream(System.IO.Stream stream)
            : this(true, Crc16CalculatorStream.UnsetLengthLimit, stream, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc16CalculatorStream"/> class.
        /// </summary>
        /// <param name="stream">Base stream to perform operations on</param>
        /// <param name="leaveOpen">Leave the base stream open once operations are complete</param>
        public Crc16CalculatorStream(System.IO.Stream stream, bool leaveOpen)
            : this(leaveOpen, Crc16CalculatorStream.UnsetLengthLimit, stream, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc16CalculatorStream"/> class.
        /// </summary>
        /// <param name="stream">Base stream to perform operations on</param>
        /// <param name="length">Maximum number of stream bytes to process</param>
        public Crc16CalculatorStream(System.IO.Stream stream, long length)
            : this(true, length, stream, null)
        {
            if (length < 0)
            {
                throw new ArgumentException("length");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc16CalculatorStream"/> class.
        /// </summary>
        /// <param name="stream">Base stream to perform operations on</param>
        /// <param name="length">Maximum number of stream bytes to process</param>
        /// <param name="leaveOpen">Leave the base stream open once operations are complete</param>
        public Crc16CalculatorStream(System.IO.Stream stream, long length, bool leaveOpen)
            : this(leaveOpen, length, stream, null)
        {
            if (length < 0)
            {
                throw new ArgumentException("length");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc16CalculatorStream"/> class.
        /// </summary>
        /// <param name="stream">Base stream to perform operations on</param>
        /// <param name="length">Maximum number of stream bytes to process</param>
        /// <param name="leaveOpen">Leave the base stream open once operations are complete</param>
        /// <param name="crc16">Initial crc register value</param>
        public Crc16CalculatorStream(System.IO.Stream stream, long length, bool leaveOpen, Crc16 crc16)
            : this(leaveOpen, length, stream, crc16)
        {
            if (length < 0)
            {
                throw new ArgumentException("length");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc16CalculatorStream"/> class.
        /// </summary>
        /// <param name="leaveOpen">Leave the base stream open once operations are complete</param>
        /// <param name="length">Maximum number of stream bytes to process</param>
        /// <param name="stream">Base stream to perform operations on</param>
        /// <param name="crc16">Initial crc register value</param>
        private Crc16CalculatorStream(bool leaveOpen, long length, System.IO.Stream stream, Crc16 crc16)
            : base()
        {
            this.innerStream = stream;
            this.crc16 = crc16 ?? new Crc16();
            this.lengthLimit = length;
            this.leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Gets or sets the base stream
        /// </summary>
        public System.IO.Stream InnerStream
        {
            get
            {
                return this.innerStream;
            }

            set
            {
                this.innerStream = value;
            }
        }

        /// <summary>
        /// Gets the number of stream bytes processed
        /// </summary>
        public long TotalBytesSlurped
        {
            get { return this.crc16.TotalBytesRead; }
        }

        /// <summary>
        /// Gets the calculated crc value
        /// </summary>
        public int Crc
        {
            get { return this.crc16.Crc16Result; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to Leave base stream open when complete
        /// </summary>
        public bool LeaveOpen
        {
            get { return this.leaveOpen; }
            set { this.leaveOpen = value; }
        }

        /// <summary>
        /// Stream can be read from
        /// </summary>
        public override bool CanRead
        {
            get { return this.innerStream.CanRead; }
        }

        /// <summary>
        /// Can seek to new position in stream
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Stream can be written to
        /// </summary>
        public override bool CanWrite
        {
            get { return this.innerStream.CanWrite; }
        }

        /// <summary>
        /// Get the stream length if limit is unlimited, or otherwise return the limit length
        /// </summary>
        public override long Length
        {
            get
            {
                if (this.lengthLimit == Crc16CalculatorStream.UnsetLengthLimit)
                {
                    return this.innerStream.Length;
                }
                else
                {
                    return this.lengthLimit;
                }
            }
        }

        /// <summary>
        /// Get the current position in the stream;
        /// </summary>
        public override long Position
        {
            get { return this.crc16.TotalBytesRead; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Read and calculate crc from the next group of bytes from base stream
        /// </summary>
        /// <param name="buffer">returned group of read bytes</param>
        /// <param name="offset">the offset in the buffer to begin storing read bytes</param>
        /// <param name="count">the number of bytes to read from base stream and store in buffer</param>
        /// <returns>Returns the number of bytes read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesToRead = count;

            if (this.lengthLimit != Crc16CalculatorStream.UnsetLengthLimit)
            {
                if (this.crc16.TotalBytesRead >= this.lengthLimit)
                {
                    return 0; // EOF
                }

                long bytesRemaining = this.lengthLimit - this.crc16.TotalBytesRead;
                if (bytesRemaining < count)
                {
                    bytesToRead = (int)bytesRemaining;
                }
            }

            int n = this.innerStream.Read(buffer, offset, bytesToRead);
            if (n > 0)
            {
                this.crc16.SlurpBlock(buffer, offset, n);
            }

            return n;
        }

        /// <summary>
        /// Write a group of bytes to base stream and calculate crc on those bytes
        /// </summary>
        /// <param name="buffer">the group of bytes to write to the base stream</param>
        /// <param name="offset">the offset of the buffer to begin writing from</param>
        /// <param name="count">the number of bytes from buffer to write to base stream</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count > 0)
            {
                this.crc16.SlurpBlock(buffer, offset, count);
            }

            this.innerStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Seek to new position in stream
        /// </summary>
        /// <param name="offset">New position in stream</param>
        /// <param name="origin">Originate position from Beginning, Current or End of stream</param>
        /// <returns>Returns the new position of the stream after seeking</returns>
        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Set a new length for the stream, not supported
        /// </summary>
        /// <param name="value">The new length to set the stream to</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Force written data to be flushed through stream
        /// </summary>
        public override void Flush()
        {
            this.innerStream.Flush();
        }

        /// <summary>
        /// Dispose of the stream, close the base stream
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Close the stream, if not leave base stream open, then close base stream
        /// </summary>
        public override void Close()
        {
            base.Close();
            if (!this.leaveOpen)
            {
                this.innerStream.Close();
            }
        }
    }
}
