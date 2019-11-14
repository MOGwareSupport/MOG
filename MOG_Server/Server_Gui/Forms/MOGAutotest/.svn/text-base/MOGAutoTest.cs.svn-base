using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG_Server.Server_Mog_Utilities;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.SYSTEMUTILITIES;

namespace MOG_Server.Server_Gui.Forms.MOGAutotest
{
	/// <summary>
	/// Summary description for MOGAutoTest.
	/// </summary>
	public class MOGAutoTest : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button AutoTestCancelButton;
		private System.Windows.Forms.Button AutoTestGoButton;
		private System.Windows.Forms.TextBox AutoTestFileTextBox;
		private System.Windows.Forms.TrackBar AutoTestTimeTrackBar;
		private System.Windows.Forms.TextBox AutoTestTimeTextBox;
		private System.Windows.Forms.Button AutoTestBrowseButton;
		private System.Windows.Forms.OpenFileDialog OpenFileDialog;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox AutoTestDelayTextBox;
		private System.Windows.Forms.TrackBar AutoTestDelayTrackBar;
		private System.Windows.Forms.TextBox AutoTestTimeMinutesTextBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox AutoTestProjectComboBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox AutoTestCopyFileLocalCheckBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox AutoTestTestNameComboBox;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox AutoTestStartIndexTextBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public string AutoTestFile
		{
			get { return this.AutoTestFileTextBox.Text; }
		}

		public bool AutoTestCopyFileLocal
		{
			get { return this.AutoTestCopyFileLocalCheckBox.Checked; }
		}

		public string AutoTestName
		{
			get { return this.AutoTestTestNameComboBox.Text; }
		}

		public string AutoTestProject
		{
			get { return this.AutoTestProjectComboBox.Text; }
		}

		public int AutoTestDuration
		{
			get { return Convert.ToInt32(this.AutoTestTimeTextBox.Text); }
		}

		public int AutoTestDelay
		{
			get { return Convert.ToInt32(this.AutoTestDelayTextBox.Text); }
		}
		public int AutoTestImportStartIndex
		{
			get { return Convert.ToInt32(this.AutoTestStartIndexTextBox.Text); }
		}		

		public MOGAutoTest()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			foreach (string projectName in MOG_ControllerSystem.GetSystem().GetProjectNames())
			{
				this.AutoTestProjectComboBox.Items.Add(projectName);
			}

			if (UserPrefs.LoadPref("AutoTest", "Project").Length != 0)
			{
				this.AutoTestProjectComboBox.Text = UserPrefs.LoadPref("AutoTest", "Project");
			}
			
			this.AutoTestFileTextBox.Text = UserPrefs.LoadPref("AutoTest", "File");
			if (UserPrefs.LoadPref("AutoTest", "Time").Length != 0)
			{
				this.AutoTestTimeTrackBar.Value = Convert.ToInt32(UserPrefs.LoadPref("AutoTest", "Time"));
				this.AutoTestTimeTextBox.Text = UserPrefs.LoadPref("AutoTest", "Time");
			}
			
			if (UserPrefs.LoadPref("AutoTest", "Delay").Length != 0)
			{
				this.AutoTestDelayTrackBar.Value = Convert.ToInt32(UserPrefs.LoadPref("AutoTest", "Delay"));
				this.AutoTestDelayTextBox.Text = UserPrefs.LoadPref("AutoTest", "Delay");
			}

			foreach (string testName in MOG_SystemUtilities.GetAutomatedTestNames())
			{
				this.AutoTestTestNameComboBox.Items.Add(testName);
			}

			if (UserPrefs.LoadPref("AutoTest", "Name").Length != 0)
			{
				this.AutoTestTestNameComboBox.Text = UserPrefs.LoadPref("AutoTest", "Name");
			}

			if (UserPrefs.LoadPref("AutoTest", "CopyFileLocal").Length != 0)
			{
				this.AutoTestCopyFileLocalCheckBox.Checked = Convert.ToBoolean(UserPrefs.LoadPref("AutoTest", "CopyFileLocal"));
			}

			if (UserPrefs.LoadPref("AutoTest", "StartIndex").Length != 0)
			{
				this.AutoTestStartIndexTextBox.Text = UserPrefs.LoadPref("AutoTest", "StartIndex");
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MOGAutoTest));
			this.AutoTestCancelButton = new System.Windows.Forms.Button();
			this.AutoTestGoButton = new System.Windows.Forms.Button();
			this.AutoTestFileTextBox = new System.Windows.Forms.TextBox();
			this.AutoTestTimeTrackBar = new System.Windows.Forms.TrackBar();
			this.AutoTestTimeTextBox = new System.Windows.Forms.TextBox();
			this.AutoTestBrowseButton = new System.Windows.Forms.Button();
			this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.AutoTestDelayTextBox = new System.Windows.Forms.TextBox();
			this.AutoTestDelayTrackBar = new System.Windows.Forms.TrackBar();
			this.AutoTestTimeMinutesTextBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.AutoTestProjectComboBox = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.AutoTestCopyFileLocalCheckBox = new System.Windows.Forms.CheckBox();
			this.AutoTestTestNameComboBox = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.AutoTestStartIndexTextBox = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.AutoTestTimeTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.AutoTestDelayTrackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// AutoTestCancelButton
			// 
			this.AutoTestCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.AutoTestCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.AutoTestCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AutoTestCancelButton.Location = new System.Drawing.Point(414, 240);
			this.AutoTestCancelButton.Name = "AutoTestCancelButton";
			this.AutoTestCancelButton.TabIndex = 0;
			this.AutoTestCancelButton.Text = "Cancel";
			// 
			// AutoTestGoButton
			// 
			this.AutoTestGoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.AutoTestGoButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.AutoTestGoButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AutoTestGoButton.Location = new System.Drawing.Point(334, 240);
			this.AutoTestGoButton.Name = "AutoTestGoButton";
			this.AutoTestGoButton.TabIndex = 1;
			this.AutoTestGoButton.Text = "Go";
			// 
			// AutoTestFileTextBox
			// 
			this.AutoTestFileTextBox.Location = new System.Drawing.Point(24, 120);
			this.AutoTestFileTextBox.Name = "AutoTestFileTextBox";
			this.AutoTestFileTextBox.Size = new System.Drawing.Size(432, 20);
			this.AutoTestFileTextBox.TabIndex = 2;
			this.AutoTestFileTextBox.Text = "";
			this.AutoTestFileTextBox.TextChanged += new System.EventHandler(this.AutoTestFileTextBox_TextChanged);
			// 
			// AutoTestTimeTrackBar
			// 
			this.AutoTestTimeTrackBar.AutoSize = false;
			this.AutoTestTimeTrackBar.LargeChange = 300;
			this.AutoTestTimeTrackBar.Location = new System.Drawing.Point(16, 160);
			this.AutoTestTimeTrackBar.Maximum = 604800;
			this.AutoTestTimeTrackBar.Name = "AutoTestTimeTrackBar";
			this.AutoTestTimeTrackBar.Size = new System.Drawing.Size(168, 24);
			this.AutoTestTimeTrackBar.TabIndex = 3;
			this.AutoTestTimeTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.AutoTestTimeTrackBar.Value = 60;
			this.AutoTestTimeTrackBar.Scroll += new System.EventHandler(this.AutoTestTimeTrackBar_Scroll);
			// 
			// AutoTestTimeTextBox
			// 
			this.AutoTestTimeTextBox.Location = new System.Drawing.Point(184, 168);
			this.AutoTestTimeTextBox.Name = "AutoTestTimeTextBox";
			this.AutoTestTimeTextBox.Size = new System.Drawing.Size(48, 20);
			this.AutoTestTimeTextBox.TabIndex = 4;
			this.AutoTestTimeTextBox.Text = "60";
			this.AutoTestTimeTextBox.TextChanged += new System.EventHandler(this.AutoTestTimeTextBox_TextChanged);
			// 
			// AutoTestBrowseButton
			// 
			this.AutoTestBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AutoTestBrowseButton.Location = new System.Drawing.Point(465, 120);
			this.AutoTestBrowseButton.Name = "AutoTestBrowseButton";
			this.AutoTestBrowseButton.Size = new System.Drawing.Size(24, 23);
			this.AutoTestBrowseButton.TabIndex = 5;
			this.AutoTestBrowseButton.Text = "...";
			this.AutoTestBrowseButton.Click += new System.EventHandler(this.AutoTestBrowseButton_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 104);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(264, 16);
			this.label1.TabIndex = 6;
			this.label1.Text = "Import File:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 152);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(152, 16);
			this.label2.TabIndex = 7;
			this.label2.Text = "Time Duration: (5 min/7 day)";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(264, 152);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(152, 16);
			this.label3.TabIndex = 10;
			this.label3.Text = "Time Delay: (0/60 min)";
			// 
			// AutoTestDelayTextBox
			// 
			this.AutoTestDelayTextBox.Location = new System.Drawing.Point(424, 168);
			this.AutoTestDelayTextBox.Name = "AutoTestDelayTextBox";
			this.AutoTestDelayTextBox.Size = new System.Drawing.Size(40, 20);
			this.AutoTestDelayTextBox.TabIndex = 9;
			this.AutoTestDelayTextBox.Text = "0";
			// 
			// AutoTestDelayTrackBar
			// 
			this.AutoTestDelayTrackBar.AutoSize = false;
			this.AutoTestDelayTrackBar.LargeChange = 60;
			this.AutoTestDelayTrackBar.Location = new System.Drawing.Point(256, 160);
			this.AutoTestDelayTrackBar.Maximum = 3600;
			this.AutoTestDelayTrackBar.Name = "AutoTestDelayTrackBar";
			this.AutoTestDelayTrackBar.Size = new System.Drawing.Size(168, 24);
			this.AutoTestDelayTrackBar.TabIndex = 8;
			this.AutoTestDelayTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.AutoTestDelayTrackBar.Scroll += new System.EventHandler(this.AutoTestDelayTrackBar_Scroll);
			// 
			// AutoTestTimeMinutesTextBox
			// 
			this.AutoTestTimeMinutesTextBox.Location = new System.Drawing.Point(184, 144);
			this.AutoTestTimeMinutesTextBox.Name = "AutoTestTimeMinutesTextBox";
			this.AutoTestTimeMinutesTextBox.ReadOnly = true;
			this.AutoTestTimeMinutesTextBox.Size = new System.Drawing.Size(48, 20);
			this.AutoTestTimeMinutesTextBox.TabIndex = 11;
			this.AutoTestTimeMinutesTextBox.Text = "1";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(232, 176);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(24, 16);
			this.label4.TabIndex = 12;
			this.label4.Text = "Sec";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(232, 152);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(24, 16);
			this.label5.TabIndex = 13;
			this.label5.Text = "Min";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(466, 168);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(24, 16);
			this.label6.TabIndex = 14;
			this.label6.Text = "Sec";
			// 
			// AutoTestProjectComboBox
			// 
			this.AutoTestProjectComboBox.Location = new System.Drawing.Point(24, 72);
			this.AutoTestProjectComboBox.Name = "AutoTestProjectComboBox";
			this.AutoTestProjectComboBox.Size = new System.Drawing.Size(168, 21);
			this.AutoTestProjectComboBox.TabIndex = 15;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(24, 56);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(152, 16);
			this.label7.TabIndex = 16;
			this.label7.Text = "Project:";
			// 
			// AutoTestCopyFileLocalCheckBox
			// 
			this.AutoTestCopyFileLocalCheckBox.Location = new System.Drawing.Point(24, 240);
			this.AutoTestCopyFileLocalCheckBox.Name = "AutoTestCopyFileLocalCheckBox";
			this.AutoTestCopyFileLocalCheckBox.Size = new System.Drawing.Size(164, 24);
			this.AutoTestCopyFileLocalCheckBox.TabIndex = 17;
			this.AutoTestCopyFileLocalCheckBox.Text = "Copy Import File Local";
			// 
			// AutoTestTestNameComboBox
			// 
			this.AutoTestTestNameComboBox.Location = new System.Drawing.Point(24, 28);
			this.AutoTestTestNameComboBox.Name = "AutoTestTestNameComboBox";
			this.AutoTestTestNameComboBox.Size = new System.Drawing.Size(168, 21);
			this.AutoTestTestNameComboBox.TabIndex = 18;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(24, 8);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(100, 16);
			this.label8.TabIndex = 19;
			this.label8.Text = "Test Name:";
			// 
			// AutoTestStartIndexTextBox
			// 
			this.AutoTestStartIndexTextBox.Location = new System.Drawing.Point(24, 208);
			this.AutoTestStartIndexTextBox.Name = "AutoTestStartIndexTextBox";
			this.AutoTestStartIndexTextBox.Size = new System.Drawing.Size(160, 20);
			this.AutoTestStartIndexTextBox.TabIndex = 20;
			this.AutoTestStartIndexTextBox.Text = "0";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(24, 192);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(264, 16);
			this.label9.TabIndex = 21;
			this.label9.Text = "Index we should start importing new files at:";
			// 
			// MOGAutoTest
			// 
			this.AcceptButton = this.AutoTestGoButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.AutoTestCancelButton;
			this.ClientSize = new System.Drawing.Size(498, 275);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.AutoTestStartIndexTextBox);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.AutoTestTestNameComboBox);
			this.Controls.Add(this.AutoTestCopyFileLocalCheckBox);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.AutoTestProjectComboBox);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.AutoTestTimeMinutesTextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.AutoTestDelayTextBox);
			this.Controls.Add(this.AutoTestDelayTrackBar);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.AutoTestBrowseButton);
			this.Controls.Add(this.AutoTestTimeTextBox);
			this.Controls.Add(this.AutoTestTimeTrackBar);
			this.Controls.Add(this.AutoTestFileTextBox);
			this.Controls.Add(this.AutoTestGoButton);
			this.Controls.Add(this.AutoTestCancelButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MOGAutoTest";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MOGAutoTest";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MOGAutoTest_Closing);
			((System.ComponentModel.ISupportInitialize)(this.AutoTestTimeTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.AutoTestDelayTrackBar)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void AutoTestBrowseButton_Click(object sender, System.EventArgs e)
		{
			if (OpenFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.AutoTestFileTextBox.Text = OpenFileDialog.FileName;
			}
		}

		private void AutoTestFileTextBox_TextChanged(object sender, System.EventArgs e)
		{
			if (File.Exists(AutoTestFile) == false)
			{
				this.AutoTestFileTextBox.ForeColor = Color.Red;
			}
			else
			{
				this.AutoTestFileTextBox.ForeColor = SystemColors.ControlText;
			}
		}

		private void AutoTestTimeTrackBar_Scroll(object sender, System.EventArgs e)
		{
			this.AutoTestTimeTextBox.Text = this.AutoTestTimeTrackBar.Value.ToString();
		}

		private void AutoTestDelayTrackBar_Scroll(object sender, System.EventArgs e)
		{
			this.AutoTestDelayTextBox.Text = this.AutoTestDelayTrackBar.Value.ToString();
		}

		private void AutoTestTimeTextBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				int time = Convert.ToInt32(this.AutoTestTimeTextBox.Text);
				this.AutoTestTimeMinutesTextBox.Text = Convert.ToString(time / 60);
			}
			catch
			{
				this.AutoTestTimeMinutesTextBox.Text = "";
			}
		}

		private void MOGAutoTest_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(this.DialogResult == DialogResult.OK)
			{
				UserPrefs.SavePref("AutoTest", "File", this.AutoTestFileTextBox.Text);
				UserPrefs.SavePref("AutoTest", "Project", this.AutoTestProjectComboBox.Text);
				UserPrefs.SavePref("AutoTest", "Time", this.AutoTestTimeTrackBar.Value.ToString());
				UserPrefs.SavePref("AutoTest", "Delay", this.AutoTestDelayTrackBar.Value.ToString());
				UserPrefs.SavePref("AutoTest", "Name", this.AutoTestName);
				UserPrefs.SavePref("AutoTest", "CopyFileLocal", this.AutoTestCopyFileLocal.ToString());
				UserPrefs.SavePref("AutoTest", "StartIndex", this.AutoTestImportStartIndex.ToString());
			}
		}
	}
}


