using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Data.SqlClient;

using MOG.INI;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

//using MOG_Server.MOG_ControlsLibrary.Common;
//using MOG_Server.MOG_ControlsLibrary.Admin;


namespace MOG_ControlsLibrary.Forms
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
		private System.Windows.Forms.RadioButton SQLNTSecurityRadioButton;
		private System.Windows.Forms.TextBox SQLUserTextBox;
		private System.Windows.Forms.TextBox SQLPasswordTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox SQLDatabaseComboBox;
		private System.Windows.Forms.Button SQLOkButton;
		private System.Windows.Forms.Button SQLCancelButton;
		private System.Windows.Forms.RadioButton SQLUserRadioButton;
		private System.Windows.Forms.Button SQLTestButton;
		private System.Windows.Forms.TextBox SQLDatabaseTextBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton SQLCreateDatabaseRadioButton;
		private System.Windows.Forms.RadioButton SQLSelectDatabaseRadioButton;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button SqlRefreshButton;
		private System.Windows.Forms.Button SQLDatabaseRefreshButton;
		private System.Windows.Forms.Label label2;
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
			this.SQLNTSecurityRadioButton = new System.Windows.Forms.RadioButton();
			this.SQLUserRadioButton = new System.Windows.Forms.RadioButton();
			this.SQLUserTextBox = new System.Windows.Forms.TextBox();
			this.SQLPasswordTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.SQLDatabaseComboBox = new System.Windows.Forms.ComboBox();
			this.SQLOkButton = new System.Windows.Forms.Button();
			this.SQLCancelButton = new System.Windows.Forms.Button();
			this.SQLTestButton = new System.Windows.Forms.Button();
			this.SQLDatabaseTextBox = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.SQLDatabaseRefreshButton = new System.Windows.Forms.Button();
			this.SQLSelectDatabaseRadioButton = new System.Windows.Forms.RadioButton();
			this.SQLCreateDatabaseRadioButton = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.SqlRefreshButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// SQLServerComboBox
			// 
			this.SQLServerComboBox.Location = new System.Drawing.Point(24, 88);
			this.SQLServerComboBox.Name = "SQLServerComboBox";
			this.SQLServerComboBox.Size = new System.Drawing.Size(248, 21);
			this.SQLServerComboBox.TabIndex = 0;
			this.SQLServerComboBox.SelectedIndexChanged += new System.EventHandler(this.SQLServerComboBox_SelectedIndexChanged);
			// 
			// label
			// 
			this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label.Location = new System.Drawing.Point(8, 8);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(344, 16);
			this.label.TabIndex = 1;
			this.label.Text = "Configure your SQL connection.";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(174, 18);
			this.label1.TabIndex = 2;
			this.label1.Text = "Server name:";
			// 
			// SQLNTSecurityRadioButton
			// 
			this.SQLNTSecurityRadioButton.Checked = true;
			this.SQLNTSecurityRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLNTSecurityRadioButton.Location = new System.Drawing.Point(16, 24);
			this.SQLNTSecurityRadioButton.Name = "SQLNTSecurityRadioButton";
			this.SQLNTSecurityRadioButton.Size = new System.Drawing.Size(224, 24);
			this.SQLNTSecurityRadioButton.TabIndex = 4;
			this.SQLNTSecurityRadioButton.TabStop = true;
			this.SQLNTSecurityRadioButton.Text = "Use &Windows Authentication";
			this.SQLNTSecurityRadioButton.Click += new System.EventHandler(this.SQLNTSecurityRadioButton_Click);
			// 
			// SQLUserRadioButton
			// 
			this.SQLUserRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLUserRadioButton.Location = new System.Drawing.Point(16, 48);
			this.SQLUserRadioButton.Name = "SQLUserRadioButton";
			this.SQLUserRadioButton.Size = new System.Drawing.Size(224, 24);
			this.SQLUserRadioButton.TabIndex = 5;
			this.SQLUserRadioButton.Text = "Use S&QL Server Authentication";
			this.SQLUserRadioButton.Click += new System.EventHandler(this.SQLUserRadioButton_Click);
			// 
			// SQLUserTextBox
			// 
			this.SQLUserTextBox.Enabled = false;
			this.SQLUserTextBox.Location = new System.Drawing.Point(96, 72);
			this.SQLUserTextBox.Name = "SQLUserTextBox";
			this.SQLUserTextBox.Size = new System.Drawing.Size(224, 20);
			this.SQLUserTextBox.TabIndex = 6;
			this.SQLUserTextBox.Text = "sa";
			// 
			// SQLPasswordTextBox
			// 
			this.SQLPasswordTextBox.Enabled = false;
			this.SQLPasswordTextBox.Location = new System.Drawing.Point(96, 96);
			this.SQLPasswordTextBox.Name = "SQLPasswordTextBox";
			this.SQLPasswordTextBox.PasswordChar = '*';
			this.SQLPasswordTextBox.Size = new System.Drawing.Size(224, 20);
			this.SQLPasswordTextBox.TabIndex = 7;
			this.SQLPasswordTextBox.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(32, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 18);
			this.label3.TabIndex = 8;
			this.label3.Text = "&User name:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(32, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 18);
			this.label4.TabIndex = 9;
			this.label4.Text = "&Passsword:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// SQLDatabaseComboBox
			// 
			this.SQLDatabaseComboBox.Enabled = false;
			this.SQLDatabaseComboBox.Location = new System.Drawing.Point(40, 96);
			this.SQLDatabaseComboBox.Name = "SQLDatabaseComboBox";
			this.SQLDatabaseComboBox.Size = new System.Drawing.Size(208, 21);
			this.SQLDatabaseComboBox.TabIndex = 11;
			// 
			// SQLOkButton
			// 
			this.SQLOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.SQLOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.SQLOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLOkButton.Location = new System.Drawing.Point(192, 408);
			this.SQLOkButton.Name = "SQLOkButton";
			this.SQLOkButton.TabIndex = 12;
			this.SQLOkButton.Text = "Ok";
			this.SQLOkButton.Click += new System.EventHandler(this.SQLOkButton_Click);
			// 
			// SQLCancelButton
			// 
			this.SQLCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.SQLCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.SQLCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLCancelButton.Location = new System.Drawing.Point(272, 408);
			this.SQLCancelButton.Name = "SQLCancelButton";
			this.SQLCancelButton.TabIndex = 13;
			this.SQLCancelButton.Text = "Cancel";
			this.SQLCancelButton.Click += new System.EventHandler(this.SQLCancelButton_Click);
			// 
			// SQLTestButton
			// 
			this.SQLTestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.SQLTestButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLTestButton.Location = new System.Drawing.Point(16, 408);
			this.SQLTestButton.Name = "SQLTestButton";
			this.SQLTestButton.Size = new System.Drawing.Size(96, 23);
			this.SQLTestButton.TabIndex = 14;
			this.SQLTestButton.Text = "Test Connection";
			this.SQLTestButton.Click += new System.EventHandler(this.SQLTestButton_Click);
			// 
			// SQLDatabaseTextBox
			// 
			this.SQLDatabaseTextBox.Location = new System.Drawing.Point(40, 48);
			this.SQLDatabaseTextBox.Name = "SQLDatabaseTextBox";
			this.SQLDatabaseTextBox.Size = new System.Drawing.Size(280, 20);
			this.SQLDatabaseTextBox.TabIndex = 16;
			this.SQLDatabaseTextBox.Text = "MOG";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.SQLDatabaseRefreshButton);
			this.groupBox1.Controls.Add(this.SQLSelectDatabaseRadioButton);
			this.groupBox1.Controls.Add(this.SQLCreateDatabaseRadioButton);
			this.groupBox1.Controls.Add(this.SQLDatabaseTextBox);
			this.groupBox1.Controls.Add(this.SQLDatabaseComboBox);
			this.groupBox1.Location = new System.Drawing.Point(16, 264);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(336, 136);
			this.groupBox1.TabIndex = 17;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Connect to a database";
			// 
			// SQLDatabaseRefreshButton
			// 
			this.SQLDatabaseRefreshButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLDatabaseRefreshButton.Location = new System.Drawing.Point(256, 94);
			this.SQLDatabaseRefreshButton.Name = "SQLDatabaseRefreshButton";
			this.SQLDatabaseRefreshButton.Size = new System.Drawing.Size(72, 23);
			this.SQLDatabaseRefreshButton.TabIndex = 20;
			this.SQLDatabaseRefreshButton.Text = "Refresh";
			this.SQLDatabaseRefreshButton.Click += new System.EventHandler(this.SQLDatabaseRefreshButton_Click);
			// 
			// SQLSelectDatabaseRadioButton
			// 
			this.SQLSelectDatabaseRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLSelectDatabaseRadioButton.Location = new System.Drawing.Point(24, 72);
			this.SQLSelectDatabaseRadioButton.Name = "SQLSelectDatabaseRadioButton";
			this.SQLSelectDatabaseRadioButton.Size = new System.Drawing.Size(240, 24);
			this.SQLSelectDatabaseRadioButton.TabIndex = 18;
			this.SQLSelectDatabaseRadioButton.Text = "Select an existing database";
			this.SQLSelectDatabaseRadioButton.CheckedChanged += new System.EventHandler(this.SQLSelectDatabaseRadioButton_CheckedChanged);
			// 
			// SQLCreateDatabaseRadioButton
			// 
			this.SQLCreateDatabaseRadioButton.Checked = true;
			this.SQLCreateDatabaseRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SQLCreateDatabaseRadioButton.Location = new System.Drawing.Point(24, 24);
			this.SQLCreateDatabaseRadioButton.Name = "SQLCreateDatabaseRadioButton";
			this.SQLCreateDatabaseRadioButton.Size = new System.Drawing.Size(240, 24);
			this.SQLCreateDatabaseRadioButton.TabIndex = 17;
			this.SQLCreateDatabaseRadioButton.TabStop = true;
			this.SQLCreateDatabaseRadioButton.Text = "Create new database";
			this.SQLCreateDatabaseRadioButton.CheckedChanged += new System.EventHandler(this.SQLCreateDatabaseRadioButton_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.SQLUserTextBox);
			this.groupBox2.Controls.Add(this.SQLUserRadioButton);
			this.groupBox2.Controls.Add(this.SQLNTSecurityRadioButton);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.SQLPasswordTextBox);
			this.groupBox2.Location = new System.Drawing.Point(16, 120);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(336, 136);
			this.groupBox2.TabIndex = 18;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Log on to the server";
			// 
			// SqlRefreshButton
			// 
			this.SqlRefreshButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.SqlRefreshButton.Location = new System.Drawing.Point(280, 87);
			this.SqlRefreshButton.Name = "SqlRefreshButton";
			this.SqlRefreshButton.Size = new System.Drawing.Size(72, 23);
			this.SqlRefreshButton.TabIndex = 19;
			this.SqlRefreshButton.Text = "Refresh";
			this.SqlRefreshButton.Click += new System.EventHandler(this.SqlRefreshButton_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(344, 32);
			this.label2.TabIndex = 20;
			this.label2.Text = "NOTE:  If you are using SQL Express your server name may be instanced.  I.e. MACH" +
				"INENAME\\SQLExpress";
			// 
			// SQLConnectControl
			// 
			this.Controls.Add(this.label2);
			this.Controls.Add(this.SqlRefreshButton);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.SQLTestButton);
			this.Controls.Add(this.SQLCancelButton);
			this.Controls.Add(this.SQLOkButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label);
			this.Controls.Add(this.SQLServerComboBox);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Name = "SQLConnectControl";
			this.Size = new System.Drawing.Size(360, 440);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
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
		}

		public void InitializeSQLControl()
		{
			packetSize = "4096";				

			// Try and get our current settings
			string connectionString = "";
			if (MOG_ControllerSystem.GetDB() != null && MOG_ControllerSystem.GetDB().GetConnectionString().Length > 0)
			{
				connectionString = MOG_ControllerSystem.GetDB().GetConnectionString();
				LoadConnectionString(connectionString.Trim("@".ToCharArray()));
			}
			else
			{
				// packet size=4096;integrated security=SSPI;data source="NEMESIS";persist security info=False;initial catalog=mog
				security = "SSPI";
				persistSecurity = "False";
				dataSource = "";
				initialCatalog = "mog";
			
				SQLServerComboBox.Text = dataSource;
				SQLDatabaseComboBox.Text =initialCatalog;
			}

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
		private void LoadConnectionString(string connectionString)
		{
			//ConnectionString=Packet size=4096;integrated security=SSPI;data source=KIERWORK\MOGEXPRESS;persist security info=False;initial catalog=MOGtest;
			// Packet size=4096;integrated security=SSPI;data source=NEMESIS;persist security info=False;initial catalog=mog16;
			if (connectionString.ToLower().IndexOf("data source=") != -1  &&  connectionString.ToLower().IndexOf("initial catalog=") != -1)
			{
				string mymachineName = "";
				string mycatalog = "";
				string mysecurity = "";
				string myuser = "";
				string mypassword = "";

				string[] connectionBits = connectionString.ToUpper().Split(";".ToCharArray());
				foreach (string connectionBit in connectionBits)
				{
					if (connectionBit.StartsWith("DATA SOURCE="))
						mymachineName = connectionBit.Substring( connectionBit.IndexOf("=")+1 );
					if (connectionBit.StartsWith("INITIAL CATALOG="))
						mycatalog = connectionBit.Substring( connectionBit.IndexOf("=")+1 );
					if (connectionBit.StartsWith("INTEGRATED SECURITY="))
						mysecurity = connectionBit.Substring( connectionBit.IndexOf("=")+1 );
					if (connectionBit.StartsWith("USER ID="))
						myuser = connectionBit.Substring( connectionBit.IndexOf("=")+1 );
					if (connectionBit.StartsWith("PASSWORD="))
						mypassword = connectionBit.Substring( connectionBit.IndexOf("=")+1 );
				}

				if (mymachineName != ""  &&  mycatalog != "")
				{
					dataSource = mymachineName;
					initialCatalog = mycatalog;

					ServerName = mymachineName;
					this.SQLSelectDatabaseRadioButton.Checked = true;

					DatabaseName = mycatalog;		
				}

				if (mysecurity.Length > 0)
				{
					this.SQLNTSecurityRadioButton.Checked = true;
					security = mysecurity;
				}
				else if (myuser.Length > 0 && mypassword.Length != 0)
				{
					this.SQLUserRadioButton.Checked = true;
					this.SQLUserTextBox.Text = myuser;
					this.SQLPasswordTextBox.Text = mypassword;					
				}
			}
		}

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

		private string GetDatabaseName()
		{			
			if (this.SQLCreateDatabaseRadioButton.Checked)
			{
				return this.SQLDatabaseTextBox.Text;
			}
			else if (this.SQLSelectDatabaseRadioButton.Checked)
			{
				return SQLDatabaseComboBox.Text;
			}
			return "";
		}

		public void BuildConnectionStringNoCatalog()
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
		}

		public void BuildConnectionString()
		{
			BuildConnectionStringNoCatalog();

			mConnectString = mConnectString + "initial catalog=" + GetDatabaseName() + ";";			
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
			try
			{
				string OriginalConnectionString = MOG_ControllerSystem.GetDB() != null ? MOG_ControllerSystem.GetDB().GetConnectionString() : "";

				// Test the connection string
				// Attempt to open a connection to make sure we will work?
				BuildConnectionStringNoCatalog();
				string databaseName = GetDatabaseName();

				
				// Is this a 'Create new' database
				if (this.SQLCreateDatabaseRadioButton.Checked)
				{
					try
					{
						// Test to see if we can get to the server
						if (MOG.DATABASE.MOG_DBAPI.TestServerConnection(mConnectString))
						{
							if (MOG.DATABASE.MOG_DBAPI.TestDatabaseConnection(mConnectString, this.SQLDatabaseTextBox.Text) == false)
							{
								if (MOG_Prompt.PromptResponse("Create new Database?", "We will need to create this database to test this connection.\n\n\tDATABASE NAME: " + this.SQLDatabaseTextBox.Text + "\n\nIs it ok to proceed?", MOGPromptButtons.OKCancel) == MOGPromptResult.OK)
								{						
									if (MOG.DATABASE.MOG_DBAPI.CreateDatabase(mConnectString, this.SQLDatabaseTextBox.Text))
									{
										databaseName = SQLDatabaseTextBox.Text;
									}
								}
								else
								{
									return;
								}
							}
						}
						else
						{
							MOG_Prompt.PromptMessage("Create Database", "Unable to access server: " + this.ServerName);
							return;
						}
					}
					catch(Exception ex)
					{
						MOG_Prompt.PromptMessage("Create Database", "Unable to check or create database with the following message:\n\n" + ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
						return;
					}
				}
					

				// Build the full connection string
				BuildConnectionString();

				try
				{
					if (!MOG_ControllerSystem.InitializeDatabase(mConnectString, "", ""))
					{
						MOG_Prompt.PromptResponse("Connection Failure", "Couldn't connect to SQL database " + databaseName + " on server " + SQLServerComboBox.Text);
						throw new Exception();
					}
					else
					{
						MOG_Prompt.PromptResponse("Connection Successful", "Successfully connected to SQL database " + databaseName + " on server " + SQLServerComboBox.Text);
					}
				}
				catch
				{
				}
				finally
				{
					if (OriginalConnectionString.Length > 0)
					{
						// Restore the original DBbase
						mConnectString = OriginalConnectionString;
						if (!MOG_ControllerSystem.InitializeDatabase(mConnectString, "", ""))
						{
							MOG_Prompt.PromptResponse("Connection Failure", "Couldn't restore to original SQL database (" + mConnectString + ")");					
						}
					}
				}
			}
			catch(Exception outer)
			{
				MOG_Prompt.PromptMessage("Error", outer.Message, outer.StackTrace, MOG_ALERT_LEVEL.ERROR);
				return;
			}
			
			// notify others that we clicked the Test button
			OnTestButtonClicked();
		}

		private void SQLOkButton_Click(object sender, System.EventArgs e)
		{
			// Test the connection string
			// Attempt to open a connection to make sure we will work?
			BuildConnectionStringNoCatalog();
			string databaseName = this.SQLDatabaseComboBox.Text;

			// Is this a 'Create new' database
			if (this.SQLCreateDatabaseRadioButton.Checked)
			{
				try
				{
					if (MOG.DATABASE.MOG_DBAPI.DatabaseExists(mConnectString, this.SQLDatabaseTextBox.Text) == false)
					{
						if (MOG_Prompt.PromptResponse("Create new Database?", "We will need to create this database to test this connection.\n\n\tDATABASE NAME: " + this.SQLDatabaseTextBox.Text + "\n\nIs it ok to proceed?", MOGPromptButtons.OKCancel) == MOGPromptResult.OK)
						{						
							if (MOG.DATABASE.MOG_DBAPI.CreateDatabase(mConnectString, this.SQLDatabaseTextBox.Text))
							{
								databaseName = SQLDatabaseTextBox.Text;
							}
							else
							{
								if (MOG_Prompt.PromptResponse("Create Database", "Unable to create database! \n\nDo you want to continue and save these new settings?", MOGPromptButtons.YesNo) == MOGPromptResult.No)
								{
									return;
								}
							}
						}
						else
						{
							return;
						}
					}
				}
				catch(Exception ex)
				{
					MOG_Prompt.PromptMessage("Create Database", "Unable to check or create database with the following message:\n\n" + ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
					return;
				}
			}

			BuildConnectionString();

			if (!MOG_ControllerSystem.InitializeDatabase(mConnectString, "", ""))
			{
				MOG_Prompt.PromptResponse("Connection Failure", "Couldn't connect to SQL database " + databaseName + " on server " + SQLServerComboBox.Text);
				return;
			}

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

		private void PopulateServerNames()
		{
			if (this.refreshServerListOnDropDown)
			{
				this.Cursor = Cursors.WaitCursor;
				Application.DoEvents();

				ArrayList servers = SqlServerLocator.FindServers();
				SQLServerComboBox.Items.Clear();

				if (servers != null)
				{
					foreach (string server in servers)
					{
						if (server.ToLower() == "(local)")
							SQLServerComboBox.Items.Add( Environment.MachineName.ToUpper() );
						else
							SQLServerComboBox.Items.Add(server);
					}

					SQLServerComboBox.SelectedItem = SQLServerComboBox.Items[0];
				}

				this.Cursor = Cursors.Default;
				this.refreshServerListOnDropDown = false;
			}
		}

		private void PopulateDatabaseNamesList()
		{
			if (this.SQLServerComboBox.Text != null  &&  this.SQLServerComboBox.Text.Length > 0)
			{
				this.SQLDatabaseComboBox.Enabled = false;
				this.SQLDatabaseComboBox.SelectedItem = null;
				this.SQLDatabaseComboBox.Text = "Querying available databases...";
				this.SQLDatabaseComboBox.Items.Clear();
				Application.DoEvents();
				Cursor.Current = Cursors.WaitCursor;

				string serverName = this.SQLServerComboBox.Text as string;

				foreach(string databaseName in SqlServerLocator.FindDatabases(serverName))
				{
					// Do not allow any of these specified databases
					if (string.Compare(databaseName, "master", true) != 0 &&
						string.Compare(databaseName, "msdb", true) != 0 &&
						string.Compare(databaseName, "tempdb", true) != 0 &&
						string.Compare(databaseName, "model", true) != 0)
					{
						this.SQLDatabaseComboBox.Items.Add(databaseName.ToUpper());
					}
				}

				// Set the initial database
				if (this.SQLDatabaseComboBox.Items.Count > 0)					
				{
					// Do we have a pre loaded database name?
					if (this.initialCatalog != null &&	this.initialCatalog.Length >0 && this.SQLDatabaseComboBox.Items.Contains(this.initialCatalog))					
					{
						// Yes, set it
						this.DatabaseName = this.initialCatalog;
					}
					else
					{
						// No, then set the first item
						this.SQLDatabaseComboBox.SelectedItem = this.SQLDatabaseComboBox.Items[0];
					}
				}
				else
				{
					// No DB's returned, clear this out then
					this.DatabaseName = "";
				}
				

				if (this.SQLSelectDatabaseRadioButton.Checked)
				{
					this.SQLDatabaseComboBox.Enabled = true;
				}
				Cursor.Current = Cursors.Default;
			}
		}

		private void SQLServerComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//PopulateDatabaseNamesList();
		}

		private void SQLCreateDatabaseRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			this.SQLDatabaseTextBox.Enabled = SQLCreateDatabaseRadioButton.Checked;
		}

		private void SQLSelectDatabaseRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			PopulateDatabaseNamesList();
			this.SQLDatabaseComboBox.Enabled = SQLSelectDatabaseRadioButton.Checked;
		}

		private void SqlRefreshButton_Click(object sender, System.EventArgs e)
		{
			PopulateServerNames();
		}

		private void SQLDatabaseRefreshButton_Click(object sender, System.EventArgs e)
		{
			PopulateDatabaseNamesList();
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


