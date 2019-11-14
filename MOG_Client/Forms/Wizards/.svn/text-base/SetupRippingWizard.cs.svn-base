using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;

using MOG.FILENAME;
using MOG.DOSUTILS;
using MOG.PROMPT;
using MOG.REPORT;
using MOG.PROPERTIES;
using MOG.UITYPESEDITORS;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERRIPPER;

using MOG_Client.Client_Mog_Utilities;
using System.IO;

namespace MOG_Client.Forms.AssetProperties
{
	/// <summary>
	/// Summary description for SetupRippingWizard.
	/// </summary>
	public class SetupRippingWizard : System.Windows.Forms.Form
	{
		public MOG_Filename assetName;
		public ArrayList assetNames = new ArrayList();
		public ArrayList PropertiesList = new ArrayList();
		public bool ShowAdvancedWizard = false;

		#region Windows Form variables
		private Gui.Wizard.Wizard SetupRipperWizard;
		private Gui.Wizard.InfoPage infoPage1;
		private Gui.Wizard.Header header1;
		private System.Windows.Forms.GroupBox groupBox1;
		private Gui.Wizard.Header header2;
		private Gui.Wizard.Header header3;
		private System.Windows.Forms.GroupBox groupBox2;
		private Gui.Wizard.Header header4;
		private System.Windows.Forms.GroupBox groupBox3;
		private Gui.Wizard.Header header5;
		private System.Windows.Forms.GroupBox groupBox4;
		private Gui.Wizard.Header header6;
		private System.Windows.Forms.GroupBox groupBox5;
		private Gui.Wizard.Header header7;
		private System.Windows.Forms.GroupBox groupBox6;
		private Gui.Wizard.Header header8;
		public System.Windows.Forms.TextBox WizardValidSlavesTextBox;
		private System.Windows.Forms.Label label1;
		private Gui.Wizard.InfoPage infoPage2;
		public System.Windows.Forms.TextBox WizardRipperTextBox;
		public System.Windows.Forms.RadioButton WizardDivergentYesRadioButton;
		public System.Windows.Forms.RadioButton WizardAutoDetectYesRadioButton;
		public System.Windows.Forms.RadioButton WizardTempDirYesRadioButton;
		public System.Windows.Forms.RadioButton WizardLocalYesRadioButton;
		public System.Windows.Forms.RadioButton WizardCopyAssetYesRadioButton;
		public System.Windows.Forms.RadioButton WizardShowCommandYesRadioButton;
		private System.Windows.Forms.Button ExploreRipperButton;
		private System.Windows.Forms.Label label2;
		private Gui.Wizard.WizardPage WizFinishWizardPage;
		private Gui.Wizard.WizardPage WizSlavesWizardPage;
		private Gui.Wizard.WizardPage WizShowCmdWizardPage;
		private Gui.Wizard.WizardPage WizDuplicateWizardPage;
		private Gui.Wizard.WizardPage WizLocalTempWizardPage;
		private Gui.Wizard.WizardPage WizTempWizardPage;
		private Gui.Wizard.WizardPage WizAutoDetectWizardPage;
		private Gui.Wizard.WizardPage WizDivergentWizardPage;
		private Gui.Wizard.WizardPage WizRipperWizardPage;
		private Gui.Wizard.WizardPage WizStartWizardPage;
		private Gui.Wizard.WizardPage WizAdvancedWizardPage;
		private Gui.Wizard.Header header9;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.RadioButton WizardAdvancedNoRadioButton;
		private System.Windows.Forms.RadioButton WizardTempDirNoRadioButton;
		private System.Windows.Forms.RadioButton WizardShowCommandNoRadioButton;
		private System.Windows.Forms.RadioButton WizardCopyAssetNoRadioButton;
		private System.Windows.Forms.RadioButton WizardLocalNoRadioButton;
		private System.Windows.Forms.RadioButton WizardAutoDetectNoRadioButton;
		private System.Windows.Forms.RadioButton WizardDivergentNoRadioButton;
		public System.Windows.Forms.RadioButton WizardAdvancedYesRadioButton;
		private Gui.Wizard.WizardPage WizRipperTypeWizardPage;
		private Gui.Wizard.Header header10;
		private Gui.Wizard.Header header11;
		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.Button PropertiesRippingCreateButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox PropertiesRippingBatchNameTextBox;
		private System.Windows.Forms.ToolTip MOGToolTip;
		private System.Windows.Forms.Button PropertiesBatchRipperEditButton;
		private System.Windows.Forms.RadioButton WizardRiperTypeOtherRadioButton;
		private System.Windows.Forms.RadioButton WizardRiperTypeExeRadioButton;
		private System.Windows.Forms.RadioButton WizardRiperTypeBatchRadioButton;
		private System.Windows.Forms.Label label4;
		private Gui.Wizard.WizardPage WizBatchRipperCreateWizardPage;
		private MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox WizardExeRipperArgsMogControl_ViewTokenTextBox;
		private MOG_Client.Forms.ArgumentControl.MogArgumentsButton WizardExeRipperArgsMogArgumentsButton;
		private System.Windows.Forms.Button WizardRipperTestButton;
		private System.Windows.Forms.Button PropertiesBatchRipperTestButton;
		private MOG_Client.Forms.ArgumentControl.MogArgumentsButton PropertiesRippingBatchMogArgumentsButton;
		private Button PropertiesRippingBatchBrowseButton;
		private System.ComponentModel.IContainer components;
		#endregion

		public SetupRippingWizard()
		{
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupRippingWizard));
			this.MOGToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SetupRipperWizard = new Gui.Wizard.Wizard();
			this.WizRipperTypeWizardPage = new Gui.Wizard.WizardPage();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.WizardRiperTypeOtherRadioButton = new System.Windows.Forms.RadioButton();
			this.WizardRiperTypeExeRadioButton = new System.Windows.Forms.RadioButton();
			this.WizardRiperTypeBatchRadioButton = new System.Windows.Forms.RadioButton();
			this.header11 = new Gui.Wizard.Header();
			this.WizBatchRipperCreateWizardPage = new Gui.Wizard.WizardPage();
			this.PropertiesRippingBatchMogArgumentsButton = new MOG_Client.Forms.ArgumentControl.MogArgumentsButton();
			this.PropertiesRippingBatchNameTextBox = new System.Windows.Forms.TextBox();
			this.PropertiesRippingBatchBrowseButton = new System.Windows.Forms.Button();
			this.PropertiesBatchRipperTestButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.PropertiesBatchRipperEditButton = new System.Windows.Forms.Button();
			this.PropertiesRippingCreateButton = new System.Windows.Forms.Button();
			this.header10 = new Gui.Wizard.Header();
			this.WizRipperWizardPage = new Gui.Wizard.WizardPage();
			this.WizardRipperTestButton = new System.Windows.Forms.Button();
			this.WizardExeRipperArgsMogControl_ViewTokenTextBox = new MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox();
			this.WizardRipperTextBox = new System.Windows.Forms.TextBox();
			this.WizardExeRipperArgsMogArgumentsButton = new MOG_Client.Forms.ArgumentControl.MogArgumentsButton();
			this.label2 = new System.Windows.Forms.Label();
			this.ExploreRipperButton = new System.Windows.Forms.Button();
			this.header1 = new Gui.Wizard.Header();
			this.WizAdvancedWizardPage = new Gui.Wizard.WizardPage();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.WizardAdvancedNoRadioButton = new System.Windows.Forms.RadioButton();
			this.WizardAdvancedYesRadioButton = new System.Windows.Forms.RadioButton();
			this.header9 = new Gui.Wizard.Header();
			this.WizStartWizardPage = new Gui.Wizard.WizardPage();
			this.infoPage1 = new Gui.Wizard.InfoPage();
			this.WizFinishWizardPage = new Gui.Wizard.WizardPage();
			this.infoPage2 = new Gui.Wizard.InfoPage();
			this.WizSlavesWizardPage = new Gui.Wizard.WizardPage();
			this.label1 = new System.Windows.Forms.Label();
			this.WizardValidSlavesTextBox = new System.Windows.Forms.TextBox();
			this.header8 = new Gui.Wizard.Header();
			this.WizShowCmdWizardPage = new Gui.Wizard.WizardPage();
			this.header7 = new Gui.Wizard.Header();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.WizardShowCommandNoRadioButton = new System.Windows.Forms.RadioButton();
			this.WizardShowCommandYesRadioButton = new System.Windows.Forms.RadioButton();
			this.WizDuplicateWizardPage = new Gui.Wizard.WizardPage();
			this.header6 = new Gui.Wizard.Header();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.WizardCopyAssetNoRadioButton = new System.Windows.Forms.RadioButton();
			this.WizardCopyAssetYesRadioButton = new System.Windows.Forms.RadioButton();
			this.WizLocalTempWizardPage = new Gui.Wizard.WizardPage();
			this.header5 = new Gui.Wizard.Header();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.WizardLocalNoRadioButton = new System.Windows.Forms.RadioButton();
			this.WizardLocalYesRadioButton = new System.Windows.Forms.RadioButton();
			this.WizTempWizardPage = new Gui.Wizard.WizardPage();
			this.header4 = new Gui.Wizard.Header();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.WizardTempDirNoRadioButton = new System.Windows.Forms.RadioButton();
			this.WizardTempDirYesRadioButton = new System.Windows.Forms.RadioButton();
			this.WizAutoDetectWizardPage = new Gui.Wizard.WizardPage();
			this.header3 = new Gui.Wizard.Header();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.WizardAutoDetectNoRadioButton = new System.Windows.Forms.RadioButton();
			this.WizardAutoDetectYesRadioButton = new System.Windows.Forms.RadioButton();
			this.WizDivergentWizardPage = new Gui.Wizard.WizardPage();
			this.header2 = new Gui.Wizard.Header();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.WizardDivergentNoRadioButton = new System.Windows.Forms.RadioButton();
			this.WizardDivergentYesRadioButton = new System.Windows.Forms.RadioButton();
			this.SetupRipperWizard.SuspendLayout();
			this.WizRipperTypeWizardPage.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.WizBatchRipperCreateWizardPage.SuspendLayout();
			this.WizRipperWizardPage.SuspendLayout();
			this.WizAdvancedWizardPage.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.WizStartWizardPage.SuspendLayout();
			this.WizFinishWizardPage.SuspendLayout();
			this.WizSlavesWizardPage.SuspendLayout();
			this.WizShowCmdWizardPage.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.WizDuplicateWizardPage.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.WizLocalTempWizardPage.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.WizTempWizardPage.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.WizAutoDetectWizardPage.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.WizDivergentWizardPage.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// SetupRipperWizard
			// 
			this.SetupRipperWizard.Controls.Add(this.WizRipperWizardPage);
			this.SetupRipperWizard.Controls.Add(this.WizBatchRipperCreateWizardPage);
			this.SetupRipperWizard.Controls.Add(this.WizRipperTypeWizardPage);
			this.SetupRipperWizard.Controls.Add(this.WizStartWizardPage);
			this.SetupRipperWizard.Controls.Add(this.WizFinishWizardPage);
			this.SetupRipperWizard.Controls.Add(this.WizSlavesWizardPage);
			this.SetupRipperWizard.Controls.Add(this.WizShowCmdWizardPage);
			this.SetupRipperWizard.Controls.Add(this.WizDuplicateWizardPage);
			this.SetupRipperWizard.Controls.Add(this.WizLocalTempWizardPage);
			this.SetupRipperWizard.Controls.Add(this.WizTempWizardPage);
			this.SetupRipperWizard.Controls.Add(this.WizAutoDetectWizardPage);
			this.SetupRipperWizard.Controls.Add(this.WizDivergentWizardPage);
			this.SetupRipperWizard.Controls.Add(this.WizAdvancedWizardPage);
			this.SetupRipperWizard.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SetupRipperWizard.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SetupRipperWizard.Location = new System.Drawing.Point(0, 0);
			this.SetupRipperWizard.Name = "SetupRipperWizard";
			this.SetupRipperWizard.Pages.AddRange(new Gui.Wizard.WizardPage[] {
            this.WizStartWizardPage,
            this.WizRipperTypeWizardPage,
            this.WizBatchRipperCreateWizardPage,
            this.WizRipperWizardPage,
            this.WizAdvancedWizardPage,
            this.WizDivergentWizardPage,
            this.WizAutoDetectWizardPage,
            this.WizTempWizardPage,
            this.WizLocalTempWizardPage,
            this.WizDuplicateWizardPage,
            this.WizShowCmdWizardPage,
            this.WizSlavesWizardPage,
            this.WizFinishWizardPage});
			this.SetupRipperWizard.PushPop = true;
			this.SetupRipperWizard.Size = new System.Drawing.Size(438, 348);
			this.SetupRipperWizard.TabIndex = 0;
			// 
			// WizRipperTypeWizardPage
			// 
			this.WizRipperTypeWizardPage.Controls.Add(this.label4);
			this.WizRipperTypeWizardPage.Controls.Add(this.groupBox8);
			this.WizRipperTypeWizardPage.Controls.Add(this.header11);
			this.WizRipperTypeWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WizRipperTypeWizardPage.IsFinishPage = false;
			this.WizRipperTypeWizardPage.Location = new System.Drawing.Point(0, 0);
			this.WizRipperTypeWizardPage.Name = "WizRipperTypeWizardPage";
			this.WizRipperTypeWizardPage.Size = new System.Drawing.Size(438, 300);
			this.WizRipperTypeWizardPage.TabIndex = 12;
			this.WizRipperTypeWizardPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.WizRipperTypeWizardPage_CloseFromNext);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(24, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(368, 40);
			this.label4.TabIndex = 3;
			this.label4.Text = "Select a desired ripper type for this asset.  This type will determine if MOG wil" +
				"l shell spawn a script or try to run the ripper directly.";
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.WizardRiperTypeOtherRadioButton);
			this.groupBox8.Controls.Add(this.WizardRiperTypeExeRadioButton);
			this.groupBox8.Controls.Add(this.WizardRiperTypeBatchRadioButton);
			this.groupBox8.Location = new System.Drawing.Point(32, 64);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(200, 104);
			this.groupBox8.TabIndex = 2;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Select ripper type:";
			// 
			// WizardRiperTypeOtherRadioButton
			// 
			this.WizardRiperTypeOtherRadioButton.Enabled = false;
			this.WizardRiperTypeOtherRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardRiperTypeOtherRadioButton.Location = new System.Drawing.Point(24, 72);
			this.WizardRiperTypeOtherRadioButton.Name = "WizardRiperTypeOtherRadioButton";
			this.WizardRiperTypeOtherRadioButton.Size = new System.Drawing.Size(104, 24);
			this.WizardRiperTypeOtherRadioButton.TabIndex = 2;
			this.WizardRiperTypeOtherRadioButton.Text = "Other";
			// 
			// WizardRiperTypeExeRadioButton
			// 
			this.WizardRiperTypeExeRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardRiperTypeExeRadioButton.Location = new System.Drawing.Point(24, 48);
			this.WizardRiperTypeExeRadioButton.Name = "WizardRiperTypeExeRadioButton";
			this.WizardRiperTypeExeRadioButton.Size = new System.Drawing.Size(136, 24);
			this.WizardRiperTypeExeRadioButton.TabIndex = 1;
			this.WizardRiperTypeExeRadioButton.Text = "Executable (.Exe)";
			// 
			// WizardRiperTypeBatchRadioButton
			// 
			this.WizardRiperTypeBatchRadioButton.Checked = true;
			this.WizardRiperTypeBatchRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardRiperTypeBatchRadioButton.Location = new System.Drawing.Point(24, 24);
			this.WizardRiperTypeBatchRadioButton.Name = "WizardRiperTypeBatchRadioButton";
			this.WizardRiperTypeBatchRadioButton.Size = new System.Drawing.Size(104, 24);
			this.WizardRiperTypeBatchRadioButton.TabIndex = 0;
			this.WizardRiperTypeBatchRadioButton.TabStop = true;
			this.WizardRiperTypeBatchRadioButton.Text = "Batch (.Bat)";
			// 
			// header11
			// 
			this.header11.BackColor = System.Drawing.SystemColors.Control;
			this.header11.CausesValidation = false;
			this.header11.Description = "Select the type of ripper that will be used for this asset.";
			this.header11.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.header11.Image = ((System.Drawing.Image)(resources.GetObject("header11.Image")));
			this.header11.Location = new System.Drawing.Point(0, 236);
			this.header11.Name = "header11";
			this.header11.Size = new System.Drawing.Size(438, 64);
			this.header11.TabIndex = 1;
			this.header11.Title = "Ripper type";
			// 
			// WizBatchRipperCreateWizardPage
			// 
			this.WizBatchRipperCreateWizardPage.Controls.Add(this.PropertiesRippingBatchMogArgumentsButton);
			this.WizBatchRipperCreateWizardPage.Controls.Add(this.PropertiesRippingBatchBrowseButton);
			this.WizBatchRipperCreateWizardPage.Controls.Add(this.PropertiesBatchRipperTestButton);
			this.WizBatchRipperCreateWizardPage.Controls.Add(this.PropertiesRippingBatchNameTextBox);
			this.WizBatchRipperCreateWizardPage.Controls.Add(this.label3);
			this.WizBatchRipperCreateWizardPage.Controls.Add(this.PropertiesBatchRipperEditButton);
			this.WizBatchRipperCreateWizardPage.Controls.Add(this.PropertiesRippingCreateButton);
			this.WizBatchRipperCreateWizardPage.Controls.Add(this.header10);
			this.WizBatchRipperCreateWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WizBatchRipperCreateWizardPage.IsFinishPage = false;
			this.WizBatchRipperCreateWizardPage.Location = new System.Drawing.Point(0, 0);
			this.WizBatchRipperCreateWizardPage.Name = "WizBatchRipperCreateWizardPage";
			this.WizBatchRipperCreateWizardPage.Size = new System.Drawing.Size(438, 300);
			this.WizBatchRipperCreateWizardPage.TabIndex = 13;
			this.WizBatchRipperCreateWizardPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.WizBatchRipperCreateWizardPage_CloseFromNext);
			this.WizBatchRipperCreateWizardPage.ShowFromNext += new System.EventHandler(this.WizBatchRipperCreateWizardPage_ShowFromNext);
			// 
			// PropertiesRippingBatchMogArgumentsButton
			// 
			this.PropertiesRippingBatchMogArgumentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesRippingBatchMogArgumentsButton.ButtonText = ">";
			this.PropertiesRippingBatchMogArgumentsButton.Location = new System.Drawing.Point(406, 39);
			this.PropertiesRippingBatchMogArgumentsButton.MOGAssetFilename = null;
			this.PropertiesRippingBatchMogArgumentsButton.Name = "PropertiesRippingBatchMogArgumentsButton";
			this.PropertiesRippingBatchMogArgumentsButton.Size = new System.Drawing.Size(24, 23);
			this.PropertiesRippingBatchMogArgumentsButton.TabIndex = 34;
			this.PropertiesRippingBatchMogArgumentsButton.TargetComboBox = null;
			this.PropertiesRippingBatchMogArgumentsButton.TargetTextBox = this.PropertiesRippingBatchNameTextBox;
			// 
			// PropertiesRippingBatchNameTextBox
			// 
			this.PropertiesRippingBatchNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesRippingBatchNameTextBox.Location = new System.Drawing.Point(24, 40);
			this.PropertiesRippingBatchNameTextBox.Name = "PropertiesRippingBatchNameTextBox";
			this.PropertiesRippingBatchNameTextBox.Size = new System.Drawing.Size(350, 21);
			this.PropertiesRippingBatchNameTextBox.TabIndex = 15;
			this.PropertiesRippingBatchNameTextBox.Text = "Rippers\\NewRipper.bat";
			this.PropertiesRippingBatchNameTextBox.TextChanged += new System.EventHandler(this.PropertiesRippingBatchNameTextBox_TextChanged);
			// 
			// PropertiesRippingBatchBrowseButton
			// 
			this.PropertiesRippingBatchBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesRippingBatchBrowseButton.Location = new System.Drawing.Point(380, 39);
			this.PropertiesRippingBatchBrowseButton.Name = "PropertiesRippingBatchBrowseButton";
			this.PropertiesRippingBatchBrowseButton.Size = new System.Drawing.Size(24, 23);
			this.PropertiesRippingBatchBrowseButton.TabIndex = 33;
			this.PropertiesRippingBatchBrowseButton.Text = "...";
			this.PropertiesRippingBatchBrowseButton.Click += new System.EventHandler(this.PropertiesRippingBatchBrowseButton_Click);
			// 
			// PropertiesBatchRipperTestButton
			// 
			this.PropertiesBatchRipperTestButton.Enabled = false;
			this.PropertiesBatchRipperTestButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesBatchRipperTestButton.Location = new System.Drawing.Point(22, 160);
			this.PropertiesBatchRipperTestButton.Name = "PropertiesBatchRipperTestButton";
			this.PropertiesBatchRipperTestButton.Size = new System.Drawing.Size(160, 23);
			this.PropertiesBatchRipperTestButton.TabIndex = 32;
			this.PropertiesBatchRipperTestButton.Text = "4. Test";
			this.PropertiesBatchRipperTestButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.PropertiesBatchRipperTestButton.Click += new System.EventHandler(this.PropertiesBatchRipperTestButton_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(368, 23);
			this.label3.TabIndex = 14;
			this.label3.Text = "1. What name do you want to assign to this new batch ripper?";
			// 
			// PropertiesBatchRipperEditButton
			// 
			this.PropertiesBatchRipperEditButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesBatchRipperEditButton.Enabled = false;
			this.PropertiesBatchRipperEditButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesBatchRipperEditButton.Location = new System.Drawing.Point(20, 120);
			this.PropertiesBatchRipperEditButton.Name = "PropertiesBatchRipperEditButton";
			this.PropertiesBatchRipperEditButton.Size = new System.Drawing.Size(160, 21);
			this.PropertiesBatchRipperEditButton.TabIndex = 13;
			this.PropertiesBatchRipperEditButton.Text = "3. Edit ripper...";
			this.PropertiesBatchRipperEditButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.PropertiesBatchRipperEditButton.Click += new System.EventHandler(this.PropertiesBatchRipperEditButton_Click);
			// 
			// PropertiesRippingCreateButton
			// 
			this.PropertiesRippingCreateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertiesRippingCreateButton.Enabled = false;
			this.PropertiesRippingCreateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PropertiesRippingCreateButton.Location = new System.Drawing.Point(20, 80);
			this.PropertiesRippingCreateButton.Name = "PropertiesRippingCreateButton";
			this.PropertiesRippingCreateButton.Size = new System.Drawing.Size(160, 21);
			this.PropertiesRippingCreateButton.TabIndex = 12;
			this.PropertiesRippingCreateButton.Text = "2. Create new ripper";
			this.PropertiesRippingCreateButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.PropertiesRippingCreateButton.Click += new System.EventHandler(this.PropertiesRippingCreateButton_Click);
			// 
			// header10
			// 
			this.header10.BackColor = System.Drawing.SystemColors.Control;
			this.header10.CausesValidation = false;
			this.header10.Description = "Enter the desired name for this ripper.";
			this.header10.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.header10.Image = ((System.Drawing.Image)(resources.GetObject("header10.Image")));
			this.header10.Location = new System.Drawing.Point(0, 236);
			this.header10.Name = "header10";
			this.header10.Size = new System.Drawing.Size(438, 64);
			this.header10.TabIndex = 1;
			this.header10.Title = "Batch ripper name";
			// 
			// WizRipperWizardPage
			// 
			this.WizRipperWizardPage.Controls.Add(this.WizardRipperTestButton);
			this.WizRipperWizardPage.Controls.Add(this.WizardExeRipperArgsMogControl_ViewTokenTextBox);
			this.WizRipperWizardPage.Controls.Add(this.WizardExeRipperArgsMogArgumentsButton);
			this.WizRipperWizardPage.Controls.Add(this.label2);
			this.WizRipperWizardPage.Controls.Add(this.ExploreRipperButton);
			this.WizRipperWizardPage.Controls.Add(this.WizardRipperTextBox);
			this.WizRipperWizardPage.Controls.Add(this.header1);
			this.WizRipperWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WizRipperWizardPage.IsFinishPage = false;
			this.WizRipperWizardPage.Location = new System.Drawing.Point(0, 0);
			this.WizRipperWizardPage.Name = "WizRipperWizardPage";
			this.WizRipperWizardPage.Size = new System.Drawing.Size(438, 300);
			this.WizRipperWizardPage.TabIndex = 2;
			// 
			// WizardRipperTestButton
			// 
			this.WizardRipperTestButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardRipperTestButton.Location = new System.Drawing.Point(16, 96);
			this.WizardRipperTestButton.Name = "WizardRipperTestButton";
			this.WizardRipperTestButton.Size = new System.Drawing.Size(75, 23);
			this.WizardRipperTestButton.TabIndex = 31;
			this.WizardRipperTestButton.Text = "Test";
			this.WizardRipperTestButton.Click += new System.EventHandler(this.WizardRipperTestButton_Click);
			// 
			// WizardExeRipperArgsMogControl_ViewTokenTextBox
			// 
			this.WizardExeRipperArgsMogControl_ViewTokenTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.WizardExeRipperArgsMogControl_ViewTokenTextBox.Location = new System.Drawing.Point(16, 64);
			this.WizardExeRipperArgsMogControl_ViewTokenTextBox.MOGAssetFilename = null;
			this.WizardExeRipperArgsMogControl_ViewTokenTextBox.Name = "WizardExeRipperArgsMogControl_ViewTokenTextBox";
			this.WizardExeRipperArgsMogControl_ViewTokenTextBox.Size = new System.Drawing.Size(358, 24);
			this.WizardExeRipperArgsMogControl_ViewTokenTextBox.SourceComboBox = null;
			this.WizardExeRipperArgsMogControl_ViewTokenTextBox.SourceTextBox = this.WizardRipperTextBox;
			this.WizardExeRipperArgsMogControl_ViewTokenTextBox.TabIndex = 30;
			// 
			// WizardRipperTextBox
			// 
			this.WizardRipperTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.WizardRipperTextBox.Location = new System.Drawing.Point(16, 40);
			this.WizardRipperTextBox.Name = "WizardRipperTextBox";
			this.WizardRipperTextBox.Size = new System.Drawing.Size(358, 21);
			this.WizardRipperTextBox.TabIndex = 2;
			this.WizardRipperTextBox.Text = "MyTool.exe";
			// 
			// WizardExeRipperArgsMogArgumentsButton
			// 
			this.WizardExeRipperArgsMogArgumentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.WizardExeRipperArgsMogArgumentsButton.ButtonText = ">";
			this.WizardExeRipperArgsMogArgumentsButton.Location = new System.Drawing.Point(406, 40);
			this.WizardExeRipperArgsMogArgumentsButton.MOGAssetFilename = null;
			this.WizardExeRipperArgsMogArgumentsButton.Name = "WizardExeRipperArgsMogArgumentsButton";
			this.WizardExeRipperArgsMogArgumentsButton.Size = new System.Drawing.Size(24, 23);
			this.WizardExeRipperArgsMogArgumentsButton.TabIndex = 29;
			this.WizardExeRipperArgsMogArgumentsButton.TargetComboBox = null;
			this.WizardExeRipperArgsMogArgumentsButton.TargetTextBox = this.WizardRipperTextBox;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(392, 23);
			this.label2.TabIndex = 12;
			this.label2.Text = "Enter or browse to the desired executable:";
			// 
			// ExploreRipperButton
			// 
			this.ExploreRipperButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ExploreRipperButton.Location = new System.Drawing.Point(380, 40);
			this.ExploreRipperButton.Name = "ExploreRipperButton";
			this.ExploreRipperButton.Size = new System.Drawing.Size(24, 23);
			this.ExploreRipperButton.TabIndex = 9;
			this.ExploreRipperButton.Text = "...";
			this.ExploreRipperButton.Click += new System.EventHandler(this.ExploreRipperButton_Click);
			// 
			// header1
			// 
			this.header1.BackColor = System.Drawing.SystemColors.Control;
			this.header1.CausesValidation = false;
			this.header1.Description = "Specifies what command should be executed when ripping this Asset.  (This propert" +
				"y supports tokenized MOG strings)";
			this.header1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.header1.Image = ((System.Drawing.Image)(resources.GetObject("header1.Image")));
			this.header1.Location = new System.Drawing.Point(0, 236);
			this.header1.Name = "header1";
			this.header1.Size = new System.Drawing.Size(438, 64);
			this.header1.TabIndex = 0;
			this.header1.Title = "Locate ripper";
			// 
			// WizAdvancedWizardPage
			// 
			this.WizAdvancedWizardPage.Controls.Add(this.groupBox7);
			this.WizAdvancedWizardPage.Controls.Add(this.header9);
			this.WizAdvancedWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WizAdvancedWizardPage.IsFinishPage = false;
			this.WizAdvancedWizardPage.Location = new System.Drawing.Point(0, 0);
			this.WizAdvancedWizardPage.Name = "WizAdvancedWizardPage";
			this.WizAdvancedWizardPage.Size = new System.Drawing.Size(438, 300);
			this.WizAdvancedWizardPage.TabIndex = 11;
			this.WizAdvancedWizardPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.WizAdvancedWizardPage_CloseFromNext);
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.WizardAdvancedNoRadioButton);
			this.groupBox7.Controls.Add(this.WizardAdvancedYesRadioButton);
			this.groupBox7.Location = new System.Drawing.Point(16, 16);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(416, 88);
			this.groupBox7.TabIndex = 11;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Would you like to enter the advanced configuration?";
			// 
			// WizardAdvancedNoRadioButton
			// 
			this.WizardAdvancedNoRadioButton.Checked = true;
			this.WizardAdvancedNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardAdvancedNoRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WizardAdvancedNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.WizardAdvancedNoRadioButton.Name = "WizardAdvancedNoRadioButton";
			this.WizardAdvancedNoRadioButton.Size = new System.Drawing.Size(104, 24);
			this.WizardAdvancedNoRadioButton.TabIndex = 1;
			this.WizardAdvancedNoRadioButton.TabStop = true;
			this.WizardAdvancedNoRadioButton.Text = "No (Finished)";
			// 
			// WizardAdvancedYesRadioButton
			// 
			this.WizardAdvancedYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardAdvancedYesRadioButton.Location = new System.Drawing.Point(24, 24);
			this.WizardAdvancedYesRadioButton.Name = "WizardAdvancedYesRadioButton";
			this.WizardAdvancedYesRadioButton.Size = new System.Drawing.Size(104, 24);
			this.WizardAdvancedYesRadioButton.TabIndex = 0;
			this.WizardAdvancedYesRadioButton.Text = "Yes";
			// 
			// header9
			// 
			this.header9.BackColor = System.Drawing.SystemColors.Control;
			this.header9.CausesValidation = false;
			this.header9.Description = "Advanced options allow you to configure this ripper for options like Local cachin" +
				"g, Divergent platforms, Showing command windows, etc...";
			this.header9.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.header9.Image = ((System.Drawing.Image)(resources.GetObject("header9.Image")));
			this.header9.Location = new System.Drawing.Point(0, 236);
			this.header9.Name = "header9";
			this.header9.Size = new System.Drawing.Size(438, 64);
			this.header9.TabIndex = 0;
			this.header9.Title = "Advanced Options";
			// 
			// WizStartWizardPage
			// 
			this.WizStartWizardPage.Controls.Add(this.infoPage1);
			this.WizStartWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WizStartWizardPage.IsFinishPage = false;
			this.WizStartWizardPage.Location = new System.Drawing.Point(0, 0);
			this.WizStartWizardPage.Name = "WizStartWizardPage";
			this.WizStartWizardPage.Size = new System.Drawing.Size(438, 300);
			this.WizStartWizardPage.TabIndex = 1;
			// 
			// infoPage1
			// 
			this.infoPage1.BackColor = System.Drawing.Color.White;
			this.infoPage1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.infoPage1.Image = ((System.Drawing.Image)(resources.GetObject("infoPage1.Image")));
			this.infoPage1.Location = new System.Drawing.Point(0, 0);
			this.infoPage1.Name = "infoPage1";
			this.infoPage1.PageText = "This wizard enables you to setup a base ripper for this asset that can later be p" +
				"romoted to rip a set or class of assets.";
			this.infoPage1.PageTitle = "Welcome to the Ripper Creation Wizard.";
			this.infoPage1.Size = new System.Drawing.Size(438, 300);
			this.infoPage1.TabIndex = 0;
			// 
			// WizFinishWizardPage
			// 
			this.WizFinishWizardPage.Controls.Add(this.infoPage2);
			this.WizFinishWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WizFinishWizardPage.IsFinishPage = false;
			this.WizFinishWizardPage.Location = new System.Drawing.Point(0, 0);
			this.WizFinishWizardPage.Name = "WizFinishWizardPage";
			this.WizFinishWizardPage.Size = new System.Drawing.Size(438, 300);
			this.WizFinishWizardPage.TabIndex = 10;
			// 
			// infoPage2
			// 
			this.infoPage2.BackColor = System.Drawing.Color.White;
			this.infoPage2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.infoPage2.Image = ((System.Drawing.Image)(resources.GetObject("infoPage2.Image")));
			this.infoPage2.Location = new System.Drawing.Point(0, 0);
			this.infoPage2.Name = "infoPage2";
			this.infoPage2.PageText = resources.GetString("infoPage2.PageText");
			this.infoPage2.PageTitle = "Complete!";
			this.infoPage2.Size = new System.Drawing.Size(438, 300);
			this.infoPage2.TabIndex = 0;
			// 
			// WizSlavesWizardPage
			// 
			this.WizSlavesWizardPage.Controls.Add(this.label1);
			this.WizSlavesWizardPage.Controls.Add(this.WizardValidSlavesTextBox);
			this.WizSlavesWizardPage.Controls.Add(this.header8);
			this.WizSlavesWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WizSlavesWizardPage.IsFinishPage = false;
			this.WizSlavesWizardPage.Location = new System.Drawing.Point(0, 0);
			this.WizSlavesWizardPage.Name = "WizSlavesWizardPage";
			this.WizSlavesWizardPage.Size = new System.Drawing.Size(438, 300);
			this.WizSlavesWizardPage.TabIndex = 9;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(376, 40);
			this.label1.TabIndex = 13;
			this.label1.Text = "Enter one or more dedicated slaves for the running of this ripper if you would li" +
				"ke restrict it to certain machines.";
			// 
			// WizardValidSlavesTextBox
			// 
			this.WizardValidSlavesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.WizardValidSlavesTextBox.Location = new System.Drawing.Point(16, 56);
			this.WizardValidSlavesTextBox.Name = "WizardValidSlavesTextBox";
			this.WizardValidSlavesTextBox.Size = new System.Drawing.Size(382, 21);
			this.WizardValidSlavesTextBox.TabIndex = 12;
			// 
			// header8
			// 
			this.header8.BackColor = System.Drawing.SystemColors.Control;
			this.header8.CausesValidation = false;
			this.header8.Description = "Comma delimited list specifying what slave machines are authorized to rip this As" +
				"set.  Blank means all slaves are valid.";
			this.header8.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.header8.Image = ((System.Drawing.Image)(resources.GetObject("header8.Image")));
			this.header8.Location = new System.Drawing.Point(0, 236);
			this.header8.Name = "header8";
			this.header8.Size = new System.Drawing.Size(438, 64);
			this.header8.TabIndex = 11;
			this.header8.Title = "Enable dedicated slave machines";
			// 
			// WizShowCmdWizardPage
			// 
			this.WizShowCmdWizardPage.Controls.Add(this.header7);
			this.WizShowCmdWizardPage.Controls.Add(this.groupBox6);
			this.WizShowCmdWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WizShowCmdWizardPage.IsFinishPage = false;
			this.WizShowCmdWizardPage.Location = new System.Drawing.Point(0, 0);
			this.WizShowCmdWizardPage.Name = "WizShowCmdWizardPage";
			this.WizShowCmdWizardPage.Size = new System.Drawing.Size(438, 300);
			this.WizShowCmdWizardPage.TabIndex = 8;
			// 
			// header7
			// 
			this.header7.BackColor = System.Drawing.SystemColors.Control;
			this.header7.CausesValidation = false;
			this.header7.Description = "Indicates whether or not to show the slave\'s rip command window when processing t" +
				"his Asset.  This is used to view ripper output in realtime.";
			this.header7.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.header7.Image = ((System.Drawing.Image)(resources.GetObject("header7.Image")));
			this.header7.Location = new System.Drawing.Point(0, 236);
			this.header7.Name = "header7";
			this.header7.Size = new System.Drawing.Size(438, 64);
			this.header7.TabIndex = 11;
			this.header7.Title = "Enable command window";
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.WizardShowCommandNoRadioButton);
			this.groupBox6.Controls.Add(this.WizardShowCommandYesRadioButton);
			this.groupBox6.Location = new System.Drawing.Point(8, 16);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(416, 88);
			this.groupBox6.TabIndex = 10;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Would you like to show the command window for this ripper when it is executing?";
			// 
			// WizardShowCommandNoRadioButton
			// 
			this.WizardShowCommandNoRadioButton.Checked = true;
			this.WizardShowCommandNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardShowCommandNoRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WizardShowCommandNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.WizardShowCommandNoRadioButton.Name = "WizardShowCommandNoRadioButton";
			this.WizardShowCommandNoRadioButton.Size = new System.Drawing.Size(128, 24);
			this.WizardShowCommandNoRadioButton.TabIndex = 1;
			this.WizardShowCommandNoRadioButton.TabStop = true;
			this.WizardShowCommandNoRadioButton.Text = "No (Recomended)";
			// 
			// WizardShowCommandYesRadioButton
			// 
			this.WizardShowCommandYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardShowCommandYesRadioButton.Location = new System.Drawing.Point(24, 26);
			this.WizardShowCommandYesRadioButton.Name = "WizardShowCommandYesRadioButton";
			this.WizardShowCommandYesRadioButton.Size = new System.Drawing.Size(104, 24);
			this.WizardShowCommandYesRadioButton.TabIndex = 0;
			this.WizardShowCommandYesRadioButton.Text = "Yes";
			// 
			// WizDuplicateWizardPage
			// 
			this.WizDuplicateWizardPage.Controls.Add(this.header6);
			this.WizDuplicateWizardPage.Controls.Add(this.groupBox5);
			this.WizDuplicateWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WizDuplicateWizardPage.IsFinishPage = false;
			this.WizDuplicateWizardPage.Location = new System.Drawing.Point(0, 0);
			this.WizDuplicateWizardPage.Name = "WizDuplicateWizardPage";
			this.WizDuplicateWizardPage.Size = new System.Drawing.Size(438, 300);
			this.WizDuplicateWizardPage.TabIndex = 7;
			// 
			// header6
			// 
			this.header6.BackColor = System.Drawing.SystemColors.Control;
			this.header6.CausesValidation = false;
			this.header6.Description = "Copy the imported asset files into the temporary directory before ripping to pres" +
				"erve source file integrity.  Usually turned off when ripping large (>300 MB) ass" +
				"ets.";
			this.header6.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.header6.Image = ((System.Drawing.Image)(resources.GetObject("header6.Image")));
			this.header6.Location = new System.Drawing.Point(0, 236);
			this.header6.Name = "header6";
			this.header6.Size = new System.Drawing.Size(438, 64);
			this.header6.TabIndex = 9;
			this.header6.Title = "Enable source asset duplication";
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.WizardCopyAssetNoRadioButton);
			this.groupBox5.Controls.Add(this.WizardCopyAssetYesRadioButton);
			this.groupBox5.Location = new System.Drawing.Point(8, 16);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(416, 88);
			this.groupBox5.TabIndex = 8;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Would you like the imported file to be duplicated to the temp directory before ex" +
				"ecuting the rip?";
			// 
			// WizardCopyAssetNoRadioButton
			// 
			this.WizardCopyAssetNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardCopyAssetNoRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WizardCopyAssetNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.WizardCopyAssetNoRadioButton.Name = "WizardCopyAssetNoRadioButton";
			this.WizardCopyAssetNoRadioButton.Size = new System.Drawing.Size(104, 24);
			this.WizardCopyAssetNoRadioButton.TabIndex = 1;
			this.WizardCopyAssetNoRadioButton.Text = "No";
			// 
			// WizardCopyAssetYesRadioButton
			// 
			this.WizardCopyAssetYesRadioButton.Checked = true;
			this.WizardCopyAssetYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardCopyAssetYesRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WizardCopyAssetYesRadioButton.Location = new System.Drawing.Point(24, 26);
			this.WizardCopyAssetYesRadioButton.Name = "WizardCopyAssetYesRadioButton";
			this.WizardCopyAssetYesRadioButton.Size = new System.Drawing.Size(136, 24);
			this.WizardCopyAssetYesRadioButton.TabIndex = 0;
			this.WizardCopyAssetYesRadioButton.TabStop = true;
			this.WizardCopyAssetYesRadioButton.Text = "Yes (Recomended)";
			// 
			// WizLocalTempWizardPage
			// 
			this.WizLocalTempWizardPage.Controls.Add(this.header5);
			this.WizLocalTempWizardPage.Controls.Add(this.groupBox4);
			this.WizLocalTempWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WizLocalTempWizardPage.IsFinishPage = false;
			this.WizLocalTempWizardPage.Location = new System.Drawing.Point(0, 0);
			this.WizLocalTempWizardPage.Name = "WizLocalTempWizardPage";
			this.WizLocalTempWizardPage.Size = new System.Drawing.Size(438, 300);
			this.WizLocalTempWizardPage.TabIndex = 6;
			// 
			// header5
			// 
			this.header5.BackColor = System.Drawing.SystemColors.Control;
			this.header5.CausesValidation = false;
			this.header5.Description = "The temporary ripping directory will be created on the local hard drive.  If the " +
				"ripper requires IO access durring the rip, making it local will improve performa" +
				"nce.";
			this.header5.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.header5.Image = ((System.Drawing.Image)(resources.GetObject("header5.Image")));
			this.header5.Location = new System.Drawing.Point(0, 236);
			this.header5.Name = "header5";
			this.header5.Size = new System.Drawing.Size(438, 64);
			this.header5.TabIndex = 7;
			this.header5.Title = "Enable local temp ripping directory";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.WizardLocalNoRadioButton);
			this.groupBox4.Controls.Add(this.WizardLocalYesRadioButton);
			this.groupBox4.Location = new System.Drawing.Point(8, 16);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(416, 88);
			this.groupBox4.TabIndex = 6;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Would you like this temp ripping directory to be created on your local hard drive" +
				"?";
			// 
			// WizardLocalNoRadioButton
			// 
			this.WizardLocalNoRadioButton.Checked = true;
			this.WizardLocalNoRadioButton.Cursor = System.Windows.Forms.Cursors.Default;
			this.WizardLocalNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardLocalNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.WizardLocalNoRadioButton.Name = "WizardLocalNoRadioButton";
			this.WizardLocalNoRadioButton.Size = new System.Drawing.Size(104, 24);
			this.WizardLocalNoRadioButton.TabIndex = 1;
			this.WizardLocalNoRadioButton.TabStop = true;
			this.WizardLocalNoRadioButton.Text = "No";
			// 
			// WizardLocalYesRadioButton
			// 
			this.WizardLocalYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardLocalYesRadioButton.Location = new System.Drawing.Point(24, 24);
			this.WizardLocalYesRadioButton.Name = "WizardLocalYesRadioButton";
			this.WizardLocalYesRadioButton.Size = new System.Drawing.Size(104, 24);
			this.WizardLocalYesRadioButton.TabIndex = 0;
			this.WizardLocalYesRadioButton.Text = "Yes";
			// 
			// WizTempWizardPage
			// 
			this.WizTempWizardPage.Controls.Add(this.header4);
			this.WizTempWizardPage.Controls.Add(this.groupBox3);
			this.WizTempWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WizTempWizardPage.IsFinishPage = false;
			this.WizTempWizardPage.Location = new System.Drawing.Point(0, 0);
			this.WizTempWizardPage.Name = "WizTempWizardPage";
			this.WizTempWizardPage.Size = new System.Drawing.Size(438, 300);
			this.WizTempWizardPage.TabIndex = 5;
			this.WizTempWizardPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.WizTempWizardPage_CloseFromNext);
			// 
			// header4
			// 
			this.header4.BackColor = System.Drawing.SystemColors.Control;
			this.header4.CausesValidation = false;
			this.header4.Description = "Run the ripper from a temporary directory other than the asset source files direc" +
				"tory.";
			this.header4.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.header4.Image = ((System.Drawing.Image)(resources.GetObject("header4.Image")));
			this.header4.Location = new System.Drawing.Point(0, 236);
			this.header4.Name = "header4";
			this.header4.Size = new System.Drawing.Size(438, 64);
			this.header4.TabIndex = 5;
			this.header4.Title = "Enable temp ripping directory";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.WizardTempDirNoRadioButton);
			this.groupBox3.Controls.Add(this.WizardTempDirYesRadioButton);
			this.groupBox3.Location = new System.Drawing.Point(8, 16);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(416, 88);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Would you like this ripper to run in an independant rip directory?";
			// 
			// WizardTempDirNoRadioButton
			// 
			this.WizardTempDirNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardTempDirNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.WizardTempDirNoRadioButton.Name = "WizardTempDirNoRadioButton";
			this.WizardTempDirNoRadioButton.Size = new System.Drawing.Size(104, 24);
			this.WizardTempDirNoRadioButton.TabIndex = 1;
			this.WizardTempDirNoRadioButton.Text = "No";
			// 
			// WizardTempDirYesRadioButton
			// 
			this.WizardTempDirYesRadioButton.Checked = true;
			this.WizardTempDirYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardTempDirYesRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WizardTempDirYesRadioButton.Location = new System.Drawing.Point(24, 24);
			this.WizardTempDirYesRadioButton.Name = "WizardTempDirYesRadioButton";
			this.WizardTempDirYesRadioButton.Size = new System.Drawing.Size(136, 24);
			this.WizardTempDirYesRadioButton.TabIndex = 0;
			this.WizardTempDirYesRadioButton.TabStop = true;
			this.WizardTempDirYesRadioButton.Text = "Yes (Recomended)";
			// 
			// WizAutoDetectWizardPage
			// 
			this.WizAutoDetectWizardPage.Controls.Add(this.header3);
			this.WizAutoDetectWizardPage.Controls.Add(this.groupBox2);
			this.WizAutoDetectWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WizAutoDetectWizardPage.IsFinishPage = false;
			this.WizAutoDetectWizardPage.Location = new System.Drawing.Point(0, 0);
			this.WizAutoDetectWizardPage.Name = "WizAutoDetectWizardPage";
			this.WizAutoDetectWizardPage.Size = new System.Drawing.Size(438, 300);
			this.WizAutoDetectWizardPage.TabIndex = 4;
			// 
			// header3
			// 
			this.header3.BackColor = System.Drawing.SystemColors.Control;
			this.header3.CausesValidation = false;
			this.header3.Description = "If enabled, MOG will automatically detect any files that were created or touched " +
				"by the ripping process and identify them as ripped files.";
			this.header3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.header3.Image = ((System.Drawing.Image)(resources.GetObject("header3.Image")));
			this.header3.Location = new System.Drawing.Point(0, 236);
			this.header3.Name = "header3";
			this.header3.Size = new System.Drawing.Size(438, 64);
			this.header3.TabIndex = 3;
			this.header3.Title = "Enable Auto Detect";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.WizardAutoDetectNoRadioButton);
			this.groupBox2.Controls.Add(this.WizardAutoDetectYesRadioButton);
			this.groupBox2.Location = new System.Drawing.Point(8, 16);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(416, 88);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Would you like this ripper to attemp to auto-detect the ripped files?";
			// 
			// WizardAutoDetectNoRadioButton
			// 
			this.WizardAutoDetectNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardAutoDetectNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.WizardAutoDetectNoRadioButton.Name = "WizardAutoDetectNoRadioButton";
			this.WizardAutoDetectNoRadioButton.Size = new System.Drawing.Size(104, 24);
			this.WizardAutoDetectNoRadioButton.TabIndex = 1;
			this.WizardAutoDetectNoRadioButton.Text = "No";
			// 
			// WizardAutoDetectYesRadioButton
			// 
			this.WizardAutoDetectYesRadioButton.Checked = true;
			this.WizardAutoDetectYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardAutoDetectYesRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WizardAutoDetectYesRadioButton.Location = new System.Drawing.Point(24, 24);
			this.WizardAutoDetectYesRadioButton.Name = "WizardAutoDetectYesRadioButton";
			this.WizardAutoDetectYesRadioButton.Size = new System.Drawing.Size(128, 24);
			this.WizardAutoDetectYesRadioButton.TabIndex = 0;
			this.WizardAutoDetectYesRadioButton.TabStop = true;
			this.WizardAutoDetectYesRadioButton.Text = "Yes (Recomended)";
			// 
			// WizDivergentWizardPage
			// 
			this.WizDivergentWizardPage.Controls.Add(this.header2);
			this.WizDivergentWizardPage.Controls.Add(this.groupBox1);
			this.WizDivergentWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WizDivergentWizardPage.IsFinishPage = false;
			this.WizDivergentWizardPage.Location = new System.Drawing.Point(0, 0);
			this.WizDivergentWizardPage.Name = "WizDivergentWizardPage";
			this.WizDivergentWizardPage.Size = new System.Drawing.Size(438, 300);
			this.WizDivergentWizardPage.TabIndex = 3;
			// 
			// header2
			// 
			this.header2.BackColor = System.Drawing.SystemColors.Control;
			this.header2.CausesValidation = false;
			this.header2.Description = "Indicates whether or not this Asset requires unique ripping for each platform.  \'" +
				"Enabled\' means there will be multiple slave tasks generated so unique data can b" +
				"e prepared for each platform.";
			this.header2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.header2.Image = ((System.Drawing.Image)(resources.GetObject("header2.Image")));
			this.header2.Location = new System.Drawing.Point(0, 236);
			this.header2.Name = "header2";
			this.header2.Size = new System.Drawing.Size(438, 64);
			this.header2.TabIndex = 1;
			this.header2.Title = "Enable divergent ripping";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.WizardDivergentNoRadioButton);
			this.groupBox1.Controls.Add(this.WizardDivergentYesRadioButton);
			this.groupBox1.Location = new System.Drawing.Point(8, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(416, 88);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Will this asset produce a different binary for each platform in this project?";
			// 
			// WizardDivergentNoRadioButton
			// 
			this.WizardDivergentNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardDivergentNoRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WizardDivergentNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.WizardDivergentNoRadioButton.Name = "WizardDivergentNoRadioButton";
			this.WizardDivergentNoRadioButton.Size = new System.Drawing.Size(104, 24);
			this.WizardDivergentNoRadioButton.TabIndex = 1;
			this.WizardDivergentNoRadioButton.Text = "No";
			// 
			// WizardDivergentYesRadioButton
			// 
			this.WizardDivergentYesRadioButton.Checked = true;
			this.WizardDivergentYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WizardDivergentYesRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WizardDivergentYesRadioButton.Location = new System.Drawing.Point(24, 24);
			this.WizardDivergentYesRadioButton.Name = "WizardDivergentYesRadioButton";
			this.WizardDivergentYesRadioButton.Size = new System.Drawing.Size(136, 24);
			this.WizardDivergentYesRadioButton.TabIndex = 0;
			this.WizardDivergentYesRadioButton.TabStop = true;
			this.WizardDivergentYesRadioButton.Text = "Yes (Recomended)";
			// 
			// SetupRippingWizard
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(438, 348);
			this.Controls.Add(this.SetupRipperWizard);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SetupRippingWizard";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Setup Ripper Wizard";
			this.Activated += new System.EventHandler(this.SetupRippingWizard_Activated);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.SetupRippingWizard_Closing);
			this.Load += new System.EventHandler(this.SetupRippingWizard_Load);
			this.SetupRipperWizard.ResumeLayout(false);
			this.WizRipperTypeWizardPage.ResumeLayout(false);
			this.groupBox8.ResumeLayout(false);
			this.WizBatchRipperCreateWizardPage.ResumeLayout(false);
			this.WizBatchRipperCreateWizardPage.PerformLayout();
			this.WizRipperWizardPage.ResumeLayout(false);
			this.WizRipperWizardPage.PerformLayout();
			this.WizAdvancedWizardPage.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.WizStartWizardPage.ResumeLayout(false);
			this.WizFinishWizardPage.ResumeLayout(false);
			this.WizSlavesWizardPage.ResumeLayout(false);
			this.WizSlavesWizardPage.PerformLayout();
			this.WizShowCmdWizardPage.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.WizDuplicateWizardPage.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.WizLocalTempWizardPage.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.WizTempWizardPage.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.WizAutoDetectWizardPage.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.WizDivergentWizardPage.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SetupRippingWizard_Load(object sender, System.EventArgs e)
		{
		
		}

		private void SetupRippingWizard_Activated(object sender, System.EventArgs e)
		{
			// Get a properties object based on how many properties we have run the wizard on
			MOG_Properties assetProperties = GetPropertiesFromActive();

			if (assetProperties != null && ShowAdvancedWizard)
			{
				WizardRipperTextBox.Text = assetProperties.AssetRipper;
				PropertiesRippingBatchNameTextBox.Text = assetProperties.AssetRipper;

				// If we have a ripper set enable the edit and test buttons
				if (assetProperties.AssetRipper.Length > 0)
				{
					PropertiesBatchRipperEditButton.Enabled = true;
					PropertiesBatchRipperTestButton.Enabled = true;
				}

				if (assetProperties.AutoDetectRippedFiles) WizardAutoDetectYesRadioButton.Checked = true;
				else WizardAutoDetectNoRadioButton.Checked = true;

				if (assetProperties.CopyFilesIntoTempRipDir) WizardCopyAssetYesRadioButton.Checked = true;
				else WizardCopyAssetNoRadioButton.Checked = true;

				if (assetProperties.DivergentPlatformDataType) WizardDivergentYesRadioButton.Checked = true;
				else WizardDivergentNoRadioButton.Checked = true;

				if (assetProperties.ShowRipCommandWindow) WizardShowCommandYesRadioButton.Checked = true;
				else WizardShowCommandNoRadioButton.Checked = true;

				if (assetProperties.UseLocalTempRipDir) WizardLocalYesRadioButton.Checked = true;
				else WizardLocalNoRadioButton.Checked = true;

				if (assetProperties.UseTempRipDir) WizardTempDirYesRadioButton.Checked = true;
				else WizardTempDirNoRadioButton.Checked = true;

				WizardValidSlavesTextBox.Text = assetProperties.ValidSlaves;

				// Enable the advanced menu option
				// KLK - John wants me to default the wizard to no advanced always!
				//	WizardAdvancedYesRadioButton.Checked = ShowAdvancedWizard;
			}

			// Set the asset name for the MOG argument buttons
			if (assetName != null)
			{
				WizardExeRipperArgsMogArgumentsButton.MOGAssetFilename = assetName;
				WizardExeRipperArgsMogControl_ViewTokenTextBox.MOGAssetFilename = assetName;
			}
			else if (assetNames.Count > 0)
			{
				WizardExeRipperArgsMogArgumentsButton.MOGAssetFilename = assetNames[0] as MOG_Filename;
				WizardExeRipperArgsMogControl_ViewTokenTextBox.MOGAssetFilename = assetNames[0] as MOG_Filename;
			}
		}

		private void ExploreRipperButton_Click(object sender, System.EventArgs e)
		{
			MOG_Properties properties = new MOG_Properties();
			
			UITypeEditor editor = null;

			// Get our collection of properties
			PropertyDescriptorCollection propCollection = properties.GetProperties(new Attribute[]{new BrowsableAttribute(true)});
			// Foreach PropertyDescriptor, see if we find what we're looking for
			foreach( PropertyDescriptor descriptor in propCollection)
			{
				// If our displayName == the propertyToLookFor, we can get our editor and newValue...
				if( descriptor.DisplayName.ToLower() == "PackagePostMergeEvent".ToLower())
				{
					editor = descriptor.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
				}
			}

			if(editor != null)
			{
				this.WizardRipperTextBox.Text = GetValueForProperty(editor, this.WizardRipperTextBox.Text) as String;
			}
		}

		private void PropertiesRippingBatchBrowseButton_Click(object sender, EventArgs e)
		{
			MOG_Properties properties = new MOG_Properties();

			UITypeEditor editor = null;

			// Get our collection of properties
			PropertyDescriptorCollection propCollection = properties.GetProperties(new Attribute[] { new BrowsableAttribute(true) });
			// Foreach PropertyDescriptor, see if we find what we're looking for
			foreach (PropertyDescriptor descriptor in propCollection)
			{
				// If our displayName == the propertyToLookFor, we can get our editor and newValue...
				if (descriptor.DisplayName.ToLower() == "PackagePostMergeEvent".ToLower())
				{
					editor = descriptor.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
				}
			}

			if (editor != null)
			{
				this.PropertiesRippingBatchNameTextBox.Text = GetValueForProperty(editor, this.WizardRipperTextBox.Text) as String;

				// Now enable the edit and test buttons
				PropertiesBatchRipperEditButton.Enabled = true;
				PropertiesBatchRipperTestButton.Enabled = true;
			}
		}

		/// <summary>
		/// Given an editor, gets the value of a property
		/// </summary>
		/// <param name="editor"></param>
		/// <param name="oldValue"></param>
		/// <returns></returns>
		public object GetValueForProperty(UITypeEditor editor, object oldValue)
		{
			object newValue = null;

			try
			{
				newValue = editor.EditValue(null, null, "");
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("Error editting Property",
					"At runtime, the editor was unable to be used.\r\n\r\n" + ex.Message + "\r\n\r\n", ex.StackTrace, 
					MOG_ALERT_LEVEL.ERROR);

			}

			if (newValue == null)
			{
				return oldValue;
			}
			else if ((newValue as string) != null && ((string)newValue).Length == 0)
			{
				return oldValue;
			}
			else
			{
				return newValue;
			}
		}

		private void PropertiesRippingCreateButton_Click(object sender, System.EventArgs e)
		{
			CreateRipper();
		}
		
		private void PropertiesBatchRipperTestButton_Click(object sender, EventArgs e)
		{
			// Loop through all the selected properties
			foreach (MOG_Properties properties in this.PropertiesList)
			{
				// Perform the ripper test
				MOG_ControllerRipper ripper = new MOG_ControllerRipper(properties.GetAssetFilename());
				if (!ripper.LocalRipper(properties, ""))
				{
					// Inform the user that the rip failed...
					// All needed rip information and logs can be found within the asset
				}
			}
		}

		private string CreateRipperName()
		{
			// Are we file based or properties based?
			if (assetNames.Count > 0 || assetName != null)
			{
				// Do we have a single?
				if (assetName != null)
				{
					return assetName.GetAssetClassification();
				}
				else
				{
					string className = "";
					foreach (MOG_Filename clsname in assetNames)
					{
						if (className == "")
						{
							className = clsname.GetAssetClassification();
						}
						else if (string.Compare(className, clsname.GetAssetClassification(), true) != 0)
						{
							// Ask for a name
							string newName = Microsoft.VisualBasic.Interaction.InputBox("The classifications of these assets are not the same therefore we cannot guess what to call this ripper.  Enter a ripper name:", "Enter Ripper Name", "NewRipper", -1, -1);
							return newName;
						}
					}

					return className;
				}
			}
			else if (this.PropertiesList.Count > 0)
			{
				string className = "";
				foreach (MOG_Properties clsname in this.PropertiesList)
				{
					if (className == "")
					{
						className = clsname.Classification;
					}
					else if (string.Compare(className, clsname.Classification, true) != 0)
					{
						// Ask for a name
						string newName = Microsoft.VisualBasic.Interaction.InputBox("The classifications of these assets are not the same therefore we cannot guess what to call this ripper.  Enter a ripper name:", "Enter Ripper Name", "NewRipper", -1, -1);
						return newName;
					}
				}

				return className;
			}

			return "NewRipper";
		}

		private MOG_Properties GetPropertiesFromActive()
		{
			// Are we file based or properties based?
			if (assetNames.Count > 0 || assetName != null)
			{
				// Do we have a single?
				if (assetName != null)
				{
					return new MOG_Properties(assetName);
				}
				
				return new MOG_Properties();				
			}
			else if (this.PropertiesList.Count > 1)
			{
				 return new MOG_Properties();
			}
			else if (this.PropertiesList.Count == 1)
			{
				return PropertiesList[0] as MOG_Properties;
			}

			return new MOG_Properties();
		}

		/// <summary>
		/// Edit the ripper in notepad
		/// </summary>
		private void CreateRipper()
		{
			// Create one named after this assets class
			string defaultRipperName = PropertiesRippingBatchNameTextBox.Text;
			string ripper = MOG_ControllerSystem.LocateTool(defaultRipperName);

			if (ripper.Length == 0)
			{
				// Make sure our ripper ends in a bat
				if (string.Compare(DosUtils.PathGetExtension(defaultRipperName), "Bat", true) != 0)
				{
					defaultRipperName += ".bat";
				}

				// If not, do we want to create one?
				if (MOG_Prompt.PromptResponse("No existing ripper found!", "There is no ripper yet defined by this name.  Would you like to create \n(" + defaultRipperName + ")\n for editing?", MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
				{
					// Create one named after this assets class
					ripper = Path.Combine(MOG_ControllerProject.GetProject().GetProjectToolsPath(), defaultRipperName);

					// Do we want to start from the template?
					if (MOG_Prompt.PromptResponse("User ripper template!", "Would you like start this new ripper from the MOG ripper template?", MOGPromptButtons.YesNo) == MOGPromptResult.Yes)
					{
						// If so, copy the template to this new ripper
						string template = MOG_ControllerSystem.GetSystemRepositoryPath() + "\\Tools\\Rippers\\TemplateComplex_ripper.bat";

						DosUtils.CopyFast(template, ripper, false);						
					}
				}
			}			
						
			// Now, set this new ripper as the ripper assigned to this asset
			string ripper2 = MOG_ControllerSystem.StripInternalizedToolPath(ripper);
			PropertiesRippingBatchNameTextBox.Text = ripper2;

			// Now enable the edit button
			if (ripper2.Length > 0)
			{
				PropertiesBatchRipperEditButton.Enabled = true;
				PropertiesBatchRipperTestButton.Enabled = true;
			}
		}

		private void PropertiesBatchRipperEditButton_Click(object sender, System.EventArgs e)
		{
			// Get the assigned ripper information
			string ripperAndArgs = PropertiesRippingBatchNameTextBox.Text;
			string tool = DosUtils.FileStripArguments(ripperAndArgs);
			string args = DosUtils.FileGetArguments(ripperAndArgs);

			// Get the assigned ripper
			string ripper = MOG_ControllerSystem.LocateTool("", tool);

			// If the ripper is defined, open it
			if (ripper.Length > 0 && tool.Length > 0)
			{
				guiCommandLine.ShellSpawn("Notepad.exe", ripper);
			}
			else
			{
				// If not, do we want to create one?
				MOG_Prompt.PromptResponse("No existing ripper found!", "There is no ripper defined by this name!", MOGPromptButtons.OK);				
			}			
		}			

		private void WizardEditButton_Click(object sender, System.EventArgs e)
		{
			// Launch the editor
			string output = "";
			if (WizardRipperTextBox.Text.Length > 0)
			{
				string ripperAndArgs = WizardRipperTextBox.Text;
			
				// Get the assigned ripper information
				string tool = DosUtils.FileStripArguments(ripperAndArgs);
				string args = DosUtils.FileGetArguments(ripperAndArgs);

				string ripper = MOG_ControllerSystem.LocateTool(tool);

				if (ripper.Length > 0)
				{
					guiCommandLine.ShellExecute("Notepad.exe", ripper, System.Diagnostics.ProcessWindowStyle.Normal, ref output);
				}
			}
		}

		private void WizRipperTypeWizardPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
		{
			// If we are an exe type ripper, skip to exe stuff
			if (WizardRiperTypeExeRadioButton.Checked)
			{
				e.Page = WizRipperWizardPage;
			}		
		}

		private void WizBatchRipperCreateWizardPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
		{
			// This page always skips to the advanced page when next is clicked
			e.Page = this.WizAdvancedWizardPage;
		}	

		private void WizAdvancedWizardPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
		{
			// If no advanced is checked, jump to end
			if (WizardAdvancedNoRadioButton.Checked)
			{
				e.Page = WizFinishWizardPage;
			}
		}
	
		private void WizTempWizardPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
		{
			// If no temp dir, can skip next two options
			if (WizardTempDirNoRadioButton.Checked)
			{
				e.Page = WizShowCmdWizardPage;
			}
		}

		private void SetupRippingWizard_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Special case if only one asset was passed in
			if (assetNames.Count == 0 && assetName != null)
			{
				assetNames.Add(assetName);
				SavePropertiesFiles();
			}
			else if (this.PropertiesList.Count > 0)
			{
				SaveProperties();
			}
			else
			{
				SavePropertiesFiles();
			}			
		}

		private void SaveProperties()
		{
			// Open the wizard
			if (this.DialogResult == DialogResult.OK)
			{
				foreach (MOG_Properties properties in PropertiesList)
				{	
					// Save either the exe or batch ripper we setup
					if (this.WizardRiperTypeBatchRadioButton.Checked)
					{
						properties.AssetRipper = PropertiesRippingBatchNameTextBox.Text;
					}
					else if (this.WizardRiperTypeExeRadioButton.Checked)
					{
						properties.AssetRipper = WizardRipperTextBox.Text;
					}

					properties.AutoDetectRippedFiles = WizardAutoDetectYesRadioButton.Checked;
					properties.CopyFilesIntoTempRipDir = WizardCopyAssetYesRadioButton.Checked;
					properties.DivergentPlatformDataType = WizardDivergentYesRadioButton.Checked;
					properties.ShowRipCommandWindow = WizardShowCommandYesRadioButton.Checked;
					properties.UseLocalTempRipDir = WizardLocalYesRadioButton.Checked;
					properties.UseTempRipDir = WizardTempDirYesRadioButton.Checked;
					properties.ValidSlaves = WizardValidSlavesTextBox.Text;							
				}
			}
		}

		private void SavePropertiesFiles()
		{
			// Open the wizard
			if (this.DialogResult == DialogResult.OK)
			{
				foreach (MOG_Filename filename in assetNames)
				{		
					// Make sure we are an asset before showing log
					if (filename.GetFilenameType() == MOG_FILENAME_TYPE.MOG_FILENAME_Asset)
					{	
						// Set our new properties
						MOG_ControllerAsset asset = MOG_ControllerAsset.OpenAsset(filename);

						if (asset != null)
						{
							// Save either the exe or batch ripper we setup
							if (this.WizardRiperTypeBatchRadioButton.Checked)
							{
								asset.GetProperties().AssetRipper = PropertiesRippingBatchNameTextBox.Text;
							}
							else if (this.WizardRiperTypeExeRadioButton.Checked)
							{
								asset.GetProperties().AssetRipper = WizardRipperTextBox.Text;
							}
							
							asset.GetProperties().AutoDetectRippedFiles = WizardAutoDetectYesRadioButton.Checked;
							asset.GetProperties().CopyFilesIntoTempRipDir = WizardCopyAssetYesRadioButton.Checked;
							asset.GetProperties().DivergentPlatformDataType = WizardDivergentYesRadioButton.Checked;
							asset.GetProperties().ShowRipCommandWindow = WizardShowCommandYesRadioButton.Checked;
							asset.GetProperties().UseLocalTempRipDir = WizardLocalYesRadioButton.Checked;
							asset.GetProperties().UseTempRipDir = WizardTempDirYesRadioButton.Checked;
							asset.GetProperties().ValidSlaves = WizardValidSlavesTextBox.Text;

							// Save these properties
							asset.Close();
						}
					}					
				}
			}
		}

		private void PropertiesRippingBatchNameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			if(PropertiesRippingBatchNameTextBox.Text.Length > 0)
			{
				PropertiesRippingCreateButton.Enabled = true;
			}
			else
			{
				PropertiesRippingCreateButton.Enabled = false;
				PropertiesBatchRipperEditButton.Enabled = false;
				PropertiesBatchRipperTestButton.Enabled = false;
			}
		}

		private void WizBatchRipperCreateWizardPage_ShowFromNext(object sender, EventArgs e)
		{
			PropertiesRippingBatchNameTextBox.Text = "Rippers\\NewRipper.bat";
			PropertiesRippingCreateButton.Enabled = true;
		}

		private void WizardRipperTestButton_Click(object sender, EventArgs e)
		{
			// Loop through all the selected properties
			foreach (MOG_Properties properties in this.PropertiesList)
			{
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
		
