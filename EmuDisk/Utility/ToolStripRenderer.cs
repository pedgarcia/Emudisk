//-----------------------------------------------------------------------
// <copyright file="ToolStripRenderer.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Tool Strip Custom Renderer
    /// </summary>
    public class ToolStripRenderer : ToolStripSystemRenderer
    {
        /// <summary>
        /// Override the OnRenderItemText Method
        /// </summary>
        /// <param name="e">ToolStripItem Text Render Event Arguments</param>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item is ToolStripMenuItem)
            {
                if (((ToolStripMenuItem)e.Item).Checked)
                {
                    e.TextColor = Color.FromArgb(49, 106, 197);
                }
            }

            base.OnRenderItemText(e);
        }
    }
}
