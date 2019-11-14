using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server.Server_Utilities
{
	/// <summary>
	/// Summary description for ListViewColumnFormatterForm.
	/// </summary>
	public class ListViewColumnFormatterForm : System.Windows.Forms.Form
	{
		#region System definitions
		private System.Windows.Forms.ListView lvAvailableColumns;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnRestoreDefaults;
		private System.Windows.Forms.Button btnMoveDown;
		private System.Windows.Forms.Button btnMoveUp;
		private System.Windows.Forms.ListView lvDisplayedColumns;
		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.Label lblDisplayedColumns;
		private System.Windows.Forms.Label lblAvailableColumns;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ListViewColumnFormatterForm));
			this.lvAvailableColumns = new System.Windows.Forms.ListView();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnRestoreDefaults = new System.Windows.Forms.Button();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this.lvDisplayedColumns = new System.Windows.Forms.ListView();
			this.groupBox = new System.Windows.Forms.GroupBox();
			this.lblDisplayedColumns = new System.Windows.Forms.Label();
			this.lblAvailableColumns = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lvAvailableColumns
			// 
			this.lvAvailableColumns.FullRowSelect = true;
			this.lvAvailableColumns.HideSelection = false;
			this.lvAvailableColumns.Location = new System.Drawing.Point(16, 40);
			this.lvAvailableColumns.Name = "lvAvailableColumns";
			this.lvAvailableColumns.Size = new System.Drawing.Size(152, 200);
			this.lvAvailableColumns.TabIndex = 0;
			this.lvAvailableColumns.View = System.Windows.Forms.View.SmallIcon;
			this.lvAvailableColumns.DoubleClick += new System.EventHandler(this.lvAvailableColumns_DoubleClick);
			this.lvAvailableColumns.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvAvailableColumns_MouseUp);
			// 
			// btnAdd
			// 
			this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnAdd.Location = new System.Drawing.Point(176, 88);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(96, 23);
			this.btnAdd.TabIndex = 1;
			this.btnAdd.Text = "A&dd >";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemove.Location = new System.Drawing.Point(176, 112);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(96, 23);
			this.btnRemove.TabIndex = 2;
			this.btnRemove.Text = "< &Remove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnOK
			// 
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(360, 272);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(440, 272);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			// 
			// btnRestoreDefaults
			// 
			this.btnRestoreDefaults.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRestoreDefaults.Location = new System.Drawing.Point(176, 208);
			this.btnRestoreDefaults.Name = "btnRestoreDefaults";
			this.btnRestoreDefaults.Size = new System.Drawing.Size(96, 23);
			this.btnRestoreDefaults.TabIndex = 5;
			this.btnRestoreDefaults.Text = "Re&store Defaults";
			this.btnRestoreDefaults.Click += new System.EventHandler(this.btnRestoreDefaults_Click);
			// 
			// btnMoveDown
			// 
			this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnMoveDown.Location = new System.Drawing.Point(440, 112);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.TabIndex = 6;
			this.btnMoveDown.Text = "Move &Down";
			this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
			// 
			// btnMoveUp
			// 
			this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnMoveUp.Location = new System.Drawing.Point(440, 88);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.TabIndex = 7;
			this.btnMoveUp.Text = "Move &Up";
			this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
			// 
			// lvDisplayedColumns
			// 
			this.lvDisplayedColumns.FullRowSelect = true;
			this.lvDisplayedColumns.HideSelection = false;
			this.lvDisplayedColumns.Location = new System.Drawing.Point(280, 40);
			this.lvDisplayedColumns.Name = "lvDisplayedColumns";
			this.lvDisplayedColumns.Size = new System.Drawing.Size(152, 200);
			this.lvDisplayedColumns.TabIndex = 8;
			this.lvDisplayedColumns.View = System.Windows.Forms.View.SmallIcon;
			this.lvDisplayedColumns.DoubleClick += new System.EventHandler(this.lvDisplayedColumns_DoubleClick);
			this.lvDisplayedColumns.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvDisplayedColumns_MouseUp);
			// 
			// groupBox
			// 
			this.groupBox.Location = new System.Drawing.Point(16, 256);
			this.groupBox.Name = "groupBox";
			this.groupBox.Size = new System.Drawing.Size(512, 8);
			this.groupBox.TabIndex = 9;
			this.groupBox.TabStop = false;
			// 
			// lblDisplayedColumns
			// 
			this.lblDisplayedColumns.Location = new System.Drawing.Point(280, 24);
			this.lblDisplayedColumns.Name = "lblDisplayedColumns";
			this.lblDisplayedColumns.Size = new System.Drawing.Size(152, 16);
			this.lblDisplayedColumns.TabIndex = 10;
			this.lblDisplayedColumns.Text = "Display&ed Columns";
			// 
			// lblAvailableColumns
			// 
			this.lblAvailableColumns.Location = new System.Drawing.Point(16, 24);
			this.lblAvailableColumns.Name = "lblAvailableColumns";
			this.lblAvailableColumns.Size = new System.Drawing.Size(152, 16);
			this.lblAvailableColumns.TabIndex = 11;
			this.lblAvailableColumns.Text = "&Available Columns";
			// 
			// ListViewColumnFormatterForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(544, 310);
			this.Controls.Add(this.lblAvailableColumns);
			this.Controls.Add(this.lblDisplayedColumns);
			this.Controls.Add(this.groupBox);
			this.Controls.Add(this.lvDisplayedColumns);
			this.Controls.Add(this.btnMoveUp);
			this.Controls.Add(this.btnMoveDown);
			this.Controls.Add(this.btnRestoreDefaults);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.lvAvailableColumns);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ListViewColumnFormatterForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add/Remove Columns";
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region User definitions
		private ArrayList defaultCols;
		private ArrayList permanentCols;
		private ArrayList displayedColumns;

		public ArrayList DisplayedColumns { get { return this.displayedColumns; } }

		#endregion
		#region Constructor
		public ListViewColumnFormatterForm(ArrayList allColumns, ArrayList defaultColumns, ArrayList permanentColumns, ListView listView)
		{
			InitializeComponent();
			
			// set up member vars
			this.defaultCols = (ArrayList)defaultColumns.Clone();
			this.permanentCols = (ArrayList)permanentColumns.Clone();
			this.displayedColumns = new ArrayList();
			
			// get displayed columns out of listview
			foreach (ColumnHeader header in listView.Columns)
			{
				this.displayedColumns.Add( header.Text );
				this.lvDisplayedColumns.Items.Add( header.Text );
			}

			// put the rest in available listview
			foreach (string colName in allColumns)
			{
				if ( !this.displayedColumns.Contains(colName) )
					this.lvAvailableColumns.Items.Add( colName );
			}

			UpdateButtonsEnabledStatus();
		}

		public ListViewColumnFormatterForm(ArrayList allColumns, ArrayList defaultColumns, ArrayList visibleColumns, ArrayList permanentColumns, ListView listView)
		{
			InitializeComponent();
			
			// set up member vars
			this.defaultCols = (ArrayList)defaultColumns.Clone();
			this.permanentCols = (ArrayList)permanentColumns.Clone();
			this.displayedColumns = (ArrayList)visibleColumns.Clone();
			
			// get displayed columns out of listview
			foreach (ColumnHeader header in listView.Columns)
			{
				this.displayedColumns.Add( header.Text );
				this.lvDisplayedColumns.Items.Add( header.Text );
			}

			// put the rest in available listview
			foreach (string colName in allColumns)
			{
				if ( !this.displayedColumns.Contains(colName) )
					this.lvAvailableColumns.Items.Add( colName );
			}

			UpdateButtonsEnabledStatus();
		}

//		public ListViewColumnFormatterForm(ArrayList cols)
//		{
//			InitializeComponent();
//
//			init(cols, new ArrayList());
//		}
//
//		public ListViewColumnFormatterForm(ArrayList cols, ArrayList permanentCols)
//		{
//			InitializeComponent();
//
//			init(cols, permanentCols);
//		}

		private void init(ArrayList cols, ArrayList permanentCols)
		{
			this.defaultCols = cols;
			this.permanentCols = permanentCols;
			this.displayedColumns = new ArrayList();

			this.lvAvailableColumns.Items.Clear();
			this.lvDisplayedColumns.Items.Clear();
			
			foreach (string colName in cols)
				this.lvDisplayedColumns.Items.Add(colName);

			UpdateButtonsEnabledStatus();
		}
		#endregion
		#region Member functions
		private void RemoveSelectedColumns()
		{
			foreach (ListViewItem item in this.lvDisplayedColumns.SelectedItems)
			{
				if ( !this.permanentCols.Contains(item.Text) )
				{
					this.lvDisplayedColumns.Items.Remove( item );
					this.lvAvailableColumns.Items.Add( item );
				}
			}
		}
		private void AddSelectedColumns()
		{
			foreach (ListViewItem item in this.lvAvailableColumns.SelectedItems)
			{
				this.lvAvailableColumns.Items.Remove( item );
				this.lvDisplayedColumns.Items.Add( item );
			}		
		}

		private void UpdateButtonsEnabledStatus()
		{
			if (this.lvAvailableColumns.Items.Count > 0)
				this.btnAdd.Enabled = true;
			else
				this.btnAdd.Enabled = false;

			if ((this.lvDisplayedColumns.Items.Count > 0  ||  (this.lvDisplayedColumns.SelectedItems.Count > 0 && !this.permanentCols.Contains(this.lvDisplayedColumns.SelectedItems[0]))))
				this.btnRemove.Enabled = true;
			else
				this.btnRemove.Enabled = false;

			if ((this.lvDisplayedColumns.SelectedItems.Count > 0  &&  this.lvDisplayedColumns.SelectedItems[0] != this.lvDisplayedColumns.Items[0]))
				this.btnMoveUp.Enabled = true;
			else
				this.btnMoveUp.Enabled = false;

			if ((this.lvDisplayedColumns.SelectedItems.Count > 0  &&  this.lvDisplayedColumns.SelectedItems[0] != this.lvDisplayedColumns.Items[ this.lvDisplayedColumns.Items.Count-1 ]))
				this.btnMoveDown.Enabled = true;
			else
				this.btnMoveDown.Enabled = false;
		}
		#endregion
		#region Events

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			AddSelectedColumns();
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			RemoveSelectedColumns();
		}

		private void btnRestoreDefaults_Click(object sender, System.EventArgs e)
		{
			this.lvAvailableColumns.Items.Clear();
			this.lvAvailableColumns.View = View.List;
			this.lvDisplayedColumns.Items.Clear();
			this.lvDisplayedColumns.View = View.List;

			foreach (string colName in this.defaultCols)
				this.lvDisplayedColumns.Items.Add(colName);
		}

		private void btnMoveUp_Click(object sender, System.EventArgs e)
		{
			if (this.lvDisplayedColumns.SelectedItems.Count > 0)
			{
				ArrayList items = new ArrayList();
				for (int i=0; i<this.lvDisplayedColumns.Items.Count; i++)
				{
					ListViewItem lvItem = this.lvDisplayedColumns.Items[i];
					if (lvItem == this.lvDisplayedColumns.SelectedItems[0])
					{
						items.Remove( this.lvDisplayedColumns.Items[i-1] );
						items.Add( this.lvDisplayedColumns.Items[i] );
						items.Add( this.lvDisplayedColumns.Items[i-1] );
						//++i;
					}
					else
						items.Add( lvItem );
				}

				this.lvDisplayedColumns.Items.Clear();
				this.lvDisplayedColumns.View = View.List;
				for (int i=0; i<items.Count; i++)
					this.lvDisplayedColumns.Items.Add( (ListViewItem)items[i]);

				UpdateButtonsEnabledStatus();
				this.lvDisplayedColumns.Focus();
			}
		}

		private void btnMoveDown_Click(object sender, System.EventArgs e)
		{
			if (this.lvDisplayedColumns.SelectedItems.Count > 0)
			{
				ArrayList items = new ArrayList();
				for (int i=0; i<this.lvDisplayedColumns.Items.Count; i++)
				{
					ListViewItem lvItem = this.lvDisplayedColumns.Items[i];
					if (lvItem == this.lvDisplayedColumns.SelectedItems[0])
					{
						items.Add( this.lvDisplayedColumns.Items[i+1] );
						items.Add( this.lvDisplayedColumns.Items[i] );
						++i;
					}
					else
						items.Add( lvItem );
				}

				this.lvDisplayedColumns.Items.Clear();
				this.lvDisplayedColumns.View = View.List;
				for (int i=0; i<items.Count; i++)
					this.lvDisplayedColumns.Items.Add( (ListViewItem)items[i]);

				UpdateButtonsEnabledStatus();
				this.lvDisplayedColumns.Focus();
			}
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if (this.lvDisplayedColumns.Items.Count <= 0)
			{
				MessageBox.Show("You must include at least one column in the Displayed Columns box", "Missing Column", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			this.displayedColumns.Clear();
			foreach (ListViewItem item in this.lvDisplayedColumns.Items)
				this.displayedColumns.Add( item.Text );

			this.DialogResult = DialogResult.OK;
			Dispose();
		}

		private void lvAvailableColumns_DoubleClick(object sender, System.EventArgs e)
		{
			this.btnAdd.PerformClick();
			UpdateButtonsEnabledStatus();
		}

		private void lvDisplayedColumns_DoubleClick(object sender, System.EventArgs e)
		{		
			this.btnRemove.PerformClick();
			UpdateButtonsEnabledStatus();
		}

		private void lvAvailableColumns_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			UpdateButtonsEnabledStatus();
		}

		private void lvDisplayedColumns_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			UpdateButtonsEnabledStatus();
		}
	}
	#endregion
}
