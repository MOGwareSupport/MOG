using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace MOG_Client.Forms.ToolBoxForms
{
	/// <summary>
	/// Summary description for ToolBoxLocationCheckBox.
	/// </summary>
	public class ToolBoxLocationComboBox : System.Windows.Forms.ComboBox
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ToolBoxLocationComboBox()
			:base()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// If we have any existing items, clear them
			if( this.Items.Count > 0 )
			{
				this.Items.Clear();
			}
			this.Items.Add( ToolBoxControlLocation.User.ToString() );
			this.Items.Add( ToolBoxControlLocation.Department.ToString() );
			this.Items.Add( ToolBoxControlLocation.Project.ToString() );
			this.SelectedIndex = 0;
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
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
