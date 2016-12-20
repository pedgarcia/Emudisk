//-----------------------------------------------------------------------
// <copyright file="FormatTrackChangedEventArgs.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;

    /// <summary>
    /// Event arguments supplied when during a format operation
    /// </summary>
    internal class FormatTrackChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Cylinder being formatted
        /// </summary>
        private int track;

        /// <summary>
        /// Side being formatted
        /// </summary>
        private int head;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatTrackChangedEventArgs"/> class.
        /// </summary>
        /// <param name="track">Cylinder being formatted</param>
        /// <param name="head">Side being formatted</param>
        public FormatTrackChangedEventArgs(int track, int head)
        {
            this.track = track;
            this.head = head;
        }
        
        /// <summary>
        /// Gets the current Cylinder being formatted
        /// </summary>
        public int Track 
        { 
            get 
            { 
                return this.track; 
            } 
        }

        /// <summary>
        /// Gets the current Side being formatted
        /// </summary>
        public int Head 
        { 
            get 
            { 
                return this.head; 
            } 
        }
    }
}
