using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;

using MOG.INI;
using MOG.DOSUTILS;
using MOG_Client.Client_Mog_Utilities;
using MOG.CONTROLLER.CONTROLLERPROJECT;



namespace MOG_Client.Forms
{
	enum ETYPE 
	{
		STD_BUTTON,
		STD_BUTTON_EDIT,
		STD_FILETYPE_LIST,
		STD_FOLDERBROWSER
	};

	/// <summary>
	/// Summary description for ToolBox.
	/// </summary>
	public class ToolBox : System.Windows.Forms.UserControl
	{

		const int BUTTON_POS_STARTY=0;
		const int BUTTON_POS_STARTX=0;
		const int BUTTON_HEIGHT=23;
		const int BUTTON_WIDTH=75;

	
		ArrayList mControlsList = new ArrayList();
		private MogMainForm mainForm;				// Pointer to the main form
		string mPlatformName;

		private System.Windows.Forms.VScrollBar vScrollBar1;
		private System.Windows.Forms.ContextMenu MainContext;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.ContextMenu ButtonContext;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ToolBox()
		{

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			//	AllocateButtons();
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
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.MainContext = new System.Windows.Forms.ContextMenu();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.ButtonContext = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Location = new System.Drawing.Point(232, 0);
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(17, 184);
			this.vScrollBar1.TabIndex = 0;
			this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
			// 
			// MainContext
			// 
			this.MainContext.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.menuItem3,
																						this.menuItem4,
																						this.menuItem5,
																						this.menuItem6});
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 0;
			this.menuItem3.Text = "Add Button";
			this.menuItem3.Click += new System.EventHandler(this.AddNewButton);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 1;
			this.menuItem4.Text = "Add Parameter Button";
			this.menuItem4.Click += new System.EventHandler(this.AddNewParamButton);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 2;
			this.menuItem5.Text = "Add File Browser Button";
			this.menuItem5.Click += new System.EventHandler(this.AddFileTypeButton);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 3;
			this.menuItem6.Text = "Add Folder Browser Control";
			this.menuItem6.Click += new System.EventHandler(this.AddFolderBrowserButton);
			// 
			// ButtonContext
			// 
			this.ButtonContext.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						  this.menuItem1,
																						  this.menuItem2});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "Edit";
			this.menuItem1.Click += new System.EventHandler(this.EditControl);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Text = "Delete";
			this.menuItem2.Click += new System.EventHandler(this.DeleteControl);
			// 
			// ToolBox
			// 
			this.ContextMenu = this.MainContext;
			this.Controls.Add(this.vScrollBar1);
			this.Name = "ToolBox";
			this.Size = new System.Drawing.Size(256, 184);
			this.ResumeLayout(false);

		}
		#endregion


		//----------------------------------------------------------
		#region Windows Event Functions

		private void vScrollBar1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			// update button positions according to scroll percentage
			ScrollButtons(e.NewValue);		
		}
		#endregion
		//----------------------------------------------------------

		public void SetMogMainForm(MogMainForm main)
		{
			mainForm = main;
		}


		//------------------------------------------------------
		// Positions Controls according to scroll value
		private void ScrollButtons(int val)
		{
			int row_x = 0;
			int row_y = 0;
			int flag = 0;


			//......................................................
			// go through controls list, do STANDARD buttons first

			foreach(ControlDefinition def in mControlsList)		
			{
				if( (ETYPE)def.mEType ==  ETYPE.STD_BUTTON)
				{
					def.mButton.Location = new System.Drawing.Point(
						BUTTON_POS_STARTX + BUTTON_WIDTH * row_x,
						BUTTON_POS_STARTY - val + row_y * BUTTON_HEIGHT);

					row_x++;
					if(row_x == 3)
					{
						row_x = 0;
						row_y++;
					}

					flag = 1;
				}
			}

			//.....................................................
			// go through controls list, do USER EDIT buttons first

			if(flag == 1)
			{
				if(row_x != 0)		row_y++;
				//else				row_y+=2;
			}

			flag = 0;
			row_x = 0;
			foreach(ControlDefinition def in mControlsList)		
			{
				if( (ETYPE)def.mEType ==  ETYPE.STD_BUTTON_EDIT)
				{
					def.mTextBox.Location = new System.Drawing.Point(
						BUTTON_POS_STARTX,
						BUTTON_POS_STARTY - val + row_y * BUTTON_HEIGHT);

					def.mButton.Location = new System.Drawing.Point(
						BUTTON_POS_STARTX + def.mTextBox.Size.Width,
						BUTTON_POS_STARTY - val + row_y * BUTTON_HEIGHT);

					row_y++;

					flag = 1;
				}
			}

			//....................................................
			// go through controls list, do FileType buttons first

			if(flag == 1)
			{
		//		if(row_x == 0)	row_y++;
		//		else			row_y+=2;
			}
			row_x = 0;
			foreach(ControlDefinition def in mControlsList)		
			{
				if( (ETYPE)def.mEType ==  ETYPE.STD_FILETYPE_LIST)
				{
					def.mComboBox.Location = new System.Drawing.Point(
						BUTTON_POS_STARTX,
						BUTTON_POS_STARTY - val + row_y * BUTTON_HEIGHT);

					def.mButton.Location = new System.Drawing.Point(
						BUTTON_POS_STARTX + def.mComboBox.Size.Width,
						BUTTON_POS_STARTY - val + row_y * BUTTON_HEIGHT);

					row_y++;
				}
			}

			//....................................................
			// go through controls list, do FileType buttons first

		//	row_y++;
			row_x = 0;
			foreach(ControlDefinition def in mControlsList)		
			{
				if( (ETYPE)def.mEType ==  ETYPE.STD_FOLDERBROWSER)
				{
					def.mComboBox.Location = new System.Drawing.Point(
						BUTTON_POS_STARTX,
						BUTTON_POS_STARTY - val + row_y * BUTTON_HEIGHT);

					def.mButton.Location = new System.Drawing.Point(
						BUTTON_POS_STARTX + def.mComboBox.Size.Width,
						BUTTON_POS_STARTY - val + row_y * BUTTON_HEIGHT);

					row_y++;
				}
			}


			// Now set the scrollbar as we know the size of our buttons
			if(row_y* BUTTON_HEIGHT < this.Size.Height)
				this.vScrollBar1.Enabled = false;
			else
			{
				row_y++;
				this.vScrollBar1.Enabled = true;
				this.vScrollBar1.Maximum = row_y * BUTTON_HEIGHT - this.Size.Height;
			}

			this.Invalidate();
		}


		//------------------------------------------------------
		public void LoadCustomButtons(string platformName)
		{
			if (MOG_ControllerProject.IsProject())
			{
				UnloadCustomButtons();
				mPlatformName = platformName;

				// Get the project defaults
				string projectDefaultButtonsFile = MOG_ControllerProject.GetProject().GetProjectToolsPath() + "\\ClientConfigs\\" + MOG_ControllerProject.GetProject().GetProjectName() + ".Client.Buttons."+platformName+".info";
				if (DosUtils.FileExist(projectDefaultButtonsFile))
				{
					MOG_Ini pIni = new MOG_Ini(projectDefaultButtonsFile);		

					int i=0;

					string section = "BUTTON" + i;

					while( pIni.SectionExist(section) )	
					{
						int val = pIni.GetValue(section, "TYPE");
						// glk: This can be replaced by a bool instead of an int... (more clear)
						int hide = pIni.GetValue(section, "HIDEWINDOW");
						switch( (ETYPE)val )
						{
							case ETYPE.STD_BUTTON:
								CreateStdButton( pIni.GetString(section, "BUTTONNAME"), 
									pIni.GetString(section, "COMMAND"), 
									pIni.GetString(section, "ARGUMENTS"), 
									hide, section);
								break;

							case ETYPE.STD_BUTTON_EDIT:
								CreateEditButton( pIni.GetString(section, "BUTTONNAME"), 
									pIni.GetString(section, "FIELDNAME"),
									pIni.GetString(section, "ARGUMENTS"), 
									pIni.GetString(section, "COMMAND"), 
									hide, section );
								break;

							case ETYPE.STD_FILETYPE_LIST:
								CreateFileTypeButton( pIni.GetString(section, "BUTTONNAME"), 
									pIni.GetString(section, "FIELDNAME"),
									pIni.GetString(section, "DIRECTORY"), 
									pIni.GetString(section, "EXTENSION"), 
									pIni.GetString(section,"COMMAND"),
									hide );
								break;

							case ETYPE.STD_FOLDERBROWSER:
								CreateFileBrowser(	pIni.GetString(section, "BUTTONNAME"), 
									pIni.GetString(section, "FIELDNAME"),
									pIni.GetString(section, "DIRECTORY"), 
									pIni.GetString(section, "EXTENSION") );
								break;

						}
						section = "BUTTON" + ++i;
					}
				}
			}
		
			ScrollButtons(0);
		}


		//----------------------------------------------------------------
		// writes out settings to an ini file
		public void WriteSettingsToIni()
		{
			string projectDefaultButtonsFile = MOG_ControllerProject.GetProject().GetProjectToolsPath() + "\\ClientConfigs\\" + MOG_ControllerProject.GetProject().GetProjectName() + ".Client.Buttons."+mPlatformName+".info";
			if(projectDefaultButtonsFile.Length == 0)
				return;

			MOG_Ini pIni = new MOG_Ini(projectDefaultButtonsFile);		

			pIni.Empty();

			int i=0;
			foreach(ControlDefinition def in mControlsList)		
			{
				string section = "Button" + i;

				pIni.PutString(section, "Type", ""+def.mEType);
				pIni.PutString(section, "BUTTONNAME", def.mButton.Text);
				pIni.PutString(section, "Command", def.mCommand);
				pIni.PutString(section, "HideWindow", ""+def.mHiddenOutput);

				if((ETYPE)def.mEType == ETYPE.STD_BUTTON)
					pIni.PutString(section, "ARGUMENTS", def.mArguments);

				if((ETYPE)def.mEType == ETYPE.STD_BUTTON_EDIT)
				{
					pIni.PutString(section, "FIELDNAME", def.mTextBox.Name);
					pIni.PutString(section, "ARGUMENTS", def.mEditText);
				}

				if(	(ETYPE)def.mEType == ETYPE.STD_FILETYPE_LIST ||
					(ETYPE)def.mEType == ETYPE.STD_FOLDERBROWSER)
				{
					pIni.PutString(section, "Directory", def.mDirectory);
					pIni.PutString(section, "Extension", def.mExtension);
					pIni.PutString(section, "FIELDNAME", def.mComboBox.Name);
				}

				i++;
			}

			pIni.Close();
		}


		//------------------------------------------------------
		// Adds a new single click button
		private void AddNewButton(object sender, System.EventArgs e)
		{
			if(ProjectCheck())		return;

			CustomButtonEdit bEdit = new CustomButtonEdit();

			bEdit.StartPosition = FormStartPosition.CenterScreen;
			bEdit.FieldName.ReadOnly = true;

			DialogResult r = bEdit.ShowDialog();
			if( r  == DialogResult.OK)
			{
				CreateStdButton( bEdit.NameBox.Text, bEdit.CommandBox.Text, bEdit.Arguments.Text, bEdit.hideoutput.Checked? 1:0, "" );
				WriteSettingsToIni();
				ScrollButtons(0);
				return;
			}
		}


		//------------------------------------------------------
		// Adds a button with a Parameter field
		private void AddNewParamButton(object sender, System.EventArgs e)
		{
			if(ProjectCheck())		return;

			CustomButtonEdit bEdit = new CustomButtonEdit();
			bEdit.StartPosition = FormStartPosition.CenterScreen;

			if( bEdit.ShowDialog()  == DialogResult.OK)
			{
				CreateEditButton( bEdit.NameBox.Text, 
					bEdit.FieldName.Text,	// field name
					bEdit.Arguments.Text,
					bEdit.CommandBox.Text, 
					bEdit.hideoutput.Checked? 1:0, 
					"" );


				WriteSettingsToIni();
				ScrollButtons(0);
				return;
			}
		}


		//------------------------------------------------------
		// Adds a folder browser control
		private void AddFolderBrowserButton(object sender, System.EventArgs e)
		{
			if(ProjectCheck())		return;

			EditFileBrowserButton form2 = new EditFileBrowserButton();
			form2.StartPosition = FormStartPosition.CenterScreen;

			if(form2.ShowDialog() == DialogResult.OK)
			{
				CreateFileBrowser( form2.ButtonName.Text, 
					form2.FieldName.Text, 
					"C:\\",
					"*.*" );
				WriteSettingsToIni();
				ScrollButtons(0);
			}
		}


		//------------------------------------------------------
		//
		private void AddFileTypeButton(object sender, System.EventArgs e)
		{
			if(ProjectCheck())		return;

			EditFileTypeButton form = new EditFileTypeButton();
			form.StartPosition = FormStartPosition.CenterScreen;

			if(form.ShowDialog() == DialogResult.OK)
			{
				CreateFileTypeButton( form.TBox_ButName.Text, 
					form.FieldName.Text,	// field name
					form.TBox_Path.Text, 
					form.TBox_Ext.Text, 
					form.TBox_Command.Text, 
					form.Check_Hidden.Checked?1:0 );
				WriteSettingsToIni();

				RefreshDirList( (ControlDefinition)mControlsList[mControlsList.Count-1] );
				ScrollButtons(0);
			}
		}


		private bool ProjectCheck()
		{
//			string projectDefaultButtonsFile = MOG_ControllerProject.GetProject().GetProjectToolsPath() + "\\ClientConfigs\\" + MOG_ControllerProject.GetProject().GetProjectName() + ".Client.Buttons."+mPlatformName+".info";
			if(mPlatformName == null || mPlatformName.Length == 0)
			{
				MessageBox.Show("You must select a valid project before adding controls", "ERROR",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				return true;
			}
			return false;
		}

		/// <summary>
		/// Create a standard MOG toolbox button
		/// </summary>
		/// <param name="ButtonName"></param>
		/// <param name="Command">Command for shellspawn</param>
		/// <param name="arguments">Command spawn arguments</param>
		/// <param name="HiddenOutput">Greater than 0 to hide output</param>
		/// <param name="Section">Ini section name for button</param>
		private void CreateStdButton( string ButtonName, string Command, string arguments, int HiddenOutput, string Section )
		{
			ControlDefinition def = new ControlDefinition();
			def.mEType = (int)ETYPE.STD_BUTTON;
			def.mCommand = Command;
			def.mHiddenOutput = HiddenOutput;
			def.mIniSectionName = Section;
			def.mArguments = arguments;

			def.mButton = new Button();
			def.mButton.Location = new System.Drawing.Point(0, 0);
			def.mButton.Text = ButtonName;
			def.mButton.FlatStyle = FlatStyle.System;
			def.mButton.Name = "" + mControlsList.Count;
			def.mButton.Click += new System.EventHandler(this.Event_ButtonClickHandler);
			def.mButton.ContextMenu = this.ButtonContext;

			// Add the button to the form.
			this.Controls.Add(def.mButton);
			mControlsList.Add(def);
		}


		/// <summary>
		/// Create an edit Mog button
		/// </summary>
		/// <param name="ButtonName"></param>
		/// <param name="EditName">Name of edit text box</param>
		/// <param name="Text">Text in edit text box, by default</param>
		/// <param name="Command">Command for shellspawn</param>
		/// <param name="HiddenOutput">Greater than 0 to hide output</param>
		/// <param name="Section">Ini section name for button</param>
		private void CreateEditButton( string ButtonName, string EditName, string Text, string Command, int HiddenOutput, string Section )
		{
			ControlDefinition def = new ControlDefinition();
			def.mEType = (int)ETYPE.STD_BUTTON_EDIT;
			def.mCommand = Command;
			def.mHiddenOutput = HiddenOutput;
			def.mIniSectionName = Section;
			def.mEditText = Text;

			def.mButton = new Button();
			def.mButton.Location = new System.Drawing.Point(0, 0);
			def.mButton.Text = ButtonName;
			def.mButton.FlatStyle = FlatStyle.System;
			def.mButton.Name = "" + mControlsList.Count;
			def.mButton.Click += new System.EventHandler(this.Event_ParamButtonClickHandler);
			def.mButton.ContextMenu = this.ButtonContext;

			def.mTextBox = new System.Windows.Forms.TextBox();
			def.mTextBox.Location = new System.Drawing.Point(0, 0);
			def.mTextBox.Size = new System.Drawing.Size(def.mButton.Size.Width*2, 20);
			def.mTextBox.Name = EditName;
			def.mTextBox.TextChanged += new System.EventHandler(this.ParameterTextChanged);
			def.mTextBox.TabIndex = 1;
			def.mTextBox.Text = Text;

			this.Controls.Add(def.mButton);
			this.Controls.Add(def.mTextBox);

			mControlsList.Add(def);
		}


		//------------------------------------------------------
		private void CreateFileTypeButton( string ButtonName, string ComboName, string path, string extension, string command, int HiddenOutput )
		{
			ControlDefinition def = new ControlDefinition();
			def.mEType = (int)ETYPE.STD_FILETYPE_LIST;
			def.mHiddenOutput = HiddenOutput;
			def.mDirectory = path;
			def.mExtension = extension;
			def.mCommand = command;

			def.mButton = new Button();
			def.mButton.Location = new System.Drawing.Point(0, 0);
			def.mButton.Text = ButtonName;
			def.mButton.FlatStyle = FlatStyle.System;
			def.mButton.Name = "" + mControlsList.Count;
			def.mButton.Click += new System.EventHandler(this.Event_FileTypeClickHandler);
			def.mButton.ContextMenu = this.ButtonContext;

			def.mComboBox = new ComboBox();
			def.mComboBox.Location = new System.Drawing.Point(0, 0);
			def.mComboBox.Size = new System.Drawing.Size(def.mButton.Size.Width*2, 20);
			def.mComboBox.Name = ComboName;
			def.mComboBox.TabIndex = 1;
			def.mComboBox.Text = "";

			this.Controls.Add(def.mButton);
			this.Controls.Add(def.mComboBox);
			mControlsList.Add(def);
			RefreshDirList( def );
		}


		//------------------------------------------------------
		private void CreateFileBrowser( string ButtonName, string ComboName, string path, string extension )
		{
			ControlDefinition def = new ControlDefinition();
			def.mEType = (int)ETYPE.STD_FOLDERBROWSER;
			def.mDirectory = path;
			def.mExtension = extension;

			def.mButton = new Button();
			def.mButton.Location = new System.Drawing.Point(0, 0);
			def.mButton.Text = ButtonName;
			def.mButton.FlatStyle = FlatStyle.System;
			def.mButton.Name = "" + mControlsList.Count;
			def.mButton.Click += new System.EventHandler(this.Event_FolderBrowserHandler);
			def.mButton.ContextMenu = this.ButtonContext;

			def.mComboBox = new ComboBox();
			def.mComboBox.Location = new System.Drawing.Point(0, 0);
			def.mComboBox.Size = new System.Drawing.Size(def.mButton.Size.Width*2, 20);
			def.mComboBox.Name = ComboName;
			def.mComboBox.TabIndex = 1;
			def.mComboBox.Text = "";

			this.Controls.Add(def.mButton);
			this.Controls.Add(def.mComboBox);
			mControlsList.Add(def);
			RefreshDirList( def );
		}


		//------------------------------------------------------
		// handle a click event on a button
		private void Event_ButtonClickHandler(object sender, System.EventArgs e)
		{
			string output = "";
			Button b = (Button)sender;
			//int index = Convert.ToInt32(b.Name);
			int index = LocateIndex( b );

			ControlDefinition def = (ControlDefinition)mControlsList[index];
			string args = def.mArguments;
			args = ReplaceKeyWords(args);
			guiCommandLine.ShellSpawn( def.mCommand, args, def.mHiddenOutput != 0 ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal, ref output);
		}


		//------------------------------------------------------
		// handle a click event on a button with associated edit / param box
		private void Event_ParamButtonClickHandler(object sender, System.EventArgs e)
		{
			string output = "";
			Button b = (Button)sender;
			int index = LocateIndex( b );
			ControlDefinition def = (ControlDefinition)mControlsList[index];

			string args = def.mTextBox.Text;
			args = ReplaceKeyWords(args);
			guiCommandLine.ShellSpawn( def.mCommand, args, def.mHiddenOutput != 0 ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal, ref output);
		}


		//-------------------------------------------------------
		// handle FileType Click event
		private void Event_FileTypeClickHandler(object sender, System.EventArgs e)
		{
			string output = "";
			Button b = (Button)sender;
			int idx = LocateIndex(b);
			ControlDefinition def = (ControlDefinition)mControlsList[idx];
			Object selectedItem = def.mComboBox.SelectedItem;

			string args = selectedItem.ToString();
			args = ReplaceKeyWords(args);

			guiCommandLine.ShellSpawn( def.mCommand, args, def.mHiddenOutput != 0 ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal, ref output);
			System.Console.WriteLine("TEST"); 
		}


		//------------------------------------------------------
		private void Event_FolderBrowserHandler(object sender, System.EventArgs e)
		{
			Button b = (Button)sender;
			int idx = LocateIndex(b);
			ControlDefinition def = (ControlDefinition)mControlsList[idx];

			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.SelectedPath = def.mDirectory;

			if (fbd.ShowDialog() == DialogResult.OK) 
			{
				def.mDirectory = fbd.SelectedPath;
				WriteSettingsToIni();
			}
		}


		//------------------------------------------------------
		private void ParameterTextChanged(object sender, System.EventArgs e)
		{
			TextBox t = (TextBox)sender;

			foreach(ControlDefinition def in mControlsList)		
			{
				if (def.mTextBox == t)
				{
					def.mEditText = t.Text;
				}
			}
		}


		//------------------------------------------------------
		// Refresh the FileType listbox for this control
		private void RefreshDirList(ControlDefinition def)
		{
			def.mComboBox.Items.Clear();

			// Obtain the file system entries in the directory path.
			if(System.IO.Directory.Exists(def.mDirectory) == false)
			{
				return;
			}

			string pattern = "*." + def.mExtension;
			string[] directoryEntries =	System.IO.Directory.GetFileSystemEntries(def.mDirectory, pattern); 

			foreach (string str in directoryEntries) 
			{
				System.Console.WriteLine(str);
				def.mComboBox.Items.Add(str);
			}

			if(def.mComboBox.Items.Count > 0)
				def.mComboBox.SelectedIndex = 0;
		}


		//------------------------------------------------------
		// we are removing a single button
		private void UnloadCustomButton(ControlDefinition def, int RemoveFromList)
		{
			switch((ETYPE) def.mEType )
			{
				case ETYPE.STD_BUTTON:
					def.mButton.Dispose();
					break;

				case ETYPE.STD_BUTTON_EDIT:
					def.mButton.Dispose();
					def.mTextBox.Dispose();
					break;

				case ETYPE.STD_FILETYPE_LIST:
				case ETYPE.STD_FOLDERBROWSER:
					def.mButton.Dispose();
					def.mComboBox.Dispose();
					break;
			}

			if(RemoveFromList != 0)
				mControlsList.RemoveAt( mControlsList.IndexOf(def) );
		}


		//------------------------------------------------------
		// we are dying, clean up
		public void UnloadCustomButtons()
		{
			foreach(ControlDefinition def in mControlsList)		// go through client list
			{
				UnloadCustomButton(def, 0);
			}

			mControlsList.Clear();
		}


		//----------------------------------------------------------------
		// Someone clicked to remove this button
		private void DeleteControl(object sender, System.EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			ContextMenu menu = item.GetContextMenu();			
			Button b = (Button)menu.SourceControl;
			int index = LocateIndex( b );

			UnloadCustomButton( (ControlDefinition)mControlsList[index], 1 );
			ScrollButtons(0);
			WriteSettingsToIni();
			
		}


		//------------------------------------------------------------------
		// Edit the Control
		private void EditControl(object sender, System.EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			ContextMenu menu = item.GetContextMenu();			
			Button b = (Button)menu.SourceControl;

			int index = LocateIndex( b );
			ControlDefinition def = (ControlDefinition)mControlsList[index];

			switch((ETYPE) def.mEType )
			{
				case ETYPE.STD_BUTTON:
				case ETYPE.STD_BUTTON_EDIT:
					CustomButtonEdit bEdit = new CustomButtonEdit();

					bEdit.NameBox.Text = b.Text;
					bEdit.CommandBox.Text = def.mCommand;
					bEdit.StartPosition = FormStartPosition.CenterScreen;
					if((ETYPE) def.mEType == ETYPE.STD_BUTTON)
					{
						bEdit.FieldName.ReadOnly = true;
						bEdit.Arguments.Text = def.mArguments;
					}
					else
					{
						bEdit.FieldName.Text = def.mTextBox.Name;
						bEdit.Arguments.Text = def.mTextBox.Text;
					}

					DialogResult r = bEdit.ShowDialog();
					if( r  == DialogResult.OK)
					{
						def.mButton.Text = bEdit.NameBox.Text;
						def.mButton.Text = bEdit.NameBox.Text;
						def.mCommand = bEdit.CommandBox.Text;
						def.mHiddenOutput = bEdit.hideoutput.Checked? 1:0;

						if((ETYPE) def.mEType == ETYPE.STD_BUTTON)
							def.mArguments = bEdit.Arguments.Text;

						if((ETYPE) def.mEType == ETYPE.STD_BUTTON_EDIT)
						{
							def.mTextBox.Name = bEdit.FieldName.Text;
							def.mTextBox.Text = bEdit.Arguments.Text;
						}

						// write out update to ini
						WriteSettingsToIni();
						return;
					}
					break;

				case ETYPE.STD_FILETYPE_LIST:
					EditFileTypeButton form = new EditFileTypeButton();
					form.TBox_ButName.Text = def.mButton.Text;
					form.TBox_Path.Text = def.mDirectory;
					form.TBox_Ext.Text = def.mExtension;
					form.TBox_Command.Text = def.mCommand;
					form.Check_Hidden.Checked = def.mHiddenOutput == 0 ? false:true;
					form.StartPosition = FormStartPosition.CenterScreen;
					form.FieldName.Text = def.mComboBox.Name;

					if(form.ShowDialog() == DialogResult.OK)
					{
						def.mButton.Text = form.TBox_ButName.Text;
						def.mDirectory = form.TBox_Path.Text;
						def.mExtension = form.TBox_Ext.Text;
						def.mCommand = form.TBox_Command.Text;
						def.mHiddenOutput = form.Check_Hidden.Checked == true ? 1:0;
						def.mComboBox.Name = form.FieldName.Text;

						WriteSettingsToIni();
						RefreshDirList( def );
						ScrollButtons(0);
					} 
					break;

				case ETYPE.STD_FOLDERBROWSER:

					EditFileBrowserButton form2 = new EditFileBrowserButton();
					form2.StartPosition = FormStartPosition.CenterScreen;
					form2.FieldName.Text = def.mComboBox.Name;
					form2.ButtonName.Text = def.mButton.Text;
					if(form2.ShowDialog() == DialogResult.OK)
					{
						def.mComboBox.Name = form2.FieldName.Text;
						def.mButton.Text = form2.ButtonName.Text;
						WriteSettingsToIni();
						RefreshDirList( def );
						ScrollButtons(0);
					}
					break;

			}

			System.Console.WriteLine("TEST"); 
		}


		//---------------------------------------------------------------
		// returns the index of the Def from a button ptr
		private int LocateIndex(Button b)
		{
			int i=0;
			foreach(ControlDefinition def in mControlsList)		
			{
				if (def.mButton == b)
					return i;
				i++;
			}

			return -1;
		}


		//---------------------------------------------------------------
		// Replaces Control Names with Control field data
		private string ReplaceKeyWords(string inStr)
		{	
//			string correctString = errString.Replace("docment", "document");
			string pattern;
				
			foreach(ControlDefinition def in mControlsList)		
			{
				switch((ETYPE)def.mEType)
				{
					case ETYPE.STD_BUTTON_EDIT:

						pattern = "<" + def.mTextBox.Name + ">";

						int i = inStr.IndexOf(pattern);
						if( inStr.IndexOf(pattern) != -1)	// this pattern even exist?
						{
							inStr = inStr.Replace(pattern, def.mTextBox.Text);
						}
						break;

					case ETYPE.STD_FOLDERBROWSER:
					case ETYPE.STD_FILETYPE_LIST:

						pattern = "<" + def.mComboBox.Name + ">";

						i = inStr.IndexOf(pattern);
						if( inStr.IndexOf(pattern) != -1)	// this pattern even exist?
						{
							Object selectedItem = def.mComboBox.SelectedItem;
							inStr = inStr.Replace(pattern, selectedItem.ToString());
						}
						break;
				}
			}

			return inStr;
		}
	}


	//---------------------------------------------
	// node class to hold infos
	public class ControlDefinition
	{
		public string	mIniSectionName;

		public int		mEType;
		public int		mHiddenOutput;
		public string	mCommand;
		public string	mArguments;
		public string	mEditText;
		public Button	mButton;
		public TextBox	mTextBox;

		// dir list control
		public string	mDirectory;
		public string	mExtension;
		public ComboBox mComboBox;


		public ControlDefinition(){  }
	}
}
