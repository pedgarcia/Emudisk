//-----------------------------------------------------------------------
// <copyright file="DiskViewForm.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Form used to show the disk's directories and files.
    /// </summary>
    internal partial class DiskViewForm : Form
    {
        #region Private Fields

        /// <summary>
        /// Contains the Disk Format instance
        /// </summary>
        private IDiskFormat diskFormat;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskViewForm"/> class.
        /// </summary>
        /// <param name="diskFormat">Disk Format instance</param>
        public DiskViewForm(IDiskFormat diskFormat)
        {
            this.InitializeComponent();
            this.diskFormat = diskFormat;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the disk's format
        /// </summary>
        public IDiskFormat DiskFormat
        {
            get
            {
                return this.diskFormat;
            }

            set
            {
                this.diskFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the Partition Menu Items for this instance
        /// </summary>
        public ToolStripMenuItem PartitionItems { get; set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handle the form's Closing Event
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Form Closing Event arguments</param>
        private void DiskViewForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            this.diskFormat.DiskImage.Close();
        }

        #endregion
    }
}
