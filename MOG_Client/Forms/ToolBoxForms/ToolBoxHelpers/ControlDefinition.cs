using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;

using MOG.INI;
using MOG_Client.Forms.ToolBoxForms;

using MOG.CONTROLLER.CONTROLLERPROJECT;
using System.IO;
using MOG.PROMPT;


namespace MOG_Client.Forms
{
	/// <summary>
	/// Holds all the meta-Control information for ToolBox Controls
	/// </summary>
	public class ControlDefinition : Control 
	{
		/// <summary>
		/// Default depth for Controls that have depth
		/// </summary>
		public const int Default_ComboBox_Depth = 15;
		public const string ArgumentButton_Text = ">";
		public const int ArgumentButton_Width = 24;
		public const string ArgumentButton_ToolTip = "Click here to add a tag to the end of your " 
			+ "current argument";

		private ToolBox mParent;
		private ArrayList CustomControls;

		#region Custom Controls
		private Button mArgumentButton;
		public Button DefArgumentButton
		{
			get
			{
				if( mArgumentButton != null )
					return mArgumentButton;
				else
					return new Button();
			}
			set
			{
				this.mArgumentButton = value;
				AddControl( this.mArgumentButton );
			}
		}
		private CheckBox mCheckBox;
		public CheckBox DefCheckBox
		{
			get
			{
				if( mCheckBox != null )
					return mCheckBox;
				else
					return new CheckBox();
			}
			set
			{
				this.mCheckBox = value;
				AddControl( this.mCheckBox );
			}
		}
		private ToolBoxGroupBox mRadioGroupBox;
		/// <summary>
		/// ToolBoxGroupBox to hold all our RadioButtons
		/// </summary>
		public ToolBoxGroupBox DefRadioGroupBox
		{
			get
			{
				if( mRadioGroupBox != null )
					return mRadioGroupBox;
				else
					return new ToolBoxGroupBox();
			}
			set
			{
				this.mRadioGroupBox = value;
				AddControl( mRadioGroupBox );
			}
		}

		private SortedList mRadioButtons;
		/// <summary>
		/// SortedList of RadioButtons (should be assigned from Ini)
		/// </summary>
		public SortedList RadioButtons
		{
			get
			{
				if( mRadioButtons != null )
					return mRadioButtons;
				else
				{
					SortedList radioButtons = new SortedList();
					radioButtons.Add( "Name", "Arg" );
					return radioButtons;
				}					
			}
			set
			{
				this.mRadioButtons = value;
			}
		}
		private ComboBox mDefComboBox;
		/// <summary>
		/// ComboBox for this ControlDefinition
		/// </summary>
		public ComboBox DefComboBox
		{
			get
			{
				if( mDefComboBox != null )
					return mDefComboBox;
				else
					return new ComboBox();
			}
			set
			{
				this.mDefComboBox = value;
				AddControl( mDefComboBox );
			}
		}

		private Button	mDefButton;
		/// <summary>
		/// The Button attached to this ControlDefinition
		/// </summary>
		public Button DefButton
		{
			get
			{
				if( mDefButton != null )
					return mDefButton;
				else
					return new Button();
			}
			set
			{
				this.mDefButton = value;
				AddControl( mDefButton );
			}
		}

		private TextBox	mDefTextBox;
		/// <summary>
		/// TextBox for this ControlDefinition
		/// </summary>
		public TextBox DefTextBox
		{
			get
			{
				if( mDefTextBox != null )
					return mDefTextBox;
				else
					return new TextBox();
			}
			set
			{
				this.mDefTextBox = value;
				AddControl( mDefTextBox );
			}
		}

		private Label	mDefLabel;
		public Label DefLabel
		{
			get
			{
				if( mDefLabel != null )
					return mDefLabel;
				else
					return new Label();
			}
			set
			{
				this.mDefLabel = value;
				AddControl( mDefLabel );
			}
		}

		private LinkLabel mDefLinkLabel;
		public LinkLabel DefLinkLabel
		{
			get
			{
				if (mDefLinkLabel != null)
					return mDefLinkLabel;
				else
					return new LinkLabel();
			}
			set
			{
				this.mDefLinkLabel = value;
				AddControl(mDefLinkLabel);
			}
		}
		#endregion Custom Controls

		#region Control Definition Properties/Variables

		// NOTE:  Anything that needs to be written out to the INI should return a non-null object pointer.

		/// <summary>
		/// If true, command line output for a command is not shown
		/// </summary>
		public bool		HideOutput;


		private int mLastSelectedIndex;
		/// <summary>
		/// Indicates the last selected index for thing such as ComboBoxes
		/// </summary>
		public int LastSelectedIndex
		{
			get
			{
				return mLastSelectedIndex;
			}
			set
			{
				this.mLastSelectedIndex = value;
			}
		}
		private string mArgumentTrue;
		/// <summary>
		/// Argument if checkbox is checked
		/// </summary>
		public string ArgumentTrue
		{
			get
			{
				if( mArgumentTrue != null )
					return mArgumentTrue;
				else
					return "";
			}
			set
			{
				this.mArgumentTrue = value;
			}
		}
		private string mArgumentFalse;
		/// <summary>
		/// Argument if checkbox is unchecked
		/// </summary>
		public string ArgumentFalse
		{
			get
			{
				if( mArgumentFalse != null )
					return mArgumentFalse;
				else
					return "";
			}
			set
			{
				this.mArgumentFalse = value;
			}
		}
		private string mPattern;
		/// <summary>
		/// The pattern to be used to select files within a FileOpenDialog
		/// </summary>
		public string Pattern
		{
			get
			{
				if( mPattern != null )
					return mPattern;
				else
					return "";
			}
			set
			{
				this.mPattern = value;
			}
		}

		private MogTaggedString[] mComboBoxItems;
		public MogTaggedString[] ComboBoxItems
		{
			get
			{
				return mComboBoxItems;
			}
			set
			{
				this.mComboBoxItems = value;
			}
		}

		private ToolBoxControlType	mControlType;
		/// <summary>
		/// The ToolBoxControlType of the Custom Control attached to ControlDefinition
		/// </summary>
		public ToolBoxControlType ControlType
		{
			get
			{
				return this.mControlType;
			}
			set
			{
				this.mControlType = value;
			}
		}

		private ToolBoxControlLocation mLocation;
		/// <summary>
		/// Indicates the location we should save this Custom Control to.  (Overrides Control::Location)
		/// </summary>
		public new ToolBoxControlLocation Location
		{
			get
			{
				return this.mLocation;
			}
			set
			{
				this.mLocation = value;
			}
		}

		private string	mCommand;
		/// <summary>
		/// Command to be executed by the shell when Custom Control is activated
		/// </summary>
		public string	Command
		{
			get
			{
				if( this.mCommand != null )
					return this.mCommand;	
				else
					return "";
			}
			set
			{
				// Save our user's input into mCommand
				this.mCommand = value;

				// Decide whether we want to have this command be relational or not
				if( MOG_ControllerProject.GetCurrentSyncDataController() != null )
				{
					string localPath = MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory().ToLower();
					// If our localPath is the first thing we see in this command...
					if( localPath != null && this.mCommand.ToLower().IndexOf( localPath ) == 0 )
					{
						// Get our pattern we need to replace
						string pattern = mCommand.Substring(0, localPath.Length );
						// Replace it with nothing
						this.mCommand = mCommand.Replace( pattern, "" );
					}
				}
			}
		}

		private string	mArguments;
		/// <summary>
		/// Arguments associated with this Custom Control
		/// </summary>
		public string	Arguments
		{
			get
			{
				if( mArguments != null )
					return mArguments;
				else
					return "";
			}
			set
			{
				this.mArguments = value;
			}
		}
		private string mTagName;
		/// <summary>
		/// The Tag string by which we look up this Control (Utility Name)
		/// </summary>
		public string TagName
		{
			get
			{
				if( mTagName != null )
					return mTagName;
				else
					return mControlName;
			}
			set
			{
				this.mTagName = value;
			}
		}
		private string mControlName;
		/// <summary>
		/// The Display Name of our Control.  Empty until set.
		/// </summary>
		public string ControlName
		{
			get
			{
				if( mControlName != null )
					return mControlName;
				else
					return "";
			}
			set
			{
				this.mControlName = value;
			}
		}
		private string mFolderName;
		/// <summary>
		/// Represents the folder we will start at for any File/Folder OpenDialogs
		/// </summary>
		public string FolderName
		{
			get
			{
				if( this.mFolderName != null )
					return mFolderName;
				else
					return "";
			}
			set
			{
				this.mFolderName = value;
			}
		}
		private string mToolTipString;
		/// <summary>
		/// String to be attached to ToolBox's ToolTip
		/// </summary>
		public string ToolTipString
		{
			get
			{
				if( this.mToolTipString != null )
					return mToolTipString;
				else
					return "";
			}
			set
			{
				this.mToolTipString = value;
			}
		}
		private int mComboBoxDepth;
		/// <summary>
		/// Depth of items to hold for certain Controls (ComboBox) in ControlDefinition
		/// </summary>
		public int ComboBoxDepth
		{
			get
			{
				if( mComboBoxDepth == 0 )
					return Default_ComboBox_Depth;
				else
					return mComboBoxDepth;
			}
			set
			{
				mComboBoxDepth = value;
			}
		}

		private bool mShowFullPaths;
		/// <summary>
		/// Determine if controls show the full path of the items that were browsed to
		/// </summary>
		public bool ShowFullPaths
		{
			get { return mShowFullPaths; }
			set { mShowFullPaths = value; }
		}

		private bool mShowWorkspaceRelativePaths;
		public bool ShowWorkspaceRelativePaths
		{
			get { return mShowWorkspaceRelativePaths; }
			set { mShowWorkspaceRelativePaths = value; }
		}

		private bool mShowFolderRelativePaths;
		public bool ShowFolderRelativePaths
		{
			get { return mShowFolderRelativePaths; }
			set { mShowFolderRelativePaths = value; }
		}

		private bool mShowBasePaths;
		public bool ShowBasePaths
		{
			get { return mShowBasePaths; }
			set { mShowBasePaths = value; }
		}

		public MogTaggedString.FilenameStyleTypes FilenameStyle
		{
			get
			{
				if (ShowFullPaths)
				{
					return MogTaggedString.FilenameStyleTypes.FullFilename;
				}
				else if (ShowWorkspaceRelativePaths)
				{
					return MogTaggedString.FilenameStyleTypes.WorkspaceRelativeFilename;
				}
				else if (ShowFolderRelativePaths)
				{
					return MogTaggedString.FilenameStyleTypes.FolderRelativeFilename;
				}
				else if (ShowBasePaths)
				{
					return MogTaggedString.FilenameStyleTypes.BaseFilename;
				}

				return MogTaggedString.FilenameStyleTypes.Raw;
			}
		}
	

		private bool mTagFullPaths;
		public bool TagFullPaths
		{
			get { return mTagFullPaths; }
			set { mTagFullPaths = value; }
		}

		private bool mTagWorkspaceRelativePaths;
		public bool TagWorkspaceRelativePaths
		{
			get { return mTagWorkspaceRelativePaths; }
			set { mTagWorkspaceRelativePaths = value; }
		}

		private bool mTagFolderRelativePaths;
		public bool TagFolderRelativePaths
		{
			get { return mTagFolderRelativePaths; }
			set { mTagFolderRelativePaths = value; }
		}

		private bool mTagBasePaths;
		public bool TagBasePaths
		{
			get { return mTagBasePaths; }
			set { mTagBasePaths = value; }
		}

		private bool mRecurseFolders;
		public bool RecurseFolders
		{
			get { return mRecurseFolders; }
			set { mRecurseFolders = value; }
		}
	
	
		private string	mIniSectionName;
		/// <summary>
		/// Represents the INI section name in use by an INI combo box
		/// </summary>
		public string IniSectionName
		{
			get
			{
				if( mIniSectionName != null )
					return mIniSectionName;
				else
					return "";
			}
			set
			{
				this.mIniSectionName = value;
			}
		}
		private string	mIniFilename;
		/// <summary>
		/// Represents the INI in use by an INI combo box
		/// </summary>
		public string IniFilename
		{
			get
			{
				if( mIniFilename != null )
					return mIniFilename;
				else
					return "";
			}
			set
			{
				this.mIniFilename = value;
			}
		}

		private string mDialogStartPath;
		/// <summary>
		/// Allows user to set from which path a FileOpenDialog or other Dialog should start.
		/// </summary>
		public string DialogStartPath
		{
			get
			{
				if( mDialogStartPath != null )
					return mDialogStartPath;
				else
					return "";
			}
			set
			{
				this.mDialogStartPath = value;
			}
		}

		private int mVisibleIndex = 0;
		public int VisibleIndex
		{
			get { return mVisibleIndex; }
			set { mVisibleIndex = value; }
		}

		private System.Collections.Generic.SortedList<int, ControlDefinition> mSiblings;
		public System.Collections.Generic.SortedList<int, ControlDefinition> Siblings
		{
			get { return mSiblings; }
			set { mSiblings = value; }
		}

		private string mControlGUID = "";
		public string ControlGuid
		{
			get { return mControlGUID; }
			set { mControlGUID = value; }
		}

		#endregion Control Definition Properties/Variables

		#region Savers / Loaders
		
		// Ini constants: Keys
		// Common
		const string GUID_Key = "GUID";
		const string ControlName_Key = "CONTROLNAME";
		const string Location_Key = "LocationToSave";
		const string TagName_Key = "TAGNAME";
		const string ToolTip_Key = "ToolTip";
		const string Type_Key = "Type";
		const string VisisbleIndex_Key = "VisisbleIndex";
		const string CustomSectionIndicator_Text = "CustomControl";
		const string SubSectionIndicator_Text = "_";

		// Button
		const string Command_Key = "COMMAND";
		const string HideWindow_Key = "HIDEWINDOW";
		const string Arguments_Key = "ARGUMENTS";
		
		// Ini ComboBox
		const string IniSection_Key = "IniSection";
		const string IniFilename_Key = "IniFilename";
		const string FolderName_Key = "FolderName";
		const string DialogStartPath_Key = "DialogStartPath";
		
		// FileComboBox
		const string ShowFullPaths_Key = "ShowFullPaths";
		const string ShowWorkspaceRelativePaths_Key = "ShowWorkspaceRelativePaths";
		const string ShowFolderRelativePaths_Key = "ShowFolderRelativePaths";
		const string ShowBasePaths_Key = "ShowBasePaths";
		const string FullPaths_Key = "FullPaths";
		const string WorkspaceRelativePaths_Key = "WorkspaceRelativePaths";
		const string FolderRelativePaths_Key = "FolderRelativePaths";
		const string BasePaths_Key = "BasePaths";
		const string RecurseFolders_Key = "RecursePaths";
		const string Pattern_Key = "FileExtPattern";
		const string ComboBoxDepth_Key = "ComboBoxDepth";
		const string ComboItems_Key = "Item";
		const string LastIndex_Key = "LastSelectedIndex";
		const string ComboItems_Section = SubSectionIndicator_Text + "ComboItems";
		
		// CheckBox
		const string ArgumentsTrue_Key = "ArgumentIfTrue";
		const string ArgumentsFalse_Key = "ArgumentIfFalse";

		// Radio button group
		const string RadioButton_Section = SubSectionIndicator_Text + "RadioButtons";
		
		
		public void Save(MOG_Ini pIni, string section)
		{
			// Write the type out to the INI
			pIni.PutString(section, Type_Key, ControlType.ToString());

			// Write our location out to the INI
			pIni.PutString(section, Location_Key, Location.ToString());

			// Write the name of the control out to our INI
			pIni.PutString(section, ControlName_Key, ControlName);
			// Write our TagName
			pIni.PutString(section, TagName_Key, TagName);

			// Write our Command
			pIni.PutString(section, Command_Key, Command);
			// Write whether or not we will be hiding output window
			pIni.PutBool(section, HideWindow_Key, HideOutput);

			// (ADD_HERE):  Any new Controls must be added here... 

			pIni.PutString(section, VisisbleIndex_Key, VisibleIndex.ToString());

			// Write our arguments
			pIni.PutString(section, Arguments_Key, Arguments);
			pIni.PutString(section, ArgumentsTrue_Key, ArgumentTrue);
			pIni.PutString(section, ArgumentsFalse_Key, ArgumentFalse);

			// Write our ToolTipString
			pIni.PutString(section, ToolTip_Key, ToolTipString);

			// Write the folder path we will start at for dialogs
			pIni.PutString(section, FolderName_Key, FolderName);

			// Write out the depth we will support for this def's ComboBox
			pIni.PutString(section, ComboBoxDepth_Key, ComboBoxDepth.ToString());

			// Write out the show full paths bool
			pIni.PutString(section, ShowFullPaths_Key, ShowFullPaths.ToString());
			pIni.PutString(section, ShowWorkspaceRelativePaths_Key, ShowWorkspaceRelativePaths.ToString());
			pIni.PutString(section, ShowFolderRelativePaths_Key, ShowFolderRelativePaths.ToString());
			pIni.PutString(section, ShowBasePaths_Key, ShowBasePaths.ToString());

			// Write out the relative paths bool
			pIni.PutString(section, FullPaths_Key, TagFullPaths.ToString());
			pIni.PutString(section, WorkspaceRelativePaths_Key, TagWorkspaceRelativePaths.ToString());
			pIni.PutString(section, FolderRelativePaths_Key, TagFolderRelativePaths.ToString());
			pIni.PutString(section, BasePaths_Key, TagBasePaths.ToString());

			pIni.PutString(section, RecurseFolders_Key, RecurseFolders.ToString());

			// Write our IniSectionName
			pIni.PutString(section, IniSection_Key, IniSectionName);
			// Write out our IniFilename
			pIni.PutString(section, IniFilename_Key, IniFilename);
			// Write out our GUID
			pIni.PutString(section, GUID_Key, section);

			// Write out our ComboBoxDepth
			pIni.PutString(section, ComboBoxDepth_Key, ComboBoxDepth.ToString());

			// By adding the scope of the user's name to the section key we can retain a user's own settings seperatly from the rest of the team
			string usersLastIndex_Key = MOG_ControllerProject.IsUser() ? LastIndex_Key + SubSectionIndicator_Text + MOG_ControllerProject.GetUserName(): LastIndex_Key;
			pIni.PutString(section, usersLastIndex_Key, LastSelectedIndex.ToString());

			// If we are a fileComboBox write out our items
			if (ControlType == ToolBoxControlType.FileCombo)
			{
				// By adding the scope of the user's name to the section key we can retain a user's own settings seperatly from the rest of the team
				string usersSection = MOG_ControllerProject.IsUser() ? section + ComboItems_Section + SubSectionIndicator_Text + MOG_ControllerProject.GetUserName(): section + ComboItems_Section;

				int count = 0;
				foreach (MogTaggedString item in DefComboBox.Items)
				{
					pIni.PutString(usersSection, ComboItems_Key + count++, (string)item.AttachedItem);
				}
			}

			// Write out the fileComboBox patterns
			pIni.PutString(section, Pattern_Key, Pattern);

			// If we are a ToolBoxGroupBox, write out our radio buttons
			if (ControlType == ToolBoxControlType.RadioButton)
			{
				foreach (DictionaryEntry button in RadioButtons)
				{
					pIni.PutString(section + RadioButton_Section, (string)button.Key, (string)button.Value);
				}
			}
		}

		public void Load(ToolBox toolBox, MOG_Ini pIni, string section, ToolBoxControlLocation currentLocation)
		{
			// If we can, get the IniSection...
			ControlGuid = section;
		
			// Set our comboBoxDepth to default
			//int ComboBoxDepth = ControlDefinition.Default_ComboBox_Depth;
			// Set our index for ComboBoxes (or anything else that needs it)
			//int LastSelectedIndex = 0;
			// Set saved/loadable comboBoxItems to null
			//MogTaggedString[] ComboBoxItems = null;
			// Set saved/loadable RadioButtons to null
			//SortedList RadioButtons = null;

			// Seed the default location now justin case the ini is missing this critical information
			Location = currentLocation;

			UpdateControl(pIni, toolBox);
		}

		private static bool CheckConfigurationForOutOfDate(string currentIniFilename)
		{
			MOG_Ini pIni = new MOG_Ini();
			if (pIni.Open(currentIniFilename, FileShare.Read))
			{
				foreach (string upgradeSection in pIni.GetSections(null))
				{
					if (pIni.KeyExist(upgradeSection, VisisbleIndex_Key) == false ||
						upgradeSection.StartsWith(CustomSectionIndicator_Text, StringComparison.CurrentCultureIgnoreCase))
					{
						// Only add the visisble key to ini's with a controlName defined
						if (pIni.KeyExist(upgradeSection, ControlName_Key) && pIni.KeyExist(upgradeSection, VisisbleIndex_Key) == false)
						{
							return true;
						}

						// Check for depricated section names
						if (upgradeSection.StartsWith("CustomControl", StringComparison.CurrentCultureIgnoreCase))
						{
							return true;
						}
					}
				}
				pIni.Close();
			}

			return false;
		}

		public void UpdateControl(ToolBox toolBox)
		{
			UpdateControl(null, toolBox);
		}
		
		public void UpdateControl(MOG_Ini pIni, ToolBox toolBox)
		{
			try
			{
				// Do we need to load this file?
				if (pIni == null)
				{
					string currentIniFilename = toolBox.GetCurrentIniFilenameFromLocation(Location);
					pIni = new MOG_Ini(currentIniFilename);
				}

				string type = "";
				// If we can, get the ToolBoxControlType of this control...
				if (pIni.KeyExist(ControlGuid, Type_Key))
					type = pIni.GetString(ControlGuid, Type_Key);
				// Get the ToolBoxControlType from the ini
				ControlType = (ToolBoxControlType)Enum.Parse(ToolBoxControlType.Button.GetType(), type, true);

				// If we can, get our ToolBoxControlLocation for this control...
				if (pIni.KeyExist(ControlGuid, Location_Key))
				{
					// Get the location from the ini
					Location = (ToolBoxControlLocation)Enum.Parse(ToolBoxControlLocation.Project.GetType(), pIni.GetString(ControlGuid, Location_Key), true);
				}

				// If we can, get the HideWindow value...
				if (pIni.KeyExist(ControlGuid, HideWindow_Key))
					HideOutput = pIni.GetBool(ControlGuid, HideWindow_Key);

				// Get all of our default values: (see ControlDefinition for what these identifiers mean)
				// If we can, get the ControlName...
				if (pIni.KeyExist(ControlGuid, ControlName_Key))
				{
					ControlName = pIni.GetString(ControlGuid, ControlName_Key);

					// Should this control have it's text updated?
					bool bUpdateControlText = false;
					switch(this.ControlType )
					{
						case ToolBoxControlType.Button:				bUpdateControlText = true;			break;
						case ToolBoxControlType.ParameterButton:	bUpdateControlText = true;			break;
						case ToolBoxControlType.Label:				bUpdateControlText = true;			break;
						case ToolBoxControlType.LinkLabel:			bUpdateControlText = true;			break;
						case ToolBoxControlType.IniCombo:			bUpdateControlText = false;			break;
						case ToolBoxControlType.FileCombo:			bUpdateControlText = false;			break;
						case ToolBoxControlType.CheckBox:			bUpdateControlText = true;			break;
						case ToolBoxControlType.RadioButton:		bUpdateControlText = true;			break;
					}

					if (bUpdateControlText)
					{
						DefButton.Text = ControlName;
					}
				}
				// If we can, get the Command...
				if (pIni.KeyExist(ControlGuid, Command_Key))
					Command = pIni.GetString(ControlGuid, Command_Key);
				// If we can, get the Arguments...
				if (pIni.KeyExist(ControlGuid, Arguments_Key))
					Arguments = pIni.GetString(ControlGuid, Arguments_Key);
				// If we can, get the ArgumentTrue...
				if (pIni.KeyExist(ControlGuid, ArgumentsTrue_Key))
					ArgumentTrue = pIni.GetString(ControlGuid, ArgumentsTrue_Key);
				// If we can, get the ArgumentFalse...
				if (pIni.KeyExist(ControlGuid, ArgumentsFalse_Key))
					ArgumentFalse = pIni.GetString(ControlGuid, ArgumentsFalse_Key);

				// If we can, get the TagName...
				if (pIni.KeyExist(ControlGuid, TagName_Key))
					TagName = pIni.GetString(ControlGuid, TagName_Key);
				// If we can, get the ToolTipString...
				if (pIni.KeyExist(ControlGuid, ToolTip_Key))
					ToolTipString = pIni.GetString(ControlGuid, ToolTip_Key);
				// If we can, get the FolderName...
				if (pIni.KeyExist(ControlGuid, FolderName_Key))
					FolderName = pIni.GetString(ControlGuid, FolderName_Key);
				// If we can, get the ComboBoxDepth...
				if (pIni.KeyExist(ControlGuid, ComboBoxDepth_Key))
					ComboBoxDepth = int.Parse(pIni.GetString(ControlGuid, ComboBoxDepth_Key));
				// If we can, get our Pattern
				if (pIni.KeyExist(ControlGuid, Pattern_Key))
					Pattern = pIni.GetString(ControlGuid, Pattern_Key);
				// If we can, get our ini filename path
				if (pIni.KeyExist(ControlGuid, IniFilename_Key))
					IniFilename = pIni.GetString(ControlGuid, IniFilename_Key);
				// If we can, get our ini section name
				if (pIni.KeyExist(ControlGuid, IniSection_Key))
					IniSectionName = pIni.GetString(ControlGuid, IniSection_Key);
				// If we can, get our DialogStartPath
				if (pIni.KeyExist(ControlGuid, DialogStartPath_Key))
					DialogStartPath = pIni.GetString(ControlGuid, DialogStartPath_Key);
				// If we can, set our LastSelectedIndex
				// By adding the scope of the user's name to the section key we can retain a user's own settings seperatly from the rest of the team
				string usersLastIndex_Key = MOG_ControllerProject.IsUser() ? LastIndex_Key + SubSectionIndicator_Text + MOG_ControllerProject.GetUserName(): LastIndex_Key;
				if (pIni.KeyExist(ControlGuid, usersLastIndex_Key))
					LastSelectedIndex = int.Parse(pIni.GetString(ControlGuid, usersLastIndex_Key));

				if (pIni.KeyExist(ControlGuid, ShowFullPaths_Key))
					ShowFullPaths = Convert.ToBoolean(pIni.GetString(ControlGuid, ShowFullPaths_Key));
				if (pIni.KeyExist(ControlGuid, ShowWorkspaceRelativePaths_Key))
					ShowWorkspaceRelativePaths = Convert.ToBoolean(pIni.GetString(ControlGuid, ShowWorkspaceRelativePaths_Key));
				if (pIni.KeyExist(ControlGuid, ShowFolderRelativePaths_Key))
					ShowFolderRelativePaths = Convert.ToBoolean(pIni.GetString(ControlGuid, ShowFolderRelativePaths_Key));
				if (pIni.KeyExist(ControlGuid, ShowBasePaths_Key))
					ShowBasePaths = Convert.ToBoolean(pIni.GetString(ControlGuid, ShowBasePaths_Key));

				if (pIni.KeyExist(ControlGuid, VisisbleIndex_Key))
				{
					VisibleIndex = Convert.ToInt32(pIni.GetString(ControlGuid, VisisbleIndex_Key));
				}

				if (pIni.KeyExist(ControlGuid, FullPaths_Key))
					TagFullPaths = Convert.ToBoolean(pIni.GetString(ControlGuid, FullPaths_Key));

				if (pIni.KeyExist(ControlGuid, WorkspaceRelativePaths_Key))
					TagWorkspaceRelativePaths = Convert.ToBoolean(pIni.GetString(ControlGuid, WorkspaceRelativePaths_Key));

				if (pIni.KeyExist(ControlGuid, FolderRelativePaths_Key))
					TagFolderRelativePaths = Convert.ToBoolean(pIni.GetString(ControlGuid, FolderRelativePaths_Key));

				if (pIni.KeyExist(ControlGuid, BasePaths_Key))
					TagBasePaths = Convert.ToBoolean(pIni.GetString(ControlGuid, BasePaths_Key));

				if (pIni.KeyExist(ControlGuid, RecurseFolders_Key))
					RecurseFolders = Convert.ToBoolean(pIni.GetString(ControlGuid, RecurseFolders_Key));

				// If we can, get the ComboBoxItems
				// By adding the scope of the user's name to the section key we can retain a user's own settings seperatly from the rest of the team
				string usersSection = MOG_ControllerProject.IsUser() ? ControlGuid + ComboItems_Section + SubSectionIndicator_Text + MOG_ControllerProject.GetUserName(): ControlGuid + ComboItems_Section;
				if (pIni.SectionExist(usersSection))
				{
					ComboBoxItems = LoadComboBoxItems(usersSection, pIni);
				}

				// If we can, get our RadioButtons
				if (pIni.SectionExist(ControlGuid + RadioButton_Section))
				{
					RadioButtons = LoadRadioButtons(ControlGuid + RadioButton_Section, pIni);
				}
			}
			catch (Exception e)
			{
				e.ToString();
			}
		}

		/// <summary>
		/// Load saved ComboBoxItems from an Ini
		/// </summary>
		/// <param name="section">Section where to look for the items</param>
		/// <param name="ini">Ini in which to look for section</param>
		/// <returns>Array of MogTaggedStrings that can be placed directly into a ComboBox</returns>
		private MogTaggedString[] LoadComboBoxItems(string section, MOG_Ini ini)
		{
			int count = ini.CountKeys(section);
			MogTaggedString[] tbStrings = new MogTaggedString[count];

			// Determin the desired directory
			string directory = string.IsNullOrEmpty(this.FolderName) ? MOG_ControllerProject.GetWorkspaceDirectory() : this.FolderName;
			// Check if we are missing a root path?
			if (!Path.IsPathRooted(directory))
			{
				// Append on the current workspace directory
				directory = Path.Combine(MOG_ControllerProject.GetWorkspaceDirectory(), directory.Trim("\\".ToCharArray()));
			}

			// Load the items from the ini
			for (int i = 0; i < count; ++i)
			{
				string filename = ini.GetKeyByIndexSLOW(section, i);

				tbStrings[i] = new MogTaggedString(filename, filename, this.FilenameStyle, directory);
			}
			return tbStrings;
		}

		/// <summary>
		/// Load our RadioButtons, given a section and a MOG_Ini
		/// </summary>
		/// <param name="section"></param>
		/// <param name="ini"></param>
		/// <returns></returns>
		private static SortedList LoadRadioButtons(string section, MOG_Ini ini)
		{
			ini.Load(ini.GetFilename());
			int count = ini.CountKeys(section);
			SortedList radioButtons = new SortedList();
			for (int i = 0; i < count; ++i)
			{
				radioButtons.Add(ini.GetKeyNameByIndexSLOW(section, i), ini.GetKeyByIndexSLOW(section, i));
			}
			return radioButtons;
		}
		#endregion

		private string mOpeningStackTrace;
		/// <summary>
		/// Initialize a ControlDefinition with no assigned type.  ControlType MUST be assigned before use.
		/// </summary>
		public ControlDefinition( ToolBox parent, string openingStackTrace )
		{
			this.Visible = false;
			this.Enabled = false;
			this.LastSelectedIndex = 0;
			this.mParent = parent;
			this.mOpeningStackTrace = openingStackTrace;
			this.CustomControls = new ArrayList();
		}
		/// <summary>
		/// Initialize a ControlDefinition (always NOT visible and NOT enabled)
		/// </summary>
		/// <param name="controlType">Indicates the type of the control we are representing.</param>
		public ControlDefinition( ToolBoxControlType controlType, ToolBox parent, string openingStackTrace )
			: this( parent, openingStackTrace )
		{  
			this.mControlType = controlType;
		}

		public void AddControl( Control control )
		{
			// If we do not already have this control, add it
			if( !this.CustomControls.Contains( control ) )
			{
				this.CustomControls.Add( control );
			}
		}

		public void SetToolTip( ToolTip toolTip )
		{
			// Make sure we have a valid toolTipString
			string toolTipString = this.ToolTipString;
			if( toolTipString.Length < 1 )
			{
				toolTipString = this.ControlName;
			}

			switch( this.ControlType )
			{
				case ToolBoxControlType.Button:
				case ToolBoxControlType.ParameterButton:
					string fullToolTipString = toolTipString + ToolBox.Evaluate_Text + mParent.GetEvaluationString( this );
					toolTip.SetToolTip( this.DefButton, fullToolTipString);
					toolTip.SetToolTip( this.DefTextBox, fullToolTipString);
					toolTip.SetToolTip( this.DefArgumentButton, ArgumentButton_ToolTip );
					break;
				case ToolBoxControlType.CheckBox:
					toolTip.SetToolTip( this.DefCheckBox, "Argument: '" + this.Arguments + "'  " + 
						ToolBox.Evaluate_Text + mParent.GetEvaluationString(this));
					break; 
				case ToolBoxControlType.FileCombo:
				case ToolBoxControlType.IniCombo:
					SetDefaultToolTips(toolTip, toolTipString);// + ToolBox.Evaluate_Text + mParent.GetEvaluationString(this));
					break;
				case ToolBoxControlType.RadioButton:
					SetDefaultToolTips(toolTip, toolTipString);
					ToolBox.SetGroupBoxToolTips( toolTip, this.DefRadioGroupBox, toolTipString );
					break;
					// Ignore Labels
				case ToolBoxControlType.Label:
					break;
				case ToolBoxControlType.LinkLabel:
					break;
				default:
					throw new Exception("Programmer Error: A new ToolBoxControlType, " + this.ControlType.ToString()
						+ ", has been added without the ability to set a ToolTip" );
			}
		}

		private void SetDefaultToolTips( ToolTip toolTip, string toolTipString )
		{
			// As our default, set everything to the current toolTipString
			foreach( Control control in this.Controls )
			{
				if( control != null )
				{
					toolTip.SetToolTip( control, toolTipString );
				}
			}
		}

		internal void MoveUp()
		{
			foreach (KeyValuePair<int, ControlDefinition> sibling in mSiblings)
			{
				if (sibling.Value.VisibleIndex == VisibleIndex -1)
				{
					int oldVal = VisibleIndex;
					VisibleIndex = sibling.Value.VisibleIndex;
					sibling.Value.VisibleIndex = oldVal;
					break;
				}
			}
		}

		internal void MoveDown()
		{
			foreach (KeyValuePair<int, ControlDefinition> sibling in mSiblings)
			{
				if (sibling.Value.VisibleIndex == VisibleIndex + 1)
				{
					int oldVal = VisibleIndex;
					VisibleIndex = sibling.Value.VisibleIndex;
					sibling.Value.VisibleIndex = oldVal;
					break;
				}
			}
		}

		internal void Remove(ToolBox toolbox)
		{
			// Write out our change
			string currentIniFilename = toolbox.GetCurrentIniFilenameFromLocation(Location);
			MOG_Ini pIni = new MOG_Ini();

			OpenConfigIni:
			if (pIni.Open(currentIniFilename, FileShare.Write))
			{
				// Remove this control from the siblings
				mSiblings.Remove(VisibleIndex);

				// Go thru all of this controls siblings and move their visual index up one so that there is not a break in the chain
				foreach (KeyValuePair<int, ControlDefinition> sibling in mSiblings)
				{
					// Is this guy visually after the one we are deleting?
					if (sibling.Value.VisibleIndex > VisibleIndex)
					{
						// Then move it up one
						sibling.Value.VisibleIndex = sibling.Value.VisibleIndex -1;

						// Make sure this section actually exists
						if (pIni.SectionExist(sibling.Value.ControlGuid))
						{
							pIni.PutString(sibling.Value.ControlGuid, VisisbleIndex_Key, sibling.Value.VisibleIndex.ToString());
						}
					}
				}

				pIni.RemoveSection(ControlGuid);

				// Scan all the sections looking for any related subsections
				ArrayList relatedSubSections = new ArrayList();
				string subControlGuid = ControlGuid + SubSectionIndicator_Text;
				foreach (string section in pIni.GetSections(null))
				{
					if (section.StartsWith(subControlGuid, StringComparison.CurrentCultureIgnoreCase))
					{
						// Schedule this subsection for removal
						relatedSubSections.Add(section);
					}
				}
				// Remove any subsections related to this control
				foreach (string section in relatedSubSections)
				{
					pIni.RemoveSection(section);
				}

				pIni.Save();
				pIni.Close();
			}
			else
			{
				if (
					MOG_Prompt.PromptResponse("Configuration locked!",
					                          "Configuration file for this control is currently in use by another user",
					                          MOGPromptButtons.RetryCancel) == MOGPromptResult.Retry)
				{
					goto OpenConfigIni;
				}
			}
		}
	}
}
