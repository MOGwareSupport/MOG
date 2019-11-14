using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;

using MOG;
using MOG.INI;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.COMMAND;
using MOG.REPORT;
using MOG.PROMPT;

namespace MOG_ControlsLibrary.Common.MogControl_PrivilegesForm
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MogControl_PrivilegesForm : System.Windows.Forms.Form
	{
		//private const int OptionsPanel_MinWidth = 280;
		//private int OldWidth;
		private MOG_Privileges mPrivileges;
		
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Panel MainStatusBarPanel;
		private System.Windows.Forms.StatusBar MainStatusBar;
		private MogControl_Privileges PrivilegesControl;
		private Panel ButtonsPanel;
		private Button CancelButton1;
		private Button OKButton;
		private System.ComponentModel.IContainer components;
		
		/// <summary>
		/// Create a new Privileges Change Form, given our MOG_Privileges
		/// </summary>
		/// <param name="privileges">Privileges from which to populate our MainTreeView</param>
		public MogControl_PrivilegesForm(MOG_Privileges privileges)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mPrivileges = privileges;
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
			// MOG_Main.Shutdown( true );
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MogControl_PrivilegesForm));
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.MainStatusBarPanel = new System.Windows.Forms.Panel();
			this.MainStatusBar = new System.Windows.Forms.StatusBar();
			this.ButtonsPanel = new System.Windows.Forms.Panel();
			this.CancelButton1 = new System.Windows.Forms.Button();
			this.OKButton = new System.Windows.Forms.Button();
			this.PrivilegesControl = new MOG_ControlsLibrary.Common.MogControl_PrivilegesForm.MogControl_Privileges();
			this.MainStatusBarPanel.SuspendLayout();
			this.ButtonsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// MainStatusBarPanel
			// 
			this.MainStatusBarPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.MainStatusBarPanel.Controls.Add(this.MainStatusBar);
			this.MainStatusBarPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.MainStatusBarPanel.Location = new System.Drawing.Point(0, 548);
			this.MainStatusBarPanel.Name = "MainStatusBarPanel";
			this.MainStatusBarPanel.Size = new System.Drawing.Size(776, 24);
			this.MainStatusBarPanel.TabIndex = 3;
			// 
			// MainStatusBar
			// 
			this.MainStatusBar.Location = new System.Drawing.Point(0, -2);
			this.MainStatusBar.Name = "MainStatusBar";
			this.MainStatusBar.Size = new System.Drawing.Size(772, 22);
			this.MainStatusBar.TabIndex = 0;
			// 
			// ButtonsPanel
			// 
			this.ButtonsPanel.Controls.Add(this.CancelButton1);
			this.ButtonsPanel.Controls.Add(this.OKButton);
			this.ButtonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ButtonsPanel.Location = new System.Drawing.Point(0, 508);
			this.ButtonsPanel.Name = "ButtonsPanel";
			this.ButtonsPanel.Size = new System.Drawing.Size(776, 40);
			this.ButtonsPanel.TabIndex = 5;
			// 
			// CancelButton1
			// 
			this.CancelButton1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.CancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CancelButton1.Location = new System.Drawing.Point(395, 9);
			this.CancelButton1.Name = "CancelButton1";
			this.CancelButton1.Size = new System.Drawing.Size(75, 23);
			this.CancelButton1.TabIndex = 3;
			this.CancelButton1.Text = "Cancel";
			// 
			// OKButton
			// 
			this.OKButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.OKButton.Location = new System.Drawing.Point(307, 9);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(75, 23);
			this.OKButton.TabIndex = 2;
			this.OKButton.Text = "OK";
			// 
			// PrivilegesControl
			// 
			this.PrivilegesControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PrivilegesControl.Location = new System.Drawing.Point(0, 0);
			this.PrivilegesControl.Name = "PrivilegesControl";
			this.PrivilegesControl.Size = new System.Drawing.Size(776, 508);
			this.PrivilegesControl.TabIndex = 4;
			// 
			// MogControl_PrivilegesForm
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.CancelButton1;
			this.ClientSize = new System.Drawing.Size(776, 572);
			this.Controls.Add(this.PrivilegesControl);
			this.Controls.Add(this.ButtonsPanel);
			this.Controls.Add(this.MainStatusBarPanel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(528, 256);
			this.Name = "MogControl_PrivilegesForm";
			this.Text = "Change MOG Group Privileges";
			this.Load += new System.EventHandler(this.MogControl_PrivilegesForm_Load);
			this.MainStatusBarPanel.ResumeLayout(false);
			this.ButtonsPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void MogControl_PrivilegesForm_Load(object sender, EventArgs e)
		{
			PrivilegesControl.Initialize_Control(mPrivileges);
		}


	} // end class
}// end namespace
