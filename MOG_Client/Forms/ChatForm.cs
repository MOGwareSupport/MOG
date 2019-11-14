using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.COMMAND;

using MOG_Client.Client_Gui;

namespace MOG_Client
{
	/// <summary>
	/// Summary description for ChatForm.
	/// </summary>
	public class ChatForm : System.Windows.Forms.Form
	{
		private BASE mMog;
		private MogMainForm	mainForm;
		private string mActive;
		private string mInvite;
		private string mHandle;
		private guiChatManager mParent;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RichTextBox MessagesRichTextBox;
		private System.Windows.Forms.RichTextBox SendRichTextBox;
		private System.Windows.Forms.Button SendButton;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Label InvitedLabel;
		private System.Windows.Forms.Label ActiveLabel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChatForm(MogMainForm main, guiChatManager parent, string invited, string handle)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mMog = main.gMog;
			mainForm = main;
			mInvite = invited;
			mHandle = handle;
			mParent = parent;

			Initialize();
		}

		private void Initialize()
		{	
            UpdateUsers();
		}

		private void UpdateUsers()
		{
			InvitedLabel.Text = string.Concat("Invited: ", mInvite);
			ActiveLabel.Text = string.Concat("Active: ", mActive);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ChatForm));
			this.panel1 = new System.Windows.Forms.Panel();
			this.SendRichTextBox = new System.Windows.Forms.RichTextBox();
			this.SendButton = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.MessagesRichTextBox = new System.Windows.Forms.RichTextBox();
			this.panel5 = new System.Windows.Forms.Panel();
			this.ActiveLabel = new System.Windows.Forms.Label();
			this.InvitedLabel = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel4.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.SendRichTextBox,
																				 this.SendButton});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 198);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(544, 48);
			this.panel1.TabIndex = 0;
			// 
			// SendRichTextBox
			// 
			this.SendRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SendRichTextBox.Name = "SendRichTextBox";
			this.SendRichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.SendRichTextBox.Size = new System.Drawing.Size(488, 48);
			this.SendRichTextBox.TabIndex = 0;
			this.SendRichTextBox.Text = "";
			this.SendRichTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SendRichTextBox_KeyUp);
			// 
			// SendButton
			// 
			this.SendButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.SendButton.Location = new System.Drawing.Point(488, 0);
			this.SendButton.Name = "SendButton";
			this.SendButton.Size = new System.Drawing.Size(56, 48);
			this.SendButton.TabIndex = 1;
			this.SendButton.Text = "Send";
			this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
			// 
			// panel2
			// 
			this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.panel4});
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(544, 246);
			this.panel2.TabIndex = 1;
			// 
			// panel4
			// 
			this.panel4.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.MessagesRichTextBox,
																				 this.panel5,
																				 this.ActiveLabel,
																				 this.InvitedLabel});
			this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(544, 246);
			this.panel4.TabIndex = 1;
			// 
			// MessagesRichTextBox
			// 
			this.MessagesRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MessagesRichTextBox.HideSelection = false;
			this.MessagesRichTextBox.Location = new System.Drawing.Point(0, 32);
			this.MessagesRichTextBox.Name = "MessagesRichTextBox";
			this.MessagesRichTextBox.ReadOnly = true;
			this.MessagesRichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
			this.MessagesRichTextBox.Size = new System.Drawing.Size(544, 142);
			this.MessagesRichTextBox.TabIndex = 0;
			this.MessagesRichTextBox.Text = "";
			// 
			// panel5
			// 
			this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel5.Location = new System.Drawing.Point(0, 174);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(544, 72);
			this.panel5.TabIndex = 1;
			// 
			// ActiveLabel
			// 
			this.ActiveLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.ActiveLabel.Location = new System.Drawing.Point(0, 16);
			this.ActiveLabel.Name = "ActiveLabel";
			this.ActiveLabel.Size = new System.Drawing.Size(544, 16);
			this.ActiveLabel.TabIndex = 3;
			this.ActiveLabel.Text = "Active:";
			// 
			// InvitedLabel
			// 
			this.InvitedLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.InvitedLabel.Name = "InvitedLabel";
			this.InvitedLabel.Size = new System.Drawing.Size(544, 16);
			this.InvitedLabel.TabIndex = 2;
			this.InvitedLabel.Text = "Invited:";
			// 
			// ChatForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(544, 246);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.panel1,
																		  this.panel2});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ChatForm";
			this.Text = "ChatForm";
			this.TopMost = true;
			this.Closed += new System.EventHandler(this.ChatForm_Closed);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SendButton_Click(object sender, System.EventArgs e)
		{
			SendMessage();
		}

		private void SendMessage()
		{
			// Insert code to send the message
			string MessageText = "";

			foreach(string line in SendRichTextBox.Lines)
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

			mMog.InstantMessage(mHandle, mInvite, MessageText);
			SendRichTextBox.Text = "";
		}

		public void RecieveMessage(MOG_Command message)
		{
			// Check if this is a first time response
			if (mHandle.Length == 0)
			{
				mHandle = message.GetVersion();
			}
			else if (string.Compare(mHandle, message.GetVersion(), true) != 0)
			{
				// This is not our conversation
				return;
			}

			string userMessage = message.GetDescription();
			string user = message.GetUser();

			// Update title of box to show who is talking

			// Update message window
			MessagesRichTextBox.AppendText(string.Concat("<", user, ">: ", userMessage));

			// Add a carriage return if the message does not have one
			if(!userMessage.EndsWith("\n"))
			{
				MessagesRichTextBox.AppendText(string.Concat(MessagesRichTextBox.Text, "\n"));
			}

			// Make sure we scroll to the bottom
			MessagesRichTextBox.Focus();
			MessagesRichTextBox.ScrollToCaret();
			SendRichTextBox.Focus();

			// Update users
			mActive = message.GetDestination();
			UpdateUsers();

			// Play sound
			mainForm.mSoundManager.PlayStatusSound("ClientEvents", "RecieveChat");
		}

		private void SendRichTextBox_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				SendMessage();
			}
		}

		private void ChatForm_Closed(object sender, System.EventArgs e)
		{
			//mParent.ChatEnd(mHandle);
		}

		public string ChatGetHandle()
		{
			return mHandle;
		}
	}
}
