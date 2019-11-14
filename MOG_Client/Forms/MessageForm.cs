using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERMESSAGE;

using MOG_Client.Client_Gui;

namespace MOG_Client
{
	/// <summary>
	/// Summary description for MessageForm.
	/// </summary>
	public class MessageForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel MessageTopPanel;
		private System.Windows.Forms.Label MessageToLabel;
		private System.Windows.Forms.TextBox MessageToTextBox;
		private System.Windows.Forms.Panel MessageBccPanel;
		private System.Windows.Forms.Label MessageBccLabel;
		private System.Windows.Forms.TextBox MessageBccTextBox;
		private System.Windows.Forms.Panel MessageSubjectPanel;
		private System.Windows.Forms.Label MessageSubjectLabel;
		private System.Windows.Forms.TextBox MessageSubjectTextBox;
		private System.Windows.Forms.RichTextBox MessageRichTextBox;
		private System.Windows.Forms.Panel MessageToPanel;
		private System.Windows.Forms.ToolBar MessageToolBar;
		private System.Windows.Forms.ToolBarButton MessageSendToolBarButton;
		private System.Windows.Forms.ImageList MessageImageList;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolBarButton MessageAttachToolBarButton;
		private System.Windows.Forms.Panel MessageCcPanel;
		private System.Windows.Forms.Label MessageCcLabel;
		private System.Windows.Forms.TextBox MessageCcTextBox;

		private BASE mMog;
		private System.Windows.Forms.OpenFileDialog MessageAttachOpenFileDialog;
		private System.Windows.Forms.Panel MessageAttachmentPanel;
		private System.Windows.Forms.Label MessageAttachLabel;
		private System.Windows.Forms.TextBox MessageAttachTextBox;
		private MOG_ControllerMessage mMessage;

		public MessageForm(MogMainForm main)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mMog = main.gMog;
			mMessage = new MOG_ControllerMessage(mMog);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MessageForm));
			this.MessageTopPanel = new System.Windows.Forms.Panel();
			this.MessageAttachmentPanel = new System.Windows.Forms.Panel();
			this.MessageAttachTextBox = new System.Windows.Forms.TextBox();
			this.MessageAttachLabel = new System.Windows.Forms.Label();
			this.MessageSubjectPanel = new System.Windows.Forms.Panel();
			this.MessageSubjectTextBox = new System.Windows.Forms.TextBox();
			this.MessageSubjectLabel = new System.Windows.Forms.Label();
			this.MessageBccPanel = new System.Windows.Forms.Panel();
			this.MessageBccTextBox = new System.Windows.Forms.TextBox();
			this.MessageBccLabel = new System.Windows.Forms.Label();
			this.MessageCcPanel = new System.Windows.Forms.Panel();
			this.MessageCcTextBox = new System.Windows.Forms.TextBox();
			this.MessageCcLabel = new System.Windows.Forms.Label();
			this.MessageToolBar = new System.Windows.Forms.ToolBar();
			this.MessageSendToolBarButton = new System.Windows.Forms.ToolBarButton();
			this.MessageAttachToolBarButton = new System.Windows.Forms.ToolBarButton();
			this.MessageImageList = new System.Windows.Forms.ImageList(this.components);
			this.MessageToPanel = new System.Windows.Forms.Panel();
			this.MessageToTextBox = new System.Windows.Forms.TextBox();
			this.MessageToLabel = new System.Windows.Forms.Label();
			this.MessageRichTextBox = new System.Windows.Forms.RichTextBox();
			this.MessageAttachOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.MessageTopPanel.SuspendLayout();
			this.MessageAttachmentPanel.SuspendLayout();
			this.MessageSubjectPanel.SuspendLayout();
			this.MessageBccPanel.SuspendLayout();
			this.MessageCcPanel.SuspendLayout();
			this.MessageToPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// MessageTopPanel
			// 
			this.MessageTopPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																						  this.MessageAttachmentPanel,
																						  this.MessageSubjectPanel,
																						  this.MessageBccPanel,
																						  this.MessageCcPanel,
																						  this.MessageToolBar,
																						  this.MessageToPanel});
			this.MessageTopPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.MessageTopPanel.Name = "MessageTopPanel";
			this.MessageTopPanel.Size = new System.Drawing.Size(472, 144);
			this.MessageTopPanel.TabIndex = 0;
			// 
			// MessageAttachmentPanel
			// 
			this.MessageAttachmentPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																								 this.MessageAttachTextBox,
																								 this.MessageAttachLabel});
			this.MessageAttachmentPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.MessageAttachmentPanel.Location = new System.Drawing.Point(0, 96);
			this.MessageAttachmentPanel.Name = "MessageAttachmentPanel";
			this.MessageAttachmentPanel.Size = new System.Drawing.Size(472, 24);
			this.MessageAttachmentPanel.TabIndex = 6;
			// 
			// MessageAttachTextBox
			// 
			this.MessageAttachTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MessageAttachTextBox.Location = new System.Drawing.Point(72, 0);
			this.MessageAttachTextBox.Name = "MessageAttachTextBox";
			this.MessageAttachTextBox.Size = new System.Drawing.Size(400, 20);
			this.MessageAttachTextBox.TabIndex = 0;
			this.MessageAttachTextBox.Text = "";
			// 
			// MessageAttachLabel
			// 
			this.MessageAttachLabel.Dock = System.Windows.Forms.DockStyle.Left;
			this.MessageAttachLabel.Name = "MessageAttachLabel";
			this.MessageAttachLabel.Size = new System.Drawing.Size(72, 24);
			this.MessageAttachLabel.TabIndex = 0;
			this.MessageAttachLabel.Text = "Attachments";
			this.MessageAttachLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// MessageSubjectPanel
			// 
			this.MessageSubjectPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																							  this.MessageSubjectTextBox,
																							  this.MessageSubjectLabel});
			this.MessageSubjectPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.MessageSubjectPanel.Location = new System.Drawing.Point(0, 72);
			this.MessageSubjectPanel.Name = "MessageSubjectPanel";
			this.MessageSubjectPanel.Size = new System.Drawing.Size(472, 24);
			this.MessageSubjectPanel.TabIndex = 5;
			// 
			// MessageSubjectTextBox
			// 
			this.MessageSubjectTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MessageSubjectTextBox.Location = new System.Drawing.Point(72, 0);
			this.MessageSubjectTextBox.Name = "MessageSubjectTextBox";
			this.MessageSubjectTextBox.Size = new System.Drawing.Size(400, 20);
			this.MessageSubjectTextBox.TabIndex = 0;
			this.MessageSubjectTextBox.Text = "New Message";
			// 
			// MessageSubjectLabel
			// 
			this.MessageSubjectLabel.Dock = System.Windows.Forms.DockStyle.Left;
			this.MessageSubjectLabel.Name = "MessageSubjectLabel";
			this.MessageSubjectLabel.Size = new System.Drawing.Size(72, 24);
			this.MessageSubjectLabel.TabIndex = 0;
			this.MessageSubjectLabel.Text = "Subject";
			this.MessageSubjectLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// MessageBccPanel
			// 
			this.MessageBccPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																						  this.MessageBccTextBox,
																						  this.MessageBccLabel});
			this.MessageBccPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.MessageBccPanel.Location = new System.Drawing.Point(0, 48);
			this.MessageBccPanel.Name = "MessageBccPanel";
			this.MessageBccPanel.Size = new System.Drawing.Size(472, 24);
			this.MessageBccPanel.TabIndex = 4;
			// 
			// MessageBccTextBox
			// 
			this.MessageBccTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MessageBccTextBox.Location = new System.Drawing.Point(72, 0);
			this.MessageBccTextBox.Name = "MessageBccTextBox";
			this.MessageBccTextBox.Size = new System.Drawing.Size(400, 20);
			this.MessageBccTextBox.TabIndex = 0;
			this.MessageBccTextBox.Text = "";
			// 
			// MessageBccLabel
			// 
			this.MessageBccLabel.Dock = System.Windows.Forms.DockStyle.Left;
			this.MessageBccLabel.Name = "MessageBccLabel";
			this.MessageBccLabel.Size = new System.Drawing.Size(72, 24);
			this.MessageBccLabel.TabIndex = 0;
			this.MessageBccLabel.Text = "Bcc";
			this.MessageBccLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// MessageCcPanel
			// 
			this.MessageCcPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																						 this.MessageCcTextBox,
																						 this.MessageCcLabel});
			this.MessageCcPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.MessageCcPanel.Location = new System.Drawing.Point(0, 24);
			this.MessageCcPanel.Name = "MessageCcPanel";
			this.MessageCcPanel.Size = new System.Drawing.Size(472, 24);
			this.MessageCcPanel.TabIndex = 3;
			// 
			// MessageCcTextBox
			// 
			this.MessageCcTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MessageCcTextBox.Location = new System.Drawing.Point(72, 0);
			this.MessageCcTextBox.Name = "MessageCcTextBox";
			this.MessageCcTextBox.Size = new System.Drawing.Size(400, 20);
			this.MessageCcTextBox.TabIndex = 0;
			this.MessageCcTextBox.Text = "";
			// 
			// MessageCcLabel
			// 
			this.MessageCcLabel.Dock = System.Windows.Forms.DockStyle.Left;
			this.MessageCcLabel.Name = "MessageCcLabel";
			this.MessageCcLabel.Size = new System.Drawing.Size(72, 24);
			this.MessageCcLabel.TabIndex = 0;
			this.MessageCcLabel.Text = "Cc";
			this.MessageCcLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// MessageToolBar
			// 
			this.MessageToolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.MessageToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																							  this.MessageSendToolBarButton,
																							  this.MessageAttachToolBarButton});
			this.MessageToolBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.MessageToolBar.DropDownArrows = true;
			this.MessageToolBar.ImageList = this.MessageImageList;
			this.MessageToolBar.Location = new System.Drawing.Point(0, 119);
			this.MessageToolBar.Name = "MessageToolBar";
			this.MessageToolBar.ShowToolTips = true;
			this.MessageToolBar.Size = new System.Drawing.Size(472, 25);
			this.MessageToolBar.TabIndex = 7;
			this.MessageToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.MessageToolBar_ButtonClick);
			// 
			// MessageSendToolBarButton
			// 
			this.MessageSendToolBarButton.ImageIndex = 0;
			this.MessageSendToolBarButton.ToolTipText = "Send the message";
			// 
			// MessageAttachToolBarButton
			// 
			this.MessageAttachToolBarButton.ImageIndex = 1;
			this.MessageAttachToolBarButton.ToolTipText = "Attach file or folder";
			// 
			// MessageImageList
			// 
			this.MessageImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.MessageImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.MessageImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MessageImageList.ImageStream")));
			this.MessageImageList.TransparentColor = System.Drawing.Color.Magenta;
			// 
			// MessageToPanel
			// 
			this.MessageToPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																						 this.MessageToTextBox,
																						 this.MessageToLabel});
			this.MessageToPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.MessageToPanel.Name = "MessageToPanel";
			this.MessageToPanel.Size = new System.Drawing.Size(472, 24);
			this.MessageToPanel.TabIndex = 2;
			// 
			// MessageToTextBox
			// 
			this.MessageToTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MessageToTextBox.Location = new System.Drawing.Point(72, 0);
			this.MessageToTextBox.Name = "MessageToTextBox";
			this.MessageToTextBox.Size = new System.Drawing.Size(400, 20);
			this.MessageToTextBox.TabIndex = 0;
			this.MessageToTextBox.Text = "";
			// 
			// MessageToLabel
			// 
			this.MessageToLabel.Dock = System.Windows.Forms.DockStyle.Left;
			this.MessageToLabel.Name = "MessageToLabel";
			this.MessageToLabel.Size = new System.Drawing.Size(72, 24);
			this.MessageToLabel.TabIndex = 0;
			this.MessageToLabel.Text = "To";
			this.MessageToLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// MessageRichTextBox
			// 
			this.MessageRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MessageRichTextBox.Location = new System.Drawing.Point(0, 144);
			this.MessageRichTextBox.Name = "MessageRichTextBox";
			this.MessageRichTextBox.Size = new System.Drawing.Size(472, 150);
			this.MessageRichTextBox.TabIndex = 1;
			this.MessageRichTextBox.Text = "";
			// 
			// MessageAttachOpenFileDialog
			// 
			this.MessageAttachOpenFileDialog.Filter = "All Files (*.*)|*.*";
			this.MessageAttachOpenFileDialog.Multiselect = true;
			this.MessageAttachOpenFileDialog.Title = "Select files to be attached";
			this.MessageAttachOpenFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.MessageAttachOpenFileDialog_FileOk);
			// 
			// MessageForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(472, 294);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.MessageRichTextBox,
																		  this.MessageTopPanel});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MessageForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MessageForm";
			this.MessageTopPanel.ResumeLayout(false);
			this.MessageAttachmentPanel.ResumeLayout(false);
			this.MessageSubjectPanel.ResumeLayout(false);
			this.MessageBccPanel.ResumeLayout(false);
			this.MessageCcPanel.ResumeLayout(false);
			this.MessageToPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void MessageToolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			// Evaluate the Button property to determine which button was clicked.
			switch(MessageToolBar.Buttons.IndexOf(e.Button))
			{
				case 0:
					// Insert code to send the message
					string MessageText = "";

					foreach(string line in this.MessageRichTextBox.Lines)
					{
						if (MessageText.Length == 0)
						{
							MessageText = line;
						}
						else
						{
							MessageText = String.Concat(MessageText, "\r\n", line);
						}
					}
					mMessage.Send(MessageToTextBox.Text, MessageCcTextBox.Text, MessageBccTextBox.Text, MessageSubjectTextBox.Text, MessageText);
					mMessage.Close();

					// Close down this window
					this.Close();
					break; 
				case 1:
					// Insert code to attach files
					MessageAttachOpenFileDialog.ShowDialog(this);					
					break; 
				case 2:
					// Insert code to ?
					break; 
			}

		}

		public void Create()
		{
			// Populate the form with that data
			MessageToTextBox.Text = mMessage.GetTo();
			MessageCcTextBox.Text = mMessage.GetCC();
			MessageSubjectTextBox.Text = mMessage.GetSubject();
			MessageRichTextBox.Text = mMessage.GetMessage();
		}

		public void Open(ListViewItem message)
		{
			// Load the message
			string messageFile = message.SubItems[(int)guiAssetManager.MessageBoxColumns.FULLNAME].Text;
			mMessage.Open(messageFile);

			// Populate the form with that data
			MessageToTextBox.Text = mMessage.GetTo();
			MessageCcTextBox.Text = mMessage.GetCC();
			MessageSubjectTextBox.Text = mMessage.GetSubject();
			MessageRichTextBox.Text = mMessage.GetMessage();

			// Update the status
			mMessage.SetStatus("Read");
		}

		private void MessageAttachOpenFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (string file in MessageAttachOpenFileDialog.FileNames)
			{
				MOG_Filename filename = new MOG_Filename(file);
				mMessage.AttachmentAdd(filename);
				MessageAttachTextBox.Text = String.Concat(MessageAttachTextBox.Text, "\r\n", file);
			}
		}

	}
}
