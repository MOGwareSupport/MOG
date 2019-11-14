using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG;
using MOG.TIME;
using MOG.PROMPT;
using MOG.FILENAME;

using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_Client.Forms.ArgumentControl
{
	/// <summary>
	/// Summary description for MogArgumentsButton.
	/// </summary>
	[DefaultEvent("MogContextMenuItemClick")]
	public class MogArgumentsButton : System.Windows.Forms.UserControl
	{
		#region User Variables
		private ComboBox mComboBox;
		private TextBox mTextBox;
		public const string RecommendedToolTip_Text = "Display a list of MOG-related arguments."
			+"\r\nLeft side shows \"Tokens\" available."
			+"\r\nRight side shows what the given Token currently evaluates to.";

		/// <summary>
		/// The text of the Button
		/// </summary>
		[Category("Appearance"), Description("The text contained in the MogArgumentsButton.")]
		public string ButtonText
		{
			get
			{
				return this.ArgsButton.Text;
			}
			set
			{
				this.ArgsButton.Text = value;
			}
		}

		[Category("TargetControl"), Description("Target control to recieve token")]
		public ComboBox TargetComboBox
		{
			set 
			{ 
				this.mComboBox = value;				
			}
			get { return this.mComboBox;}
		}

		[Category("TargetControl"), Description("Target control to recieve token")]
		public TextBox TargetTextBox
		{
			set 
			{ 
				this.mTextBox = value;				
			}
			get { return this.mTextBox;}
		}

		/// <summary>
		/// Occurs whenever a MenuItem in the ContextMenu is Clicked.
		/// </summary>
		[Category("Action"), Description("Occurs whenever a MenuItem in the ContextMenu is Clicked.  If this is not "
								 + "attached, you (the programmer) will get an error.")]
		public event EventHandler MogContextMenuItemClick;
		
		private EventHandler mInternalClick;	

		#region Properties for Arguments
		private MOG_Filename mCurrentMogFilename;
		/// <summary>
		/// The MOG_Filename to use for the MOG_Tokens filename identifiers...
		/// </summary>
		[Category("Behavior"), Description("When set, this will be used to fill in the MenuItems that are MOG_Filename-related.  "
			+ "This is NOT MEANT TO BE SET AT DESIGN-TIME.")]
		public MOG_Filename MOGAssetFilename
		{
			get
			{
				return mCurrentMogFilename;
			}
			set
			{
				this.mCurrentMogFilename = value;
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

		private ArrayList mTimeArguments;
		/// <summary>
		/// Read-only list of TimeArguments
		/// </summary>
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
		/// <summary>
		/// Read-only list of UserAndFileArguments
		/// </summary>
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
		private ArrayList mProjectArguments;
		/// <summary>
		/// Read-only list of ProjectArguments
		/// </summary>
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
		private ArrayList mPackageArguments;
		/// <summary>
		/// Read-only list of TimeArguments
		/// </summary>
		public ArrayList PackageArguments
		{
			get
			{
				if( mPackageArguments == null )
				{
					mPackageArguments = new ArrayList();
					string[] packageTokens = MOG_Tokens.GetAllPackageTokens();
					AddTokensToArrayList( packageTokens, mPackageArguments );
				}
				return mPackageArguments;
			}
		}

		private ArrayList mRippingArguments;
		/// <summary>
		/// Read-only list of TimeArguments
		/// </summary>
		public ArrayList RippingArguments
		{
			get
			{
				if( mRippingArguments == null )
				{
					mRippingArguments = new ArrayList();
					string[] packageTokens = MOG_Tokens.GetAllRipperTokens();
					AddTokensToArrayList( packageTokens, mRippingArguments );
				}
				return mRippingArguments;
			}
		}

		private System.Windows.Forms.Button ArgsButton;
		private System.Windows.Forms.ToolTip toolTip;
		
		#endregion Properties for Arguments

		/// <summary>
		/// This region gets rid of some of our confusing EventHandlers
		/// </summary>
		#region Hidden EventHandlers
		// Hide our Click (programmer can still use it in source, but not at design-time)
		[Browsable(false)]
		public new event EventHandler Click;
		// Hide our DoubleClick
		[Browsable(false)]
		public new event EventHandler DoubleClick;

		#endregion Hidden EventHandlers

		#endregion User Variables

		private System.ComponentModel.IContainer components;

		public MogArgumentsButton()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Statement to keep our Click and DoubleClick event delegates from complaining about not being used
			if(Click == null || DoubleClick == null){/*Do nothing*/}

			// Set our tooltip to be a multi-line tooltip
			this.toolTip.SetToolTip(this.ArgsButton, RecommendedToolTip_Text);

			mInternalClick = new EventHandler(this.InternalMenuClick);
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
			this.components = new System.ComponentModel.Container();
			this.ArgsButton = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// ArgsButton
			// 
			this.ArgsButton.Location = new System.Drawing.Point(0, 0);
			this.ArgsButton.Name = "ArgsButton";
			this.ArgsButton.Size = new System.Drawing.Size(24, 23);
			this.ArgsButton.TabIndex = 0;
			this.ArgsButton.Text = ">";
			this.ArgsButton.Click += new System.EventHandler(this.ArgsButton_Click);
			// 
			// MogArgumentsButton
			// 
			this.Controls.Add(this.ArgsButton);
			this.Name = "MogArgumentsButton";
			this.Size = new System.Drawing.Size(24, 23);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Show our ContextMenu for our Args, based on the EventHandler the user has given us
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ArgsButton_Click(object sender, System.EventArgs e)
		{
			// If we have our MogContextMenuItemClick event defined...
			if(MogContextMenuItemClick != null)
			{
				ContextMenu cm = GenerateTagContextMenu(MogContextMenuItemClick);
				cm.Show( this.ArgsButton, new Point( this.ArgsButton.Width, 0 ) );
			}
			else if(mComboBox != null)
			{

				ContextMenu cm = GenerateTagContextMenu(mInternalClick);
				cm.Show( this.ArgsButton, new Point( this.ArgsButton.Width, 0 ) );
//				mComboBox.Text += "new";
			}
			else if(mTextBox != null)
			{
				ContextMenu cm = GenerateTagContextMenu(mInternalClick);
				cm.Show( this.ArgsButton, new Point( this.ArgsButton.Width, 0 ) );
//				mTextBox.Text += "new";
			}
				// Otherwise, warn our programmer that he screwed up!
			else
			{
				MessageBox.Show(this, "No MogContextMenuClickHandler defined!", "Programmer Error!", MessageBoxButtons.OK, 
					MessageBoxIcon.Exclamation);
			}
		}

		private void InternalMenuClick(object sender, System.EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			
			if(mComboBox != null)
			{
				// Check to see if the user has selected a place for the new argument to go
				if (mComboBox.SelectedText.Length > 0)
				{
					string firstPart = mComboBox.Text.Substring(0, mComboBox.SelectionStart);
					string lastPart = mComboBox.Text.Substring(mComboBox.SelectionStart);
					mComboBox.Text = firstPart + GetArgument(item) + lastPart;
				}
				else
				{
					mComboBox.Text += GetArgument(item);
				}
			}
			else if(mTextBox != null)
			{
				// Check to see if the user has selected a place for the new argument to go
				if (mTextBox.SelectedText.Length > 0)
				{
					string firstPart = mTextBox.Text.Substring(0, mTextBox.SelectionStart);
					string lastPart = mTextBox.Text.Substring(mTextBox.SelectionStart);
					mTextBox.Text = firstPart + GetArgument(item) + lastPart;
				}
				else
				{
					mTextBox.Text += GetArgument(item);
				}
			}
			else
			{
				MessageBox.Show(this, "No MogContextMenuClickHandler defined!", "Programmer Error!", MessageBoxButtons.OK, 
					MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// Generates a ContextMenu for all the tags the user can select
		/// </summary>
		/// <param name="mogMenuItemClick"></param>
		/// <returns></returns>
		private ContextMenu GenerateTagContextMenu(EventHandler mogMenuItemClick)
		{
			ContextMenu cm = new ContextMenu();
			SortedList tagNames = new SortedList();//this.GetControlTagNames();  //Put this back in if we want to do ToolBox controls...
			// Foreach entry we have in tagNames...
//			foreach( DictionaryEntry entry in tagNames )
//			{
//				// Get our tag and arguments
//				string tag = (string)entry.Key;
//				string arguments = (string)entry.Value;
//
//				// Indicate whether we have arguments attached
//				if( arguments.Length < 1 )
//				{
//					arguments = "No arguments";
//				}
//
//				// If we have a filename, lets only get the last bit of it
//				if( arguments.IndexOf( "\\" ) > -1 )
//				{
//					arguments = arguments.Substring( arguments.LastIndexOf("\\")+1 );
//				}
//
//				string formattedArgs = MOG_Tokens.GetFormattedString( "{"+tag+"}", null, null );
//
//				// Add our MenuItem with an event handler
//				MenuItem tagMenuItem = new MenuItem( "<" + tag + ">=='" + arguments + "'");
//
//				tagMenuItem.Click += mogMenuItemClick;
//				cm.MenuItems.Add( tagMenuItem );
//			}

//			// If we have no Custom tags, let the user know...
//			if( tagNames.Count < 1 )
//			{
//				cm.MenuItems.Add( "No Custom Tags Available" );
//			}

			// Create our full seeds list
			string seeds = MOG_Tokens.GetPackageTokenSeeds("Game~Packages{All}PackageFile.Pak", "All", "", "", "Packages\\PackageFile.Pak");
			seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetSystemTokenSeeds());
			seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetOSTokenSeeds());
			seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetProjectTokenSeeds(MOG_ControllerProject.GetProject()));
			seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetTimeTokenSeeds(new MOG_Time()));
			if (mCurrentMogFilename != null)
			{
				// Setup any asset specific seeds
				seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetFilenameTokenSeeds(mCurrentMogFilename));
				seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetRipperTokenSeeds(mCurrentMogFilename.GetEncodedFilename() + "\\Files.Imported", "*.*", mCurrentMogFilename.GetEncodedFilename() + "\\Files.All"));
			}
			

			// Populate System Items
			GenerateTagContextMenu_PopulateFromArrayList( cm, "System Related", this.SystemArguments, mogMenuItemClick, seeds );
			// Populate Project Items
			GenerateTagContextMenu_PopulateFromArrayList( cm, "Project Related", this.ProjectArguments, mogMenuItemClick, seeds );
			// Populate Time Items
			GenerateTagContextMenu_PopulateFromArrayList( cm, "Time Related", this.TimeArguments, mogMenuItemClick, seeds );
			// Populate File and User Items
			GenerateTagContextMenu_PopulateFromArrayList( cm, "File and User Related", this.UserAndFileArguments, mogMenuItemClick, seeds );
			// Populate Package
			GenerateTagContextMenu_PopulateFromArrayList( cm, "Package Related", this.PackageArguments, mogMenuItemClick, seeds );
			// Populate Ripper related
			GenerateTagContextMenu_PopulateFromArrayList( cm, "Ripping Related", this.RippingArguments, mogMenuItemClick, seeds );

			return cm;
		}

		// Copied over from ToolBox...  Not needed, but might come in handy.
//		private SortedList GetControlTagNames()
//		{
//			SortedList tagNameArguments = new SortedList();
//			// Go through all of our controls
//			foreach( Control control in this.CustomControls )
//			{
//				ControlDefinition def = GetControlDefinition( control );
//
//				// If we found an invalid ControlDefinition, skip it
//				if( def == null )
//				{
//					continue; 
//				}
//
//				// Make sure we have a unique TagName before proceeding
//				def.TagName = ParseTagNameForDuplicateKey( tagNameArguments, def.TagName, 0 );
//				tagNameArguments.Add( def.TagName, def.Arguments );
//			}
//			return tagNameArguments;
//		}


		/// <summary>
		/// Takes a ContextMenu, a name for a MenuItem, and an ArrayList of MogTaggedStrings to
		///  populate and add a MenuItem with the name passed in as `menuItemName`.
		/// </summary>
		/// <param name="cm">ContextMenu to populate</param>
		/// <param name="menuItemName">MenuItem name to create</param>
		/// <param name="tags">Tags to populate with</param>
		public void GenerateTagContextMenu_PopulateFromArrayList( ContextMenu cm, string menuItemName, 
			ArrayList tags, EventHandler menuItemEventHandler, string seeds )
		{
			MenuItem argumentsMenuItem = new MenuItem( menuItemName );
			foreach( MogTaggedString tbString in tags )
			{
				// Get our argument
				string formattedString = tbString.AttachedItem.ToString();

				formattedString = MOG_Tokens.GetFormattedString(formattedString, seeds);

				if( formattedString.Length < 1 )
				{
					formattedString = "Did Not Evaluate";
				}

				// Add our MenuItem with an event handler
				MenuItem tagMenuItem = new MenuItem("{" + tbString.ToString() + "} --- '" 
					+formattedString+ "'");
				tagMenuItem.Click += menuItemEventHandler;
				argumentsMenuItem.MenuItems.Add( tagMenuItem );
			}
			cm.MenuItems.Add( argumentsMenuItem );
		}

		#region Utility Methods
		/// <summary>
		/// Used for our properties (see top of code under User Variables)
		/// </summary>
		/// <param name="tokens"></param>
		/// <param name="arrayList"></param>
		private void AddTokensToArrayList( string[] tokens, ArrayList arrayList )
		{
			foreach( string token in tokens )
			{
				string tokenString =  token.Substring( token.IndexOf("{")+1, token.Length - "{".Length - 1 );
				arrayList.Add(new MogTaggedString(tokenString, token, MogTaggedString.FilenameStyleTypes.Raw, MOG_ControllerProject.GetWorkspaceDirectory()));
			}
		}
		#endregion Utility Methods

		#region Public Utility Methods
		public static string GetArgumentStatic(MenuItem item)
		{
			string tag = item.Text;
			try
			{
				return tag.Substring( tag.IndexOf("{"), tag.IndexOf("}") + ("}").Length);
			}
			catch(Exception ex)
			{
				MOG_Prompt.PromptMessage("Error Getting Argument", ex.Message, ex.StackTrace, MOG_ALERT_LEVEL.ERROR);
				return "";
			}
		}

		public string GetArgument(MenuItem item)
		{
			return GetArgumentStatic(item);
		}
		#endregion Public Utility Methods
	}
}