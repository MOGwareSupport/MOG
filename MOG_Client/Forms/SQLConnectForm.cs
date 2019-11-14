using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Data.SqlClient;

using MOG;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.INI;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG_ControlsLibrary.Common;
using MOG_ControlsLibrary.Common.MogUtil_Sql;


namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for SQLConnectForm.
	/// </summary>
	public class SQLConnectForm : System.Windows.Forms.Form
	{
		public string mConnectString;
		private string packetSize;
		private string security;
		private string dataSource;
		private string persistSecurity;
		private string initialCatalog;

		private System.Windows.Forms.ComboBox SQLServerComboBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton SQLNTSecurityRadioButton;
		private System.Windows.Forms.TextBox SQLUserTextBox;
		private System.Windows.Forms.TextBox SQLPasswordTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox SQLDatabaseComboBox;
		private System.Windows.Forms.Button SQLCancelButton;
		private System.Windows.Forms.RadioButton SQLUserRadioButton;
		private System.Windows.Forms.Button SQLTestButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SQLConnectForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			packetSize = "4096";
			security = "SSPI";
			persistSecurity = "False";
			dataSource = "";
			initialCatalog = "MOG";

			MOG_Ini config = new MOG_Ini(MOG_Main.GetExecutablePath() + "\\MOG.ini");
			if (config.KeyExist("SQL", "ConnectionString"))
			{				
				string connectionString = config.GetString("SQL", "ConnectionString");
				string []tokens;
				tokens = connectionString.Split(";".ToCharArray());
				foreach (string token in tokens)
				{
					string []values = token.Split("=".ToCharArray());
					if (values != null && values.Length >= 2)
					{
						switch(values[0].ToLower())
						{
							case "data source":
								dataSource = values[1];
								break;
							case "initial catalog":
								initialCatalog = values[1];
								break;
						}
					}
				}
			}

			// packet size=4096;integrated security=SSPI;data source="NEMESIS";persist security info=False;initial catalog=mog
			
			SQLServerComboBox.Text = dataSource;
			SQLDatabaseComboBox.Text = initialCatalog;

			mConnectString = "";
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SQLConnectForm));
			this.SQLServerComboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SQLNTSecurityRadioButton = new System.Windows.Forms.RadioButton();
			this.SQLUserRadioButton = new System.Windows.Forms.RadioButton();
			this.SQLUserTextBox = new System.Windows.Forms.TextBox();
			this.SQLPasswordTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SQLDatabaseComboBox = new System.Windows.Forms.ComboBox();
			this.SQLCancelButton = new System.Windows.Forms.Button();
			this.SQLTestButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// SQLServerComboBox
			// 
			this.SQLServerComboBox.Location = new System.Drawing.Point(24, 27);
			this.SQLServerComboBox.Name = "SQLServerComboBox";
			this.SQLServerComboBox.Size = new System.Drawing.Size(240, 21);
			this.SQLServerComboBox.TabIndex = 0;
			this.SQLServerComboBox.DropDown += new System.EventHandler(this.SQLServerComboBox_DropDown);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(174, 18);
			this.label1.TabIndex = 2;
			this.label1.Text = "Server name:";
			// 
			// SQLNTSecurityRadioButton
			// 
			this.SQLNTSecurityRadioButton.Checked = true;
			this.SQLNTSecurityRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLNTSecurityRadioButton.Location = new System.Drawing.Point(16, 16);
			this.SQLNTSecurityRadioButton.Name = "SQLNTSecurityRadioButton";
			this.SQLNTSecurityRadioButton.Size = new System.Drawing.Size(224, 24);
			this.SQLNTSecurityRadioButton.TabIndex = 4;
			this.SQLNTSecurityRadioButton.TabStop = true;
			this.SQLNTSecurityRadioButton.Text = "Use Windows Authentication";
			this.SQLNTSecurityRadioButton.Click += new System.EventHandler(this.SQLNTSecurityRadioButton_Click);
			// 
			// SQLUserRadioButton
			// 
			this.SQLUserRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLUserRadioButton.Location = new System.Drawing.Point(16, 40);
			this.SQLUserRadioButton.Name = "SQLUserRadioButton";
			this.SQLUserRadioButton.Size = new System.Drawing.Size(224, 24);
			this.SQLUserRadioButton.TabIndex = 5;
			this.SQLUserRadioButton.Text = "Use SQL Authentication";
			this.SQLUserRadioButton.Click += new System.EventHandler(this.SQLUserRadioButton_Click);
			// 
			// SQLUserTextBox
			// 
			this.SQLUserTextBox.Enabled = false;
			this.SQLUserTextBox.Location = new System.Drawing.Point(104, 64);
			this.SQLUserTextBox.Name = "SQLUserTextBox";
			this.SQLUserTextBox.Size = new System.Drawing.Size(136, 20);
			this.SQLUserTextBox.TabIndex = 6;
			this.SQLUserTextBox.Text = "sa";
			// 
			// SQLPasswordTextBox
			// 
			this.SQLPasswordTextBox.Enabled = false;
			this.SQLPasswordTextBox.Location = new System.Drawing.Point(104, 88);
			this.SQLPasswordTextBox.Name = "SQLPasswordTextBox";
			this.SQLPasswordTextBox.PasswordChar = '*';
			this.SQLPasswordTextBox.Size = new System.Drawing.Size(136, 20);
			this.SQLPasswordTextBox.TabIndex = 7;
			this.SQLPasswordTextBox.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(32, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 18);
			this.label3.TabIndex = 8;
			this.label3.Text = "User Name:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(32, 88);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 18);
			this.label4.TabIndex = 9;
			this.label4.Text = "Passsword:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(224, 18);
			this.label5.TabIndex = 10;
			this.label5.Text = "Select database:";
			// 
			// SQLDatabaseComboBox
			// 
			this.SQLDatabaseComboBox.Location = new System.Drawing.Point(16, 48);
			this.SQLDatabaseComboBox.Name = "SQLDatabaseComboBox";
			this.SQLDatabaseComboBox.Size = new System.Drawing.Size(232, 21);
			this.SQLDatabaseComboBox.TabIndex = 11;
			// 
			// SQLCancelButton
			// 
			this.SQLCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.SQLCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.SQLCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLCancelButton.Location = new System.Drawing.Point(207, 288);
			this.SQLCancelButton.Name = "SQLCancelButton";
			this.SQLCancelButton.Size = new System.Drawing.Size(64, 23);
			this.SQLCancelButton.TabIndex = 13;
			this.SQLCancelButton.Text = "Close";
			// 
			// SQLTestButton
			// 
			this.SQLTestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.SQLTestButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLTestButton.Location = new System.Drawing.Point(16, 288);
			this.SQLTestButton.Name = "SQLTestButton";
			this.SQLTestButton.Size = new System.Drawing.Size(96, 23);
			this.SQLTestButton.TabIndex = 14;
			this.SQLTestButton.Text = "Test Connection";
			this.SQLTestButton.Click += new System.EventHandler(this.SQLTestButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.SQLUserRadioButton);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.SQLUserTextBox);
			this.groupBox1.Controls.Add(this.SQLNTSecurityRadioButton);
			this.groupBox1.Controls.Add(this.SQLPasswordTextBox);
			this.groupBox1.Location = new System.Drawing.Point(16, 56);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(256, 120);
			this.groupBox1.TabIndex = 15;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Log on to the server";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.SQLDatabaseComboBox);
			this.groupBox2.Location = new System.Drawing.Point(16, 192);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(256, 88);
			this.groupBox2.TabIndex = 16;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Connect to a database";
			// 
			// SQLConnectForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.SQLCancelButton;
			this.ClientSize = new System.Drawing.Size(282, 323);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.SQLTestButton);
			this.Controls.Add(this.SQLCancelButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.SQLServerComboBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SQLConnectForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Test SQL Connection";
			this.TopMost = true;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.SQLConnectForm_Closing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void BuildConnectionString()
		{
			mConnectString = "Packet size=" +packetSize + ";";

			if (SQLNTSecurityRadioButton.Checked)
			{
				mConnectString = mConnectString + "integrated security=" + security + ";";
			}
			else
			{
				mConnectString = mConnectString + "User ID=" + SQLUserTextBox.Text + ";";
				mConnectString = mConnectString + "Password=" + SQLPasswordTextBox.Text + ";";
			}

			mConnectString = mConnectString + "data source=" + SQLServerComboBox.Text + ";";
			mConnectString = mConnectString + "persist security info=" + persistSecurity + ";";


			mConnectString = mConnectString + "initial catalog=" + SQLDatabaseComboBox.Text + ";";
		}

		private void SQLConnectForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			BuildConnectionString();
		}

		private void SQLUserRadioButton_Click(object sender, System.EventArgs e)
		{
			SQLUserTextBox.Enabled = true;
			SQLPasswordTextBox.Enabled = true;
		}

		private void SQLNTSecurityRadioButton_Click(object sender, System.EventArgs e)
		{
			SQLUserTextBox.Enabled = false;
			SQLPasswordTextBox.Enabled = false;
		}

		private void SQLTestButton_Click(object sender, System.EventArgs e)
		{
			// Test the connection string
			// Attempt to open a connection to make sure we will work?
			BuildConnectionString();

			SqlConnection myConnection = new SqlConnection(mConnectString);

			MOG_Prompt.PromptMessage("SQL Connection", "Successfully connected to SQL database " + SQLServerComboBox.Text + "!");
		}

		private void SQLServerComboBox_DropDown(object sender, System.EventArgs e)
		{
			SQLServerComboBox.Items.Clear();

			string []servers = SqlLocator.GetServers();
			if (servers != null)
			{
				this.Cursor = Cursors.WaitCursor;
				Application.DoEvents();

				foreach (string server in servers)
				{
					SQLServerComboBox.Items.Add(server);
				}

				this.Cursor = Cursors.Default;
			}
			else
			{				
				SQLServerComboBox.Items.Add(MOG_ControllerSystem.GetComputerName());
			}
		}
	}
}
