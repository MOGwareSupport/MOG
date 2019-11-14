using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG_ControlsLibrary.Common.MogControl_TaskEditor;


namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for NewTaskForm.
	/// </summary>
	public class NewTaskForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button TaskOkButton;
		private System.Windows.Forms.Button TaskCancelButton;
		private MogControl_TaskEditor mogControl_TaskEditor;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NewTaskForm(MOG_TaskInfo task)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mogControl_TaskEditor.Initialize(task);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(NewTaskForm));
			this.TaskOkButton = new System.Windows.Forms.Button();
			this.TaskCancelButton = new System.Windows.Forms.Button();
			this.mogControl_TaskEditor = new MOG_ControlsLibrary.Common.MogControl_TaskEditor.MogControl_TaskEditor();
			this.SuspendLayout();
			// 
			// TaskOkButton
			// 
			this.TaskOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.TaskOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.TaskOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TaskOkButton.Location = new System.Drawing.Point(456, 360);
			this.TaskOkButton.Name = "TaskOkButton";
			this.TaskOkButton.TabIndex = 1;
			this.TaskOkButton.Text = "Ok";
			this.TaskOkButton.Click += new System.EventHandler(this.TaskOkButton_Click);
			// 
			// TaskCancelButton
			// 
			this.TaskCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.TaskCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.TaskCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.TaskCancelButton.Location = new System.Drawing.Point(376, 360);
			this.TaskCancelButton.Name = "TaskCancelButton";
			this.TaskCancelButton.TabIndex = 2;
			this.TaskCancelButton.Text = "Cancel";
			// 
			// mogControl_TaskEditor
			// 
			this.mogControl_TaskEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mogControl_TaskEditor.EditableMode = true;
			this.mogControl_TaskEditor.Location = new System.Drawing.Point(0, 0);
			this.mogControl_TaskEditor.Name = "mogControl_TaskEditor";
			this.mogControl_TaskEditor.Size = new System.Drawing.Size(544, 360);
			this.mogControl_TaskEditor.TabIndex = 3;
			// 
			// NewTaskForm
			// 
			this.AcceptButton = this.TaskOkButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.TaskCancelButton;
			this.ClientSize = new System.Drawing.Size(544, 389);
			this.Controls.Add(this.mogControl_TaskEditor);
			this.Controls.Add(this.TaskCancelButton);
			this.Controls.Add(this.TaskOkButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "NewTaskForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create\\Edit Task";
			this.ResumeLayout(false);

		}
		#endregion		

		public MOG_TaskInfo MOGTaskInfo
		{
			get
			{
				return mogControl_TaskEditor.TaskInfo;
			}
		}

		public bool EditableMode
		{
			get 
			{
				return mogControl_TaskEditor.EditableMode;
			}

			set 
			{
				mogControl_TaskEditor.EditableMode = value;
			}
		}

		private void TaskOkButton_Click(object sender, System.EventArgs e)
		{
			// Update the taskInfo
			mogControl_TaskEditor.FinializeTaskInfo();
		}
	}
}
