using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MOG_Client.Client_Gui;

namespace MOG_Client
{
	/// <summary>
	/// Summary description for SurveyVote.
	/// </summary>
	public class SurveyVote : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label SurveyVoteLabel;
		private System.Windows.Forms.ListBox SurveyVoteListBox;
		private System.Windows.Forms.TextBox SurveyReplyTextBox;
		private System.Windows.Forms.Button SurveyReplybutton;
		private guiSurveyManager mSrvyMngr;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SurveyVote(guiSurveyManager main)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mSrvyMngr = main;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.SurveyVoteLabel = new System.Windows.Forms.Label();
			this.SurveyVoteListBox = new System.Windows.Forms.ListBox();
			this.SurveyReplyTextBox = new System.Windows.Forms.TextBox();
			this.SurveyReplybutton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// SurveyVoteLabel
			// 
			this.SurveyVoteLabel.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.SurveyVoteLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.SurveyVoteLabel.Location = new System.Drawing.Point(8, 16);
			this.SurveyVoteLabel.Name = "SurveyVoteLabel";
			this.SurveyVoteLabel.Size = new System.Drawing.Size(264, 23);
			this.SurveyVoteLabel.TabIndex = 0;
			this.SurveyVoteLabel.Text = "New Survey for you";
			// 
			// SurveyVoteListBox
			// 
			this.SurveyVoteListBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.SurveyVoteListBox.Location = new System.Drawing.Point(8, 72);
			this.SurveyVoteListBox.Name = "SurveyVoteListBox";
			this.SurveyVoteListBox.Size = new System.Drawing.Size(272, 95);
			this.SurveyVoteListBox.TabIndex = 1;
			// 
			// SurveyReplyTextBox
			// 
			this.SurveyReplyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.SurveyReplyTextBox.Location = new System.Drawing.Point(8, 192);
			this.SurveyReplyTextBox.Name = "SurveyReplyTextBox";
			this.SurveyReplyTextBox.Size = new System.Drawing.Size(272, 20);
			this.SurveyReplyTextBox.TabIndex = 2;
			this.SurveyReplyTextBox.Text = "textBox1";
			// 
			// SurveyReplybutton
			// 
			this.SurveyReplybutton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.SurveyReplybutton.Location = new System.Drawing.Point(8, 224);
			this.SurveyReplybutton.Name = "SurveyReplybutton";
			this.SurveyReplybutton.TabIndex = 3;
			this.SurveyReplybutton.Text = "Vote";
			this.SurveyReplybutton.Click += new System.EventHandler(this.SurveyReplybutton_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 51);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Select Item below and vote";
			// 
			// label2
			// 
			this.label2.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.label2.Location = new System.Drawing.Point(16, 176);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Your comment:";
			// 
			// SurveyVote
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label2,
																		  this.label1,
																		  this.SurveyReplybutton,
																		  this.SurveyReplyTextBox,
																		  this.SurveyVoteListBox,
																		  this.SurveyVoteLabel});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SurveyVote";
			this.Text = "New Survey";
			this.ResumeLayout(false);

		}
		#endregion

		private void SurveyReplybutton_Click(object sender, System.EventArgs e)
		{
			mSrvyMngr.Vote(SurveyVoteListBox.SelectedIndex,SurveyReplyTextBox.Text);
		}
	}
}
