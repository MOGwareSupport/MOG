using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.INI;
using MOG.REPORT;
using MOG.DOSUTILS;
using MOG.PROMPT;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for CustomToolOptionsForm.
	/// </summary>
	public class CustomToolOptionsForm : System.Windows.Forms.Form
	{
		private string mDialogInfoFilename;
		private ArrayList mDynamicControls;

		private System.Windows.Forms.Button ToolOkButton;
		private System.Windows.Forms.Button ToolCancelButton;
		private System.Windows.Forms.Panel ControlsPanel;
		private System.Windows.Forms.PictureBox pictureBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public CustomToolOptionsForm(string dialogInfo)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mDialogInfoFilename = dialogInfo;
			mDynamicControls = new ArrayList();
			InitializeDialog();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CustomToolOptionsForm));
			this.ToolOkButton = new System.Windows.Forms.Button();
			this.ToolCancelButton = new System.Windows.Forms.Button();
			this.ControlsPanel = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// ToolOkButton
			// 
			this.ToolOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ToolOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ToolOkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ToolOkButton.Location = new System.Drawing.Point(232, 168);
			this.ToolOkButton.Name = "ToolOkButton";
			this.ToolOkButton.TabIndex = 0;
			this.ToolOkButton.Text = "OK";
			// 
			// ToolCancelButton
			// 
			this.ToolCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ToolCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.ToolCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ToolCancelButton.Location = new System.Drawing.Point(152, 168);
			this.ToolCancelButton.Name = "ToolCancelButton";
			this.ToolCancelButton.TabIndex = 1;
			this.ToolCancelButton.Text = "Cancel";
			// 
			// ControlsPanel
			// 
			this.ControlsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.ControlsPanel.Location = new System.Drawing.Point(8, 8);
			this.ControlsPanel.Name = "ControlsPanel";
			this.ControlsPanel.Size = new System.Drawing.Size(296, 152);
			this.ControlsPanel.TabIndex = 2;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(16, 168);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(16, 16);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Visible = false;
			// 
			// CustomToolOptionsForm
			// 
			this.AcceptButton = this.ToolOkButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.ToolCancelButton;
			this.ClientSize = new System.Drawing.Size(312, 197);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.ControlsPanel);
			this.Controls.Add(this.ToolCancelButton);
			this.Controls.Add(this.ToolOkButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "CustomToolOptionsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Tool Name Here";
			this.ResumeLayout(false);

		}
		#endregion

		private void InitializeDialog()
		{
			if (DosUtils.FileExist(mDialogInfoFilename))
			{
				MOG_Ini dialogInfo = new MOG_Ini(mDialogInfoFilename);

				// Init the controls
				if (dialogInfo.SectionExist("Controls"))
				{
					int Y = 8;
						
					for (int i = 0; i < dialogInfo.CountKeys("Controls"); i++)
					{
						string control = dialogInfo.GetKeyNameByIndexSLOW("Controls", i);
						string controlSection = dialogInfo.GetKeyByIndexSLOW("Controls", i);
						switch (control.ToLower())
						{
							case "toggleoptions":
								Y = CreateToggleCroupControl(Y, dialogInfo, controlSection);
								break;
							case "combooptions":
								Y = CreateComboControls(Y, dialogInfo, controlSection);								
								break;
							case "editoptions":
								Y = CreateEditControls(Y, dialogInfo, controlSection);								
								break;
						}
					}

					// Make sure our form is tall enough to handle the new controls
					if (Height < Y)
					{
						Height = Y + 80;
					}					
				}
			}
		}

		private int CreateComboControls(int startY, MOG_Ini dialogInfo, string section)
		{
			int x = 8;
			int y = startY;

			SuspendLayout();
			for (int i = 0; i < dialogInfo.CountKeys(section); i++)
			{
				string key = dialogInfo.GetKeyNameByIndexSLOW(section, i);
				string keySection = section + "." + key;
				
				if (dialogInfo.SectionExist(keySection))
				{
					if (dialogInfo.KeyExist(keySection, "Description"))
					{
						y = CreateLabelControl(x, y, dialogInfo.GetString(keySection, "Description"));
					}
					else
					{
						MOG_Prompt.PromptMessage("Custom Tool", "Cound not correctly create ComboControl due to missing (Description) field in info", Environment.StackTrace);
					}

					if (dialogInfo.KeyExist(keySection, "range"))
					{
						if (dialogInfo.KeyExist(keySection, "defaultValue"))
						{
							y = CreateComboBoxControl(x, y, dialogInfo.GetString(keySection, "range"), dialogInfo.GetString(keySection, "defaultValue"));
						}
						else
						{
							y = CreateComboBoxControl(x, y, dialogInfo.GetString(keySection, "range"), "0");
						}
					}
					else
					{
						MOG_Prompt.PromptMessage("Custom Tool", "Cound not correctly create ComboControl due to missing (Range) field in info", Environment.StackTrace);
					}
				}
			}

			ResumeLayout(false);
			
			return y;
		}
        
		private int CreateEditControls(int startY, MOG_Ini dialogInfo, string section)
		{
			int x = 8;
			int y = startY;

			SuspendLayout();
			for (int i = 0; i < dialogInfo.CountKeys(section); i++)
			{
				string key = dialogInfo.GetKeyNameByIndexSLOW(section, i);
				string keySection = section + "." + key;
				
				if (dialogInfo.SectionExist(keySection))
				{
					if (dialogInfo.KeyExist(keySection, "Description"))
					{
						y = CreateLabelControl(x, y, dialogInfo.GetString(keySection, "Description"));
					}
					else
					{
						MOG_Prompt.PromptMessage("Custom Tool", "Cound not correctly create ComboControl due to missing (Description) field in info", Environment.StackTrace);
					}

					if (dialogInfo.KeyExist(keySection, "defaultValue"))
					{
						y = CreateEditControl(x, y, dialogInfo.GetString(keySection, "defaultValue"));
					}
					else
					{
						y = CreateEditControl(x, y, "");
					}
				}
			}

			ResumeLayout(false);
			
			return y;
		}

		private int CreateEditControl(int x, int y, string defaultVal)
		{
			TextBox editBox = new TextBox();
			editBox.Location = new System.Drawing.Point(x, y);
			editBox.Name = defaultVal;
			editBox.Size = new System.Drawing.Size(64, 16);
			editBox.Text = defaultVal;
			editBox.Parent = ControlsPanel;

			if (editBox.Width > Width)
			{
				ControlsPanel.Width = editBox.Width + 10;
				Width = editBox.Width + 10;
			}

			mDynamicControls.Add(editBox);
			return y + editBox.Height;
		}

		private int CreateLabelControl(int x, int y, string name)
		{
			Label label = new Label();
			label.Location = new System.Drawing.Point(x, y);
			label.Name = name;
			label.Size = new System.Drawing.Size(MeasureString(name, label.Font), 16);
			label.Text = name;
			label.Parent = ControlsPanel;

			if (label.Width > Width)
			{
				ControlsPanel.Width = label.Width + 10;
				Width = label.Width + 10;
			}

			mDynamicControls.Add(label);
			return y + label.Height;
		}

		private int CreateComboBoxControl(int x, int y, string name, string defaultVal)
		{
			ComboBox comboBox = new ComboBox();
			comboBox.Location = new System.Drawing.Point(x, y);
			comboBox.Name = name;
			comboBox.Size = new System.Drawing.Size(MeasureString(name, comboBox.Font), 24);
			comboBox.Text = name;
			comboBox.Parent = ControlsPanel;

			if (comboBox.Width > Width)
			{
				ControlsPanel.Width = comboBox.Width + 10;
				Width = comboBox.Width + 10;
			}

			string []range = name.Trim().Split(",".ToCharArray());
			foreach (string val in range)
			{
				comboBox.Items.Add(val);
			}

			comboBox.SelectedIndex = Convert.ToInt32(defaultVal);

			mDynamicControls.Add(comboBox);

			return y + comboBox.Height;
		}		

		private int CreateToggleCroupControl(int startY, MOG_Ini dialogInfo, string section)
		{
			SuspendLayout();

			GroupBox groupBox = new System.Windows.Forms.GroupBox();

			groupBox.Location = new System.Drawing.Point(8, startY);
			groupBox.Name = section;
			groupBox.TabIndex = 2;
			groupBox.TabStop = false;
			groupBox.Text = section;
			groupBox.Visible = true;
			groupBox.Parent = ControlsPanel;

			groupBox.SuspendLayout();
			
			int X = 5;
			int Y = 12;

			//Graphics Gdi = Graphics.FromImage(pictureBox1.Image);

			for (int i = 0; i < dialogInfo.CountKeys(section); i++)
			{
				RadioButton radioButton = new System.Windows.Forms.RadioButton();

				string option = dialogInfo.GetKeyNameByIndexSLOW(section, i);
				string command = dialogInfo.GetKeyByIndexSLOW(section, i);

				radioButton.Location = new System.Drawing.Point(X, Y);
				radioButton.Name = command;
				radioButton.TabIndex = 0;
				radioButton.FlatStyle = FlatStyle.System;
				radioButton.Text = option;
				radioButton.Visible = true;
				radioButton.Parent = groupBox;

				// Measure string.
				radioButton.Width = MeasureString(option, radioButton.Font);
				if (radioButton.Width > groupBox.Width)
				{
					groupBox.Width = radioButton.Width + 10;
				}

				if (groupBox.Width > Width)
				{
					Width = groupBox.Width + 10;
				}

				groupBox.Controls.Add(radioButton);
				mDynamicControls.Add(radioButton);
			
				Y += radioButton.Height;
			}

			groupBox.Height = Y + 5;
			groupBox.ResumeLayout(false);
			ResumeLayout(false);
			mDynamicControls.Add(groupBox);

			return startY + groupBox.Height;
		}

		private int MeasureString(string text, Font font)
		{
			Graphics Gdi = Graphics.FromImage(pictureBox1.Image);

			// Measure string.
			SizeF stringSize = new SizeF();
			stringSize = Gdi.MeasureString(text, font);

			return (int)stringSize.Width + 10;
		}
	}
}
