using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG.DOSUTILS;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_Server.MOG_ControlsLibrary.Common
{
	/// <summary>
	/// Summary description for MOGDialog.
	/// </summary>
	public class CallbackDialogForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public System.Windows.Forms.Label CallbackDialoglMessageLabel;
		public System.Windows.Forms.Button CallbackDialogCancelButton;
		private System.Windows.Forms.Button CallbackDialogOkButton;
		public System.Windows.Forms.ProgressBar CallbackDialogFilesProgressBar;
		public System.Windows.Forms.Label CallbackDialogFilenameLabel;
		public System.Windows.Forms.Button CallbackDialogOptional1Button;
		public System.Windows.Forms.Button CallbackDialogOptional2Button;
		public System.Windows.Forms.Button CallbackDialogOptional3Button;
		public System.Windows.Forms.Button CallbackDialogOptional4Button;

		public bool cancel;
		public int result;
		public string[]buttons;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		public System.Windows.Forms.ListView CallbackDialogListView;

		public CallbackDialogForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			cancel = false;
			result = -1;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CallbackDialogForm));
			this.CallbackDialoglMessageLabel = new System.Windows.Forms.Label();
			this.CallbackDialogCancelButton = new System.Windows.Forms.Button();
			this.CallbackDialogOkButton = new System.Windows.Forms.Button();
			this.CallbackDialogFilesProgressBar = new System.Windows.Forms.ProgressBar();
			this.CallbackDialogFilenameLabel = new System.Windows.Forms.Label();
			this.CallbackDialogOptional1Button = new System.Windows.Forms.Button();
			this.CallbackDialogOptional2Button = new System.Windows.Forms.Button();
			this.CallbackDialogOptional3Button = new System.Windows.Forms.Button();
			this.CallbackDialogOptional4Button = new System.Windows.Forms.Button();
			this.CallbackDialogListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// CallbackDialoglMessageLabel
			// 
			this.CallbackDialoglMessageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.CallbackDialoglMessageLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CallbackDialoglMessageLabel.Location = new System.Drawing.Point(8, 0);
			this.CallbackDialoglMessageLabel.Name = "CallbackDialoglMessageLabel";
			this.CallbackDialoglMessageLabel.Size = new System.Drawing.Size(369, 48);
			this.CallbackDialoglMessageLabel.TabIndex = 0;
			// 
			// CallbackDialogCancelButton
			// 
			this.CallbackDialogCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CallbackDialogCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CallbackDialogCancelButton.Location = new System.Drawing.Point(144, 119);
			this.CallbackDialogCancelButton.Name = "CallbackDialogCancelButton";
			this.CallbackDialogCancelButton.Size = new System.Drawing.Size(76, 23);
			this.CallbackDialogCancelButton.TabIndex = 1;
			this.CallbackDialogCancelButton.Text = "Cancel";
			this.CallbackDialogCancelButton.Visible = false;
			this.CallbackDialogCancelButton.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// CallbackDialogOkButton
			// 
			this.CallbackDialogOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CallbackDialogOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CallbackDialogOkButton.Location = new System.Drawing.Point(304, 119);
			this.CallbackDialogOkButton.Name = "CallbackDialogOkButton";
			this.CallbackDialogOkButton.Size = new System.Drawing.Size(68, 23);
			this.CallbackDialogOkButton.TabIndex = 2;
			this.CallbackDialogOkButton.Text = "Ok";
			this.CallbackDialogOkButton.Visible = false;
			// 
			// CallbackDialogFilesProgressBar
			// 
			this.CallbackDialogFilesProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.CallbackDialogFilesProgressBar.Location = new System.Drawing.Point(8, 95);
			this.CallbackDialogFilesProgressBar.Name = "CallbackDialogFilesProgressBar";
			this.CallbackDialogFilesProgressBar.Size = new System.Drawing.Size(361, 16);
			this.CallbackDialogFilesProgressBar.TabIndex = 3;
			this.CallbackDialogFilesProgressBar.Visible = false;
			// 
			// CallbackDialogFilenameLabel
			// 
			this.CallbackDialogFilenameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.CallbackDialogFilenameLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CallbackDialogFilenameLabel.Location = new System.Drawing.Point(8, 48);
			this.CallbackDialogFilenameLabel.Name = "CallbackDialogFilenameLabel";
			this.CallbackDialogFilenameLabel.Size = new System.Drawing.Size(361, 56);
			this.CallbackDialogFilenameLabel.TabIndex = 4;
			this.CallbackDialogFilenameLabel.Text = "Filename to be copied . . .";
			this.CallbackDialogFilenameLabel.Visible = false;
			// 
			// CallbackDialogOptional1Button
			// 
			this.CallbackDialogOptional1Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CallbackDialogOptional1Button.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CallbackDialogOptional1Button.Location = new System.Drawing.Point(20, 119);
			this.CallbackDialogOptional1Button.Name = "CallbackDialogOptional1Button";
			this.CallbackDialogOptional1Button.Size = new System.Drawing.Size(68, 23);
			this.CallbackDialogOptional1Button.TabIndex = 5;
			this.CallbackDialogOptional1Button.Text = "Unknown";
			this.CallbackDialogOptional1Button.Visible = false;
			this.CallbackDialogOptional1Button.Click += new System.EventHandler(this.CallbackDialogOptionalButton_Click);
			// 
			// CallbackDialogOptional2Button
			// 
			this.CallbackDialogOptional2Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CallbackDialogOptional2Button.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CallbackDialogOptional2Button.Location = new System.Drawing.Point(95, 119);
			this.CallbackDialogOptional2Button.Name = "CallbackDialogOptional2Button";
			this.CallbackDialogOptional2Button.Size = new System.Drawing.Size(68, 23);
			this.CallbackDialogOptional2Button.TabIndex = 6;
			this.CallbackDialogOptional2Button.Text = "Unknown";
			this.CallbackDialogOptional2Button.Visible = false;
			this.CallbackDialogOptional2Button.Click += new System.EventHandler(this.CallbackDialogOptionalButton_Click);
			// 
			// CallbackDialogOptional3Button
			// 
			this.CallbackDialogOptional3Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CallbackDialogOptional3Button.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CallbackDialogOptional3Button.Location = new System.Drawing.Point(170, 119);
			this.CallbackDialogOptional3Button.Name = "CallbackDialogOptional3Button";
			this.CallbackDialogOptional3Button.Size = new System.Drawing.Size(68, 23);
			this.CallbackDialogOptional3Button.TabIndex = 7;
			this.CallbackDialogOptional3Button.Text = "Unknown";
			this.CallbackDialogOptional3Button.Visible = false;
			this.CallbackDialogOptional3Button.Click += new System.EventHandler(this.CallbackDialogOptionalButton_Click);
			// 
			// CallbackDialogOptional4Button
			// 
			this.CallbackDialogOptional4Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CallbackDialogOptional4Button.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CallbackDialogOptional4Button.Location = new System.Drawing.Point(245, 119);
			this.CallbackDialogOptional4Button.Name = "CallbackDialogOptional4Button";
			this.CallbackDialogOptional4Button.Size = new System.Drawing.Size(68, 23);
			this.CallbackDialogOptional4Button.TabIndex = 8;
			this.CallbackDialogOptional4Button.Text = "Unknown";
			this.CallbackDialogOptional4Button.Visible = false;
			this.CallbackDialogOptional4Button.Click += new System.EventHandler(this.CallbackDialogOptionalButton_Click);
			// 
			// CallbackDialogListView
			// 
			this.CallbackDialogListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.CallbackDialogListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																									 this.columnHeader1,
																									 this.columnHeader2});
			this.CallbackDialogListView.FullRowSelect = true;
			this.CallbackDialogListView.Location = new System.Drawing.Point(8, 8);
			this.CallbackDialogListView.MultiSelect = false;
			this.CallbackDialogListView.Name = "CallbackDialogListView";
			this.CallbackDialogListView.Size = new System.Drawing.Size(360, 104);
			this.CallbackDialogListView.TabIndex = 9;
			this.CallbackDialogListView.View = System.Windows.Forms.View.Details;
			this.CallbackDialogListView.Visible = false;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Asset Name";
			this.columnHeader1.Width = 203;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Comments";
			this.columnHeader2.Width = 152;
			// 
			// CallbackDialogForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(384, 149);
			this.ControlBox = false;
			this.Controls.Add(this.CallbackDialogListView);
			this.Controls.Add(this.CallbackDialogOptional4Button);
			this.Controls.Add(this.CallbackDialogOptional3Button);
			this.Controls.Add(this.CallbackDialogOptional2Button);
			this.Controls.Add(this.CallbackDialogOptional1Button);
			this.Controls.Add(this.CallbackDialogFilesProgressBar);
			this.Controls.Add(this.CallbackDialogOkButton);
			this.Controls.Add(this.CallbackDialogCancelButton);
			this.Controls.Add(this.CallbackDialoglMessageLabel);
			this.Controls.Add(this.CallbackDialogFilenameLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CallbackDialogForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MOGDialog";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			cancel = true;
		}

		public void DialogInitialize(string title, string message, string button)
		{
			Text = title;

			CallbackDialogFilenameLabel.Visible = false;
			CallbackDialogFilesProgressBar.Visible = false;
			CallbackDialogCancelButton.Visible = true;

			CallbackDialoglMessageLabel.Text = message;
			if (button.Length != 0)
			{
				CallbackDialogCancelButton.Text = button;
			}
			else
			{
				CallbackDialogCancelButton.Visible = false;
			}

			Show();
		}
        
		public void DialogInitialize(string title, ArrayList message, string button)
		{
			Text = title;

			CallbackDialogFilenameLabel.Visible = false;
			CallbackDialogFilesProgressBar.Visible = false;
			CallbackDialogCancelButton.Visible = true;

			CallbackDialoglMessageLabel.Visible = false;

			// Check if the button string needs to be parsed
			if (button.IndexOf("/") != -1)
			{
				CallbackDialogCancelButton.Visible = false;

				string []buttons = button.Split(new Char[] {'/'});
				this.buttons = buttons;

				for (int i = 0; i < buttons.Length; i++)
				{
					switch (i)
					{
						case 0:
							CallbackDialogOptional1Button.Text = buttons[i];
							CallbackDialogOptional1Button.Visible = true;
							break;
						case 1:
							CallbackDialogOptional2Button.Text = buttons[i];
							CallbackDialogOptional2Button.Visible = true;
							break;
						case 2:
							CallbackDialogOptional3Button.Text = buttons[i];
							CallbackDialogOptional3Button.Visible = true;
							break;
						case 3:
							CallbackDialogOptional4Button.Text = buttons[i];
							CallbackDialogOptional4Button.Visible = true;
							break;
						default:
							// Error
							break;
					}
				}
			}
			else
			{
				if (button.Length != 0)
				{
					CallbackDialogCancelButton.Visible = true;
					CallbackDialogCancelButton.Text = button;
				}
				else
				{
					CallbackDialogCancelButton.Visible = false;
				}				
			}

			// Enable the multi item message window
			CallbackDialogListView.Visible = true;
			CallbackDialogListView.Items.Clear();
			CallbackDialogListView.BeginUpdate();
			foreach (string str in message)
			{
				string []parts = str.Split(",;".ToCharArray());
				if (parts != null && parts.Length >= 2)
				{
					ListViewItem item = new ListViewItem();
					item.Text = parts[0];
					item.SubItems.Add(parts[1]);
					CallbackDialogListView.Items.Add(item);
				}
			}
			CallbackDialogListView.EndUpdate();

			if (CallbackDialogListView.Items.Count > 10)
			{
				FormBorderStyle = FormBorderStyle.SizableToolWindow;
				this.Height += CallbackDialogListView.Items.Count;
			}
			
			Show();
		}

		public void DialogShowModal()
		{
			while(result == -1)
			{
				Application.DoEvents();
			}
		}

		public bool DialogProcess()
		{
			return cancel;
		}

		public void DialogUpdate(int percent, string description)
		{
			if (description.Length != 0)
			{
				CallbackDialogFilenameLabel.Visible = true;
				CallbackDialogFilenameLabel.Text = description;

				CallbackDialogFilesProgressBar.Visible = true;
				if (percent <= CallbackDialogFilesProgressBar.Maximum && percent >= CallbackDialogFilesProgressBar.Minimum)
				{
					CallbackDialogFilesProgressBar.Value = percent;
				}
			}

			Application.DoEvents();
		}

		public void DialogKill()
		{
			Dispose();
		}

		private void CallbackDialogOptionalButton_Click(object sender, System.EventArgs e)
		{
			string buttonString = ((Button)sender).Text;
			for (int i = 0; i < buttons.Length; i++)
			{
				if (string.Compare(buttons[i], buttonString, true) == 0)
				{
					result =  i;
				}
			}
		}
	}
}
