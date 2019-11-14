using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

using MOG.COMMAND;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for NotifyEventForm.
	/// </summary>
	public class NotifyEventForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button CancelButton1;

		public delegate void AddCommandDelegate( MOG_Command command );

		private AddCommandDelegate mAddDelegate;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
	
		public AddCommandDelegate AddDelegate
		{
			get
			{
				return this.mAddDelegate;
			}
		}

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NotifyEventForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.mAddDelegate = new AddCommandDelegate( this.AddCommand );
		}

		/// <summary>
		/// Add a MOG_Command to the NotifyEventForm
		/// </summary>
		/// <param name="command"></param>
		public void AddCommand( MOG_Command command )
		{
			string commandStr = "";
			switch( command.GetCommandType() )
			{
				case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemAlert:
					commandStr = "ALERT";
					break;
				case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemError:
					commandStr = "ERROR";
					break;
				case MOG_COMMAND_TYPE.MOG_COMMAND_NotifySystemException:
					commandStr = "EXCEPTION";
					break;
				default:
					commandStr = command.ToString();
					break;
			}

			ListViewItem item = new ListViewItem( commandStr );
			item.SubItems.Add( command.GetDescription() );
			item.SubItems.Add( MOG.TIME.MOG_Time.FormatTimestamp(command.GetCommandTimeStamp(), "" ) );
			this.listView1.Items.Add( item );
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(NotifyEventForm));
			this.CancelButton1 = new System.Windows.Forms.Button();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// CancelButton1
			// 
			this.CancelButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CancelButton1.Location = new System.Drawing.Point(512, 136);
			this.CancelButton1.Name = "CancelButton1";
			this.CancelButton1.TabIndex = 1;
			this.CancelButton1.Text = "Close";
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader3});
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.Location = new System.Drawing.Point(16, 16);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(576, 112);
			this.listView1.TabIndex = 2;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Command";
			this.columnHeader1.Width = 78;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Description";
			this.columnHeader2.Width = 370;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Time";
			this.columnHeader3.Width = 139;
			// 
			// NotifyEventForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.CancelButton1;
			this.ClientSize = new System.Drawing.Size(600, 172);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.CancelButton1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NotifyEventForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "NotifyEventForm";
			this.ResumeLayout(false);

		}
		#endregion

		private void OKButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	} // end class
} // end ns
