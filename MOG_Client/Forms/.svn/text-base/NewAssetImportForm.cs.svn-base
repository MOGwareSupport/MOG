using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using MOG;
using MOG.INI;
using MOG.PROJECT;
using MOG.PROPERTIES;
using MOG.PLATFORM;
using MOG.FILENAME;
using MOG.DATABASE;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MogUtils_DataSets;



namespace MOG_Client
{
	/// <summary>
	/// Summary description for NewAssetImportForm.
	/// </summary>
	public class NewAssetImportForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ComboBox AssetProjectKeyComboBox;
		private System.Windows.Forms.ComboBox AssetAssetKeyComboBox;
		private System.Windows.Forms.ComboBox AssetPackageComboBox;
		private System.Windows.Forms.ComboBox AssetGroupComboBox;
		private System.Windows.Forms.ComboBox AssetPlatformComboBox;
		private System.Windows.Forms.TextBox AssetLabelTextBox;
		private System.Windows.Forms.TextBox AssetExtensionTextBox;
		private System.Windows.Forms.Button AssetOkButton;
		private System.Windows.Forms.Button AssetOkAllButton;
		private System.Windows.Forms.Button AssetCancelButton;
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.CheckBox AssetRenameSourceCheckBox;
		public bool mLabelChanged;
		private bool mBuilding;

		private System.Windows.Forms.Label AssetFilename;
		private System.Windows.Forms.PictureBox AssetValidPictureBox;
		private System.Windows.Forms.ImageList AssetValidImageList;
		private string mFullFilename;
		public System.Windows.Forms.CheckBox AssetSmartImportCheckBox;
//		private string[] mField;

		public NewAssetImportForm(string fullFilename)
		{
			mFullFilename = fullFilename;
			mLabelChanged = false;
			mBuilding = false;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

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

		public string GetFixedAssetName(string assetName)
		{
			string targetName;

			// Setup asset filename
			targetName = String.Concat(AssetProjectKeyComboBox.Text, ".", AssetAssetKeyComboBox.Text, ".", AssetPackageComboBox.Text, ".", AssetGroupComboBox.Text, ".", AssetLabelTextBox.Text, ".", AssetPlatformComboBox.Text, ".", AssetExtensionTextBox.Text);

			return targetName;
		}

		/// <summary>
		/// Loads up a string array with each of the elements of a qulaified MOG_Filename
		/// </summary>
		/// <param name="field"></param>
		public void SaveFields(ref string[] field)
		{
			field[0] = AssetProjectKeyComboBox.Text;
			field[1] = AssetAssetKeyComboBox.Text;
			field[2] = AssetPackageComboBox.Text;
			field[3] = AssetGroupComboBox.Text;
			field[4] = AssetPlatformComboBox.Text;
		}

		/// <summary>
		/// This is the first function called to startup this form.  In this method, we setup all the form
		/// controlls with the correct information for this asset
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="field"></param>
		public void InitializeFile(string filename, ref string[] field)
		{
//			string targetName;
//
//			// Fill in default values for full name
//			this.Text = filename;
//			this.AssetFilename.Text = "Importing...\n" + mFullFilename.Replace("\\", " \\ ");
//			mField = field;
//
//			// Initlize project Key
//			if(field[0].Length != 0)
//			{
//				AssetProjectKeyComboBox.Text = field[0];
//			}
//			else
//			{
//				this.AssetProjectKeyComboBox.Text = MOG_ControllerProject.GetProject().GetProjectKey();
//			}
//			
//			//  Initialize Key comboBox
//			if(field[1].Length != 0)
//			{
//				this.AssetAssetKeyComboBox.Text = field[1];
//				this.AssetAssetKeyComboBox_SelectedIndexChanged(AssetAssetKeyComboBox, new EventArgs());
//			}
//			else
//			{				
//				this.AssetAssetKeyComboBox.Items.Clear();
//				System.Collections.IEnumerator myEnumerator = MOG_ControllerProject.GetProject().GetAssetTypes().GetEnumerator();
//				while ( myEnumerator.MoveNext() )
//				{				
//					this.AssetAssetKeyComboBox.Items.Add(((MOG_Properties)myEnumerator.Current).mAssetKey);
//				}
//			}
//			
//			// Initialize the packages
//			if(field[2].Length != 0)
//			{
//				this.AssetPackageComboBox.Text = field[2];
//			}
//			else
//			{
//				
//				
//			}
//			
//			// Initialize the groups
//			if(field[3].Length != 0)
//			{
//				this.AssetGroupComboBox.Text = field[3];
//			}
//			else
//			{
//				
//				
//			}
//
//			// Initilize the label
//			targetName = filename.Substring(filename.LastIndexOf("\\")+1);
//			if (targetName.IndexOf(".") != -1)
//			{
//				try
//				{
//					this.AssetLabelTextBox.Text = Path.GetFileNameWithoutExtension(targetName); //targetName.Substring(0,targetName.LastIndexOf("."));
//				}
//				catch(Exception e)
//				{
//					e.ToString();
//				}
//			}
//			else
//			{
//				this.AssetLabelTextBox.Text = targetName;
//			}
//
//
//			// Initialize platforms comboBox
//			if(field[4].Length != 0)
//			{
//				this.AssetPlatformComboBox.Text = field[4];
//			}
//			else
//			{
//				this.AssetPlatformComboBox.Items.Clear();
//				this.AssetPlatformComboBox.Items.Add("All");
//			
//				// Add all the platforms
//				System.Collections.IEnumerator myEnumerator = MOG_ControllerProject.GetProject().GetPlatforms().GetEnumerator();
//				while ( myEnumerator.MoveNext() )
//				{
//					this.AssetPlatformComboBox.Items.Add(((MOG_Platform)myEnumerator.Current).mPlatformName);
//				}
//			}
//			
//			// Initilize the Extension
//			if (targetName.IndexOf(".") != -1)
//			{
//				this.AssetExtensionTextBox.Text = filename.Substring(filename.LastIndexOf(".")+1);
//			}
//			else
//			{
//				this.AssetExtensionTextBox.Text = "";
//			}
			
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(NewAssetImportForm));
			this.AssetProjectKeyComboBox = new System.Windows.Forms.ComboBox();
			this.AssetAssetKeyComboBox = new System.Windows.Forms.ComboBox();
			this.AssetPackageComboBox = new System.Windows.Forms.ComboBox();
			this.AssetGroupComboBox = new System.Windows.Forms.ComboBox();
			this.AssetPlatformComboBox = new System.Windows.Forms.ComboBox();
			this.AssetLabelTextBox = new System.Windows.Forms.TextBox();
			this.AssetExtensionTextBox = new System.Windows.Forms.TextBox();
			this.AssetOkButton = new System.Windows.Forms.Button();
			this.AssetOkAllButton = new System.Windows.Forms.Button();
			this.AssetCancelButton = new System.Windows.Forms.Button();
			this.AssetRenameSourceCheckBox = new System.Windows.Forms.CheckBox();
			this.AssetFilename = new System.Windows.Forms.Label();
			this.AssetValidPictureBox = new System.Windows.Forms.PictureBox();
			this.AssetValidImageList = new System.Windows.Forms.ImageList(this.components);
			this.AssetSmartImportCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// AssetProjectKeyComboBox
			// 
			this.AssetProjectKeyComboBox.Location = new System.Drawing.Point(8, 40);
			this.AssetProjectKeyComboBox.Name = "AssetProjectKeyComboBox";
			this.AssetProjectKeyComboBox.Size = new System.Drawing.Size(80, 21);
			this.AssetProjectKeyComboBox.TabIndex = 0;
			this.AssetProjectKeyComboBox.Text = "[Project key]";
			// 
			// AssetAssetKeyComboBox
			// 
			this.AssetAssetKeyComboBox.Location = new System.Drawing.Point(96, 40);
			this.AssetAssetKeyComboBox.Name = "AssetAssetKeyComboBox";
			this.AssetAssetKeyComboBox.Size = new System.Drawing.Size(112, 21);
			this.AssetAssetKeyComboBox.Sorted = true;
			this.AssetAssetKeyComboBox.TabIndex = 1;
			this.AssetAssetKeyComboBox.Text = "[Key]";
			this.AssetAssetKeyComboBox.TextChanged += new System.EventHandler(this.AssetAssetKeyComboBox_TextChanged);
			this.AssetAssetKeyComboBox.SelectedIndexChanged += new System.EventHandler(this.AssetAssetKeyComboBox_SelectedIndexChanged);
			// 
			// AssetPackageComboBox
			// 
			this.AssetPackageComboBox.DropDownWidth = 135;
			this.AssetPackageComboBox.Location = new System.Drawing.Point(216, 40);
			this.AssetPackageComboBox.Name = "AssetPackageComboBox";
			this.AssetPackageComboBox.Size = new System.Drawing.Size(112, 21);
			this.AssetPackageComboBox.Sorted = true;
			this.AssetPackageComboBox.TabIndex = 2;
			this.AssetPackageComboBox.Text = "[Package]";
			this.AssetPackageComboBox.TextChanged += new System.EventHandler(this.AssetPackageComboBox_TextChanged);
			this.AssetPackageComboBox.SelectedIndexChanged += new System.EventHandler(this.AssetPackageComboBox_SelectedIndexChanged);
			// 
			// AssetGroupComboBox
			// 
			this.AssetGroupComboBox.DropDownWidth = 135;
			this.AssetGroupComboBox.Location = new System.Drawing.Point(328, 40);
			this.AssetGroupComboBox.Name = "AssetGroupComboBox";
			this.AssetGroupComboBox.Size = new System.Drawing.Size(112, 21);
			this.AssetGroupComboBox.Sorted = true;
			this.AssetGroupComboBox.TabIndex = 3;
			this.AssetGroupComboBox.Text = "[Group]";
			this.AssetGroupComboBox.TextChanged += new System.EventHandler(this.AssetGroupComboBox_TextChanged);
			this.AssetGroupComboBox.SelectedIndexChanged += new System.EventHandler(this.AssetGroupComboBox_SelectedIndexChanged);
			// 
			// AssetPlatformComboBox
			// 
			this.AssetPlatformComboBox.Location = new System.Drawing.Point(600, 40);
			this.AssetPlatformComboBox.Name = "AssetPlatformComboBox";
			this.AssetPlatformComboBox.Size = new System.Drawing.Size(64, 21);
			this.AssetPlatformComboBox.TabIndex = 4;
			this.AssetPlatformComboBox.Text = "All";
			this.AssetPlatformComboBox.TextChanged += new System.EventHandler(this.AssetPlatformComboBox_TextChanged);
			this.AssetPlatformComboBox.SelectedIndexChanged += new System.EventHandler(this.AssetPlatformComboBox_SelectedIndexChanged);
			// 
			// AssetLabelTextBox
			// 
			this.AssetLabelTextBox.Location = new System.Drawing.Point(448, 40);
			this.AssetLabelTextBox.Name = "AssetLabelTextBox";
			this.AssetLabelTextBox.Size = new System.Drawing.Size(144, 20);
			this.AssetLabelTextBox.TabIndex = 5;
			this.AssetLabelTextBox.Text = "Label";
			this.AssetLabelTextBox.TextChanged += new System.EventHandler(this.AssetLabelTextBox_TextChanged);
			// 
			// AssetExtensionTextBox
			// 
			this.AssetExtensionTextBox.Location = new System.Drawing.Point(672, 40);
			this.AssetExtensionTextBox.Name = "AssetExtensionTextBox";
			this.AssetExtensionTextBox.Size = new System.Drawing.Size(64, 20);
			this.AssetExtensionTextBox.TabIndex = 6;
			this.AssetExtensionTextBox.Text = "Extension";
			// 
			// AssetOkButton
			// 
			this.AssetOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.AssetOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetOkButton.Location = new System.Drawing.Point(256, 72);
			this.AssetOkButton.Name = "AssetOkButton";
			this.AssetOkButton.TabIndex = 7;
			this.AssetOkButton.Text = "Ok";
			// 
			// AssetOkAllButton
			// 
			this.AssetOkAllButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.AssetOkAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetOkAllButton.Location = new System.Drawing.Point(336, 72);
			this.AssetOkAllButton.Name = "AssetOkAllButton";
			this.AssetOkAllButton.TabIndex = 8;
			this.AssetOkAllButton.Text = "Ok to All";
			// 
			// AssetCancelButton
			// 
			this.AssetCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.AssetCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetCancelButton.Location = new System.Drawing.Point(416, 72);
			this.AssetCancelButton.Name = "AssetCancelButton";
			this.AssetCancelButton.TabIndex = 9;
			this.AssetCancelButton.Text = "Cancel";
			// 
			// AssetRenameSourceCheckBox
			// 
			this.AssetRenameSourceCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AssetRenameSourceCheckBox.Location = new System.Drawing.Point(13, 72);
			this.AssetRenameSourceCheckBox.Name = "AssetRenameSourceCheckBox";
			this.AssetRenameSourceCheckBox.TabIndex = 10;
			this.AssetRenameSourceCheckBox.Text = "Rename Source";
			// 
			// AssetFilename
			// 
			this.AssetFilename.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.AssetFilename.Location = new System.Drawing.Point(8, 0);
			this.AssetFilename.Name = "AssetFilename";
			this.AssetFilename.Size = new System.Drawing.Size(704, 40);
			this.AssetFilename.TabIndex = 11;
			// 
			// AssetValidPictureBox
			// 
			this.AssetValidPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("AssetValidPictureBox.Image")));
			this.AssetValidPictureBox.Location = new System.Drawing.Point(720, 8);
			this.AssetValidPictureBox.Name = "AssetValidPictureBox";
			this.AssetValidPictureBox.Size = new System.Drawing.Size(24, 16);
			this.AssetValidPictureBox.TabIndex = 12;
			this.AssetValidPictureBox.TabStop = false;
			// 
			// AssetValidImageList
			// 
			this.AssetValidImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.AssetValidImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("AssetValidImageList.ImageStream")));
			this.AssetValidImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// AssetSmartImportCheckBox
			// 
			this.AssetSmartImportCheckBox.Location = new System.Drawing.Point(616, 72);
			this.AssetSmartImportCheckBox.Name = "AssetSmartImportCheckBox";
			this.AssetSmartImportCheckBox.Size = new System.Drawing.Size(136, 24);
			this.AssetSmartImportCheckBox.TabIndex = 13;
			this.AssetSmartImportCheckBox.Text = "Smart Import (Slower)";
			// 
			// NewAssetImportForm
			// 
			this.AcceptButton = this.AssetOkButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.AssetCancelButton;
			this.ClientSize = new System.Drawing.Size(752, 103);
			this.Controls.Add(this.AssetSmartImportCheckBox);
			this.Controls.Add(this.AssetValidPictureBox);
			this.Controls.Add(this.AssetFilename);
			this.Controls.Add(this.AssetRenameSourceCheckBox);
			this.Controls.Add(this.AssetCancelButton);
			this.Controls.Add(this.AssetOkAllButton);
			this.Controls.Add(this.AssetOkButton);
			this.Controls.Add(this.AssetExtensionTextBox);
			this.Controls.Add(this.AssetLabelTextBox);
			this.Controls.Add(this.AssetPlatformComboBox);
			this.Controls.Add(this.AssetGroupComboBox);
			this.Controls.Add(this.AssetPackageComboBox);
			this.Controls.Add(this.AssetAssetKeyComboBox);
			this.Controls.Add(this.AssetProjectKeyComboBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewAssetImportForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "NewAssetImportForm";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// A new asset key was selected.  Now go and pupulate the package and group controls
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssetAssetKeyComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (!MOG_ControllerProject.IsProject())
			{
				return;
			}

//			mBuilding = true;
//
//			// Clear our list
//			AssetPackageComboBox.Items.Clear();
//
//			// Clear both, group and package
//			AssetPackageComboBox.Text = "";
//			AssetGroupComboBox.Text = "";
//			AssetPackageComboBox.Enabled = true;
//
//			string Key = ((ComboBox)sender).Text;
//
//
//			if (MOG_ControllerProject.GetProject().GetProperties(Key).mAssetTypes.Count > 0)
//			{
//				// Add all the type names for this asset key
//				foreach (MOG_AssetType packageName in MOG_ControllerProject.GetProject().GetProperties(Key).mAssetTypes)
//				{
//					AssetPackageComboBox.Items.Add(packageName.mTypeName);
//				}
//
//				// Attempt to fill in the rest of the name
//				MOG_Filename autoFile = new MOG_Filename();
//// JohnRen - Removed - Should simply check database to construct the assetFilename
////				autoFile.ConstructAssetName(mFullFilename, string.Concat(MOG_ControllerProject.GetProject().GetProjectKey(), ".", Key,".All"));
//				
//				// Update our valid icon
//				//UpdateAssetVerifyIcon(autoFile);
//
//				if (autoFile.GetAssetClassification().Length != 0)
//				{
//					string Package = autoFile.GetAssetClassification();
//					AssetPackageComboBox.Text = Package;
//					mField[2] = Package;
//				}
//			}
//			else
//			{
//				MessageBox.Show("Asset_Types ini file was not initialized correctly.  Cannot populate the packages and groups!", "ERROR", MessageBoxButtons.OK);
//				Close();
//			}
//
//			// Set the package as the first valid package in the combo box
//			if (AssetPackageComboBox.Items.Count > 0 && AssetPackageComboBox.SelectedItem == null)
//			{
//				AssetPackageComboBox.SelectedIndex = 0;
//			}
//
//			// only allow changing of the combo box if there are more than one options
//			if (AssetPackageComboBox.Items.Count == 1)
//			{
//				AssetPackageComboBox.Enabled = false;
//			}

			mBuilding = false;
		}

		/// <summary>
		/// A new Package was selected, populate the group control now.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssetPackageComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (!MOG_ControllerProject.IsProject())
			{
				return;
			}
//			mBuilding = true;
//
//			// Clear our list
//			AssetGroupComboBox.Items.Clear();
//			AssetGroupComboBox.Text = "";
//			AssetGroupComboBox.Enabled = true;
//
//			string Key = AssetAssetKeyComboBox.Text;
//			string Package = ((ComboBox)sender).Text;
//
//			// Find the asset that is in the key ComboBox
//			foreach (MOG_AssetType packageName in MOG_ControllerProject.GetProject().GetProperties(Key).mAssetTypes)
//			{
//				// Find this type in the assets tyle lists
//				if (string.Compare(packageName.mTypeName, Package, true) == 0)
//				{
//					// Add all the group names for this type
//					foreach (string groupName in packageName.mSubTypes)
//					{
//						if (string.Compare(groupName, "*") == 0)
//						{
//							// Lets try and guess a possible group names
//							Array names = FindWildCardNames(Package);
//							if (names != null)
//							{
//								foreach (string wildGroupName in names)
//								{
//									AssetGroupComboBox.Items.Add(wildGroupName);
//								}
//							}
//						}
//						else
//						{
//							AssetGroupComboBox.Items.Add(groupName);
//						}
//					}
//				}
//
//				// Set the package as the first valid package in the combo box
//				if (AssetGroupComboBox.Items.Count > 0)
//				{
//					AssetGroupComboBox.SelectedIndex = 0;
//				}
//
//				// only allow changing of the combo box if there are more than one options
//				if (AssetGroupComboBox.Items.Count == 1)
//				{
//					AssetGroupComboBox.Enabled = false;
//				}
//			}
//
//			// Attempt to fill in the rest of the name
//			MOG_Filename autoFile = new MOG_Filename();
//// JohnRen - Removed - Should simply check database to construct the assetFilename
////			autoFile.ConstructAssetName(mFullFilename, string.Concat(MOG_ControllerProject.GetProject().GetProjectKey(), ".", Key,".All"));
//			
//			// Update our valid icon
//			UpdateAssetVerifyIcon(autoFile);
//
//			if (autoFile.GetAssetSubClass().Length != 0)
//			{
//				string Group = "";
//				// Check to see if the group is a wildcard
//				if (string.Compare(autoFile.GetAssetSubClass(), "*") == 0)
//				{
//					Group = (string)AssetGroupComboBox.Items[0];
//				}
//				else
//				{
//					Group = autoFile.GetAssetSubClass();
//					mField[3] = Group;
//				}
//
//				AssetGroupComboBox.Text = Group;
//			}
//		
			mBuilding = false;
		}

		/// <summary>
		/// If the label changes, we must rename the source file in the import
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssetLabelTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// If the label changes, we must rename the source file in the import
			mLabelChanged = true;

			if (!mBuilding && AssetSmartImportCheckBox.Checked)
			{
				string label = ((TextBox)sender).Text;

				// Update our valid icon
				MOG_Filename checkFile = new MOG_Filename();
// JohnRen - Removed - Should simply check database to construct the assetFilename
//				checkFile.ConstructAssetName(label, MOG_ControllerProject.GetProject().GetProjectKey() + "." + AssetAssetKeyComboBox.Text + "." + AssetPackageComboBox.Text + "." + AssetGroupComboBox.Text + "." + AssetPlatformComboBox.Text);
				UpdateAssetVerifyIcon(checkFile);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseName"></param>
		/// <returns></returns>
		private Array FindWildCardNames(string baseName)
		{
			string path = mFullFilename;

			try
			{
				// Walk back the path of the full filename till we find the baseName
				while(path.Length > 0)
				{
					string name = path.Substring(path.LastIndexOf("\\")+1);
					if (string.Compare(name, baseName, true) == 0)
					{
						// We've got our path! Now exit
						break;
					}
					else
					{
						// Chuck path down by a directory and continue to look
						path = path.Substring(0, path.LastIndexOf("\\"));
					}
				}
			}
			catch
			{
				// No base name was found
				return null;
			}

			// Ok now lets find some potential names
			DirectoryInfo dir = new DirectoryInfo(path);
			if (dir.Exists)
			{
				DirectoryInfo []dirNames = dir.GetDirectories();
				Array validNames = Array.CreateInstance( typeof(string), dirNames.Length);
				for (int i = 0; i < dirNames.Length; i++)
				{
					validNames.SetValue((string)dirNames[i].Name, i);
				}

				return validNames;
			}

			return null;
		}

		private void UpdateAssetVerifyIcon(MOG_Filename verifyFile)
		{
			if (verifyFile.GetType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
			{
				if (MOG_ControllerSystem.GetDB() != null && 
					MOG_DBAssetAPI.GetAssetVersion(verifyFile) != null &&
					MOG_DBAssetAPI.GetAssetVersion(verifyFile).Length != 0)
				{
					// This is a perfect asset that has a prior version
					this.AssetValidPictureBox.Image = new Bitmap(AssetValidImageList.Images[2]);
				}
				else
				{
					// good asset with no prior version
					this.AssetValidPictureBox.Image = new Bitmap(AssetValidImageList.Images[1]);
				}
			}
			else
			{
				// Bad Asset name
				this.AssetValidPictureBox.Image = new Bitmap(AssetValidImageList.Images[0]);
			}
		}

		private void AssetGroupComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (!mBuilding && AssetSmartImportCheckBox.Checked)
			{
				// Update our valid icon
				MOG_Filename checkFile = new MOG_Filename();
// JohnRen - Removed - Should simply check database to construct the assetFilename
//				checkFile.ConstructAssetName(AssetLabelTextBox.Text, MOG_ControllerProject.GetProject().GetProjectKey() + "." + AssetAssetKeyComboBox.Text + "." + AssetPackageComboBox.Text + "." + AssetGroupComboBox.Text + "." + AssetPlatformComboBox.Text);
				UpdateAssetVerifyIcon(checkFile);
			}
		}

		private void AssetPlatformComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (!mBuilding && AssetSmartImportCheckBox.Checked)
			{
				// Update our valid icon
//				MOG_Filename checkFile = new MOG_Filename();
//				checkFile.ConstructAssetName(AssetLabelTextBox.Text, MOG_ControllerProject.GetProject().GetProjectKey() + "." + AssetAssetKeyComboBox.Text + "." + AssetPackageComboBox.Text + "." + AssetGroupComboBox.Text + "." + AssetPlatformComboBox.Text);
//				UpdateAssetVerifyIcon(checkFile);
			}
		}

		private void AssetPackageComboBox_TextChanged(object sender, System.EventArgs e)
		{
			if (!mBuilding && AssetSmartImportCheckBox.Checked)
			{
				string label = ((ComboBox)sender).Text;

				// Update our valid icon
				MOG_Filename checkFile = new MOG_Filename();
// JohnRen - Removed - Should simply check database to construct the assetFilename
//				checkFile.ConstructAssetName(AssetLabelTextBox.Text, MOG_ControllerProject.GetProject().GetProjectKey() + "." + label + "." + AssetAssetKeyComboBox.Text + "." + AssetGroupComboBox.Text + "." + AssetPlatformComboBox.Text);
				UpdateAssetVerifyIcon(checkFile);
			}
		}

		private void AssetGroupComboBox_TextChanged(object sender, System.EventArgs e)
		{
			if (!mBuilding && AssetSmartImportCheckBox.Checked)
			{
				string label = ((ComboBox)sender).Text;

				// Update our valid icon
				MOG_Filename checkFile = new MOG_Filename();
// JohnRen - Removed - Should simply check database to construct the assetFilename
//				checkFile.ConstructAssetName(AssetLabelTextBox.Text, MOG_ControllerProject.GetProject().GetProjectKey() + "." + AssetAssetKeyComboBox.Text + "." +AssetPackageComboBox.Text + "." + label + "." + AssetPlatformComboBox.Text);
				UpdateAssetVerifyIcon(checkFile);
			}
		}

		private void AssetPlatformComboBox_TextChanged(object sender, System.EventArgs e)
		{
			if (!mBuilding && AssetSmartImportCheckBox.Checked)
			{
				string label = ((ComboBox)sender).Text;

				// Update our valid icon
				MOG_Filename checkFile = new MOG_Filename();
// JohnRen - Removed - Should simply check database to construct the assetFilename
//				checkFile.ConstructAssetName(AssetLabelTextBox.Text, MOG_ControllerProject.GetProject().GetProjectKey() + "." + AssetAssetKeyComboBox.Text + "." + AssetPackageComboBox.Text + "." + AssetGroupComboBox.Text + "." + label);
				UpdateAssetVerifyIcon(checkFile);
			}
		}

		private void AssetAssetKeyComboBox_TextChanged(object sender, System.EventArgs e)
		{
			if (!mBuilding && AssetSmartImportCheckBox.Checked)
			{
				string label = ((ComboBox)sender).Text;

				// Update our valid icon
				MOG_Filename checkFile = new MOG_Filename();
// JohnRen - Removed - Should simply check database to construct the assetFilename
//				checkFile.ConstructAssetName(AssetLabelTextBox.Text, MOG_ControllerProject.GetProject().GetProjectKey() + "." + label + "." + AssetPackageComboBox.Text + "." + AssetGroupComboBox.Text + "." + AssetPlatformComboBox.Text);
				UpdateAssetVerifyIcon(checkFile);
			}
		}
	}
}
