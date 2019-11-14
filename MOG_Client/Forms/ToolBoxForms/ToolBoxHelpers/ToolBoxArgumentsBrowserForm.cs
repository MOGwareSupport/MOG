using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MOG_Client.Forms;

namespace MOG_Client.Forms.ToolBoxForms
{
	/// <summary>
	/// Summary description for ToolBoxArgumentsBrowserForm.
	/// </summary>
	public class ToolBoxArgumentsBrowserForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Provides access to everything inside our ToolBox
		/// </summary>
		private ToolBox mToolBox;
		public System.Windows.Forms.ListView TagsListView;
		private System.Windows.Forms.ToolTip toolTip;
		public System.Windows.Forms.ColumnHeader TagHeader;
		public System.Windows.Forms.ColumnHeader ArgumentHeader;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Button CancelButton1;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Initialize this form with standard InitializeComponent method.
		/// </summary>
		protected ToolBoxArgumentsBrowserForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}
		/// <summary>
		/// Initialize with a toolbox pointer
		/// </summary>
		/// <param name="toolBox">Pointer to the ToolBox that contains all our Custom Controls</param>
		public ToolBoxArgumentsBrowserForm( ToolBox toolBox ) : this()
		{
			// Set our global ToolBox
			this.mToolBox = toolBox;

			// Set our column width
			TagsListView_Resize( this.TagsListView, new EventArgs() );

			SortedList tagArgumentPairs = mToolBox.GetControlTagNames();
			PopulateListViewFromSortedLists( tagArgumentPairs );
			SortedList mogTagArgumentPairs = mToolBox.GetMogTagNames();
			PopulateListViewFromSortedLists( mogTagArgumentPairs );
		}

		private void PopulateListViewFromSortedLists( SortedList tagArgumentPairs )
		{
			foreach( DictionaryEntry entry in tagArgumentPairs )
			{
				string tag = (string)entry.Key;
				ListViewItem item = new ListViewItem( tag );
				// If we do not have a valid tag, skip...
				if( tag.Length < 1 )
					continue;
				// Add our argument
				item.SubItems.Add( (string)entry.Value );
				this.TagsListView.Items.Add( item );
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ToolBoxArgumentsBrowserForm));
			this.OKButton = new System.Windows.Forms.Button();
			this.CancelButton1 = new System.Windows.Forms.Button();
			this.TagsListView = new System.Windows.Forms.ListView();
			this.TagHeader = new System.Windows.Forms.ColumnHeader();
			this.ArgumentHeader = new System.Windows.Forms.ColumnHeader();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// OKButton
			// 
			this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.OKButton.Location = new System.Drawing.Point(160, 256);
			this.OKButton.Name = "OKButton";
			this.OKButton.TabIndex = 1;
			this.OKButton.Text = "OK";
			// 
			// CancelButton1
			// 
			this.CancelButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CancelButton1.Location = new System.Drawing.Point(248, 256);
			this.CancelButton1.Name = "CancelButton1";
			this.CancelButton1.TabIndex = 2;
			this.CancelButton1.Text = "Cancel";
			// 
			// TagsListView
			// 
			this.TagsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.TagsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						   this.TagHeader,
																						   this.ArgumentHeader});
			this.TagsListView.GridLines = true;
			this.TagsListView.Location = new System.Drawing.Point(0, 0);
			this.TagsListView.Name = "TagsListView";
			this.TagsListView.Size = new System.Drawing.Size(328, 248);
			this.TagsListView.TabIndex = 0;
			this.toolTip.SetToolTip(this.TagsListView, "Select Tags you would like to use as Arguments (Hold down [CTRL] to select multip" +
				"le)");
			this.TagsListView.View = System.Windows.Forms.View.Details;
			this.TagsListView.Resize += new System.EventHandler(this.TagsListView_Resize);
			// 
			// TagHeader
			// 
			this.TagHeader.Text = "Tag Name";
			this.TagHeader.Width = 106;
			// 
			// ArgumentHeader
			// 
			this.ArgumentHeader.Text = "Argument(s)";
			this.ArgumentHeader.Width = 192;
			// 
			// ToolBoxArgumentsBrowserForm
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.CancelButton1;
			this.ClientSize = new System.Drawing.Size(328, 285);
			this.Controls.Add(this.TagsListView);
			this.Controls.Add(this.CancelButton1);
			this.Controls.Add(this.OKButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ToolBoxArgumentsBrowserForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ToolBoxArgumentsBrowserForm";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Makes it so that our Arguments column will expand with our ListView
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TagsListView_Resize(object sender, System.EventArgs e)
		{
			ListView view = (ListView)sender;
			this.ArgumentHeader.Width = view.Width - this.TagHeader.Width;
		}
	}
}
