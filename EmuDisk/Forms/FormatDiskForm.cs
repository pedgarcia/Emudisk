//-----------------------------------------------------------------------
// <copyright file="FormatDiskForm.cs" company="Walter Zydhek">
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
    /// Dialog showing status of disk formatting
    /// </summary>
    internal partial class FormatDiskForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatDiskForm"/> class.
        /// This is the default constructor.
        /// </summary>
        public FormatDiskForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Update the status of the disk formatting
        /// </summary>
        /// <param name="o">Sending object</param>
        /// <param name="args">Format Track Changed event arguments</param>
        public void Update(object o, FormatTrackChangedEventArgs args)
        {
            this.label1.Text = string.Format(MainForm.ResourceManager.GetString("FormatDiskForm_FormattingDisk", MainForm.CultureInfo), args.Track, args.Head);
            Application.DoEvents();
        }
    }
}
