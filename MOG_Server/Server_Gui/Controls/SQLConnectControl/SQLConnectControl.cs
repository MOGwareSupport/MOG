using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Data.SqlClient;

using MOG.INI;
using MOG.REPORT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG_Server.MOG_ControlsLibrary.Common;
using MOG_Server.MOG_ControlsLibrary.Admin;


namespace MOG_Server.Server_Gui
{
	/// <summary>
	/// Summary description for SQLConnectControl.
	/// </summary>
	public class SQLConnectControl : System.Windows.Forms.UserControl
	{
		#region System definitions

		private System.Windows.Forms.ComboBox SQLServerComboBox;
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton SQLNTSecurityRadioButton;
		private System.Windows.Forms.TextBox SQLUserTextBox;
		private System.Windows.Forms.TextBox SQLPasswordTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox SQLDatabaseComboBox;
		private System.Windows.Forms.Button SQLOkButton;
		private System.Windows.Forms.Button SQLCancelButton;
		private System.Windows.Forms.RadioButton SQLUserRadioButton;
		private System.Windows.Forms.Button SQLTestButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
			this.SQLServerComboBox = new System.Windows.Forms.ComboBox();
			this.label = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SQLNTSecurityRadioButton = new System.Windows.Forms.RadioButton();
			this.SQLUserRadioButton = new System.Windows.Forms.RadioButton();
			this.SQLUserTextBox = new System.Windows.Forms.TextBox();
			this.SQLPasswordTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SQLDatabaseComboBox = new System.Windows.Forms.ComboBox();
			this.SQLOkButton = new System.Windows.Forms.Button();
			this.SQLCancelButton = new System.Windows.Forms.Button();
			this.SQLTestButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// SQLServerComboBox
			// 
			this.SQLServerComboBox.Location = new System.Drawing.Point(24, 72);
			this.SQLServerComboBox.Name = "SQLServerComboBox";
			this.SQLServerComboBox.Size = new System.Drawing.Size(240, 21);
			this.SQLServerComboBox.TabIndex = 0;
			this.SQLServerComboBox.DropDown += new System.EventHandler(this.SQLServerComboBox_DropDown);
			this.SQLServerComboBox.SelectedIndexChanged += new System.EventHandler(this.SQLServerComboBox_SelectedIndexChanged);
			// 
			// label
			// 
			this.label.Location = new System.Drawing.Point(8, 8);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(256, 32);
			this.label.TabIndex = 1;
			this.label.Text = "Creating a connection to the SQL database takes thee steps:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(174, 18);
			this.label1.TabIndex = 2;
			this.label1.Text = "1. Select or enter a server name:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 104);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(240, 18);
			this.label2.TabIndex = 3;
			this.label2.Text = "2. Enter information to log on to the server:";
			// 
			// SQLNTSecurityRadioButton
			// 
			this.SQLNTSecurityRadioButton.Checked = true;
			this.SQLNTSecurityRadioButton.Location = new System.Drawing.Point(48, 128);
			this.SQLNTSecurityRadioButton.Name = "SQLNTSecurityRadioButton";
			this.SQLNTSecurityRadioButton.Size = new System.Drawing.Size(224, 24);
			this.SQLNTSecurityRadioButton.TabIndex = 4;
			this.SQLNTSecurityRadioButton.TabStop = true;
			this.SQLNTSecurityRadioButton.Text = "Use Windows NT integrated security";
			this.SQLNTSecurityRadioButton.Click += new System.EventHandler(this.SQLNTSecurityRadioButton_Click);
			// 
			// SQLUserRadioButton
			// 
			this.SQLUserRadioButton.Location = new System.Drawing.Point(48, 152);
			this.SQLUserRadioButton.Name = "SQLUserRadioButton";
			this.SQLUserRadioButton.Size = new System.Drawing.Size(224, 24);
			this.SQLUserRadioButton.TabIndex = 5;
			this.SQLUserRadioButton.Text = "Use specific user name and password";
			this.SQLUserRadioButton.Click += new System.EventHandler(this.SQLUserRadioButton_Click);
			// 
			// SQLUserTextBox
			// 
			this.SQLUserTextBox.Enabled = false;
			this.SQLUserTextBox.Location = new System.Drawing.Point(128, 176);
			this.SQLUserTextBox.Name = "SQLUserTextBox";
			this.SQLUserTextBox.Size = new System.Drawing.Size(176, 20);
			this.SQLUserTextBox.TabIndex = 6;
			this.SQLUserTextBox.Text = "sa";
			// 
			// SQLPasswordTextBox
			// 
			this.SQLPasswordTextBox.Enabled = false;
			this.SQLPasswordTextBox.Location = new System.Drawing.Point(128, 200);
			this.SQLPasswordTextBox.Name = "SQLPasswordTextBox";
			this.SQLPasswordTextBox.PasswordChar = '*';
			this.SQLPasswordTextBox.Size = new System.Drawing.Size(176, 20);
			this.SQLPasswordTextBox.TabIndex = 7;
			this.SQLPasswordTextBox.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(64, 176);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 18);
			this.label3.TabIndex = 8;
			this.label3.Text = "User Name:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(64, 200);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 18);
			this.label4.TabIndex = 9;
			this.label4.Text = "Passsword:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(16, 240);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(240, 18);
			this.label5.TabIndex = 10;
			this.label5.Text = "3. Select the database on the server:";
			// 
			// SQLDatabaseComboBox
			// 
			this.SQLDatabaseComboBox.Location = new System.Drawing.Point(24, 264);
			this.SQLDatabaseComboBox.Name = "SQLDatabaseComboBox";
			this.SQLDatabaseComboBox.Size = new System.Drawing.Size(240, 21);
			this.SQLDatabaseComboBox.TabIndex = 11;
			// 
			// SQLOkButton
			// 
			this.SQLOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.SQLOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLOkButton.Location = new System.Drawing.Point(152, 296);
			this.SQLOkButton.Name = "SQLOkButton";
			this.SQLOkButton.TabIndex = 12;
			this.SQLOkButton.Text = "Ok";
			this.SQLOkButton.Click += new System.EventHandler(this.SQLOkButton_Click);
			// 
			// SQLCancelButton
			// 
			this.SQLCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.SQLCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLCancelButton.Location = new System.Drawing.Point(232, 296);
			this.SQLCancelButton.Name = "SQLCancelButton";
			this.SQLCancelButton.TabIndex = 13;
			this.SQLCancelButton.Text = "Cancel";
			// 
			// SQLTestButton
			// 
			this.SQLTestButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLTestButton.Location = new System.Drawing.Point(16, 296);
			this.SQLTestButton.Name = "SQLTestButton";
			this.SQLTestButton.Size = new System.Drawing.Size(96, 23);
			this.SQLTestButton.TabIndex = 14;
			this.SQLTestButton.Text = "Test Connection";
			this.SQLTestButton.Click += new System.EventHandler(this.SQLTestButton_Click);
			// 
			// SQLConnectControl
			// 
			this.Controls.Add(this.SQLTestButton);
			this.Controls.Add(this.SQLCancelButton);
			this.Controls.Add(this.SQLOkButton);
			this.Controls.Add(this.SQLDatabaseComboBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.SQLPasswordTextBox);
			this.Controls.Add(this.SQLUserTextBox);
			this.Controls.Add(this.SQLUserRadioButton);
			this.Controls.Add(this.SQLNTSecurityRadioButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label);
			this.Controls.Add(this.SQLServerComboBox);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Name = "SQLConnectControl";
			this.Size = new System.Drawing.Size(312, 328);
			this.ResumeLayout(false);

		}
		#endregion

		#endregion
		#region Member vars
		public string mConnectString;
		private string packetSize;
		private string security;
		private string dataSource;
		private string persistSecurity;
		private string initialCatalog;

		private bool refreshServerListOnDropDown = true;
		#endregion
		#region Properties
		public ArrayList ServerNames
		{
			get 
			{
				ArrayList names = new ArrayList();
				foreach (string name in this.SQLServerComboBox.Items)
					names.Add(name);

				return names;
			}

			set
			{
				this.SQLServerComboBox.Items.Clear();
				foreach (string name in value)
					this.SQLServerComboBox.Items.Add(name);
			}
		}

		public string ServerName
		{
			get 
			{
				return this.SQLServerComboBox.Text;
			}
			set
			{
				if (!this.SQLServerComboBox.Items.Contains(value))
					this.SQLServerComboBox.Items.Add(value);

				this.SQLServerComboBox.SelectedItem = value;
			}
		}

		public string DatabaseName
		{
			get 
			{
				return this.SQLDatabaseComboBox.Text;
			}
			set
			{
				if (value == "")
				{
					this.SQLDatabaseComboBox.SelectedItem = null;
					this.SQLDatabaseComboBox.Text = "";
					return;
				}

				if (!this.SQLDatabaseComboBox.Items.Contains(value))
					this.SQLDatabaseComboBox.Items.Add(value);

				this.SQLDatabaseComboBox.SelectedItem = value;
			}
		}

		public string DataSource
		{
			get { return this.dataSource; }
			set { this.dataSource = value; }
		}

		public string InitialCatalog
		{
			get { return this.initialCatalog; }
			set { this.initialCatalog = value; }
		}

		public bool ButtonsVisible
		{
			get { return (this.SQLOkButton.Visible && this.SQLCancelButton.Visible && this.SQLTestButton.Visible); }
			set { this.SQLOkButton.Visible = this.SQLCancelButton.Visible = this.SQLTestButton.Visible = value; }
		}

		public bool OKButtonVisible 
		{
			get { return this.SQLOkButton.Visible; }
			set { this.SQLOkButton.Visible = value; }
		}

		public bool SQLCancelButtonVisible 
		{
			get { return this.SQLCancelButton.Visible; }
			set { this.SQLCancelButton.Visible = value; }
		}

		public bool SQLTestButtonVisible 
		{
			get { return this.SQLTestButton.Visible; }
			set { this.SQLTestButton.Visible = true; }//value; }
		}

		public string ConnectionString
		{
			get { return this.mConnectString; }
		}
		#endregion
		#region Constructors
		public SQLConnectControl()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// packet size=4096;integrated security=SSPI;data source="NEMESIS";persist security info=False;initial catalog=mog
			packetSize = "4096";
			security = "SSPI";
			persistSecurity = "False";
			dataSource = "NEMESIS";
			initialCatalog = "mog";
			
			SQLServerComboBox.Text = dataSource;
			SQLDatabaseComboBox.Text =initialCatalog;

			mConnectString = "";
		}
		#endregion
		#region Events
		// events
		public event EventHandler OKClicked;
		public event EventHandler CancelClicked;
		public event EventHandler TestButtonClicked;
		#endregion
		#region Public functions
		// outside access to test connection button's functionality
		public void TestConnection()
		{
			this.SQLTestButton.PerformClick();
		}

		public void SaveToIni(string iniFilename)
		{
			MOG_Ini ini = new MOG_Ini(iniFilename);
			ini.PutString("SQL", "ConnectionString", this.mConnectString);
			ini.Save();
			ini.Close();
		}
		#endregion
		#region Private functions

		// raise OKClicked event
		private void OnOKClicked()
		{
			if (this.OKClicked != null)
				this.OKClicked(this, new EventArgs());
		}

		// raise OKClicked event
		private void OnCancelClicked()
		{
			if (this.CancelClicked != null)
				this.CancelClicked(this, new EventArgs());
		}

		// raise OKClicked event
		private void OnTestButtonClicked()
		{
			if (this.TestButtonClicked != null)
				this.TestButtonClicked(this, new EventArgs());
		}

		public void BuildConnectionString()
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
		#endregion
		#region Event handlers

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

			
//			SqlConnection myConnection = new SqlConnection(mConnectString);
//			try
//			{
//				myConnection.Open();
//			}
//			catch(Exception ex)

			if (!MOG_ControllerSystem.InitializeDatabase(mConnectString, "", ""))
			{
				//MOG_Report.ReportMessage("SQL Database", "Could not open SQL database!", MOGPromptButtons.OK);
				Utils.ShowMessageBoxExclamation("Couldn't connect to SQL database " + this.SQLDatabaseComboBox.Text + " on server " + SQLServerComboBox.Text, "Connection Failure");
				return;
			}

			//MOG_Report.ReportMessage("SQL Database", "Successfully connected to SQL database " + SQLServerComboBox.Text + "!",MOGPromptButtons.OK);
			Utils.ShowMessageBoxInfo("Successfully connected to SQL database " + this.SQLDatabaseComboBox.Text + " on server " + SQLServerComboBox.Text, "Connection Successful");

			// notify others that we clicked the Test button
			OnTestButtonClicked();
		}

		private void SQLOkButton_Click(object sender, System.EventArgs e)
		{
			BuildConnectionString();

			// notify others that we clicked the OK button
			OnOKClicked();
		}

		private void SQLCancelButton_Click(object sender, System.EventArgs e)
		{
			// notify others that we clicked the Cancel button
			OnCancelClicked();
		}

		private void SQLConnectControl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
				this.SQLOkButton.PerformClick();
			else if (e.KeyData == Keys.Escape)
				this.SQLCancelButton.PerformClick();
		}

		private void SQLServerComboBox_DropDown(object sender, System.EventArgs e)
		{
			if (this.refreshServerListOnDropDown)
			{
				this.Cursor = Cursors.WaitCursor;
				Application.DoEvents();

				ArrayList servers = SqlServerLocator.FindServers();
				SQLServerComboBox.Items.Clear();

				foreach (string server in servers)
				{
					if (server.ToLower() == "(local)")
						SQLServerComboBox.Items.Add( Environment.MachineName.ToUpper() );
					else
						SQLServerComboBox.Items.Add(server);
				}

				this.Cursor = Cursors.Default;
				this.refreshServerListOnDropDown = false;
			}
		}

		private void SQLServerComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.SQLServerComboBox.SelectedItem != null  &&  this.SQLServerComboBox.SelectedItem is string)
			{
				this.SQLDatabaseComboBox.Enabled = false;
				this.SQLDatabaseComboBox.SelectedItem = null;
				this.SQLDatabaseComboBox.Text = "";
				this.SQLDatabaseComboBox.Items.Clear();
				Application.DoEvents();
				Cursor.Current = Cursors.WaitCursor;

				string serverName = this.SQLServerComboBox.SelectedItem as string;

				foreach(string databaseName in SqlServerLocator.FindDatabases(serverName))
				{
					this.SQLDatabaseComboBox.Items.Add(databaseName);
				}

				if (this.SQLDatabaseComboBox.Items.Count > 0)
				{
					this.SQLDatabaseComboBox.SelectedItem = this.SQLDatabaseComboBox.Items[0];
				}

				this.SQLDatabaseComboBox.Enabled = true;
				Cursor.Current = Cursors.Default;
			}
		}
	}
		#endregion
}



/*
#region System definitions
#endregion
#region Member vars
#endregion
#region Properties
#endregion
#region Constructors
#endregion
#region Member functions
#endregion
#region Event handlers
#endregion
*/ 


