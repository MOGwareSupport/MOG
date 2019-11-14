using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using MOG;

namespace MOG_LicenseGenerator
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblMacAddress;
		private System.Windows.Forms.Label lblMacExample;
		private System.Windows.Forms.DateTimePicker DatePicker;
		private System.Windows.Forms.Label lblExpiration;
		private System.Windows.Forms.SaveFileDialog mSaveFileDialog;
		private System.Windows.Forms.Button GenerateButton;
		private System.Windows.Forms.TextBox MacAddress;
		private System.Windows.Forms.Label lblClientLicenseCount;
		private NumericUpDown ClientLicenseCount;
		private DateTimePicker TimePicker;
		private Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			DatePicker.Value = DateTime.Today;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.MacAddress = new System.Windows.Forms.TextBox();
			this.lblMacAddress = new System.Windows.Forms.Label();
			this.lblMacExample = new System.Windows.Forms.Label();
			this.DatePicker = new System.Windows.Forms.DateTimePicker();
			this.lblExpiration = new System.Windows.Forms.Label();
			this.mSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.GenerateButton = new System.Windows.Forms.Button();
			this.lblClientLicenseCount = new System.Windows.Forms.Label();
			this.ClientLicenseCount = new System.Windows.Forms.NumericUpDown();
			this.TimePicker = new System.Windows.Forms.DateTimePicker();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.ClientLicenseCount)).BeginInit();
			this.SuspendLayout();
			// 
			// MacAddress
			// 
			this.MacAddress.Location = new System.Drawing.Point(120, 32);
			this.MacAddress.MaxLength = 17;
			this.MacAddress.Name = "MacAddress";
			this.MacAddress.Size = new System.Drawing.Size(152, 20);
			this.MacAddress.TabIndex = 0;
			this.MacAddress.Text = "00:00:00:00:00:00";
			// 
			// lblMacAddress
			// 
			this.lblMacAddress.Location = new System.Drawing.Point(8, 35);
			this.lblMacAddress.Name = "lblMacAddress";
			this.lblMacAddress.Size = new System.Drawing.Size(88, 16);
			this.lblMacAddress.TabIndex = 1;
			this.lblMacAddress.Text = "MAC Address:";
			// 
			// lblMacExample
			// 
			this.lblMacExample.Location = new System.Drawing.Point(104, 8);
			this.lblMacExample.Name = "lblMacExample";
			this.lblMacExample.Size = new System.Drawing.Size(144, 16);
			this.lblMacExample.TabIndex = 2;
			this.lblMacExample.Text = "ex. 00:A0:C2:05:13:FB";
			// 
			// DatePicker
			// 
			this.DatePicker.Location = new System.Drawing.Point(120, 80);
			this.DatePicker.Name = "DatePicker";
			this.DatePicker.Size = new System.Drawing.Size(200, 20);
			this.DatePicker.TabIndex = 3;
			this.DatePicker.Value = new System.DateTime(2007, 4, 18, 0, 0, 0, 0);
			// 
			// lblExpiration
			// 
			this.lblExpiration.Location = new System.Drawing.Point(8, 84);
			this.lblExpiration.Name = "lblExpiration";
			this.lblExpiration.Size = new System.Drawing.Size(80, 16);
			this.lblExpiration.TabIndex = 4;
			this.lblExpiration.Text = "Expiration:";
			// 
			// mSaveFileDialog
			// 
			this.mSaveFileDialog.FileName = "mog_license";
			this.mSaveFileDialog.Filter = "All Files|*.*";
			this.mSaveFileDialog.RestoreDirectory = true;
			// 
			// GenerateButton
			// 
			this.GenerateButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.GenerateButton.Location = new System.Drawing.Point(162, 153);
			this.GenerateButton.Name = "GenerateButton";
			this.GenerateButton.Size = new System.Drawing.Size(75, 23);
			this.GenerateButton.TabIndex = 5;
			this.GenerateButton.Text = "Generate";
			this.GenerateButton.Click += new System.EventHandler(this.mGenerateButton_Click);
			// 
			// lblClientLicenseCount
			// 
			this.lblClientLicenseCount.Location = new System.Drawing.Point(8, 58);
			this.lblClientLicenseCount.Name = "lblClientLicenseCount";
			this.lblClientLicenseCount.Size = new System.Drawing.Size(112, 16);
			this.lblClientLicenseCount.TabIndex = 7;
			this.lblClientLicenseCount.Text = "Client License Count:";
			// 
			// ClientLicenseCount
			// 
			this.ClientLicenseCount.Location = new System.Drawing.Point(120, 56);
			this.ClientLicenseCount.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
			this.ClientLicenseCount.Name = "ClientLicenseCount";
			this.ClientLicenseCount.Size = new System.Drawing.Size(152, 20);
			this.ClientLicenseCount.TabIndex = 8;
			this.ClientLicenseCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// TimePicker
			// 
			this.TimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.TimePicker.Location = new System.Drawing.Point(120, 107);
			this.TimePicker.Name = "TimePicker";
			this.TimePicker.ShowUpDown = true;
			this.TimePicker.Size = new System.Drawing.Size(200, 20);
			this.TimePicker.TabIndex = 9;
			this.TimePicker.Value = new System.DateTime(2007, 9, 7, 14, 0, 0, 0);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 130);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(381, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "12:00 AM (Midnight) means the very beginning of the chosen date, not the end.";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(399, 188);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.TimePicker);
			this.Controls.Add(this.ClientLicenseCount);
			this.Controls.Add(this.lblClientLicenseCount);
			this.Controls.Add(this.GenerateButton);
			this.Controls.Add(this.lblExpiration);
			this.Controls.Add(this.DatePicker);
			this.Controls.Add(this.lblMacExample);
			this.Controls.Add(this.lblMacAddress);
			this.Controls.Add(this.MacAddress);
			this.Name = "Form1";
			this.Text = "MOG License Generator";
			((System.ComponentModel.ISupportInitialize)(this.ClientLicenseCount)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
            Control.CheckForIllegalCrossThreadCalls = false;

            Application.Run(new Form1());
		}

		private void mGenerateButton_Click(object sender, System.EventArgs e)
		{
			if (mSaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				//Take the time of day off the expiration date so it will expire at midnight
				DateTime expiration = DatePicker.Value.Date + TimePicker.Value.TimeOfDay;
				
				int licenseCount = Convert.ToInt32(ClientLicenseCount.Value);

				Generator.Generate(mSaveFileDialog.FileName, MacAddress.Text, expiration, licenseCount);

				MOG_TimeBomb test = new MOG_TimeBomb(mSaveFileDialog.FileName);
				if (!test.Load())
				{
					MessageBox.Show("License File is invalid");
				}
			}
		}
	}
}
