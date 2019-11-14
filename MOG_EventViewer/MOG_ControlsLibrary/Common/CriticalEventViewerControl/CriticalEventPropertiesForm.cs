using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG.TIME;
using MOG.DATABASE;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG_ControlsLibrary.MogUtils_Settings;

namespace MOG_EventViewer.MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for CriticalEventPropertiesForm.
	/// </summary>
	public class CriticalEventPropertiesForm : System.Windows.Forms.Form
	{
		#region System defs
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.TabPage tpEvent;
		private System.Windows.Forms.TextBox tbEventID;
		private System.Windows.Forms.TextBox tbComputer;
		private System.Windows.Forms.TextBox tbUser;
		private System.Windows.Forms.TextBox tbType;
		private System.Windows.Forms.TextBox tbTime;
		private System.Windows.Forms.TextBox tbDate;
		private System.Windows.Forms.Label lblEventID;
		private System.Windows.Forms.Label lblUser;
		private System.Windows.Forms.Label lblComputer;
		private System.Windows.Forms.Label lblType;
		private System.Windows.Forms.Label lblTime;
		private System.Windows.Forms.Label lblDate;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.RichTextBox rtbDescription;
		private System.Windows.Forms.Label lblID;
		private System.Windows.Forms.TextBox tbBranch;
		private System.Windows.Forms.TextBox tbProject;
		private System.Windows.Forms.Label lblProject;
		private System.Windows.Forms.Label lblBranch;
		private System.Windows.Forms.TextBox tbID;
		private System.Windows.Forms.CheckBox chbxAlwaysOnTop;
		private Button PrevButton;
		private Button NextButton;
		private Button btnSendToMogware;
		private ToolTip toolTip1;
		private BackgroundWorker ErrorSendMailBackgroundWorker;
		private StatusStrip statusStrip1;
		private ToolStripProgressBar ErrorToolStripProgressBar;
		private ToolStripStatusLabel ErrorToolStripStatusLabel;
		private Timer ErrorMailProgressTimer;
		private IContainer components;

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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CriticalEventPropertiesForm));
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tpEvent = new System.Windows.Forms.TabPage();
			this.btnSendToMogware = new System.Windows.Forms.Button();
			this.tbBranch = new System.Windows.Forms.TextBox();
			this.tbProject = new System.Windows.Forms.TextBox();
			this.lblProject = new System.Windows.Forms.Label();
			this.lblBranch = new System.Windows.Forms.Label();
			this.tbEventID = new System.Windows.Forms.TextBox();
			this.tbID = new System.Windows.Forms.TextBox();
			this.tbComputer = new System.Windows.Forms.TextBox();
			this.tbUser = new System.Windows.Forms.TextBox();
			this.tbType = new System.Windows.Forms.TextBox();
			this.tbTime = new System.Windows.Forms.TextBox();
			this.tbDate = new System.Windows.Forms.TextBox();
			this.lblEventID = new System.Windows.Forms.Label();
			this.lblID = new System.Windows.Forms.Label();
			this.lblUser = new System.Windows.Forms.Label();
			this.lblComputer = new System.Windows.Forms.Label();
			this.lblType = new System.Windows.Forms.Label();
			this.lblTime = new System.Windows.Forms.Label();
			this.lblDate = new System.Windows.Forms.Label();
			this.lblDescription = new System.Windows.Forms.Label();
			this.rtbDescription = new System.Windows.Forms.RichTextBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.chbxAlwaysOnTop = new System.Windows.Forms.CheckBox();
			this.PrevButton = new System.Windows.Forms.Button();
			this.NextButton = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.ErrorSendMailBackgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.ErrorToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.ErrorToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.ErrorMailProgressTimer = new System.Windows.Forms.Timer(this.components);
			this.tabControl.SuspendLayout();
			this.tpEvent.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tpEvent);
			this.tabControl.Location = new System.Drawing.Point(8, 8);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(536, 477);
			this.tabControl.TabIndex = 0;
			// 
			// tpEvent
			// 
			this.tpEvent.Controls.Add(this.btnSendToMogware);
			this.tpEvent.Controls.Add(this.tbBranch);
			this.tpEvent.Controls.Add(this.tbProject);
			this.tpEvent.Controls.Add(this.lblProject);
			this.tpEvent.Controls.Add(this.lblBranch);
			this.tpEvent.Controls.Add(this.tbEventID);
			this.tpEvent.Controls.Add(this.tbID);
			this.tpEvent.Controls.Add(this.tbComputer);
			this.tpEvent.Controls.Add(this.tbUser);
			this.tpEvent.Controls.Add(this.tbType);
			this.tpEvent.Controls.Add(this.tbTime);
			this.tpEvent.Controls.Add(this.tbDate);
			this.tpEvent.Controls.Add(this.lblEventID);
			this.tpEvent.Controls.Add(this.lblID);
			this.tpEvent.Controls.Add(this.lblUser);
			this.tpEvent.Controls.Add(this.lblComputer);
			this.tpEvent.Controls.Add(this.lblType);
			this.tpEvent.Controls.Add(this.lblTime);
			this.tpEvent.Controls.Add(this.lblDate);
			this.tpEvent.Controls.Add(this.lblDescription);
			this.tpEvent.Controls.Add(this.rtbDescription);
			this.tpEvent.Location = new System.Drawing.Point(4, 22);
			this.tpEvent.Name = "tpEvent";
			this.tpEvent.Size = new System.Drawing.Size(528, 451);
			this.tpEvent.TabIndex = 0;
			this.tpEvent.Text = "Event";
			// 
			// btnSendToMogware
			// 
			this.btnSendToMogware.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSendToMogware.Image = global::MOG_EventViewer.Properties.Resources.Send_mail2;
			this.btnSendToMogware.Location = new System.Drawing.Point(480, 6);
			this.btnSendToMogware.Name = "btnSendToMogware";
			this.btnSendToMogware.Size = new System.Drawing.Size(39, 39);
			this.btnSendToMogware.TabIndex = 5;
			this.toolTip1.SetToolTip(this.btnSendToMogware, "Send this event to Mogware...");
			this.btnSendToMogware.UseVisualStyleBackColor = true;
			this.btnSendToMogware.Click += new System.EventHandler(this.btnSendToMogware_Click);
			// 
			// tbBranch
			// 
			this.tbBranch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbBranch.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbBranch.Location = new System.Drawing.Point(296, 64);
			this.tbBranch.Name = "tbBranch";
			this.tbBranch.ReadOnly = true;
			this.tbBranch.Size = new System.Drawing.Size(136, 13);
			this.tbBranch.TabIndex = 44;
			// 
			// tbProject
			// 
			this.tbProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbProject.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbProject.Location = new System.Drawing.Point(296, 48);
			this.tbProject.Name = "tbProject";
			this.tbProject.ReadOnly = true;
			this.tbProject.Size = new System.Drawing.Size(136, 13);
			this.tbProject.TabIndex = 43;
			// 
			// lblProject
			// 
			this.lblProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblProject.Location = new System.Drawing.Point(240, 48);
			this.lblProject.Name = "lblProject";
			this.lblProject.Size = new System.Drawing.Size(56, 16);
			this.lblProject.TabIndex = 42;
			this.lblProject.Text = "Project";
			this.lblProject.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblBranch
			// 
			this.lblBranch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblBranch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBranch.Location = new System.Drawing.Point(240, 64);
			this.lblBranch.Name = "lblBranch";
			this.lblBranch.Size = new System.Drawing.Size(56, 16);
			this.lblBranch.TabIndex = 41;
			this.lblBranch.Text = "Branch";
			this.lblBranch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbEventID
			// 
			this.tbEventID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbEventID.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbEventID.Location = new System.Drawing.Point(296, 32);
			this.tbEventID.Name = "tbEventID";
			this.tbEventID.ReadOnly = true;
			this.tbEventID.Size = new System.Drawing.Size(136, 13);
			this.tbEventID.TabIndex = 40;
			// 
			// tbID
			// 
			this.tbID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbID.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbID.Location = new System.Drawing.Point(296, 16);
			this.tbID.Name = "tbID";
			this.tbID.ReadOnly = true;
			this.tbID.Size = new System.Drawing.Size(136, 13);
			this.tbID.TabIndex = 38;
			// 
			// tbComputer
			// 
			this.tbComputer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbComputer.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbComputer.Location = new System.Drawing.Point(88, 64);
			this.tbComputer.Name = "tbComputer";
			this.tbComputer.ReadOnly = true;
			this.tbComputer.Size = new System.Drawing.Size(136, 13);
			this.tbComputer.TabIndex = 37;
			// 
			// tbUser
			// 
			this.tbUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbUser.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbUser.Location = new System.Drawing.Point(88, 48);
			this.tbUser.Name = "tbUser";
			this.tbUser.ReadOnly = true;
			this.tbUser.Size = new System.Drawing.Size(136, 13);
			this.tbUser.TabIndex = 36;
			// 
			// tbType
			// 
			this.tbType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbType.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbType.Location = new System.Drawing.Point(96, 80);
			this.tbType.Name = "tbType";
			this.tbType.ReadOnly = true;
			this.tbType.Size = new System.Drawing.Size(336, 13);
			this.tbType.TabIndex = 35;
			// 
			// tbTime
			// 
			this.tbTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbTime.Location = new System.Drawing.Point(88, 32);
			this.tbTime.Name = "tbTime";
			this.tbTime.ReadOnly = true;
			this.tbTime.Size = new System.Drawing.Size(136, 13);
			this.tbTime.TabIndex = 34;
			// 
			// tbDate
			// 
			this.tbDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbDate.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbDate.Location = new System.Drawing.Point(88, 16);
			this.tbDate.Name = "tbDate";
			this.tbDate.ReadOnly = true;
			this.tbDate.Size = new System.Drawing.Size(136, 13);
			this.tbDate.TabIndex = 33;
			// 
			// lblEventID
			// 
			this.lblEventID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblEventID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblEventID.Location = new System.Drawing.Point(240, 32);
			this.lblEventID.Name = "lblEventID";
			this.lblEventID.Size = new System.Drawing.Size(56, 16);
			this.lblEventID.TabIndex = 30;
			this.lblEventID.Text = "Event ID";
			this.lblEventID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblID
			// 
			this.lblID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblID.Location = new System.Drawing.Point(240, 16);
			this.lblID.Name = "lblID";
			this.lblID.Size = new System.Drawing.Size(56, 16);
			this.lblID.TabIndex = 28;
			this.lblID.Text = "ID";
			this.lblID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblUser
			// 
			this.lblUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblUser.Location = new System.Drawing.Point(16, 48);
			this.lblUser.Name = "lblUser";
			this.lblUser.Size = new System.Drawing.Size(72, 16);
			this.lblUser.TabIndex = 27;
			this.lblUser.Text = "User";
			this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblComputer
			// 
			this.lblComputer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblComputer.Location = new System.Drawing.Point(16, 64);
			this.lblComputer.Name = "lblComputer";
			this.lblComputer.Size = new System.Drawing.Size(72, 16);
			this.lblComputer.TabIndex = 26;
			this.lblComputer.Text = "Computer";
			this.lblComputer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblType
			// 
			this.lblType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType.Location = new System.Drawing.Point(16, 80);
			this.lblType.Name = "lblType";
			this.lblType.Size = new System.Drawing.Size(80, 16);
			this.lblType.TabIndex = 25;
			this.lblType.Text = "Type";
			this.lblType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblTime
			// 
			this.lblTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTime.Location = new System.Drawing.Point(16, 32);
			this.lblTime.Name = "lblTime";
			this.lblTime.Size = new System.Drawing.Size(72, 16);
			this.lblTime.TabIndex = 24;
			this.lblTime.Text = "Time";
			this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblDate
			// 
			this.lblDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDate.Location = new System.Drawing.Point(16, 16);
			this.lblDate.Name = "lblDate";
			this.lblDate.Size = new System.Drawing.Size(72, 16);
			this.lblDate.TabIndex = 23;
			this.lblDate.Text = "Date";
			this.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblDescription
			// 
			this.lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDescription.Location = new System.Drawing.Point(16, 120);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(152, 16);
			this.lblDescription.TabIndex = 22;
			this.lblDescription.Text = "Description";
			// 
			// rtbDescription
			// 
			this.rtbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.rtbDescription.Location = new System.Drawing.Point(16, 136);
			this.rtbDescription.Name = "rtbDescription";
			this.rtbDescription.ReadOnly = true;
			this.rtbDescription.Size = new System.Drawing.Size(496, 293);
			this.rtbDescription.TabIndex = 21;
			this.rtbDescription.Text = "";
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnClose.Location = new System.Drawing.Point(456, 491);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 1;
			this.btnClose.Text = "Close";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// chbxAlwaysOnTop
			// 
			this.chbxAlwaysOnTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chbxAlwaysOnTop.Location = new System.Drawing.Point(40, 499);
			this.chbxAlwaysOnTop.Name = "chbxAlwaysOnTop";
			this.chbxAlwaysOnTop.Size = new System.Drawing.Size(104, 16);
			this.chbxAlwaysOnTop.TabIndex = 2;
			this.chbxAlwaysOnTop.Text = "Always on top";
			this.chbxAlwaysOnTop.CheckedChanged += new System.EventHandler(this.chbxAlwaysOnTop_CheckedChanged);
			// 
			// PrevButton
			// 
			this.PrevButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.PrevButton.Location = new System.Drawing.Point(197, 491);
			this.PrevButton.Name = "PrevButton";
			this.PrevButton.Size = new System.Drawing.Size(75, 23);
			this.PrevButton.TabIndex = 3;
			this.PrevButton.Text = "Previous";
			this.PrevButton.UseVisualStyleBackColor = true;
			this.PrevButton.Click += new System.EventHandler(this.PrevButton_Click);
			// 
			// NextButton
			// 
			this.NextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.NextButton.Location = new System.Drawing.Point(278, 491);
			this.NextButton.Name = "NextButton";
			this.NextButton.Size = new System.Drawing.Size(75, 23);
			this.NextButton.TabIndex = 4;
			this.NextButton.Text = "Next";
			this.NextButton.UseVisualStyleBackColor = true;
			this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
			// 
			// ErrorSendMailBackgroundWorker
			// 
			this.ErrorSendMailBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ErrorSendMailBackgroundWorker_DoWork);
			this.ErrorSendMailBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ErrorSendMailBackgroundWorker_RunWorkerCompleted);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ErrorToolStripProgressBar,
            this.ErrorToolStripStatusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 530);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(552, 22);
			this.statusStrip1.TabIndex = 5;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// ErrorToolStripProgressBar
			// 
			this.ErrorToolStripProgressBar.Name = "ErrorToolStripProgressBar";
			this.ErrorToolStripProgressBar.Size = new System.Drawing.Size(100, 16);
			this.ErrorToolStripProgressBar.Visible = false;
			// 
			// ErrorToolStripStatusLabel
			// 
			this.ErrorToolStripStatusLabel.Name = "ErrorToolStripStatusLabel";
			this.ErrorToolStripStatusLabel.Size = new System.Drawing.Size(131, 17);
			this.ErrorToolStripStatusLabel.Text = "Processing mail request...";
			this.ErrorToolStripStatusLabel.Visible = false;
			// 
			// ErrorMailProgressTimer
			// 
			this.ErrorMailProgressTimer.Tick += new System.EventHandler(this.ErrorMailProgressTimer_Tick);
			// 
			// CriticalEventPropertiesForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(552, 552);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.NextButton);
			this.Controls.Add(this.PrevButton);
			this.Controls.Add(this.chbxAlwaysOnTop);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.tabControl);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CriticalEventPropertiesForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Critical Event Properties";
			this.tabControl.ResumeLayout(false);
			this.tpEvent.ResumeLayout(false);
			this.tpEvent.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		#endregion

		private ListView mListView = null;
		private int mCurrentIndex = 0;
		private MOG_DBEventInfo info = null;
		private bool mComplete = false;

		public CriticalEventPropertiesForm(ListView listView)
		{
			InitializeComponent();

			mListView = listView;			
		}
				
		public string EventDescription
		{
			get
			{
				if (info != null)
				{
					return info.mDescription;
				}

				return "";
			}
		}

		public string EventStackTrace
		{
			get
			{
				if (info != null)
				{
					return info.mStackTrace;
				}

				return "";
			}
		}

		public string EventTitle
		{
			get
			{
				if (info != null)
				{
					return info.mTitle;
				}

				return "";
			}
		}

		public string EventVerboseDescription
		{
			get
			{
				if (info != null)
				{
					return "Title:\n" + info.mTitle + "\n\nMessage:\n" + info.mDescription + "\n\nStackTrace:\n" + info.mStackTrace;
				}

				return "";
			}
		}		

		public string EventDate
		{
			get
			{
				if (info != null)
				{
					return (new MOG_Time(info.mTimeStamp)).FormatString("{Day.3} {Day.1} {Month.3} {Year.4}");
				}

				return "";
			}
		}

		public string EventTime
		{
			get
			{
				if (info != null)
				{
					return (new MOG_Time(info.mTimeStamp)).FormatString("{Hour.2}:{Minute.2}:{Second.2} {AMPM}");
				}

				return "";
			}
		}

		public string EventType
		{
			get
			{
				if (info != null)
				{
					return info.mType.ToString();
				}

				return "";
			}
		}

		public string ErrorEmail
		{
			get
			{
				string email = MogUtils_Settings.LoadSetting_default("EventViewer", "ReturnEmail", "");
				if (email.Length == 0)
				{
					email = Microsoft.VisualBasic.Interaction.InputBox("Enter your local Email address so you can be notified when a fix for this issue is completed", "Local return email address", "", -1, -1);
					MogUtils_Settings.SaveSetting("EventViewer", "ReturnEmail", email);					
				}
				
				// Is it still blank?
				if (email.Length == 0)
				{
					email = "unknown@mogware.com";
				}

				return email; 
			}
		}

		internal void LoadEvent(int index)
		{
			if (mListView != null)
			{
				if (index >= 0 && index < mListView.Items.Count)
				{
					mCurrentIndex = index;
					
					EventData data = mListView.Items[index].Tag as EventData;
					if (data != null)
					{
						info = data.eventInfo;
						if (info != null)
						{
							//"ID", "Type", "Timestamp", "Description", "EventID", "Username", "Computer", "Project", "Branch" } );
							tbID.Text = info.mID.ToString();
							tbType.Text = info.mType;
							tbDate.Text = EventDate;
							tbTime.Text = EventTime;
							tbEventID.Text = info.mEventID;
							tbUser.Text = info.mUserName;
							tbComputer.Text = info.mComputerName;
							tbProject.Text = info.mProjectName;
							tbBranch.Text = info.mBranchName;
							rtbDescription.Text = EventVerboseDescription;
						}
					}

					PrevButton.Enabled = index > 0;
					NextButton.Enabled = index < mListView.Items.Count - 1;
				}
			}
		}
		
		private void btnClose_Click(object sender, System.EventArgs e)
		{
			Dispose();
		}

		private void chbxAlwaysOnTop_CheckedChanged(object sender, System.EventArgs e)
		{
			this.TopMost = this.chbxAlwaysOnTop.Checked;
		}

		private void NextButton_Click(object sender, EventArgs e)
		{
			LoadEvent(mCurrentIndex + 1);
		}

		private void PrevButton_Click(object sender, EventArgs e)
		{
			LoadEvent(mCurrentIndex - 1);
		}

		private void btnSendToMogware_Click(object sender, EventArgs e)
		{
			mComplete = false;
			ErrorToolStripStatusLabel.Text = "Processing mail request...";

			btnClose.Enabled = false;
			btnSendToMogware.Enabled = false;
			NextButton.Enabled = false;
			PrevButton.Enabled = false;

			ErrorToolStripStatusLabel.Visible = true;
			ErrorToolStripProgressBar.Visible = true;
			ErrorToolStripProgressBar.Step = 1;

			string message = FormatErrorMessage();

			ErrorMailProgressTimer.Enabled = true;
			ErrorMailProgressTimer.Start();

			ErrorSendMailBackgroundWorker.RunWorkerAsync();	
		}

		private String FormatErrorMessage()
		{
			String systemMode = "MOG";

			return String.Concat(
//				"User Description:\r\n",
//				"---------------------------------\r\n",
//				ErrorDescription, "\r\n\r\n\r\n\r\n",
				"==================================================\r\n",
				"MOG Event \r\n",
				"==================================================\r\n",
				" MOG Component: ", systemMode, "\r\n",
				" Date:", EventDate, "\r\n",
				" Time:", EventTime, "\r\n",
				" Type:", EventType, "\r\n",
				"\r\n",
				" ---------------------------------\r\n",
				" Event Description", "\r\n",
				" ---------------------------------\r\n",
				" ", EventVerboseDescription
				);
		}

		public string AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}	

		private void ErrorSendMailBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				string userEmail = (ErrorEmail != null && ErrorEmail.Length > 0 && ErrorEmail.IndexOf("@") != -1) ? ErrorEmail : "unspecified@mogware.com";
				MailMessage message = new MailMessage(userEmail, "support@mogware.com");
				string subject = "";

				try
				{
					subject = String.Format("MOG({0}) {1}", AssemblyVersion, EventVerboseDescription);
					if (subject.Length > 250) subject = subject.Substring(0, 255);
					if (subject.Contains("\n"))
					{
						subject = subject.Replace("\n", " ");
					}
					message.Subject = subject;
				}
				catch(Exception ex)
				{
					try
					{
						ex.ToString();
						subject = String.Format("MOG({0})", AssemblyVersion);
						message.Subject = subject;
					}
					catch
					{
						message.Subject = "MOG Event";						
					}
				}

				message.Body = FormatErrorMessage();

				SmtpClient client = new SmtpClient("mail.mogware.com");
				client.Credentials = new NetworkCredential("support@mogware.com", "J3RKK");
				client.DeliveryMethod = SmtpDeliveryMethod.Network;

				client.Send(message);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Send Email");
			}
		}

		private void ErrorSendMailBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			mComplete = true;
			ErrorMailProgressTimer.Interval = 1000;
			ErrorToolStripStatusLabel.Text = "Send mail complete!";
			ErrorToolStripProgressBar.Value = ErrorToolStripProgressBar.Maximum;			

			btnClose.Enabled = true;
			btnSendToMogware.Enabled = true;
			NextButton.Enabled = true;
			PrevButton.Enabled = true;
		}

		private void ErrorMailProgressTimer_Tick(object sender, EventArgs e)
		{
			if (mComplete == false)
			{
				ErrorToolStripProgressBar.PerformStep();
				if (ErrorToolStripProgressBar.Value >= ErrorToolStripProgressBar.Maximum)
				{
					ErrorToolStripProgressBar.Value = 0;
				}
			}
			else
			{
				ErrorToolStripStatusLabel.Text = "";
				ErrorToolStripProgressBar.Value = ErrorToolStripProgressBar.Minimum;
				ErrorToolStripStatusLabel.Visible = false;
				ErrorToolStripProgressBar.Visible = false;

				ErrorMailProgressTimer.Stop();
				ErrorMailProgressTimer.Enabled = false;
			}
		}
	}
}
