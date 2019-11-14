using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Client.Forms.AssetProperties
{
	/// <summary>
	/// Summary description for SetupPackageForm.
	/// </summary>
    public class SetupPackageForm : System.Windows.Forms.Form
    {
        private Gui.Wizard.Wizard Wizard;
        private Gui.Wizard.WizardPage wizardPage1;
        private Gui.Wizard.InfoPage infoPage1;
        private Gui.Wizard.WizardPage wizardPage2;
        private Gui.Wizard.Header header1;
        private System.Windows.Forms.RadioButton PackageSimpleRadioButton;
        private System.Windows.Forms.RadioButton PackageMultiRadioButton;
        private Gui.Wizard.WizardPage wizardPage3;
        private Gui.Wizard.Header header2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Label label1;
        private Gui.Wizard.WizardPage wizardPage4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton4;
        private Gui.Wizard.Header header3;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public SetupPackageForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupPackageForm));
            this.Wizard = new Gui.Wizard.Wizard();
            this.wizardPage1 = new Gui.Wizard.WizardPage();
            this.infoPage1 = new Gui.Wizard.InfoPage();
            this.wizardPage4 = new Gui.Wizard.WizardPage();
            this.header3 = new Gui.Wizard.Header();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.wizardPage3 = new Gui.Wizard.WizardPage();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.header2 = new Gui.Wizard.Header();
            this.wizardPage2 = new Gui.Wizard.WizardPage();
            this.PackageMultiRadioButton = new System.Windows.Forms.RadioButton();
            this.PackageSimpleRadioButton = new System.Windows.Forms.RadioButton();
            this.header1 = new Gui.Wizard.Header();
            this.Wizard.SuspendLayout();
            this.wizardPage1.SuspendLayout();
            this.wizardPage4.SuspendLayout();
            this.wizardPage3.SuspendLayout();
            this.wizardPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Wizard
            // 
            this.Wizard.Controls.Add(this.wizardPage4);
            this.Wizard.Controls.Add(this.wizardPage3);
            this.Wizard.Controls.Add(this.wizardPage2);
            this.Wizard.Controls.Add(this.wizardPage1);
            this.Wizard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Wizard.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Wizard.Location = new System.Drawing.Point(0, 0);
            this.Wizard.Name = "Wizard";
            this.Wizard.Pages.AddRange(new Gui.Wizard.WizardPage[] {
            this.wizardPage1,
            this.wizardPage2,
            this.wizardPage3,
            this.wizardPage4});
            this.Wizard.PushPop = false;
            this.Wizard.Size = new System.Drawing.Size(400, 351);
            this.Wizard.TabIndex = 0;
//            this.Wizard.Load += new System.EventHandler(this.Wizard_Load);
            // 
            // wizardPage1
            // 
            this.wizardPage1.Controls.Add(this.infoPage1);
            this.wizardPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage1.IsFinishPage = false;
            this.wizardPage1.Location = new System.Drawing.Point(0, 0);
            this.wizardPage1.Name = "wizardPage1";
            this.wizardPage1.Size = new System.Drawing.Size(400, 303);
            this.wizardPage1.TabIndex = 1;
            // 
            // infoPage1
            // 
            this.infoPage1.BackColor = System.Drawing.Color.White;
            this.infoPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoPage1.Image = ((System.Drawing.Image)(resources.GetObject("infoPage1.Image")));
            this.infoPage1.Location = new System.Drawing.Point(0, 0);
            this.infoPage1.Name = "infoPage1";
            this.infoPage1.PageText = "This wizard enables setup all the package properties for this package to be able " +
                "to add and remove assets";
            this.infoPage1.PageTitle = "Welcome to the Package Setup Wizard";
            this.infoPage1.Size = new System.Drawing.Size(400, 303);
            this.infoPage1.TabIndex = 0;
            // 
            // wizardPage4
            // 
            this.wizardPage4.Controls.Add(this.header3);
            this.wizardPage4.Controls.Add(this.radioButton3);
            this.wizardPage4.Controls.Add(this.radioButton4);
            this.wizardPage4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage4.IsFinishPage = false;
            this.wizardPage4.Location = new System.Drawing.Point(0, 0);
            this.wizardPage4.Name = "wizardPage4";
            this.wizardPage4.Size = new System.Drawing.Size(400, 303);
            this.wizardPage4.TabIndex = 4;
            // 
            // header3
            // 
            this.header3.BackColor = System.Drawing.SystemColors.Control;
            this.header3.CausesValidation = false;
            this.header3.Description = "Do you want MOG to remove the files in the temp directory after the merge has com" +
                "pleted?";
            this.header3.Dock = System.Windows.Forms.DockStyle.Top;
            this.header3.Image = ((System.Drawing.Image)(resources.GetObject("header3.Image")));
            this.header3.Location = new System.Drawing.Point(0, 0);
            this.header3.Name = "header3";
            this.header3.Size = new System.Drawing.Size(400, 64);
            this.header3.TabIndex = 9;
            this.header3.Title = "Cleanup Temporary Directory";
            // 
            // radioButton3
            // 
            this.radioButton3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioButton3.Location = new System.Drawing.Point(16, 104);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(104, 24);
            this.radioButton3.TabIndex = 7;
            this.radioButton3.Text = "No";
            // 
            // radioButton4
            // 
            this.radioButton4.Checked = true;
            this.radioButton4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioButton4.Location = new System.Drawing.Point(16, 80);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(104, 24);
            this.radioButton4.TabIndex = 6;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "Yes";
            // 
            // wizardPage3
            // 
            this.wizardPage3.Controls.Add(this.label1);
            this.wizardPage3.Controls.Add(this.radioButton1);
            this.wizardPage3.Controls.Add(this.radioButton2);
            this.wizardPage3.Controls.Add(this.header2);
            this.wizardPage3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage3.IsFinishPage = false;
            this.wizardPage3.Location = new System.Drawing.Point(0, 0);
            this.wizardPage3.Name = "wizardPage3";
            this.wizardPage3.Size = new System.Drawing.Size(400, 303);
            this.wizardPage3.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(360, 32);
            this.label1.TabIndex = 5;
            this.label1.Text = "This is helpfull if you need working files required by the merge to stick around " +
                "till subsequent merges.";
            // 
            // radioButton1
            // 
            this.radioButton1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioButton1.Location = new System.Drawing.Point(16, 136);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(104, 24);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.Text = "No";
            // 
            // radioButton2
            // 
            this.radioButton2.Checked = true;
            this.radioButton2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioButton2.Location = new System.Drawing.Point(16, 112);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(104, 24);
            this.radioButton2.TabIndex = 3;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Yes";
            // 
            // header2
            // 
            this.header2.BackColor = System.Drawing.SystemColors.Control;
            this.header2.CausesValidation = false;
            this.header2.Description = "Would you like MOG to create a Temporary directory for package merging?";
            this.header2.Dock = System.Windows.Forms.DockStyle.Top;
            this.header2.Image = ((System.Drawing.Image)(resources.GetObject("header2.Image")));
            this.header2.Location = new System.Drawing.Point(0, 0);
            this.header2.Name = "header2";
            this.header2.Size = new System.Drawing.Size(400, 64);
            this.header2.TabIndex = 0;
            this.header2.Title = "Temporary Packaging Folder";
            // 
            // wizardPage2
            // 
            this.wizardPage2.Controls.Add(this.PackageMultiRadioButton);
            this.wizardPage2.Controls.Add(this.PackageSimpleRadioButton);
            this.wizardPage2.Controls.Add(this.header1);
            this.wizardPage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage2.IsFinishPage = false;
            this.wizardPage2.Location = new System.Drawing.Point(0, 0);
            this.wizardPage2.Name = "wizardPage2";
            this.wizardPage2.Size = new System.Drawing.Size(400, 303);
            this.wizardPage2.TabIndex = 2;
            // 
            // PackageMultiRadioButton
            // 
            this.PackageMultiRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.PackageMultiRadioButton.Location = new System.Drawing.Point(16, 104);
            this.PackageMultiRadioButton.Name = "PackageMultiRadioButton";
            this.PackageMultiRadioButton.Size = new System.Drawing.Size(104, 24);
            this.PackageMultiRadioButton.TabIndex = 2;
            this.PackageMultiRadioButton.Text = "No";
            // 
            // PackageSimpleRadioButton
            // 
            this.PackageSimpleRadioButton.Checked = true;
            this.PackageSimpleRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.PackageSimpleRadioButton.Location = new System.Drawing.Point(16, 80);
            this.PackageSimpleRadioButton.Name = "PackageSimpleRadioButton";
            this.PackageSimpleRadioButton.Size = new System.Drawing.Size(104, 24);
            this.PackageSimpleRadioButton.TabIndex = 1;
            this.PackageSimpleRadioButton.TabStop = true;
            this.PackageSimpleRadioButton.Text = "Yes";
            // 
            // header1
            // 
            this.header1.BackColor = System.Drawing.SystemColors.Control;
            this.header1.CausesValidation = false;
            this.header1.Description = "Does this asset add files to it one at a time via commandline arguments?";
            this.header1.Dock = System.Windows.Forms.DockStyle.Top;
            this.header1.Image = ((System.Drawing.Image)(resources.GetObject("header1.Image")));
            this.header1.Location = new System.Drawing.Point(0, 0);
            this.header1.Name = "header1";
            this.header1.Size = new System.Drawing.Size(400, 64);
            this.header1.TabIndex = 0;
            this.header1.Title = "Packaging type";
            // 
            // SetupPackageForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(400, 351);
            this.Controls.Add(this.Wizard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SetupPackageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Setup package properties wizard";
            this.Wizard.ResumeLayout(false);
            this.wizardPage1.ResumeLayout(false);
            this.wizardPage4.ResumeLayout(false);
            this.wizardPage3.ResumeLayout(false);
            this.wizardPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
    }
}
