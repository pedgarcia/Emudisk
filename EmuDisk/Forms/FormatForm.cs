//-----------------------------------------------------------------------
// <copyright file="FormatForm.cs" company="Walter Zydhek">
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
    /// Choose a disk format 
    /// </summary>
    internal partial class FormatForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatForm"/> class
        /// </summary>
        public FormatForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets a value indicating whether the OS9 format was chosen
        /// </summary>
        public bool OS9
        {
            get
            {
                return this.radOS9.Checked;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the RSDOS format was chosen
        /// </summary>
        public bool RSDOS
        {
            get
            {
                return this.radRSDOS.Checked;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the DragonDos format was chosen
        /// </summary>
        public bool DragonDos
        {
            get
            {
                return this.radDragonDos.Checked;
            }
        }

        /// <summary>
        /// Gets or sets the label's text
        /// </summary>
        public string Label1
        {
            get
            {
                return label1.Text;
            }

            set
            {
                label1.Text = value;
            }
        }
        
        /// <summary>
        /// Handle the OK button event
        /// </summary>
        /// <param name="sender">Sending Object</param>
        /// <param name="e">Event Arguments</param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Handle the Cancel button event
        /// </summary>
        /// <param name="sender">Sending Object</param>
        /// <param name="e">Event Arguments</param>
        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
