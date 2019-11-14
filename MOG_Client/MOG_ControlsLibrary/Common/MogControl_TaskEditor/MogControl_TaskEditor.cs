using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG;
using MOG.TIME;
using MOG.DATABASE;
using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG_ControlsLibrary.Utils;


namespace MOG_ControlsLibrary.Common.MogControl_TaskEditor
{
	/// <summary>
	/// Summary description for MogControl_TaskEditor.
	/// </summary>
	public class MogControl_TaskEditor : System.Windows.Forms.UserControl
	{
		#region Form variables

		private System.Windows.Forms.ListView TaskAsscociatedAssetsListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		public System.Windows.Forms.TextBox TaskImportanceTextBox;
		private System.Windows.Forms.RichTextBox TaskCommentsHistoryRichTextBox;
		private System.Windows.Forms.RichTextBox TaskCommentsRichTextBox;
		private System.Windows.Forms.ComboBox TaskUsersComboBox;
		private System.Windows.Forms.DateTimePicker TaskDueDateTimePicker;
		public System.Windows.Forms.ComboBox TaskPriorityComboBox;
		private System.Windows.Forms.TextBox TaskNameTextBox;
		private System.Windows.Forms.Panel TaskEditorPanel;
		private System.Windows.Forms.Label PercentCompleteLabel;
		private System.Windows.Forms.Label PercentCompleteValueLabel;
		private System.Windows.Forms.TrackBar PercentCompleteTrackBar;
		private System.Windows.Forms.Label TaskAscociatedAssetsLabel;
		private System.Windows.Forms.Label TaskCommentHistoryLabel;
		private System.Windows.Forms.Label TaskAssignToLabel;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion
		#region User variables
		private MOG_TaskInfo mTaskInfo;
		private MOG_TaskInfo mOriginalTaskInfo;
		private MOG_Database mDb;
		private bool mEditableMode = true;
		#endregion

		#region Getters and Setters
		[Category("Appearance"), Description("Show in edit or view format.  True is editable and false is view.")]
		public bool EditableMode 
		{
			get 
			{
				return mEditableMode;
			}
			set
			{
				mEditableMode = value;

				// Percentage complete bar
				this.PercentCompleteLabel.Visible = mEditableMode;
				this.PercentCompleteTrackBar.Visible = mEditableMode;
				this.PercentCompleteValueLabel.Visible = mEditableMode;

				// Ascociated assets
				this.TaskAscociatedAssetsLabel.Visible = mEditableMode;
				this.TaskAsscociatedAssetsListView.Visible = mEditableMode;

				// New comments
				this.TaskCommentsRichTextBox.Visible = mEditableMode;
				if (!mEditableMode)
				{
					this.TaskCommentHistoryLabel.Location = this.PercentCompleteLabel.Location;
					this.TaskCommentsHistoryRichTextBox.Location = this.PercentCompleteTrackBar.Location;
				}
			}
		}

		/// <summary>
		/// Get assigned asset
		/// </summary>
		private string TaskAssignedAsset
		{
			get
			{
				if (TaskAsscociatedAssetsListView.Items.Count > 0)
				{
					return TaskAsscociatedAssetsListView.Items[0].SubItems[3].Text;
				}

				return "";
			}
			set
			{
				if (value != null && value.Length > 0)
				{
					MOG_Filename mogAssetName = new MOG_Filename(value);
					ListViewItem item = new ListViewItem();

					item.Text = mogAssetName.GetAssetLabel();
					item.SubItems.Add(mogAssetName.GetAssetClassification());
					item.SubItems.Add(mogAssetName.GetVersionTimeStampString(""));
					item.SubItems.Add(value);
					item.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex(value);

					TaskAsscociatedAssetsListView.Items.Add(item);
				}
			}
		}
		/// <summary>
		/// Get the taskInfo
		/// </summary>
		/// <returns></returns>
		public MOG_TaskInfo TaskInfo
		{
			get
			{
				return mTaskInfo;
			}
		}		
		/// <summary>
		/// Name of this task
		/// </summary>
		private string TaskName
		{
			get 
			{
				return TaskNameTextBox.Text;
			}
			set 
			{
				TaskNameTextBox.Text = value;
				if (value.Length == 0)
				{
					TaskNameTextBox.Enabled = false;
				}
				else
				{
					TaskNameTextBox.Enabled = true;
				}
			}
		}
		/// <summary>
		/// Percentage complete of this task
		/// </summary>
		private int TaskPercentage
		{
			set			
			{
				PercentCompleteTrackBar.Value = value;
				PercentCompleteValueLabel.Text = value.ToString();
			}
			get
			{
				return PercentCompleteTrackBar.Value;
			}
		}
		/// <summary>
		/// Priority of this task
		/// </summary>
		private string TaskPriority
		{
			get { return TaskPriorityComboBox.Text; }
			set { TaskPriorityComboBox.Text = value; }
		}
		/// <summary>
		/// User that is assigned this task
		/// </summary>
		private string TaskAssignedTo
		{
			get 
			{
				// Check to see if this assigned user it a department
				if (TaskUsersComboBox.Text.ToLower().IndexOf("department{") != -1)
				{
					// If so, we need to strip off the department tag, set the user to blank
					string []parts = TaskUsersComboBox.Text.Split("{}".ToCharArray());
					if (parts != null && parts.Length >= 2)
					{
						string department = parts[1];

						// Set our department
						mTaskInfo.SetDepartment(department);

						TaskUsersComboBox.Text = "";						
					}

					return "";
				}
				else
				{
					return TaskUsersComboBox.Text;
				}
			}
			set { TaskUsersComboBox.Text = value; }
		}
		/// <summary>
		/// Task comments
		/// </summary>
		private string TaskComments
		{
			get 
			{ 
				return TaskCommentsHistoryRichTextBox.Text + "\n" + mTaskInfo.GetChangesAsComment(this.mOriginalTaskInfo);
//				MOG_Time current = new MOG_Time();
//				if (TaskCommentsRichTextBox.Text.Length == 0)
//				{
//					return TaskCommentsHistoryRichTextBox.Text + "\n" + mTaskInfo.GetChangesAsComment(this.mOriginalTaskInfo);
//				}
//				else
//				{
//					return TaskCommentsHistoryRichTextBox.Text + "\n" + mTaskInfo.GetCreator() + "[" + current.FormatString("") + "]" + TaskCommentsRichTextBox.Text + "\n" + mTaskInfo.GetChangesAsComment(this.mOriginalTaskInfo);
//				}
			}
			set 
			{ 
				TaskCommentsHistoryRichTextBox.Text = value; 
				TaskCommentsHistoryRichTextBox.ForeColor = Color.Red;
			}
		}
		#endregion
		
		/// <summary>
		/// Default constructor
		/// </summary>
		public MogControl_TaskEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			mDb = MOG_ControllerSystem.GetDB();
			TaskAsscociatedAssetsListView.SmallImageList = MogUtil_AssetIcons.Images;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TaskAscociatedAssetsLabel = new System.Windows.Forms.Label();
			this.TaskAsscociatedAssetsListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.TaskCommentHistoryLabel = new System.Windows.Forms.Label();
			this.PercentCompleteLabel = new System.Windows.Forms.Label();
			this.TaskImportanceTextBox = new System.Windows.Forms.TextBox();
			this.TaskCommentsHistoryRichTextBox = new System.Windows.Forms.RichTextBox();
			this.TaskCommentsRichTextBox = new System.Windows.Forms.RichTextBox();
			this.TaskUsersComboBox = new System.Windows.Forms.ComboBox();
			this.TaskAssignToLabel = new System.Windows.Forms.Label();
			this.TaskDueDateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.TaskPriorityComboBox = new System.Windows.Forms.ComboBox();
			this.PercentCompleteValueLabel = new System.Windows.Forms.Label();
			this.PercentCompleteTrackBar = new System.Windows.Forms.TrackBar();
			this.TaskNameTextBox = new System.Windows.Forms.TextBox();
			this.TaskEditorPanel = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.PercentCompleteTrackBar)).BeginInit();
			this.TaskEditorPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// TaskAscociatedAssetsLabel
			// 
			this.TaskAscociatedAssetsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.TaskAscociatedAssetsLabel.BackColor = System.Drawing.SystemColors.Control;
			this.TaskAscociatedAssetsLabel.Location = new System.Drawing.Point(8, 264);
			this.TaskAscociatedAssetsLabel.Name = "TaskAscociatedAssetsLabel";
			this.TaskAscociatedAssetsLabel.Size = new System.Drawing.Size(248, 16);
			this.TaskAscociatedAssetsLabel.TabIndex = 29;
			this.TaskAscociatedAssetsLabel.Text = "Ascociated assets";
			// 
			// TaskAsscociatedAssetsListView
			// 
			this.TaskAsscociatedAssetsListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TaskAsscociatedAssetsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																											this.columnHeader1,
																											this.columnHeader3,
																											this.columnHeader2});
			this.TaskAsscociatedAssetsListView.FullRowSelect = true;
			this.TaskAsscociatedAssetsListView.Location = new System.Drawing.Point(8, 280);
			this.TaskAsscociatedAssetsListView.Name = "TaskAsscociatedAssetsListView";
			this.TaskAsscociatedAssetsListView.Size = new System.Drawing.Size(520, 80);
			this.TaskAsscociatedAssetsListView.TabIndex = 28;
			this.TaskAsscociatedAssetsListView.TabStop = false;
			this.TaskAsscociatedAssetsListView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Asset Name";
			this.columnHeader1.Width = 252;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Classification";
			this.columnHeader3.Width = 195;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Date";
			this.columnHeader2.Width = 76;
			// 
			// TaskCommentHistoryLabel
			// 
			this.TaskCommentHistoryLabel.BackColor = System.Drawing.SystemColors.Control;
			this.TaskCommentHistoryLabel.Location = new System.Drawing.Point(8, 112);
			this.TaskCommentHistoryLabel.Name = "TaskCommentHistoryLabel";
			this.TaskCommentHistoryLabel.Size = new System.Drawing.Size(336, 16);
			this.TaskCommentHistoryLabel.TabIndex = 27;
			this.TaskCommentHistoryLabel.Text = "Comment history";
			// 
			// PercentCompleteLabel
			// 
			this.PercentCompleteLabel.BackColor = System.Drawing.SystemColors.Control;
			this.PercentCompleteLabel.Location = new System.Drawing.Point(8, 64);
			this.PercentCompleteLabel.Name = "PercentCompleteLabel";
			this.PercentCompleteLabel.Size = new System.Drawing.Size(296, 16);
			this.PercentCompleteLabel.TabIndex = 26;
			this.PercentCompleteLabel.Text = "Percent complete";
			// 
			// TaskImportanceTextBox
			// 
			this.TaskImportanceTextBox.Location = new System.Drawing.Point(496, 8);
			this.TaskImportanceTextBox.Name = "TaskImportanceTextBox";
			this.TaskImportanceTextBox.Size = new System.Drawing.Size(32, 20);
			this.TaskImportanceTextBox.TabIndex = 25;
			this.TaskImportanceTextBox.TabStop = false;
			this.TaskImportanceTextBox.Text = "";
			this.TaskImportanceTextBox.Visible = false;
			// 
			// TaskCommentsHistoryRichTextBox
			// 
			this.TaskCommentsHistoryRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TaskCommentsHistoryRichTextBox.Location = new System.Drawing.Point(8, 128);
			this.TaskCommentsHistoryRichTextBox.Name = "TaskCommentsHistoryRichTextBox";
			this.TaskCommentsHistoryRichTextBox.ReadOnly = true;
			this.TaskCommentsHistoryRichTextBox.Size = new System.Drawing.Size(520, 80);
			this.TaskCommentsHistoryRichTextBox.TabIndex = 24;
			this.TaskCommentsHistoryRichTextBox.TabStop = false;
			this.TaskCommentsHistoryRichTextBox.Text = "";
			// 
			// TaskCommentsRichTextBox
			// 
			this.TaskCommentsRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TaskCommentsRichTextBox.Location = new System.Drawing.Point(8, 216);
			this.TaskCommentsRichTextBox.Name = "TaskCommentsRichTextBox";
			this.TaskCommentsRichTextBox.Size = new System.Drawing.Size(520, 48);
			this.TaskCommentsRichTextBox.TabIndex = 4;
			this.TaskCommentsRichTextBox.Text = "";
			// 
			// TaskUsersComboBox
			// 
			this.TaskUsersComboBox.Location = new System.Drawing.Point(400, 32);
			this.TaskUsersComboBox.Name = "TaskUsersComboBox";
			this.TaskUsersComboBox.Size = new System.Drawing.Size(120, 21);
			this.TaskUsersComboBox.TabIndex = 3;
			// 
			// TaskAssignToLabel
			// 
			this.TaskAssignToLabel.BackColor = System.Drawing.SystemColors.Control;
			this.TaskAssignToLabel.Location = new System.Drawing.Point(400, 16);
			this.TaskAssignToLabel.Name = "TaskAssignToLabel";
			this.TaskAssignToLabel.Size = new System.Drawing.Size(80, 16);
			this.TaskAssignToLabel.TabIndex = 21;
			this.TaskAssignToLabel.Text = "Assign To";
			// 
			// TaskDueDateTimePicker
			// 
			this.TaskDueDateTimePicker.Location = new System.Drawing.Point(8, 32);
			this.TaskDueDateTimePicker.Name = "TaskDueDateTimePicker";
			this.TaskDueDateTimePicker.Size = new System.Drawing.Size(216, 20);
			this.TaskDueDateTimePicker.TabIndex = 1;
			// 
			// TaskPriorityComboBox
			// 
			this.TaskPriorityComboBox.Location = new System.Drawing.Point(240, 32);
			this.TaskPriorityComboBox.Name = "TaskPriorityComboBox";
			this.TaskPriorityComboBox.Size = new System.Drawing.Size(136, 21);
			this.TaskPriorityComboBox.TabIndex = 2;
			// 
			// PercentCompleteValueLabel
			// 
			this.PercentCompleteValueLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.PercentCompleteValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.PercentCompleteValueLabel.Location = new System.Drawing.Point(464, 80);
			this.PercentCompleteValueLabel.Name = "PercentCompleteValueLabel";
			this.PercentCompleteValueLabel.Size = new System.Drawing.Size(40, 16);
			this.PercentCompleteValueLabel.TabIndex = 18;
			this.PercentCompleteValueLabel.Text = "0 %";
			this.PercentCompleteValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// PercentCompleteTrackBar
			// 
			this.PercentCompleteTrackBar.AutoSize = false;
			this.PercentCompleteTrackBar.Location = new System.Drawing.Point(8, 80);
			this.PercentCompleteTrackBar.Maximum = 100;
			this.PercentCompleteTrackBar.Name = "PercentCompleteTrackBar";
			this.PercentCompleteTrackBar.Size = new System.Drawing.Size(440, 16);
			this.PercentCompleteTrackBar.TabIndex = 17;
			this.PercentCompleteTrackBar.TabStop = false;
			this.PercentCompleteTrackBar.TickFrequency = 10;
			this.PercentCompleteTrackBar.Scroll += new System.EventHandler(this.TaskPercentageTrackBar_Scroll);
			// 
			// TaskNameTextBox
			// 
			this.TaskNameTextBox.Location = new System.Drawing.Point(8, 8);
			this.TaskNameTextBox.Name = "TaskNameTextBox";
			this.TaskNameTextBox.Size = new System.Drawing.Size(376, 20);
			this.TaskNameTextBox.TabIndex = 0;
			this.TaskNameTextBox.Text = "";
			// 
			// TaskEditorPanel
			// 
			this.TaskEditorPanel.AutoScroll = true;
			this.TaskEditorPanel.Controls.Add(this.TaskNameTextBox);
			this.TaskEditorPanel.Controls.Add(this.TaskAssignToLabel);
			this.TaskEditorPanel.Controls.Add(this.TaskImportanceTextBox);
			this.TaskEditorPanel.Controls.Add(this.TaskDueDateTimePicker);
			this.TaskEditorPanel.Controls.Add(this.TaskPriorityComboBox);
			this.TaskEditorPanel.Controls.Add(this.TaskUsersComboBox);
			this.TaskEditorPanel.Controls.Add(this.PercentCompleteLabel);
			this.TaskEditorPanel.Controls.Add(this.PercentCompleteValueLabel);
			this.TaskEditorPanel.Controls.Add(this.PercentCompleteTrackBar);
			this.TaskEditorPanel.Controls.Add(this.TaskCommentHistoryLabel);
			this.TaskEditorPanel.Controls.Add(this.TaskCommentsHistoryRichTextBox);
			this.TaskEditorPanel.Controls.Add(this.TaskCommentsRichTextBox);
			this.TaskEditorPanel.Controls.Add(this.TaskAscociatedAssetsLabel);
			this.TaskEditorPanel.Controls.Add(this.TaskAsscociatedAssetsListView);
			this.TaskEditorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TaskEditorPanel.Location = new System.Drawing.Point(0, 0);
			this.TaskEditorPanel.Name = "TaskEditorPanel";
			this.TaskEditorPanel.Size = new System.Drawing.Size(536, 368);
			this.TaskEditorPanel.TabIndex = 32;
			// 
			// MogControl_TaskEditor
			// 
			this.Controls.Add(this.TaskEditorPanel);
			this.Name = "MogControl_TaskEditor";
			this.Size = new System.Drawing.Size(536, 368);
			this.EnabledChanged += new System.EventHandler(this.MogControl_TaskEditor_EnabledChanged);
			this.Load += new System.EventHandler(this.MogControl_TaskEditor_Load);
			((System.ComponentModel.ISupportInitialize)(this.PercentCompleteTrackBar)).EndInit();
			this.TaskEditorPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Initializers
		/// <summary>
		/// Initialize our fields with the data within this task
		/// </summary>
		/// <param name="TaskInfo"></param>
		public void Initialize(MOG_TaskInfo TaskInfo)
		{
			// Make sure we have a valid task
			if (TaskInfo != null)
			{
				// Save the original
				mOriginalTaskInfo = TaskInfo.Clone();

				// Set our task member var
				mTaskInfo = TaskInfo;

				// Populate the priorities
				PopulatePriority();
				// Populate the assigned to
				PopulateAssignTo();				

				// Set the name
				this.TaskName = mTaskInfo.GetName();
				this.TaskPercentage = mTaskInfo.GetPercentComplete();
				this.TaskPriority = mTaskInfo.GetPriority();
				this.TaskAssignedTo = mTaskInfo.GetAssignedTo();
				this.TaskComments = mTaskInfo.GetComment();
				GetAssignedAssets();
//				this.TaskAssignedAsset = mTaskInfo.GetAsset();

				// Set the due date
				if (mTaskInfo.GetDueDate() != null)
				{
					MOG_Time time = new MOG_Time(mTaskInfo.GetDueDate());
					TaskDueDateTimePicker.Value = time.ToDateTime();
				}

				// Set our importance rating
				TaskImportanceTextBox.Text = mTaskInfo.GetImportanceRating().ToString();
			}
			else
			{
				// Clear out our data
				this.TaskName = "";
				this.TaskPriority = "";
				this.TaskAssignedTo = "";
				this.TaskPercentage = -1;
				this.TaskComments = "";
//
//				TaskPriorityComboBox.Enabled = false;
//				TaskUsersComboBox.Enabled = false;
//				TaskDueDateTimePicker.Enabled = false;
			}
		}

		private void GetAssignedAssets()
		{
			try
			{
				if (mTaskInfo.GetHasAsset())
				{
					TaskAsscociatedAssetsListView.Items.Clear();
					TaskAsscociatedAssetsListView.BeginUpdate();

					foreach (MOG_Filename mogAssetName in MOG_DBTaskAPI.GetTaskAssetLinks(mTaskInfo.GetTaskID()))
					{
						ListViewItem item = new ListViewItem();

						item.Text = mogAssetName.GetAssetLabel();
						item.SubItems.Add(mogAssetName.GetAssetClassification());
						item.SubItems.Add(mogAssetName.GetVersionTimeStampString(""));
						item.SubItems.Add(mogAssetName.GetEncodedFilename());
						item.ImageIndex = MogUtil_AssetIcons.GetClassIconIndex(mogAssetName.GetEncodedFilename());

						TaskAsscociatedAssetsListView.Items.Add(item);
					}

					TaskAsscociatedAssetsListView.EndUpdate();
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Write out our changes back to the TaskInfo
		/// </summary>
		public void FinializeTaskInfo()
		{		
			mTaskInfo.SetName(this.TaskName);
			mTaskInfo.SetPriority(this.TaskPriority);
			mTaskInfo.SetAssignedTo(this.TaskAssignedTo);
			mTaskInfo.SetPercentComplete(this.TaskPercentage);
			mTaskInfo.SetComment(this.TaskComments);			
			mTaskInfo.SetDueDate(MOG_Time.GetVersionTimestamp());
		}
		#endregion
		#region Populators
		/// <summary>
		/// Populate the assign to comboBox
		/// </summary>
		private void PopulateAssignTo()
		{
			ArrayList departments = MOG_DBProjectAPI.GetDepartments();

			if (departments != null)
			{
				foreach (string departmentId in departments)
				{
					// Add the department
					TaskUsersComboBox.Items.Add("Department{" + departmentId + "}");

					ArrayList users = MOG_DBProjectAPI.GetDepartmentUsers(departmentId);
								
					if (users != null)
					{
						foreach (string user in users)
						{
							// Add the user
							TaskUsersComboBox.Items.Add(user);
						}
					}
				}
			}
		}

		/// <summary>
		/// Populate the priorities
		/// </summary>
		private void PopulatePriority()
		{
			string []priorities = {"Critical", "High", "Medium", "Low"};

			foreach (string priority in priorities)
			{
				TaskPriorityComboBox.Items.Add(priority);
			}

			TaskPriorityComboBox.Text = "Medium";
		}
		#endregion

		/// <summary>
		/// Handle percent track bar sliding
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TaskPercentageTrackBar_Scroll(object sender, System.EventArgs e)
		{
			// Update the percentage field
			PercentCompleteValueLabel.Text = string.Concat(PercentCompleteTrackBar.Value.ToString(), " %");
		}
		
		/// <summary>
		/// Handle new date pick in dataTime picker
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TaskDueDateTimePicker_ValueChanged(object sender, System.EventArgs e)
		{
			DateTimePicker item = (DateTimePicker)sender;
			mTaskInfo.SetDueDate(MOG_Time.GetVersionTimestamp());
		}

		/// <summary>
		/// Enable / Disable all child controls of this main control when its enabled flag is changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MogControl_TaskEditor_EnabledChanged(object sender, System.EventArgs e)
		{
			foreach( Control control in TaskEditorPanel.Controls )
			{
				control.Enabled = TaskEditorPanel.Enabled;
			}
		}

		/// <summary>
		/// Put us into view only mode if the EditableMode flag is not set to true
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MogControl_TaskEditor_Load(object sender, System.EventArgs e)
		{
			if (!this.EditableMode)
			{
				this.TaskNameTextBox.ReadOnly = true;
				this.TaskCommentsHistoryRichTextBox.ReadOnly = true;
				this.TaskCommentsRichTextBox.ReadOnly = true;
				this.TaskDueDateTimePicker.Enabled = false;
				this.TaskPriorityComboBox.Enabled = false;
				this.TaskUsersComboBox.Enabled = false;
			}
			else
			{
				this.TaskNameTextBox.ReadOnly = false;
				this.TaskCommentsHistoryRichTextBox.ReadOnly = true;
				this.TaskCommentsRichTextBox.ReadOnly = false;
				this.TaskDueDateTimePicker.Enabled = true;
				this.TaskPriorityComboBox.Enabled = true;
				this.TaskUsersComboBox.Enabled = true;
			}
		}
	}
}
