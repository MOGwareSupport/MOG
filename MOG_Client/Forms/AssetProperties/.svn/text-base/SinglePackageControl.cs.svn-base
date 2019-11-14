using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG;
using MOG.FILENAME;
using MOG.PROPERTIES;

namespace MOG_Client.Forms.AssetProperties
{
	/// <summary>
	/// Summary description for SinglePackageControl.
	/// </summary>
	public class SinglePackageControl : System.Windows.Forms.UserControl
	{
		private object[] mPropertiesList;
		public object[] PropertiesList
		{
			get
			{
				if (DesignMode == false)
				{
					if (this.mPropertiesList == null)
					{
						throw new Exception("PropertiesList was never set.");
					}
				}
				return mPropertiesList;
			}
			set
			{
				this.mPropertiesList = value;
			}
		}
		private bool mDisableEvents = false;
		public bool mResetWildcardCheck = false;
		public AssetPropertiesForm mParent;
		private System.Windows.Forms.Panel PropertiesSingleAssetPanel;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.GroupBox PropertiesMultiPackagingEventsGroupBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox PropertiesMultiClusterCheckBox;
		private System.Windows.Forms.ToolTip MOGToolTip;
		private System.Windows.Forms.ComboBox PropertiesPackageEventsPreComboBox;
		private System.Windows.Forms.ComboBox PropertiesPackageEventsPostComboBox;
		private System.Windows.Forms.ComboBox PropertiesSinglePakDelComboBox;
		private System.Windows.Forms.Button PropertiesPackageEventsPostButton;
		private System.Windows.Forms.Button PropertiesPackageEventsPreButton;
		private System.ComponentModel.IContainer components;

		// Delegates
		public delegate void PropertyChangedEvent(object sender, System.EventArgs e);
		[Category("Behavior"), Description("Occures after a property is changed")]
		public event PropertyChangedEvent ProptertyChanged;

		public SinglePackageControl()
		{
			// This call is required by the Windows.Forms Form Designer.
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SinglePackageControl));
			this.PropertiesSingleAssetPanel = new System.Windows.Forms.Panel();
			this.PropertiesMultiPackagingEventsGroupBox = new System.Windows.Forms.GroupBox();
			this.PropertiesPackageEventsPreButton = new System.Windows.Forms.Button();
			this.PropertiesPackageEventsPostButton = new System.Windows.Forms.Button();
			this.PropertiesPackageEventsPostComboBox = new System.Windows.Forms.ComboBox();
			this.PropertiesPackageEventsPreComboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.PropertiesMultiClusterCheckBox = new System.Windows.Forms.CheckBox();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.PropertiesSinglePakDelComboBox = new System.Windows.Forms.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			this.MOGToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.PropertiesSingleAssetPanel.SuspendLayout();
			this.PropertiesMultiPackagingEventsGroupBox.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// PropertiesSingleAssetPanel
			// 
			this.PropertiesSingleAssetPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesSingleAssetPanel.AutoScroll = true;
			this.PropertiesSingleAssetPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.PropertiesSingleAssetPanel.Controls.Add(this.PropertiesMultiPackagingEventsGroupBox);
			this.PropertiesSingleAssetPanel.Controls.Add(this.PropertiesMultiClusterCheckBox);
			this.PropertiesSingleAssetPanel.Controls.Add(this.groupBox6);
			this.PropertiesSingleAssetPanel.Location = new System.Drawing.Point(8, 8);
			this.PropertiesSingleAssetPanel.Name = "PropertiesSingleAssetPanel";
			this.PropertiesSingleAssetPanel.Size = new System.Drawing.Size(288, 212);
			this.PropertiesSingleAssetPanel.TabIndex = 14;
			// 
			// PropertiesMultiPackagingEventsGroupBox
			// 
			this.PropertiesMultiPackagingEventsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesMultiPackagingEventsGroupBox.Controls.Add(this.PropertiesPackageEventsPreButton);
			this.PropertiesMultiPackagingEventsGroupBox.Controls.Add(this.PropertiesPackageEventsPostButton);
			this.PropertiesMultiPackagingEventsGroupBox.Controls.Add(this.PropertiesPackageEventsPostComboBox);
			this.PropertiesMultiPackagingEventsGroupBox.Controls.Add(this.PropertiesPackageEventsPreComboBox);
			this.PropertiesMultiPackagingEventsGroupBox.Controls.Add(this.label1);
			this.PropertiesMultiPackagingEventsGroupBox.Controls.Add(this.label2);
			this.PropertiesMultiPackagingEventsGroupBox.Location = new System.Drawing.Point(8, 8);
			this.PropertiesMultiPackagingEventsGroupBox.Name = "PropertiesMultiPackagingEventsGroupBox";
			this.PropertiesMultiPackagingEventsGroupBox.Size = new System.Drawing.Size(268, 72);
			this.PropertiesMultiPackagingEventsGroupBox.TabIndex = 0;
			this.PropertiesMultiPackagingEventsGroupBox.TabStop = false;
			this.PropertiesMultiPackagingEventsGroupBox.Text = "Package Event Commands";
			// 
			// PropertiesPackageEventsPreButton
			// 
			this.PropertiesPackageEventsPreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesPackageEventsPreButton.Location = new System.Drawing.Point(240, 17);
			this.PropertiesPackageEventsPreButton.Name = "PropertiesPackageEventsPreButton";
			this.PropertiesPackageEventsPreButton.Size = new System.Drawing.Size(24, 21);
			this.PropertiesPackageEventsPreButton.TabIndex = 35;
			this.PropertiesPackageEventsPreButton.TabStop = false;
			this.PropertiesPackageEventsPreButton.Text = "...";
			this.PropertiesPackageEventsPreButton.Click += new System.EventHandler(this.PropertiesPackageEventsPreButton_Click);
			// 
			// PropertiesPackageEventsPostButton
			// 
			this.PropertiesPackageEventsPostButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesPackageEventsPostButton.Location = new System.Drawing.Point(240, 44);
			this.PropertiesPackageEventsPostButton.Name = "PropertiesPackageEventsPostButton";
			this.PropertiesPackageEventsPostButton.Size = new System.Drawing.Size(24, 21);
			this.PropertiesPackageEventsPostButton.TabIndex = 34;
			this.PropertiesPackageEventsPostButton.TabStop = false;
			this.PropertiesPackageEventsPostButton.Text = "...";
			this.PropertiesPackageEventsPostButton.Click += new System.EventHandler(this.PropertiesPackageEventsPostButton_Click);
			// 
			// PropertiesPackageEventsPostComboBox
			// 
			this.PropertiesPackageEventsPostComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesPackageEventsPostComboBox.Items.AddRange(new object[] {
																					 "None",
																					 "Inherited"});
			this.PropertiesPackageEventsPostComboBox.Location = new System.Drawing.Point(120, 44);
			this.PropertiesPackageEventsPostComboBox.Name = "PropertiesPackageEventsPostComboBox";
			this.PropertiesPackageEventsPostComboBox.Size = new System.Drawing.Size(118, 21);
			this.PropertiesPackageEventsPostComboBox.TabIndex = 2;
			this.PropertiesPackageEventsPostComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesPackageEventsPostComboBox_Validating);
			this.PropertiesPackageEventsPostComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesPackageEventsPostComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			this.PropertiesPackageEventsPostComboBox.Leave += new System.EventHandler(this.PropertiesPackageEventsPostComboBox_Leave);
			// 
			// PropertiesPackageEventsPreComboBox
			// 
			this.PropertiesPackageEventsPreComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesPackageEventsPreComboBox.Items.AddRange(new object[] {
																					"None",
																					"Inherited"});
			this.PropertiesPackageEventsPreComboBox.Location = new System.Drawing.Point(120, 17);
			this.PropertiesPackageEventsPreComboBox.Name = "PropertiesPackageEventsPreComboBox";
			this.PropertiesPackageEventsPreComboBox.Size = new System.Drawing.Size(118, 21);
			this.PropertiesPackageEventsPreComboBox.TabIndex = 1;
			this.PropertiesPackageEventsPreComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesPackageEventsPreComboBox_Validating);
			this.PropertiesPackageEventsPreComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesPackageEventsPreComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			this.PropertiesPackageEventsPreComboBox.Leave += new System.EventHandler(this.PropertiesPackageEventsPreComboBox_Leave);
			// 
			// label1
			// 
			this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
			this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label1.Location = new System.Drawing.Point(16, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 22;
			this.label1.Text = "      Pre-Merge tool:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.MOGToolTip.SetToolTip(this.label1, "Specifies the command to be used in preparation of packaging.  (This property sup" +
				"ports tokenized MOG strings)");
			// 
			// label2
			// 
			this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
			this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label2.Location = new System.Drawing.Point(16, 46);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 16);
			this.label2.TabIndex = 24;
			this.label2.Text = "      Post-Merge tool:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.MOGToolTip.SetToolTip(this.label2, "Specifies the command to be used after packaging is completed.  (This property su" +
				"pports tokenized MOG strings)");
			// 
			// PropertiesMultiClusterCheckBox
			// 
			this.PropertiesMultiClusterCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesMultiClusterCheckBox.Location = new System.Drawing.Point(24, 176);
			this.PropertiesMultiClusterCheckBox.Name = "PropertiesMultiClusterCheckBox";
			this.PropertiesMultiClusterCheckBox.Size = new System.Drawing.Size(112, 24);
			this.PropertiesMultiClusterCheckBox.TabIndex = 6;
			this.PropertiesMultiClusterCheckBox.Text = "Cluster Packaging";
			this.PropertiesMultiClusterCheckBox.ThreeState = true;
			this.MOGToolTip.SetToolTip(this.PropertiesMultiClusterCheckBox, "Indicates whether multiple packages can be packaged together as a single package " +
				"event on the same slave.");
			this.PropertiesMultiClusterCheckBox.CheckStateChanged += new System.EventHandler(this.Properties_PropertyChanged);
			// 
			// groupBox6
			// 
			this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox6.Controls.Add(this.PropertiesSinglePakDelComboBox);
			this.groupBox6.Controls.Add(this.label9);
			this.groupBox6.Location = new System.Drawing.Point(8, 88);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(268, 56);
			this.groupBox6.TabIndex = 3;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Package Commands";
			// 
			// PropertiesSinglePakDelComboBox
			// 
			this.PropertiesSinglePakDelComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesSinglePakDelComboBox.Items.AddRange(new object[] {
																				"None",
																				"Inherited"});
			this.PropertiesSinglePakDelComboBox.Location = new System.Drawing.Point(120, 20);
			this.PropertiesSinglePakDelComboBox.Name = "PropertiesSinglePakDelComboBox";
			this.PropertiesSinglePakDelComboBox.Size = new System.Drawing.Size(144, 21);
			this.PropertiesSinglePakDelComboBox.TabIndex = 4;
			this.PropertiesSinglePakDelComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesSinglePakDelComboBox_Validating);
			this.PropertiesSinglePakDelComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesSinglePakDelComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			this.PropertiesSinglePakDelComboBox.Leave += new System.EventHandler(this.PropertiesSinglePakDelComboBox_Leave);
			// 
			// label9
			// 
			this.label9.Image = ((System.Drawing.Image)(resources.GetObject("label9.Image")));
			this.label9.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label9.Location = new System.Drawing.Point(16, 24);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(112, 16);
			this.label9.TabIndex = 16;
			this.label9.Text = "      Delete Package:";
			this.MOGToolTip.SetToolTip(this.label9, @"Specifies the command to use when deleting the entire Package.  This command is only used when rebuilding a package from scratch.  These commands will be formatted and placed inside the '\\MOG\\PackageCommands.Info' file located in the working directory.  (This property supports tokenized MOG strings)");
			// 
			// SinglePackageControl
			// 
			this.Controls.Add(this.PropertiesSingleAssetPanel);
			this.Name = "SinglePackageControl";
			this.Size = new System.Drawing.Size(304, 232);
			this.PropertiesSingleAssetPanel.ResumeLayout(false);
			this.PropertiesMultiPackagingEventsGroupBox.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SetComboBoxProperty(ComboBox EditComboBox, string newVal, bool force)
		{
			if (mResetWildcardCheck)
			{
				EditComboBox.Text = newVal;
				EditComboBox.Tag = null;
			}
			else
			{
				// Check if this new text is the same as the one already placed by a previous asset properties
				if (!force && EditComboBox.Tag == null && EditComboBox.Text.Length > 0 && string.Compare(EditComboBox.Text, newVal, true) != 0)
				{
					// If they don't match, then we have to clear this value
					EditComboBox.Text = "";
					EditComboBox.Tag = "Already Set";
				}
				else
				{
					EditComboBox.Text = newVal;
				}
			}
		}

		private void SetCheckBoxProperty(CheckBox EditCheckBox, MOG_InheritedBoolean newVal)
		{
			CheckState state = CheckState.Unchecked;
			switch (newVal)
			{
				case MOG_InheritedBoolean.True:
					state = CheckState.Checked;
					break;
				case MOG_InheritedBoolean.False:
					state = CheckState.Unchecked;
					break;
				case MOG_InheritedBoolean.Inherited:
					state = CheckState.Indeterminate;
					break;
			}

			if (mResetWildcardCheck)
			{
				EditCheckBox.CheckState = state;
				EditCheckBox.Tag = null;
			}
			else
			{
				// Check if this new text is the same as the one already placed by a previous asset properties
				if (EditCheckBox.Tag != null && EditCheckBox.CheckState != state)
				{
					// If they don't match, then we have to clear this value
					EditCheckBox.CheckState = CheckState.Indeterminate;
					EditCheckBox.Tag = "Already set";
				}
				else
				{
					EditCheckBox.CheckState = state;
				}
			}
		}

		public void InitializePackagingInfo(MOG_Properties properties, MOG_Filename currentFilename)
		{
			mDisableEvents = true;
			// Put in the Multi-asset commands
			SetComboBoxProperty(this.PropertiesSinglePakDelComboBox, properties.PackageCommand_DeletePackageFile, false);
			SetComboBoxProperty(this.PropertiesPackageEventsPreComboBox, properties.PackagePreMergeEvent, false);
			SetComboBoxProperty(this.PropertiesPackageEventsPostComboBox, properties.PackagePostMergeEvent, false);

			SetCheckBoxProperty(this.PropertiesMultiClusterCheckBox, properties.ClusterPackaging_InheritedBoolean);
			mResetWildcardCheck = false;
			mDisableEvents = false;
		}

		public void IntializeWizardSettings(ArrayList commands)
		{
			foreach (WizardSetting setting in commands)
			{
				switch(setting.Command)
				{
					case "PackageDeletePackageFile":
						SetComboBoxProperty(this.PropertiesSinglePakDelComboBox, setting.Setting, true);
						break;
				}
			}
		}

		public void SavePackagingChanges(MOG_Properties properties)
		{
			switch(this.PropertiesMultiClusterCheckBox.CheckState)
			{
				case CheckState.Checked:
					properties.ClusterPackaging_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.True;
					break;
				case CheckState.Unchecked:
					properties.ClusterPackaging_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.False;
					break;
				case CheckState.Indeterminate:
					properties.ClusterPackaging_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.Inherited;
					break;
			}
		}

		private void Properties_PropertyChanged(object sender, System.EventArgs e)
		{
			// Inform any delegates of this event
			if (ProptertyChanged != null && !mDisableEvents)
			{
				object []args = {sender, e};
				this.Invoke(ProptertyChanged, args);
			}
		}

		#region Combo edited and validated events

		private void PropertiesActivateControl_Click(object sender, System.EventArgs e)
		{
			this.ActiveControl = sender as Control;
		}

		private void PropertiesPackageEventsPostComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SetComboBoxProperty(this.PropertiesPackageEventsPostComboBox, properties.PackagePostMergeEvent, true);
			}
		}

		private void PropertiesPackageEventsPostComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.PackagePostMergeEvent = this.PropertiesPackageEventsPostComboBox.Text;
			}
		}


		private void PropertiesPackageEventsPreComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.PackagePreMergeEvent = this.PropertiesPackageEventsPreComboBox.Text;
			}
		}

		private void PropertiesPackageEventsPreComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SetComboBoxProperty(this.PropertiesPackageEventsPreComboBox, properties.PackagePreMergeEvent, true);
			}
		}
		private void PropertiesSinglePakDelComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.PackageCommand_DeletePackageFile = this.PropertiesSinglePakDelComboBox.Text;
			}
		}

		private void PropertiesSinglePakDelComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SetComboBoxProperty(this.PropertiesSinglePakDelComboBox, properties.PackageCommand_DeletePackageFile, true);
			}
		}
		#endregion
	
		private void PropertiesPackageEventsPreButton_Click(object sender, System.EventArgs e)
		{
			this.PropertiesPackageEventsPreComboBox.Text = mParent.GetValueForProperty("PackagePreMergeEvent", PropertiesPackageEventsPreComboBox.Text) as string;

			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.PackagePreMergeEvent = this.PropertiesPackageEventsPreComboBox.Text;
			}
		}

		private void PropertiesPackageEventsPostButton_Click(object sender, System.EventArgs e)
		{
			this.PropertiesPackageEventsPostComboBox.Text = mParent.GetValueForProperty("PackagePostMergeEvent", PropertiesPackageEventsPostComboBox.Text) as string;

			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.PackagePostMergeEvent = this.PropertiesPackageEventsPostComboBox.Text;
			}
		}
	}
}

