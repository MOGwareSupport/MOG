using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using MOG_ServerManager.Utilities;

namespace MOG_ServerManager
{
	public class TextSelectorButton : Button
	{
		#region System definitions
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			// 
			// TextSelectorButton
			// 
			this.FlatStyle = System.Windows.Forms.FlatStyle.System;

		}
		#endregion
		#endregion
		#region User definitions
		protected bool tokenize;
		protected bool hasControl;
		protected string selection;
		protected string selectedToken;
		protected string selectedValue;
		protected string delimiter;
		protected TokenMeister tokenizer;
		protected TextBox associatedTextBox;
		private System.Windows.Forms.ToolTip toolTip;
		protected Label expansionLabel;
		#endregion
		#region Properties
		[Category("TextSelectorButton")]
		public bool Tokenize { get { return this.tokenize; } set { this.tokenize = value; } }
		
		[Category("TextSelectorButton")]
		public TextBox AssociatedTextBox 
		{
			get { return this.associatedTextBox; }
			set 
			{
				this.associatedTextBox = value;
				InstallControlHandler();
			}
		}

		[Category("TextSelectorButton")]
		public Label ExpansionLabel 
		{
			get { return this.expansionLabel; }
			set 
			{
				this.expansionLabel = value;
				InstallControlHandler();
			}
		}

		[Category("TextSelectorButton")]
		public bool HasControl 
		{
			get { return this.hasControl; }
			set 
			{ 
				this.hasControl = value;
				InstallControlHandler();
			}
		}

		public string Selection { get { return this.selection; } }
		public string SelectedToken { get { return this.selectedToken; } }
		public string SelectedValue { get { return this.selectedValue; } }
		public string Delimiter { get { return this.delimiter; } }

		[Category("TextSelectorButton")]
		public ArrayList Tokens
		{
			get { return new ArrayList(); }
			set { }
		}
		#endregion
		#region Events
		public event tsbEventHandler SelectionChanged;
		#endregion
		#region Constructor	
		public TextSelectorButton()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			init();
		}

		private void init()
		{ 
			this.tokenizer = new TokenMeister();
			this.Click += new EventHandler(TextSelectorButton_Click);
			this.MouseDown += new MouseEventHandler(TextSelectorButton_MouseDown);
			this.delimiter = "\t";
		}
		#endregion
		#region Member functions
		public void ClearTokens()
		{
			this.tokenizer.ClearTokens();
		}

		private void InstallControlHandler()
		{
			if (this.hasControl  &&  this.associatedTextBox != null)
			{
				this.associatedTextBox.TextChanged += new EventHandler(associatedTextBox_TextChanged);
				this.toolTip.SetToolTip(this.associatedTextBox, this.DereferenceLine(this.associatedTextBox.Text));
			}
		}

		public void ShowMenu()
		{
			ConstructMenu( new EventHandler(SubmenuItem_Click) ).Show(this, new Point(this.Size.Width, 0));
		}

		public ContextMenuStrip ConstructMenu(EventHandler eventHandler)
		{
			ContextMenuStrip contextMenu = new ContextMenuStrip();
			ArrayList names;

			if (this.tokenize)
				names = this.tokenizer.GetTokenNamesAndValues( this.delimiter );
			else
				names = this.tokenizer.GetTokenNames();

			foreach (string tokenName in names) 
			{
				contextMenu.Items.Add(new ToolStripMenuItem(tokenName, null, eventHandler));
			}

			return contextMenu;
		}

		public void Add(string text)
		{
			this.tokenizer.AddToken(text, "NOVALUE");
		}

		public void AddToken(string token, string val)
		{
			this.tokenizer.AddToken(token, val);
		}

		public void ChangeTokenValue(string token, string newVal)
		{
			this.tokenizer.ChangeTokenValue(token, newVal);
		}

		public void OnSelectionChanged()
		{
			if (this.SelectionChanged != null)
				this.SelectionChanged(this, new tsbEventArgs( this.selection, this.selectedToken, this.SelectedValue ));
		}

		public string DereferenceToken(string token)
		{
			return this.tokenizer.DereferenceToken(token);
		}

		public bool IsToken(string token)
		{
			return this.tokenizer.IsToken(token);
		}

		public string DereferenceLine(string line)
		{
			return this.tokenizer.ExpandString(line, "{}".ToCharArray());
		
			
			/*string newLine = "";

			string[] strings = line.Split("{}".ToCharArray());
			foreach (string token in strings)
			{
				if ( IsToken(token) )
				{
					// this is a token that needs to be dereferenced
					newLine += DereferenceToken( token );
				}
				else
				{
					// just put in the text
					newLine += token;
				}
			}

			return newLine;	*/
		}
		#endregion
		#region Event handlers
		private void associatedTextBox_TextChanged(object sender, EventArgs e)
		{
			if (this.hasControl  &&  this.associatedTextBox != null)//  &&  this.expansionLabel != null)
				this.toolTip.SetToolTip(this.associatedTextBox, this.DereferenceLine(this.associatedTextBox.Text));
				//this.expansionLabel.Text = DereferenceLine( this.associatedTextBox.Text );
		}

		public void SubmenuItem_Click(object sender, System.EventArgs e)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			if (item == null)
				return;

			this.selection = item.Text;
			this.selectedToken = item.Text.Split(this.delimiter.ToCharArray())[0];
			this.selectedValue = this.tokenizer.DereferenceToken( this.selectedToken );

			// fill in textbox and label if they exist
			if (this.associatedTextBox != null)
			{
				this.associatedTextBox.Text += "{" + this.selectedToken + "}";
				if (this.expansionLabel != null)
					this.expansionLabel.Text = DereferenceLine( this.associatedTextBox.Text );
			}

			// notify listeners of selection
			OnSelectionChanged();
		}

		public void TextSelectorButton_Click(object sender, System.EventArgs e) 
		{
			ShowMenu();
		}

		public void TextSelectorButton_MouseDown(object sender, MouseEventArgs e)
		{
		}
	
	}
		#endregion

	#region Event handling stuff
	public delegate void tsbEventHandler(object sender, tsbEventArgs e);
	public class tsbEventArgs : System.EventArgs
	{
		private string selection;
		private string selectedToken;
		private string selectedValue;
		
		public tsbEventArgs(string selection, string selectedToken, string selectedValue) :base()
		{
			this.selection = selection;
			this.selectedToken = selectedToken;
			this.selectedValue = selectedValue;
		}

		public string Selection { get { return this.selection; } }
		public string SelectedToken { get { return this.selectedToken; } }
		public string SelectedValue { get { return this.selectedValue; } }
	}
	#endregion

}




