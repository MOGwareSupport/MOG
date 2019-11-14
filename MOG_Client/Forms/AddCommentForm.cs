using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG_Client.Client_Utilities;
using MOG.PROPERTIES;

namespace MOG_Client.Forms
{
	public enum CommentType
	{
		Checkout,
		Checkin,
		Lock,
		UnLock,
	}
	
	/// <summary>
	/// Summary description for AddCommentForm.
	/// </summary>
	public class AddCommentForm : System.Windows.Forms.Form
	{
		private CommentType mType;
		public System.Windows.Forms.RichTextBox CommentsRichTextBox;
		private System.Windows.Forms.Button CommentsOkButton;
		private System.Windows.Forms.Button CommentsCancelButton;
		private System.Windows.Forms.Label CommentsLabel;
		private System.Windows.Forms.CheckBox EditCheckbox;
		private CheckBox EditMaintainLockCheckBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public bool Edit
		{
			get { return EditCheckbox.Checked; }
			set { EditCheckbox.Checked = value; }
		}

		private bool mMaintainLock;
		public bool MaintainLock
		{
			get { return mMaintainLock; }
			set { mMaintainLock = value; }
		}	

		public AddCommentForm(CommentType type, bool enableEditCheckbox)
			: this(type, null, enableEditCheckbox, false, false)
		{

		}

		public AddCommentForm(CommentType type, string filename, bool enableEditCheckbox)
			: this(type, filename, enableEditCheckbox, false, false)
		{

		}

		public AddCommentForm(CommentType type, bool enableEditCheckbox, bool enableMaintainLock)
			: this(type, null, enableEditCheckbox, enableMaintainLock, false)
		{

		}

		public AddCommentForm(CommentType type, bool enableEditCheckbox, bool enableMaintainLock, bool checkMaintainLock)
			: this(type, null, enableEditCheckbox, enableMaintainLock, checkMaintainLock)
		{

		}

		public AddCommentForm(CommentType type, string filename, bool enableEditCheckbox, bool enableMaintainLock, bool checkMaintainLock)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			Text = type.ToString();

			if (!String.IsNullOrEmpty(filename))
			{
				Text += ": " + filename;
			}

			mType = type;

			if (mType == CommentType.Checkout)
			{
				CommentsLabel.Text = "Why are you checking out these files?";
			}
			else if (mType == CommentType.Checkin)
			{
				CommentsLabel.Text = "What have you done to these files?";
			}
			else if (mType == CommentType.Lock)
			{
				CommentsLabel.Text = "Why are you requesting this lock?";
			}

			EditCheckbox.Checked = false;
			EditCheckbox.Enabled = enableEditCheckbox;
			EditCheckbox.Visible = enableEditCheckbox;

			// Set the status of the EditMaintainLockCheckBox visible property
			EditMaintainLockCheckBox.Enabled = enableMaintainLock;
			// Should we force check maintain lock?
			if (checkMaintainLock)
			{
				EditMaintainLockCheckBox.Checked = checkMaintainLock;
			}

			guiUserPrefs.LoadDynamic_LayoutPrefs("CheckoutComments:" + mType.ToString(), this);
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

		public bool IsEditChecked()
		{
			return EditCheckbox.Checked;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddCommentForm));
			this.CommentsRichTextBox = new System.Windows.Forms.RichTextBox();
			this.CommentsLabel = new System.Windows.Forms.Label();
			this.CommentsOkButton = new System.Windows.Forms.Button();
			this.CommentsCancelButton = new System.Windows.Forms.Button();
			this.EditCheckbox = new System.Windows.Forms.CheckBox();
			this.EditMaintainLockCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// CommentsRichTextBox
			// 
			this.CommentsRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.CommentsRichTextBox.Location = new System.Drawing.Point(8, 27);
			this.CommentsRichTextBox.Name = "CommentsRichTextBox";
			this.CommentsRichTextBox.Size = new System.Drawing.Size(309, 143);
			this.CommentsRichTextBox.TabIndex = 0;
			this.CommentsRichTextBox.Text = "";
			// 
			// CommentsLabel
			// 
			this.CommentsLabel.Location = new System.Drawing.Point(8, 8);
			this.CommentsLabel.Name = "CommentsLabel";
			this.CommentsLabel.Size = new System.Drawing.Size(309, 16);
			this.CommentsLabel.TabIndex = 1;
			this.CommentsLabel.Text = "Comments";
			// 
			// CommentsOkButton
			// 
			this.CommentsOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CommentsOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.CommentsOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CommentsOkButton.Location = new System.Drawing.Point(326, 115);
			this.CommentsOkButton.Name = "CommentsOkButton";
			this.CommentsOkButton.Size = new System.Drawing.Size(75, 23);
			this.CommentsOkButton.TabIndex = 2;
			this.CommentsOkButton.Text = "Ok";
			// 
			// CommentsCancelButton
			// 
			this.CommentsCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CommentsCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CommentsCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CommentsCancelButton.Location = new System.Drawing.Point(326, 147);
			this.CommentsCancelButton.Name = "CommentsCancelButton";
			this.CommentsCancelButton.Size = new System.Drawing.Size(75, 23);
			this.CommentsCancelButton.TabIndex = 3;
			this.CommentsCancelButton.Text = "Cancel";
			// 
			// EditCheckbox
			// 
			this.EditCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.EditCheckbox.Location = new System.Drawing.Point(8, 176);
			this.EditCheckbox.Name = "EditCheckbox";
			this.EditCheckbox.Size = new System.Drawing.Size(120, 17);
			this.EditCheckbox.TabIndex = 4;
			this.EditCheckbox.Text = "Open for Editing";
			// 
			// EditMaintainLockCheckBox
			// 
			this.EditMaintainLockCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.EditMaintainLockCheckBox.AutoSize = true;
			this.EditMaintainLockCheckBox.Location = new System.Drawing.Point(312, 8);
			this.EditMaintainLockCheckBox.Name = "EditMaintainLockCheckBox";
			this.EditMaintainLockCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.EditMaintainLockCheckBox.Size = new System.Drawing.Size(89, 17);
			this.EditMaintainLockCheckBox.TabIndex = 5;
			this.EditMaintainLockCheckBox.Text = "Maintain lock";
			this.EditMaintainLockCheckBox.UseVisualStyleBackColor = true;
			this.EditMaintainLockCheckBox.Enabled = false;
			this.EditMaintainLockCheckBox.CheckedChanged += new System.EventHandler(this.EditMaintainLockCheckBox_CheckedChanged);
			// 
			// AddCommentForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(413, 196);
			this.Controls.Add(this.EditMaintainLockCheckBox);
			this.Controls.Add(this.EditCheckbox);
			this.Controls.Add(this.CommentsCancelButton);
			this.Controls.Add(this.CommentsOkButton);
			this.Controls.Add(this.CommentsLabel);
			this.Controls.Add(this.CommentsRichTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddCommentForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "AddCommentForm";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.AddCommentForm_Closing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void AddCommentForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			guiUserPrefs.SaveDynamic_LayoutPrefs("CheckoutComments:" + mType.ToString(), this);
		}

		private void EditMaintainLockCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			mMaintainLock = EditMaintainLockCheckBox.Checked;
		}
	}
}
