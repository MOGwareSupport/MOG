using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Drawing.Design;
using System.ComponentModel.Design;

using MOG.REPORT;
using MOG.PROMPT;
using MOG.FILENAME;
using MOG.PROPERTIES;
using MOG.CONTROLLER.CONTROLLERASSET;
using MOG.CONTROLLER.CONTROLLERREPOSITORY;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG;

using MOG_Client.Client_Gui;
using MOG_Client.Forms;
using MOG_ControlsLibrary.Utils;
using MOG_CoreControls;
using MOG_ControlsLibrary.Common.MOG_ContextMenu;
using MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews;
using MOG_ControlsLibrary.Controls;


namespace MOG_Client.Forms.AssetProperties
{
	/// <summary>
	/// Summary description for SetupPackagingWizard.
	/// </summary>
	public class SetupPackagingWizard : System.Windows.Forms.Form
	{
		public MOG_Filename assetName;
		public ArrayList assetNames = new ArrayList();
		public ArrayList PropertiesList = new ArrayList();		
		public bool mHasBlessedAsset;

		#region Windows Form Variables
		private Gui.Wizard.WizardPage PackagingStartWizardPage;
		private Gui.Wizard.InfoPage PackagingStartInfoPage;
		private Gui.Wizard.WizardPage PackagingSingleCommandsWizardPage;
		private Gui.Wizard.Header header4;
		public System.Windows.Forms.TextBox PackageSimpleRemoveTextBox;
		public System.Windows.Forms.TextBox PackageSimpleAddTextBox;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label2;
		private Gui.Wizard.WizardPage PackagingDoneWizardPage;
		private Gui.Wizard.Wizard Wizard;
		private Gui.Wizard.WizardPage PackagingAssignmentsWizardPage;
		private Gui.Wizard.Header header1;
		private System.Windows.Forms.Button PackageManagementButton;
		private Gui.Wizard.InfoPage infoPage1;
		private Gui.Wizard.WizardPage PackageCreatePackageWizardPage;
		private Gui.Wizard.WizardPage PackageInsertSupportWizardPage;
		private Gui.Wizard.WizardPage PackageStyleWizardPage;
		private Gui.Wizard.WizardPage PackageTaskFileToolWizardPage;
		private Gui.Wizard.WizardPage PackageTaskFilenameWizardPage;
		private Gui.Wizard.WizardPage PackageDeleteCommandWizardPage;
		private Gui.Wizard.WizardPage PackageNeedEventsWizardPage;
		private Gui.Wizard.WizardPage PackageEventsWizardPage;
		private Gui.Wizard.WizardPage PackageCleanWizardPage;
		private Gui.Wizard.WizardPage PackageClusterWizardPage;
		private Gui.Wizard.WizardPage PackageAsynchWizardPage;
		private Gui.Wizard.WizardPage PackageCustomAddWizardPage;
		private Gui.Wizard.WizardPage PackageCommandsWizardPage;
		private Gui.Wizard.WizardPage PackageDependanciesWizardPage;
		private Gui.Wizard.WizardPage PackageLateResolversWizardPage;
		private Gui.Wizard.Header header2;
		private System.Windows.Forms.GroupBox groupBox7;
		private Gui.Wizard.Header header3;
		private Gui.Wizard.Header header5;
		private System.Windows.Forms.GroupBox groupBox1;
		private Gui.Wizard.Header header6;
		private System.Windows.Forms.GroupBox groupBox2;
		private Gui.Wizard.Header header7;
		private System.Windows.Forms.Label label6;
		private Gui.Wizard.Header header8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label10;
		private Gui.Wizard.Header header9;
		private Gui.Wizard.Header header10;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label13;
		private Gui.Wizard.Header header11;
		private System.Windows.Forms.GroupBox groupBox4;
		private Gui.Wizard.Header header12;
		private System.Windows.Forms.GroupBox groupBox5;
		private Gui.Wizard.Header header13;
		private System.Windows.Forms.GroupBox groupBox6;
		private Gui.Wizard.Header header14;
		private System.Windows.Forms.GroupBox groupBox8;
		private Gui.Wizard.Header header15;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private Gui.Wizard.Header header16;
		private System.Windows.Forms.GroupBox groupBox9;
		private Gui.Wizard.Header header17;
		private System.Windows.Forms.Label label16;
		private Gui.Wizard.Header header18;
		private System.Windows.Forms.RadioButton PackageCustomCommandNoRadioButton;
		public System.Windows.Forms.RadioButton PackageCustomCommandYesRadioButton;
		private System.Windows.Forms.RadioButton PackageDependencyNoRadioButton;
		public System.Windows.Forms.RadioButton PackageDependencyYesRadioButton;
		private System.Windows.Forms.RadioButton PackageInsertSupportNoRadioButton;
		public System.Windows.Forms.RadioButton PackageInsertSupportYesRadioButton;
		private System.Windows.Forms.RadioButton PackageStyleTaskRadioButton;
		public System.Windows.Forms.RadioButton PackageStyleSimpleRadioButton;
		private System.Windows.Forms.RadioButton PackageNeedEventsNoRadioButton;
		public System.Windows.Forms.RadioButton PackageNeedEventsYesRadioButton;
		private System.Windows.Forms.RadioButton PackageClusterNoRadioButton;
		public System.Windows.Forms.RadioButton PackageClusterYesRadioButton;
		private System.Windows.Forms.RadioButton PackageAsyncNoRadioButton;
		public System.Windows.Forms.RadioButton PackageAsyncYesRadioButton;
		public System.Windows.Forms.TextBox PackageDefaultAddCommandTextBox;
		public System.Windows.Forms.TextBox PackageDefaultRemoveCommandTextBox;
		private System.Windows.Forms.Button PackageExplorePackagerButton;
		private System.Windows.Forms.Button PackagePostEventExploreButton;
		private System.Windows.Forms.Button PackagePreEventExploreButton;
		public System.Windows.Forms.TextBox PackagePostEventTextBox;
		public System.Windows.Forms.TextBox PackagePreEventTextBox;
		public System.Windows.Forms.TextBox PackageDeleteCommandTextBox;
		public System.Windows.Forms.TextBox PackageTaskFileOutTextBox;
		public System.Windows.Forms.TextBox PackageTaskFileInTextBox;
		private MOG_Client.Forms.ArgumentControl.MogArgumentsButton PackageDefaultAddCommandMogArgumentsButton;
		private MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox PackageDefaultAddCommandMogControl_ViewTokenTextBox;
		private MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox PackageDefaultRemoveCommandMogControl_ViewTokenTextBox;
		private MOG_Client.Forms.ArgumentControl.MogArgumentsButton PackageDefaultRemoveCommandMogArgumentsButton;
		private MOG_Client.Forms.ArgumentControl.MogArgumentsButton PackageSimpleRemoveMogArgumentsButton;
		private MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox PackageSimpleRemoveMogControl_ViewTokenTextBox;
		private MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox PackageSimpleAddMogControl_ViewTokenTextBox;
		private MOG_Client.Forms.ArgumentControl.MogArgumentsButton PackageSimpleAddMogArgumentsButton;
		private MogControl_PackageManagementTreeView PackageCreateNewMogControl_PackageManagementTreeView;
		private System.Windows.Forms.RadioButton PackageNewPackageNoRadioButton;
		public System.Windows.Forms.RadioButton PackageNewPackageYesRadioButton;
		private Gui.Wizard.WizardPage PackageNewPackageWizardPage;
		public System.Windows.Forms.TextBox PackageLateResolverTextBox;
		public System.Windows.Forms.TextBox PackagePackagerToolTextBox;
		private System.Windows.Forms.RadioButton PackageCleanupDirNoRadioButton;
		public System.Windows.Forms.RadioButton PackageCleanupDirYesRadioButton;
		private MOG_XpProgressBar WizardMog_XpProgressBar;
		private Gui.Wizard.WizardPage PackagePackageClassWizardPage;
		private Gui.Wizard.Header header19;
		public MogControl_FullTreeView MogRepositoryTreeView;
		#endregion
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SetupPackagingWizard()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.WizardMog_XpProgressBar.PositionMax = this.Wizard.Pages.Count-1;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SetupPackagingWizard));
			this.Wizard = new Gui.Wizard.Wizard();
			this.PackagingDoneWizardPage = new Gui.Wizard.WizardPage();
			this.infoPage1 = new Gui.Wizard.InfoPage();
			this.PackageLateResolversWizardPage = new Gui.Wizard.WizardPage();
			this.PackageLateResolverTextBox = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.header18 = new Gui.Wizard.Header();
			this.PackageDependanciesWizardPage = new Gui.Wizard.WizardPage();
			this.groupBox9 = new System.Windows.Forms.GroupBox();
			this.PackageDependencyNoRadioButton = new System.Windows.Forms.RadioButton();
			this.PackageDependencyYesRadioButton = new System.Windows.Forms.RadioButton();
			this.header17 = new Gui.Wizard.Header();
			this.PackagingSingleCommandsWizardPage = new Gui.Wizard.WizardPage();
			this.PackageSimpleRemoveMogArgumentsButton = new MOG_Client.Forms.ArgumentControl.MogArgumentsButton();
			this.PackageSimpleRemoveTextBox = new System.Windows.Forms.TextBox();
			this.PackageSimpleRemoveMogControl_ViewTokenTextBox = new MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox();
			this.PackageSimpleAddMogControl_ViewTokenTextBox = new MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox();
			this.PackageSimpleAddTextBox = new System.Windows.Forms.TextBox();
			this.PackageSimpleAddMogArgumentsButton = new MOG_Client.Forms.ArgumentControl.MogArgumentsButton();
			this.label2 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.header4 = new Gui.Wizard.Header();
			this.PackageCommandsWizardPage = new Gui.Wizard.WizardPage();
			this.PackageDefaultRemoveCommandMogArgumentsButton = new MOG_Client.Forms.ArgumentControl.MogArgumentsButton();
			this.PackageDefaultRemoveCommandTextBox = new System.Windows.Forms.TextBox();
			this.PackageDefaultRemoveCommandMogControl_ViewTokenTextBox = new MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox();
			this.PackageDefaultAddCommandMogControl_ViewTokenTextBox = new MOG_Client.Forms.AssetProperties.MogControl_ViewTokenTextBox();
			this.PackageDefaultAddCommandTextBox = new System.Windows.Forms.TextBox();
			this.PackageDefaultAddCommandMogArgumentsButton = new MOG_Client.Forms.ArgumentControl.MogArgumentsButton();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.header16 = new Gui.Wizard.Header();
			this.PackageCustomAddWizardPage = new Gui.Wizard.WizardPage();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.PackageCustomCommandNoRadioButton = new System.Windows.Forms.RadioButton();
			this.PackageCustomCommandYesRadioButton = new System.Windows.Forms.RadioButton();
			this.header15 = new Gui.Wizard.Header();
			this.PackagingAssignmentsWizardPage = new Gui.Wizard.WizardPage();
			this.PackageManagementButton = new System.Windows.Forms.Button();
			this.header1 = new Gui.Wizard.Header();
			this.PackageAsynchWizardPage = new Gui.Wizard.WizardPage();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.PackageAsyncNoRadioButton = new System.Windows.Forms.RadioButton();
			this.PackageAsyncYesRadioButton = new System.Windows.Forms.RadioButton();
			this.header14 = new Gui.Wizard.Header();
			this.PackageClusterWizardPage = new Gui.Wizard.WizardPage();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.PackageClusterNoRadioButton = new System.Windows.Forms.RadioButton();
			this.PackageClusterYesRadioButton = new System.Windows.Forms.RadioButton();
			this.header13 = new Gui.Wizard.Header();
			this.PackageCleanWizardPage = new Gui.Wizard.WizardPage();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.PackageCleanupDirNoRadioButton = new System.Windows.Forms.RadioButton();
			this.PackageCleanupDirYesRadioButton = new System.Windows.Forms.RadioButton();
			this.header12 = new Gui.Wizard.Header();
			this.PackageEventsWizardPage = new Gui.Wizard.WizardPage();
			this.PackagePostEventExploreButton = new System.Windows.Forms.Button();
			this.PackagePreEventExploreButton = new System.Windows.Forms.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.PackagePostEventTextBox = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.PackagePreEventTextBox = new System.Windows.Forms.TextBox();
			this.header11 = new Gui.Wizard.Header();
			this.PackageNeedEventsWizardPage = new Gui.Wizard.WizardPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.PackageNeedEventsNoRadioButton = new System.Windows.Forms.RadioButton();
			this.PackageNeedEventsYesRadioButton = new System.Windows.Forms.RadioButton();
			this.header10 = new Gui.Wizard.Header();
			this.PackageDeleteCommandWizardPage = new Gui.Wizard.WizardPage();
			this.label10 = new System.Windows.Forms.Label();
			this.PackageDeleteCommandTextBox = new System.Windows.Forms.TextBox();
			this.header9 = new Gui.Wizard.Header();
			this.PackageTaskFilenameWizardPage = new Gui.Wizard.WizardPage();
			this.label8 = new System.Windows.Forms.Label();
			this.PackageTaskFileOutTextBox = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.PackageTaskFileInTextBox = new System.Windows.Forms.TextBox();
			this.header8 = new Gui.Wizard.Header();
			this.PackageTaskFileToolWizardPage = new Gui.Wizard.WizardPage();
			this.label6 = new System.Windows.Forms.Label();
			this.PackageExplorePackagerButton = new System.Windows.Forms.Button();
			this.PackagePackagerToolTextBox = new System.Windows.Forms.TextBox();
			this.header7 = new Gui.Wizard.Header();
			this.PackageStyleWizardPage = new Gui.Wizard.WizardPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.PackageStyleTaskRadioButton = new System.Windows.Forms.RadioButton();
			this.PackageStyleSimpleRadioButton = new System.Windows.Forms.RadioButton();
			this.header6 = new Gui.Wizard.Header();
			this.PackageInsertSupportWizardPage = new Gui.Wizard.WizardPage();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.PackageInsertSupportNoRadioButton = new System.Windows.Forms.RadioButton();
			this.PackageInsertSupportYesRadioButton = new System.Windows.Forms.RadioButton();
			this.header5 = new Gui.Wizard.Header();
			this.PackageCreatePackageWizardPage = new Gui.Wizard.WizardPage();
			this.PackageCreateNewMogControl_PackageManagementTreeView = new MogControl_PackageManagementTreeView();
			this.header3 = new Gui.Wizard.Header();
			this.PackagePackageClassWizardPage = new Gui.Wizard.WizardPage();
			this.MogRepositoryTreeView = new MogControl_FullTreeView();
			this.header19 = new Gui.Wizard.Header();
			this.PackageNewPackageWizardPage = new Gui.Wizard.WizardPage();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.PackageNewPackageNoRadioButton = new System.Windows.Forms.RadioButton();
			this.PackageNewPackageYesRadioButton = new System.Windows.Forms.RadioButton();
			this.header2 = new Gui.Wizard.Header();
			this.PackagingStartWizardPage = new Gui.Wizard.WizardPage();
			this.PackagingStartInfoPage = new Gui.Wizard.InfoPage();
			this.WizardMog_XpProgressBar = new MOG_XpProgressBar();
			this.Wizard.SuspendLayout();
			this.PackagingDoneWizardPage.SuspendLayout();
			this.PackageLateResolversWizardPage.SuspendLayout();
			this.PackageDependanciesWizardPage.SuspendLayout();
			this.groupBox9.SuspendLayout();
			this.PackagingSingleCommandsWizardPage.SuspendLayout();
			this.PackageCommandsWizardPage.SuspendLayout();
			this.PackageCustomAddWizardPage.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.PackagingAssignmentsWizardPage.SuspendLayout();
			this.PackageAsynchWizardPage.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.PackageClusterWizardPage.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.PackageCleanWizardPage.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.PackageEventsWizardPage.SuspendLayout();
			this.PackageNeedEventsWizardPage.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.PackageDeleteCommandWizardPage.SuspendLayout();
			this.PackageTaskFilenameWizardPage.SuspendLayout();
			this.PackageTaskFileToolWizardPage.SuspendLayout();
			this.PackageStyleWizardPage.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.PackageInsertSupportWizardPage.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.PackageCreatePackageWizardPage.SuspendLayout();
			this.PackagePackageClassWizardPage.SuspendLayout();
			this.PackageNewPackageWizardPage.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.PackagingStartWizardPage.SuspendLayout();
			this.SuspendLayout();
			// 
			// Wizard
			// 
			this.Wizard.Controls.Add(this.PackagingStartWizardPage);
			this.Wizard.Controls.Add(this.PackageNewPackageWizardPage);
			this.Wizard.Controls.Add(this.PackagingDoneWizardPage);
			this.Wizard.Controls.Add(this.PackageLateResolversWizardPage);
			this.Wizard.Controls.Add(this.PackageDependanciesWizardPage);
			this.Wizard.Controls.Add(this.PackagingSingleCommandsWizardPage);
			this.Wizard.Controls.Add(this.PackageCommandsWizardPage);
			this.Wizard.Controls.Add(this.PackageCustomAddWizardPage);
			this.Wizard.Controls.Add(this.PackagingAssignmentsWizardPage);
			this.Wizard.Controls.Add(this.PackageAsynchWizardPage);
			this.Wizard.Controls.Add(this.PackageClusterWizardPage);
			this.Wizard.Controls.Add(this.PackageCleanWizardPage);
			this.Wizard.Controls.Add(this.PackageEventsWizardPage);
			this.Wizard.Controls.Add(this.PackageNeedEventsWizardPage);
			this.Wizard.Controls.Add(this.PackageDeleteCommandWizardPage);
			this.Wizard.Controls.Add(this.PackageTaskFilenameWizardPage);
			this.Wizard.Controls.Add(this.PackageTaskFileToolWizardPage);
			this.Wizard.Controls.Add(this.PackageStyleWizardPage);
			this.Wizard.Controls.Add(this.PackageInsertSupportWizardPage);
			this.Wizard.Controls.Add(this.PackageCreatePackageWizardPage);
			this.Wizard.Controls.Add(this.PackagePackageClassWizardPage);
			this.Wizard.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Wizard.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Wizard.Location = new System.Drawing.Point(0, 0);
			this.Wizard.Name = "Wizard";
			this.Wizard.Pages.AddRange(new Gui.Wizard.WizardPage[] {
																	   this.PackagingStartWizardPage,
																	   this.PackageNewPackageWizardPage,
																	   this.PackagePackageClassWizardPage,
																	   this.PackageCreatePackageWizardPage,
																	   this.PackageInsertSupportWizardPage,
																	   this.PackageStyleWizardPage,
																	   this.PackageTaskFileToolWizardPage,
																	   this.PackageTaskFilenameWizardPage,
																	   this.PackageDeleteCommandWizardPage,
																	   this.PackageNeedEventsWizardPage,
																	   this.PackageEventsWizardPage,
																	   this.PackageCleanWizardPage,
																	   this.PackageClusterWizardPage,
																	   this.PackageAsynchWizardPage,
																	   this.PackagingAssignmentsWizardPage,
																	   this.PackageCustomAddWizardPage,
																	   this.PackageCommandsWizardPage,
																	   this.PackagingSingleCommandsWizardPage,
																	   this.PackageDependanciesWizardPage,
																	   this.PackageLateResolversWizardPage,
																	   this.PackagingDoneWizardPage});
			this.Wizard.PushPop = true;
			this.Wizard.Size = new System.Drawing.Size(458, 355);
			this.Wizard.TabIndex = 0;
			this.Wizard.NextClicked += new System.EventHandler(this.Wizard_UpdateProgressBar);
			this.Wizard.BackClicked += new System.EventHandler(this.Wizard_UpdateProgressBar);
			// 
			// PackagingDoneWizardPage
			// 
			this.PackagingDoneWizardPage.Controls.Add(this.infoPage1);
			this.PackagingDoneWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackagingDoneWizardPage.IsFinishPage = true;
			this.PackagingDoneWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackagingDoneWizardPage.Name = "PackagingDoneWizardPage";
			this.PackagingDoneWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackagingDoneWizardPage.TabIndex = 6;
			// 
			// infoPage1
			// 
			this.infoPage1.BackColor = System.Drawing.Color.White;
			this.infoPage1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.infoPage1.Image = ((System.Drawing.Image)(resources.GetObject("infoPage1.Image")));
			this.infoPage1.Location = new System.Drawing.Point(0, 0);
			this.infoPage1.Name = "infoPage1";
			this.infoPage1.PageText = "This asset will now be packaged into the assigned packages with the add and remov" +
				"e commands you specified.";
			this.infoPage1.PageTitle = "Package Setup Complete!";
			this.infoPage1.Size = new System.Drawing.Size(458, 307);
			this.infoPage1.TabIndex = 0;
			// 
			// PackageLateResolversWizardPage
			// 
			this.PackageLateResolversWizardPage.Controls.Add(this.PackageLateResolverTextBox);
			this.PackageLateResolversWizardPage.Controls.Add(this.label16);
			this.PackageLateResolversWizardPage.Controls.Add(this.header18);
			this.PackageLateResolversWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageLateResolversWizardPage.IsFinishPage = false;
			this.PackageLateResolversWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageLateResolversWizardPage.Name = "PackageLateResolversWizardPage";
			this.PackageLateResolversWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageLateResolversWizardPage.TabIndex = 23;
			// 
			// PackageLateResolverTextBox
			// 
			this.PackageLateResolverTextBox.Location = new System.Drawing.Point(80, 112);
			this.PackageLateResolverTextBox.Name = "PackageLateResolverTextBox";
			this.PackageLateResolverTextBox.Size = new System.Drawing.Size(360, 21);
			this.PackageLateResolverTextBox.TabIndex = 26;
			this.PackageLateResolverTextBox.Text = "";
			// 
			// label16
			// 
			this.label16.Image = ((System.Drawing.Image)(resources.GetObject("label16.Image")));
			this.label16.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label16.Location = new System.Drawing.Point(8, 112);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(72, 16);
			this.label16.TabIndex = 28;
			this.label16.Text = "     Command";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// header18
			// 
			this.header18.BackColor = System.Drawing.SystemColors.Control;
			this.header18.CausesValidation = false;
			this.header18.Description = "";
			this.header18.Dock = System.Windows.Forms.DockStyle.Top;
			this.header18.Image = ((System.Drawing.Image)(resources.GetObject("header18.Image")));
			this.header18.Location = new System.Drawing.Point(0, 0);
			this.header18.Name = "header18";
			this.header18.Size = new System.Drawing.Size(458, 64);
			this.header18.TabIndex = 27;
			this.header18.Title = "Late resolver command";
			// 
			// PackageDependanciesWizardPage
			// 
			this.PackageDependanciesWizardPage.Controls.Add(this.groupBox9);
			this.PackageDependanciesWizardPage.Controls.Add(this.header17);
			this.PackageDependanciesWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageDependanciesWizardPage.IsFinishPage = false;
			this.PackageDependanciesWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageDependanciesWizardPage.Name = "PackageDependanciesWizardPage";
			this.PackageDependanciesWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageDependanciesWizardPage.TabIndex = 22;
			this.PackageDependanciesWizardPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.PackageDependanciesWizardPage_CloseFromNext);
			// 
			// groupBox9
			// 
			this.groupBox9.Controls.Add(this.PackageDependencyNoRadioButton);
			this.groupBox9.Controls.Add(this.PackageDependencyYesRadioButton);
			this.groupBox9.Location = new System.Drawing.Point(20, 104);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Size = new System.Drawing.Size(416, 88);
			this.groupBox9.TabIndex = 18;
			this.groupBox9.TabStop = false;
			this.groupBox9.Text = "Do you have any package-to-package dependencies?";
			// 
			// PackageDependencyNoRadioButton
			// 
			this.PackageDependencyNoRadioButton.Checked = true;
			this.PackageDependencyNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageDependencyNoRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.PackageDependencyNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.PackageDependencyNoRadioButton.Name = "PackageDependencyNoRadioButton";
			this.PackageDependencyNoRadioButton.TabIndex = 1;
			this.PackageDependencyNoRadioButton.TabStop = true;
			this.PackageDependencyNoRadioButton.Text = "No";
			// 
			// PackageDependencyYesRadioButton
			// 
			this.PackageDependencyYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageDependencyYesRadioButton.Location = new System.Drawing.Point(24, 24);
			this.PackageDependencyYesRadioButton.Name = "PackageDependencyYesRadioButton";
			this.PackageDependencyYesRadioButton.TabIndex = 0;
			this.PackageDependencyYesRadioButton.Text = "Yes";
			// 
			// header17
			// 
			this.header17.BackColor = System.Drawing.SystemColors.Control;
			this.header17.CausesValidation = false;
			this.header17.Description = "Description";
			this.header17.Dock = System.Windows.Forms.DockStyle.Top;
			this.header17.Image = ((System.Drawing.Image)(resources.GetObject("header17.Image")));
			this.header17.Location = new System.Drawing.Point(0, 0);
			this.header17.Name = "header17";
			this.header17.Size = new System.Drawing.Size(458, 64);
			this.header17.TabIndex = 17;
			this.header17.Title = "Package Dependancies";
			// 
			// PackagingSingleCommandsWizardPage
			// 
			this.PackagingSingleCommandsWizardPage.Controls.Add(this.PackageSimpleRemoveMogArgumentsButton);
			this.PackagingSingleCommandsWizardPage.Controls.Add(this.PackageSimpleRemoveMogControl_ViewTokenTextBox);
			this.PackagingSingleCommandsWizardPage.Controls.Add(this.PackageSimpleAddMogControl_ViewTokenTextBox);
			this.PackagingSingleCommandsWizardPage.Controls.Add(this.PackageSimpleAddMogArgumentsButton);
			this.PackagingSingleCommandsWizardPage.Controls.Add(this.label2);
			this.PackagingSingleCommandsWizardPage.Controls.Add(this.label12);
			this.PackagingSingleCommandsWizardPage.Controls.Add(this.label11);
			this.PackagingSingleCommandsWizardPage.Controls.Add(this.header4);
			this.PackagingSingleCommandsWizardPage.Controls.Add(this.PackageSimpleRemoveTextBox);
			this.PackagingSingleCommandsWizardPage.Controls.Add(this.PackageSimpleAddTextBox);
			this.PackagingSingleCommandsWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackagingSingleCommandsWizardPage.IsFinishPage = false;
			this.PackagingSingleCommandsWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackagingSingleCommandsWizardPage.Name = "PackagingSingleCommandsWizardPage";
			this.PackagingSingleCommandsWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackagingSingleCommandsWizardPage.TabIndex = 5;
			this.PackagingSingleCommandsWizardPage.Visible = false;
			// 
			// PackageSimpleRemoveMogArgumentsButton
			// 
			this.PackageSimpleRemoveMogArgumentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PackageSimpleRemoveMogArgumentsButton.ButtonText = ">";
			this.PackageSimpleRemoveMogArgumentsButton.Location = new System.Drawing.Point(416, 181);
			this.PackageSimpleRemoveMogArgumentsButton.MOGAssetFilename = null;
			this.PackageSimpleRemoveMogArgumentsButton.Name = "PackageSimpleRemoveMogArgumentsButton";
			this.PackageSimpleRemoveMogArgumentsButton.Size = new System.Drawing.Size(24, 23);
			this.PackageSimpleRemoveMogArgumentsButton.TabIndex = 34;
			this.PackageSimpleRemoveMogArgumentsButton.TargetComboBox = null;
			this.PackageSimpleRemoveMogArgumentsButton.TargetTextBox = this.PackageSimpleRemoveTextBox;
			// 
			// PackageSimpleRemoveTextBox
			// 
			this.PackageSimpleRemoveTextBox.Location = new System.Drawing.Point(80, 181);
			this.PackageSimpleRemoveTextBox.Name = "PackageSimpleRemoveTextBox";
			this.PackageSimpleRemoveTextBox.Size = new System.Drawing.Size(328, 21);
			this.PackageSimpleRemoveTextBox.TabIndex = 1;
			this.PackageSimpleRemoveTextBox.Text = "";
			// 
			// PackageSimpleRemoveMogControl_ViewTokenTextBox
			// 
			this.PackageSimpleRemoveMogControl_ViewTokenTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PackageSimpleRemoveMogControl_ViewTokenTextBox.Location = new System.Drawing.Point(80, 208);
			this.PackageSimpleRemoveMogControl_ViewTokenTextBox.MOGAssetFilename = null;
			this.PackageSimpleRemoveMogControl_ViewTokenTextBox.Name = "PackageSimpleRemoveMogControl_ViewTokenTextBox";
			this.PackageSimpleRemoveMogControl_ViewTokenTextBox.Size = new System.Drawing.Size(330, 24);
			this.PackageSimpleRemoveMogControl_ViewTokenTextBox.SourceComboBox = null;
			this.PackageSimpleRemoveMogControl_ViewTokenTextBox.SourceTextBox = this.PackageSimpleRemoveTextBox;
			this.PackageSimpleRemoveMogControl_ViewTokenTextBox.TabIndex = 33;
			// 
			// PackageSimpleAddMogControl_ViewTokenTextBox
			// 
			this.PackageSimpleAddMogControl_ViewTokenTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PackageSimpleAddMogControl_ViewTokenTextBox.Location = new System.Drawing.Point(80, 152);
			this.PackageSimpleAddMogControl_ViewTokenTextBox.MOGAssetFilename = null;
			this.PackageSimpleAddMogControl_ViewTokenTextBox.Name = "PackageSimpleAddMogControl_ViewTokenTextBox";
			this.PackageSimpleAddMogControl_ViewTokenTextBox.Size = new System.Drawing.Size(330, 24);
			this.PackageSimpleAddMogControl_ViewTokenTextBox.SourceComboBox = null;
			this.PackageSimpleAddMogControl_ViewTokenTextBox.SourceTextBox = this.PackageSimpleAddTextBox;
			this.PackageSimpleAddMogControl_ViewTokenTextBox.TabIndex = 32;
			// 
			// PackageSimpleAddTextBox
			// 
			this.PackageSimpleAddTextBox.Location = new System.Drawing.Point(80, 126);
			this.PackageSimpleAddTextBox.Name = "PackageSimpleAddTextBox";
			this.PackageSimpleAddTextBox.Size = new System.Drawing.Size(328, 21);
			this.PackageSimpleAddTextBox.TabIndex = 0;
			this.PackageSimpleAddTextBox.Text = "";
			// 
			// PackageSimpleAddMogArgumentsButton
			// 
			this.PackageSimpleAddMogArgumentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PackageSimpleAddMogArgumentsButton.ButtonText = ">";
			this.PackageSimpleAddMogArgumentsButton.Location = new System.Drawing.Point(416, 125);
			this.PackageSimpleAddMogArgumentsButton.MOGAssetFilename = null;
			this.PackageSimpleAddMogArgumentsButton.Name = "PackageSimpleAddMogArgumentsButton";
			this.PackageSimpleAddMogArgumentsButton.Size = new System.Drawing.Size(24, 23);
			this.PackageSimpleAddMogArgumentsButton.TabIndex = 31;
			this.PackageSimpleAddMogArgumentsButton.TargetComboBox = null;
			this.PackageSimpleAddMogArgumentsButton.TargetTextBox = this.PackageSimpleAddTextBox;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(312, 32);
			this.label2.TabIndex = 16;
			this.label2.Text = "Add Example: PkZip -a {PackageFile} {AssetFilesName}    Remove Example: PkZip -r " +
				"{PackageFile} {AssetFilesName}";
			// 
			// label12
			// 
			this.label12.Image = ((System.Drawing.Image)(resources.GetObject("label12.Image")));
			this.label12.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label12.Location = new System.Drawing.Point(8, 128);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(48, 16);
			this.label12.TabIndex = 14;
			this.label12.Text = "     Add";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label11
			// 
			this.label11.Image = ((System.Drawing.Image)(resources.GetObject("label11.Image")));
			this.label11.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label11.Location = new System.Drawing.Point(8, 181);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(72, 16);
			this.label11.TabIndex = 15;
			this.label11.Text = "      Remove";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// header4
			// 
			this.header4.BackColor = System.Drawing.SystemColors.Control;
			this.header4.CausesValidation = false;
			this.header4.Description = "Enter the full command including the tool to add and remove an asset to your pack" +
				"age file.";
			this.header4.Dock = System.Windows.Forms.DockStyle.Top;
			this.header4.Image = ((System.Drawing.Image)(resources.GetObject("header4.Image")));
			this.header4.Location = new System.Drawing.Point(0, 0);
			this.header4.Name = "header4";
			this.header4.Size = new System.Drawing.Size(458, 64);
			this.header4.TabIndex = 2;
			this.header4.Title = "Custom Packaging Commands";
			// 
			// PackageCommandsWizardPage
			// 
			this.PackageCommandsWizardPage.Controls.Add(this.PackageDefaultRemoveCommandMogArgumentsButton);
			this.PackageCommandsWizardPage.Controls.Add(this.PackageDefaultRemoveCommandMogControl_ViewTokenTextBox);
			this.PackageCommandsWizardPage.Controls.Add(this.PackageDefaultAddCommandMogControl_ViewTokenTextBox);
			this.PackageCommandsWizardPage.Controls.Add(this.PackageDefaultAddCommandMogArgumentsButton);
			this.PackageCommandsWizardPage.Controls.Add(this.PackageDefaultAddCommandTextBox);
			this.PackageCommandsWizardPage.Controls.Add(this.PackageDefaultRemoveCommandTextBox);
			this.PackageCommandsWizardPage.Controls.Add(this.label14);
			this.PackageCommandsWizardPage.Controls.Add(this.label15);
			this.PackageCommandsWizardPage.Controls.Add(this.header16);
			this.PackageCommandsWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageCommandsWizardPage.IsFinishPage = false;
			this.PackageCommandsWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageCommandsWizardPage.Name = "PackageCommandsWizardPage";
			this.PackageCommandsWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageCommandsWizardPage.TabIndex = 21;
			// 
			// PackageDefaultRemoveCommandMogArgumentsButton
			// 
			this.PackageDefaultRemoveCommandMogArgumentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PackageDefaultRemoveCommandMogArgumentsButton.ButtonText = ">";
			this.PackageDefaultRemoveCommandMogArgumentsButton.Location = new System.Drawing.Point(416, 144);
			this.PackageDefaultRemoveCommandMogArgumentsButton.MOGAssetFilename = null;
			this.PackageDefaultRemoveCommandMogArgumentsButton.Name = "PackageDefaultRemoveCommandMogArgumentsButton";
			this.PackageDefaultRemoveCommandMogArgumentsButton.Size = new System.Drawing.Size(24, 23);
			this.PackageDefaultRemoveCommandMogArgumentsButton.TabIndex = 30;
			this.PackageDefaultRemoveCommandMogArgumentsButton.TargetComboBox = null;
			this.PackageDefaultRemoveCommandMogArgumentsButton.TargetTextBox = this.PackageDefaultRemoveCommandTextBox;
			// 
			// PackageDefaultRemoveCommandTextBox
			// 
			this.PackageDefaultRemoveCommandTextBox.Location = new System.Drawing.Point(80, 144);
			this.PackageDefaultRemoveCommandTextBox.Name = "PackageDefaultRemoveCommandTextBox";
			this.PackageDefaultRemoveCommandTextBox.Size = new System.Drawing.Size(328, 21);
			this.PackageDefaultRemoveCommandTextBox.TabIndex = 23;
			this.PackageDefaultRemoveCommandTextBox.Text = "";
			// 
			// PackageDefaultRemoveCommandMogControl_ViewTokenTextBox
			// 
			this.PackageDefaultRemoveCommandMogControl_ViewTokenTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PackageDefaultRemoveCommandMogControl_ViewTokenTextBox.Location = new System.Drawing.Point(80, 168);
			this.PackageDefaultRemoveCommandMogControl_ViewTokenTextBox.MOGAssetFilename = null;
			this.PackageDefaultRemoveCommandMogControl_ViewTokenTextBox.Name = "PackageDefaultRemoveCommandMogControl_ViewTokenTextBox";
			this.PackageDefaultRemoveCommandMogControl_ViewTokenTextBox.Size = new System.Drawing.Size(330, 24);
			this.PackageDefaultRemoveCommandMogControl_ViewTokenTextBox.SourceComboBox = null;
			this.PackageDefaultRemoveCommandMogControl_ViewTokenTextBox.SourceTextBox = this.PackageDefaultRemoveCommandTextBox;
			this.PackageDefaultRemoveCommandMogControl_ViewTokenTextBox.TabIndex = 29;
			// 
			// PackageDefaultAddCommandMogControl_ViewTokenTextBox
			// 
			this.PackageDefaultAddCommandMogControl_ViewTokenTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PackageDefaultAddCommandMogControl_ViewTokenTextBox.Location = new System.Drawing.Point(80, 112);
			this.PackageDefaultAddCommandMogControl_ViewTokenTextBox.MOGAssetFilename = null;
			this.PackageDefaultAddCommandMogControl_ViewTokenTextBox.Name = "PackageDefaultAddCommandMogControl_ViewTokenTextBox";
			this.PackageDefaultAddCommandMogControl_ViewTokenTextBox.Size = new System.Drawing.Size(330, 24);
			this.PackageDefaultAddCommandMogControl_ViewTokenTextBox.SourceComboBox = null;
			this.PackageDefaultAddCommandMogControl_ViewTokenTextBox.SourceTextBox = this.PackageDefaultAddCommandTextBox;
			this.PackageDefaultAddCommandMogControl_ViewTokenTextBox.TabIndex = 28;
			// 
			// PackageDefaultAddCommandTextBox
			// 
			this.PackageDefaultAddCommandTextBox.Location = new System.Drawing.Point(80, 88);
			this.PackageDefaultAddCommandTextBox.Name = "PackageDefaultAddCommandTextBox";
			this.PackageDefaultAddCommandTextBox.Size = new System.Drawing.Size(328, 21);
			this.PackageDefaultAddCommandTextBox.TabIndex = 22;
			this.PackageDefaultAddCommandTextBox.Text = "";
			// 
			// PackageDefaultAddCommandMogArgumentsButton
			// 
			this.PackageDefaultAddCommandMogArgumentsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PackageDefaultAddCommandMogArgumentsButton.ButtonText = ">";
			this.PackageDefaultAddCommandMogArgumentsButton.Location = new System.Drawing.Point(416, 88);
			this.PackageDefaultAddCommandMogArgumentsButton.MOGAssetFilename = null;
			this.PackageDefaultAddCommandMogArgumentsButton.Name = "PackageDefaultAddCommandMogArgumentsButton";
			this.PackageDefaultAddCommandMogArgumentsButton.Size = new System.Drawing.Size(24, 23);
			this.PackageDefaultAddCommandMogArgumentsButton.TabIndex = 27;
			this.PackageDefaultAddCommandMogArgumentsButton.TargetComboBox = null;
			this.PackageDefaultAddCommandMogArgumentsButton.TargetTextBox = this.PackageDefaultAddCommandTextBox;
			// 
			// label14
			// 
			this.label14.Image = ((System.Drawing.Image)(resources.GetObject("label14.Image")));
			this.label14.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label14.Location = new System.Drawing.Point(8, 88);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(48, 16);
			this.label14.TabIndex = 25;
			this.label14.Text = "     Add";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label15
			// 
			this.label15.Image = ((System.Drawing.Image)(resources.GetObject("label15.Image")));
			this.label15.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label15.Location = new System.Drawing.Point(8, 144);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(72, 16);
			this.label15.TabIndex = 26;
			this.label15.Text = "      Remove";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// header16
			// 
			this.header16.BackColor = System.Drawing.SystemColors.Control;
			this.header16.CausesValidation = false;
			this.header16.Description = "Enter the full command including the tool to add and remove an assets to your pac" +
				"kage file.";
			this.header16.Dock = System.Windows.Forms.DockStyle.Top;
			this.header16.Image = ((System.Drawing.Image)(resources.GetObject("header16.Image")));
			this.header16.Location = new System.Drawing.Point(0, 0);
			this.header16.Name = "header16";
			this.header16.Size = new System.Drawing.Size(458, 64);
			this.header16.TabIndex = 24;
			this.header16.Title = "Default Package Commands";
			// 
			// PackageCustomAddWizardPage
			// 
			this.PackageCustomAddWizardPage.Controls.Add(this.groupBox8);
			this.PackageCustomAddWizardPage.Controls.Add(this.header15);
			this.PackageCustomAddWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageCustomAddWizardPage.IsFinishPage = false;
			this.PackageCustomAddWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageCustomAddWizardPage.Name = "PackageCustomAddWizardPage";
			this.PackageCustomAddWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageCustomAddWizardPage.TabIndex = 20;
			this.PackageCustomAddWizardPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.PackageCustomAddWizardPage_CloseFromNext);
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.PackageCustomCommandNoRadioButton);
			this.groupBox8.Controls.Add(this.PackageCustomCommandYesRadioButton);
			this.groupBox8.Location = new System.Drawing.Point(20, 104);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(416, 88);
			this.groupBox8.TabIndex = 16;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Do you want to provide custom add and remove commands for this asset?";
			// 
			// PackageCustomCommandNoRadioButton
			// 
			this.PackageCustomCommandNoRadioButton.Checked = true;
			this.PackageCustomCommandNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageCustomCommandNoRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.PackageCustomCommandNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.PackageCustomCommandNoRadioButton.Name = "PackageCustomCommandNoRadioButton";
			this.PackageCustomCommandNoRadioButton.TabIndex = 1;
			this.PackageCustomCommandNoRadioButton.TabStop = true;
			this.PackageCustomCommandNoRadioButton.Text = "No";
			// 
			// PackageCustomCommandYesRadioButton
			// 
			this.PackageCustomCommandYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageCustomCommandYesRadioButton.Location = new System.Drawing.Point(24, 24);
			this.PackageCustomCommandYesRadioButton.Name = "PackageCustomCommandYesRadioButton";
			this.PackageCustomCommandYesRadioButton.TabIndex = 0;
			this.PackageCustomCommandYesRadioButton.Text = "Yes";
			// 
			// header15
			// 
			this.header15.BackColor = System.Drawing.SystemColors.Control;
			this.header15.CausesValidation = false;
			this.header15.Description = "Description";
			this.header15.Dock = System.Windows.Forms.DockStyle.Top;
			this.header15.Image = ((System.Drawing.Image)(resources.GetObject("header15.Image")));
			this.header15.Location = new System.Drawing.Point(0, 0);
			this.header15.Name = "header15";
			this.header15.Size = new System.Drawing.Size(458, 64);
			this.header15.TabIndex = 15;
			this.header15.Title = "Custom addition commands";
			// 
			// PackagingAssignmentsWizardPage
			// 
			this.PackagingAssignmentsWizardPage.Controls.Add(this.PackageManagementButton);
			this.PackagingAssignmentsWizardPage.Controls.Add(this.header1);
			this.PackagingAssignmentsWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackagingAssignmentsWizardPage.IsFinishPage = false;
			this.PackagingAssignmentsWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackagingAssignmentsWizardPage.Name = "PackagingAssignmentsWizardPage";
			this.PackagingAssignmentsWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackagingAssignmentsWizardPage.TabIndex = 7;
			this.PackagingAssignmentsWizardPage.ShowFromNext += new System.EventHandler(this.PackagingAssignmentsWizardPage_ShowFromNext);
			// 
			// PackageManagementButton
			// 
			this.PackageManagementButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageManagementButton.Location = new System.Drawing.Point(8, 80);
			this.PackageManagementButton.Name = "PackageManagementButton";
			this.PackageManagementButton.Size = new System.Drawing.Size(216, 23);
			this.PackageManagementButton.TabIndex = 1;
			this.PackageManagementButton.Text = "Launch Package Assignment Editor...";
			this.PackageManagementButton.Click += new System.EventHandler(this.PackageManagementButton_Click);
			// 
			// header1
			// 
			this.header1.BackColor = System.Drawing.SystemColors.Control;
			this.header1.CausesValidation = false;
			this.header1.Description = "All packaged assets need package assignments.";
			this.header1.Dock = System.Windows.Forms.DockStyle.Top;
			this.header1.Image = ((System.Drawing.Image)(resources.GetObject("header1.Image")));
			this.header1.Location = new System.Drawing.Point(0, 0);
			this.header1.Name = "header1";
			this.header1.Size = new System.Drawing.Size(458, 64);
			this.header1.TabIndex = 0;
			this.header1.Title = "Package Assignments";
			// 
			// PackageAsynchWizardPage
			// 
			this.PackageAsynchWizardPage.Controls.Add(this.groupBox6);
			this.PackageAsynchWizardPage.Controls.Add(this.header14);
			this.PackageAsynchWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageAsynchWizardPage.IsFinishPage = false;
			this.PackageAsynchWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageAsynchWizardPage.Name = "PackageAsynchWizardPage";
			this.PackageAsynchWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageAsynchWizardPage.TabIndex = 19;
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.PackageAsyncNoRadioButton);
			this.groupBox6.Controls.Add(this.PackageAsyncYesRadioButton);
			this.groupBox6.Location = new System.Drawing.Point(20, 104);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(416, 88);
			this.groupBox6.TabIndex = 16;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Do you want simultaneous packaging across all slaves?";
			// 
			// PackageAsyncNoRadioButton
			// 
			this.PackageAsyncNoRadioButton.Checked = true;
			this.PackageAsyncNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageAsyncNoRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.PackageAsyncNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.PackageAsyncNoRadioButton.Name = "PackageAsyncNoRadioButton";
			this.PackageAsyncNoRadioButton.TabIndex = 1;
			this.PackageAsyncNoRadioButton.TabStop = true;
			this.PackageAsyncNoRadioButton.Text = "No";
			// 
			// PackageAsyncYesRadioButton
			// 
			this.PackageAsyncYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageAsyncYesRadioButton.Location = new System.Drawing.Point(24, 24);
			this.PackageAsyncYesRadioButton.Name = "PackageAsyncYesRadioButton";
			this.PackageAsyncYesRadioButton.TabIndex = 0;
			this.PackageAsyncYesRadioButton.Text = "Yes";
			// 
			// header14
			// 
			this.header14.BackColor = System.Drawing.SystemColors.Control;
			this.header14.CausesValidation = false;
			this.header14.Description = "Description";
			this.header14.Dock = System.Windows.Forms.DockStyle.Top;
			this.header14.Image = ((System.Drawing.Image)(resources.GetObject("header14.Image")));
			this.header14.Location = new System.Drawing.Point(0, 0);
			this.header14.Name = "header14";
			this.header14.Size = new System.Drawing.Size(458, 64);
			this.header14.TabIndex = 15;
			this.header14.Title = "Asynch Packaging";
			// 
			// PackageClusterWizardPage
			// 
			this.PackageClusterWizardPage.Controls.Add(this.groupBox5);
			this.PackageClusterWizardPage.Controls.Add(this.header13);
			this.PackageClusterWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageClusterWizardPage.IsFinishPage = false;
			this.PackageClusterWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageClusterWizardPage.Name = "PackageClusterWizardPage";
			this.PackageClusterWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageClusterWizardPage.TabIndex = 18;
			this.PackageClusterWizardPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.PackageClusterWizardPage_CloseFromNext);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.PackageClusterNoRadioButton);
			this.groupBox5.Controls.Add(this.PackageClusterYesRadioButton);
			this.groupBox5.Location = new System.Drawing.Point(20, 104);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(416, 88);
			this.groupBox5.TabIndex = 16;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Do you want to cluster the packaging together on a single slave?";
			// 
			// PackageClusterNoRadioButton
			// 
			this.PackageClusterNoRadioButton.Checked = true;
			this.PackageClusterNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageClusterNoRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.PackageClusterNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.PackageClusterNoRadioButton.Name = "PackageClusterNoRadioButton";
			this.PackageClusterNoRadioButton.TabIndex = 1;
			this.PackageClusterNoRadioButton.TabStop = true;
			this.PackageClusterNoRadioButton.Text = "No";
			// 
			// PackageClusterYesRadioButton
			// 
			this.PackageClusterYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageClusterYesRadioButton.Location = new System.Drawing.Point(24, 24);
			this.PackageClusterYesRadioButton.Name = "PackageClusterYesRadioButton";
			this.PackageClusterYesRadioButton.TabIndex = 0;
			this.PackageClusterYesRadioButton.Text = "Yes";
			// 
			// header13
			// 
			this.header13.BackColor = System.Drawing.SystemColors.Control;
			this.header13.CausesValidation = false;
			this.header13.Description = "Description";
			this.header13.Dock = System.Windows.Forms.DockStyle.Top;
			this.header13.Image = ((System.Drawing.Image)(resources.GetObject("header13.Image")));
			this.header13.Location = new System.Drawing.Point(0, 0);
			this.header13.Name = "header13";
			this.header13.Size = new System.Drawing.Size(458, 64);
			this.header13.TabIndex = 15;
			this.header13.Title = "Cluster packaging";
			// 
			// PackageCleanWizardPage
			// 
			this.PackageCleanWizardPage.Controls.Add(this.groupBox4);
			this.PackageCleanWizardPage.Controls.Add(this.header12);
			this.PackageCleanWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageCleanWizardPage.IsFinishPage = false;
			this.PackageCleanWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageCleanWizardPage.Name = "PackageCleanWizardPage";
			this.PackageCleanWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageCleanWizardPage.TabIndex = 17;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.PackageCleanupDirNoRadioButton);
			this.groupBox4.Controls.Add(this.PackageCleanupDirYesRadioButton);
			this.groupBox4.Location = new System.Drawing.Point(20, 104);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(416, 88);
			this.groupBox4.TabIndex = 16;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Does this package need to cleanup packaging directories after merge?";
			// 
			// PackageCleanupDirNoRadioButton
			// 
			this.PackageCleanupDirNoRadioButton.Checked = true;
			this.PackageCleanupDirNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageCleanupDirNoRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.PackageCleanupDirNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.PackageCleanupDirNoRadioButton.Name = "PackageCleanupDirNoRadioButton";
			this.PackageCleanupDirNoRadioButton.TabIndex = 1;
			this.PackageCleanupDirNoRadioButton.TabStop = true;
			this.PackageCleanupDirNoRadioButton.Text = "No";
			// 
			// PackageCleanupDirYesRadioButton
			// 
			this.PackageCleanupDirYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageCleanupDirYesRadioButton.Location = new System.Drawing.Point(24, 24);
			this.PackageCleanupDirYesRadioButton.Name = "PackageCleanupDirYesRadioButton";
			this.PackageCleanupDirYesRadioButton.TabIndex = 0;
			this.PackageCleanupDirYesRadioButton.Text = "Yes";
			// 
			// header12
			// 
			this.header12.BackColor = System.Drawing.SystemColors.Control;
			this.header12.CausesValidation = false;
			this.header12.Description = "Description";
			this.header12.Dock = System.Windows.Forms.DockStyle.Top;
			this.header12.Image = ((System.Drawing.Image)(resources.GetObject("header12.Image")));
			this.header12.Location = new System.Drawing.Point(0, 0);
			this.header12.Name = "header12";
			this.header12.Size = new System.Drawing.Size(458, 64);
			this.header12.TabIndex = 15;
			this.header12.Title = "Cleanup packaging directories";
			// 
			// PackageEventsWizardPage
			// 
			this.PackageEventsWizardPage.Controls.Add(this.PackagePostEventExploreButton);
			this.PackageEventsWizardPage.Controls.Add(this.PackagePreEventExploreButton);
			this.PackageEventsWizardPage.Controls.Add(this.label9);
			this.PackageEventsWizardPage.Controls.Add(this.PackagePostEventTextBox);
			this.PackageEventsWizardPage.Controls.Add(this.label13);
			this.PackageEventsWizardPage.Controls.Add(this.PackagePreEventTextBox);
			this.PackageEventsWizardPage.Controls.Add(this.header11);
			this.PackageEventsWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageEventsWizardPage.IsFinishPage = false;
			this.PackageEventsWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageEventsWizardPage.Name = "PackageEventsWizardPage";
			this.PackageEventsWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageEventsWizardPage.TabIndex = 16;
			// 
			// PackagePostEventExploreButton
			// 
			this.PackagePostEventExploreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PackagePostEventExploreButton.Location = new System.Drawing.Point(418, 160);
			this.PackagePostEventExploreButton.Name = "PackagePostEventExploreButton";
			this.PackagePostEventExploreButton.Size = new System.Drawing.Size(24, 21);
			this.PackagePostEventExploreButton.TabIndex = 26;
			this.PackagePostEventExploreButton.Text = "...";
			this.PackagePostEventExploreButton.Click += new System.EventHandler(this.PackagePostEventExploreButton_Click);
			// 
			// PackagePreEventExploreButton
			// 
			this.PackagePreEventExploreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PackagePreEventExploreButton.Location = new System.Drawing.Point(418, 104);
			this.PackagePreEventExploreButton.Name = "PackagePreEventExploreButton";
			this.PackagePreEventExploreButton.Size = new System.Drawing.Size(24, 21);
			this.PackagePreEventExploreButton.TabIndex = 25;
			this.PackagePreEventExploreButton.Text = "...";
			this.PackagePreEventExploreButton.Click += new System.EventHandler(this.PackagePreEventExploreButton_Click);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(16, 136);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(392, 23);
			this.label9.TabIndex = 24;
			this.label9.Text = "Enter or browse to the Post-prackage event or tool:";
			// 
			// PackagePostEventTextBox
			// 
			this.PackagePostEventTextBox.Location = new System.Drawing.Point(16, 160);
			this.PackagePostEventTextBox.Name = "PackagePostEventTextBox";
			this.PackagePostEventTextBox.Size = new System.Drawing.Size(392, 21);
			this.PackagePostEventTextBox.TabIndex = 23;
			this.PackagePostEventTextBox.Text = "";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(16, 80);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(392, 23);
			this.label13.TabIndex = 22;
			this.label13.Text = "Enter  or browse to the Pre-package event or tool:";
			// 
			// PackagePreEventTextBox
			// 
			this.PackagePreEventTextBox.Location = new System.Drawing.Point(16, 104);
			this.PackagePreEventTextBox.Name = "PackagePreEventTextBox";
			this.PackagePreEventTextBox.Size = new System.Drawing.Size(392, 21);
			this.PackagePreEventTextBox.TabIndex = 21;
			this.PackagePreEventTextBox.Text = "";
			// 
			// header11
			// 
			this.header11.BackColor = System.Drawing.SystemColors.Control;
			this.header11.CausesValidation = false;
			this.header11.Description = "Description";
			this.header11.Dock = System.Windows.Forms.DockStyle.Top;
			this.header11.Image = ((System.Drawing.Image)(resources.GetObject("header11.Image")));
			this.header11.Location = new System.Drawing.Point(0, 0);
			this.header11.Name = "header11";
			this.header11.Size = new System.Drawing.Size(458, 64);
			this.header11.TabIndex = 20;
			this.header11.Title = "Enter Pre or Post pacakge events";
			// 
			// PackageNeedEventsWizardPage
			// 
			this.PackageNeedEventsWizardPage.Controls.Add(this.groupBox3);
			this.PackageNeedEventsWizardPage.Controls.Add(this.header10);
			this.PackageNeedEventsWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageNeedEventsWizardPage.IsFinishPage = false;
			this.PackageNeedEventsWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageNeedEventsWizardPage.Name = "PackageNeedEventsWizardPage";
			this.PackageNeedEventsWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageNeedEventsWizardPage.TabIndex = 15;
			this.PackageNeedEventsWizardPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.PackageNeedEventsWizardPage_CloseFromNext);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.PackageNeedEventsNoRadioButton);
			this.groupBox3.Controls.Add(this.PackageNeedEventsYesRadioButton);
			this.groupBox3.Location = new System.Drawing.Point(20, 107);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(416, 88);
			this.groupBox3.TabIndex = 14;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Does this package need pre or post packaging events?";
			// 
			// PackageNeedEventsNoRadioButton
			// 
			this.PackageNeedEventsNoRadioButton.Checked = true;
			this.PackageNeedEventsNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageNeedEventsNoRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.PackageNeedEventsNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.PackageNeedEventsNoRadioButton.Name = "PackageNeedEventsNoRadioButton";
			this.PackageNeedEventsNoRadioButton.TabIndex = 1;
			this.PackageNeedEventsNoRadioButton.TabStop = true;
			this.PackageNeedEventsNoRadioButton.Text = "No";
			// 
			// PackageNeedEventsYesRadioButton
			// 
			this.PackageNeedEventsYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageNeedEventsYesRadioButton.Location = new System.Drawing.Point(24, 24);
			this.PackageNeedEventsYesRadioButton.Name = "PackageNeedEventsYesRadioButton";
			this.PackageNeedEventsYesRadioButton.TabIndex = 0;
			this.PackageNeedEventsYesRadioButton.Text = "Yes";
			// 
			// header10
			// 
			this.header10.BackColor = System.Drawing.SystemColors.Control;
			this.header10.CausesValidation = false;
			this.header10.Description = "Description";
			this.header10.Dock = System.Windows.Forms.DockStyle.Top;
			this.header10.Image = ((System.Drawing.Image)(resources.GetObject("header10.Image")));
			this.header10.Location = new System.Drawing.Point(0, 0);
			this.header10.Name = "header10";
			this.header10.Size = new System.Drawing.Size(458, 64);
			this.header10.TabIndex = 0;
			this.header10.Title = "Pre and Post Events Required?";
			// 
			// PackageDeleteCommandWizardPage
			// 
			this.PackageDeleteCommandWizardPage.Controls.Add(this.label10);
			this.PackageDeleteCommandWizardPage.Controls.Add(this.PackageDeleteCommandTextBox);
			this.PackageDeleteCommandWizardPage.Controls.Add(this.header9);
			this.PackageDeleteCommandWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageDeleteCommandWizardPage.IsFinishPage = false;
			this.PackageDeleteCommandWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageDeleteCommandWizardPage.Name = "PackageDeleteCommandWizardPage";
			this.PackageDeleteCommandWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageDeleteCommandWizardPage.TabIndex = 14;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(16, 80);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(392, 23);
			this.label10.TabIndex = 22;
			this.label10.Text = "Enter the command for this packager to delete a selected package file:";
			// 
			// PackageDeleteCommandTextBox
			// 
			this.PackageDeleteCommandTextBox.Location = new System.Drawing.Point(16, 104);
			this.PackageDeleteCommandTextBox.Name = "PackageDeleteCommandTextBox";
			this.PackageDeleteCommandTextBox.Size = new System.Drawing.Size(424, 21);
			this.PackageDeleteCommandTextBox.TabIndex = 21;
			this.PackageDeleteCommandTextBox.Text = "";
			// 
			// header9
			// 
			this.header9.BackColor = System.Drawing.SystemColors.Control;
			this.header9.CausesValidation = false;
			this.header9.Description = "Description";
			this.header9.Dock = System.Windows.Forms.DockStyle.Top;
			this.header9.Image = ((System.Drawing.Image)(resources.GetObject("header9.Image")));
			this.header9.Location = new System.Drawing.Point(0, 0);
			this.header9.Name = "header9";
			this.header9.Size = new System.Drawing.Size(458, 64);
			this.header9.TabIndex = 20;
			this.header9.Title = "Task delete package command";
			// 
			// PackageTaskFilenameWizardPage
			// 
			this.PackageTaskFilenameWizardPage.Controls.Add(this.label8);
			this.PackageTaskFilenameWizardPage.Controls.Add(this.PackageTaskFileOutTextBox);
			this.PackageTaskFilenameWizardPage.Controls.Add(this.label7);
			this.PackageTaskFilenameWizardPage.Controls.Add(this.PackageTaskFileInTextBox);
			this.PackageTaskFilenameWizardPage.Controls.Add(this.header8);
			this.PackageTaskFilenameWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageTaskFilenameWizardPage.IsFinishPage = false;
			this.PackageTaskFilenameWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageTaskFilenameWizardPage.Name = "PackageTaskFilenameWizardPage";
			this.PackageTaskFilenameWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageTaskFilenameWizardPage.TabIndex = 13;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(16, 136);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(392, 23);
			this.label8.TabIndex = 19;
			this.label8.Text = "Enter the filename for the output task file:";
			// 
			// PackageTaskFileOutTextBox
			// 
			this.PackageTaskFileOutTextBox.Location = new System.Drawing.Point(16, 160);
			this.PackageTaskFileOutTextBox.Name = "PackageTaskFileOutTextBox";
			this.PackageTaskFileOutTextBox.Size = new System.Drawing.Size(424, 21);
			this.PackageTaskFileOutTextBox.TabIndex = 18;
			this.PackageTaskFileOutTextBox.Text = "";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(16, 80);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(392, 23);
			this.label7.TabIndex = 17;
			this.label7.Text = "Enter the filename for the input task file:";
			// 
			// PackageTaskFileInTextBox
			// 
			this.PackageTaskFileInTextBox.Location = new System.Drawing.Point(16, 104);
			this.PackageTaskFileInTextBox.Name = "PackageTaskFileInTextBox";
			this.PackageTaskFileInTextBox.Size = new System.Drawing.Size(424, 21);
			this.PackageTaskFileInTextBox.TabIndex = 16;
			this.PackageTaskFileInTextBox.Text = "";
			// 
			// header8
			// 
			this.header8.BackColor = System.Drawing.SystemColors.Control;
			this.header8.CausesValidation = false;
			this.header8.Description = "Description";
			this.header8.Dock = System.Windows.Forms.DockStyle.Top;
			this.header8.Image = ((System.Drawing.Image)(resources.GetObject("header8.Image")));
			this.header8.Location = new System.Drawing.Point(0, 0);
			this.header8.Name = "header8";
			this.header8.Size = new System.Drawing.Size(458, 64);
			this.header8.TabIndex = 0;
			this.header8.Title = "Task filenames";
			// 
			// PackageTaskFileToolWizardPage
			// 
			this.PackageTaskFileToolWizardPage.Controls.Add(this.label6);
			this.PackageTaskFileToolWizardPage.Controls.Add(this.PackageExplorePackagerButton);
			this.PackageTaskFileToolWizardPage.Controls.Add(this.PackagePackagerToolTextBox);
			this.PackageTaskFileToolWizardPage.Controls.Add(this.header7);
			this.PackageTaskFileToolWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageTaskFileToolWizardPage.IsFinishPage = false;
			this.PackageTaskFileToolWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageTaskFileToolWizardPage.Name = "PackageTaskFileToolWizardPage";
			this.PackageTaskFileToolWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageTaskFileToolWizardPage.TabIndex = 12;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 80);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(392, 23);
			this.label6.TabIndex = 15;
			this.label6.Text = "Enter or browse to the selected packager for this package:";
			// 
			// PackageExplorePackagerButton
			// 
			this.PackageExplorePackagerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PackageExplorePackagerButton.Location = new System.Drawing.Point(402, 104);
			this.PackageExplorePackagerButton.Name = "PackageExplorePackagerButton";
			this.PackageExplorePackagerButton.Size = new System.Drawing.Size(24, 21);
			this.PackageExplorePackagerButton.TabIndex = 14;
			this.PackageExplorePackagerButton.Text = "...";
			this.PackageExplorePackagerButton.Click += new System.EventHandler(this.PackageExplorePackagerButton_Click);
			// 
			// PackagePackagerToolTextBox
			// 
			this.PackagePackagerToolTextBox.Location = new System.Drawing.Point(16, 104);
			this.PackagePackagerToolTextBox.Name = "PackagePackagerToolTextBox";
			this.PackagePackagerToolTextBox.Size = new System.Drawing.Size(376, 21);
			this.PackagePackagerToolTextBox.TabIndex = 13;
			this.PackagePackagerToolTextBox.Text = "";
			// 
			// header7
			// 
			this.header7.BackColor = System.Drawing.SystemColors.Control;
			this.header7.CausesValidation = false;
			this.header7.Description = "Description";
			this.header7.Dock = System.Windows.Forms.DockStyle.Top;
			this.header7.Image = ((System.Drawing.Image)(resources.GetObject("header7.Image")));
			this.header7.Location = new System.Drawing.Point(0, 0);
			this.header7.Name = "header7";
			this.header7.Size = new System.Drawing.Size(458, 64);
			this.header7.TabIndex = 0;
			this.header7.Title = "Locate taskfile driven packager";
			// 
			// PackageStyleWizardPage
			// 
			this.PackageStyleWizardPage.Controls.Add(this.groupBox2);
			this.PackageStyleWizardPage.Controls.Add(this.header6);
			this.PackageStyleWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageStyleWizardPage.IsFinishPage = false;
			this.PackageStyleWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageStyleWizardPage.Name = "PackageStyleWizardPage";
			this.PackageStyleWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageStyleWizardPage.TabIndex = 11;
			this.PackageStyleWizardPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.PackageStyleWizardPage_CloseFromNext);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.PackageStyleTaskRadioButton);
			this.groupBox2.Controls.Add(this.PackageStyleSimpleRadioButton);
			this.groupBox2.Location = new System.Drawing.Point(20, 107);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(416, 88);
			this.groupBox2.TabIndex = 13;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "What is the style of this package?";
			// 
			// PackageStyleTaskRadioButton
			// 
			this.PackageStyleTaskRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageStyleTaskRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.PackageStyleTaskRadioButton.Location = new System.Drawing.Point(24, 56);
			this.PackageStyleTaskRadioButton.Name = "PackageStyleTaskRadioButton";
			this.PackageStyleTaskRadioButton.TabIndex = 1;
			this.PackageStyleTaskRadioButton.Text = "Task File";
			// 
			// PackageStyleSimpleRadioButton
			// 
			this.PackageStyleSimpleRadioButton.Checked = true;
			this.PackageStyleSimpleRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageStyleSimpleRadioButton.Location = new System.Drawing.Point(24, 24);
			this.PackageStyleSimpleRadioButton.Name = "PackageStyleSimpleRadioButton";
			this.PackageStyleSimpleRadioButton.TabIndex = 0;
			this.PackageStyleSimpleRadioButton.TabStop = true;
			this.PackageStyleSimpleRadioButton.Text = "Simple";
			// 
			// header6
			// 
			this.header6.BackColor = System.Drawing.SystemColors.Control;
			this.header6.CausesValidation = false;
			this.header6.Description = "Description";
			this.header6.Dock = System.Windows.Forms.DockStyle.Top;
			this.header6.Image = ((System.Drawing.Image)(resources.GetObject("header6.Image")));
			this.header6.Location = new System.Drawing.Point(0, 0);
			this.header6.Name = "header6";
			this.header6.Size = new System.Drawing.Size(458, 64);
			this.header6.TabIndex = 0;
			this.header6.Title = "Select Package Style";
			// 
			// PackageInsertSupportWizardPage
			// 
			this.PackageInsertSupportWizardPage.Controls.Add(this.groupBox1);
			this.PackageInsertSupportWizardPage.Controls.Add(this.header5);
			this.PackageInsertSupportWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageInsertSupportWizardPage.IsFinishPage = false;
			this.PackageInsertSupportWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageInsertSupportWizardPage.Name = "PackageInsertSupportWizardPage";
			this.PackageInsertSupportWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageInsertSupportWizardPage.TabIndex = 10;
			this.PackageInsertSupportWizardPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.PackageInsertSupportWizardPage_CloseFromNext);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.PackageInsertSupportNoRadioButton);
			this.groupBox1.Controls.Add(this.PackageInsertSupportYesRadioButton);
			this.groupBox1.Location = new System.Drawing.Point(20, 107);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(416, 88);
			this.groupBox1.TabIndex = 13;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Does this package support asset insertion and deletion?";
			// 
			// PackageInsertSupportNoRadioButton
			// 
			this.PackageInsertSupportNoRadioButton.Checked = true;
			this.PackageInsertSupportNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageInsertSupportNoRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.PackageInsertSupportNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.PackageInsertSupportNoRadioButton.Name = "PackageInsertSupportNoRadioButton";
			this.PackageInsertSupportNoRadioButton.TabIndex = 1;
			this.PackageInsertSupportNoRadioButton.TabStop = true;
			this.PackageInsertSupportNoRadioButton.Text = "No";
			// 
			// PackageInsertSupportYesRadioButton
			// 
			this.PackageInsertSupportYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageInsertSupportYesRadioButton.Location = new System.Drawing.Point(24, 24);
			this.PackageInsertSupportYesRadioButton.Name = "PackageInsertSupportYesRadioButton";
			this.PackageInsertSupportYesRadioButton.TabIndex = 0;
			this.PackageInsertSupportYesRadioButton.Text = "Yes";
			// 
			// header5
			// 
			this.header5.BackColor = System.Drawing.SystemColors.Control;
			this.header5.CausesValidation = false;
			this.header5.Description = "Description";
			this.header5.Dock = System.Windows.Forms.DockStyle.Top;
			this.header5.Image = ((System.Drawing.Image)(resources.GetObject("header5.Image")));
			this.header5.Location = new System.Drawing.Point(0, 0);
			this.header5.Name = "header5";
			this.header5.Size = new System.Drawing.Size(458, 64);
			this.header5.TabIndex = 0;
			this.header5.Title = "Insert support";
			// 
			// PackageCreatePackageWizardPage
			// 
			this.PackageCreatePackageWizardPage.Controls.Add(this.PackageCreateNewMogControl_PackageManagementTreeView);
			this.PackageCreatePackageWizardPage.Controls.Add(this.header3);
			this.PackageCreatePackageWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageCreatePackageWizardPage.IsFinishPage = false;
			this.PackageCreatePackageWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageCreatePackageWizardPage.Name = "PackageCreatePackageWizardPage";
			this.PackageCreatePackageWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageCreatePackageWizardPage.TabIndex = 9;
			this.PackageCreatePackageWizardPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.PackageCreatePackageWizardPage_CloseFromNext);
			this.PackageCreatePackageWizardPage.ShowFromNext += new System.EventHandler(this.PackageCreatePackageWizardPage_ShowFromNext);
			// 
			// PackageCreateNewMogControl_PackageManagementTreeView
			// 
			this.PackageCreateNewMogControl_PackageManagementTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageCreateNewMogControl_PackageManagementTreeView.ExpandAssets = false;
			this.PackageCreateNewMogControl_PackageManagementTreeView.Location = new System.Drawing.Point(0, 64);
			this.PackageCreateNewMogControl_PackageManagementTreeView.Name = "PackageCreateNewMogControl_PackageManagementTreeView";
			this.PackageCreateNewMogControl_PackageManagementTreeView.Size = new System.Drawing.Size(458, 243);
			this.PackageCreateNewMogControl_PackageManagementTreeView.TabIndex = 9;
			this.PackageCreateNewMogControl_PackageManagementTreeView.UsePlatformSpecificCheckBox = false;
			this.PackageCreateNewMogControl_PackageManagementTreeView.AfterPackageSelect += new TreeViewEventHandler(this.PackageCreateNewMogControl_PackageManagementTreeView_AfterPackageSelect);
			// 
			// header3
			// 
			this.header3.BackColor = System.Drawing.SystemColors.Control;
			this.header3.CausesValidation = false;
			this.header3.Description = "To create your package, browse to the desired classification and right click to o" +
				"pen the \'new package\' menu item.";
			this.header3.Dock = System.Windows.Forms.DockStyle.Top;
			this.header3.Image = ((System.Drawing.Image)(resources.GetObject("header3.Image")));
			this.header3.Location = new System.Drawing.Point(0, 0);
			this.header3.Name = "header3";
			this.header3.Size = new System.Drawing.Size(458, 64);
			this.header3.TabIndex = 0;
			this.header3.Title = "Create new package";
			// 
			// PackagePackageClassWizardPage
			// 
			this.PackagePackageClassWizardPage.Controls.Add(this.MogRepositoryTreeView);
			this.PackagePackageClassWizardPage.Controls.Add(this.header19);
			this.PackagePackageClassWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackagePackageClassWizardPage.IsFinishPage = false;
			this.PackagePackageClassWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackagePackageClassWizardPage.Name = "PackagePackageClassWizardPage";
			this.PackagePackageClassWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackagePackageClassWizardPage.TabIndex = 24;
			this.PackagePackageClassWizardPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.PackagePackageClassWizardPage_CloseFromNext);
			this.PackagePackageClassWizardPage.ShowFromNext += new System.EventHandler(this.PackagePackageClassWizardPage_ShowFromNext);
			// 
			// MogRepositoryTreeView
			// 
			this.MogRepositoryTreeView.ArchivedNodeForeColor = System.Drawing.SystemColors.WindowText;
			this.MogRepositoryTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MogRepositoryTreeView.ExpandAssets = false;
			this.MogRepositoryTreeView.ExpandPackageGroupAssets = true;
			this.MogRepositoryTreeView.ExpandPackageGroups = false;
			this.MogRepositoryTreeView.FocusForAssetNodes = LeafFocusLevel.RepositoryItems;
			this.MogRepositoryTreeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.MogRepositoryTreeView.HotTracking = true;
			this.MogRepositoryTreeView.Location = new System.Drawing.Point(0, 64);
			this.MogRepositoryTreeView.Name = "MogRepositoryTreeView";
			this.MogRepositoryTreeView.PathSeparator = "~";
			this.MogRepositoryTreeView.ShowAssets = false;
			this.MogRepositoryTreeView.Size = new System.Drawing.Size(458, 243);
			this.MogRepositoryTreeView.TabIndex = 4;
			// 
			// header19
			// 
			this.header19.BackColor = System.Drawing.SystemColors.Control;
			this.header19.CausesValidation = false;
			this.header19.Description = "Packages need to be housed under a classification that is set to hold packages.  " +
				"Select or create this classification.";
			this.header19.Dock = System.Windows.Forms.DockStyle.Top;
			this.header19.Image = ((System.Drawing.Image)(resources.GetObject("header19.Image")));
			this.header19.Location = new System.Drawing.Point(0, 0);
			this.header19.Name = "header19";
			this.header19.Size = new System.Drawing.Size(458, 64);
			this.header19.TabIndex = 1;
			this.header19.Title = "Select Package Classification.";
			// 
			// PackageNewPackageWizardPage
			// 
			this.PackageNewPackageWizardPage.Controls.Add(this.groupBox7);
			this.PackageNewPackageWizardPage.Controls.Add(this.header2);
			this.PackageNewPackageWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageNewPackageWizardPage.IsFinishPage = false;
			this.PackageNewPackageWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackageNewPackageWizardPage.Name = "PackageNewPackageWizardPage";
			this.PackageNewPackageWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackageNewPackageWizardPage.TabIndex = 8;
			this.PackageNewPackageWizardPage.CloseFromNext += new Gui.Wizard.PageEventHandler(this.PackageNewPackageWizardPage_CloseFromNext);
			this.PackageNewPackageWizardPage.ShowFromNext += new System.EventHandler(this.PackageNewPackageWizardPage_ShowFromNext);
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.PackageNewPackageNoRadioButton);
			this.groupBox7.Controls.Add(this.PackageNewPackageYesRadioButton);
			this.groupBox7.Location = new System.Drawing.Point(20, 107);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(416, 88);
			this.groupBox7.TabIndex = 12;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Does this asset have an existing package to go into?";
			// 
			// PackageNewPackageNoRadioButton
			// 
			this.PackageNewPackageNoRadioButton.Checked = true;
			this.PackageNewPackageNoRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageNewPackageNoRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.PackageNewPackageNoRadioButton.Location = new System.Drawing.Point(24, 56);
			this.PackageNewPackageNoRadioButton.Name = "PackageNewPackageNoRadioButton";
			this.PackageNewPackageNoRadioButton.TabIndex = 1;
			this.PackageNewPackageNoRadioButton.TabStop = true;
			this.PackageNewPackageNoRadioButton.Text = "No";
			// 
			// PackageNewPackageYesRadioButton
			// 
			this.PackageNewPackageYesRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PackageNewPackageYesRadioButton.Location = new System.Drawing.Point(24, 24);
			this.PackageNewPackageYesRadioButton.Name = "PackageNewPackageYesRadioButton";
			this.PackageNewPackageYesRadioButton.TabIndex = 0;
			this.PackageNewPackageYesRadioButton.Text = "Yes";
			// 
			// header2
			// 
			this.header2.BackColor = System.Drawing.SystemColors.Control;
			this.header2.CausesValidation = false;
			this.header2.Description = "Description";
			this.header2.Dock = System.Windows.Forms.DockStyle.Top;
			this.header2.Image = ((System.Drawing.Image)(resources.GetObject("header2.Image")));
			this.header2.Location = new System.Drawing.Point(0, 0);
			this.header2.Name = "header2";
			this.header2.Size = new System.Drawing.Size(458, 64);
			this.header2.TabIndex = 0;
			this.header2.Title = "Is this a new package?";
			// 
			// PackagingStartWizardPage
			// 
			this.PackagingStartWizardPage.Controls.Add(this.PackagingStartInfoPage);
			this.PackagingStartWizardPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackagingStartWizardPage.IsFinishPage = false;
			this.PackagingStartWizardPage.Location = new System.Drawing.Point(0, 0);
			this.PackagingStartWizardPage.Name = "PackagingStartWizardPage";
			this.PackagingStartWizardPage.Size = new System.Drawing.Size(458, 307);
			this.PackagingStartWizardPage.TabIndex = 1;
			// 
			// PackagingStartInfoPage
			// 
			this.PackagingStartInfoPage.BackColor = System.Drawing.Color.White;
			this.PackagingStartInfoPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackagingStartInfoPage.Image = ((System.Drawing.Image)(resources.GetObject("PackagingStartInfoPage.Image")));
			this.PackagingStartInfoPage.Location = new System.Drawing.Point(0, 0);
			this.PackagingStartInfoPage.Name = "PackagingStartInfoPage";
			this.PackagingStartInfoPage.PageText = "This wizard help you setup one or more assets as a packaged asset.  Though a seri" +
				"es of questions, we will record all the properties required to complete this pac" +
				"kaged asset.";
			this.PackagingStartInfoPage.PageTitle = "Welcome to the Package Setup Wizard";
			this.PackagingStartInfoPage.Size = new System.Drawing.Size(458, 307);
			this.PackagingStartInfoPage.TabIndex = 0;
			// 
			// WizardMog_XpProgressBar
			// 
			this.WizardMog_XpProgressBar.ColorBackGround = System.Drawing.Color.White;
			this.WizardMog_XpProgressBar.ColorBarBorder = System.Drawing.Color.DodgerBlue;
			this.WizardMog_XpProgressBar.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(1)), ((System.Byte)(50)), ((System.Byte)(131)));
			this.WizardMog_XpProgressBar.ColorText = System.Drawing.Color.Black;
			this.WizardMog_XpProgressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.WizardMog_XpProgressBar.Location = new System.Drawing.Point(0, 355);
			this.WizardMog_XpProgressBar.Name = "WizardMog_XpProgressBar";
			this.WizardMog_XpProgressBar.Position = 0;
			this.WizardMog_XpProgressBar.PositionMax = 100;
			this.WizardMog_XpProgressBar.PositionMin = 0;
			this.WizardMog_XpProgressBar.Size = new System.Drawing.Size(458, 12);
			this.WizardMog_XpProgressBar.TabIndex = 1;
			// 
			// SetupPackagingWizard
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(458, 367);
			this.Controls.Add(this.Wizard);
			this.Controls.Add(this.WizardMog_XpProgressBar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SetupPackagingWizard";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Setup Packaging Wizard";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.SetupPackagingWizard_Closing);
			this.Load += new System.EventHandler(this.SetupPackagingWizard_Load);
			this.Wizard.ResumeLayout(false);
			this.PackagingDoneWizardPage.ResumeLayout(false);
			this.PackageLateResolversWizardPage.ResumeLayout(false);
			this.PackageDependanciesWizardPage.ResumeLayout(false);
			this.groupBox9.ResumeLayout(false);
			this.PackagingSingleCommandsWizardPage.ResumeLayout(false);
			this.PackageCommandsWizardPage.ResumeLayout(false);
			this.PackageCustomAddWizardPage.ResumeLayout(false);
			this.groupBox8.ResumeLayout(false);
			this.PackagingAssignmentsWizardPage.ResumeLayout(false);
			this.PackageAsynchWizardPage.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.PackageClusterWizardPage.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.PackageCleanWizardPage.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.PackageEventsWizardPage.ResumeLayout(false);
			this.PackageNeedEventsWizardPage.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.PackageDeleteCommandWizardPage.ResumeLayout(false);
			this.PackageTaskFilenameWizardPage.ResumeLayout(false);
			this.PackageTaskFileToolWizardPage.ResumeLayout(false);
			this.PackageStyleWizardPage.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.PackageInsertSupportWizardPage.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.PackageCreatePackageWizardPage.ResumeLayout(false);
			this.PackagePackageClassWizardPage.ResumeLayout(false);
			this.PackageNewPackageWizardPage.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.PackagingStartWizardPage.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SetupPackagingWizard_Load(object sender, System.EventArgs e)
		{
			// Set the asset name for the MOG argument buttons
			if (assetName != null)
			{
				this.PackageDefaultAddCommandMogArgumentsButton.MOGAssetFilename = assetName;
				this.PackageDefaultRemoveCommandMogArgumentsButton.MOGAssetFilename = assetName;
				
				this.PackageSimpleAddMogArgumentsButton.MOGAssetFilename = assetName;
				this.PackageSimpleAddMogControl_ViewTokenTextBox.MOGAssetFilename = assetName;
				this.PackageSimpleRemoveMogArgumentsButton.MOGAssetFilename = assetName;
				this.PackageSimpleRemoveMogControl_ViewTokenTextBox.MOGAssetFilename = assetName;

				this.PackageDefaultRemoveCommandMogControl_ViewTokenTextBox.MOGAssetFilename = assetName;
				this.PackageDefaultAddCommandMogControl_ViewTokenTextBox.MOGAssetFilename = assetName;
			}
			else if (assetNames.Count > 0)
			{
				this.PackageDefaultAddCommandMogArgumentsButton.MOGAssetFilename = assetNames[0] as MOG_Filename;
				this.PackageDefaultRemoveCommandMogArgumentsButton.MOGAssetFilename = assetNames[0] as MOG_Filename;
				
				this.PackageSimpleAddMogArgumentsButton.MOGAssetFilename = assetNames[0] as MOG_Filename;
				this.PackageSimpleAddMogControl_ViewTokenTextBox.MOGAssetFilename = assetNames[0] as MOG_Filename;
				this.PackageSimpleRemoveMogArgumentsButton.MOGAssetFilename = assetNames[0] as MOG_Filename;
				this.PackageSimpleRemoveMogControl_ViewTokenTextBox.MOGAssetFilename = assetNames[0] as MOG_Filename;

				this.PackageDefaultRemoveCommandMogControl_ViewTokenTextBox.MOGAssetFilename = assetNames[0] as MOG_Filename;
				this.PackageDefaultAddCommandMogControl_ViewTokenTextBox.MOGAssetFilename = assetNames[0] as MOG_Filename;
			}
		}

		#region Navigation events

		private void PackageNewPackageWizardPage_ShowFromNext(object sender, System.EventArgs e)
		{
			// Check to see if any packages exist in our project
			// if(false)
			//{
				// Skip to create package			
			//}
		}

		private void PackageNewPackageWizardPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
		{
			// Do we have a package to be assigned to?
			if (PackageNewPackageYesRadioButton.Checked)
			{
				// Yes, then skip to package assignment
				e.Page = this.PackagingAssignmentsWizardPage;
			}
		}

		private void PackageCustomAddWizardPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
		{
			// If no advanced is checked, jump to end
			if (this.PackageCustomCommandNoRadioButton.Checked)
			{
				// Check to be sure the package has add remove commands assigned?
//				if (false)//No commands in package
//				{
//					e.Page = this.PackageCommandsWizardPage;
//				}
//				else
				{
					e.Page = this.PackageDependanciesWizardPage;
				}
			}
			else
			{
				e.Page = this.PackagingSingleCommandsWizardPage;
			}
		}

		private void PackageDependanciesWizardPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
		{
			// Do we have package-to-package dependancies?
			if (this.PackageDependencyNoRadioButton.Checked)
			{
				// No, then skip to done!
				e.Page = this.PackagingDoneWizardPage;
			}
		}

		private void PackageInsertSupportWizardPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
		{
			// Does this package support inserts?
			if (this.PackageInsertSupportNoRadioButton.Checked)
			{
				// No, then skip to events
				e.Page = this.PackageNeedEventsWizardPage;
			}
		}

		private void PackageStyleWizardPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
		{
			// Are we simple package style?
			if (this.PackageStyleSimpleRadioButton.Checked)
			{
				// Yes, then jump to events
				e.Page = this.PackageNeedEventsWizardPage;
			}
		}

		private void PackageNeedEventsWizardPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
		{
			// Do we need events?
			if (this.PackageNeedEventsNoRadioButton.Checked)
			{
				// No, then skip to package cleaning
				e.Page = this.PackageCleanWizardPage;
			}
		}

		private void PackageClusterWizardPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
		{
			// Do we want package clustering?
			if (this.PackageClusterYesRadioButton.Checked)
			{
				// Yes, then check if we want custom add/removes
				e.Page = this.PackageCustomAddWizardPage;
			}
		}
		
		private void PackageCreatePackageWizardPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
		{
			// Make sure the user has created a package here		
		}

		private void PackageCreatePackageWizardPage_ShowFromNext(object sender, System.EventArgs e)
		{

			// Disable the next button till a package has been created
			//this.Wizard.NextEnabled = false;
		}

		private void PackagePackageClassWizardPage_ShowFromNext(object sender, System.EventArgs e)
		{
			// Intialize and show our package management treeView
			this.PackageCreateNewMogControl_PackageManagementTreeView.TreeView.Initialize();

			// We can skip this page if a the packageManagement Treeview is enabled, meaning that package classes were found
			if (this.PackageCreateNewMogControl_PackageManagementTreeView.TreeView.Enabled)
			{
				// Skip this page
				Wizard.PageIndex++;
			}
			else
			{
				MogControl_AssetContextMenu assetMenu = new MogControl_AssetContextMenu(this.MogRepositoryTreeView);
				this.MogRepositoryTreeView.ContextMenuStrip = assetMenu.InitializeContextMenu(MogControl_AssetContextMenu.ImportMenu_Text);
			
				this.MogRepositoryTreeView.Initialize();
			}
		}
		
		private void PackagePackageClassWizardPage_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e)
		{
			if (MogRepositoryTreeView.SelectedNode != null)
			{
				string classification = MogRepositoryTreeView.SelectedNode.FullPath;
				MOG_Properties props = MOG_Properties.OpenClassificationProperties(classification);
				props.IsPackage = true;
				props.Close();
			}

			// Re-Intialize and show our package management treeView with this newly added class
			this.PackageCreateNewMogControl_PackageManagementTreeView.TreeView.Initialize();
		}

		private void PackagingAssignmentsWizardPage_ShowFromNext(object sender, System.EventArgs e)
		{
			// We can skip this page if 1 or more packages were created within the wizard
			if (this.PackageCreateNewMogControl_PackageManagementTreeView.ProjectPackagesTreeView.CreatedPackages.Count > 0)
			{
				// Skip this page
				Wizard.PageIndex++;
			}
		}

		#endregion

		#region Utility functions

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

		private string LaunchUITypeEditor(string defaultString, string property)
		{
			MOG_Properties properties = new MOG_Properties();
			
			UITypeEditor editor = null;

			// Get our collection of properties
			PropertyDescriptorCollection propCollection = properties.GetProperties(new Attribute[]{new BrowsableAttribute(true)});
			// Foreach PropertyDescriptor, see if we find what we're looking for
			foreach( PropertyDescriptor descriptor in propCollection)
			{
				// If our displayName == the propertyToLookFor, we can get our editor and newValue...
				if( descriptor.DisplayName.ToLower() == property.ToLower())
				{
					editor = descriptor.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
				}
			}

			if(editor != null)
			{
				return GetValueForProperty(editor, defaultString) as String;
			}

			return null;
		}
		#endregion

		private void PackageExplorePackagerButton_Click(object sender, System.EventArgs e)
		{
			this.PackagePackagerToolTextBox.Text = LaunchUITypeEditor(this.PackagePackagerToolTextBox.Text, "PackagePostMergeEvent");
		}

		private void PackagePreEventExploreButton_Click(object sender, System.EventArgs e)
		{
			this.PackagePreEventTextBox.Text = LaunchUITypeEditor(this.PackagePreEventTextBox.Text, "PackagePostMergeEvent");
		}

		private void PackagePostEventExploreButton_Click(object sender, System.EventArgs e)
		{
			this.PackagePostEventTextBox.Text = LaunchUITypeEditor(this.PackagePostEventTextBox.Text, "PackagePostMergeEvent");
		}

		private void PackageManagementButton_Click(object sender, System.EventArgs e)
		{
			// Launch the Form
			PackageManagementForm form = new PackageManagementForm(assetNames, mHasBlessedAsset);
			form.ShowDialog(this);
		}

		private void Wizard_UpdateProgressBar(object sender, System.EventArgs e)
		{
			this.WizardMog_XpProgressBar.Position = this.Wizard.PageIndex;
		}

		private void PackageCreateNewMogControl_PackageManagementTreeView_AfterPackageSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			// Has a package been created?
			if (PackageCreateNewMogControl_PackageManagementTreeView.ProjectPackagesTreeView.CreatedPackages.Count > 0)
			{
				// Now that a package has been created, allow the navagation to move forward.
				this.Wizard.NextEnabled = true;
			}
		}

		#region Property Saving code
		private void SetupPackagingWizard_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Save the properties
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
//			if (this.DialogResult == DialogResult.OK)
//			{
//				foreach (MOG_Properties properties in PropertiesList)
//				{						
//				}
//			}
		}

		private void SavePropertiesFiles()
		{
			try
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
								// Do we need to setup a new package?
								if (this.PackageNewPackageNoRadioButton.Checked)
								{
									// Get an asset handle to the newly created package
									foreach (MOG_Filename packageFile in this.PackageCreateNewMogControl_PackageManagementTreeView.ProjectPackagesTreeView.CreatedPackages)
									{
										ArrayList properties = new ArrayList();

										// Save package assignment
										asset.GetProperties().AddPackage(MOG_PropertyFactory.MOG_Relationships.New_PackageAssignment(packageFile.GetAssetFullName(), "", ""));

										// Set the package style
										if (this.PackageStyleSimpleRadioButton.Checked)
										{
											properties.Add(MOG_PropertyFactory.MOG_Package_OptionsProperties.New_PackageStyle(MOG_PackageStyle.Simple.ToString()));
										
											// Late resolvers?
											if (this.PackageDependencyYesRadioButton.Checked)
											{
												properties.Add(MOG_PropertyFactory.MOG_Package_OptionsProperties.New_PackageCommand_ResolveLateResolvers(this.PackageLateResolverTextBox.Text));
											}
										}
										else
										{
											properties.Add(MOG_PropertyFactory.MOG_Package_OptionsProperties.New_PackageStyle(MOG_PackageStyle.TaskFile.ToString()));
										
											properties.Add(MOG_PropertyFactory.MOG_Package_OptionsProperties.New_TaskFileTool(PackagePackagerToolTextBox.Text));
											properties.Add(MOG_PropertyFactory.MOG_Package_OptionsProperties.New_InputPackageTaskFile(PackageTaskFileInTextBox.Text));
											properties.Add(MOG_PropertyFactory.MOG_Package_OptionsProperties.New_OutputPackageTaskFile(PackageTaskFileOutTextBox.Text));
											properties.Add(MOG_PropertyFactory.MOG_Package_OptionsProperties.New_PackageCommand_DeletePackageFile(PackageDeleteCommandTextBox.Text));
										}

										// Default add/remove commands?
										if (this.PackageCustomCommandYesRadioButton.Checked)
										{
											properties.Add(MOG_PropertyFactory.MOG_Package_CommandsProperties.New_PackageCommand_Add(PackageDefaultAddCommandTextBox.Text));
											properties.Add(MOG_PropertyFactory.MOG_Package_CommandsProperties.New_PackageCommand_Remove(PackageDefaultRemoveCommandTextBox.Text));
										}

										// Do we have pre/post events?
										if (this.PackageNeedEventsYesRadioButton.Checked)
										{
											properties.Add(MOG_PropertyFactory.MOG_Package_OptionsProperties.New_PackagePreMergeEvent(PackagePreEventTextBox.Text));
											properties.Add(MOG_PropertyFactory.MOG_Package_OptionsProperties.New_PackagePostMergeEvent(PackagePostEventTextBox.Text));
										}

										// Do we cleanup directories?
										properties.Add(MOG_PropertyFactory.MOG_Package_OptionsProperties.New_CleanupPackageWorkspaceDirectory(PackageCleanupDirYesRadioButton.Checked));
									
										// Cluster packages?
										properties.Add(MOG_PropertyFactory.MOG_Package_OptionsProperties.New_ClusterPackaging(PackageClusterYesRadioButton.Checked));
									
										// Save these latest changes to the properties file of this new package
										//packageFile = MOG_ControllerProject.GetAssetCurrentBlessedVersionPath(packageFile);
										MOG_ControllerProject.GetProject().ModifyBlessedAssetProperties(packageFile, null, properties);
									}
								}

								// Set add/remove commands
								asset.GetProperties().PackageCommand_Add = this.PackageSimpleAddTextBox.Text;
								asset.GetProperties().PackageCommand_Remove = this.PackageSimpleRemoveTextBox.Text;
							
								// Save these properties
								asset.Close();
							}
						}					
					}
				}
			}
			catch(Exception ex)
			{
				ex.ToString();
			}
		}
		#endregion				
	}
}
