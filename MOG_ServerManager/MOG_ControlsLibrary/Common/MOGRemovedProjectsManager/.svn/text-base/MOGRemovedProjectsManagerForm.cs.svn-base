using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for MOGRemovedProjectsManagerForm.
	/// </summary>
	public class MOGRemovedProjectsManagerForm : System.Windows.Forms.Form
	{
		private MOGRemovedProjectsManager mogRemovedProjectsManager1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MOGRemovedProjectsManagerForm(string ini, string mogProjectsPath)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			this.mogRemovedProjectsManager1.Init(ini, mogProjectsPath);
			this.mogRemovedProjectsManager1.CloseClicked += new EventHandler(CloseClickedEventHandler);
		}

		private void CloseClickedEventHandler(object sender, EventArgs e)
		{
			Dispose();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MOGRemovedProjectsManagerForm));
			this.mogRemovedProjectsManager1 = new MOGRemovedProjectsManager();
			this.SuspendLayout();
			// 
			// mogRemovedProjectsManager1
			// 
			this.mogRemovedProjectsManager1.CloseButtonVisible = true;
			this.mogRemovedProjectsManager1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mogRemovedProjectsManager1.InfoBoxVisible = true;
			this.mogRemovedProjectsManager1.Location = new System.Drawing.Point(0, 0);
			this.mogRemovedProjectsManager1.Name = "mogRemovedProjectsManager1";
			this.mogRemovedProjectsManager1.Size = new System.Drawing.Size(520, 230);
			this.mogRemovedProjectsManager1.TabIndex = 0;
			this.mogRemovedProjectsManager1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mogRemovedProjectsManager1_KeyDown);
			// 
			// MOGRemovedProjectsManagerForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(520, 230);
			this.Controls.Add(this.mogRemovedProjectsManager1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MOGRemovedProjectsManagerForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Removed Projects Manager";
			this.ResumeLayout(false);

		}
		#endregion

		private void mogRemovedProjectsManager1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape)
				Dispose();
		}

	}
}
