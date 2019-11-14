using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server.Server_Gui.guiConfigurationsHelpers
{
	public enum OneLineInputSize { ExtraLarge = 1000, Large = 500, Medium = 250, Small = 100, ExtraSmall = 50 }
	public class OneLineInputForm : System.Windows.Forms.Form
	{
		#region System definitions
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label OLILabel;
		private System.Windows.Forms.TextBox OLITextBox;

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
			this.OLITextBox = new System.Windows.Forms.TextBox();
			this.OLILabel = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// OLITextBox
			// 
			this.OLITextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.OLITextBox.Location = new System.Drawing.Point(32, 44);
			this.OLITextBox.Name = "OLITextBox";
			this.OLITextBox.Size = new System.Drawing.Size(416, 20);
			this.OLITextBox.TabIndex = 0;
			this.OLITextBox.Text = "";
			// 
			// OLILabel
			// 
			this.OLILabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.OLILabel.Location = new System.Drawing.Point(32, 28);
			this.OLILabel.Name = "OLILabel";
			this.OLILabel.Size = new System.Drawing.Size(416, 16);
			this.OLILabel.TabIndex = 1;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnOK.Location = new System.Drawing.Point(360, 80);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(272, 80);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			// 
			// OneLineInputForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(480, 134);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.OLILabel);
			this.Controls.Add(this.OLITextBox);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(1176, 168);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(256, 168);
			this.Name = "OneLineInputForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "One-line input";
			this.ResumeLayout(false);

		}
		#endregion

		#endregion

		#region User definitions
		public string InputLine { get{ return OLITextBox.Text; } }
		#endregion

		#region Constructors
		public OneLineInputForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}
		public OneLineInputForm(string TitleName, string LabelName)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.Text = TitleName;
			this.OLILabel.Text = LabelName;
		}
		public OneLineInputForm(string TitleName, string LabelName, OneLineInputSize size)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.Text = TitleName;
			this.OLILabel.Text = LabelName;
			this.Size = new Size((int)size, this.Size.Height);
		}
		#endregion

		#region Static members
		public static string ShowMedium(string title, string label) 
		{
			OneLineInputForm olif = new OneLineInputForm(title, label, OneLineInputSize.Medium);
			if (olif.ShowDialog() == DialogResult.OK) 
			{
				return olif.InputLine;
			}
			else 
			{
				return null;
			}
		}
		#endregion

		#region Events
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		#endregion

	}
}

