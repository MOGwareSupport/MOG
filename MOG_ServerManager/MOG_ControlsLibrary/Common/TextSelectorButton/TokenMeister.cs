using System;
using System.Collections;
using System.Windows.Forms;

using MOG;
using MOG.FILENAME;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for TokenMeister.
	/// </summary>
	public class TokenMeister
	{
		#region User definitions
		protected TokenArrayList tokens;

		public int Count { get { return this.tokens.Count; } }
		#endregion
		#region Constructors
		public TokenMeister()
		{
			//
			// TODO: Add constructor logic here
			//

			tokens = new TokenArrayList();
		}
		#endregion
		#region Member functions

		public void ClearTokens()
		{
			this.tokens.Clear();
		}

		public ArrayList GetTokenNames()
		{
			ArrayList tokenNames = new ArrayList();
			
			foreach (Token tok in this.tokens)
				tokenNames.Add( tok.tokenName );

			return tokenNames;
		}

		public ArrayList GetTokenNamesAndValues(string separator)
		{
			ArrayList tokenNames = new ArrayList();
			
			foreach (Token tok in this.tokens)
				tokenNames.Add( tok.tokenName + separator + tok.Dereference() );

			return tokenNames;
		}

		public void AddToken(string name, string val)
		{
			this.tokens.AddToken( name, val );
		}

		public void ChangeTokenValue(string token, string newVal)
		{
			foreach (Token tok in this.tokens)
			{
				if (tok.tokenName.ToLower() == token.ToLower())
				{
					tok.val = newVal;
					return;
				}

			}
		}

		public string DereferenceToken( string name ) 
		{
			Token tok = this.tokens.Get(name);
			if (tok == null)
				return "TOKEN NOT FOUND (" + name + ")";

			return tok.Dereference();
		}

		public ContextMenuStrip GetMenu(EventHandler eventHandler) 
		{
			ArrayList tokenList = new ArrayList();
			foreach (Token tok in this.tokens)
				tokenList.Add(tok.tokenName);

			return this.GetMenu(eventHandler, tokenList);
		}

		public ContextMenuStrip GetMenu(EventHandler eventHandler, ArrayList tokenList) 
		{
			ContextMenuStrip contextMenu = new ContextMenuStrip();

			foreach (string tokenName in tokenList) 
			{
				contextMenu.Items.Add(new ToolStripMenuItem(tokenName + "\t- " + this.DereferenceToken(tokenName), null, eventHandler));
			}
			
			return contextMenu;
		}

		public bool IsToken(string name) 
		{
			foreach (Token tok in this.tokens)
			{
				if (name.ToLower() == tok.tokenName.ToLower())
					return true;
			}
			
			return false;
		}

		public string ExpandString(string line, char[] delimiters) 
		{
			if (delimiters.Length < 2)
				return "";

			string expansion = "";
            string[] substrings = line.Split(delimiters);

			foreach (string substring in substrings)
			{
				if (this.IsToken(substring))
					expansion += this.DereferenceToken(substring);
				else
					expansion += substring;
			}

			return expansion;
		}
		#endregion
	}

	#region Supporting classes
	public class TokenArrayList : ArrayList 
	{
		public void AddToken( string name, string val) 
		{
			this.Add( new Token(name, val) );
		}

		public void AddToken(Token tok) 
		{
			this.Add(tok);
		}

		public Token Get(string name) 
		{
			foreach (Token tok in this)
			{
				if (tok.tokenName.ToLower() == name.ToLower())
					return tok;
			}

			return null;
		}
	}

	public class Token 
	{
		public string tokenName;
		public string val;

		public Token(string name, string val)
		{
			this.tokenName = name;
			this.val = val;
		}

		public string Dereference() 
		{
			return this.val;
		}
	}
	#endregion

}









