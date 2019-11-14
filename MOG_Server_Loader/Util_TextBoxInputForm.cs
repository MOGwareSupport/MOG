using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server_Loader
{
	/// <summary>
	/// Summary description for Util_TextBoxInputForm.
	/// </summary>
	public class Util_TextBoxInputForm : System.Windows.Forms.Form
	{
		#region System defs
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox tbInput;
		private System.Windows.Forms.Label lblInstructions;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Util_TextBoxInputForm));
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.tbInput = new System.Windows.Forms.TextBox();
			this.lblInstructions = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(272, 48);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(272, 72);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 10;
			this.btnCancel.Text = "Cancel";
			// 
			// tbInput
			// 
			this.tbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbInput.Location = new System.Drawing.Point(8, 48);
			this.tbInput.Name = "tbInput";
			this.tbInput.Size = new System.Drawing.Size(248, 20);
			this.tbInput.TabIndex = 0;
			this.tbInput.Text = "";
			// 
			// lblInstructions
			// 
			this.lblInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblInstructions.Location = new System.Drawing.Point(16, 8);
			this.lblInstructions.Name = "lblInstructions";
			this.lblInstructions.Size = new System.Drawing.Size(328, 24);
			this.lblInstructions.TabIndex = 15;
			this.lblInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// Util_TextBoxInputForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(352, 110);
			this.Controls.Add(this.lblInstructions);
			this.Controls.Add(this.tbInput);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Util_TextBoxInputForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Input";
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Properties
		public string OKButtonText
		{
			get { return this.btnOK.Text; }
			set { this.btnOK.Text = value; }
		}

		public string CancelButtonText
		{
			get { return this.btnCancel.Text; }
			set { this.btnCancel.Text = value; }
		}

		public string InstructionsText
		{
			get { return this.lblInstructions.Text; }
			set { this.lblInstructions.Text = value; }
		}

		public string Title
		{
			get { return this.Text; }
			set { this.Text = value; }
		}

		public string InputText
		{
			get { return this.tbInput.Text; }
			set { this.tbInput.Text = value; }
		}
		#endregion
		#region Constructors
		public Util_TextBoxInputForm()
		{
			InitializeComponent();
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			Hide();
		}

		public Util_TextBoxInputForm(string title, string instructions)
		{
			InitializeComponent();

			this.Text = title;
			this.lblInstructions.Text = instructions;
		}
		#endregion
	}
}
