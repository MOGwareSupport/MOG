using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_ControlsLibrary.Forms
{
	/// <summary>
	/// Summary description for SQLConnectForm.
	/// </summary>
	public class SQLConnectForm : System.Windows.Forms.Form
	{
		#region System definitions
		private SQLConnectControl sqlConnectControl;

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
			this.sqlConnectControl = new SQLConnectControl();
			this.SuspendLayout();
			// 
			// sqlConnectControl
			// 
			this.sqlConnectControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.sqlConnectControl.ButtonsVisible = true;
			this.sqlConnectControl.Cursor = System.Windows.Forms.Cursors.Default;
			this.sqlConnectControl.DataSource = "";
			this.sqlConnectControl.Location = new System.Drawing.Point(0, 0);
			this.sqlConnectControl.Name = "sqlConnectControl";
			this.sqlConnectControl.OKButtonVisible = true;
			this.sqlConnectControl.Size = new System.Drawing.Size(360, 456);
			this.sqlConnectControl.SQLCancelButtonVisible = true;
			this.sqlConnectControl.SQLTestButtonVisible = true;
			this.sqlConnectControl.TabIndex = 0;
			// 
			// SQLConnectForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(360, 457);
			this.Controls.Add(this.sqlConnectControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SQLConnectForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
		public void BuildConnectionString()
		{
			this.sqlConnectControl.BuildConnectionString();
		}
		#endregion
		#region Member vars
		#endregion
		#region Constructors
		public SQLConnectForm()
		{
			InitializeComponent();

			this.sqlConnectControl.InitializeSQLControl();
		}
		#endregion
		#region Member functions
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