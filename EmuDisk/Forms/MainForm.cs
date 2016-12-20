//-----------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Walter Zydhek">
//     Copyright (c) Walter Zydhek. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EmuDisk
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Resources;
    using System.Windows.Forms;

    /// <summary>
    /// Main Application Form
    /// </summary>
    public partial class MainForm : Form, IMRUClient
    {
        #region Private Fields

        /// <summary>
        /// Registry path to store application settings
        /// </summary>
        private const string RegistryPath = "Software\\WaltZ\\EmuDisk";

        /// <summary>
        /// Current Culture Information
        /// </summary>
        private static CultureInfo cultureInfo;

        /// <summary>
        /// Resource Manager used to get localized strings
        /// </summary>
        private static ResourceManager resourceManager;

        /// <summary>
        /// True if FDRAWCMD.SYS driver is installed
        /// </summary>
        private static bool driverInstalled = PhysicalDisk.DriverInstalled;

        /// <summary>
        /// MRU List manager
        /// </summary>
        private MRUManager mruManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// This is the default constructor.
        /// </summary>
        public MainForm()
        {
            resourceManager = new ResourceManager("EmuDisk.Resource.Res", typeof(MainForm).Assembly);
            this.InitializeComponent();
            string currentCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            foreach (ToolStripMenuItem item in this.languageToolStripMenuItem.DropDownItems)
            {
                if (item.Tag.ToString() == currentCulture)
                {
                    item.Checked = true;
                    this.SetLanguage(currentCulture);
                    break;
                }
            }

            if (cultureInfo == null)
            {
                this.SetLanguage("en");
            }

            this.mruManager = new MRUManager();
            this.mruManager.Initialize(this, this.recentFilesToolStripMenuItem, RegistryPath);
            this.physicalDriveToolStripMenuItem.Enabled = driverInstalled;
            this.UpdateMenu();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class
        /// with argument list of disk image files to open
        /// </summary>
        /// <param name="args">Argument list of disk files to open</param>
        public MainForm(string[] args) : this()
        {
            foreach (string arg in args)
            {
                this.OpenDiskView(arg);
            }

            this.UpdateMenu();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current Culture Information
        /// </summary>
        public static CultureInfo CultureInfo
        {
            get
            {
                return cultureInfo;
            }
        }

        /// <summary>
        /// Gets the Resource Manager used to get localized strings
        /// </summary>
        public static ResourceManager ResourceManager
        {
            get
            {
                return resourceManager;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Static InputBox provider
        /// </summary>
        /// <param name="title">Caption of InputBox</param>
        /// <param name="promptText">Prompt asking for information</param>
        /// <param name="maxLength">Maximum length of requested input</param>
        /// <param name="value">Variable to store result in</param>
        /// <returns>Dialog result of the InputBox</returns>
        public static DialogResult InputBox(string title, string promptText, int maxLength, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            textBox.MaxLength = maxLength;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        /// <summary>
        /// Most Recently Used menu item handler
        /// </summary>
        /// <param name="fileName">Selected filename to open</param>
        public void OpenMRUFile(string fileName)
        {
            this.OpenDiskView(fileName);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Open virtual disk and display it in a DiskViewForm instance
        /// </summary>
        /// <param name="filename">Filename to open</param>
        internal void OpenDiskView(string filename)
        {
            IDiskImage diskimage = null;
            IDiskFormat diskformat = null;

            if (!File.Exists(filename))
            {
                this.mruManager.Remove(filename);
                return;
            }

            switch (Path.GetExtension(filename).ToUpper())
            {
                case ".OS9":
                    diskimage = new OS9Image(filename);
                    break;
                case ".JVC":
                    diskimage = new JVCImage(filename);
                    break;
                case ".VDK":
                    diskimage = new VDKImage(filename);
                    break;
                case ".DMK":
                    diskimage = new DMKImage(filename);
                    break;
                case ".VHD":
                    diskimage = new VHDImage(filename);
                    if (!diskimage.IsValidImage)
                    {
                        diskimage = new PartitionedVHDImage(filename);
                    }

                    break;
                case ".DSK":
                    diskimage = new JVCImage(filename);
                    if (!diskimage.IsValidImage)
                    {
                        diskimage = new DMKImage(filename);
                    }

                    break;
            }

            if (diskimage == null || !diskimage.IsValidImage)
            {
                this.mruManager.Remove(filename);
                MessageBox.Show(string.Format(resourceManager.GetString("MainForm_NotValidDiskImage", cultureInfo), filename), resourceManager.GetString("MainForm_NotValidDiskImageCaption", cultureInfo), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (diskimage.IsPartitioned)
                {
                    ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
                    PartitionedVHDImage pi = (PartitionedVHDImage)diskimage;

                    Cursor cursor = this.Cursor;
                    this.Cursor = Cursors.WaitCursor;
                    this.selectRGBDOSDriveToolStripMenuItem.DropDownItems.Clear();
                    ToolStripMenuItem tsi;

                    int i = 0;
                    if (diskimage.ImagePartitionOffset != 0)
                    {
                        tsi = new ToolStripMenuItem();
                        tsi.Name = "0";
                        tsi.Text = "OS9 Drive";
                        tsi.Size = new System.Drawing.Size(152, 22);
                        tsi.CheckOnClick = true;
                        tsi.Click += new EventHandler(this.SelectRGBDOSDrive_Click);
                        tsi.Image = (System.Drawing.Image)resources.GetObject("OS9.image");
                        diskformat = new OS9Format(diskimage);
                        if (diskformat != null && diskformat.IsValidFormat && !string.IsNullOrEmpty(diskformat.DiskLabel))
                        {
                            tsi.Text = diskformat.DiskLabel;
                        }

                        this.selectRGBDOSDriveToolStripMenuItem.DropDownItems.Add(tsi);
                        i++;
                    }

                    for (int j = 0; i < diskimage.Partitions; j++, i++)
                    {
                        tsi = new ToolStripMenuItem();
                        tsi.Name = i.ToString();
                        tsi.Text = string.Format("RGBDOS Drive {0}", j);
                        tsi.Size = new System.Drawing.Size(152, 22);
                        tsi.CheckOnClick = true;
                        tsi.Click += new EventHandler(this.SelectRGBDOSDrive_Click);
                        if (i == 0)
                        {
                            tsi.Checked = true;
                        }

                        pi.CurrentPartition = i;

                        diskformat = new OS9Format(pi);
                        tsi.Image = (System.Drawing.Image)resources.GetObject("OS9.image");

                        if (!diskformat.IsValidFormat)
                        {
                            diskformat = new DragonDosFormat(pi);
                            tsi.Image = (System.Drawing.Image)resources.GetObject("DragonDos.image");
                            if (!diskformat.IsValidFormat)
                            {
                                diskformat = new RSDOSFormat(pi);
                                tsi.Image = (System.Drawing.Image)resources.GetObject("RSDOS.image");
                            }
                        }

                        if (diskformat != null && diskformat.IsValidFormat && !string.IsNullOrEmpty(diskformat.DiskLabel))
                        {
                            tsi.Text = diskformat.DiskLabel;
                        }

                        this.selectRGBDOSDriveToolStripMenuItem.DropDownItems.Add(tsi);
                    }

                    pi.CurrentPartition = 0;
                    ((ToolStripMenuItem)this.selectRGBDOSDriveToolStripMenuItem.DropDownItems[0]).Checked = true;

                    this.selectRGBDOSDriveToolStripMenuItem.Enabled = true;
                    this.gotoRGBDOSDriveToolStripMenuItem.Enabled = true;

                    this.Cursor = cursor;
                }
                else
                {
                    this.selectRGBDOSDriveToolStripMenuItem.Enabled = false;
                    this.gotoRGBDOSDriveToolStripMenuItem.Enabled = false;
                }
            }

            diskformat = new OS9Format(diskimage);
            if (diskformat == null || !diskformat.IsValidFormat)
            {
                diskformat = new DragonDosFormat(diskimage);
                if (diskformat == null || !diskformat.IsValidFormat)
                {
                    diskformat = new RSDOSFormat(diskimage);
                }
            }

            if (!diskformat.IsValidFormat && !diskimage.IsPartitioned)
            {
                DialogResult dr = MessageBox.Show(resourceManager.GetString("MainForm_disknotformatted", cultureInfo), resourceManager.GetString("MainForm_formatnotrecognized", cultureInfo), MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    // FormatWizardForm form = new FormatWizardForm(false, true);
                    // form.Filename = diskimage.Filename;
                    // form.Tracks = diskimage.PhysicalTracks;
                    // form.Heads = diskimage.PhysicalHeads;
                    // form.Sectors = diskimage.PhysicalSectors;
                    // form.SectorSize = diskimage.PhysicalSectorSize;
                    // form.Partitions = diskimage.Partitions;
                    // form.RootPartitionSize = diskimage.ImagePartitionOffset;
                    // form.DiskImageType = diskimage.ImageType;
                    // dr = form.ShowDialog();
                    FormatForm form = new FormatForm();
                    form.Label1 = resourceManager.GetString("FormatForm_formatdisk", cultureInfo);
                    dr = form.ShowDialog();
                    if (dr == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        if (form.OS9)
                        {
                            diskformat = new OS9Format(diskimage);
                            diskformat.FormatDisk();
                        }
                        else if (form.RSDOS)
                        {
                            diskformat = new RSDOSFormat(diskimage);
                            diskformat.FormatDisk();
                        }
                        else if (form.DragonDos)
                        {
                            diskformat = new DragonDosFormat(diskimage);
                            diskformat.FormatDisk();
                        }
                    }
                }
            }
            else if (!diskformat.IsValidFormat && diskimage.IsPartitioned)
            {
                DialogResult dr = MessageBox.Show(resourceManager.GetString("MainForm_partitionnotformatted", cultureInfo), resourceManager.GetString("MainForm_formatnotrecognized", cultureInfo), MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    FormatForm form = new FormatForm();
                    form.Label1 = resourceManager.GetString("FormatForm_formatpartition", cultureInfo);
                    dr = form.ShowDialog();
                    if (dr == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        if (form.OS9)
                        {
                            diskformat = new OS9Format(diskimage);
                            diskformat.FormatDisk();
                        }
                        else if (form.RSDOS)
                        {
                            diskformat = new RSDOSFormat(diskimage);
                            diskformat.FormatDisk();
                        }
                        else if (form.DragonDos)
                        {
                            diskformat = new DragonDosFormat(diskimage);
                            diskformat.FormatDisk();
                        }
                    }
                }
            }

            DiskViewForm diskviewform = new DiskViewForm(diskformat);
            diskviewform.Text = string.Format("EMUDisk - {0}", diskimage.Filename);
            diskviewform.MdiParent = this;
            diskviewform.Activated += new EventHandler(this.DiskViewForm_Activated);
            diskviewform.Disposed += new EventHandler(this.DiskViewForm_Disposed);
            diskviewform.PartitionItems = this.selectRGBDOSDriveToolStripMenuItem;
            diskviewform.Show();

            this.mruManager.Add(filename);
        }

        #endregion

        #region Window Events

        /// <summary>
        /// Form's Load event handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Load event arguments</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.UpdateMenu();
        }

        /// <summary>
        /// Form's Drag Enter event handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Drag event arguments</param>
        private void MainForm_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Form's Drag Drop event handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Drag event arguments</param>
        private void MainForm_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (File.Exists(file))
                    {
                        this.OpenDiskView(file);
                    }
                }
            }
        }

        #endregion

        #region Menu Events

        #region File Menu Events

        /// <summary>
        /// Open File menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CoCo Virtual Disk Files (*.dsk;*.dmk;*.os9;*.vdk;*.vhd)|*.dsk;*.dmk;*.os9;*.vdk;*.vhd";

            DialogResult dr = ofd.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                string strFileName = ofd.FileName;
                this.OpenDiskView(strFileName);
            }

            this.UpdateMenu();
        }

        /// <summary>
        /// Open Drive A: menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void DriveAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PhysicalDisk disk = null;
            try
            {
                disk = new PhysicalDisk(0);
            }
            catch (DriveNotFoundException)
            {
                MessageBox.Show(string.Format(resourceManager.GetString("MainForm_DriveNotFoundError", cultureInfo), "A:"), resourceManager.GetString("MainForm_OpenDiskErrorCaption", cultureInfo), MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (disk != null)
                {
                    disk.Close();
                }

                return;
            }
            catch (DiskNotPresentException)
            {
                MessageBox.Show(string.Format(resourceManager.GetString("MainForm_DiskNotPresentError", cultureInfo), "A:"), resourceManager.GetString("MainForm_OpenDiskErrorCaption", cultureInfo), MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (disk != null)
                {
                    disk.Close();
                }

                return;
            }
            catch (DiskFormatException)
            {
                MessageBox.Show(resourceManager.GetString("PhysicalDisk_DiskFormatError", cultureInfo), resourceManager.GetString("MainForm_OpenDiskErrorCaption", cultureInfo), MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (disk != null)
                {
                    disk.Close();
                }

                return;
            }
            catch
            {
                MessageBox.Show(resourceManager.GetString("MainForm_OpenDiskError", cultureInfo), resourceManager.GetString("MainForm_OpenDiskErrorCaption", cultureInfo), MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (disk != null)
                {
                    disk.Close();
                }

                return;
            }
        }

        /// <summary>
        /// Open Drive B: menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void DriveBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PhysicalDisk disk = null;
            try
            {
                disk = new PhysicalDisk(1);
            }
            catch (DriveNotFoundException)
            {
                MessageBox.Show(string.Format(resourceManager.GetString("MainForm_DriveNotFoundError", cultureInfo), "B:"), resourceManager.GetString("MainForm_OpenDiskErrorCaption", cultureInfo), MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (disk != null)
                {
                    disk.Close();
                }

                return;
            }
            catch (DiskNotPresentException)
            {
                MessageBox.Show(string.Format(resourceManager.GetString("MainForm_DiskNotPresentError", cultureInfo), "B:"), resourceManager.GetString("MainForm_OpenDiskErrorCaption", cultureInfo), MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (disk != null)
                {
                    disk.Close();
                }

                return;
            }
            catch (DiskFormatException)
            {
                MessageBox.Show(resourceManager.GetString("PhysicalDisk_DiskFormatError", cultureInfo), resourceManager.GetString("MainForm_OpenDiskErrorCaption", cultureInfo), MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (disk != null)
                {
                    disk.Close();
                }

                return;
            }
            catch
            {
                MessageBox.Show(resourceManager.GetString("MainForm_OpenDiskError", cultureInfo), resourceManager.GetString("MainForm_OpenDiskErrorCaption", cultureInfo), MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (disk != null)
                {
                    disk.Close();
                }

                return;
            }
        }

        /// <summary>
        /// Create New File menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Close menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                this.ActiveMdiChild.Close();
            }

            this.UpdateMenu();
        }

        /// <summary>
        /// Exit Application menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length != 0)
            {
                foreach (Form form in this.MdiChildren)
                {
                    form.Close();
                    form.Dispose();
                }
            }

            Environment.Exit(0);
        }

        #endregion

        #region Disk Menu Events

        /// <summary>
        /// Disk Information menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Format Disk menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void FormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Re-label Disk menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void ReLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Sector Editor menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void SectorEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Go to specific partition menu handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void GotoRGBDOSDriveToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Go to selected partition menu handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void SelectRGBDOSDrive_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsi = (ToolStripMenuItem)sender;

            foreach (ToolStripMenuItem item in this.selectRGBDOSDriveToolStripMenuItem.DropDownItems)
            {
                if (item.Name != tsi.Name)
                {
                    if (item.Checked)
                    {
                        item.Checked = false;
                    }
                }
            }

            DiskViewForm form = (DiskViewForm)this.ActiveMdiChild;
            int partition = int.Parse(tsi.Name);
            PartitionedVHDImage diskimage = (PartitionedVHDImage)form.DiskFormat.DiskImage;
            diskimage.CurrentPartition = partition;

            IDiskFormat diskformat = new OS9Format(diskimage);
            if (diskformat != null && !diskformat.IsValidFormat)
            {
                diskformat = new DragonDosFormat(diskimage);
                if (diskformat != null && !diskformat.IsValidFormat)
                {
                    diskformat = new RSDOSFormat(diskimage);
                }
            }

            if (diskformat == null || !diskformat.IsValidFormat)
            {
                // Disk is blank, format?
            }

            if (diskformat != null && diskformat.IsValidFormat)
            {
                form.DiskFormat = diskformat;
            }
        }

        /// <summary>
        /// Bootstrap menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void BootstrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        #endregion

        #region Window Menu Events

        /// <summary>
        /// Close Window menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void CloseWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CloseToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        /// <summary>
        /// Cascade Windows menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        /// <summary>
        /// Tile Windows Horizontally menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        /// <summary>
        /// Tile Windows Vertically menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        #endregion

        #region Language Menu Events

        /// <summary>
        /// Language menu item handler
        /// </summary>
        /// <param name="sender">Selected Language object</param>
        /// <param name="e">Event Arguments</param>
        private void LanguageChange_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem selectedLanguage = (ToolStripMenuItem)sender;
            this.SetLanguage(selectedLanguage.Tag.ToString());
        }

        #endregion

        #region Options Menu Events

        /// <summary>
        /// Options menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        #endregion

        #region Help Menu Events

        /// <summary>
        /// About menu item handler
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets new language
        /// </summary>
        /// <param name="language">Specified language to change to</param>
        private void SetLanguage(string language)
        {
            foreach (ToolStripMenuItem item in this.languageToolStripMenuItem.DropDownItems)
            {
                if (item.Tag.ToString() != language)
                {
                    item.Checked = false;
                }
                else
                {
                    item.Checked = true;
                }
            }

            cultureInfo = CultureInfo.CreateSpecificCulture(language);

            foreach (ToolStripItem item in this.menuStrip1.Items)
            {
                if (item is ToolStripMenuItem)
                {
                    this.SetMenuText((ToolStripMenuItem)item);
                }
            }
        }

        /// <summary>
        /// Update the menus
        /// </summary>
        private void UpdateMenu()
        {
            if (this.MdiChildren.Length == 0)
            {
                this.closeToolStripMenuItem.Enabled = false;
                this.diskToolStripMenuItem.Enabled = false;
                this.windowToolStripMenuItem.Enabled = false;
            }
            else
            {
                this.closeToolStripMenuItem.Enabled = true;
                this.diskToolStripMenuItem.Enabled = true;
                this.windowToolStripMenuItem.Enabled = true;
                this.bootstrapToolStripMenuItem.Enabled = false;
                DiskViewForm form = (DiskViewForm)this.ActiveMdiChild;
                if (form.DiskFormat.DiskFormat == DiskFormatType.OS9Format)
                {
                    OS9Format diskformat = (OS9Format)form.DiskFormat;
                    if (diskformat.Lsn0.BootStrap != 0)
                    {
                        this.bootstrapToolStripMenuItem.Enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Update menu text when language has been changed
        /// </summary>
        /// <param name="item">Menu item to update</param>
        private void SetMenuText(ToolStripMenuItem item)
        {
            string localizedText;
            try
            {
                localizedText = resourceManager.GetString("MainForm_" + item.Name, cultureInfo);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    item.Text = localizedText;
                }
            }
            catch
            {
            }

            if (item.DropDownItems.Count != 0)
            {
                foreach (ToolStripItem subitem in item.DropDownItems)
                {
                    if (subitem is ToolStripMenuItem)
                    {
                        this.SetMenuText((ToolStripMenuItem)subitem);
                    }
                }
            }
        }

        /// <summary>
        /// Cleanup after disposing an instance of the DiskViewForm
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void DiskViewForm_Disposed(object sender, EventArgs e)
        {
            this.UpdateMenu();
        }

        /// <summary>
        /// Update menu when a DiskViewForm is activated
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event Arguments</param>
        private void DiskViewForm_Activated(object sender, EventArgs e)
        {
            DiskViewForm form = (DiskViewForm)this.ActiveMdiChild;
            IDiskImage di = form.DiskFormat.DiskImage;
            if (di.IsPartitioned)
            {
                this.selectRGBDOSDriveToolStripMenuItem = form.PartitionItems;

                this.selectRGBDOSDriveToolStripMenuItem.Enabled = true;
                this.gotoRGBDOSDriveToolStripMenuItem.Enabled = true;
            }
            else
            {
                this.selectRGBDOSDriveToolStripMenuItem.Enabled = false;
                this.gotoRGBDOSDriveToolStripMenuItem.Enabled = false;
            }

            this.UpdateMenu();
        }

        #endregion
    }
}
