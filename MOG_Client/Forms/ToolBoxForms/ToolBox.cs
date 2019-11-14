using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MOG;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.DOSUTILS;
using MOG.INI;
using MOG.PROMPT;
using MOG.REPORT;
using MOG_Client.Client_Mog_Utilities;
using MOG_Client.Forms.ToolBoxForms;
using MOG_ControlsLibrary.Utils;
using System.Collections.Generic;

#region TODO List (comment)
/*
 *		NOT NECESSARILY IN ORDER:
 * 
 * *Make it so the user can use MOG-specific tags (<Project Root>, etc.)
 * *Make sure all input into all create/edit forms is correct (part 1)
 * *Check user input for Tag Name (in create/edit forms) to make sure input is valid without any Regex escape
 *	characters, such as *, \, ., (, ), etc. (part 2)
 * *Create a MogToolBox_FrameworkForm that can be used to encapsulate all the properties used by 
 *	ControlDefinition.  The form could then be inherited from and populated at design time with the
 *	correct information from Control Definition. 
 * *Make it so each create method accepts a ControlDefnition with everything assigned (but the controls)
 * *All forms should have appropriate tab indices
 * *Make it so we can parse commands, if necessary, and access other Custom Control tags AS commands...
 * *DONE: Replace invalid arguments (i.e. after a tag name change) at the click of a button in all create/edit forms.
 * *DONE: Add CheckBox and RadioButton controls.
 * *DONE: Pad all of our controls by at least 1 pixel (this changes EVERY Create() ).
 */
#endregion TODO List

#region How to add new controls (comment)
/*
 * Use the `(ADD_HERE)` text keyword located throughout ToolBox related items to see where you must
 * add ini, display, event, and processing logic for each custom form added to this ToolBox
 * 
 * Areas you will need to review/touch:
 *	Outside this class: Create a Form to setup your control.
 *	Add a Create-method for the Control Layout.
 *	Add a MenuItemClick() for the ContextMenu, MainContext (along with the MenuItem)
 *	LoadCustomButtons() -- Add logic for loading any kind of INI settings.
 *	WriteSettingsToIni() -- Same as LoadCustomButtons(), except in reverse order.
 *	EditControl_MenuItemClick() -- For the ContextMenu, PanelContext, add logic to edit the
 *		Control in a User-designed Form.
 *	Add a #region that contains event methods and a Create() for the CustomControl.  The Create()
 *		should have all the GUI logic needed to display the Control correctly within a Panel.
 *	Add any custom events that need to be inside the Custom Control.
 *	
 *	
 */
#endregion How to add new controls

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for ToolBox.
	/// </summary>
	public class ToolBox : System.Windows.Forms.UserControl
	{
		#region Consts/Variables/Properties

		#region GUI Consts/Variables
		/// <summary>
		/// The Control that is placed on the far right of the ToolBox's MainPanel should subtract this from its width
		/// </summary>
		public const int MainPanel_Right_Padding = 2;

		// GUI Text constants
		public const string Add_Text = "Add";
		public const string Panel_ToolTip_Text = "Right click or double-click here to change this ToolBox Control";
		public const string RadioButton_No_Arg_ToolTip = "No argument attached";
		public const string Evaluate_Text = " Evaluates to: ";
		public const string ProjectMenuItem_Text = "Add to Project...";
		public const string DepartmentMenuItem_Text = "Add to Department...";

		// Layout constants used in LayoutControls()
		const int Control_StartY=1;
		const int Control_StartX=0;
		// Indicates the padding to be used so that Button::Text displays correctly in a Button Control
		const int Button_TextPadding = 10;
		// This is used as default height for ComboBox and TextBox objects.
		const int Default_Control_Height = 20;
		// This is used as a general layout separator, to keep our controls from looking smooshed together
		const int General_Padding = 2;
		// This is for a checkbox to display the check that is boxed
		const int CheckBox_Padding = 20;
		// This is for our separator panel's height
		const int SeparatorPanel_Height = 5;
		const int SeparatorPanel_LeftWidth = 16;
				
		// Ini constants: Sections
		const string SubSectionIndicator_Text = "_";

		/// <summary> 
		/// Mutex set TRUE so we *write to the Ini* each time we create a CustomControl
		/// </summary>
		private bool bWriteToIni = true;

		#endregion GUI Consts/Variables
	
		// User Variables
		private MogMainForm mainForm;				// Pointer to the main form
		private string mPlatformName = "";				// Platform name (so that we only ask the DB once for it)
		//		private string mAllIniFilename;				// Ini filename to which all controls will be saved
		private string mDepartmentIniFilename = "";
		private string mUserIniFilename = "";
		private string mProjectIniFilename = "";
		internal MOG_Privileges mPrivileges;
		internal string mCurrentUserName = "";
		// Save values of whether user can save to Project or Department
//		private bool bSaveToProject;
//		private bool bSaveToDepartment;
		public System.Windows.Forms.ContextMenu MainContext;
        private ArrayList mSavedOriginalMenuItems;

		#region VS.NET GUI Variables

		private System.Windows.Forms.MenuItem MainContextAddButtonMenuItem;
		private System.Windows.Forms.MenuItem MainContextAddParameterMenuItem;
		private System.Windows.Forms.Panel ToolBoxMainPanel;
		private System.Windows.Forms.ContextMenu PanelContext;
		private System.Windows.Forms.MenuItem PanelContextEditMenuItem;
		private System.Windows.Forms.MenuItem PanelContextDeleteMenuItem;
		private System.Windows.Forms.MenuItem MainContextAddLabelMenuItem;
		private System.Windows.Forms.MenuItem PanelContextMoveUpMenuItem;
		private System.Windows.Forms.MenuItem PanelContextMoveDownMenuItem;
		private System.Windows.Forms.MenuItem MainContextAddIniComboMenuItem;
		private System.Windows.Forms.MenuItem MainContextAddFileBrowserComboMenuItem;
		private System.Windows.Forms.MenuItem PanelContextSeparator1;
		private System.Windows.Forms.MenuItem MainContextAddCheckBoxMenuItem;
		private System.Windows.Forms.MenuItem MainContextAddRadioButtonMenuItem;
		private System.Windows.Forms.MenuItem MainContextSeparator1;
		private System.Windows.Forms.MenuItem MainContextSeparator2;
		private System.Windows.Forms.MenuItem MainContextSeparator3;
		private System.Windows.Forms.ToolTip ToolBoxToolTip;
		private System.ComponentModel.IContainer components;
		#endregion VS.NET GUI Variables

		#region Properties
		/// <summary>
		/// Read-only ControlCollection pointing to ToolBoxMainPanel, which holds all our Custom Controls
		/// </summary>
		public ControlCollection CustomControls	{	get	{	return this.ToolBoxMainPanel.Controls;	}	}

		[Category( "MOGSpecific" ), 
		DescriptionAttribute( "Used at run-time to get and set the MainForm." )]
		public MogMainForm MainForm
		{	
			get	
			{	
				return this.mainForm;	
			}
			set	
			{	
				this.mainForm = value;	
			}
		}	

	
		private ArrayList mSystemArguments;
		/// <summary>
		/// Read-only list of SystemArguments
		/// </summary>
		public ArrayList SystemArguments
		{
			get
			{
				// If system arguments is null, initialize it...
				if( mSystemArguments == null )
				{
					mSystemArguments = new ArrayList();
					string[] systemTokens = MOG_Tokens.GetAllSystemTokens();
					AddTokensToArrayList( systemTokens, mSystemArguments );
				}
				return mSystemArguments;
			}
		}
		private ArrayList mProjectArguments;
		public ArrayList ProjectArguments
		{
			get
			{
				if( mProjectArguments == null )
				{
					mProjectArguments = new ArrayList();
					string[] projectTokens = MOG_Tokens.GetAllProjectTokens();
					AddTokensToArrayList( projectTokens, mProjectArguments );
				}
				return mProjectArguments;
			}
		}

		private MenuItem PanelContextSeparator2;
		private MenuItem PanelContextUserMenuItem;
		private MenuItem PanelContextDepartmentMenuItem;
		private MenuItem PanelContextProjectMenuItem;
	
		private ArrayList mTimeArguments;
		private MenuItem MainContextAddLinkLabelMenuItem;
	
		public ArrayList TimeArguments
		{
			get
			{
				if( mTimeArguments == null )
				{
					mTimeArguments = new ArrayList();
					string[] projectTokens = MOG_Tokens.GetAllTimeTokens();
					AddTokensToArrayList( projectTokens, mTimeArguments );
				}
				return mTimeArguments;
			}
		}
		private ArrayList mUserAndFileArguments;
		public ArrayList UserAndFileArguments
		{
			get
			{
				if( mUserAndFileArguments == null )
				{
					mUserAndFileArguments = new ArrayList();
					string[] projectTokens = MOG_Tokens.GetAllFilenameTokens();
					AddTokensToArrayList( projectTokens, mUserAndFileArguments );
				}
				return mUserAndFileArguments;
			}
		}
		#endregion Properties
	
		#endregion Variables/Properties

		/// <summary>
		/// Initialize our default ToolBox for the designer (must call Initialize() during GUI init)
		/// </summary>
		public ToolBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		#region Initialization, Display, and Setting Methods
		/// <summary>
		/// Initialize ToolBox for a given platform
		/// </summary>
		/// <param name="platformName"></param>
		public void Initialize( string platformName )
		{
			// Assign a value to our privileges
			this.mPrivileges = MOG_ControllerProject.GetPrivileges();
			// Assign our current user
			this.mCurrentUserName = MOG_ControllerProject.GetUserName_DefaultAdmin();

			// Create a MOG_Ini* we can use for all our INI creation
			MOG_Ini temp = new MOG_Ini("nullFilename");

			if (MOG_ControllerProject.GetProject() != null)
			{
				// Get the project ini filename
				this.mProjectIniFilename = MOG_ControllerProject.GetProject().GetProjectToolsPath()
					+ "\\ClientConfigs\\" + MOG_ControllerProject.GetProjectName()
					+ ".Client.Buttons." + platformName + ".info";

				// If this file does not exist, we will create it
				if (!DosUtils.FileExist(mProjectIniFilename))
				{
					temp.Save(mProjectIniFilename);
					temp.Close();
				}

				// Get the department ini filename
				this.mDepartmentIniFilename = MOG_ControllerProject.GetProject().GetProjectToolsPath()
					+ "\\ClientConfigs\\" + MOG_ControllerProject.GetUser().GetUserDepartment()
					+ ".Client.Buttons." + platformName + ".info";
				// If this file does not exist, we will create it
				if (!DosUtils.FileExist(mDepartmentIniFilename))
				{
					temp.Save(mDepartmentIniFilename);
					temp.Close();
				}

				// Get user ini filename
				this.mUserIniFilename = MOG_ControllerProject.GetUser().GetUserToolsPath()
					+ "\\ClientConfigs\\" + MOG_ControllerProject.GetProjectName()
					+ ".Client.Buttons." + platformName + ".userOnly.info";
				// If this file does not exist, we will create it
				if (!DosUtils.FileExist(mUserIniFilename))
				{
					temp.Save(mUserIniFilename);
					temp.Close();
				}
			}

			// Assign our global platformName
			this.mPlatformName = platformName;

			LoadCustomControls();
		}

		/// <summary>
		/// Load Buttons from Ini for given platformName
		/// </summary>
		public void LoadCustomControls()
		{
			// Suspend all our layouts, while we place our controls
			this.SuspendLayout();

			// If we have a valid user...
			if (  MOG_ControllerProject.IsUser())
			{
				// Clear any controls we might have
				if( this.ToolBoxMainPanel.Controls.Count > 0 )
				{
					// Give our ToolBoxMainPanel focus before we clear all these controls (glk: addresses problem encountered at Sensory Sweep)
					this.ToolBoxMainPanel.Focus();
					this.ToolBoxMainPanel.Controls.Clear();
				}

				// Create our custom controls for each tier
				CreateAllCustomControls();
			}
			if( this.CustomControls.Count > 0 )
			{
				LayoutControls();
				this.Visible = true;
			}
			else
			{
				this.Visible = false;
			}

			// Resume our Layout (and perform a layout), so the user can see the change(s)
			this.ResumeLayout( true );
		} // end ()

		/// <summary>
		/// Creates our Custom Controls from each of our available INIs (which available INIs are hard-coded)
		/// </summary>
		private void CreateAllCustomControls()
		{
			// Create our project tools
			CreateCustomControlsForLocation( ToolBoxControlLocation.Project );
			// Create our department tools
			CreateCustomControlsForLocation( ToolBoxControlLocation.Department );
			// Create any user tools we have left over
			CreateCustomControlsForLocation( ToolBoxControlLocation.User );

			// If we have not CustomControls, clear our MainPanel
			if( this.CustomControls.Count < 1 )
			{
				this.ToolBoxMainPanel.Controls.Clear();
			}
		}


		/// <summary>
		/// Creates Custom Controls for a given ToolBoxControlLocation
		/// </summary>
		/// <param name="currentLocation"></param>
		private void CreateCustomControlsForLocation( ToolBoxControlLocation currentLocation )
		{
			// Get the ini filename we need to be using
			string currentIniFilename = GetCurrentIniFilenameFromLocation( currentLocation );

			// If we have our ini file (double-check)...
			if (DosUtils.FileExist(currentIniFilename))
			{
				// Initialize ini file
				MOG_Ini pIni = new MOG_Ini(currentIniFilename);		

				// Don't write to the Ini for each control we create
				bWriteToIni = false;

				// Get a count of sections in this ini
				int count = pIni.CountSections();

				// See if we'll be adding any controls for this...
				int controlCount = 0;
				for( int i = 0; i < count; ++i)
				{
					// Initialize the section name we'll be looking into
					string section = pIni.GetSectionByIndexSLOW(i);
					if( !(section.IndexOf( SubSectionIndicator_Text ) > -1) )
					{
						++controlCount;
					}

					if( controlCount > 0 )
					{
						break;
					}
				}

				// If we have custom controls for Department, Add a separator panel...
				if( controlCount > 0 )
				{
					this.ToolBoxMainPanel.Controls.Add( GetNewSeparatorPanel( currentLocation ) );
				}

				SortedList<int, ControlDefinition> VisibilityControlSiblings = new SortedList<int, ControlDefinition>();

				// Starting at 0, go through each section we find.
				for( int i = 0 ; i < count ; ++i)
				{
					// Initialize the section name we'll be looking into
					string section = pIni.GetSectionByIndexSLOW(i);

					// If we have a sub-CustomControl section, skip it
					if (section.IndexOf(SubSectionIndicator_Text) > -1)
					{
						continue;
					}

					// Initialize and set all the variables for our ControlDefinition
					ControlDefinition def = new ControlDefinition(this, Environment.StackTrace);
					def.Load(this, pIni, section, currentLocation);

					// Check if this already exists in VisibilityControlSiblings?
					if (VisibilityControlSiblings.ContainsKey(def.VisibleIndex))
					{
						// Force this back to 0 so it will be reset properly
						def.VisibleIndex = 0;
					}
					// Does this have an uninitialized visible index?
					if (def.VisibleIndex == 0)
					{
						def.VisibleIndex = VisibilityControlSiblings.Count +1;
					}

					try
					{
						// Add this control to our list for tracking of the visibility index
						VisibilityControlSiblings.Add(def.VisibleIndex, def);
					}
					catch(Exception e)
					{
						e.ToString();
					}
				}

				// Now walk thru our list in the order of their visibility index
				foreach (KeyValuePair<int, ControlDefinition> control in VisibilityControlSiblings)
				{
					// Save the siblings of this control in its siblings property
					control.Value.Siblings = VisibilityControlSiblings;

					// Depending on what type we have, create the Custom Control
					switch( control.Value.ControlType )
					{
						case ToolBoxControlType.Button:
							CreateButton(control.Value);
							break;

						case ToolBoxControlType.ParameterButton:
							CreateParameterButton(control.Value);
							break;

						case ToolBoxControlType.Label:
							CreateLabel(control.Value);
							break;

						case ToolBoxControlType.LinkLabel:
							CreateLinkLabel(control.Value);
							break;

						case ToolBoxControlType.IniCombo:
							CreateIniComboBox(control.Value);
							break;

						case ToolBoxControlType.FileCombo:
							CreateFileComboBox(control.Value);
							break;

						case ToolBoxControlType.CheckBox:
							CreateCheckBox(control.Value);
							break;

						case ToolBoxControlType.RadioButton:
							CreateRadioButtonGroup(control.Value);
							break;						
					} // end switch
				} // end for

				// Turn back on our ability to write to the ini
				bWriteToIni = true;
			} // end if on existing ini
		}

		/// <summary>
		/// Writes settings to all our available INIs
		/// </summary>
		private void WriteSettingsToAllIni()
		{
			// Write settings for Project
			WriteSettingsToIni( ToolBoxControlLocation.Project );
			// Write settings for Department
			WriteSettingsToIni( ToolBoxControlLocation.Department );
			// Write settings for User
			WriteSettingsToIni( ToolBoxControlLocation.User );
		}

		/// <summary>
		/// Returns the INI filename given for saving to a given location
		/// </summary>
		/// <param name="currentLocation">Location to save to</param>
		/// <returns>The filename of the location to save to</returns>
		public string GetCurrentIniFilenameFromLocation( ToolBoxControlLocation currentLocation )
		{
			switch( currentLocation )
			{
				case ToolBoxControlLocation.Project:
					return mProjectIniFilename;

				case ToolBoxControlLocation.Department:
					return mDepartmentIniFilename;

				default:
					return mUserIniFilename;
			}
		}

		/// <summary>
		/// Writes out our settings to an ini file
		/// </summary>
		public void WriteSettingsToIni( ToolBoxControlLocation currentLocation )
		{
			string currentIniFilename = GetCurrentIniFilenameFromLocation(currentLocation);
			
			// If we should be writing to the Ini...
			if( bWriteToIni )
			{
				// Write our settings to our INIs (which are stored as global strings
				//	initialized in LoadCustomButtons() )...

				// Pull information into our MOG_Ini object and clear that information
				//TODO: This needs to change to a more specific filename
				MOG_Ini pIni = new MOG_Ini( );

			OpenConfigIni:
				if (pIni.Open(currentIniFilename, FileShare.Write))
				{
					// Go through each control and save settings
					foreach (Control control in CustomControls)
					{
						// Get the meta-object definition of our control
						ControlDefinition def = GetControlDefinition(control);

						// If we did not find a ControlDefinition, skip
						if (def == null)
						{
							continue;
						}

						// Create a GUID control section name if we need one
						if (def.ControlGuid.StartsWith("CustomControl", StringComparison.CurrentCultureIgnoreCase) || def.ControlGuid.Length == 0)
						{
							def.ControlGuid = Guid.NewGuid().ToString();
						}

						// Concat our section name
						string section = def.ControlGuid;

						// If we are not in the right location to be saving this control, skip it
						if (def.Location != currentLocation)
						{
							continue;
						}

						def.Save(pIni, section);

						def.SetToolTip(ToolBoxToolTip);
					}
					pIni.Save();
					// Close our MOG_Ini, now that we're done with it
					pIni.Close();
				}
				else
				{
					if (MOG_Prompt.PromptResponse("Configuration locked!", "Configuration file for this control is currently in use by another user", MOGPromptButtons.RetryCancel) == MOGPromptResult.Retry)
					{
						goto OpenConfigIni;
					}
				}
			} // end if on writing to Ini
		}

		/// <summary>
		/// Position controls left-to-right, top-to-bottom.  This should only be called from 
		/// LoadCustomControls() and Delete_MenuItemClick(), since it is not INI aware
		/// </summary>
		private void LayoutControls()
		{
			// Suspend layout while we're organizing our controls
			SuspendLayout();

			// Set our variables x and y based on our constants (above)
			int x = Control_StartX;
			int y = Control_StartY;


			// Store our last panel, through each iteration
			Panel lastPanel = null;

			// Foreach control, display it correctly...
			foreach( Control control in CustomControls )
			{
				// If we have a Panel... (This should always be true)
				if( control is Panel )
				{
					// Get our CustomControl's panel and meta-control
					Panel currentPanel = (Panel)control;
					ControlDefinition controlDef = GetControlDefinition( (object)control );

					// If we did not find a ControlDefinition (Separator Panel), continue to next panel
					if( controlDef != null )
					{
						// (ADD_HERE):  Add any ControlType-specific logic for organization here. 

						// Specialize our organization by control type
						switch( controlDef.ControlType )
						{
							default:
								break;
						}
					}
                    
					// If x (currentX coord) + currentPanel's width exceed our total width...
					if( (x + currentPanel.Width) > Width && lastPanel != null)
					{
						// x goes back to Control_StartX and y goes to the next line
						x = Control_StartX;
						if( lastPanel == null )
							y += currentPanel.Height + General_Padding;
						else
							y += lastPanel.Height + General_Padding;
					}
					// Our currentPanel's location will be ( x, (y - our scroll value) )
					currentPanel.Location = new Point( x, y  );

					// Add our currentPanel's width to x for the next panel to be added in
					x += currentPanel.Width + General_Padding;

					// Store this panel as our last panel
					lastPanel = currentPanel;

				}
					// Else, we should NEVER hit this line of code...
				else
					MessageBox.Show( this, "Error finding panel in ToolBox.Controls! --Gustavo's Problem." );
			}

			Invalidate( true );
			// Resume our layout and perform a layout
			ResumeLayout( true );
		}

		/// <summary>
		/// Allow Controls outside of this instance to clear our controls.
		/// </summary>
		public void Clear()	
		{
			foreach( Control control in ToolBoxMainPanel.Controls )
			{
				// If one of our child controls has focus, give focus to our parent before we clear
				if( control.Focused )
				{
					ToolBoxMainPanel.Focus();
					break;
				}
			}

			ToolBoxMainPanel.Controls.Clear();	
		}
		#endregion Display and Settings Methods

		#region PanelContextMenu event Methods
		private void PanelContextLocationSwitchMenuItem_Click(object sender, EventArgs e)
		{
			MenuItem selectedItem = (MenuItem)sender;
			ControlDefinition def = GetControlDefinition( ((MenuItem)sender).GetContextMenu().SourceControl  );

			// Figure out which location we need to switch to
			ToolBoxControlLocation locationToSwitchTo;
			if( selectedItem.Text == PanelContextDepartmentMenuItem.Text )
			{
				locationToSwitchTo = ToolBoxControlLocation.Department;
			}
			else if( selectedItem.Text == PanelContextUserMenuItem.Text )
			{
				locationToSwitchTo = ToolBoxControlLocation.User;
			}
			else // selectedItem == this.PanelContextProjectMenuItem )
			{
				locationToSwitchTo = ToolBoxControlLocation.Project;
			}

			// Switch our location to the given location, so long as we are not already there...
			switch( def.Location )
			{
				case ToolBoxControlLocation.Department:
				case ToolBoxControlLocation.User:
				case ToolBoxControlLocation.Project:
					if( locationToSwitchTo != def.Location )
					{
						// Remove this control from its current ini
						def.Remove(this);
						// Set the new location of this control
						def.Location = locationToSwitchTo;
						// This will trigger it to reset this the next time it is loaded
						def.VisibleIndex = 0;
						// Save all ini files
						WriteSettingsToAllIni();
						// Reload all ini files
						LoadCustomControls();
					}
					break;
					// We didn't get a valid case...  Programmer error.
				default:
					throw new Exception( "Invalid location chosen by programmer." );
			}
		}

		private void PanelContext_Popup(object sender, System.EventArgs e)
		{
			ControlDefinition def = GetControlDefinition( ((ContextMenu)sender).SourceControl );

			// Make sure all our MenuItems are enabled
			foreach( MenuItem item in PanelContext.MenuItems )
			{
				item.Enabled = true;
				item.Visible = true;
			}

			// Save values of whether user can save to Project or Department
			bool bSaveToProject = mPrivileges.GetUserPrivilege( mCurrentUserName, 
				MOG_PRIVILEGE.ConfigureProjectCustomTools );
			bool bSaveToDepartment = mPrivileges.GetUserPrivilege( mCurrentUserName, 
				MOG_PRIVILEGE.ConfigureDepartmentCustomTools );

			PanelContextValidateProjectUserDepartmentMenuItems( bSaveToProject, bSaveToDepartment );

			// If we are in Project and user does not have rights to edit...
			if( def.Location == ToolBoxControlLocation.Project && !bSaveToProject )
			{
				//MessageBox.Show( this, "Sorry, you ("+mCurrentUserName+") do not "
				//    +"have rights to edit this Custom Control", "Cannot Edit Control", MessageBoxButtons.OK,
				//    MessageBoxIcon.Information );
				foreach( MenuItem item in PanelContext.MenuItems )
				{
					// It has been requested that we not disable the viewing of buttons, just the editing of buttons.  Moved this enable=false to the actual editor
					if (item.Text != "Edit")
					{
						item.Enabled = false;
					}
				}
			}
				// Else, if we are in Department and use does not have right to edit...
			else if( def.Location == ToolBoxControlLocation.Department && !bSaveToDepartment )
			{
				//MessageBox.Show( this, "Sorry, you ("+mCurrentUserName+") do not "
				//    +"have rights to edit this Custom Control", "Cannot Edit Control", MessageBoxButtons.OK,
				//    MessageBoxIcon.Information );
				foreach( MenuItem item in PanelContext.MenuItems )
				{
					// It has been requested that we not disable the viewing of buttons, just the editing of buttons.  Moved this enable=false to the actual editor
					if (item.Text != "Edit")
					{
						item.Enabled = false;
					}
				}
			}

			PanelContextDepartmentMenuItem.Checked = false;
			PanelContextProjectMenuItem.Checked = false;
			PanelContextUserMenuItem.Checked = false;

			// Depending on location, decide which of our MenuItems should have a check beside it
			switch( def.Location )
			{
				case ToolBoxControlLocation.Department:
					PanelContextDepartmentMenuItem.Checked = true;
					break;
				case ToolBoxControlLocation.Project:
					PanelContextProjectMenuItem.Checked = true;
					break;
				case ToolBoxControlLocation.User:
					PanelContextUserMenuItem.Checked = true;
					break;
					// Throw if we've skipped all our valid cases (programmer error)
				default:
					throw new Exception( "An invalid Location has been chosen by programmer." );
			}
            MogUtil_VersionInfo.SetLightVersionControl(PanelContextDepartmentMenuItem);
            MogUtil_VersionInfo.SetLightVersionControl(PanelContextUserMenuItem);
		}

		private void PanelContextValidateProjectUserDepartmentMenuItems(bool bSaveToProject, bool bSaveToDepartment)
		{
			// If we are not able to save to Department AND not able to save to Project
			if( !bSaveToDepartment && !bSaveToProject )
			{
				// Completely keep the user from seeing what they are missing...
				PanelContextUserMenuItem.Visible = false;
				PanelContextProjectMenuItem.Visible = false;
				PanelContextDepartmentMenuItem.Visible = false;
				PanelContextSeparator2.Visible = false;
			}
				// Else, if we have either no Department xOR no Project save ability...
				//  NOTE:  This is an xOR because above, we cover the AND case...
			else if( !bSaveToDepartment || !bSaveToProject )
			{
				// Disable the DepartmentMenuItem...
				if( !bSaveToDepartment )
				{
					PanelContextDepartmentMenuItem.Enabled = false;
				}
				// Disable ProjectMenuItem
				if( !bSaveToProject )
				{
					PanelContextProjectMenuItem.Enabled = false;
				}
			}
			// Else, we should be able to see everything
		}

		/// <summary>
		/// Process deletion of a Custom Control
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PanelContextDeleteControl_MenuItemClick(object sender, System.EventArgs e)
		{
			// Get our control
			Control control = (Control)((MenuItem)sender).GetContextMenu().SourceControl;

			// Make sure we have a Panel
			if( control != null && control is Panel )
			{
				// Get the meta-object definition of our control
				ControlDefinition def = GetControlDefinition(control);
				def.Remove(this);

				// Reload all ini files
				LoadCustomControls();
			}
		}

		/// <summary>
		/// Edit a Control
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PanelContextEditControl_MenuItemClick(object sender, System.EventArgs e)
		{
			try
			{
				// Reload our Controls
				//LoadCustomControls();

				DialogResult result = DialogResult.Cancel;
				MenuItem item = (MenuItem)sender;
				ContextMenu menu = item.GetContextMenu();
				ControlDefinition def = GetControlDefinition((object)menu.SourceControl);
				SuspendLayout();

				// Load off the file
				def.UpdateControl(this);

				// Depending on the ControlType, populate then display the form to edit it...
				switch (def.ControlType)
				{
					// (ADD_HERE):  Any control must have logic here.  

					// If we have a Button or a ParameterButton
					case ToolBoxControlType.Button:
					case ToolBoxControlType.ParameterButton:
						EditParameterButtonForm buttonForm = new EditParameterButtonForm(this);
						buttonForm.StartPosition = FormStartPosition.CenterParent;

						buttonForm.NameTextBox.Text = def.DefButton.Text;
						buttonForm.CommandTextBox.Text = def.Command;
						buttonForm.ToolTipTextBox.Text = def.ToolTipString;
						if ((ToolBoxControlType)def.ControlType == ToolBoxControlType.Button)
						{
							buttonForm.TagTextBox.ReadOnly = true;
							buttonForm.TagTextBox.Text = def.TagName;
							buttonForm.ArgumentsTextBox.Text = def.Arguments;
						}
						else
						{
							buttonForm.TagTextBox.Text = def.TagName;
							buttonForm.ArgumentsTextBox.Text = def.DefTextBox.Text;
							if (def.Arguments.Length > 0 && def.DefTextBox.Text != def.Arguments)
								buttonForm.ArgumentsTextBox.Text += " " + def.Arguments;
						}

						buttonForm.HideOutputCheckBox.Checked = def.HideOutput;

						buttonForm.SetControlEnableBasedOnPrivilege(def.Location);

						result = buttonForm.ShowDialog();
						if (result == DialogResult.OK)
						{
							def.DefButton.Text = buttonForm.NameTextBox.Text;
							def.ControlName = def.DefButton.Text;
							def.Command = buttonForm.CommandTextBox.Text;
							def.HideOutput = buttonForm.HideOutputCheckBox.Checked;
							def.ToolTipString = buttonForm.ToolTipTextBox.Text;
							def.TagName = buttonForm.TagTextBox.Text;

							if ((ToolBoxControlType)def.ControlType == ToolBoxControlType.Button)
								def.Arguments = buttonForm.ArgumentsTextBox.Text;

							if ((ToolBoxControlType)def.ControlType == ToolBoxControlType.ParameterButton)
							{
								def.ControlName = buttonForm.TagTextBox.Text;
								def.DefTextBox.Text = buttonForm.ArgumentsTextBox.Text;
							}
						}
						break;

					case ToolBoxControlType.Label:
						ToolBoxLabelForm labelForm = new ToolBoxLabelForm(this);
						labelForm.StartPosition = FormStartPosition.CenterParent;

						labelForm.LabelNameTextBox.Text = def.DefLabel.Text;

						labelForm.SetControlEnableBasedOnPrivilege(def.Location);

						result = labelForm.ShowDialog();
						if (result == DialogResult.OK)
						{
							def.ControlName = labelForm.LabelNameTextBox.Text;
						}
						break;

					case ToolBoxControlType.LinkLabel:
						ToolBoxLinkLabelForm linkLabelForm = new ToolBoxLinkLabelForm(this);
						linkLabelForm.StartPosition = FormStartPosition.CenterParent;

						linkLabelForm.LabelNameTextBox.Text = def.DefLinkLabel.Text;
						linkLabelForm.LabelLinkTextBox.Text = def.TagName;

						linkLabelForm.SetControlEnableBasedOnPrivilege(def.Location);

						result = linkLabelForm.ShowDialog();
						if (result == DialogResult.OK)
						{
							def.ControlName = linkLabelForm.LabelNameTextBox.Text;
							def.TagName = linkLabelForm.LabelLinkTextBox.Text;
						}
						break;

					case ToolBoxControlType.IniCombo:
						ToolBoxIniComboBoxForm comboForm = new ToolBoxIniComboBoxForm(this);
						comboForm.StartPosition = FormStartPosition.CenterParent;

						comboForm.mStartPath = def.DialogStartPath;
						comboForm.PathTextBox.Text = def.IniFilename;
						comboForm.mSelectedSection = def.IniSectionName;
						comboForm.NameTextBox.Text = def.ControlName;
						comboForm.TagTextBox.Text = def.TagName;
						comboForm.ToolTipTextBox.Text = def.ToolTipString;
						comboForm.InitializeComboBox(def.IniFilename);

						comboForm.SetControlEnableBasedOnPrivilege(def.Location);

						result = comboForm.ShowDialog();
						if (result == DialogResult.OK)
						{
							def.FolderName = comboForm.mStartPath;
							def.IniFilename = comboForm.PathTextBox.Text;
							def.ControlName = comboForm.NameTextBox.Text;
							def.TagName = comboForm.TagTextBox.Text;
							def.IniSectionName = comboForm.SectionComboBox.Text;
							def.ToolTipString = comboForm.ToolTipTextBox.Text;
						}
						break;

					case ToolBoxControlType.FileCombo:
						ToolBoxFileComboBoxForm fileForm = new ToolBoxFileComboBoxForm(this);
						fileForm.StartPosition = FormStartPosition.CenterParent;

						fileForm.PathTextBox.Text = def.FolderName;
						fileForm.NameTextBox.Text = def.ControlName;
						fileForm.TagTextBox.Text = def.TagName;
						fileForm.ToolTipTextBox.Text = def.ToolTipString;
						fileForm.DepthTextBox.Text = def.ComboBoxDepth.ToString();
						fileForm.DefComboBox = def.DefComboBox;
						
						fileForm.ShowFullPaths = def.ShowFullPaths;
						fileForm.ShowWorkspaceRelativePaths = def.ShowWorkspaceRelativePaths;
						fileForm.ShowFolderRelativePaths = def.ShowFolderRelativePaths;
						fileForm.ShowBasePaths = def.ShowBasePaths;

						fileForm.TagFullPaths = def.TagFullPaths;
						fileForm.TagWorkspaceRelativePaths = def.TagWorkspaceRelativePaths;
						fileForm.TagFolderRelativePaths = def.TagFolderRelativePaths;
						fileForm.TagBasePaths = def.TagBasePaths;

						fileForm.RecurseFolder = def.RecurseFolders;
						fileForm.PatternTextBox.Text = def.Pattern;

						fileForm.SetControlEnableBasedOnPrivilege(def.Location);

						result = fileForm.ShowDialog();
						if (result == DialogResult.OK)
						{
							def.FolderName = fileForm.PathTextBox.Text;
							def.ControlName = fileForm.NameTextBox.Text;
							def.TagName = fileForm.TagTextBox.Text;
							def.ToolTipString = fileForm.ToolTipTextBox.Text;
							def.ComboBoxDepth = fileForm.ComboBoxDepth;
							
							def.ShowFullPaths = fileForm.ShowFullPaths;
							def.ShowWorkspaceRelativePaths = fileForm.ShowWorkspaceRelativePaths;
							def.ShowFolderRelativePaths = fileForm.ShowFolderRelativePaths;
							def.ShowBasePaths = fileForm.ShowBasePaths;

							def.TagFullPaths= fileForm.TagFullPaths;
							def.TagWorkspaceRelativePaths = fileForm.TagWorkspaceRelativePaths;
							def.TagFolderRelativePaths = fileForm.TagFolderRelativePaths;
							def.TagBasePaths = fileForm.TagBasePaths;

							def.RecurseFolders = fileForm.RecurseFolder;
							def.Pattern = fileForm.PatternTextBox.Text;
						}
						PopulateFolderComboBox(def);
						break;

					case ToolBoxControlType.CheckBox:
						ToolBoxCheckBoxForm checkForm = new ToolBoxCheckBoxForm(this);
						checkForm.StartPosition = FormStartPosition.CenterParent;

						checkForm.TagTextBox.Text = def.TagName;
						checkForm.FalseArgumentTextBox.Text = def.Arguments;
						checkForm.ControlNameTextBox.Text = def.ControlName;
						checkForm.ToolTipTextBox.Text = def.ToolTipString;
						checkForm.FalseArgumentTextBox.Text = def.ArgumentFalse;
						checkForm.TrueArgumentTextBox.Text = def.ArgumentTrue;

						checkForm.SetControlEnableBasedOnPrivilege(def.Location);

						result = checkForm.ShowDialog();
						if (result == DialogResult.OK)
						{
							def.TagName = checkForm.TagTextBox.Text;
							def.Arguments = checkForm.FalseArgumentTextBox.Text;
							def.ControlName = checkForm.ControlNameTextBox.Text;
							def.ToolTipString = checkForm.ToolTipTextBox.Text;
							def.ArgumentFalse = checkForm.FalseArgumentTextBox.Text;
							def.ArgumentTrue = checkForm.TrueArgumentTextBox.Text;
						}
						break;

					case ToolBoxControlType.RadioButton:
						ToolBoxRadioButtonForm radioForm = new ToolBoxRadioButtonForm(this, def.RadioButtons);
						radioForm.StartPosition = FormStartPosition.CenterParent;

						//radioForm.RadioButtons = def.RadioButtons;
						radioForm.NameTextBox.Text = def.ControlName;
						radioForm.TagTextBox.Text = def.TagName;
						radioForm.ToolTipTextBox.Text = def.ToolTipString;

						radioForm.SetControlEnableBasedOnPrivilege(def.Location);

						result = radioForm.ShowDialog();
						if (result == DialogResult.OK)
						{
							def.Arguments = radioForm.Arguments;
							def.RadioButtons = radioForm.RadioButtons;
							def.ControlName = radioForm.NameTextBox.Text;
							def.TagName = radioForm.TagTextBox.Text;
							def.ToolTipString = radioForm.ToolTipTextBox.Text;
						}
						break;
				}
				ResumeLayout();

				if (result != DialogResult.Cancel)
				{
					// Write our change(s) out
					WriteSettingsToAllIni();
					// Reload our Controls
					LoadCustomControls();
				}
			}
			catch(Exception ex)
			{
				ex.ToString();
			}
		}

		/// <summary>
		/// Moves an item up in ToolBoxMainPanel's list of controls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PanelContextMoveUp_Click(object sender, System.EventArgs e)
		{
			MenuItem menuItem = (MenuItem)sender;
			ContextMenu context = menuItem.GetContextMenu();
			Control control = (Control)context.SourceControl;

			if (control is Panel)
			{
				// Get the meta-object definition of our control
				ControlDefinition def = GetControlDefinition(control);

				def.MoveUp();

				WriteSettingsToAllIni();
				LoadCustomControls();
			}
		}

		/// <summary>
		/// Moves an item down in ToolBoxMainPanel's list of controls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PanelContextMoveDown_Click(object sender, System.EventArgs e)
		{	
			MenuItem menuItem = (MenuItem)sender;
			ContextMenu context = menuItem.GetContextMenu();
			Control control = (Control)context.SourceControl;

			if (control is Panel)
			{
				// Get the meta-object definition of our control
				ControlDefinition def = GetControlDefinition(control);

				def.MoveDown();

				WriteSettingsToAllIni();
				LoadCustomControls();
			}
		} // end ()
		#endregion PanelContextMenu event Methods

		#region MainContextMenu event Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender">Should never be anything other than a ContextMenu</param>
		/// <param name="e"></param>
		private void MainContext_Popup(object sender, System.EventArgs e)
		{
			try
			{
				// If we have no localdata branches...
				if( MOG_ControllerProject.GetCurrentSyncDataController() == null )
				{
					SwitchMainContextMenuItems( MainContext.MenuItems, false );
				}
				else
				{
					if (mSavedOriginalMenuItems == null)
                    {
                        mSavedOriginalMenuItems = GetArrayListOfMenuItems((ContextMenu)sender);
                    }
					MenuItem project = new MenuItem( ProjectMenuItem_Text );
					MenuItem department = new MenuItem( DepartmentMenuItem_Text );

                    MogUtil_VersionInfo.SetLightVersionControl(department);

					// Save values of whether user can save to Project or Department
					bool bSaveToProject = mPrivileges.GetUserPrivilege( mCurrentUserName, 
						MOG_PRIVILEGE.ConfigureProjectCustomTools );
					bool bSaveToDepartment = mPrivileges.GetUserPrivilege( mCurrentUserName, 
						MOG_PRIVILEGE.ConfigureDepartmentCustomTools );

					// If we need to add a Project or a Department MenuItem...
					if( (bSaveToProject || bSaveToDepartment) 
						&& MainContext.MenuItems.Count > 0 )
					{
						// Clear existing MenuItems (which have been saved)
						MainContext.MenuItems.Clear();
						// Add back our UserMenuItems
                        AddArrayListOfMenuItemsToCollection(mSavedOriginalMenuItems, MainContext.MenuItems, true, true);

                        // If we don't have a final separator, add it
						if( MainContext.MenuItems[ MainContext.MenuItems.Count-1 ].Text != "-" )
						{
							MainContext.MenuItems.Add( new MenuItem("-") );
						}
					}

					// If user has right to add custom control to Project OR Department...
					if( bSaveToProject && bSaveToDepartment )
					{
						MainContext.MenuItems.Add( project );
                        AddArrayListOfMenuItemsToCollection(mSavedOriginalMenuItems, project.MenuItems, true, false);
						MainContext.MenuItems.Add( department );
                        AddArrayListOfMenuItemsToCollection(mSavedOriginalMenuItems, department.MenuItems, true, true);
					}
						// Else, if user only has right to save to project
					else if( bSaveToProject && !bSaveToDepartment )
					{
						MainContext.MenuItems.Add( project );
                        AddArrayListOfMenuItemsToCollection(mSavedOriginalMenuItems, project.MenuItems, true, false);
					}
						// Else, if user only has right to save to department
					else if ( bSaveToDepartment && !bSaveToProject )
					{
						MainContext.MenuItems.Add( department );
                        AddArrayListOfMenuItemsToCollection(mSavedOriginalMenuItems, department.MenuItems, true, true);
					}
					// Else, eat popup

                    if (MOG_Main.IsLicensed())
                    {
                        SwitchMainContextMenuItems(MainContext.MenuItems, true);
                    }
				}
			}
			catch(Exception ex)
			{
				MOG_Report.ReportMessage("MainContext_Popup", ex.Message, ex.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.CRITICAL);
			}		
		}

		/// <summary>
		/// Add a new Command Button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddButton_MenuItemClick(object sender, System.EventArgs e)
		{
			if(ProjectCheck())		
				return;

			EditParameterButtonForm editForm = new EditParameterButtonForm( this );

			// Position the dialogue at center of MOGMainForm
			editForm.StartPosition = FormStartPosition.CenterParent;
			editForm.TagTextBox.ReadOnly = true;

			editForm.SetControlEnableBasedOnPrivilege(GetMenuItemLocation((MenuItem)sender));

			if (editForm.ShowDialog() == DialogResult.OK)
			{
				ControlDefinition def = new ControlDefinition( this, Environment.StackTrace );
				def.ControlName =  editForm.NameTextBox.Text;
				def.Command = editForm.CommandTextBox.Text;
				def.Arguments = editForm.ArgumentsTextBox.Text;
				def.ToolTipString = editForm.ToolTipTextBox.Text;
				def.HideOutput = editForm.HideOutputCheckBox.Checked;
				def.Location = GetMenuItemLocation( (MenuItem)sender );

				CreateButton( def );
				WriteSettingsToIni( def.Location );
				LoadCustomControls();
				return;
			}
		}
		
		/// <summary>
		/// Add a new ParameterButton
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddParamButton_MenuItemClick(object sender, System.EventArgs e)
		{
			if(ProjectCheck())		
				return;

			EditParameterButtonForm editForm = new EditParameterButtonForm( this );
			editForm.StartPosition = FormStartPosition.CenterParent;

			editForm.SetControlEnableBasedOnPrivilege(GetMenuItemLocation((MenuItem)sender));

			if (editForm.ShowDialog() == DialogResult.OK)
			{
				ControlDefinition def = new ControlDefinition( this, Environment.StackTrace );
				def.ControlName = editForm.NameTextBox.Text;
				def.TagName = editForm.TagTextBox.Text;
				def.Arguments = editForm.ArgumentsTextBox.Text;
				def.Command = editForm.CommandTextBox.Text;
				def.ToolTipString = editForm.ToolTipTextBox.Text;
				def.HideOutput = editForm.HideOutputCheckBox.Checked;
				def.Location = GetMenuItemLocation( (MenuItem)sender );

				CreateParameterButton( def );
				WriteSettingsToIni( def.Location );
				LoadCustomControls();
				return;
			}
		}

		/// <summary>
		/// Add a new Label
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddLabel_MenuItemClick(object sender, System.EventArgs e)
		{
			try
			{
				if(ProjectCheck())		
					return;

				ToolBoxLabelForm editForm = new ToolBoxLabelForm(this);

				// Position the dialogue at center of MOGMainForm
				editForm.StartPosition = FormStartPosition.CenterParent;

				editForm.SetControlEnableBasedOnPrivilege(GetMenuItemLocation( (MenuItem)sender ));

				if (editForm.ShowDialog() == DialogResult.OK)
				{
					ControlDefinition def = new ControlDefinition( this, Environment.StackTrace );
					def.ControlName = editForm.LabelNameTextBox.Text;
					def.Location = GetMenuItemLocation( (MenuItem)sender );

					CreateLabel( def );
					WriteSettingsToIni( def.Location );
					LoadCustomControls();
					return;
				}
			}
				// For now, eat any error(s) we get
			catch( Exception ex )
			{
				MessageBox.Show(this, "Error occured:\n\n" + ex);
			}
		}
		
		private void MainContextAddLinkLabelMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				if (ProjectCheck())
					return;

				ToolBoxLinkLabelForm editForm = new ToolBoxLinkLabelForm(this);

				// Position the dialogue at center of MOGMainForm
				editForm.StartPosition = FormStartPosition.CenterParent;

				editForm.SetControlEnableBasedOnPrivilege(GetMenuItemLocation((MenuItem)sender));

				if (editForm.ShowDialog() == DialogResult.OK)
				{
					ControlDefinition def = new ControlDefinition(this, Environment.StackTrace);
					def.ControlName = editForm.LabelNameTextBox.Text;
					def.TagName = editForm.LabelLinkTextBox.Text;
					def.Location = GetMenuItemLocation((MenuItem)sender);

					CreateLinkLabel(def);
					WriteSettingsToIni(def.Location);
					LoadCustomControls();
					return;
				}
			}
			// For now, eat any error(s) we get
			catch (Exception ex)
			{
				MessageBox.Show(this, "Error occured:\n\n" + ex.ToString());
			}
		}

		/// <summary>
		/// Add a new IniComboBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddIniCombo_MenuItemClick(object sender, System.EventArgs e)
		{
			try
			{
				if(ProjectCheck())		
					return;

				ToolBoxIniComboBoxForm editForm = new ToolBoxIniComboBoxForm(this);
				// Set the path for the file browser (by proxy)
				editForm.mStartPath = MOG_ControllerProject.GetProject().GetProjectToolsPath() + "\\UserConfigs";
				// If we do not have a directory, try using our gamedata path
				if( !DosUtils.DirectoryExist( editForm.mStartPath ) && MOG_ControllerProject.GetCurrentSyncDataController() != null)
				{
					editForm.mStartPath = MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory();
				}
				editForm.StartPosition = FormStartPosition.CenterParent;

				editForm.SetControlEnableBasedOnPrivilege(GetMenuItemLocation( (MenuItem)sender ));

				if (editForm.ShowDialog() == DialogResult.OK)
				{
					ControlDefinition def = new ControlDefinition( ToolBoxControlType.IniCombo, this, Environment.StackTrace );
					def.ControlName = editForm.NameTextBox.Text;
					def.ToolTipString = editForm.ToolTipTextBox.Text;
					def.IniFilename = editForm.PathTextBox.Text;
					def.IniSectionName = (string)editForm.SectionComboBox.SelectedItem;
					def.DialogStartPath = editForm.mStartPath;
					def.Location = GetMenuItemLocation( (MenuItem)sender );

					CreateIniComboBox( def );
					WriteSettingsToIni( def.Location );
					LoadCustomControls();
					return;
				}
			}
				// For now, eat any error(s) we get
			catch( Exception ex )
			{
				MessageBox.Show(this, "Error occured:\n\n" + ex.ToString());
			}

		}

		/// <summary>
		/// TODO: Finish implementing this...
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddFileBrowserCombo_MenuItemClick(object sender, System.EventArgs e)
		{
			try
			{
				// If we do not have a valid project, return
				if(ProjectCheck())		
					return;

				ToolBoxFileComboBoxForm editForm = new ToolBoxFileComboBoxForm(this);
				editForm.SetControlEnableBasedOnPrivilege(GetMenuItemLocation( (MenuItem)sender ));
				if (editForm.ShowDialog() == DialogResult.OK)
				{
					ControlDefinition def = new ControlDefinition( this, Environment.StackTrace );
					def.TagName = editForm.TagTextBox.Text;
					def.ControlName = editForm.NameTextBox.Text;
					def.ToolTipString = editForm.ToolTipTextBox.Text;
					def.FolderName = editForm.SelectedPath;
					def.Pattern = editForm.PatternTextBox.Text;
					def.ComboBoxDepth = editForm.ComboBoxDepth;
					def.ShowFullPaths = editForm.ShowFullPaths;
					def.TagFullPaths = editForm.TagFullPaths;
					def.TagWorkspaceRelativePaths = editForm.TagWorkspaceRelativePaths;
					def.TagFolderRelativePaths = editForm.TagFolderRelativePaths;
					def.TagBasePaths = editForm.TagBasePaths;
					def.RecurseFolders = editForm.RecurseFolder;
					def.Location = GetMenuItemLocation( (MenuItem)sender );

					CreateFileComboBox( def );
					WriteSettingsToIni( def.Location );
					LoadCustomControls();
				}
			}
				// For now, eat any error(s) we get
			catch( Exception ex )
			{
				MOG_Report.ReportMessage("Error occured in Toolbox File Browser Creation!",
					ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
			}

		} // end ()

		/// <summary>
		/// TODO: Finish implementing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddRadioButton_MenuItemClick(object sender, System.EventArgs e)
		{
			try
			{
				ToolBoxRadioButtonForm radioForm = new ToolBoxRadioButtonForm( this );
				radioForm.SetControlEnableBasedOnPrivilege(GetMenuItemLocation((MenuItem)sender));
				if (radioForm.ShowDialog() == DialogResult.OK)
				{
					ControlDefinition def = new ControlDefinition( this, Environment.StackTrace );

					def.RadioButtons = radioForm.RadioButtons;
					def.ControlName = radioForm.NameTextBox.Text;
					def.TagName = radioForm.TagTextBox.Text;
					def.ToolTipString = radioForm.ToolTipTextBox.Text;
					def.Arguments = radioForm.Arguments;
					def.LastSelectedIndex = radioForm.LastSelectedIndex;
					def.Location = GetMenuItemLocation( (MenuItem)sender );

					CreateRadioButtonGroup( def );
					WriteSettingsToIni( def.Location );
					LoadCustomControls();
				}
			}
				// For now, eat any error(s) we get
			catch( Exception ex )
			{
				MOG_Prompt.PromptMessage( "Error Adding Radio Button Group", ex.Message, ex.StackTrace,
					MOG_ALERT_LEVEL.ERROR );
			}

		}

		/// <summary>
		/// TODO: Finish comment
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddCheckBox_MenuItemClick(object sender, System.EventArgs e)
		{
			try
			{
				ToolBoxCheckBoxForm checkForm = new ToolBoxCheckBoxForm( this );
				checkForm.SetControlEnableBasedOnPrivilege(GetMenuItemLocation((MenuItem)sender));
				if (checkForm.ShowDialog() == DialogResult.OK)
				{
					ControlDefinition def = new ControlDefinition( this, Environment.StackTrace );
					def.TagName = checkForm.TagTextBox.Text;
					def.Arguments = checkForm.FalseArgumentTextBox.Text;
					def.ControlName = checkForm.ControlNameTextBox.Text;
					def.ToolTipString = checkForm.ToolTipTextBox.Text;
					def.ArgumentFalse = checkForm.FalseArgumentTextBox.Text;
					def.ArgumentTrue = checkForm.TrueArgumentTextBox.Text;
					def.Location = GetMenuItemLocation( (MenuItem)sender );
				
					CreateCheckBox( def );
					WriteSettingsToIni( def.Location );
					LoadCustomControls();
				}
			}
				// For now, eat any error(s) we get
			catch( Exception ex )
			{
				MessageBox.Show(this, "Error occured:\n\n" + ex.ToString());
			}

		}
		#endregion MainContextMenu event Methods

		#region Create Controls

		#region Add Label
		/// <summary>
		/// Create a new label and add it to the controls list
		/// </summary>
		private ControlDefinition CreateLabel( ControlDefinition def )
		{
			def.ControlType = ToolBoxControlType.Label;

//			def.DefContextPanel = GetNewContextPanel();

			def.DefLabel = new Label();
			def.DefLabel.Location = new System.Drawing.Point(General_Padding, 0);
			def.DefLabel.Text = def.ControlName;
			def.ControlName = def.DefLabel.Text;
			// Get width based on our GUI text width rounded to nearest int (casting to int floors floats)
			def.DefLabel.Width = (int)
				(def.DefLabel.CreateGraphics().MeasureString( def.DefLabel.Text, def.DefLabel.Font ).Width + 0.5f)
				+ (2*General_Padding);
			def.DefLabel.FlatStyle = FlatStyle.System;
			def.DefLabel.BorderStyle = BorderStyle.None;
			def.DefLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

			Panel panel = new Panel();
			panel.Height = def.DefLabel.Height;
			//panel.Width = def.mContextPanel.Width + def.mLabel.Width;
			panel.Width = def.DefLabel.Width;
			panel.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			panel.Controls.Add( def );
//			panel.Controls.Add( def.DefContextPanel );
			panel.Controls.Add( def.DefLabel );

			panel.ContextMenu = this.PanelContext;

			this.ToolBoxMainPanel.Controls.Add( panel );

			return def;
		}
		#endregion Add Label

		#region Add Link Label
		/// <summary>
		/// Create a new label and add it to the controls list
		/// </summary>
		private ControlDefinition CreateLinkLabel(ControlDefinition def)
		{
			def.ControlType = ToolBoxControlType.LinkLabel;

			//			def.DefContextPanel = GetNewContextPanel();

			def.DefLinkLabel = new LinkLabel();
			def.DefLinkLabel.Location = new System.Drawing.Point(General_Padding, 0);
			def.DefLinkLabel.Text = def.ControlName;
			def.ControlName = def.DefLinkLabel.Text;
			def.DefLinkLabel.LinkClicked += DefLinkLabel_LinkClicked;
			// Get width based on our GUI text width rounded to nearest int (casting to int floors floats)
			def.DefLinkLabel.Width = (int)
				(def.DefLinkLabel.CreateGraphics().MeasureString(def.DefLinkLabel.Text, def.DefLinkLabel.Font).Width + 0.5f)
				+ (2 * General_Padding);
			def.DefLinkLabel.FlatStyle = FlatStyle.System;
			def.DefLinkLabel.BorderStyle = BorderStyle.None;
			def.DefLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

			Panel panel = new Panel();
			panel.Height = def.DefLinkLabel.Height;
			//panel.Width = def.mContextPanel.Width + def.mLabel.Width;
			panel.Width = def.DefLinkLabel.Width;
			panel.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			panel.Controls.Add(def);
			//			panel.Controls.Add( def.DefContextPanel );
			panel.Controls.Add(def.DefLinkLabel);

			panel.ContextMenu = PanelContext;

			ToolBoxMainPanel.Controls.Add(panel);

			return def;
		}

		void DefLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ControlDefinition def = GetControlDefinition(sender);

			string target = def.TagName as string;

			try
			{
				System.Diagnostics.Process.Start(target);
			}
			catch (Exception ex)
			{
				MOG_Report.ReportMessage("Toolbox: Error Executing Command",
					"MOG cannot open the following command: " + target + "\r\n\r\nFor the following reason:\r\n\t"
					+ ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
			}
		}
		#endregion Add Link Label

		#region Add Button
		/// <summary>
		/// Create a standard MOG toolbox button
		/// </summary>
		private void CreateButton( ControlDefinition def )
		{
			string buttonName = def.ControlName;
			string Command = def.Command;
			string arguments = def.Arguments;
			string toolTipString = def.ToolTipString;
			bool hiddenOutput = def.HideOutput;
			def.ControlType = ToolBoxControlType.Button;

			def.Command = Command;
			def.HideOutput = hiddenOutput;
			def.Arguments = arguments;

			def.DefButton = new Button();
			def.DefButton.Location = new System.Drawing.Point(0, 0);
			def.DefButton.Text = buttonName;
			def.ControlName = def.DefButton.Text;
			def.DefButton.FlatStyle = FlatStyle.System;
			def.DefButton.Width = (int)
				(def.DefButton.CreateGraphics().MeasureString( def.DefButton.Text, def.DefButton.Font ).Width + 0.5f) 
				+ Button_TextPadding;
			def.DefButton.Name = "" + CustomControls.Count;
			def.DefButton.Click += this.DefButton_Click;
			//def.mButton.ContextMenu = this.ButtonContext;
			def.DefButton.Anchor = AnchorStyles.Left | AnchorStyles.Top;

			Panel panel = new Panel();
			panel.Height = def.DefButton.Height;
			panel.Width = def.DefButton.Width;

			if( toolTipString.Length < 1 )
			{
				toolTipString = def.ControlName;
			}
			def.ToolTipString = toolTipString;
			def.SetToolTip( ToolBoxToolTip );

			panel.Controls.Add( def );
			panel.Controls.Add( def.DefButton );

			panel.ContextMenu = PanelContext;

			// Add the button to the form.
			ToolBoxMainPanel.Controls.Add(panel);
			//mControlsList.Add(def);
		}

		/// <summary>
		/// Handle click for a Simple or Parameter Button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DefButton_Click(object sender, System.EventArgs e)
		{
			string output = "";

			ControlDefinition def = GetControlDefinition( sender );

			// Save our command as relational (if needed)
//Keep this comment
//			SaveRelationalCommand( def );

			string args = ParseArguments( def.Arguments );
			string command = ParseArguments( def.Command );

			// If we have no command, try passing our first argument to our shell as the command
			int argsSplitLength = args.Split(" ".ToCharArray()).Length;
			// If we have a 0 length command, OR command is 1 length empty string AND
			//	splitting args gives us more than one array element AND args length is greater than 1
			if( command.Length == 1 || command == " " && argsSplitLength > 0 && args.Length > 1 )
			{
				command = args.Split(" ".ToCharArray())[0];
				args = args.Replace( command, "" );
			}

			try
			{
				// If we cannot find our command...
				if( !DosUtils.FileExist( command ) )
				{
					// Try using the command as a local command
					command = ParseRelationalCommand( command );
				}

				guiCommandLine.ShellSpawn(command, args, def.HideOutput ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal, ref output);
				// glk: From ControlDefinition, it would be possible to attach a delegate here to 
				// process information from output and display it to the user, if desired...
			}
			catch( Exception ex )
			{
				MOG_Report.ReportMessage( "Toolbox: Error Executing Command", 
					"MOG cannot open the following command: " + command + "\r\n\r\nFor the following reason:\r\n\t"
					+ ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
			}
		}
		#endregion Add Button

		#region Add ParameterButton
		/// <summary>
		/// Create an edit Mog button
		/// </summary>
		private void CreateParameterButton( ControlDefinition def )
		{
			string buttonName = def.ControlName;
			//string editName = def.DefTextBox.te;
			string text = def.Arguments;
			string command = def.Command; 
			string toolTipString = def.ToolTipString;
			// If we have no ToolTipString, go ahead and user the control's name
			if( toolTipString.Length < 1 )
			{
				toolTipString = buttonName;
			}
			bool hiddenOutput = def.HideOutput;

			def.ControlType = ToolBoxControlType.ParameterButton;

			// Setup our main panel (in which all controls will be placed
			Panel panel = new Panel();
			panel.Height = def.DefButton.Height + General_Padding;
			panel.Width = this.ToolBoxMainPanel.Width;
			panel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

			// Setup our Button.Text and Width
			def.DefButton = new Button();
			def.DefButton.Text = buttonName;
			if( def.ControlName == null || def.ControlName.Length < 1 )
				def.ControlName = def.DefButton.Text;
			def.DefButton.Width = (int)
				(def.DefButton.CreateGraphics().MeasureString( def.DefButton.Text, def.DefButton.Font ).Width + 0.5f) 
				+ Button_TextPadding;
			def.DefButton.FlatStyle = FlatStyle.System;
			def.DefButton.Click += this.DefButton_Click;

			// Setup our ArgumentButton for a ContextMenu
			def.DefArgumentButton = new Button();
			def.DefArgumentButton.Text = ControlDefinition.ArgumentButton_Text;
			def.DefArgumentButton.Width = ControlDefinition.ArgumentButton_Width;
			def.DefArgumentButton.FlatStyle = FlatStyle.System;
			def.DefArgumentButton.Click += DefArgumentButton_Click;

			// Setup our TextBox
			def.DefTextBox = new System.Windows.Forms.TextBox();
			def.DefTextBox.Location = new System.Drawing.Point(0, 3);
			def.DefTextBox.Width = ToolBoxMainPanel.Width - def.DefButton.Width 
				- def.DefArgumentButton.Width - General_Padding;
			def.DefTextBox.Text = def.Arguments;
			def.DefTextBox.TextChanged += this.DefTextBox_Changed;
			def.DefTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;

			def.DefButton.Location = new System.Drawing.Point(def.DefTextBox.Width + def.DefArgumentButton.Width, 1);
			def.DefButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

			def.DefArgumentButton.Location = new Point( def.DefTextBox.Width, 1 );
			def.DefArgumentButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

			def.ToolTipString = toolTipString;
			def.SetToolTip( this.ToolBoxToolTip );

			panel.Controls.Add( def );
			panel.Controls.Add( def.DefButton );
			panel.Controls.Add( def.DefArgumentButton );
			panel.Controls.Add( def.DefTextBox );

			panel.ContextMenu = this.PanelContext;

			this.ToolBoxMainPanel.Controls.Add( panel );
		}

		/// <summary>
		/// Change our ParameterButton ControlDefinition Argument on the fly...
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DefTextBox_Changed(object sender, System.EventArgs e)
		{
			TextBox tb = (TextBox)sender;
			ControlDefinition def = GetControlDefinition( sender );

			if (def.DefTextBox == tb)
			{
				def.Arguments = tb.Text;
				def.SetToolTip( this.ToolBoxToolTip );
			}
		}
		#endregion Add ParameterButton

		#region Add IniComboBox
		private void CreateIniComboBox( ControlDefinition def )
		{
			def.ControlType = ToolBoxControlType.IniCombo;

			def.DefButton = new Button();
			def.DefButton.Text = Add_Text;
			def.DefButton.Width = (int)
				(def.DefButton.CreateGraphics().MeasureString( def.DefButton.Text, def.DefButton.Font ).Width + 0.5f)
				+ Button_TextPadding;
			def.DefButton.FlatStyle = FlatStyle.System;

			def.DefComboBox = new ComboBox();
			def.DefComboBox.Location = new System.Drawing.Point(0, General_Padding );
			def.DefComboBox.Width = this.ToolBoxMainPanel.Width - def.DefButton.Width - General_Padding;
			def.DefComboBox.TabIndex = 1;
			def.DefComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			// Populate our ini comboBox before we set the index
			PopulateIniComboBox( def );
			def.DefComboBox.SelectedIndexChanged += new EventHandler(DefIniComboBox_SelectedIndexChanged);

			def.DefButton.Location = new Point( def.DefComboBox.Width, 1 );
			def.DefButton.Click += new EventHandler( DefIniComboBoxAddButton_Click );
			def.DefButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

			Panel panel = new Panel();
			panel.Height = def.DefButton.Height + General_Padding;
			panel.Width = def.DefComboBox.Width + def.DefButton.Width;
			panel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

			def.SetToolTip( this.ToolBoxToolTip );

			panel.Controls.Add( def );
			panel.Controls.Add( def.DefComboBox );
			panel.Controls.Add( def.DefButton );

			panel.ContextMenu = this.PanelContext;

			// Add the panel to the form.
			this.ToolBoxMainPanel.Controls.Add(panel);
		}
		/// <summary>
		/// Change arguments if index is changed...
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DefIniComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ControlDefinition def = GetControlDefinition( sender );
			def.LastSelectedIndex = def.DefComboBox.SelectedIndex;
			def.Arguments = def.DefComboBox.Text;

			WriteSettingsToIni( def.Location );
		}
		private void PopulateIniComboBox( ControlDefinition def )
		{
			string iniFilename = def.IniFilename;
			// Check if we are missing a root path?
			if (!Path.IsPathRooted(iniFilename))
			{
				// Append on the current workspace directory
				iniFilename = Path.Combine(MOG_ControllerProject.GetWorkspaceDirectory(), iniFilename.Trim("\\".ToCharArray()));
			}

			MOG_Ini ini = new MOG_Ini( iniFilename );

			string section = def.IniSectionName;

			int keyCount = 0;
			// Set our keyCount, if our section exists
			if( ini.SectionExist( section ) )
				keyCount = ini.CountKeys( section );

			def.DefComboBox.Items.Clear();
			for( int i = 0; i < keyCount; ++i)
			{
				string valueStr = ini.GetKeyByIndexSLOW( section, i );
				if( valueStr != null && valueStr.Length > 0 )
					def.DefComboBox.Items.Add( ini.GetKeyNameByIndexSLOW( section, i ) + "=" + valueStr );
				else
					def.DefComboBox.Items.Add( ini.GetKeyNameByIndexSLOW( section, i ));
			}

			// Make sure we set the last selected item
			if (def.DefComboBox.Items.Count > def.LastSelectedIndex)
			{
				def.DefComboBox.SelectedIndex = def.LastSelectedIndex;
			}
			// Always reset the arguments to match
			def.Arguments = def.DefComboBox.Text;
		}
        
		/// <summary>
		/// Whenever a user clicks to `Add`, the we take ComboBox.Text and write it out to the 
		/// INI the ComboBox is attached to.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DefIniComboBoxAddButton_Click( object sender, EventArgs e )
		{
			Panel panel = (Panel)((Control)sender).Parent;
			foreach( Control control in panel.Controls )
			{
				if( control.GetType().AssemblyQualifiedName == (new ComboBox()).GetType().AssemblyQualifiedName )
				{
					ComboBox cb = (ComboBox)control;
					string key = cb.Text.Replace( " ", "" );
					string valueStr = "";
					// If we have a key-value pair, indicate such
					if( key.IndexOf("=") > -1 )
					{
						valueStr = key.Substring( key.IndexOf("=")+1 );
						key = key.Replace( "=" + valueStr, "" );
					}
					ControlDefinition def = GetControlDefinition( cb );

					string iniFilename = def.IniFilename;
					// Check if we are missing a root path?
					if (!Path.IsPathRooted(iniFilename))
					{
						// Append on the current workspace directory
						iniFilename = Path.Combine(MOG_ControllerProject.GetWorkspaceDirectory(), iniFilename.Trim("\\".ToCharArray()));
					}
					MOG_Ini ini = new MOG_Ini( iniFilename );
					if( key != null && key.Length > 0 )
					{
						ini.Open( iniFilename, FileShare.Write );
						ini.PutString( def.IniSectionName, key, valueStr );
						ini.Save();
						ini.Close();
						PopulateIniComboBox( def );
						cb.Text = "";
					}
					else
						MessageBox.Show( this, "Please select a valid key or type a key to add!", "Missing Key", 
							MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				}
			}
		}
		#endregion Add IniComboBox

		#region Add FileBrowserComboBox
		/// <summary>
		/// Creates a FolderComboBox to be placed in MOG Custom Controls
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="name"></param>
		/// <param name="folderName">Full folder path</param>
		private void CreateFileComboBox( ControlDefinition def )
		{
			def.ControlType = ToolBoxControlType.FileCombo;

			def.DefButton = new Button();
			def.DefButton.Text = "...";
			def.DefButton.Width = 
				(int)(def.DefButton.CreateGraphics().MeasureString(def.DefButton.Text, def.DefButton.Font).Width + 0.5f)
				+ Button_TextPadding;
			def.DefButton.Click += DefFileComboBoxButton_Click;
			def.DefButton.FlatStyle = FlatStyle.System;
			def.RightToLeft = RightToLeft.Yes;

			// ComboBox -- Pt 1
			def.DefComboBox = new ComboBox();
			def.DefComboBox.Location = new System.Drawing.Point(0, General_Padding);
			def.DefComboBox.Width = this.ToolBoxMainPanel.Width	- def.DefButton.Width - General_Padding;
			def.DefComboBox.TabIndex = 1;
			def.DefComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			
			def.DefButton.Location = new Point( def.DefComboBox.Location.X + def.DefComboBox.Width, 1 );
			def.DefButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			def.DefButton.Visible = true; 
			def.DefButton.Enabled = true;

			// ComboBox -- Pt 2
			// Populate our ComboBox before we set the last selected index
			PopulateFolderComboBox( def );
			def.DefComboBox.SelectedIndexChanged += DefFileComboBox_SelectedIndexChanged;
			def.DefComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;


			Panel panel = new Panel();
			panel.Height = def.DefButton.Height + General_Padding;
			panel.Width = def.DefComboBox.Width + def.DefButton.Width;
			panel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

			def.SetToolTip( this.ToolBoxToolTip );

			panel.Controls.Add( def );
			panel.Controls.Add( def.DefComboBox );
			panel.Controls.Add( def.DefButton );

			panel.ContextMenu = this.PanelContext;

			// Add the panel to the form.
			this.ToolBoxMainPanel.Controls.Add(panel);
		}
		
		private void DefFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox cb = (ComboBox)sender;
			// Make sure we have a MogTaggedString selected...
			if( cb.SelectedItem is MogTaggedString )
			{
				MogTaggedString thisMogTaggedString = (MogTaggedString)cb.SelectedItem;
				string filename = (string)thisMogTaggedString.AttachedItem;
				ControlDefinition def = GetControlDefinition( sender );
				def.LastSelectedIndex = cb.SelectedIndex;
				def.Arguments = thisMogTaggedString.ToString();

				// Save out the tool tip for this control to be the full name of this item
				// P.s. I looked all over the net for a way to do this as we scroll down
				//      the combobox, but to no avail. 9/27/2007
				ToolTip toolTip1 = new ToolTip();
				toolTip1.AutoPopDelay = 0;
				toolTip1.InitialDelay = 0;
				toolTip1.ReshowDelay = 0;
				toolTip1.ShowAlways = true;
				toolTip1.SetToolTip(cb, filename);

				WriteSettingsToIni( def.Location );				
			}
		}
		private void DefFileComboBoxButton_Click(object sender, EventArgs e)
		{
			ControlDefinition def = GetControlDefinition(sender);

			string directory = string.IsNullOrEmpty(def.FolderName) ? MOG_ControllerProject.GetWorkspaceDirectory() : def.FolderName;
			// Check if we are missing a root path?
			if (!Path.IsPathRooted(directory))
			{
				// Append on the current workspace directory
				directory = Path.Combine(MOG_ControllerProject.GetWorkspaceDirectory(), directory.Trim("\\".ToCharArray()));
			}

			// Show user the OpenFileDialog
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = directory;
			ofd.Filter = "Specified Files (" + def.Pattern + ")|" + def.Pattern;
			if( ofd.ShowDialog( this ) == DialogResult.OK )
			{
				string [] fileNames = ofd.FileNames;

				// First, remove these new filenames from the list because we will insert them later at the top
				foreach (string filename in fileNames)
				{
					MogTaggedString filePlus = new MogTaggedString(filename, filename, def.FilenameStyle, directory);
					if (def.DefComboBox.Items.Contains(filePlus) == true)
					{
						// Remove this filename
						def.DefComboBox.Items.Remove(filePlus);
					}
				}

				// Second, insert these new filenames at the top of our list
				foreach (string filename in fileNames)
				{
					MogTaggedString filePlus = new MogTaggedString(filename, filename, def.FilenameStyle, directory);
					// Insert this filename
					def.DefComboBox.Items.Insert(0, filePlus);
				}

				// Third, cap our list to respect the proper depth
				while (def.DefComboBox.Items.Count > def.ComboBoxDepth)
				{
					// Remove the bottom item
					def.DefComboBox.Items.RemoveAt(def.DefComboBox.Items.Count - 1);
				}

				// Force our last selected item to the top of the list
				def.DefComboBox.SelectedIndex = 0;
				def.LastSelectedIndex = 0;
				// Always reset the arguments to match the selected item
				def.Arguments = def.DefComboBox.Text;
			}
			WriteSettingsToIni( def.Location );
		}
		
		/// <summary>
		/// Search for files and fill ComboBox based on a pattern
		/// </summary>
		/// <param name="def"></param>
		private void PopulateFolderComboBox( ControlDefinition def )
		{
			this.SuspendLayout();
			
			string directory = string.IsNullOrEmpty(def.FolderName) ? MOG_ControllerProject.GetWorkspaceDirectory() : def.FolderName;
			// Check if we are missing a root path?
			if (!Path.IsPathRooted(directory))
			{
				// Append on the current workspace directory
				directory = Path.Combine(MOG_ControllerProject.GetWorkspaceDirectory(), directory.Trim("\\".ToCharArray()));
			}

			// Create our index for the depth we are going to
			int i = 0;
			// If we need to populate based on what the user has given us...
			if( def.ComboBoxItems != null && def.ComboBoxItems.Length > 0 )
			{
				// If we have items, clear them
				if( def.DefComboBox.Items.Count > 0 )
					def.DefComboBox.Items.Clear();

				foreach( MogTaggedString item in def.ComboBoxItems )
				{
					// Propagate the show full path variable
					item.FilenameStyle = def.FilenameStyle;
					
					// If within depth, add
					if (i < def.ComboBoxDepth)
					{						
						def.DefComboBox.Items.Add(item);
					}
					// Else break out
					else
						break;
					++i;
				}
			}				
				// Else, If we have a directory from folderName...
			else if( DosUtils.DirectoryExist( directory )  )
			{
				FileInfo[] fileList = null;
				if (def.RecurseFolders)
				{
					ArrayList list = DosUtils.FileGetRecursiveList(directory, def.Pattern);
					fileList = new FileInfo[list.Count];
					for (int x = 0; x < list.Count; x++)
					{
						FileInfo file = new FileInfo(list[x] as string);
						fileList[x] = file;
					}
				}
				else
				{
					fileList = DosUtils.FileGetList(directory, def.Pattern);
				}
				
				// If we have items, clear them
				if( def.DefComboBox.Items.Count > 0 )
					def.DefComboBox.Items.Clear();

				// Get a graphics handle for the measureString call below
				Graphics graphics = Graphics.FromHwnd(this.Handle);

				// Loop thru all the files we got from DosUtils.FileGetList
				foreach( FileInfo file in fileList )
				{
					// If we are within depth, add each file we find
					if (i < def.ComboBoxDepth)
					{
						MogTaggedString item = new MogTaggedString(file.FullName, file.FullName, def.FilenameStyle, directory);
						def.DefComboBox.Items.Add(item);

						// Make sure the string will fit within the dropdown box
						SizeF stringSize = graphics.MeasureString(item.ToString(), def.DefComboBox.Font);
						if (def.DefComboBox.DropDownWidth < stringSize.Width)
						{
							// If not, then expand the width to meet the requirment
							def.DefComboBox.DropDownWidth = (int)stringSize.Width;
						}
					}
					// Else, break out of this loop
					else
					{
						break;
					}
					++i;
				}
			}

			// Make sure to select the last selected item
			if (def.DefComboBox.Items.Count > def.LastSelectedIndex)
			{
				def.DefComboBox.SelectedIndex = def.LastSelectedIndex;
			}
			// Always reset the arguments to match the selected item
			def.Arguments = def.DefComboBox.Text;

			this.ResumeLayout();
		}
		#endregion Add FileBrowserComboBox

		#region Add RadioButtonGroup
		private void CreateRadioButtonGroup( ControlDefinition def )
		{			
			def.ControlType = ToolBoxControlType.RadioButton;

			def.DefRadioGroupBox = new ToolBoxGroupBox( def.RadioButtons );
			if( def.DefRadioGroupBox.Controls.Count > def.LastSelectedIndex )
			{
				((RadioButton)def.DefRadioGroupBox.Controls[def.LastSelectedIndex]).Checked = true;
			}
			def.DefRadioGroupBox.Text = def.ControlName;
			def.DefRadioGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
			def.DefRadioGroupBox.Location = new Point(0, 0);
			def.DefRadioGroupBox.Width = this.ToolBoxMainPanel.Width - def.DefRadioGroupBox.Location.X - MainPanel_Right_Padding;
			def.DefRadioGroupBox.Height = def.DefRadioGroupBox.CalculateHeightBasedOnWidth( def.DefRadioGroupBox.Width );
			def.DefRadioGroupBox.SelectionChanged +=new MOG_Client.Forms.ToolBoxForms.ToolBoxGroupBox.SelectionChangedHandler(DefRadioGroupBox_SelectionChanged);
			def.DefRadioGroupBox.FlatStyle = FlatStyle.System;

			Panel panel = new Panel();
			panel.Height = def.DefRadioGroupBox.Height;
			panel.Width = def.DefRadioGroupBox.Width;
			panel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

			def.SetToolTip( this.ToolBoxToolTip );

			panel.Controls.Add( def );
			panel.Controls.Add( def.DefRadioGroupBox );

			panel.ContextMenu = this.PanelContext;

			// Add the panel to the form.
			this.ToolBoxMainPanel.Controls.Add(panel);
		}

		private void DefRadioGroupBox_SelectionChanged(object sender, EventArgs e)
		{
			RadioButton button = (RadioButton)sender;
			ControlDefinition def = GetControlDefinition( sender );
			def.LastSelectedIndex = def.DefRadioGroupBox.Controls.GetChildIndex( button, false );
			def.Arguments = button.Tag.ToString();

			WriteSettingsToIni( def.Location );
		}
		#endregion Add RadioButtonGroup

		#region Add CheckBox
		private void CreateCheckBox( ControlDefinition def )
		{
			def.ControlType = ToolBoxControlType.CheckBox;

			def.DefCheckBox = new CheckBox();
			def.DefCheckBox.FlatStyle = FlatStyle.System;
			def.DefCheckBox.Location = new Point( 0, 0 );
			def.DefCheckBox.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			def.DefCheckBox.Text = def.ControlName;
			def.DefCheckBox.Width = ((int)(def.DefCheckBox.CreateGraphics()
				.MeasureString( def.DefCheckBox.Text, def.DefCheckBox.Font ).Width + 0.5f)) 
				+ General_Padding + CheckBox_Padding;

			// Set our checked state
			if( def.Arguments == def.ArgumentTrue )
				def.DefCheckBox.Checked = true;
			else
				def.DefCheckBox.Checked = false;

			// Now that we've set our check state, attach our event handler
			def.DefCheckBox.CheckedChanged += new EventHandler(DefCheckBox_CheckedChanged);

			Panel panel = new Panel();
			panel.Height = def.DefCheckBox.Height;
			panel.Width = def.DefCheckBox.Width;
			panel.Anchor = def.DefCheckBox.Anchor;

			def.SetToolTip( this.ToolBoxToolTip );

			panel.Controls.Add( def );
			panel.Controls.Add( def.DefCheckBox );

			panel.ContextMenu = this.PanelContext;

			this.ToolBoxMainPanel.Controls.Add( panel );
		}

		/// <summary>
		/// Document changes in our CheckBox state
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DefCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			// Get our checkbox
			CheckBox checkBox = (CheckBox)sender;
			// Get the ControlDef for it
			ControlDefinition def = GetControlDefinition( sender );
			// Change state of def.Arguments according to Checked state
			if( checkBox.Checked )
			{
				def.Arguments = def.ArgumentTrue;
			}
			else
			{
				def.Arguments = def.ArgumentFalse;
			}
			ToolBoxToolTip.SetToolTip( (Control)sender, "Argument: '" + def.Arguments +  "'" );

			// Save our change
			WriteSettingsToIni( def.Location );
		}
		#endregion Add CheckBox

		#region Context Panel Methods

		#endregion Context Panel Methods

		#endregion Add Controls

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.MainContext = new System.Windows.Forms.ContextMenu();
			this.MainContextAddLabelMenuItem = new System.Windows.Forms.MenuItem();
			this.MainContextSeparator1 = new System.Windows.Forms.MenuItem();
			this.MainContextAddCheckBoxMenuItem = new System.Windows.Forms.MenuItem();
			this.MainContextAddRadioButtonMenuItem = new System.Windows.Forms.MenuItem();
			this.MainContextSeparator2 = new System.Windows.Forms.MenuItem();
			this.MainContextAddButtonMenuItem = new System.Windows.Forms.MenuItem();
			this.MainContextAddParameterMenuItem = new System.Windows.Forms.MenuItem();
			this.MainContextSeparator3 = new System.Windows.Forms.MenuItem();
			this.MainContextAddIniComboMenuItem = new System.Windows.Forms.MenuItem();
			this.MainContextAddFileBrowserComboMenuItem = new System.Windows.Forms.MenuItem();
			this.ToolBoxMainPanel = new System.Windows.Forms.Panel();
			this.PanelContext = new System.Windows.Forms.ContextMenu();
			this.PanelContextEditMenuItem = new System.Windows.Forms.MenuItem();
			this.PanelContextDeleteMenuItem = new System.Windows.Forms.MenuItem();
			this.PanelContextSeparator1 = new System.Windows.Forms.MenuItem();
			this.PanelContextMoveUpMenuItem = new System.Windows.Forms.MenuItem();
			this.PanelContextMoveDownMenuItem = new System.Windows.Forms.MenuItem();
			this.PanelContextSeparator2 = new System.Windows.Forms.MenuItem();
			this.PanelContextProjectMenuItem = new System.Windows.Forms.MenuItem();
			this.PanelContextDepartmentMenuItem = new System.Windows.Forms.MenuItem();
			this.PanelContextUserMenuItem = new System.Windows.Forms.MenuItem();
			this.ToolBoxToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.MainContextAddLinkLabelMenuItem = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// MainContext
			// 
			this.MainContext.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MainContextAddLabelMenuItem,
            this.MainContextAddLinkLabelMenuItem,
            this.MainContextSeparator1,
            this.MainContextAddCheckBoxMenuItem,
            this.MainContextAddRadioButtonMenuItem,
            this.MainContextSeparator2,
            this.MainContextAddButtonMenuItem,
            this.MainContextAddParameterMenuItem,
            this.MainContextSeparator3,
            this.MainContextAddIniComboMenuItem,
            this.MainContextAddFileBrowserComboMenuItem});
			this.MainContext.Popup += new System.EventHandler(this.MainContext_Popup);
			// 
			// MainContextAddLabelMenuItem
			// 
			this.MainContextAddLabelMenuItem.Index = 0;
			this.MainContextAddLabelMenuItem.Text = "Add Label";
			this.MainContextAddLabelMenuItem.Click += new System.EventHandler(this.AddLabel_MenuItemClick);
			// 
			// MainContextSeparator1
			// 
			this.MainContextSeparator1.Index = 2;
			this.MainContextSeparator1.Text = "-";
			// 
			// MainContextAddCheckBoxMenuItem
			// 
			this.MainContextAddCheckBoxMenuItem.Index = 3;
			this.MainContextAddCheckBoxMenuItem.Text = "Add Check Box";
			this.MainContextAddCheckBoxMenuItem.Click += new System.EventHandler(this.AddCheckBox_MenuItemClick);
			// 
			// MainContextAddRadioButtonMenuItem
			// 
			this.MainContextAddRadioButtonMenuItem.Index = 4;
			this.MainContextAddRadioButtonMenuItem.Text = "Add Radio Button Group";
			this.MainContextAddRadioButtonMenuItem.Click += new System.EventHandler(this.AddRadioButton_MenuItemClick);
			// 
			// MainContextSeparator2
			// 
			this.MainContextSeparator2.Index = 5;
			this.MainContextSeparator2.Text = "-";
			// 
			// MainContextAddButtonMenuItem
			// 
			this.MainContextAddButtonMenuItem.Index = 6;
			this.MainContextAddButtonMenuItem.Text = "Add Button";
			this.MainContextAddButtonMenuItem.Click += new System.EventHandler(this.AddButton_MenuItemClick);
			// 
			// MainContextAddParameterMenuItem
			// 
			this.MainContextAddParameterMenuItem.Index = 7;
			this.MainContextAddParameterMenuItem.Text = "Add Parameter Button";
			this.MainContextAddParameterMenuItem.Click += new System.EventHandler(this.AddParamButton_MenuItemClick);
			// 
			// MainContextSeparator3
			// 
			this.MainContextSeparator3.Index = 8;
			this.MainContextSeparator3.Text = "-";
			// 
			// MainContextAddIniComboMenuItem
			// 
			this.MainContextAddIniComboMenuItem.Index = 9;
			this.MainContextAddIniComboMenuItem.Text = "Add Ini Combo Box";
			this.MainContextAddIniComboMenuItem.Click += new System.EventHandler(this.AddIniCombo_MenuItemClick);
			// 
			// MainContextAddFileBrowserComboMenuItem
			// 
			this.MainContextAddFileBrowserComboMenuItem.Index = 10;
			this.MainContextAddFileBrowserComboMenuItem.Text = "Add File Browser Combo Box";
			this.MainContextAddFileBrowserComboMenuItem.Click += new System.EventHandler(this.AddFileBrowserCombo_MenuItemClick);
			// 
			// ToolBoxMainPanel
			// 
			this.ToolBoxMainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ToolBoxMainPanel.AutoScroll = true;
			this.ToolBoxMainPanel.AutoScrollMinSize = new System.Drawing.Size(16, 16);
			this.ToolBoxMainPanel.ContextMenu = this.MainContext;
			this.ToolBoxMainPanel.Location = new System.Drawing.Point(0, 0);
			this.ToolBoxMainPanel.Name = "ToolBoxMainPanel";
			this.ToolBoxMainPanel.Size = new System.Drawing.Size(264, 160);
			this.ToolBoxMainPanel.TabIndex = 1;
			this.ToolBoxToolTip.SetToolTip(this.ToolBoxMainPanel, "Right-click here to add Custom MOG Tools (Buttons, File Combo Boxes, etc.)");
			// 
			// PanelContext
			// 
			this.PanelContext.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.PanelContextEditMenuItem,
            this.PanelContextDeleteMenuItem,
            this.PanelContextSeparator1,
            this.PanelContextMoveUpMenuItem,
            this.PanelContextMoveDownMenuItem,
            this.PanelContextSeparator2,
            this.PanelContextProjectMenuItem,
            this.PanelContextDepartmentMenuItem,
            this.PanelContextUserMenuItem});
			this.PanelContext.Popup += new System.EventHandler(this.PanelContext_Popup);
			// 
			// PanelContextEditMenuItem
			// 
			this.PanelContextEditMenuItem.Index = 0;
			this.PanelContextEditMenuItem.Text = "Edit";
			this.PanelContextEditMenuItem.Click += new System.EventHandler(this.PanelContextEditControl_MenuItemClick);
			// 
			// PanelContextDeleteMenuItem
			// 
			this.PanelContextDeleteMenuItem.Index = 1;
			this.PanelContextDeleteMenuItem.Text = "Delete";
			this.PanelContextDeleteMenuItem.Click += new System.EventHandler(this.PanelContextDeleteControl_MenuItemClick);
			// 
			// PanelContextSeparator1
			// 
			this.PanelContextSeparator1.Index = 2;
			this.PanelContextSeparator1.Text = "-";
			// 
			// PanelContextMoveUpMenuItem
			// 
			this.PanelContextMoveUpMenuItem.Index = 3;
			this.PanelContextMoveUpMenuItem.Text = "Move Up";
			this.PanelContextMoveUpMenuItem.Click += new System.EventHandler(this.PanelContextMoveUp_Click);
			// 
			// PanelContextMoveDownMenuItem
			// 
			this.PanelContextMoveDownMenuItem.Index = 4;
			this.PanelContextMoveDownMenuItem.Text = "Move Down";
			this.PanelContextMoveDownMenuItem.Click += new System.EventHandler(this.PanelContextMoveDown_Click);
			// 
			// PanelContextSeparator2
			// 
			this.PanelContextSeparator2.Index = 5;
			this.PanelContextSeparator2.Text = "-";
			// 
			// PanelContextProjectMenuItem
			// 
			this.PanelContextProjectMenuItem.Index = 6;
			this.PanelContextProjectMenuItem.Text = "Project";
			this.PanelContextProjectMenuItem.Click += new System.EventHandler(this.PanelContextLocationSwitchMenuItem_Click);
			// 
			// PanelContextDepartmentMenuItem
			// 
			this.PanelContextDepartmentMenuItem.Index = 7;
			this.PanelContextDepartmentMenuItem.Text = "Department";
			this.PanelContextDepartmentMenuItem.Click += new System.EventHandler(this.PanelContextLocationSwitchMenuItem_Click);
			// 
			// PanelContextUserMenuItem
			// 
			this.PanelContextUserMenuItem.Index = 8;
			this.PanelContextUserMenuItem.Text = "User";
			this.PanelContextUserMenuItem.Click += new System.EventHandler(this.PanelContextLocationSwitchMenuItem_Click);
			// 
			// ToolBoxToolTip
			// 
			this.ToolBoxToolTip.AutomaticDelay = 200;
			this.ToolBoxToolTip.AutoPopDelay = 2000;
			this.ToolBoxToolTip.InitialDelay = 200;
			this.ToolBoxToolTip.ReshowDelay = 100;
			// 
			// MainContextAddLinkLabelMenuItem
			// 
			this.MainContextAddLinkLabelMenuItem.Index = 1;
			this.MainContextAddLinkLabelMenuItem.Text = "Add Link Label";
			this.MainContextAddLinkLabelMenuItem.Click += new System.EventHandler(this.MainContextAddLinkLabelMenuItem_Click);
			// 
			// ToolBox
			// 
			this.AutoScroll = true;
			this.ContextMenu = this.MainContext;
			this.Controls.Add(this.ToolBoxMainPanel);
			this.Name = "ToolBox";
			this.Size = new System.Drawing.Size(264, 160);
			this.Resize += new System.EventHandler(this.ToolBox_Resize);
			this.ResumeLayout(false);

		}
		#endregion

		#region Utility/Other Methods
		private static ToolBoxControlLocation GetMenuItemLocation( MenuItem menuItem )
		{
			ToolBoxControlLocation location = ToolBoxControlLocation.User;

			// If we have a Parent, and it is a MenuItem type...
			if( menuItem.Parent != null && menuItem.Parent.GetType().UnderlyingSystemType
				== menuItem.GetType() )
			{
				// Get our parent
				MenuItem parent = (MenuItem)menuItem.Parent;

				// If we are Project, set location to Project
				if( parent.Text == ProjectMenuItem_Text )
				{
					location = ToolBoxControlLocation.Project;
				}
					// Else, if we are department, set location to Department
				else if( parent.Text == DepartmentMenuItem_Text )
				{
					location = ToolBoxControlLocation.Department;
				}
			}

			return location; 
		}

		private static void AddArrayListOfMenuItemsToCollection( ArrayList savedMenuItems, Menu.MenuItemCollection MenuItems, bool clone, bool checkVersionInfo )
		{
			foreach( MenuItem menuItem in savedMenuItems )
			{
				if( clone )
				{
                    MenuItem clonedMenuItem = menuItem.CloneMenu();

                    // Set the light version menuItem only if the bool is true AND the menuItem is not a separator
                    if (checkVersionInfo && clonedMenuItem.Text != "-") MogUtil_VersionInfo.SetLightVersionControl(clonedMenuItem);

                    MenuItems.Add(clonedMenuItem);
				}
				else
				{
                    // Set the light version menuItem only if the bool is true AND the menuItem is not a separator
                    if (checkVersionInfo && menuItem.Text != "-") MogUtil_VersionInfo.SetLightVersionControl(menuItem);

					MenuItems.Add( menuItem );
				}
			}
		}

		private static ArrayList GetArrayListOfMenuItems( ContextMenu cm )
		{
			ArrayList menuItems = new ArrayList();
			foreach( MenuItem menuItem in cm.MenuItems )
			{
				// If we are not duplicating runtime MenuItems
				if( menuItem.Text != ProjectMenuItem_Text  
					&& menuItem.Text != DepartmentMenuItem_Text )
				{
					menuItems.Add( menuItem );
				}
			}

			// Get our last MenuItem
			MenuItem lastMenuItem = (MenuItem)menuItems[ menuItems.Count-1 ];

			// If last menu item is a separator, remove it
			if( lastMenuItem.Text == "-" )
			{
				menuItems.Remove( lastMenuItem );
			}

			return menuItems;
		}

		// Recursively go through a menu, setting everything to value of enabled
		private static void SwitchMainContextMenuItems( MenuItem.MenuItemCollection items, bool enabled )
		{
			foreach( MenuItem item in items )
			{
				item.Enabled = enabled;
			}
		}

		/// <summary>
		/// Returns a panel that spans the width of the MainPanel and is anchored top,left,right.
		/// </summary>
		/// <returns></returns>
		private Panel GetNewSeparatorPanel( ToolBoxControlLocation currentLocation )
		{
			string locationStr = "";
			switch( currentLocation )
			{
				case ToolBoxControlLocation.User:
					locationStr = "User (" + MOG_ControllerProject.GetUserName() + ")";
					break;
				case ToolBoxControlLocation.Department:
					locationStr = "Department (" + MOG_ControllerProject.GetUser().GetUserDepartment() + ")";
					break;
				case ToolBoxControlLocation.Project:
					locationStr = "Project (" + MOG_ControllerProject.GetProjectName() + ")";
					break;
				default:
					throw new Exception( "Programmer forgot to add a case for a new ToolBoxControlLocation" );
			}

			Panel panel = new Panel();
			panel.Location = new Point(General_Padding,0);
			panel.Width = ToolBoxMainPanel.Width;
			panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

			Panel leftPanel = new Panel();
			leftPanel.Location = new Point(0,General_Padding+1);
			leftPanel.Width = SeparatorPanel_LeftWidth;
			leftPanel.BorderStyle = BorderStyle.Fixed3D;
			leftPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			leftPanel.BackColor = SystemColors.ControlDark;

			Label textLabel = new Label();
			textLabel.Text = locationStr;
			textLabel.Font = new Font( textLabel.Font, FontStyle.Bold );
			textLabel.Size = textLabel.CreateGraphics().MeasureString( textLabel.Text, textLabel.Font).ToSize();
			textLabel.Width += (2*General_Padding);
			textLabel.Location = new Point( leftPanel.Location.X + leftPanel.Width, 0 );
			textLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;

			Panel rightPanel = new Panel();
			int rightPanelLeftX = textLabel.Location.X + textLabel.Width + General_Padding;
			rightPanel.Location = new Point(rightPanelLeftX, General_Padding+1);
			rightPanel.Width = this.ToolBoxMainPanel.Width - rightPanelLeftX - General_Padding;
			rightPanel.BorderStyle = BorderStyle.Fixed3D;
			rightPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			rightPanel.BackColor = SystemColors.ControlDark;

			rightPanel.Height = textLabel.Height - (2 * General_Padding);
			leftPanel.Height = textLabel.Height - (2 * General_Padding);
			panel.Height = textLabel.Height;

			panel.Controls.Add( leftPanel );
			panel.Controls.Add( textLabel );
			panel.Controls.Add( rightPanel );

			return panel;
		}

		/// <summary>
		/// Sets tooltips for each button in a ToolBoxGroupBox so that each button shows its argument
		/// as its tooltip
		/// </summary>
		/// <param name="toolTip"></param>
		/// <param name="groupBox"></param>
		/// <param name="toolTipString"></param>
		protected internal static void SetGroupBoxToolTips( ToolTip toolTip, ToolBoxGroupBox groupBox, string toolTipString )
		{
			toolTip.SetToolTip( groupBox, toolTipString );
			// Foreach RadioButton, set our tooltip so that it shows the argument attached...
			foreach( Control testControl in groupBox.Controls )
			{
				if( testControl.GetType().AssemblyQualifiedName ==
					(new RadioButton()).GetType().AssemblyQualifiedName )
				{
					RadioButton button = (RadioButton)testControl;
					string buttonToolTip = button.Tag.ToString();

					// If we have no argument, indicate such.
					if( buttonToolTip.Length < 1 )
						buttonToolTip = RadioButton_No_Arg_ToolTip;
						// Else, we do have an argument, so make it look nice
					else
						buttonToolTip = "Argument: `" + buttonToolTip + "`";

					toolTip.SetToolTip( button, buttonToolTip );
				}
			}
		}

		/// <summary>
		/// Make sure we have a valid project
		/// </summary>
		private bool ProjectCheck()
		{
			// If we have no platform or no local gamedata tabs, display an error.
			if(mPlatformName == null || mPlatformName.Length == 0)
			{
				MessageBox.Show("You must select a valid project and/or have a gamedata tab before adding controls", "ERROR",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns the control definition from the sender
		/// </summary>
		/// <param name="sender">Object (usually a Panel) from which to get our ControlDefinition object</param>
		/// <returns>Null if there is no Control Defintion</returns>
		private static ControlDefinition GetControlDefinition( object sender )
		{
			// If we have a Panel
			if( sender is Panel  )
			{
				Panel panel = (Panel)sender;
				// Check to make sure we have the right panel
				if( panel.Controls.Count < 1  && panel.Parent != null
					&& panel.Parent is Panel )
				{
					panel = (Panel)panel.Parent;
				}

				// Foreach control in panel
				foreach( Control control in panel.Controls )
				{
					// If we have the ControlDefinition, return it.
					if( control is ControlDefinition )
					{
						return (ControlDefinition)control;
					}
				}
			}
			else if( sender is MenuItem )
			{
				Control parent = ((MenuItem)sender).GetContextMenu().SourceControl;
				return GetControlDefinition( parent );
			}
				// We have the child of a Panel...
			else 
			{
				Control control = (Control)sender;
				// If we have a parent...
				if( control.Parent != null  )
				{
					// The parent should have our ControlDefinition
					return GetControlDefinition( (object)control.Parent );
				}
			}

			// Else return null
			return null;
		}

		/// <summary>
		/// Gets ControlDefinition TagNames and Arguments from our Controls (should only be used
		///  externally by ToolBoxArgumentsBrowserForm
		/// </summary>
		/// <returns>SortedList of all our TagNames with Arguments attached</returns>
		public SortedList GetControlTagNames()
		{
			SortedList tagNameArguments = new SortedList();
			// Go through all of our controls
			foreach( Control control in this.CustomControls )
			{
				ControlDefinition def = GetControlDefinition( control );

				// If we found an invalid ControlDefinition, skip it
				if( def == null )
				{
					continue; 
				}

				// Make sure we have a unique TagName before proceeding
				def.TagName = ParseTagNameForDuplicateKey( tagNameArguments, def.TagName, 0 );
				tagNameArguments.Add( def.TagName, def.Arguments );
			}
			return tagNameArguments;
		}

		public ContextMenu GenerateTagContextMenu( EventHandler menuItemClickEventHandler )
		{
			ContextMenu cm = new ContextMenu();
			SortedList tagNames = this.GetControlTagNames();
			// Foreach entry we have in tagNames...
			foreach( DictionaryEntry entry in tagNames )
			{
				// Get our tag and arguments
				string tag = (string)entry.Key;
				string arguments = (string)entry.Value;

				// Indicate whether we have arguments attached
				if( arguments.Length < 1 )
					arguments = "No arguments";

				// If we have a filename, lets only get the last bit of it
				if( arguments.IndexOf( "\\" ) > -1 )
					arguments = arguments.Substring( arguments.LastIndexOf("\\")+1 );

				string formattedArgs = MOG_Tokens.GetFormattedString( "{"+tag+"}");

				// Add our MenuItem with an event handler
				MenuItem tagMenuItem = new MenuItem( "<" + tag + ">=='" + arguments + "'");

				tagMenuItem.Click += menuItemClickEventHandler;
				cm.MenuItems.Add( tagMenuItem );
			}

			if( tagNames.Count < 1 )
				cm.MenuItems.Add( "No Custom Tags Available" );

			// Populate System Items
			GenerateTagContextMenu_PopulateFromArrayList( cm, "System Related", this.SystemArguments, 
				menuItemClickEventHandler );
			// Populate Project Items
			GenerateTagContextMenu_PopulateFromArrayList( cm, "Project Related", this.ProjectArguments, 
				menuItemClickEventHandler );
			// Populate Time Items
			GenerateTagContextMenu_PopulateFromArrayList( cm, "Time Related", this.TimeArguments, 
				menuItemClickEventHandler );
			// Populate File and User Items
			GenerateTagContextMenu_PopulateFromArrayList( cm, "File and User Related", this.UserAndFileArguments, 
				menuItemClickEventHandler );

			return cm;
		}

		/// <summary>
		/// Takes a ContextMenu, a name for a MenuItem, and an ArrayList of MogTaggedStrings to
		///  populate and add a MenuItem with the name passed in as `menuItemName`.
		/// </summary>
		/// <param name="cm">ContextMenu to populate</param>
		/// <param name="menuItemName">MenuItem name to create</param>
		/// <param name="tags">Tags to populate with</param>
		/// <param name="menuItemEventHandler"></param>
		public void GenerateTagContextMenu_PopulateFromArrayList( ContextMenu cm, string menuItemName, 
			ArrayList tags, EventHandler menuItemEventHandler )
		{
			MenuItem argumentsMenuItem = new MenuItem( menuItemName );
			foreach( MogTaggedString tbString in tags )
			{
				// Get our argument
				string formattedString = tbString.AttachedItem.ToString();
				formattedString = MOG_Tokens.GetFormattedString( formattedString);
				if( formattedString.Length < 1 )
				{
					formattedString = "Did Not Evaluate";
				}

				// Add our MenuItem with an event handler
				MenuItem tagMenuItem = new MenuItem("<" + tbString + "> --- '" 
					+formattedString+ "'");
				tagMenuItem.Click += menuItemEventHandler;
				argumentsMenuItem.MenuItems.Add( tagMenuItem );
			}
			cm.MenuItems.Add( argumentsMenuItem );
		}

		/// <summary>
		/// Checks our tag names to make sure that we do not have any duplicates
		/// </summary>
		/// <param name="arguments">SortedList of arguments to add to</param>
		/// <param name="tagName">Name to be searched for</param>
		/// <param name="count">The number of times we have executed this code</param>
		/// <returns>A valid, unique tagName to be added to the SortedList, `arguments`</returns>
		private static string ParseTagNameForDuplicateKey( SortedList arguments, string tagName, int count)
		{
			// If we are the first iteration and not a copy, return tagNamel...
			if( count == 0 && !arguments.ContainsKey( tagName ) )
				return tagName;
				// Else, if we are the first iteration and we are a copy, try again...
			else if( count == 0 && arguments.ContainsKey( tagName ) )
				return ParseTagNameForDuplicateKey( arguments, tagName, ++count );
				// Else, if we are *after* the first iteration, and still a copy, try again...
			else if( arguments.ContainsKey( tagName + count ) )
				return ParseTagNameForDuplicateKey( arguments, tagName, ++count );
				// Else, we are not a copy, and we are after the first iteration
			else
				return tagName + count;
		}

		/// <summary>
		/// TODO:  Finish comment
		/// </summary>
		/// <param name="def"></param>
//		private void SaveRelationalCommand( ControlDefinition def )
//		{
//			if( MOG_ControllerProject.GetCurrentSyncDataController() == null )
//			{
//				return;
//			}
//
//			string localPath = MOG_ControllerProject.GetCurrentSyncDataController().GetGameDataPath();
//			// If our localPath is the first thing we see in this command...
//			if( localPath != null && def.Command.ToLower().IndexOf( localPath ) == 0 )
//			{
//				// Get our pattern we need to replace
//				string pattern = def.Command.Substring(0, localPath.Length );
//				// Replace it with nothing
//				def.Command = def.Command.Replace( pattern, "" );
//			}
//		}

		private static string ParseRelationalCommand( string command )
		{
			if( MOG_ControllerProject.GetCurrentSyncDataController() == null )
			{
				return "";
			}

            string localPath = MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory();			

			if( command.IndexOf("\\") > -1 )
			{
				return localPath + command;
			}
			else
			{
				return localPath + "\\" + command;
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

		private static void AddTokensToArrayList( string[] tokens, ArrayList arrayList )
		{
			foreach( string token in tokens )
			{
				string tokenString =  token.Substring( token.IndexOf("{")+1, token.Length - "{".Length - 1 );
				arrayList.Add( new MogTaggedString( tokenString, token, MogTaggedString.FilenameStyleTypes.Raw, MOG_ControllerProject.GetWorkspaceDirectory() ) );
			}
		}

		private ArrayList GetArrayListOfAllTokenRelatedTags()
		{
			ArrayList returnArray = new ArrayList();
			foreach( MogTaggedString systemString in SystemArguments )
			{
				returnArray.Add("<" + systemString + ">");
			}
			foreach( MogTaggedString projectString in ProjectArguments ) 
			{
				returnArray.Add("<" + projectString + ">");
			}
			foreach( MogTaggedString timeString in TimeArguments )
			{
				returnArray.Add("<" + timeString + ">");
			}
			foreach( MogTaggedString userString in UserAndFileArguments )
			{
				returnArray.Add("<" + userString + ">");
			}
			return returnArray;
		}

		/// <summary>
		/// Get our MOG tags.
		/// </summary>
		/// <returns></returns>
		public SortedList GetMogTagNames()
		{
			SortedList returnList = new SortedList();
			try
			{
				foreach( MogTaggedString systemString in SystemArguments )
				{
					AddToReturnList(returnList, systemString.ToString(), systemString.AttachedItem.ToString());
				}
				foreach( MogTaggedString projectString in ProjectArguments ) 
				{
					AddToReturnList(returnList, projectString.ToString(), projectString.AttachedItem.ToString());
				}
				foreach( MogTaggedString timeString in TimeArguments )
				{
					AddToReturnList(returnList, timeString.ToString(), timeString.AttachedItem.ToString());
				}
				foreach( MogTaggedString userString in UserAndFileArguments )
				{
					AddToReturnList(returnList, userString.ToString(), userString.AttachedItem.ToString());
				}
			}
			catch( Exception ex )
			{
				ex.ToString();
			}
			return returnList;
		}
		/// <summary>
		/// Add to our SortedList (called returnList) in GetMogTagNames() without getting any exceptions
		/// </summary>
		/// <param name="listToAddTo"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		private static void AddToReturnList( SortedList listToAddTo, string key, string value )
		{
			if( !listToAddTo.ContainsKey( key ) )
			{
				string formattedString = MOG_Tokens.GetFormattedString( value );
				if( formattedString.Length < 1 )
				{
					formattedString = "Did Not Evaluate";
				}
				listToAddTo.Add( key, formattedString );
			}
		}

		#endregion Utility Methods

		#region ControlDef Event Methods
		private void DefArgumentButton_Click(object sender, EventArgs e)
		{
			ContextMenu cm = GenerateTagContextMenu( DefArgumentMenuItem_Click );
			Button button = (Button)sender;
			cm.Show( button, new Point( button.Width, 0 ) );
		}

		private static void DefArgumentMenuItem_Click( object sender, EventArgs e )
		{
			// Get the control definition??
			ControlDefinition def = GetControlDefinition( sender );
			string tag = ((MenuItem)sender).Text;
			def.DefTextBox.Text += tag.Substring( tag.IndexOf("<"), tag.IndexOf(">") + (">").Length);
		}

		#endregion ControlDef Event Methods

		#region Argument Parsing Methods
		/// <summary>
		/// Returns a string for what a command/argument pair's evaluation will be.
		/// </summary>
		/// <param name="def"></param>
		/// <returns></returns>
		protected internal string GetEvaluationString( ControlDefinition def ) 
		{
			return ParseArguments( def.Command ) + " " + ParseArguments( def.Arguments );
		}

		/// <summary>
		/// Replaces Tags with Arguments, getting rid of invalid tags (i.e. self-referencing or circular tag references)
		/// </summary>
		/// <param name="input">String of tags to parse</param>
		/// <returns></returns>
		private string ParseArguments(string input)
		{
			int maxDepth = 10;
			return ParseArguments(input, 0, maxDepth);
		}
		/// <summary>
		/// Not to be used by outside methods (see ParseArguments(string), instead).  
		///  Recursively finds the results to arguments passed in by evaluating ToolBox tags.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		/// <param name="currentDepth"></param>
		/// <param name="maxDepth"></param>
		private string ParseArguments(string input, int currentDepth, int maxDepth)
		{
			// If we are beyond our ability to interpret a tag, return our input and reset our currentDepth
			if( currentDepth >= maxDepth )
			{
				// Display our error out to the DevEnv...
				System.Diagnostics.Debug.WriteLine("\tMinor Error:  ToolBox::ParseArguments() reached the end of its recursion for input: "+input);
				return input;
			}
			// Increment current depth
			++currentDepth;
			// Store a list of our valid tags for later use...
			ArrayList validTags = GetArrayListOfAllTokenRelatedTags();

			// For each control in our custom controls, replace tag with argument
			foreach(Control control in this.CustomControls)		
			{
				ControlDefinition def = GetControlDefinition( control );
				// If this control has not ControlDefinition, skip it
				if( def == null || def.ControlType == ToolBoxControlType.Label)
				{
					continue;
				}

				// Create our tag pattern to search for
				string pattern = "<" + def.TagName + ">";
				// Store it
				validTags.Add( pattern );

				// Make sure we're able to parse filenames (i.e. keep Regex from seeing escape chars)
				pattern = Regex.Escape( pattern );

				// Get our arguments string to check for the repeatition of this tag
				string arguments = def.Arguments;
				// If we have an infinitely looping tag...
				if( Regex.Matches( arguments, pattern ).Count > 0 )
				{
					// Yank the tag, then skip it
					input = Regex.Replace( input, pattern, "" );
					continue;
				}

				// Replace all the occurences of this tag with our arguments
				input = Regex.Replace( input, pattern, arguments );
				// If we still have matches, we need to get rid of them...
				if( Regex.Matches( input, pattern ).Count > 0 )
					input = Regex.Replace( input, pattern, "" );
			}
			
			// Get an ArrayList of ONLY our MOG-related tags
			ArrayList mogTags = GetArrayListOfAllTokenRelatedTags();

			// See if we have any other tags ( open bracket + space OR word char(s) + close bracket )
			MatchCollection otherTags = Regex.Matches( input, @"<(\s|\w|\.)*>" );
			// If we have other tag(s), make sure they are valid.
			if( otherTags.Count > 0 )
			{
				foreach( Match possibleTag in otherTags )
				{
					string tag = possibleTag.Value; 
					// If the tag is a valid, continue...
					if( mogTags.Contains(tag) )
					{
						string tempTag = tag.Replace("<", "{");
						tempTag = tempTag.Replace(">", "}");
						// TODO: glk:  Here, I can eventually add ways to place an argument from a selected MOG_Filename or whatever...
						string formattedString = MOG_Tokens.GetFormattedString(tempTag);
						input = input.Replace( tag, formattedString );
					}
					else if( validTags.Contains( tag ) )
						continue;
						// Else, replace it with nothing
					else
						input = input.Replace( tag, "" );
				}
			}
			// If we still have a valid tag, recurse it
			if( input.IndexOf("<") > -1 && input.IndexOf(">") > -1 )
			{
				input = ParseArguments( input, ++currentDepth, maxDepth );
			}

			return input;
		}
		#endregion Argument Parsing Methods


		private void ToolBox_Resize(object sender, System.EventArgs e)
		{
			if( Controls.Count > 0 )
			{
				try
				{
					// Try re-initializing, which forces a re-draw/re-creation of our controls
					Initialize( mPlatformName );
				}
					// Catch everything, eating any error(s)
				catch
				{
					// Specifically, this is meant to catch errors we will find when we are given
					//  a resize command from MogMainForm when we are not even logged into a project...
				}
			} // end if
		}

		
       
	} // end class ToolBox

	#region ToolBoxControlType enumeration
	/// <summary>
	/// The types of CustomControls we have available to our ToolBox
	/// </summary>
	public enum ToolBoxControlType 
	{
		Button,
		ParameterButton,
		Label,
		FileCombo,
		IniCombo,
		RadioButton,
		CheckBox,
		LinkLabel,
	};
	#endregion ToolBoxControlType enumeration

	#region ToolBoxControlLocation enumeration
	/// <summary>
	/// Indicates which file a control should be saved to 
	///  (in addition to the file that contains all controls, that is)
	/// </summary>
	public enum ToolBoxControlLocation
	{
		/// <summary>
		/// User-level control only
		/// </summary>
		User,
		/// <summary>
		/// Control located in our department ini file
		/// </summary>
		Department,
		/// <summary>
		/// Control that is project-wide for every user
		/// </summary>
		Project,
	};
	#endregion ToolBoxControlLocation enumeration
} // end ns
