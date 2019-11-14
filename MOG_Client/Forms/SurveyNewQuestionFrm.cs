using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Client
{
	/// <summary>
	/// Summary description for SurveyNewQuestionFrm.
	/// </summary>
	public class SurveyNewQuestionFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox SurveyAnswerTextBox;
		private System.Windows.Forms.TextBox SurveyQuestiontextBox;
		private System.Windows.Forms.ListBox SurveyAnswerListBox;
		private System.Windows.Forms.Button SurveyAddButton;
		private System.Windows.Forms.Button OKbutton;
		private System.Windows.Forms.Button Cancelbutton;
		private System.Windows.Forms.Button UPbutton;
		private System.Windows.Forms.Button Downbutton;
		private SurveyForm mSurveyFrm;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SurveyNewQuestionFrm(SurveyForm sfrm)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			mSurveyFrm = sfrm;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SurveyNewQuestionFrm));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SurveyAddButton = new System.Windows.Forms.Button();
			this.SurveyAnswerTextBox = new System.Windows.Forms.TextBox();
			this.SurveyQuestiontextBox = new System.Windows.Forms.TextBox();
			this.SurveyAnswerListBox = new System.Windows.Forms.ListBox();
			this.OKbutton = new System.Windows.Forms.Button();
			this.Cancelbutton = new System.Windows.Forms.Button();
			this.UPbutton = new System.Windows.Forms.Button();
			this.Downbutton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Question:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 23);
			this.label2.TabIndex = 10;
			this.label2.Text = "Answer:";
			// 
			// SurveyAddButton
			// 
			this.SurveyAddButton.Location = new System.Drawing.Point(184, 96);
			this.SurveyAddButton.Name = "SurveyAddButton";
			this.SurveyAddButton.Size = new System.Drawing.Size(48, 23);
			this.SurveyAddButton.TabIndex = 3;
			this.SurveyAddButton.Text = "Add ->";
			this.SurveyAddButton.Click += new System.EventHandler(this.SurveyAddButton_Click);
			// 
			// SurveyAnswerTextBox
			// 
			this.SurveyAnswerTextBox.Location = new System.Drawing.Point(72, 64);
			this.SurveyAnswerTextBox.Name = "SurveyAnswerTextBox";
			this.SurveyAnswerTextBox.Size = new System.Drawing.Size(160, 20);
			this.SurveyAnswerTextBox.TabIndex = 2;
			this.SurveyAnswerTextBox.Text = "";
			this.SurveyAnswerTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SurveyAnswerTextBox_KeyDown);
			// 
			// SurveyQuestiontextBox
			// 
			this.SurveyQuestiontextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.SurveyQuestiontextBox.Location = new System.Drawing.Point(72, 16);
			this.SurveyQuestiontextBox.Name = "SurveyQuestiontextBox";
			this.SurveyQuestiontextBox.Size = new System.Drawing.Size(376, 20);
			this.SurveyQuestiontextBox.TabIndex = 1;
			this.SurveyQuestiontextBox.Text = "";
			// 
			// SurveyAnswerListBox
			// 
			this.SurveyAnswerListBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.SurveyAnswerListBox.Location = new System.Drawing.Point(248, 56);
			this.SurveyAnswerListBox.Name = "SurveyAnswerListBox";
			this.SurveyAnswerListBox.Size = new System.Drawing.Size(192, 121);
			this.SurveyAnswerListBox.TabIndex = 5;
			// 
			// OKbutton
			// 
			this.OKbutton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.OKbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKbutton.Location = new System.Drawing.Point(16, 160);
			this.OKbutton.Name = "OKbutton";
			this.OKbutton.Size = new System.Drawing.Size(40, 23);
			this.OKbutton.TabIndex = 6;
			this.OKbutton.Text = "OK";
			this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
			// 
			// Cancelbutton
			// 
			this.Cancelbutton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancelbutton.Location = new System.Drawing.Point(72, 160);
			this.Cancelbutton.Name = "Cancelbutton";
			this.Cancelbutton.TabIndex = 7;
			this.Cancelbutton.Text = "Cancel";
			// 
			// UPbutton
			// 
			this.UPbutton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.UPbutton.Image = ((System.Drawing.Bitmap)(resources.GetObject("UPbutton.Image")));
			this.UPbutton.Location = new System.Drawing.Point(440, 80);
			this.UPbutton.Name = "UPbutton";
			this.UPbutton.Size = new System.Drawing.Size(16, 23);
			this.UPbutton.TabIndex = 8;
			this.UPbutton.Click += new System.EventHandler(this.UPbutton_Click);
			// 
			// Downbutton
			// 
			this.Downbutton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.Downbutton.Image = ((System.Drawing.Bitmap)(resources.GetObject("Downbutton.Image")));
			this.Downbutton.Location = new System.Drawing.Point(440, 104);
			this.Downbutton.Name = "Downbutton";
			this.Downbutton.Size = new System.Drawing.Size(16, 23);
			this.Downbutton.TabIndex = 9;
			this.Downbutton.Click += new System.EventHandler(this.Downbutton_Click);
			// 
			// SurveyNewQuestionFrm
			// 
			this.AcceptButton = this.SurveyAddButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.Cancelbutton;
			this.ClientSize = new System.Drawing.Size(472, 190);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.Downbutton,
																		  this.UPbutton,
																		  this.Cancelbutton,
																		  this.OKbutton,
																		  this.SurveyAnswerListBox,
																		  this.SurveyQuestiontextBox,
																		  this.SurveyAnswerTextBox,
																		  this.SurveyAddButton,
																		  this.label2,
																		  this.label1});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SurveyNewQuestionFrm";
			this.ResumeLayout(false);

		}
		#endregion

		private void SurveyAddButton_Click(object sender, System.EventArgs e)
		{
			SurveyAnswerListBox.Items.Add(SurveyAnswerTextBox.Text);
			SurveyAnswerTextBox.Clear();
		}

		private void UPbutton_Click(object sender, System.EventArgs e)
		{
			int tmpi=SurveyAnswerListBox.SelectedIndex;
			if(tmpi>0)
			{
				SurveyAnswerListBox.Items.Insert(tmpi-1,SurveyAnswerListBox.Items[tmpi]);
                SurveyAnswerListBox.Items.RemoveAt(tmpi+1);
				SurveyAnswerListBox.SelectedIndex=tmpi-1;
			}
		}

		private void Downbutton_Click(object sender, System.EventArgs e)
		{
			int tmpi=SurveyAnswerListBox.SelectedIndex+1;
			if(tmpi<SurveyAnswerListBox.Items.Count)
			{
				SurveyAnswerListBox.Items.Insert(tmpi-1,SurveyAnswerListBox.Items[tmpi]);
				SurveyAnswerListBox.Items.RemoveAt(tmpi+1);
				SurveyAnswerListBox.SelectedIndex=tmpi;
			}

		
		}

		private void OKbutton_Click(object sender, System.EventArgs e)
		{
			string question;
			question = SurveyQuestiontextBox.Text+":";
			for(int i=0;i<SurveyAnswerListBox.Items.Count;i++)
			{
                if(i!=0)
					question += ",";
				question +=SurveyAnswerListBox.Items[i].ToString();
			}
			mSurveyFrm.SurveySetQuestion(question);
			Close();
		}

		private void SurveyAnswerTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			/*if(e.KeyCode==Keys.Enter)
			{
				OKbutton_Click(sender, null);
			}*/
		}

	
	}
}
