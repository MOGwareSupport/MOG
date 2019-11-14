using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG.PROMPT;
using MOG.PROJECT;
using MOG.PLATFORM;

namespace MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for PlatformEditor.
	/// </summary>
	public class MOGPlatformEditor : System.Windows.Forms.UserControl
	{
		#region System definitions

		private System.Windows.Forms.Button btnAddPlatform;
		private System.Windows.Forms.TextBox tbNewPlatformName;
		private System.Windows.Forms.Label lblNewPlatformName;
		private System.Windows.Forms.Label lblPlatforms;
		private System.Windows.Forms.Label lblExamplePlatforms;
		private System.Windows.Forms.ListView lvExamplePlatforms;
		private System.Windows.Forms.ContextMenuStrip examplePlatformsContextMenu;
		private System.Windows.Forms.ToolStripMenuItem miAddPlatform;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.OpenFileDialog openImageFileDialog;
		private PlatformListView lvPlatforms;
		private System.Windows.Forms.Button btnRemove;
		private System.ComponentModel.IContainer components;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MOGPlatformEditor));
            this.lblNewPlatformName = new System.Windows.Forms.Label();
            this.lblPlatforms = new System.Windows.Forms.Label();
            this.lblExamplePlatforms = new System.Windows.Forms.Label();
            this.lvExamplePlatforms = new System.Windows.Forms.ListView();
            this.examplePlatformsContextMenu = new System.Windows.Forms.ContextMenuStrip();
            this.miAddPlatform = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.btnAddPlatform = new System.Windows.Forms.Button();
            this.tbNewPlatformName = new System.Windows.Forms.TextBox();
            this.openImageFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lvPlatforms = new PlatformListView();
            this.SuspendLayout();
            // 
            // lblNewPlatformName
            // 
            this.lblNewPlatformName.Location = new System.Drawing.Point(16, 16);
            this.lblNewPlatformName.Name = "lblNewPlatformName";
            this.lblNewPlatformName.Size = new System.Drawing.Size(152, 16);
            this.lblNewPlatformName.TabIndex = 20;
            this.lblNewPlatformName.Text = "New Platform Name";
            // 
            // lblPlatforms
            // 
            this.lblPlatforms.Location = new System.Drawing.Point(296, 16);
            this.lblPlatforms.Name = "lblPlatforms";
            this.lblPlatforms.Size = new System.Drawing.Size(104, 16);
            this.lblPlatforms.TabIndex = 19;
            this.lblPlatforms.Text = "Project Platforms";
            // 
            // lblExamplePlatforms
            // 
            this.lblExamplePlatforms.Location = new System.Drawing.Point(16, 64);
            this.lblExamplePlatforms.Name = "lblExamplePlatforms";
            this.lblExamplePlatforms.Size = new System.Drawing.Size(152, 16);
            this.lblExamplePlatforms.TabIndex = 18;
            this.lblExamplePlatforms.Text = "Example Platforms";
            // 
            // lvExamplePlatforms
            // 
            this.lvExamplePlatforms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lvExamplePlatforms.ContextMenuStrip = this.examplePlatformsContextMenu;
            this.lvExamplePlatforms.FullRowSelect = true;
            this.lvExamplePlatforms.HideSelection = false;
            this.lvExamplePlatforms.LargeImageList = this.imageList;
            this.lvExamplePlatforms.Location = new System.Drawing.Point(16, 80);
            this.lvExamplePlatforms.MultiSelect = false;
            this.lvExamplePlatforms.Name = "lvExamplePlatforms";
            this.lvExamplePlatforms.Size = new System.Drawing.Size(176, 176);
            this.lvExamplePlatforms.SmallImageList = this.imageList;
            this.lvExamplePlatforms.TabIndex = 17;
            this.lvExamplePlatforms.UseCompatibleStateImageBehavior = false;
            this.lvExamplePlatforms.View = System.Windows.Forms.View.List;
            this.lvExamplePlatforms.DoubleClick += new System.EventHandler(this.lvExamplePlatforms_DoubleClick);
            this.lvExamplePlatforms.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvExamplePlatforms_MouseUp);
            this.lvExamplePlatforms.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvExamplePlatforms_KeyUp);
            // 
            // examplePlatformsContextMenu
            // 
            this.examplePlatformsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.miAddPlatform});
            this.examplePlatformsContextMenu.Opening += this.examplePlatformsContextMenu_Popup;
            // 
            // miAddPlatform
            // 
            this.miAddPlatform.Text = "Add";
            this.miAddPlatform.Click += new System.EventHandler(this.miAddPlatform_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "");
            this.imageList.Images.SetKeyName(1, "");
            this.imageList.Images.SetKeyName(2, "");
            this.imageList.Images.SetKeyName(3, "");
            this.imageList.Images.SetKeyName(4, "");
            // 
            // btnAddPlatform
            // 
            this.btnAddPlatform.Enabled = false;
            this.btnAddPlatform.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAddPlatform.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddPlatform.Location = new System.Drawing.Point(208, 32);
            this.btnAddPlatform.Name = "btnAddPlatform";
            this.btnAddPlatform.Size = new System.Drawing.Size(75, 23);
            this.btnAddPlatform.TabIndex = 14;
            this.btnAddPlatform.Text = "&Add >";
            this.btnAddPlatform.Click += new System.EventHandler(this.btnAddPlatform_Click);
            // 
            // tbNewPlatformName
            // 
            this.tbNewPlatformName.Location = new System.Drawing.Point(16, 32);
            this.tbNewPlatformName.Name = "tbNewPlatformName";
            this.tbNewPlatformName.Size = new System.Drawing.Size(176, 20);
            this.tbNewPlatformName.TabIndex = 16;
            this.tbNewPlatformName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbNewPlatformName_KeyUp);
            this.tbNewPlatformName.TextChanged += new System.EventHandler(this.tbNewPlatformName_TextChanged);
            // 
            // openImageFileDialog
            // 
            this.openImageFileDialog.Filter = "All files|*.*";
            this.openImageFileDialog.FilterIndex = 2;
            this.openImageFileDialog.InitialDirectory = "C:\\";
            this.openImageFileDialog.RestoreDirectory = true;
            this.openImageFileDialog.Title = "Select Image File";
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemove.Location = new System.Drawing.Point(208, 80);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 23;
            this.btnRemove.Text = "&Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lvPlatforms
            // 
            this.lvPlatforms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvPlatforms.Location = new System.Drawing.Point(296, 32);
            this.lvPlatforms.Name = "lvPlatforms";
            this.lvPlatforms.Size = new System.Drawing.Size(176, 224);
            this.lvPlatforms.TabIndex = 22;
            this.lvPlatforms.UseCompatibleStateImageBehavior = false;
            this.lvPlatforms.View = System.Windows.Forms.View.List;
            this.lvPlatforms.SelectedIndexChanged += new System.EventHandler(this.lvPlatforms_SelectedIndexChanged);
            // 
            // MOGPlatformEditor
            // 
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.lvPlatforms);
            this.Controls.Add(this.lblNewPlatformName);
            this.Controls.Add(this.lblPlatforms);
            this.Controls.Add(this.lblExamplePlatforms);
            this.Controls.Add(this.lvExamplePlatforms);
            this.Controls.Add(this.btnAddPlatform);
            this.Controls.Add(this.tbNewPlatformName);
            this.Name = "MOGPlatformEditor";
            this.Size = new System.Drawing.Size(480, 264);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
		#endregion
		#region User definitions

		private string iconLocation;

		private const int BLANK_INDEX		= 0;
		private const int XBOX_INDEX		= 1;
		private const int PS2_INDEX			= 2;
		private const int PC_INDEX			= 3;
		private const int GAMECUBE_INDEX	= 4;

		private ArrayList iconFilenames;

		#endregion
		#region Properties
		public ArrayList MOG_Platforms
		{
			get
			{
				ArrayList platforms = new ArrayList();
				
				foreach (ListViewItem item in this.lvPlatforms.Items)
				{
					MOG_Platform plat = new MOG_Platform();
					plat.mPlatformName = item.Text;
					platforms.Add(plat);
				}

				return platforms;
			}
		}

		public ArrayList PlatformNames
		{
			get
			{
				return this.lvPlatforms.PlatformNames;
			}
		}

		[Category("PlatformEditor")]
		public string IconLocation
		{
			get { return this.iconLocation; }
			set 
			{ 
				this.iconLocation = value;

				// load up any existing bitmaps from the new icon location
				ImageList newImageList = new ImageList();
				int i=0;
				foreach (string iconName in this.iconFilenames)
				{
					if (File.Exists(this.iconLocation + "\\" + iconName))
						newImageList.Images.Add( Image.FromFile(this.iconLocation + "\\" + iconName) );
					else
						newImageList.Images.Add( this.imageList.Images[i] );

					++i;
				}

				this.imageList = newImageList;
			}
		}

		#endregion
		#region Constructors
		public MOGPlatformEditor()
		{
			InitializeComponent();

			this.iconFilenames = new ArrayList( new string[] { "blank.bmp", "xbox.bmp", "ps2.bmp", "pc.bmp", "gamecube.bmp" } );
		}

		public void LoadDefaults()
		{
			this.lvExamplePlatforms.Items.Clear();
			this.lvExamplePlatforms.LargeImageList = this.lvPlatforms.ImageList;
			this.lvExamplePlatforms.SmallImageList = this.lvPlatforms.ImageList;

			ArrayList platNames = this.lvPlatforms.GetDefaultPlatformNames();
			ArrayList platIndexes = this.lvPlatforms.GetDefaultPlatformImageIndexes();

			for (int i = 0; i < platNames.Count; i++)
			{
				ListViewItem item = new ListViewItem(platNames[i] as string);
				item.ImageIndex = (int)platIndexes[i];
				item.StateImageIndex = (int)platIndexes[i];
				this.lvExamplePlatforms.Items.Add(item);
			}
		}

		#endregion
		#region Accessors
		public bool ContainsPlatform(string platName)
		{
			foreach (ListViewItem item in this.lvPlatforms.Items)
			{
				if (item.Text.ToLower() == platName.ToLower())
				{
					return true;
				}
			}

			return false;
		}
		#endregion
		#region Member functions

		public void LoadFromProject(MOG_Project proj)
		{
			lvPlatforms.Clear();

			// Loop and add 'em
			foreach (MOG_Platform platform in proj.GetPlatforms())
			{
				AddPlatform(platform);
			}
		}

		private int IconIndex(string platName)
		{
			// If we've added the image for platName yet (filename = platName.bmp)
			if (this.iconFilenames.Contains(platName.ToLower() + ".bmp"))
			{
				// It's already loaded, just return the index
				return this.iconFilenames.IndexOf(platName.ToLower() + ".bmp");
			}

			// Icon file for platName isn't loaded yet, so look for it
			string platformIconFilename = this.iconLocation + "\\" + platName + ".bmp";
			if (File.Exists( platformIconFilename ))
			{
				// File exists, so add it to the list and return imageList.Count-1 as the index
				this.imageList.Images.Add( Image.FromFile(platformIconFilename) );
				return this.imageList.Images.Count-1;
			}
			else 
			{
				// the file doesn't exist, so return the default icon (index BLANK_INDEX)
				return BLANK_INDEX;
			}
		}

		public void CopyPlatformBitmaps()
		{
			CopyPlatformBitmaps(this.iconLocation);
		}

		public void CopyPlatformBitmaps(string location)
		{
			if (location == null)
			{
				return;
			}

			// Get rid of trailing backslash and make sure it exists
			location = location.TrimEnd("\\".ToCharArray());
			if (!Directory.Exists(location))
			{
				return;
			}

			// Loop through the platforms and copy the bitmaps
			foreach (ListViewItem item in this.lvPlatforms.Items)
			{
				Image image = this.imageList.Images[ item.ImageIndex ];
				string filename = location + "\\" + item.Text + ".bmp";
				
				try
				{
					// Copy over existing files
					if (File.Exists(filename))
					{
						File.Delete(filename);
					}

					// Save in BMP format
					image.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
					image = null;
				}
				catch
				{
					// Skip this one if it can't be deleted
					continue;
				}
			}
		}

		public bool ConfigurationValid()
		{
			// Just make sure we've got at least one platform
			return (this.lvPlatforms.Items.Count > 0);
		}

		public bool IsValid(bool showMessages)
		{
			// Check for more than one platform
			if (this.lvPlatforms.Items.Count <= 0)
			{
				if (showMessages)
				{
					MOG_Prompt.PromptMessage("No Platforms", "You must add at least one platform");
					this.tbNewPlatformName.Focus();
				}
				
				return false;
			}

			return true;
		}

		public void FocusNewPlatformTextBox()
		{
			this.tbNewPlatformName.Focus();
		}

		public void Clear() 
		{
			// Clear the entire control
			this.lvPlatforms.Items.Clear();
			this.lvExamplePlatforms.Items.Clear();
			this.tbNewPlatformName.Text = "";
		}

		public void AddPlatform(MOG_Platform platform)
		{
			// Grab the name
			AddPlatform(platform.mPlatformName);
		}
		
		public void AddPlatform(string platformName)
		{
			// Try to add as a default platform (i.e., one such as XBox and PC that has a built-in icon) first
			if (!this.lvPlatforms.AddDefaultPlatform(platformName))
			{
				this.lvPlatforms.AddPlatform(platformName);
			}
		}
		#endregion
		#region Event handlers
		private void btnAddPlatform_Click(object sender, System.EventArgs e)
		{
			if (this.tbNewPlatformName.Text == null  ||  this.tbNewPlatformName.Text == "")
			{
				MOG_Prompt.PromptMessage("Missing Data", "Please enter a platform name");
				return;
			}

			if (lvPlatforms.FindPlatform(tbNewPlatformName.Text) == null)
			{
				if (!this.lvPlatforms.AddDefaultPlatform(this.tbNewPlatformName.Text))
				{
                    this.lvPlatforms.AddPlatform(this.tbNewPlatformName.Text);
				}
			}
		}

		private void lvExamplePlatforms_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (this.lvExamplePlatforms.SelectedItems.Count > 0)
				this.tbNewPlatformName.Text = this.lvExamplePlatforms.SelectedItems[0].Text;
		}

		private void lvExamplePlatforms_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter  ||  e.KeyData == Keys.A)
			{
				foreach (ListViewItem item in this.lvExamplePlatforms.SelectedItems)
				{
					this.tbNewPlatformName.Text = item.Text;
					this.btnAddPlatform.PerformClick();
				}
			}
		}

		private void lvExamplePlatforms_DoubleClick(object sender, System.EventArgs e)
		{
			if (this.lvExamplePlatforms.SelectedItems.Count > 0)
			{
				this.tbNewPlatformName.Text = this.lvExamplePlatforms.SelectedItems[0].Text;
			}

			this.btnAddPlatform.PerformClick();
		}

		private void tbNewPlatformName_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				this.btnAddPlatform.PerformClick();
			}
		}

		private void examplePlatformsContextMenu_Popup(object sender, CancelEventArgs e)
		{
			if (this.lvExamplePlatforms.SelectedItems.Count > 0)
			{
				this.miAddPlatform.Enabled = true;
			}
			else
			{
				this.miAddPlatform.Enabled = false;
			}
		}

		private void miAddPlatform_Click(object sender, System.EventArgs e)
		{
			foreach (ListViewItem item in this.lvExamplePlatforms.SelectedItems)
			{
				this.tbNewPlatformName.Text = item.Text;
				this.btnAddPlatform.PerformClick();
			}
		}

		private void tbNewPlatformName_TextChanged(object sender, System.EventArgs e)
		{
			if (this.tbNewPlatformName.Text == "")
			{
				this.btnAddPlatform.Enabled = false;
			}
			else
			{
				this.btnAddPlatform.Enabled = true;
			}
		}

		private void btnIconBrowse_Click(object sender, System.EventArgs e)
		{
			//ChangeIcon();
		}

		private void miIconBrowse_Click(object sender, System.EventArgs e)
		{
			//ChangeIcon();
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			this.lvPlatforms.RemoveSelected();
		}

		private void lvPlatforms_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.lvPlatforms.SelectedItems.Count > 0)
			{
				this.btnRemove.Enabled = true;
			}
			else
			{
				this.btnRemove.Enabled = false;
			}
		}
		#endregion
	}
}
