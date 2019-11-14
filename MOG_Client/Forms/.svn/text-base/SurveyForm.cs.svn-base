using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MOG_Client.Client_Gui;
using MOG.TIME;


namespace MOG_Client
{
	/// <summary>
	/// Summary description for SurveyForm.
	/// </summary>
	public class SurveyForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button SurveySendbutton;
		private System.Windows.Forms.Button SurveyCancelButton;
		private guiSurveyManager mSrvyMngr;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		public System.Windows.Forms.TextBox SurveyNameTextBox;
		public System.Windows.Forms.TextBox SurveyDescTextBox;
		private System.Windows.Forms.Label label6;
		public System.Windows.Forms.ComboBox SurveyUserGroupcomboBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.ComboBox SurveyCatComboBox;
		private System.Windows.Forms.Button SurveyAddQuestionButton;
		public System.Windows.Forms.ListView SurveyPreListView;
		private System.Windows.Forms.ColumnHeader Question;
		private System.Windows.Forms.ColumnHeader Answers;
		private System.Windows.Forms.Button SurveyRemoveQbutton;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label7;
		public System.Windows.Forms.DateTimePicker SurveyExpireDateTimePicker;
		public System.Windows.Forms.ComboBox SurveyPriorityComboBox;
		

		public SurveyForm(guiSurveyManager main)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			mSrvyMngr = main;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			SurveyExpireDateTimePicker.CustomFormat = "hh:mm:ss MMMM dd, yyyy";
			SurveyExpireDateTimePicker.Value = DateTime.Now;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SurveyForm));
			this.SurveySendbutton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.SurveyCancelButton = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SurveyNameTextBox = new System.Windows.Forms.TextBox();
			this.SurveyDescTextBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.SurveyUserGroupcomboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SurveyExpireDateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.SurveyCatComboBox = new System.Windows.Forms.ComboBox();
			this.SurveyAddQuestionButton = new System.Windows.Forms.Button();
			this.SurveyPreListView = new System.Windows.Forms.ListView();
			this.Question = new System.Windows.Forms.ColumnHeader();
			this.Answers = new System.Windows.Forms.ColumnHeader();
			this.SurveyRemoveQbutton = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.SurveyPriorityComboBox = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// SurveySendbutton
			// 
			this.SurveySendbutton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.SurveySendbutton.Location = new System.Drawing.Point(24, 560);
			this.SurveySendbutton.Name = "SurveySendbutton";
			this.SurveySendbutton.TabIndex = 18;
			this.SurveySendbutton.Text = "Send";
			this.SurveySendbutton.Click += new System.EventHandler(this.SurveySendbutton_Click);
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(8, 336);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 14);
			this.label3.TabIndex = 17;
			this.label3.Text = "Survey Preview";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// SurveyCancelButton
			// 
			this.SurveyCancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.SurveyCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.SurveyCancelButton.Location = new System.Drawing.Point(128, 560);
			this.SurveyCancelButton.Name = "SurveyCancelButton";
			this.SurveyCancelButton.TabIndex = 23;
			this.SurveyCancelButton.Text = "Cancel";
			this.SurveyCancelButton.Click += new System.EventHandler(this.SurveyCancelButton_Click);
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label4.Location = new System.Drawing.Point(8, 18);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 25;
			this.label4.Text = "Survey Name";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label5.Location = new System.Drawing.Point(8, 57);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(112, 16);
			this.label5.TabIndex = 26;
			this.label5.Text = "Survey Description";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// SurveyNameTextBox
			// 
			this.SurveyNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.SurveyNameTextBox.Location = new System.Drawing.Point(136, 16);
			this.SurveyNameTextBox.Name = "SurveyNameTextBox";
			this.SurveyNameTextBox.Size = new System.Drawing.Size(360, 20);
			this.SurveyNameTextBox.TabIndex = 27;
			this.SurveyNameTextBox.Text = "";
			// 
			// SurveyDescTextBox
			// 
			this.SurveyDescTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.SurveyDescTextBox.Location = new System.Drawing.Point(136, 55);
			this.SurveyDescTextBox.Multiline = true;
			this.SurveyDescTextBox.Name = "SurveyDescTextBox";
			this.SurveyDescTextBox.Size = new System.Drawing.Size(360, 97);
			this.SurveyDescTextBox.TabIndex = 28;
			this.SurveyDescTextBox.Text = "";
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label6.Location = new System.Drawing.Point(8, 162);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(40, 16);
			this.label6.TabIndex = 31;
			this.label6.Text = "Invite";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// SurveyUserGroupcomboBox
			// 
			this.SurveyUserGroupcomboBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.SurveyUserGroupcomboBox.Location = new System.Drawing.Point(136, 160);
			this.SurveyUserGroupcomboBox.Name = "SurveyUserGroupcomboBox";
			this.SurveyUserGroupcomboBox.Size = new System.Drawing.Size(176, 21);
			this.SurveyUserGroupcomboBox.TabIndex = 32;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(8, 202);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 34;
			this.label1.Text = "Expire Time";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(8, 234);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 35;
			this.label2.Text = "Category";
			// 
			// SurveyExpireDateTimePicker
			// 
			this.SurveyExpireDateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.SurveyExpireDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.SurveyExpireDateTimePicker.Location = new System.Drawing.Point(136, 200);
			this.SurveyExpireDateTimePicker.MaxDate = new System.DateTime(2010, 12, 31, 0, 0, 0, 0);
			this.SurveyExpireDateTimePicker.MinDate = new System.DateTime(2003, 1, 1, 0, 0, 0, 0);
			this.SurveyExpireDateTimePicker.Name = "SurveyExpireDateTimePicker";
			this.SurveyExpireDateTimePicker.ShowUpDown = true;
			this.SurveyExpireDateTimePicker.Size = new System.Drawing.Size(176, 20);
			this.SurveyExpireDateTimePicker.TabIndex = 36;
			this.SurveyExpireDateTimePicker.Value = new System.DateTime(2003, 12, 1, 0, 0, 0, 0);
			// 
			// SurveyCatComboBox
			// 
			this.SurveyCatComboBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.SurveyCatComboBox.Location = new System.Drawing.Point(136, 232);
			this.SurveyCatComboBox.Name = "SurveyCatComboBox";
			this.SurveyCatComboBox.Size = new System.Drawing.Size(176, 21);
			this.SurveyCatComboBox.TabIndex = 37;
			// 
			// SurveyAddQuestionButton
			// 
			this.SurveyAddQuestionButton.Location = new System.Drawing.Point(8, 312);
			this.SurveyAddQuestionButton.Name = "SurveyAddQuestionButton";
			this.SurveyAddQuestionButton.Size = new System.Drawing.Size(112, 23);
			this.SurveyAddQuestionButton.TabIndex = 38;
			this.SurveyAddQuestionButton.Text = "Add New Question";
			this.SurveyAddQuestionButton.Click += new System.EventHandler(this.SurveyAddQuestionButton_Click);
			// 
			// SurveyPreListView
			// 
			this.SurveyPreListView.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.SurveyPreListView.AutoArrange = false;
			this.SurveyPreListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																								this.Question,
																								this.Answers});
			this.SurveyPreListView.FullRowSelect = true;
			this.SurveyPreListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.SurveyPreListView.Location = new System.Drawing.Point(8, 360);
			this.SurveyPreListView.MultiSelect = false;
			this.SurveyPreListView.Name = "SurveyPreListView";
			this.SurveyPreListView.Size = new System.Drawing.Size(496, 192);
			this.SurveyPreListView.TabIndex = 39;
			this.SurveyPreListView.View = System.Windows.Forms.View.Details;
			// 
			// Question
			// 
			this.Question.Width = 260;
			// 
			// Answers
			// 
			this.Answers.Width = 360;
			// 
			// SurveyRemoveQbutton
			// 
			this.SurveyRemoveQbutton.Location = new System.Drawing.Point(136, 312);
			this.SurveyRemoveQbutton.Name = "SurveyRemoveQbutton";
			this.SurveyRemoveQbutton.Size = new System.Drawing.Size(104, 23);
			this.SurveyRemoveQbutton.TabIndex = 40;
			this.SurveyRemoveQbutton.Text = "Remove Question";
			this.SurveyRemoveQbutton.Click += new System.EventHandler(this.SurveyRemoveQbutton_Click);
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label7.Location = new System.Drawing.Point(8, 271);
			this.label7.Name = "label7";
			this.label7.TabIndex = 41;
			this.label7.Text = "Priority";
			// 
			// SurveyPriorityComboBox
			// 
			this.SurveyPriorityComboBox.Location = new System.Drawing.Point(136, 272);
			this.SurveyPriorityComboBox.Name = "SurveyPriorityComboBox";
			this.SurveyPriorityComboBox.Size = new System.Drawing.Size(176, 21);
			this.SurveyPriorityComboBox.TabIndex = 42;
			// 
			// SurveyForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.SurveyCancelButton;
			this.ClientSize = new System.Drawing.Size(512, 598);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.SurveyPriorityComboBox,
																		  this.label7,
																		  this.SurveyRemoveQbutton,
																		  this.SurveyPreListView,
																		  this.SurveyAddQuestionButton,
																		  this.SurveyCatComboBox,
																		  this.SurveyExpireDateTimePicker,
																		  this.label2,
																		  this.label1,
																		  this.SurveyUserGroupcomboBox,
																		  this.label6,
																		  this.SurveyDescTextBox,
																		  this.SurveyNameTextBox,
																		  this.label5,
																		  this.label4,
																		  this.SurveyCancelButton,
																		  this.SurveySendbutton,
																		  this.label3});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(520, 520);
			this.Name = "SurveyForm";
			this.Text = "New Survey";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.SurveyForm_Closing);
			this.ResumeLayout(false);

		}
		#endregion

		private void SurveySendbutton_Click(object sender, System.EventArgs e)
		{
			mSrvyMngr.SendSurvey(this);
			Close();
		}

		private void SurveyForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
		}

		private void SurveyCancelButton_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void SurveyAddQuestionButton_Click(object sender, System.EventArgs e)
		{
			SurveyNewQuestionFrm tmpfrm = new SurveyNewQuestionFrm(this);
			tmpfrm.ShowDialog(this);
		}
		public void SurveySetQuestion(string question)
		{
			Char [] separators = {':'};			
			String [] vars = question.Split(separators);
			ListViewItem lquestion = new ListViewItem(vars[0],0);
			lquestion.SubItems.Add(vars[1]);
			SurveyPreListView.Items.AddRange(new ListViewItem[]{lquestion});			
		}

		private void SurveyRemoveQbutton_Click(object sender, System.EventArgs e)
		{
			if (SurveyPreListView.SelectedItems.Count != 0)
			{
				foreach(ListViewItem item in SurveyPreListView.SelectedItems)
				{
					SurveyPreListView.Items.Remove(item);
				}
			}
		}
	}
}
