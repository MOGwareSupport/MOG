using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using MOG.INI;
using MOG;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for ToolBoxIniComboBoxForm.
	/// </summary>
	public class ToolBoxIniComboBoxForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ToolTip ToolBoxIniToolTip;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button PathBrowseButton;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button ToolBoxIniComboOKButton;
		private System.Windows.Forms.Button ToolBoxIniComboCancelButton;
		public System.Windows.Forms.ComboBox SectionComboBox;
		private System.Windows.Forms.Label SectionLabel;

		private bool mUpdateTagTextBox = true;
		private string m_StartPath;
		/// <summary>
		/// Used by ToolBox (but not by this form)
		/// </summary>
		public string mStartPath
		{
			get
			{
				if(this.m_StartPath != null)
					return this.m_StartPath;
				return "";
			}
			set
			{
				this.m_StartPath = value;
			}
		}
		private string mIniFilename;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox NameTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox TagCheckBox;
		public System.Windows.Forms.TextBox TagTextBox;
		private System.Windows.Forms.Label label4;
		public System.Windows.Forms.TextBox ToolTipTextBox;
		public System.Windows.Forms.TextBox PathTextBox;

		private ToolBox mToolBox;
		private GroupBox groupBox1;
		private GroupBox groupBox2;
	
		public string IniFilename
		{
			get
			{
				return mIniFilename;
			}
		}
		private string m_SelectedSection;
		public string mSelectedSection
		{
			get
			{
				return m_SelectedSection;
			}
			set
			{
				this.m_SelectedSection = value;
			}
		}

		public ToolBoxIniComboBoxForm( ToolBox toolBox )
		{
			this.mToolBox = toolBox;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

		}

		public void SetControlEnableBasedOnPrivilege(ToolBoxControlLocation location)
		{
			bool enable = true;

			switch (location)
			{
				case ToolBoxControlLocation.Project:
					enable = mToolBox.mPrivileges.GetUserPrivilege(mToolBox.mCurrentUserName, MOG_PRIVILEGE.ConfigureProjectCustomTools);
					break;
				case ToolBoxControlLocation.Department:
					enable = mToolBox.mPrivileges.GetUserPrivilege(mToolBox.mCurrentUserName, MOG_PRIVILEGE.ConfigureDepartmentCustomTools);
					break;
				case ToolBoxControlLocation.User:
					enable = mToolBox.mPrivileges.GetUserPrivilege(mToolBox.mCurrentUserName, MOG_PRIVILEGE.ConfigureUserCustomTools);
					break;
			}

			foreach (Control control in Controls)
			{
				if (control.Name != "ToolBoxIniComboOKButton" && control.Name != "ToolBoxIniComboCancelButton")
				{
					control.Enabled = enable;
				}
			}
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

		public void InitializeComboBox( string iniFilename )
		{
			PopulateSectionComboBox( iniFilename );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolBoxIniComboBoxForm));
			this.ToolBoxIniToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.PathTextBox = new System.Windows.Forms.TextBox();
			this.SectionComboBox = new System.Windows.Forms.ComboBox();
			this.NameTextBox = new System.Windows.Forms.TextBox();
			this.TagTextBox = new System.Windows.Forms.TextBox();
			this.ToolTipTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.PathBrowseButton = new System.Windows.Forms.Button();
			this.ToolBoxIniComboOKButton = new System.Windows.Forms.Button();
			this.ToolBoxIniComboCancelButton = new System.Windows.Forms.Button();
			this.SectionLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.TagCheckBox = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// PathTextBox
			// 
			this.PathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.PathTextBox.Location = new System.Drawing.Point(89, 18);
			this.PathTextBox.Name = "PathTextBox";
			this.PathTextBox.Size = new System.Drawing.Size(174, 20);
			this.PathTextBox.TabIndex = 0;
			this.ToolBoxIniToolTip.SetToolTip(this.PathTextBox, "Path to INI file");
			// 
			// SectionComboBox
			// 
			this.SectionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.SectionComboBox.Location = new System.Drawing.Point(89, 44);
			this.SectionComboBox.Name = "SectionComboBox";
			this.SectionComboBox.Size = new System.Drawing.Size(260, 21);
			this.SectionComboBox.TabIndex = 5;
			this.ToolBoxIniToolTip.SetToolTip(this.SectionComboBox, "Section of values to display in the Combo Box");
			this.SectionComboBox.SelectionChangeCommitted += new System.EventHandler(this.SectionComboBox_SelectionChangeCommitted);
			// 
			// NameTextBox
			// 
			this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.NameTextBox.Location = new System.Drawing.Point(66, 8);
			this.NameTextBox.Name = "NameTextBox";
			this.NameTextBox.Size = new System.Drawing.Size(502, 20);
			this.NameTextBox.TabIndex = 2;
			this.ToolBoxIniToolTip.SetToolTip(this.NameTextBox, "Name to be displayed in MOG Toolbox");
			this.NameTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
			// 
			// TagTextBox
			// 
			this.TagTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.TagTextBox.Location = new System.Drawing.Point(46, 35);
			this.TagTextBox.Name = "TagTextBox";
			this.TagTextBox.Size = new System.Drawing.Size(148, 20);
			this.TagTextBox.TabIndex = 4;
			this.ToolBoxIniToolTip.SetToolTip(this.TagTextBox, "Name that other ToolBox Controls will see");
			this.TagTextBox.TextChanged += new System.EventHandler(this.TagTextBox_TextChanged);
			// 
			// ToolTipTextBox
			// 
			this.ToolTipTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ToolTipTextBox.Location = new System.Drawing.Point(66, 34);
			this.ToolTipTextBox.Name = "ToolTipTextBox";
			this.ToolTipTextBox.Size = new System.Drawing.Size(502, 20);
			this.ToolTipTextBox.TabIndex = 6;
			this.ToolBoxIniToolTip.SetToolTip(this.ToolTipTextBox, "ToolTip you would like other users to see when they hover their mouse over this I" +
					"NI Combo Box");
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(11, 19);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 16);
			this.label3.TabIndex = 11;
			this.label3.Text = "Ini File Path:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// PathBrowseButton
			// 
			this.PathBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PathBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.PathBrowseButton.Location = new System.Drawing.Point(269, 16);
			this.PathBrowseButton.Name = "PathBrowseButton";
			this.PathBrowseButton.Size = new System.Drawing.Size(80, 23);
			this.PathBrowseButton.TabIndex = 1;
			this.PathBrowseButton.Text = "Browse Path";
			this.PathBrowseButton.Click += new System.EventHandler(this.PathBrowseButton_Click);
			// 
			// ToolBoxIniComboOKButton
			// 
			this.ToolBoxIniComboOKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.ToolBoxIniComboOKButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ToolBoxIniComboOKButton.Location = new System.Drawing.Point(195, 148);
			this.ToolBoxIniComboOKButton.Name = "ToolBoxIniComboOKButton";
			this.ToolBoxIniComboOKButton.Size = new System.Drawing.Size(83, 23);
			this.ToolBoxIniComboOKButton.TabIndex = 7;
			this.ToolBoxIniComboOKButton.Text = "OK";
			this.ToolBoxIniComboOKButton.Click += new System.EventHandler(this.ToolBoxIniComboOKButton_Click);
			// 
			// ToolBoxIniComboCancelButton
			// 
			this.ToolBoxIniComboCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.ToolBoxIniComboCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.ToolBoxIniComboCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ToolBoxIniComboCancelButton.Location = new System.Drawing.Point(291, 148);
			this.ToolBoxIniComboCancelButton.Name = "ToolBoxIniComboCancelButton";
			this.ToolBoxIniComboCancelButton.Size = new System.Drawing.Size(83, 23);
			this.ToolBoxIniComboCancelButton.TabIndex = 8;
			this.ToolBoxIniComboCancelButton.Text = "Cancel";
			// 
			// SectionLabel
			// 
			this.SectionLabel.Location = new System.Drawing.Point(33, 44);
			this.SectionLabel.Name = "SectionLabel";
			this.SectionLabel.Size = new System.Drawing.Size(48, 16);
			this.SectionLabel.TabIndex = 15;
			this.SectionLabel.Text = "Section:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 17);
			this.label1.TabIndex = 16;
			this.label1.Text = "Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 38);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 16);
			this.label2.TabIndex = 16;
			this.label2.Text = "Tag:";
			// 
			// TagCheckBox
			// 
			this.TagCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.TagCheckBox.Checked = true;
			this.TagCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.TagCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TagCheckBox.Location = new System.Drawing.Point(11, 19);
			this.TagCheckBox.Name = "TagCheckBox";
			this.TagCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.TagCheckBox.Size = new System.Drawing.Size(183, 16);
			this.TagCheckBox.TabIndex = 3;
			this.TagCheckBox.Text = "Auto Create Tag from Name:";
			this.TagCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.TagCheckBox.CheckedChanged += new System.EventHandler(this.TagCheckBox_Changed);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(4, 37);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 16);
			this.label4.TabIndex = 19;
			this.label4.Text = "ToolTip:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.TagCheckBox);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.TagTextBox);
			this.groupBox1.Location = new System.Drawing.Point(7, 60);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 79);
			this.groupBox1.TabIndex = 20;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Tag Options";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.PathTextBox);
			this.groupBox2.Controls.Add(this.PathBrowseButton);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.SectionComboBox);
			this.groupBox2.Controls.Add(this.SectionLabel);
			this.groupBox2.Location = new System.Drawing.Point(213, 60);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(355, 79);
			this.groupBox2.TabIndex = 21;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Ini options";
			// 
			// ToolBoxIniComboBoxForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(575, 178);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.ToolTipTextBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.NameTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.ToolBoxIniComboCancelButton);
			this.Controls.Add(this.ToolBoxIniComboOKButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(3000, 280);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(583, 212);
			this.Name = "ToolBoxIniComboBoxForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create/Edit Ini Combo Box";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void PathBrowseButton_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = this.mStartPath;
			ofd.Title = "Please select an Ini file...";
			ofd.CheckFileExists = true;
			ofd.Filter = "Ini Files(*.ini;*.info)|*.ini;*.info|All files (*.*)|*.*";

			if (ofd.ShowDialog( this ) == DialogResult.OK) 
			{
				PathTextBox.Text = ofd.FileName;
				this.mStartPath = ofd.FileName.Substring( 0, ofd.FileName.LastIndexOf("\\") );
				PopulateSectionComboBox( PathTextBox.Text );
				this.mIniFilename = PathTextBox.Text;

				// If we have nothing in the NameTextBox, we will fill it with the filename
				if( this.NameTextBox.Text == null || this.NameTextBox.Text.Length < 1 )
				{
					this.NameTextBox.Text = this.mIniFilename.Substring( this.mIniFilename.LastIndexOf("\\") + 1 );
					NameTextBox_TextChanged( this.NameTextBox, null );
				}
			}
		} // end ()

		private void PopulateSectionComboBox( string iniPath )
		{
			MOG_Ini ini = new MOG_Ini( iniPath );
			int sectionCount = ini.CountSections();

			// If we are repopulating, clear our old items
			if( this.SectionComboBox.Items.Count > 0 )
				this.SectionComboBox.Items.Clear();

			for( int i = 0; i < sectionCount; ++i )
			{
				this.SectionComboBox.Items.Add( ini.GetSectionByIndexSLOW( i ) );
			}

			SectionComboBox.Text = mSelectedSection;
		}

		private void SectionComboBox_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			this.mSelectedSection = (string)comboBox.Items[ comboBox.SelectedIndex ];
		}

		private void TagCheckBox_Changed(object sender, System.EventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			this.mUpdateTagTextBox = ( checkBox.CheckState == CheckState.Checked );
		}

		private void NameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			if( this.mUpdateTagTextBox )
			{
				this.TagTextBox.Text = ((TextBox)sender).Text.Replace( " ", "" );
			}
		}

		private void TagTextBox_TextChanged(object sender, System.EventArgs e)
		{
			//glk: TODO: Find out if there are invalid characters and remove them.
			//TextBox textBox = (TextBox)sender;
			//Match match = Regex.Match( textBox.Text, @"" );
		}

		private void ToolBoxIniComboOKButton_Click(object sender, System.EventArgs e)
		{
			// Verify we have all we need
			string errorMessage = "Please complete the following items: \r\n\r\n";
			bool error = false;
			if( this.PathTextBox.Text == null || this.PathTextBox.Text.Length < 1 )
			{
				error |= true;
				errorMessage += "Select an Ini file to use\r\n";
			}
			if( this.NameTextBox.Text == null || this.NameTextBox.Text.Length < 1 )
			{
				error |= true;
				errorMessage += "Enter a name.\r\n";
			}
			if( this.TagTextBox.Text == null || this.TagTextBox.Text.Length < 1 )
			{
				error |= true;
				errorMessage += "Enter a tag name.\r\n";
			}
			if( this.SectionComboBox.Text != null && this.SectionComboBox.Text.Length > 0 )
			{
				this.mSelectedSection = this.SectionComboBox.Text;
			}
			else if (this.SectionComboBox.SelectedItem == null || this.SectionComboBox.SelectedItem.ToString().Length < 1) 
			{
				error |= true;
				errorMessage += "Select or type in a section's name.\r\n";
			}

			if( error )
				MessageBox.Show( this, errorMessage, "Missing Items", MessageBoxButtons.OK, 
					MessageBoxIcon.Exclamation );
			else
			{
				Button b = (Button)sender;
				b.DialogResult = DialogResult.OK;
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}
	} // end class
} // end ns
