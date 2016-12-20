//-----------------------------------------------------------------------
// <copyright file="MainForm.Designer.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    /// <summary>
    /// Main Application Form
    /// </summary>
    public partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Main Menu container
        /// </summary>
        private System.Windows.Forms.MenuStrip menuStrip1;

        /// <summary>
        /// File Menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;

        /// <summary>
        /// Open File menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;

        /// <summary>
        /// Physical Drive SubMenu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem physicalDriveToolStripMenuItem;

        /// <summary>
        /// Drive A: menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem driveAToolStripMenuItem;

        /// <summary>
        /// Drive B: menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem driveBToolStripMenuItem;

        /// <summary>
        /// New Disk menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;

        /// <summary>
        /// Close Disk menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;

        /// <summary>
        /// Exit Application menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;

        /// <summary>
        /// Disk Menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem diskToolStripMenuItem;

        /// <summary>
        /// Disk Information menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;

        /// <summary>
        /// Format Disk menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem formatToolStripMenuItem;

        /// <summary>
        /// Re-label Disk menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem reLabelToolStripMenuItem;

        /// <summary>
        /// Sector Editor menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem sectorEditorToolStripMenuItem;

        /// <summary>
        /// Go to specified Partition menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem gotoRGBDOSDriveToolStripMenuItem;

        /// <summary>
        /// Select Partition menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem selectRGBDOSDriveToolStripMenuItem;

        /// <summary>
        /// Bootstrap menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem bootstrapToolStripMenuItem;

        /// <summary>
        /// Window Menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;

        /// <summary>
        /// Close Window menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem closeWindowToolStripMenuItem;

        /// <summary>
        /// Window Menu separator
        /// </summary>
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

        /// <summary>
        /// Cascade Windows menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem cascadeToolStripMenuItem;

        /// <summary>
        /// Tile Windows Horizontally menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem tileHorizontalToolStripMenuItem;

        /// <summary>
        /// Tile Windows Vertically menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem tileVerticalToolStripMenuItem;

        /// <summary>
        /// Help Menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;

        /// <summary>
        /// About menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;

        /// <summary>
        /// Language Menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;

        /// <summary>
        /// English Language menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;

        /// <summary>
        /// Spanish Language menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem spanishToolStripMenuItem;

        /// <summary>
        /// Status Bar container
        /// </summary>
        private System.Windows.Forms.StatusStrip statusStrip1;

        /// <summary>
        /// Recent Files Used menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem recentFilesToolStripMenuItem;

        /// <summary>
        /// Options Menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.physicalDriveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.driveAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.driveBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sectorEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gotoRGBDOSDriveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectRGBDOSDriveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bootstrapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spanishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 569);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(843, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.diskToolStripMenuItem,
            this.windowToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.languageToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(843, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Renderer = new ToolStripRenderer();
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.physicalDriveToolStripMenuItem,
            this.newToolStripMenuItem,
            this.recentFilesToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // physicalDriveToolStripMenuItem
            // 
            this.physicalDriveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.driveAToolStripMenuItem,
            this.driveBToolStripMenuItem});
            this.physicalDriveToolStripMenuItem.Name = "physicalDriveToolStripMenuItem";
            this.physicalDriveToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.physicalDriveToolStripMenuItem.Text = "Physical &Drive";
            // 
            // driveAToolStripMenuItem
            // 
            this.driveAToolStripMenuItem.Name = "driveAToolStripMenuItem";
            this.driveAToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.driveAToolStripMenuItem.Text = "Drive &A:";
            this.driveAToolStripMenuItem.Click += new System.EventHandler(this.DriveAToolStripMenuItem_Click);
            // 
            // driveBToolStripMenuItem
            // 
            this.driveBToolStripMenuItem.Name = "driveBToolStripMenuItem";
            this.driveBToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.driveBToolStripMenuItem.Text = "Drive &B:";
            this.driveBToolStripMenuItem.Click += new System.EventHandler(this.DriveBToolStripMenuItem_Click);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.newToolStripMenuItem.Text = "&New...";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItem_Click);
            // 
            // recentFilesToolStripMenuItem
            // 
            this.recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
            this.recentFilesToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.recentFilesToolStripMenuItem.Text = "Recent Files";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // diskToolStripMenuItem
            // 
            this.diskToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoToolStripMenuItem,
            this.formatToolStripMenuItem,
            this.reLabelToolStripMenuItem,
            this.sectorEditorToolStripMenuItem,
            this.gotoRGBDOSDriveToolStripMenuItem,
            this.selectRGBDOSDriveToolStripMenuItem,
            this.bootstrapToolStripMenuItem});
            this.diskToolStripMenuItem.Name = "diskToolStripMenuItem";
            this.diskToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.diskToolStripMenuItem.Text = "&Disk";
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.infoToolStripMenuItem.Text = "&Info";
            this.infoToolStripMenuItem.Click += new System.EventHandler(this.InfoToolStripMenuItem_Click);
            // 
            // formatToolStripMenuItem
            // 
            this.formatToolStripMenuItem.Name = "formatToolStripMenuItem";
            this.formatToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.formatToolStripMenuItem.Text = "F&ormat";
            this.formatToolStripMenuItem.Click += new System.EventHandler(this.FormatToolStripMenuItem_Click);
            // 
            // reLabelToolStripMenuItem
            // 
            this.reLabelToolStripMenuItem.Name = "reLabelToolStripMenuItem";
            this.reLabelToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.reLabelToolStripMenuItem.Text = "Re-&Label...";
            this.reLabelToolStripMenuItem.Click += new System.EventHandler(this.ReLabelToolStripMenuItem_Click);
            // 
            // sectorEditorToolStripMenuItem
            // 
            this.sectorEditorToolStripMenuItem.Name = "sectorEditorToolStripMenuItem";
            this.sectorEditorToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.sectorEditorToolStripMenuItem.Text = "&Sector Editor";
            this.sectorEditorToolStripMenuItem.Click += new System.EventHandler(this.SectorEditorToolStripMenuItem_Click);
            // 
            // gotoRGBDOSDriveToolStripMenuItem
            // 
            this.gotoRGBDOSDriveToolStripMenuItem.Name = "gotoRGBDOSDriveToolStripMenuItem";
            this.gotoRGBDOSDriveToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.gotoRGBDOSDriveToolStripMenuItem.Text = "&Goto RGBDOS Drive...";
            this.gotoRGBDOSDriveToolStripMenuItem.Click += new System.EventHandler(this.GotoRGBDOSDriveToolStripMenuItem_Click);
            // 
            // selectRGBDOSDriveToolStripMenuItem
            // 
            this.selectRGBDOSDriveToolStripMenuItem.Name = "selectRGBDOSDriveToolStripMenuItem";
            this.selectRGBDOSDriveToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.selectRGBDOSDriveToolStripMenuItem.Text = "Select RGBDOS &Drive";
            // 
            // bootstrapToolStripMenuItem
            // 
            this.bootstrapToolStripMenuItem.Name = "bootstrapToolStripMenuItem";
            this.bootstrapToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.bootstrapToolStripMenuItem.Text = "&Bootstrap";
            this.bootstrapToolStripMenuItem.Click += new System.EventHandler(this.BootstrapToolStripMenuItem_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeWindowToolStripMenuItem,
            this.toolStripSeparator1,
            this.cascadeToolStripMenuItem,
            this.tileHorizontalToolStripMenuItem,
            this.tileVerticalToolStripMenuItem});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "&Window";
            // 
            // closeWindowToolStripMenuItem
            // 
            this.closeWindowToolStripMenuItem.Name = "closeWindowToolStripMenuItem";
            this.closeWindowToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.closeWindowToolStripMenuItem.Text = "Cl&ose Window";
            this.closeWindowToolStripMenuItem.Click += new System.EventHandler(this.CloseWindowToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(148, 6);
            // 
            // cascadeToolStripMenuItem
            // 
            this.cascadeToolStripMenuItem.Name = "cascadeToolStripMenuItem";
            this.cascadeToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.cascadeToolStripMenuItem.Text = "&Cascade";
            this.cascadeToolStripMenuItem.Click += new System.EventHandler(this.CascadeToolStripMenuItem_Click);
            // 
            // tileHorizontalToolStripMenuItem
            // 
            this.tileHorizontalToolStripMenuItem.Name = "tileHorizontalToolStripMenuItem";
            this.tileHorizontalToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.tileHorizontalToolStripMenuItem.Text = "Tile &Horizontal";
            this.tileHorizontalToolStripMenuItem.Click += new System.EventHandler(this.TileHorizontalToolStripMenuItem_Click);
            // 
            // tileVerticalToolStripMenuItem
            // 
            this.tileVerticalToolStripMenuItem.Name = "tileVerticalToolStripMenuItem";
            this.tileVerticalToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.tileVerticalToolStripMenuItem.Text = "Tile &Vertical";
            this.tileVerticalToolStripMenuItem.Click += new System.EventHandler(this.TileVerticalToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.spanishToolStripMenuItem});
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            this.languageToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.languageToolStripMenuItem.Text = "Lan&guage";
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.CheckOnClick = true;
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.englishToolStripMenuItem.Tag = "en";
            this.englishToolStripMenuItem.Text = "&English";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.LanguageChange_Click);
            // 
            // spanishToolStripMenuItem
            // 
            this.spanishToolStripMenuItem.CheckOnClick = true;
            this.spanishToolStripMenuItem.Name = "spanishToolStripMenuItem";
            this.spanishToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.spanishToolStripMenuItem.Tag = "es";
            this.spanishToolStripMenuItem.Text = "&Spanish";
            this.spanishToolStripMenuItem.Click += new System.EventHandler(this.LanguageChange_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.OptionsToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 591);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "EmuDisk";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
