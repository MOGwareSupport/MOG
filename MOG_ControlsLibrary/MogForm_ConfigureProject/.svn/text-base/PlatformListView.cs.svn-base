using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG.PLATFORM;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERPROJECT;

namespace MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for PlatformListView.
	/// </summary>
	public class PlatformListView : System.Windows.Forms.ListView
	{
		#region System defs

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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlatformListView));
			this.cmPlatforms = new System.Windows.Forms.ContextMenu();
			this.miNewPlatform = new System.Windows.Forms.MenuItem();
			this.miDefaultPlatforms = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.miIcon = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.miDelete = new System.Windows.Forms.MenuItem();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// cmPlatforms
			// 
			this.cmPlatforms.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miNewPlatform,
            this.miDefaultPlatforms,
            this.menuItem3,
            this.miIcon,
            this.menuItem1,
            this.miDelete});
			this.cmPlatforms.Popup += new System.EventHandler(this.cmPlatforms_Popup);
			// 
			// miNewPlatform
			// 
			this.miNewPlatform.Index = 0;
			this.miNewPlatform.Text = "&New Platform...";
			this.miNewPlatform.Click += new System.EventHandler(this.miNewPlatform_Click);
			// 
			// miDefaultPlatforms
			// 
			this.miDefaultPlatforms.Index = 1;
			this.miDefaultPlatforms.Text = "De&fault Platforms";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 2;
			this.menuItem3.Text = "-";
			// 
			// miIcon
			// 
			this.miIcon.Index = 3;
			this.miIcon.Text = "&Icon";
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 4;
			this.menuItem1.Text = "-";
			// 
			// miDelete
			// 
			this.miDelete.Index = 5;
			this.miDelete.Text = "&Delete";
			this.miDelete.Click += new System.EventHandler(this.miDelete_Click);
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
			this.imageList.Images.SetKeyName(5, "");
			this.imageList.Images.SetKeyName(6, "wii.bmp");
			this.imageList.Images.SetKeyName(7, "");
			this.imageList.Images.SetKeyName(8, "");
			// 
			// PlatformListView
			// 
			this.Size = new System.Drawing.Size(384, 334);
			this.ResumeLayout(false);

		}
		#endregion
		
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.MenuItem miIcon;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem miDefaultPlatforms;
		private System.Windows.Forms.MenuItem miNewPlatform;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem miDelete;
		private System.Windows.Forms.ContextMenu cmPlatforms;

		#endregion
		#region Member vars
		private ArrayList imgList = new ArrayList();
		private IconMenuItem newIconItem;
		#endregion
		#region Properties
		public ArrayList MOG_Platforms
		{
			get 
			{
				ArrayList platforms = new ArrayList();

				foreach (ListViewItem item in this.Items)
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
				ArrayList names = new ArrayList();

				foreach (ListViewItem item in this.Items)
					names.Add( item.Text );

				return names;
			}
		}

		public ImageList ImageList
		{
			get { return this.imageList; }
		}
		#endregion
		#region Constants
		private const int DEFAULT_INDEX		= 1;
		private const int XBOX_INDEX		= 0;
		private const int PC_INDEX			= 1;
		private const int PS2_INDEX			= 2;
		private const int GAMECUBE_INDEX	= 3;
		private const int XBOX360_INDEX		= 4;
		private const int PS3_INDEX			= 5;
		private const int REVOLUTION_INDEX	= 6;
		private const int PSP_INDEX			= 7;
		private const int DS_INDEX	= 8;
		#endregion
		#region Enums
		public enum DefaultPlatform { XBOX, XBOX360, PC, PS2, PS3, PSP, GAMECUBE, REVOLUTION, DS };
		#endregion
		#region Constructor
		public PlatformListView()
		{
			InitializeComponent();

			foreach (Image img in this.imageList.Images)
				this.imgList.Add(img);

			this.miDefaultPlatforms.MenuItems.Add( new IconMenuItem("PC", (Image)this.imgList[PC_INDEX], new EventHandler(defaultPlatform_Click)) );
			this.miDefaultPlatforms.MenuItems.Add( new IconMenuItem("XBox", (Image)this.imgList[XBOX_INDEX], new EventHandler(defaultPlatform_Click)) );
			this.miDefaultPlatforms.MenuItems.Add( new IconMenuItem("XBox360", (Image)this.imgList[XBOX360_INDEX], new EventHandler(defaultPlatform_Click)) );
			this.miDefaultPlatforms.MenuItems.Add( new IconMenuItem("PS2", (Image)this.imgList[PS2_INDEX], new EventHandler(defaultPlatform_Click)) );
			this.miDefaultPlatforms.MenuItems.Add( new IconMenuItem("PS3", (Image)this.imgList[PS3_INDEX], new EventHandler(defaultPlatform_Click)) );
			this.miDefaultPlatforms.MenuItems.Add( new IconMenuItem("PSP", (Image)this.imgList[PSP_INDEX], new EventHandler(defaultPlatform_Click)) );
			this.miDefaultPlatforms.MenuItems.Add( new IconMenuItem("GameCube", (Image)this.imgList[GAMECUBE_INDEX], new EventHandler(defaultPlatform_Click)) );
			this.miDefaultPlatforms.MenuItems.Add( new IconMenuItem("wii", (Image)this.imgList[REVOLUTION_INDEX], new EventHandler(defaultPlatform_Click)) );
			this.miDefaultPlatforms.MenuItems.Add( new IconMenuItem("DS", (Image)this.imgList[DS_INDEX], new EventHandler(defaultPlatform_Click)) );

			RebuildIconSubmenu();

			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_KeyUp);
			
			this.ContextMenu = this.cmPlatforms;
			this.View = View.List;
			this.LargeImageList = this.imageList;
			this.SmallImageList = this.imageList;
		}
		#endregion
		#region Public functions
		public void RemoveSelected()
		{
			foreach (ListViewItem item in this.SelectedItems)
			{
				// remove from database
				MOG_ControllerProject.GetProject().PlatformRemove(item.Text);

				item.Remove();
			}
		}

		public ArrayList GetDefaultPlatformNames()
		{
			ArrayList names = new ArrayList();
			
			foreach (MenuItem platformItem in this.miDefaultPlatforms.MenuItems)
				names.Add(platformItem.Text);
			
			return names;
		}

		public ArrayList GetDefaultPlatformImageIndexes()
		{
			ArrayList indexes = new ArrayList();
			
			foreach (MenuItem platformItem in this.miDefaultPlatforms.MenuItems)
				indexes.Add( this.imgList.IndexOf(((IconMenuItem)platformItem).Image) );
			
			return indexes;
		}

		public bool AddPlatform(string platformName)
		{
			if (platformName == "")
			{
				// add a new blank platform

				ListViewItem newItem = new ListViewItem("NewPlatform");
				this.Items.Add(newItem);
				newItem.ImageIndex = DEFAULT_INDEX;
				newItem.StateImageIndex = DEFAULT_INDEX;

				MOG_Platform plat = new MOG_Platform("NewPlatform");
				MOG_ControllerProject.GetProject().PlatformAdd(plat);
				CopyIcon("platform");
				
				newItem.BeginEdit();
				return true;
			}
			else
			{
				MOG_Platform plat = new MOG_Platform(platformName);
                if (MOG_ControllerProject.GetProject().PlatformAdd(plat))
                {
                    CopyIcon(platformName);

                    ListViewItem newItem = new ListViewItem(platformName);
                    this.Items.Add(newItem);
                    newItem.ImageIndex = DEFAULT_INDEX;
                    newItem.StateImageIndex = DEFAULT_INDEX;
                    return true;
                }

                return false;
			}
		}


		public bool AddDefaultPlatform(string platform)
		{
			if (platform.ToLower() == "xbox")
				return AddDefaultPlatform(DefaultPlatform.XBOX);
			else if (platform.ToLower() == "pc")
				return AddDefaultPlatform(DefaultPlatform.PC);
			else if (platform.ToLower() == "ps2")
				return AddDefaultPlatform(DefaultPlatform.PS2);
			else if (platform.ToLower() == "gamecube")
				return AddDefaultPlatform(DefaultPlatform.GAMECUBE);
			else if (platform.ToLower() == "xbox360")
				return AddDefaultPlatform(DefaultPlatform.XBOX360);
			else if (platform.ToLower() == "ps3")
				return AddDefaultPlatform(DefaultPlatform.PS3);
			else if (platform.ToLower() == "wii")
				return AddDefaultPlatform(DefaultPlatform.REVOLUTION);
			else if (platform.ToLower() == "ds")
				return AddDefaultPlatform(DefaultPlatform.DS);
			else if (platform.ToLower() == "psp")
				return AddDefaultPlatform(DefaultPlatform.PSP);

			return false;
		}
			
		public bool AddDefaultPlatform(DefaultPlatform platform)
		{
			ListViewItem newItem = null;
			switch (platform)
			{
				case DefaultPlatform.XBOX:
					if (!this.ListViewContainsItem(this, "xbox"))
					{
						newItem = new ListViewItem("XBox");
						newItem.ImageIndex = XBOX_INDEX;
						newItem.StateImageIndex = XBOX_INDEX;
					}
					break;
				case DefaultPlatform.PC:
					if (!this.ListViewContainsItem(this, "pc"))
					{
						newItem = new ListViewItem("PC");
						newItem.ImageIndex = PC_INDEX;
						newItem.StateImageIndex = PC_INDEX;
					}
					break;
				case DefaultPlatform.PS2:
					if (!this.ListViewContainsItem(this, "ps2"))
					{
						newItem = new ListViewItem("PS2");
						newItem.ImageIndex = PS2_INDEX;
						newItem.StateImageIndex = PS2_INDEX;
					}
					break;
				case DefaultPlatform.GAMECUBE:
					if (!this.ListViewContainsItem(this, "gamecube"))
					{
						newItem = new ListViewItem("GameCube");
						newItem.ImageIndex = GAMECUBE_INDEX;
						newItem.StateImageIndex = GAMECUBE_INDEX;
					}
					break;
				case DefaultPlatform.XBOX360:
					if (!this.ListViewContainsItem(this, "xbox360"))
					{
						newItem = new ListViewItem("XBox360");
						newItem.ImageIndex = XBOX360_INDEX;
						newItem.StateImageIndex = XBOX360_INDEX;
					}
					break;
				case DefaultPlatform.PS3:
					if (!this.ListViewContainsItem(this, "ps3"))
					{
						newItem = new ListViewItem("PS3");
						newItem.ImageIndex = PS3_INDEX;
						newItem.StateImageIndex = PS3_INDEX;
					}
					break;
				case DefaultPlatform.REVOLUTION:
					if (!this.ListViewContainsItem(this, "wii"))
					{
						newItem = new ListViewItem("wii");
						newItem.ImageIndex = REVOLUTION_INDEX;
						newItem.StateImageIndex = REVOLUTION_INDEX;
					}
					break;
				case DefaultPlatform.PSP:
					if (!this.ListViewContainsItem(this, "psp"))
					{
						newItem = new ListViewItem("PSP");
						newItem.ImageIndex = PSP_INDEX;
						newItem.StateImageIndex = PSP_INDEX;
					}
					break;
				case DefaultPlatform.DS:
					if (!this.ListViewContainsItem(this, "ds"))
					{
						newItem = new ListViewItem("DS");
						newItem.ImageIndex = DS_INDEX;
						newItem.StateImageIndex = DS_INDEX;
					}
					break;
			}

			if (newItem != null)
			{
				this.Items.Add(newItem);

				// add to DB
				MOG_Platform plat = new MOG_Platform(newItem.Text);
				MOG_ControllerProject.GetProject().PlatformAdd(plat);
				CopyIcon(plat.mPlatformName);

				return true;
			}


			return false;
		}

		public ListViewItem FindPlatform(string name)
		{
			foreach (ListViewItem item in Items)
			{
				if (String.Compare(name, item.Text, true) == 0)
				{
					return item;
				}
			}

			return null;
		}
		#endregion
		#region Private functions
		private void CopyIcon(string platformName)
		{
			string iconSourcePath = MOG.CONTROLLER.CONTROLLERSYSTEM.MOG_ControllerSystem.LocateInstallItem("SystemImages\\Platforms");
			string iconDestPath = MOG_ControllerProject.GetProject().GetProjectToolsPath() + "\\Images\\Platforms";

			if (!Directory.Exists(iconSourcePath))
				return;

			string sourceFilename = iconSourcePath + "\\" + platformName + ".bmp";
			string destFilename = iconDestPath + "\\" + platformName + ".bmp";
			if (!File.Exists(sourceFilename))
			{
				sourceFilename = iconSourcePath + "\\platform.bmp";
				destFilename = iconDestPath + "\\platform.bmp";
			}

			if (!Directory.Exists(iconDestPath))
				Directory.CreateDirectory(iconDestPath);

			if (File.Exists(sourceFilename)  &&  !File.Exists(destFilename))
				File.Copy(sourceFilename, destFilename, true);

		}

		private bool ListViewContainsItem(ListView listView, string text)
		{
			if (listView == null  ||  text == null)
				return false;

			foreach (ListViewItem item in listView.Items)
			{
				if (item.Text.ToLower() == text.ToLower())
					return true;
			}

			return false;
		}

		private void RebuildIconSubmenu()
		{
			this.miIcon.MenuItems.Clear();

			foreach (Image img in this.imgList)
			{
				IconMenuItem imi = new IconMenuItem("", img);
				imi.Click += new EventHandler(iconChange_Click);
				this.miIcon.MenuItems.Add( imi );
			}

			this.newIconItem = new IconMenuItem("New...", null, new EventHandler(iconChange_Click));
			this.miIcon.MenuItems.Add( this.newIconItem );
		}
		#endregion
		#region Event handlers
		private void miNewPlatform_Click(object sender, System.EventArgs e)
		{
			AddPlatform("");
		}

		private void defaultPlatform_Click(object sender, System.EventArgs e)
		{
			IconMenuItem mItem = sender as IconMenuItem;

			if (mItem != null  )
			{
				if (!ListViewContainsItem(this, mItem.Text))
				{
					AddDefaultPlatform(mItem.Text);
				}
				else
				{
					ListViewItem selItem = FindPlatform(mItem.Text);
					if (selItem != null)
					{
						this.SelectedItems.Clear();
						selItem.Selected = true;
					}
				}
			}
		}

		private void cmPlatforms_Popup(object sender, System.EventArgs e)
		{
			if (this.SelectedItems.Count > 0)
			{
				this.miIcon.Enabled = true;
				this.miDefaultPlatforms.Enabled = false;
				this.miNewPlatform.Enabled = false;
				this.miDelete.Enabled = true;
			}
			else
			{
				this.miIcon.Enabled = false;
				this.miDefaultPlatforms.Enabled = true;
				this.miNewPlatform.Enabled = true;
				this.miDelete.Enabled = false;
			}
		}

		private void iconChange_Click(object sender, EventArgs e)
		{
			if ( !(sender is IconMenuItem) )
				return;
			
			IconMenuItem imi = sender as IconMenuItem;
			int imgIndex = 0;

			if (imi == this.newIconItem)
			{
				OpenFileDialog ofd = new OpenFileDialog();
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					try
					{
						Image img = Image.FromFile(ofd.FileName);
						this.imageList.Images.Add(img);
						this.imgList.Add(img);
						RebuildIconSubmenu();
						imgIndex = this.imgList.IndexOf( img );
					}
					catch
					{
						MOG_Prompt.PromptResponse("Error", ofd.FileName + " is not a valid image file");
						return;
					}
				}
			}
			else
			{
				imgIndex = this.imgList.IndexOf( imi.Image );
			}
			
			foreach (ListViewItem item in this.SelectedItems)
				item.ImageIndex = imgIndex;
		}

		private void listView_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete)
			{
				RemoveSelected();
			}
		}

		private void miDelete_Click(object sender, System.EventArgs e)
		{
			RemoveSelected();
		}

		#endregion
	}
}



