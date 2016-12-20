//-----------------------------------------------------------------------
// <copyright file="FormatForm.Designer.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace EmuDisk
{
    /// <summary>
    /// Choose a disk format 
    /// </summary>
    internal partial class FormatForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// OK Button
        /// </summary>
        private System.Windows.Forms.Button btnOK;

        /// <summary>
        /// Cancel Button
        /// </summary>
        private System.Windows.Forms.Button btnCancel;

        /// <summary>
        /// OS9 Radio Button
        /// </summary>
        private System.Windows.Forms.RadioButton radOS9;

        /// <summary>
        /// RSDOS Radio Button
        /// </summary>
        private System.Windows.Forms.RadioButton radRSDOS;

        /// <summary>
        /// DragonDos Radio Button
        /// </summary>
        private System.Windows.Forms.RadioButton radDragonDos;

        /// <summary>
        /// Format Label
        /// </summary>
        private System.Windows.Forms.Label label1;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.radOS9 = new System.Windows.Forms.RadioButton();
            this.radRSDOS = new System.Windows.Forms.RadioButton();
            this.radDragonDos = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // radOS9
            // 
            this.radOS9.AutoSize = true;
            this.radOS9.Checked = true;
            this.radOS9.Location = new System.Drawing.Point(90, 34);
            this.radOS9.Name = "radOS9";
            this.radOS9.Size = new System.Drawing.Size(46, 17);
            this.radOS9.TabIndex = 0;
            this.radOS9.TabStop = true;
            this.radOS9.Text = "OS9";
            this.radOS9.UseVisualStyleBackColor = true;
            // 
            // radRSDOS
            // 
            this.radRSDOS.AutoSize = true;
            this.radRSDOS.Location = new System.Drawing.Point(90, 58);
            this.radRSDOS.Name = "radRSDOS";
            this.radRSDOS.Size = new System.Drawing.Size(63, 17);
            this.radRSDOS.TabIndex = 1;
            this.radRSDOS.Text = "RSDOS";
            this.radRSDOS.UseVisualStyleBackColor = true;
            // 
            // radDragonDos
            // 
            this.radDragonDos.AutoSize = true;
            this.radDragonDos.Location = new System.Drawing.Point(90, 82);
            this.radDragonDos.Name = "radDragonDos";
            this.radDragonDos.Size = new System.Drawing.Size(79, 17);
            this.radDragonDos.TabIndex = 2;
            this.radDragonDos.Text = "DragonDos";
            this.radDragonDos.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(193, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Which format would you like or this disk";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(44, 119);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(125, 119);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // FormatForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(248, 152);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radDragonDos);
            this.Controls.Add(this.radRSDOS);
            this.Controls.Add(this.radOS9);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormatForm";
            this.Text = "Format";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}