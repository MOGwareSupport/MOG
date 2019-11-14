using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG;
using MOG.DOSUTILS;
using MOG.PROMPT;
using MOG.FILENAME;
using MOG.PROPERTIES;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERRIPPER;

using MOG_Client.Client_Mog_Utilities;
using System.IO;

namespace MOG_Client.Forms.AssetProperties
{
	/// <summary>
	/// Summary description for RippersControl.
	/// </summary>
	public class RippersControl : System.Windows.Forms.UserControl
	{
		private object[] mPropertiesList;
		public object[] PropertiesList
		{
			get
			{
				if(this.mPropertiesList == null)
				{
					if (!DesignMode)
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
		private System.Windows.Forms.GroupBox PropertiesRippingSlavesGroupBox;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.CheckBox PropertiesRippingShowWindowCheckBox;
		private System.Windows.Forms.GroupBox PropertiesRippingConfigGroupBox;
		private System.Windows.Forms.Button ExploreSlaveTaskerButton;
		private System.Windows.Forms.Button ExploreRipperButton;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.CheckBox PropertiesRippingDivergentCheckBox;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.ComboBox PropertiesRippingSlaveTaskerComboBox;
		private System.Windows.Forms.ComboBox PropertiesRippingRipperComboBox;
		private System.Windows.Forms.ComboBox PropertiesRippingValidSlavesComboBox;
		private System.Windows.Forms.CheckBox PropertiesRippingAutoDetectCheckBox;
		private System.Windows.Forms.CheckBox PropertiesUseTempCheckBox;
		private System.Windows.Forms.CheckBox PropertiesRippingLocalTempCheckBox;
		private System.Windows.Forms.CheckBox PropertiesRippingCopyToTempCheckBox;
		private System.Windows.Forms.Panel RipperPanel;
		private System.Windows.Forms.Button PropertiesRippingTestButton;
		private System.Windows.Forms.Button PropertiesRippingEditButton;
		private MOG_Client.Forms.ArgumentControl.MogArgumentsButton mogArgumentsButton1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// Delegates
		public delegate void PropertyChangedEvent(object sender, System.EventArgs e);
		[Category("Behavior"), Description("Occures after a property is changed")]
		public event PropertyChangedEvent ProptertyChanged;

		public RippersControl()
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RippersControl));
			this.PropertiesRippingSlavesGroupBox = new System.Windows.Forms.GroupBox();
			this.PropertiesRippingValidSlavesComboBox = new System.Windows.Forms.ComboBox();
			this.label20 = new System.Windows.Forms.Label();
			this.PropertiesRippingShowWindowCheckBox = new System.Windows.Forms.CheckBox();
			this.PropertiesRippingConfigGroupBox = new System.Windows.Forms.GroupBox();
			this.PropertiesRippingEditButton = new System.Windows.Forms.Button();
			this.PropertiesRippingCopyToTempCheckBox = new System.Windows.Forms.CheckBox();
			this.PropertiesRippingLocalTempCheckBox = new System.Windows.Forms.CheckBox();
			this.PropertiesUseTempCheckBox = new System.Windows.Forms.CheckBox();
			this.PropertiesRippingAutoDetectCheckBox = new System.Windows.Forms.CheckBox();
			this.PropertiesRippingRipperComboBox = new System.Windows.Forms.ComboBox();
			this.PropertiesRippingSlaveTaskerComboBox = new System.Windows.Forms.ComboBox();
			this.ExploreSlaveTaskerButton = new System.Windows.Forms.Button();
			this.ExploreRipperButton = new System.Windows.Forms.Button();
			this.label18 = new System.Windows.Forms.Label();
			this.PropertiesRippingDivergentCheckBox = new System.Windows.Forms.CheckBox();
			this.label19 = new System.Windows.Forms.Label();
			this.RipperPanel = new System.Windows.Forms.Panel();
			this.PropertiesRippingTestButton = new System.Windows.Forms.Button();
			this.mogArgumentsButton1 = new MOG_Client.Forms.ArgumentControl.MogArgumentsButton();
			this.PropertiesRippingSlavesGroupBox.SuspendLayout();
			this.PropertiesRippingConfigGroupBox.SuspendLayout();
			this.RipperPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// PropertiesRippingSlavesGroupBox
			// 
			this.PropertiesRippingSlavesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesRippingSlavesGroupBox.Controls.Add(this.PropertiesRippingValidSlavesComboBox);
			this.PropertiesRippingSlavesGroupBox.Controls.Add(this.label20);
			this.PropertiesRippingSlavesGroupBox.Controls.Add(this.PropertiesRippingShowWindowCheckBox);
			this.PropertiesRippingSlavesGroupBox.Location = new System.Drawing.Point(8, 224);
			this.PropertiesRippingSlavesGroupBox.Name = "PropertiesRippingSlavesGroupBox";
			this.PropertiesRippingSlavesGroupBox.Size = new System.Drawing.Size(356, 100);
			this.PropertiesRippingSlavesGroupBox.TabIndex = 8;
			this.PropertiesRippingSlavesGroupBox.TabStop = false;
			this.PropertiesRippingSlavesGroupBox.Text = "Slave configuration";
			// 
			// PropertiesRippingValidSlavesComboBox
			// 
			this.PropertiesRippingValidSlavesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesRippingValidSlavesComboBox.Items.AddRange(new object[] {
            "None",
            "Inherited"});
			this.PropertiesRippingValidSlavesComboBox.Location = new System.Drawing.Point(40, 72);
			this.PropertiesRippingValidSlavesComboBox.Name = "PropertiesRippingValidSlavesComboBox";
			this.PropertiesRippingValidSlavesComboBox.Size = new System.Drawing.Size(308, 21);
			this.PropertiesRippingValidSlavesComboBox.TabIndex = 10;
			this.PropertiesRippingValidSlavesComboBox.Leave += new System.EventHandler(this.PropertiesRippingValidSlavesComboBox_Leave);
			this.PropertiesRippingValidSlavesComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesRippingValidSlavesComboBox_Validating);
			this.PropertiesRippingValidSlavesComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesRippingValidSlavesComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			// 
			// label20
			// 
			this.label20.Image = ((System.Drawing.Image)(resources.GetObject("label20.Image")));
			this.label20.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label20.Location = new System.Drawing.Point(16, 56);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(232, 16);
			this.label20.TabIndex = 7;
			this.label20.Text = "      Only rip using slaves on these machines:";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// PropertiesRippingShowWindowCheckBox
			// 
			this.PropertiesRippingShowWindowCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesRippingShowWindowCheckBox.Location = new System.Drawing.Point(16, 24);
			this.PropertiesRippingShowWindowCheckBox.Name = "PropertiesRippingShowWindowCheckBox";
			this.PropertiesRippingShowWindowCheckBox.Size = new System.Drawing.Size(216, 24);
			this.PropertiesRippingShowWindowCheckBox.TabIndex = 9;
			this.PropertiesRippingShowWindowCheckBox.Text = "Show the command window when ripping";
			this.PropertiesRippingShowWindowCheckBox.ThreeState = true;
			this.PropertiesRippingShowWindowCheckBox.CheckStateChanged += new System.EventHandler(this.Properties_PropertyChanged);
			// 
			// PropertiesRippingConfigGroupBox
			// 
			this.PropertiesRippingConfigGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesRippingConfigGroupBox.Controls.Add(this.PropertiesRippingEditButton);
			this.PropertiesRippingConfigGroupBox.Controls.Add(this.mogArgumentsButton1);
			this.PropertiesRippingConfigGroupBox.Controls.Add(this.PropertiesRippingCopyToTempCheckBox);
			this.PropertiesRippingConfigGroupBox.Controls.Add(this.PropertiesRippingLocalTempCheckBox);
			this.PropertiesRippingConfigGroupBox.Controls.Add(this.PropertiesUseTempCheckBox);
			this.PropertiesRippingConfigGroupBox.Controls.Add(this.PropertiesRippingAutoDetectCheckBox);
			this.PropertiesRippingConfigGroupBox.Controls.Add(this.PropertiesRippingRipperComboBox);
			this.PropertiesRippingConfigGroupBox.Controls.Add(this.PropertiesRippingSlaveTaskerComboBox);
			this.PropertiesRippingConfigGroupBox.Controls.Add(this.ExploreSlaveTaskerButton);
			this.PropertiesRippingConfigGroupBox.Controls.Add(this.ExploreRipperButton);
			this.PropertiesRippingConfigGroupBox.Controls.Add(this.label18);
			this.PropertiesRippingConfigGroupBox.Controls.Add(this.PropertiesRippingDivergentCheckBox);
			this.PropertiesRippingConfigGroupBox.Controls.Add(this.label19);
			this.PropertiesRippingConfigGroupBox.Location = new System.Drawing.Point(8, 8);
			this.PropertiesRippingConfigGroupBox.Name = "PropertiesRippingConfigGroupBox";
			this.PropertiesRippingConfigGroupBox.Size = new System.Drawing.Size(356, 208);
			this.PropertiesRippingConfigGroupBox.TabIndex = 0;
			this.PropertiesRippingConfigGroupBox.TabStop = false;
			this.PropertiesRippingConfigGroupBox.Text = "Ripper configuration";
			// 
			// PropertiesRippingEditButton
			// 
			this.PropertiesRippingEditButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesRippingEditButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesRippingEditButton.Location = new System.Drawing.Point(301, 22);
			this.PropertiesRippingEditButton.Name = "PropertiesRippingEditButton";
			this.PropertiesRippingEditButton.Size = new System.Drawing.Size(50, 23);
			this.PropertiesRippingEditButton.TabIndex = 8;
			this.PropertiesRippingEditButton.Text = "Edit";
			this.PropertiesRippingEditButton.Click += new System.EventHandler(this.PropertiesRippingEditButton_Click);
			// 
			// PropertiesRippingCopyToTempCheckBox
			// 
			this.PropertiesRippingCopyToTempCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesRippingCopyToTempCheckBox.Location = new System.Drawing.Point(72, 141);
			this.PropertiesRippingCopyToTempCheckBox.Name = "PropertiesRippingCopyToTempCheckBox";
			this.PropertiesRippingCopyToTempCheckBox.Size = new System.Drawing.Size(224, 24);
			this.PropertiesRippingCopyToTempCheckBox.TabIndex = 6;
			this.PropertiesRippingCopyToTempCheckBox.Text = "Copy files to be ripped to temp rip directory";
			this.PropertiesRippingCopyToTempCheckBox.ThreeState = true;
			this.PropertiesRippingCopyToTempCheckBox.CheckStateChanged += new System.EventHandler(this.Properties_PropertyChanged);
			// 
			// PropertiesRippingLocalTempCheckBox
			// 
			this.PropertiesRippingLocalTempCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesRippingLocalTempCheckBox.Location = new System.Drawing.Point(72, 117);
			this.PropertiesRippingLocalTempCheckBox.Name = "PropertiesRippingLocalTempCheckBox";
			this.PropertiesRippingLocalTempCheckBox.Size = new System.Drawing.Size(208, 24);
			this.PropertiesRippingLocalTempCheckBox.TabIndex = 5;
			this.PropertiesRippingLocalTempCheckBox.Text = "Make temp rip directory a local directory";
			this.PropertiesRippingLocalTempCheckBox.ThreeState = true;
			this.PropertiesRippingLocalTempCheckBox.CheckStateChanged += new System.EventHandler(this.Properties_PropertyChanged);
			// 
			// PropertiesUseTempCheckBox
			// 
			this.PropertiesUseTempCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesUseTempCheckBox.Location = new System.Drawing.Point(40, 93);
			this.PropertiesUseTempCheckBox.Name = "PropertiesUseTempCheckBox";
			this.PropertiesUseTempCheckBox.Size = new System.Drawing.Size(128, 24);
			this.PropertiesUseTempCheckBox.TabIndex = 4;
			this.PropertiesUseTempCheckBox.Text = "Use temp rip directory";
			this.PropertiesUseTempCheckBox.ThreeState = true;
			this.PropertiesUseTempCheckBox.CheckStateChanged += new System.EventHandler(this.Properties_PropertyChanged);
			// 
			// PropertiesRippingAutoDetectCheckBox
			// 
			this.PropertiesRippingAutoDetectCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesRippingAutoDetectCheckBox.Location = new System.Drawing.Point(40, 69);
			this.PropertiesRippingAutoDetectCheckBox.Name = "PropertiesRippingAutoDetectCheckBox";
			this.PropertiesRippingAutoDetectCheckBox.Size = new System.Drawing.Size(128, 24);
			this.PropertiesRippingAutoDetectCheckBox.TabIndex = 3;
			this.PropertiesRippingAutoDetectCheckBox.Text = "Auto detect ripped files";
			this.PropertiesRippingAutoDetectCheckBox.ThreeState = true;
			this.PropertiesRippingAutoDetectCheckBox.CheckStateChanged += new System.EventHandler(this.Properties_PropertyChanged);
			// 
			// PropertiesRippingRipperComboBox
			// 
			this.PropertiesRippingRipperComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesRippingRipperComboBox.Items.AddRange(new object[] {
            "None",
            "Inherited"});
			this.PropertiesRippingRipperComboBox.Location = new System.Drawing.Point(96, 23);
			this.PropertiesRippingRipperComboBox.Name = "PropertiesRippingRipperComboBox";
			this.PropertiesRippingRipperComboBox.Size = new System.Drawing.Size(152, 21);
			this.PropertiesRippingRipperComboBox.TabIndex = 1;
			this.PropertiesRippingRipperComboBox.Leave += new System.EventHandler(this.PropertiesRippingRipperComboBox_Leave);
			this.PropertiesRippingRipperComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesRippingRipperComboBox_Validating);
			this.PropertiesRippingRipperComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesRippingRipperComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			// 
			// PropertiesRippingSlaveTaskerComboBox
			// 
			this.PropertiesRippingSlaveTaskerComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesRippingSlaveTaskerComboBox.Items.AddRange(new object[] {
            "None",
            "Inherited"});
			this.PropertiesRippingSlaveTaskerComboBox.Location = new System.Drawing.Point(128, 176);
			this.PropertiesRippingSlaveTaskerComboBox.Name = "PropertiesRippingSlaveTaskerComboBox";
			this.PropertiesRippingSlaveTaskerComboBox.Size = new System.Drawing.Size(192, 21);
			this.PropertiesRippingSlaveTaskerComboBox.TabIndex = 7;
			this.PropertiesRippingSlaveTaskerComboBox.Leave += new System.EventHandler(this.PropertiesRippingSlaveTaskerComboBox_Leave);
			this.PropertiesRippingSlaveTaskerComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.PropertiesRippingSlaveTaskerComboBox_Validating);
			this.PropertiesRippingSlaveTaskerComboBox.Validated += new System.EventHandler(this.Properties_PropertyChanged);
			this.PropertiesRippingSlaveTaskerComboBox.Click += new System.EventHandler(this.PropertiesActivateControl_Click);
			// 
			// ExploreSlaveTaskerButton
			// 
			this.ExploreSlaveTaskerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ExploreSlaveTaskerButton.Location = new System.Drawing.Point(324, 175);
			this.ExploreSlaveTaskerButton.Name = "ExploreSlaveTaskerButton";
			this.ExploreSlaveTaskerButton.Size = new System.Drawing.Size(24, 23);
			this.ExploreSlaveTaskerButton.TabIndex = 6;
			this.ExploreSlaveTaskerButton.Text = "...";
			this.ExploreSlaveTaskerButton.Click += new System.EventHandler(this.ExploreSlaveTaskerButton_Click);
			// 
			// ExploreRipperButton
			// 
			this.ExploreRipperButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ExploreRipperButton.Location = new System.Drawing.Point(248, 22);
			this.ExploreRipperButton.Name = "ExploreRipperButton";
			this.ExploreRipperButton.Size = new System.Drawing.Size(24, 23);
			this.ExploreRipperButton.TabIndex = 5;
			this.ExploreRipperButton.Text = "...";
			this.ExploreRipperButton.Click += new System.EventHandler(this.ExploreRipperButton_Click);
			// 
			// label18
			// 
			this.label18.Image = ((System.Drawing.Image)(resources.GetObject("label18.Image")));
			this.label18.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label18.Location = new System.Drawing.Point(16, 24);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(96, 16);
			this.label18.TabIndex = 0;
			this.label18.Text = "      Ripper tool:";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// PropertiesRippingDivergentCheckBox
			// 
			this.PropertiesRippingDivergentCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesRippingDivergentCheckBox.Location = new System.Drawing.Point(40, 45);
			this.PropertiesRippingDivergentCheckBox.Name = "PropertiesRippingDivergentCheckBox";
			this.PropertiesRippingDivergentCheckBox.Size = new System.Drawing.Size(200, 24);
			this.PropertiesRippingDivergentCheckBox.TabIndex = 2;
			this.PropertiesRippingDivergentCheckBox.Text = "This asset rips for divergent platforms";
			this.PropertiesRippingDivergentCheckBox.ThreeState = true;
			this.PropertiesRippingDivergentCheckBox.CheckStateChanged += new System.EventHandler(this.Properties_PropertyChanged);
			// 
			// label19
			// 
			this.label19.Image = ((System.Drawing.Image)(resources.GetObject("label19.Image")));
			this.label19.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label19.Location = new System.Drawing.Point(16, 179);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(120, 16);
			this.label19.TabIndex = 2;
			this.label19.Text = "      Slave Tasker tool:";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// RipperPanel
			// 
			this.RipperPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.RipperPanel.AutoScroll = true;
			this.RipperPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.RipperPanel.Controls.Add(this.PropertiesRippingTestButton);
			this.RipperPanel.Controls.Add(this.PropertiesRippingConfigGroupBox);
			this.RipperPanel.Controls.Add(this.PropertiesRippingSlavesGroupBox);
			this.RipperPanel.Location = new System.Drawing.Point(0, 8);
			this.RipperPanel.Name = "RipperPanel";
			this.RipperPanel.Size = new System.Drawing.Size(376, 360);
			this.RipperPanel.TabIndex = 10;
			// 
			// PropertiesRippingTestButton
			// 
			this.PropertiesRippingTestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesRippingTestButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesRippingTestButton.Location = new System.Drawing.Point(288, 328);
			this.PropertiesRippingTestButton.Name = "PropertiesRippingTestButton";
			this.PropertiesRippingTestButton.Size = new System.Drawing.Size(75, 23);
			this.PropertiesRippingTestButton.TabIndex = 9;
			this.PropertiesRippingTestButton.Text = "Test";
			this.PropertiesRippingTestButton.Click += new System.EventHandler(this.PropertiesRippingTestButton_Click);
			// 
			// mogArgumentsButton1
			// 
			this.mogArgumentsButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mogArgumentsButton1.ButtonText = ">";
			this.mogArgumentsButton1.Location = new System.Drawing.Point(274, 22);
			this.mogArgumentsButton1.MOGAssetFilename = null;
			this.mogArgumentsButton1.Name = "mogArgumentsButton1";
			this.mogArgumentsButton1.Size = new System.Drawing.Size(24, 23);
			this.mogArgumentsButton1.TabIndex = 8;
			this.mogArgumentsButton1.TargetComboBox = null;
			this.mogArgumentsButton1.TargetTextBox = null;
			this.mogArgumentsButton1.MogContextMenuItemClick += new System.EventHandler(this.mogArgumentsButton1_MogContextMenuItemClick);
			// 
			// RippersControl
			// 
			this.Controls.Add(this.RipperPanel);
			this.Name = "RippersControl";
			this.Size = new System.Drawing.Size(376, 368);
			this.PropertiesRippingSlavesGroupBox.ResumeLayout(false);
			this.PropertiesRippingConfigGroupBox.ResumeLayout(false);
			this.RipperPanel.ResumeLayout(false);
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

		public void InitializeRipperInfo(MOG_Properties properties, MOG_Filename currentFile)
		{
			mDisableEvents = true;
			// Put in the Multi-asset commands
			SetComboBoxProperty(this.PropertiesRippingRipperComboBox, properties.AssetRipper, false);
			SetComboBoxProperty(this.PropertiesRippingSlaveTaskerComboBox, properties.AssetRipTasker, false);
			SetComboBoxProperty(this.PropertiesRippingValidSlavesComboBox, properties.ValidSlaves, false);

			SetCheckBoxProperty(this.PropertiesRippingDivergentCheckBox, properties.DivergentPlatformDataType_InheritedBoolean);
			SetCheckBoxProperty(this.PropertiesRippingShowWindowCheckBox, properties.ShowRipCommandWindow_InheritedBoolean);
			SetCheckBoxProperty(this.PropertiesRippingAutoDetectCheckBox, properties.AutoDetectRippedFiles_InheritedBoolean);
			SetCheckBoxProperty(this.PropertiesRippingCopyToTempCheckBox, properties.CopyFilesIntoTempRipDir_InheritedBoolean);
			SetCheckBoxProperty(this.PropertiesRippingLocalTempCheckBox, properties.UseLocalTempRipDir_InheritedBoolean);
			SetCheckBoxProperty(this.PropertiesUseTempCheckBox, properties.UseTempRipDir_InheritedBoolean);

			this.mogArgumentsButton1.MOGAssetFilename = currentFile;
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
						//SetComboBoxProperty(this.prop, setting.Setting, true);
						break;
				}
			}
		}

		public void SaveRipperChanges(MOG_Properties properties)
		{			
			switch(this.PropertiesRippingDivergentCheckBox.CheckState)
			{
				case CheckState.Checked:
					properties.DivergentPlatformDataType_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.True;
					break;
				case CheckState.Unchecked:
					properties.DivergentPlatformDataType_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.False;
					break;
				case CheckState.Indeterminate:
					properties.DivergentPlatformDataType_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.Inherited;
					break;
			}

			switch(this.PropertiesRippingShowWindowCheckBox.CheckState)
			{
				case CheckState.Checked:
					properties.ShowRipCommandWindow_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.True;
					break;
				case CheckState.Unchecked:
					properties.ShowRipCommandWindow_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.False;
					break;
				case CheckState.Indeterminate:
					properties.ShowRipCommandWindow_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.Inherited;
					break;
			}

			switch(this.PropertiesRippingAutoDetectCheckBox.CheckState)
			{
				case CheckState.Checked:
					properties.AutoDetectRippedFiles_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.True;
					break;
				case CheckState.Unchecked:
					properties.AutoDetectRippedFiles_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.False;
					break;
				case CheckState.Indeterminate:
					properties.AutoDetectRippedFiles_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.Inherited;
					break;
			}

			switch(this.PropertiesRippingCopyToTempCheckBox.CheckState)
			{
				case CheckState.Checked:
					properties.CopyFilesIntoTempRipDir_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.True;
					break;
				case CheckState.Unchecked:
					properties.CopyFilesIntoTempRipDir_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.False;
					break;
				case CheckState.Indeterminate:
					properties.CopyFilesIntoTempRipDir_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.Inherited;
					break;
			}

			switch(this.PropertiesRippingLocalTempCheckBox.CheckState)
			{
				case CheckState.Checked:
					properties.UseLocalTempRipDir_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.True;
					break;
				case CheckState.Unchecked:
					properties.UseLocalTempRipDir_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.False;
					break;
				case CheckState.Indeterminate:
					properties.UseLocalTempRipDir_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.Inherited;
					break;
			}

			switch(this.PropertiesUseTempCheckBox.CheckState)
			{
				case CheckState.Checked:
					properties.UseTempRipDir_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.True;
					break;
				case CheckState.Unchecked:
					properties.UseTempRipDir_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.False;
					break;
				case CheckState.Indeterminate:
					properties.UseTempRipDir_InheritedBoolean = MOG.PROPERTIES.MOG_InheritedBoolean.Inherited;
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

		#region ComboBox edited and validated events
		private void PropertiesActivateControl_Click(object sender, System.EventArgs e)
		{
			this.ActiveControl = sender as Control;
		}

		private void PropertiesRippingRipperComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.AssetRipper = this.PropertiesRippingRipperComboBox.Text;
			}
		}

		private void PropertiesRippingRipperComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SetComboBoxProperty(this.PropertiesRippingRipperComboBox, properties.AssetRipper, true);
			}
		}

		private void PropertiesRippingSlaveTaskerComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.AssetRipTasker = this.PropertiesRippingSlaveTaskerComboBox.Text;
			}
		}

		private void PropertiesRippingSlaveTaskerComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SetComboBoxProperty(this.PropertiesRippingSlaveTaskerComboBox, properties.AssetRipTasker, true);
			}
		}

		private void PropertiesRippingValidSlavesComboBox_Leave(object sender, System.EventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.ValidSlaves = this.PropertiesRippingValidSlavesComboBox.Text;
			}
		}

		private void PropertiesRippingValidSlavesComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SetComboBoxProperty(this.PropertiesRippingValidSlavesComboBox, properties.ValidSlaves, true);
			}
		}
		#endregion

		private void ExploreRipperButton_Click(object sender, System.EventArgs e)
		{
			string ripper = mParent.GetValueForProperty("PackagePostMergeEvent", PropertiesRippingRipperComboBox.Text) as string;

			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.AssetRipper = ripper;
				PropertiesRippingRipperComboBox.Text = properties.AssetRipper;
			}
		}

		private void ExploreSlaveTaskerButton_Click(object sender, System.EventArgs e)
		{
			string tasker = mParent.GetValueForProperty("PackagePostMergeEvent", PropertiesRippingSlaveTaskerComboBox.Text) as string;

			foreach (MOG_Properties properties in this.PropertiesList)
			{
				properties.AssetRipTasker = tasker;
				PropertiesRippingSlaveTaskerComboBox.Text = properties.AssetRipTasker;
			}
		}

		/// <summary>
		/// Edit the ripper in notepad
		/// </summary>
		private void EditRipper()
		{
			if (this.PropertiesList.Length > 0)
			{
				MOG_Properties properties = PropertiesList[0] as MOG_Properties;

				// Get the assigned ripper information
                string ripper = properties.AssetRipper;
                string tool = DosUtils.FileStripArguments(properties.AssetRipper);
                string args = DosUtils.FileGetArguments(properties.AssetRipper);

				// Do we have a ripper to edit?
				if (ripper.Length > 0)
				{
					// Locate the ripper
					ripper = MOG_ControllerSystem.LocateTool("", tool);
					if (ripper.Length > 0)
					{
						// Edit the ripper
						guiCommandLine.ShellSpawn("Notepad.exe", ripper);
					}
					else
					{
						// If not, do we want to create one?
						if (MOG_Prompt.PromptResponse("No Ripper Found", "This appears to be a new ripper.\n" +
																		 "RIPPER: " + tool + "\n\n" +
																		 "Would you like to create a new one for editing?", MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
						{
							// Create a new one using the user's specified name
							string defaultRipperName = tool;
							ripper = Path.Combine(MOG_ControllerProject.GetProject().GetProjectToolsPath(), defaultRipperName);

							// Do we want to start from the template?
							if (MOG_Prompt.PromptResponse("Use Ripper Template", "Would you like to base the new ripper from MOG's ripper template?", MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
							{
								// If so, copy the template to this new ripper
								string template = MOG_ControllerSystem.GetSystemRepositoryPath() + "\\Tools\\Rippers\\TemplateComplex_ripper.bat";

								DosUtils.CopyFast(template, ripper, false);
							}

							// Launch the editor
							string output = "";
							guiCommandLine.ShellExecute("Notepad.exe", ripper, System.Diagnostics.ProcessWindowStyle.Normal, ref output);

							// Now, set this new ripper as the ripper assigned to this asset restoring the users args
							properties.AssetRipper = ripper + " " + args;
							this.PropertiesRippingRipperComboBox.Text = properties.AssetRipper;
						}
					}
				}
				else
				{
					MOG_Prompt.PromptMessage("Edit Ripper", "There is no ripper to edit.");
				}
			}
		}

		private void PropertiesRippingEditButton_Click(object sender, System.EventArgs e)
		{
			EditRipper();
		}

		private void mogArgumentsButton1_MogContextMenuItemClick(object sender, System.EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			this.PropertiesRippingRipperComboBox.Text += mogArgumentsButton1.GetArgument(item);
		}

		private void PropertiesRippingTestButton_Click(object sender, EventArgs e)
		{
			// Loop through all the selected properties
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				SaveRipperChanges(properties);

				// Perform the ripper test
				MOG_ControllerRipper ripper = new MOG_ControllerRipper(properties.GetAssetFilename());
				if (!ripper.LocalRipper(properties, ""))
				{
					// Inform the user that the rip failed...
					// All needed rip information and logs can be found within the asset
				}
			}
		}
	}
}
