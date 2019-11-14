using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG.TIME;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for NewListForm.
	/// </summary>
	public class NewListForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button GenerateButton;
		private System.Windows.Forms.Button ListCancelButton;
		public System.Windows.Forms.DateTimePicker ListStartDateTimePicker;
		public System.Windows.Forms.DateTimePicker ListEndDateTimePicker;
		private System.Windows.Forms.GroupBox ListDatesGroupBox;
		public System.Windows.Forms.CheckBox ListFilterCheckBox;
		public System.Windows.Forms.TextBox ListFilterTextBox;
		public System.Windows.Forms.CheckBox ListFilterMatchCheckBox;
		private System.Windows.Forms.GroupBox ListFilterGroupBox;
		private System.Windows.Forms.TrackBar ListHourRangeTrackBar;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox ListTimesGroupBox;
		private System.Windows.Forms.GroupBox groupBox1;
		public System.Windows.Forms.RadioButton ListTimeRangeRadioButton;
		public System.Windows.Forms.RadioButton ListDateRangeRadioButton;
		public System.Windows.Forms.RadioButton ListInclusionRadioButton;
		public System.Windows.Forms.RadioButton ListExclusionRadioButton;
		private System.Windows.Forms.RadioButton TodayRadioButton;
		private System.Windows.Forms.RadioButton ThisWeekRadioButton;
		private System.Windows.Forms.RadioButton ThisMonthRadioButton;
		private System.Windows.Forms.RadioButton YesterdayRadioButton;
		public System.Windows.Forms.TextBox ListHoursTextBox;
		public System.Windows.Forms.CheckBox ListSearchAllCheckBox;
		public System.Windows.Forms.RadioButton ListTimeNoRangeRadioButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NewListForm()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(NewListForm));
			this.ListStartDateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.ListEndDateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.ListDatesGroupBox = new System.Windows.Forms.GroupBox();
			this.YesterdayRadioButton = new System.Windows.Forms.RadioButton();
			this.ThisMonthRadioButton = new System.Windows.Forms.RadioButton();
			this.ThisWeekRadioButton = new System.Windows.Forms.RadioButton();
			this.TodayRadioButton = new System.Windows.Forms.RadioButton();
			this.GenerateButton = new System.Windows.Forms.Button();
			this.ListCancelButton = new System.Windows.Forms.Button();
			this.ListFilterCheckBox = new System.Windows.Forms.CheckBox();
			this.ListFilterTextBox = new System.Windows.Forms.TextBox();
			this.ListFilterGroupBox = new System.Windows.Forms.GroupBox();
			this.ListExclusionRadioButton = new System.Windows.Forms.RadioButton();
			this.ListInclusionRadioButton = new System.Windows.Forms.RadioButton();
			this.ListHourRangeTrackBar = new System.Windows.Forms.TrackBar();
			this.label3 = new System.Windows.Forms.Label();
			this.ListTimesGroupBox = new System.Windows.Forms.GroupBox();
			this.ListHoursTextBox = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.ListTimeNoRangeRadioButton = new System.Windows.Forms.RadioButton();
			this.ListDateRangeRadioButton = new System.Windows.Forms.RadioButton();
			this.ListTimeRangeRadioButton = new System.Windows.Forms.RadioButton();
			this.ListSearchAllCheckBox = new System.Windows.Forms.CheckBox();
			this.ListDatesGroupBox.SuspendLayout();
			this.ListFilterGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ListHourRangeTrackBar)).BeginInit();
			this.ListTimesGroupBox.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ListStartDateTimePicker
			// 
			this.ListStartDateTimePicker.CustomFormat = "ddd, MM, yyyy - h:m tt";
			this.ListStartDateTimePicker.Location = new System.Drawing.Point(48, 64);
			this.ListStartDateTimePicker.Name = "ListStartDateTimePicker";
			this.ListStartDateTimePicker.ShowCheckBox = true;
			this.ListStartDateTimePicker.TabIndex = 11;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Start";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "End";
			// 
			// ListEndDateTimePicker
			// 
			this.ListEndDateTimePicker.CustomFormat = "ddd, MM, yyyy - h:m tt";
			this.ListEndDateTimePicker.Location = new System.Drawing.Point(48, 88);
			this.ListEndDateTimePicker.Name = "ListEndDateTimePicker";
			this.ListEndDateTimePicker.ShowCheckBox = true;
			this.ListEndDateTimePicker.TabIndex = 12;
			// 
			// ListDatesGroupBox
			// 
			this.ListDatesGroupBox.Controls.Add(this.YesterdayRadioButton);
			this.ListDatesGroupBox.Controls.Add(this.ThisMonthRadioButton);
			this.ListDatesGroupBox.Controls.Add(this.ThisWeekRadioButton);
			this.ListDatesGroupBox.Controls.Add(this.TodayRadioButton);
			this.ListDatesGroupBox.Controls.Add(this.ListEndDateTimePicker);
			this.ListDatesGroupBox.Controls.Add(this.label2);
			this.ListDatesGroupBox.Controls.Add(this.label1);
			this.ListDatesGroupBox.Controls.Add(this.ListStartDateTimePicker);
			this.ListDatesGroupBox.Enabled = false;
			this.ListDatesGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ListDatesGroupBox.Location = new System.Drawing.Point(40, 152);
			this.ListDatesGroupBox.Name = "ListDatesGroupBox";
			this.ListDatesGroupBox.Size = new System.Drawing.Size(256, 112);
			this.ListDatesGroupBox.TabIndex = 4;
			this.ListDatesGroupBox.TabStop = false;
			this.ListDatesGroupBox.Text = "Ranges";
			// 
			// YesterdayRadioButton
			// 
			this.YesterdayRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.YesterdayRadioButton.Location = new System.Drawing.Point(88, 16);
			this.YesterdayRadioButton.Name = "YesterdayRadioButton";
			this.YesterdayRadioButton.Size = new System.Drawing.Size(67, 24);
			this.YesterdayRadioButton.TabIndex = 8;
			this.YesterdayRadioButton.TabStop = true;
			this.YesterdayRadioButton.Text = "Yesterday";
			this.YesterdayRadioButton.CheckedChanged += new System.EventHandler(this.YesterdayRadioButton_CheckedChanged);
			// 
			// ThisMonthRadioButton
			// 
			this.ThisMonthRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ThisMonthRadioButton.Location = new System.Drawing.Point(88, 40);
			this.ThisMonthRadioButton.Name = "ThisMonthRadioButton";
			this.ThisMonthRadioButton.Size = new System.Drawing.Size(72, 24);
			this.ThisMonthRadioButton.TabIndex = 10;
			this.ThisMonthRadioButton.TabStop = true;
			this.ThisMonthRadioButton.Text = "This month";
			this.ThisMonthRadioButton.CheckedChanged += new System.EventHandler(this.ThisMonthRadioButton_CheckedChanged);
			// 
			// ThisWeekRadioButton
			// 
			this.ThisWeekRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ThisWeekRadioButton.Location = new System.Drawing.Point(16, 40);
			this.ThisWeekRadioButton.Name = "ThisWeekRadioButton";
			this.ThisWeekRadioButton.Size = new System.Drawing.Size(72, 24);
			this.ThisWeekRadioButton.TabIndex = 9;
			this.ThisWeekRadioButton.TabStop = true;
			this.ThisWeekRadioButton.Text = "This week";
			this.ThisWeekRadioButton.CheckedChanged += new System.EventHandler(this.ThisWeekRadioButton_CheckedChanged);
			// 
			// TodayRadioButton
			// 
			this.TodayRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TodayRadioButton.Location = new System.Drawing.Point(16, 16);
			this.TodayRadioButton.Name = "TodayRadioButton";
			this.TodayRadioButton.Size = new System.Drawing.Size(56, 24);
			this.TodayRadioButton.TabIndex = 7;
			this.TodayRadioButton.TabStop = true;
			this.TodayRadioButton.Text = "Today";
			this.TodayRadioButton.CheckedChanged += new System.EventHandler(this.TodayRadioButton_CheckedChanged);
			// 
			// GenerateButton
			// 
			this.GenerateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.GenerateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.GenerateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.GenerateButton.Location = new System.Drawing.Point(224, 438);
			this.GenerateButton.Name = "GenerateButton";
			this.GenerateButton.Size = new System.Drawing.Size(80, 24);
			this.GenerateButton.TabIndex = 0;
			this.GenerateButton.Text = "Generate!";
			// 
			// ListCancelButton
			// 
			this.ListCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ListCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.ListCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ListCancelButton.Location = new System.Drawing.Point(141, 438);
			this.ListCancelButton.Name = "ListCancelButton";
			this.ListCancelButton.Size = new System.Drawing.Size(80, 24);
			this.ListCancelButton.TabIndex = 1;
			this.ListCancelButton.Text = "Cancel";
			// 
			// ListFilterCheckBox
			// 
			this.ListFilterCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ListFilterCheckBox.Location = new System.Drawing.Point(16, 288);
			this.ListFilterCheckBox.Name = "ListFilterCheckBox";
			this.ListFilterCheckBox.TabIndex = 13;
			this.ListFilterCheckBox.Text = "Filter";
			this.ListFilterCheckBox.CheckedChanged += new System.EventHandler(this.ListFilterCheckBox_CheckedChanged);
			// 
			// ListFilterTextBox
			// 
			this.ListFilterTextBox.Location = new System.Drawing.Point(8, 64);
			this.ListFilterTextBox.Name = "ListFilterTextBox";
			this.ListFilterTextBox.Size = new System.Drawing.Size(240, 20);
			this.ListFilterTextBox.TabIndex = 16;
			this.ListFilterTextBox.Text = "";
			// 
			// ListFilterGroupBox
			// 
			this.ListFilterGroupBox.Controls.Add(this.ListExclusionRadioButton);
			this.ListFilterGroupBox.Controls.Add(this.ListInclusionRadioButton);
			this.ListFilterGroupBox.Controls.Add(this.ListFilterTextBox);
			this.ListFilterGroupBox.Enabled = false;
			this.ListFilterGroupBox.Location = new System.Drawing.Point(40, 312);
			this.ListFilterGroupBox.Name = "ListFilterGroupBox";
			this.ListFilterGroupBox.Size = new System.Drawing.Size(264, 91);
			this.ListFilterGroupBox.TabIndex = 11;
			this.ListFilterGroupBox.TabStop = false;
			this.ListFilterGroupBox.Text = "Filter";
			// 
			// ListExclusionRadioButton
			// 
			this.ListExclusionRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ListExclusionRadioButton.Location = new System.Drawing.Point(8, 36);
			this.ListExclusionRadioButton.Name = "ListExclusionRadioButton";
			this.ListExclusionRadioButton.TabIndex = 15;
			this.ListExclusionRadioButton.TabStop = true;
			this.ListExclusionRadioButton.Text = "Exclusion";
			// 
			// ListInclusionRadioButton
			// 
			this.ListInclusionRadioButton.Checked = true;
			this.ListInclusionRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ListInclusionRadioButton.Location = new System.Drawing.Point(8, 13);
			this.ListInclusionRadioButton.Name = "ListInclusionRadioButton";
			this.ListInclusionRadioButton.TabIndex = 14;
			this.ListInclusionRadioButton.TabStop = true;
			this.ListInclusionRadioButton.Text = "Inclusion";
			// 
			// ListHourRangeTrackBar
			// 
			this.ListHourRangeTrackBar.AutoSize = false;
			this.ListHourRangeTrackBar.Location = new System.Drawing.Point(8, 32);
			this.ListHourRangeTrackBar.Maximum = 12;
			this.ListHourRangeTrackBar.Minimum = 1;
			this.ListHourRangeTrackBar.Name = "ListHourRangeTrackBar";
			this.ListHourRangeTrackBar.Size = new System.Drawing.Size(232, 16);
			this.ListHourRangeTrackBar.TabIndex = 5;
			this.ListHourRangeTrackBar.Value = 1;
			this.ListHourRangeTrackBar.ValueChanged += new System.EventHandler(this.ListHourRangeTrackBar_ValueChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 16);
			this.label3.TabIndex = 5;
			this.label3.Text = "Within           hours";
			// 
			// ListTimesGroupBox
			// 
			this.ListTimesGroupBox.Controls.Add(this.ListHoursTextBox);
			this.ListTimesGroupBox.Controls.Add(this.ListHourRangeTrackBar);
			this.ListTimesGroupBox.Controls.Add(this.label3);
			this.ListTimesGroupBox.Enabled = false;
			this.ListTimesGroupBox.Location = new System.Drawing.Point(40, 72);
			this.ListTimesGroupBox.Name = "ListTimesGroupBox";
			this.ListTimesGroupBox.Size = new System.Drawing.Size(248, 54);
			this.ListTimesGroupBox.TabIndex = 14;
			this.ListTimesGroupBox.TabStop = false;
			this.ListTimesGroupBox.Text = "Range";
			// 
			// ListHoursTextBox
			// 
			this.ListHoursTextBox.Location = new System.Drawing.Point(53, 12);
			this.ListHoursTextBox.Name = "ListHoursTextBox";
			this.ListHoursTextBox.Size = new System.Drawing.Size(24, 20);
			this.ListHoursTextBox.TabIndex = 4;
			this.ListHoursTextBox.Text = "1";
			this.ListHoursTextBox.TextChanged += new System.EventHandler(this.ListHoursTextBox_TextChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.ListTimeNoRangeRadioButton);
			this.groupBox1.Controls.Add(this.ListDateRangeRadioButton);
			this.groupBox1.Controls.Add(this.ListTimeRangeRadioButton);
			this.groupBox1.Controls.Add(this.ListTimesGroupBox);
			this.groupBox1.Controls.Add(this.ListDatesGroupBox);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(304, 272);
			this.groupBox1.TabIndex = 15;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Range filter";
			// 
			// ListTimeNoRangeRadioButton
			// 
			this.ListTimeNoRangeRadioButton.Checked = true;
			this.ListTimeNoRangeRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ListTimeNoRangeRadioButton.Location = new System.Drawing.Point(16, 16);
			this.ListTimeNoRangeRadioButton.Name = "ListTimeNoRangeRadioButton";
			this.ListTimeNoRangeRadioButton.Size = new System.Drawing.Size(184, 24);
			this.ListTimeNoRangeRadioButton.TabIndex = 2;
			this.ListTimeNoRangeRadioButton.TabStop = true;
			this.ListTimeNoRangeRadioButton.Text = "No Range Filter";
			// 
			// ListDateRangeRadioButton
			// 
			this.ListDateRangeRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ListDateRangeRadioButton.Location = new System.Drawing.Point(16, 128);
			this.ListDateRangeRadioButton.Name = "ListDateRangeRadioButton";
			this.ListDateRangeRadioButton.Size = new System.Drawing.Size(184, 24);
			this.ListDateRangeRadioButton.TabIndex = 6;
			this.ListDateRangeRadioButton.TabStop = true;
			this.ListDateRangeRadioButton.Text = "Specify date range";
			this.ListDateRangeRadioButton.CheckedChanged += new System.EventHandler(this.ListDateRangeCheckBox_CheckedChanged);
			// 
			// ListTimeRangeRadioButton
			// 
			this.ListTimeRangeRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ListTimeRangeRadioButton.Location = new System.Drawing.Point(16, 48);
			this.ListTimeRangeRadioButton.Name = "ListTimeRangeRadioButton";
			this.ListTimeRangeRadioButton.Size = new System.Drawing.Size(184, 24);
			this.ListTimeRangeRadioButton.TabIndex = 3;
			this.ListTimeRangeRadioButton.TabStop = true;
			this.ListTimeRangeRadioButton.Text = "Search within time range";
			this.ListTimeRangeRadioButton.CheckedChanged += new System.EventHandler(this.ListTimeRangeCheckBox_CheckedChanged);
			// 
			// ListSearchAllCheckBox
			// 
			this.ListSearchAllCheckBox.Enabled = false;
			this.ListSearchAllCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ListSearchAllCheckBox.Location = new System.Drawing.Point(16, 408);
			this.ListSearchAllCheckBox.Name = "ListSearchAllCheckBox";
			this.ListSearchAllCheckBox.Size = new System.Drawing.Size(224, 24);
			this.ListSearchAllCheckBox.TabIndex = 16;
			this.ListSearchAllCheckBox.TabStop = false;
			this.ListSearchAllCheckBox.Text = "Search entire tree";
			// 
			// NewListForm
			// 
			this.AcceptButton = this.GenerateButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.ListCancelButton;
			this.ClientSize = new System.Drawing.Size(314, 467);
			this.Controls.Add(this.ListSearchAllCheckBox);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.ListFilterGroupBox);
			this.Controls.Add(this.ListFilterCheckBox);
			this.Controls.Add(this.ListCancelButton);
			this.Controls.Add(this.GenerateButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewListForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create Asset List";
			this.ListDatesGroupBox.ResumeLayout(false);
			this.ListFilterGroupBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ListHourRangeTrackBar)).EndInit();
			this.ListTimesGroupBox.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ListDateRangeCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			ListDatesGroupBox.Enabled = ListDateRangeRadioButton.Checked;
		}

		private void ListFilterCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			ListFilterGroupBox.Enabled = ListFilterCheckBox.Checked;
		}

		private void ListTimeRangeCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			ListTimesGroupBox.Enabled = ListTimeRangeRadioButton.Checked;
		}

		private void ListHourRangeTrackBar_ValueChanged(object sender, System.EventArgs e)
		{
			this.ListHoursTextBox.Text = this.ListHourRangeTrackBar.Value.ToString();
		}

		private void TodayRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			MOG_Time now = new MOG_Time();			
			this.ListStartDateTimePicker.Value = new DateTime(now.mYear, now.mMonth, now.mDay, 0, 0, 0, 0);
			this.ListEndDateTimePicker.Value = new DateTime(now.mYear, now.mMonth, now.mDay, 23, 59, 59, 59);
		}

		private void ThisWeekRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			MOG_Time start = new MOG_Time();
			MOG_Time now = new MOG_Time();			
			
			start.SubtractDay(start.mDayOfWeek);

			this.ListStartDateTimePicker.Value = new DateTime(start.mYear, start.mMonth, start.mDay, 0, 0, 0, 0);
			this.ListEndDateTimePicker.Value = new DateTime(now.mYear, now.mMonth, now.mDay, 23, 59, 59, 59);
		}

		private void YesterdayRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			MOG_Time start = new MOG_Time();
			
			start.SubtractDay(1);

			this.ListStartDateTimePicker.Value = new DateTime(start.mYear, start.mMonth, start.mDay, 0, 0, 0, 0);
			this.ListEndDateTimePicker.Value = new DateTime(start.mYear, start.mMonth, start.mDay, 23, 59, 59, 59);
		}

		private void ThisMonthRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			MOG_Time start = new MOG_Time();
			MOG_Time now = new MOG_Time();			
			
			start.SubtractDay(start.mDay);

			this.ListStartDateTimePicker.Value = new DateTime(start.mYear, start.mMonth, start.mDay, 0, 0, 0, 0);
			this.ListEndDateTimePicker.Value = new DateTime(now.mYear, now.mMonth, now.mDay, 23, 59, 59, 59);
		}

		private void ListHoursTextBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				int val = Convert.ToInt32(ListHoursTextBox.Text);
				if(val <= ListHourRangeTrackBar.Maximum && val >= ListHourRangeTrackBar.Minimum)
				{
					ListHourRangeTrackBar.Value = val;
				}
			}
			catch
			{
				ListHoursTextBox.Text = "1";
			}
		}
	}
}
