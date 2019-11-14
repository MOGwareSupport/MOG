using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Text.RegularExpressions;

using MOG.PROMPT;
using MOG;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for ToolBoxLabelForm.
	/// </summary>
	public class ToolBoxLabelForm : System.Windows.Forms.Form
	{
		private ToolBox mToolBox;

		private System.Windows.Forms.Label labelNameLabel;
		private System.Windows.Forms.Button EditOkButton;
		private System.Windows.Forms.Button EditCancelButton;
		private System.Windows.Forms.TextBox labelNameTextBox;
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components;
	
		public TextBox LabelNameTextBox
		{
			get { return labelNameTextBox; }
		}

		public ToolBoxLabelForm(ToolBox toolBox)
		{
			
			this.mToolBox = toolBox;
			
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public void SetControlEnableBasedOnPrivilege(ToolBoxControlLocation location)
		{
			bool enable = true;

			switch (location)
			{
				case ToolBoxControlLocation.Project:
					enable = mToolBox.mPrivileges.GetUserPrivilege(mToolBox.mCurrentUserName, MOG_PRIVILEGE.ConfigureProjectCustomTools);
					break;
				case ToolBoxControlLocation.Department:
					enable = mToolBox.mPrivileges.GetUserPrivilege(mToolBox.mCurrentUserName, MOG_PRIVILEGE.ConfigureDepartmentCustomTools);
					break;
				case ToolBoxControlLocation.User:
					enable = mToolBox.mPrivileges.GetUserPrivilege(mToolBox.mCurrentUserName, MOG_PRIVILEGE.ConfigureUserCustomTools);
					break;
			}

			foreach (Control control in Controls)
			{
				if (control.Name != "EditOkButton" && control.Name != "EditCancelButton")
				{
					control.Enabled = enable;
				}
			}
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolBoxLabelForm));
			this.labelNameLabel = new System.Windows.Forms.Label();
			this.labelNameTextBox = new System.Windows.Forms.TextBox();
			this.EditOkButton = new System.Windows.Forms.Button();
			this.EditCancelButton = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// labelNameLabel
			// 
			this.labelNameLabel.Location = new System.Drawing.Point(8, 8);
			this.labelNameLabel.Name = "labelNameLabel";
			this.labelNameLabel.Size = new System.Drawing.Size(100, 23);
			this.labelNameLabel.TabIndex = 0;
			this.labelNameLabel.Text = "Label Name";
			// 
			// labelNameTextBox
			// 
			this.labelNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.labelNameTextBox.Location = new System.Drawing.Point(8, 32);
			this.labelNameTextBox.Name = "labelNameTextBox";
			this.labelNameTextBox.Size = new System.Drawing.Size(384, 20);
			this.labelNameTextBox.TabIndex = 1;
			this.toolTip.SetToolTip(this.labelNameTextBox, "Type the Label name, as you would like for it to display...");
			// 
			// EditOkButton
			// 
			this.EditOkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.EditOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.EditOkButton.Location = new System.Drawing.Point(116, 58);
			this.EditOkButton.Name = "EditOkButton";
			this.EditOkButton.Size = new System.Drawing.Size(72, 23);
			this.EditOkButton.TabIndex = 6;
			this.EditOkButton.Text = "OK";
			this.EditOkButton.Click += new System.EventHandler(this.EditOkButton_Click);
			// 
			// EditCancelButton
			// 
			this.EditCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.EditCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.EditCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.EditCancelButton.Location = new System.Drawing.Point(204, 58);
			this.EditCancelButton.Name = "EditCancelButton";
			this.EditCancelButton.Size = new System.Drawing.Size(72, 23);
			this.EditCancelButton.TabIndex = 5;
			this.EditCancelButton.Text = "Cancel";
			// 
			// ToolBoxLabelForm
			// 
			this.AcceptButton = this.EditOkButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.EditCancelButton;
			this.ClientSize = new System.Drawing.Size(400, 86);
			this.Controls.Add(this.EditOkButton);
			this.Controls.Add(this.EditCancelButton);
			this.Controls.Add(this.labelNameTextBox);
			this.Controls.Add(this.labelNameLabel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximumSize = new System.Drawing.Size(408, 120);
			this.MinimumSize = new System.Drawing.Size(408, 120);
			this.Name = "ToolBoxLabelForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create/Edit Label";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void EditOkButton_Click(object sender, System.EventArgs e)
		{
			if( !Regex.IsMatch( this.LabelNameTextBox.Text, "\\S"))
			{
				MOG_Prompt.PromptResponse("Missing Information", "You must enter at least one character (other "
					+" than whitespace) into the label, or click 'Cancel' to cancel...");
			}
			else
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

	}
}
