using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server.Server_Gui
{
	/// <summary>
	/// Summary description for SQLConnectForm.
	/// </summary>
	public class SQLConnectForm : System.Windows.Forms.Form
	{
		#region System definitions
		private MOG_Server.Server_Gui.SQLConnectControl sqlConnectControl;

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SQLConnectForm));
			this.sqlConnectControl = new MOG_Server.Server_Gui.SQLConnectControl();
			this.SuspendLayout();
			// 
			// sqlConnectControl
			// 
			this.sqlConnectControl.ButtonsVisible = true;
			this.sqlConnectControl.Cursor = System.Windows.Forms.Cursors.Default;
			this.sqlConnectControl.DatabaseName = "mog16";
			this.sqlConnectControl.DataSource = "NEMESIS";
			this.sqlConnectControl.InitialCatalog = "mog";
			this.sqlConnectControl.Location = new System.Drawing.Point(0, 0);
			this.sqlConnectControl.Name = "sqlConnectControl";
			this.sqlConnectControl.OKButtonVisible = true;
			this.sqlConnectControl.ServerName = "NEMESIS";
			this.sqlConnectControl.Size = new System.Drawing.Size(312, 328);
			this.sqlConnectControl.SQLCancelButtonVisible = true;
			this.sqlConnectControl.SQLTestButtonVisible = true;
			this.sqlConnectControl.TabIndex = 0;
			this.sqlConnectControl.Load += new System.EventHandler(this.sqlConnectControl_Load);
			// 
			// newSQLConnectForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(312, 326);
			this.Controls.Add(this.sqlConnectControl);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "newSQLConnectForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create SQL Connection";
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Properties
		public string ConnectionString
		{
			get { return this.sqlConnectControl.mConnectString; }
		}
		#endregion
		#region Member vars
		#endregion
		#region Constructors
		public SQLConnectForm()
		{
			InitializeComponent();
		}
		#endregion
		#region Member functions
		public void LoadConnectionString(string connectionString)
		{
			// Packet size=4096;integrated security=SSPI;data source=NEMESIS;persist security info=False;initial catalog=mog16;
			if (connectionString.ToLower().IndexOf("data source=") != -1  &&  connectionString.ToLower().IndexOf("initial catalog=") != -1)
			{
				string machineName = "";
				string catalog = "";

				string[] connectionBits = connectionString.ToLower().Split(";".ToCharArray());
				foreach (string connectionBit in connectionBits)
				{
					if (connectionBit.StartsWith("data source="))
						machineName = connectionBit.Substring( connectionBit.IndexOf("=")+1 );
					if (connectionBit.StartsWith("initial catalog="))
						catalog = connectionBit.Substring( connectionBit.IndexOf("=")+1 );
				}

				if (machineName != ""  &&  catalog != "")
				{
					this.sqlConnectControl.mConnectString = connectionString;
					this.sqlConnectControl.ServerName = machineName;
					this.sqlConnectControl.DatabaseName = catalog;
				}
			}
		}
		#endregion
		#region Event handlers
		private void OKClickedHandler(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			Dispose();
		}

		private void CancelClickedHandler(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Dispose();
		}

		private void sqlConnectControl_Load(object sender, System.EventArgs e)
		{
			
		}
	}
		#endregion

}


/*
#region Member vars
#endregion
#region System definitions
#endregion
#region Constructors
#endregion
#region Member functions
#endregion
#region Event handlers
#endregion
*/