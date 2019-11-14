using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Post
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class WhatsNewLogForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button WhatsNewOKButton;
		public RichTextBoxExtended.RichTextBoxExtended WhatsNewRichTextBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public WhatsNewLogForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
			this.WhatsNewOKButton = new System.Windows.Forms.Button();
			this.WhatsNewRichTextBox = new RichTextBoxExtended.RichTextBoxExtended();
			this.SuspendLayout();
			// 
			// WhatsNewOKButton
			// 
			this.WhatsNewOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.WhatsNewOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.WhatsNewOKButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.WhatsNewOKButton.Location = new System.Drawing.Point(256, 320);
			this.WhatsNewOKButton.Name = "WhatsNewOKButton";
			this.WhatsNewOKButton.Size = new System.Drawing.Size(83, 23);
			this.WhatsNewOKButton.TabIndex = 1;
			this.WhatsNewOKButton.Text = "Ok";
			// 
			// WhatsNewRichTextBox
			// 
			this.WhatsNewRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.WhatsNewRichTextBox.DetectURLs = true;
			this.WhatsNewRichTextBox.Location = new System.Drawing.Point(0, 0);
			this.WhatsNewRichTextBox.Name = "WhatsNewRichTextBox";
			this.WhatsNewRichTextBox.ShowBold = true;
			this.WhatsNewRichTextBox.ShowCenterJustify = true;
			this.WhatsNewRichTextBox.ShowColors = true;
			this.WhatsNewRichTextBox.ShowFont = true;
			this.WhatsNewRichTextBox.ShowFontSize = true;
			this.WhatsNewRichTextBox.ShowItalic = true;
			this.WhatsNewRichTextBox.ShowLeftJustify = true;
			this.WhatsNewRichTextBox.ShowOpen = false;
			this.WhatsNewRichTextBox.ShowRedo = true;
			this.WhatsNewRichTextBox.ShowRightJustify = true;
			this.WhatsNewRichTextBox.ShowSave = false;
			this.WhatsNewRichTextBox.ShowStamp = true;
			this.WhatsNewRichTextBox.ShowStrikeout = true;
			this.WhatsNewRichTextBox.ShowUnderline = true;
			this.WhatsNewRichTextBox.ShowUndo = true;
			this.WhatsNewRichTextBox.Size = new System.Drawing.Size(608, 312);
			this.WhatsNewRichTextBox.StampAction = RichTextBoxExtended.StampActions.EditedBy;
			this.WhatsNewRichTextBox.StampColor = System.Drawing.Color.Blue;
			this.WhatsNewRichTextBox.TabIndex = 2;
			// 
			// WhatsNewLogForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(608, 345);
			this.Controls.Add(this.WhatsNewRichTextBox);
			this.Controls.Add(this.WhatsNewOKButton);
			this.Name = "WhatsNewLogForm";
			this.Text = "What\'s New";
			this.Activated += new System.EventHandler(this.WhatsNewLogForm_Activated);
			this.ResumeLayout(false);

		}
		#endregion

		private void WhatsNewLogForm_Activated(object sender, System.EventArgs e)
		{
			TopMost = false;
		}
	}
}
