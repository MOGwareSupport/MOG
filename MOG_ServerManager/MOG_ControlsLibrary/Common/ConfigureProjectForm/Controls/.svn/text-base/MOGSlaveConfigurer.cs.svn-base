using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG_ServerManager.Utilities;

using MOG.PROPERTIES;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for SlaveConfigurer.
	/// </summary>
	public class MOGSlaveConfigurer : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.GroupBox grbBoxSlaveCommands;
		private System.Windows.Forms.CheckBox chkboxSlaveWillPackage;
		private System.Windows.Forms.CheckBox chkboxSlaveWillBuild;
		private System.Windows.Forms.ListView lvAssetConfig;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.TextBox tbSlaveMachine;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ListView lvSlaveMachines;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ContextMenuStrip cmSlaves;
		private System.Windows.Forms.ToolStripMenuItem miRemoveSlave;
		private System.Windows.Forms.ContextMenuStrip cmAssetConfig;
		private System.Windows.Forms.ToolStripMenuItem miToggleAsset;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnBrowse = new System.Windows.Forms.Button();
			this.grbBoxSlaveCommands = new System.Windows.Forms.GroupBox();
			this.chkboxSlaveWillPackage = new System.Windows.Forms.CheckBox();
			this.chkboxSlaveWillBuild = new System.Windows.Forms.CheckBox();
			this.lvAssetConfig = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.tbSlaveMachine = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.lvSlaveMachines = new System.Windows.Forms.ListView();
			this.cmSlaves = new System.Windows.Forms.ContextMenuStrip();
			this.miRemoveSlave = new System.Windows.Forms.ToolStripMenuItem();
			this.cmAssetConfig = new System.Windows.Forms.ContextMenuStrip();
			this.miToggleAsset = new System.Windows.Forms.ToolStripMenuItem();
			this.grbBoxSlaveCommands.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnBrowse.Location = new System.Drawing.Point(208, 216);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(24, 23);
			this.btnBrowse.TabIndex = 34;
			this.btnBrowse.Text = "...";
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// grbBoxSlaveCommands
			// 
			this.grbBoxSlaveCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grbBoxSlaveCommands.Controls.Add(this.chkboxSlaveWillPackage);
			this.grbBoxSlaveCommands.Controls.Add(this.chkboxSlaveWillBuild);
			this.grbBoxSlaveCommands.Controls.Add(this.lvAssetConfig);
			this.grbBoxSlaveCommands.Location = new System.Drawing.Point(320, 16);
			this.grbBoxSlaveCommands.Name = "grbBoxSlaveCommands";
			this.grbBoxSlaveCommands.Size = new System.Drawing.Size(304, 232);
			this.grbBoxSlaveCommands.TabIndex = 33;
			this.grbBoxSlaveCommands.TabStop = false;
			this.grbBoxSlaveCommands.Text = "Slave Commands";
			// 
			// chkboxSlaveWillPackage
			// 
			this.chkboxSlaveWillPackage.Location = new System.Drawing.Point(24, 48);
			this.chkboxSlaveWillPackage.Name = "chkboxSlaveWillPackage";
			this.chkboxSlaveWillPackage.Size = new System.Drawing.Size(96, 16);
			this.chkboxSlaveWillPackage.TabIndex = 1;
			this.chkboxSlaveWillPackage.Text = "&Package";
			// 
			// chkboxSlaveWillBuild
			// 
			this.chkboxSlaveWillBuild.Location = new System.Drawing.Point(24, 32);
			this.chkboxSlaveWillBuild.Name = "chkboxSlaveWillBuild";
			this.chkboxSlaveWillBuild.Size = new System.Drawing.Size(96, 16);
			this.chkboxSlaveWillBuild.TabIndex = 0;
			this.chkboxSlaveWillBuild.Text = "&Build";
			// 
			// lvAssetConfig
			// 
			this.lvAssetConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lvAssetConfig.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							this.columnHeader1,
																							this.columnHeader2});
			this.lvAssetConfig.FullRowSelect = true;
			this.lvAssetConfig.Location = new System.Drawing.Point(24, 80);
			this.lvAssetConfig.Name = "lvAssetConfig";
			this.lvAssetConfig.Size = new System.Drawing.Size(264, 136);
			this.lvAssetConfig.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.lvAssetConfig.TabIndex = 28;
			this.lvAssetConfig.View = System.Windows.Forms.View.Details;
			this.lvAssetConfig.DoubleClick += new System.EventHandler(this.lvAssetConfig_DoubleClick);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Asset Name";
			this.columnHeader1.Width = 134;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Allowed";
			this.columnHeader2.Width = 53;
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemove.Location = new System.Drawing.Point(208, 176);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.TabIndex = 32;
			this.btnRemove.Text = "&Remove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAdd.Enabled = false;
			this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnAdd.Location = new System.Drawing.Point(240, 216);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.TabIndex = 31;
			this.btnAdd.Text = "&Add";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// tbSlaveMachine
			// 
			this.tbSlaveMachine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbSlaveMachine.Location = new System.Drawing.Point(16, 216);
			this.tbSlaveMachine.Name = "tbSlaveMachine";
			this.tbSlaveMachine.Size = new System.Drawing.Size(184, 20);
			this.tbSlaveMachine.TabIndex = 30;
			this.tbSlaveMachine.Text = "";
			this.tbSlaveMachine.TextChanged += new System.EventHandler(this.tbSlaveMachine_TextChanged);
			this.tbSlaveMachine.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbSlaveMachine_KeyUp);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(16, 24);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(128, 16);
			this.label8.TabIndex = 29;
			this.label8.Text = "Slave Machines";
			// 
			// lvSlaveMachines
			// 
			this.lvSlaveMachines.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.lvSlaveMachines.FullRowSelect = true;
			this.lvSlaveMachines.HideSelection = false;
			this.lvSlaveMachines.Location = new System.Drawing.Point(16, 40);
			this.lvSlaveMachines.Name = "lvSlaveMachines";
			this.lvSlaveMachines.Size = new System.Drawing.Size(184, 160);
			this.lvSlaveMachines.TabIndex = 28;
			this.lvSlaveMachines.View = System.Windows.Forms.View.List;
			this.lvSlaveMachines.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvSlaveMachines_MouseUp);
			// 
			// cmSlaves
			// 
			this.cmSlaves.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {this.miRemoveSlave});
			// 
			// miRemoveSlave
			// 
			this.miRemoveSlave.Text = "Remove";
			// 
			// cmAssetConfig
			// 
			this.cmAssetConfig.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {this.miToggleAsset});
			this.cmAssetConfig.Opening += cmAssetConfig_Popup;
			// 
			// miToggleAsset
			// 
			this.miToggleAsset.Text = "Toggle";
			// 
			// MOGSlaveConfigurer
			// 
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.grbBoxSlaveCommands);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.tbSlaveMachine);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.lvSlaveMachines);
			this.Name = "MOGSlaveConfigurer";
			this.Size = new System.Drawing.Size(640, 264);
			this.grbBoxSlaveCommands.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private ArrayList allAssets;
		private ListViewItem currentSlave;

		#region Constructors
		public MOGSlaveConfigurer()
		{
			InitializeComponent();
			init();
		}

		public MOGSlaveConfigurer(ArrayList assets)
		{
			InitializeComponent();
			init();

			LoadValues(assets);
		}

		private void init()
		{
			this.allAssets = new ArrayList();
			this.tbSlaveMachine.Focus();
			this.currentSlave = null;
			this.grbBoxSlaveCommands.Enabled = false;
		}

		public void LoadValues(ArrayList assets)
		{
			// Set this.allAssets first of all
			this.allAssets = new ArrayList();
			foreach (object obj in assets)
			{
				if (obj is string)
				{
					this.allAssets.Add( (string)obj );
				}
			}

			foreach (object obj in assets)
			{
				if (obj is string)
				{
					string asset = (string)obj;
					ListViewItem item = new ListViewItem();
					item.Text = asset;
					item.SubItems.Add("Yes");
					this.lvAssetConfig.Items.Add(item);
				}
				else if (obj is MOG_Properties)
				{
					MOG_Properties mai = (MOG_Properties)obj;
					ListViewItem item = new ListViewItem();
					////item.Text = mai.mAssetKey;
					item.SubItems.Add("Yes");
					this.lvAssetConfig.Items.Add(item);

					string[] slaveNames = mai.ValidSlaves.Split(",".ToCharArray());
					foreach (string slaveName in slaveNames)
					{
						if (slaveName.Trim() == "")
						{
							continue;
						}

						if (Utils.ListViewContainsText(this.lvSlaveMachines, slaveName))
						{
							ListViewItem slaveItem = Utils.GetListViewItem(this.lvSlaveMachines, slaveName);
							if ( !(slaveItem.Tag is SlaveData) )
							{
								slaveItem.Tag = new SlaveData(false, false, new ArrayList());
								continue;
							}
							
							SlaveData slaveData = slaveItem.Tag as SlaveData;
							////slaveData.assets.Add(mai.mAssetKey);
						}
						else
						{
							ListViewItem slaveItem = new ListViewItem(slaveName);
							slaveItem.Tag = new SlaveData(false, false, new ArrayList( new string[] { "KEYKEY" } )); ////mai.mAssetKey } ));
							this.lvSlaveMachines.Items.Add(slaveItem);
						}
					}
				}
			}
		}

		#endregion
		#region Accessors
		public string ConstructValidSlavesString( string assetName )
		{
			string validSlaves = "";

			foreach (ListViewItem item in this.lvSlaveMachines.Items)
			{
				SlaveData slaveData = item.Tag as SlaveData;
				if (slaveData == null)
					continue;

				if (slaveData.assets.Contains( assetName ))
				{
					if (validSlaves != "")
						validSlaves += ",";
					
					validSlaves += item.Text;
				}
			}

			return validSlaves;
		}
		#endregion
		#region Member functions
		public bool ConfigurationValid()
		{
			return true;
		}

		public bool IsValid(bool showMessages)
		{
			return true;
		}

		private void AddSlave(string name)
		{
			ListViewItem item;
			if ( !Utils.ListViewContainsText(this.lvSlaveMachines, name) )
			{
				item = new ListViewItem( name );
				item.Tag = new SlaveData( false, false, (ArrayList)this.allAssets.Clone() );
				this.lvSlaveMachines.Items.Add( item );
			}
		}

		private void PopulateAssetConfig( SlaveData data )
		{
			this.chkboxSlaveWillBuild.Checked = data.willBuild;
			this.chkboxSlaveWillPackage.Checked = data.willPackage;

			this.lvAssetConfig.Items.Clear();
			foreach (string assetName in this.allAssets)
			{
				ListViewItem item = new ListViewItem(assetName);

				if ( data.assets.Contains(assetName) )
				{
					item.SubItems.Add("Yes");
				}
				else
				{
					item.SubItems.Add("No");
				}

				this.lvAssetConfig.Items.Add(item);
			}

			
		}

		private void SaveAssetConfig( SlaveData data )
		{
			data.willBuild = this.chkboxSlaveWillBuild.Checked;
			data.willPackage = this.chkboxSlaveWillPackage.Checked;

			data.assets.Clear();
			foreach (ListViewItem item in this.lvAssetConfig.Items)
			{
				if (item.SubItems[1].Text == "Yes")
				{
					data.assets.Add(item.Text);
				}
			}
		}

		private void ClearSlaveOptions()
		{
			foreach (ListViewItem item in this.lvAssetConfig.Items)
			{
				item.SubItems[1].Text = "No";
			}
		}
		private void SetSlaveOptions()
		{
			foreach (ListViewItem item in this.lvAssetConfig.Items)
			{
				item.SubItems[1].Text = "Yes";
			}
		}
		#endregion
		#region Event handlers

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			foreach ( ListViewItem item in this.lvSlaveMachines.SelectedItems )
			{
				this.lvSlaveMachines.Items.Remove(item);
			}
		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			AddSlave(this.tbSlaveMachine.Text);
		}

		private void btnBrowse_Click(object sender, System.EventArgs e)
		{
			
		}

		private void tbSlaveMachine_TextChanged(object sender, System.EventArgs e)
		{
			if (this.tbSlaveMachine.Text == "")
			{
				this.btnAdd.Enabled = false;
			}
			else
			{
				this.btnAdd.Enabled = true;
			}
		}

		private void tbSlaveMachine_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				this.btnAdd.PerformClick();
				this.tbSlaveMachine.Focus();
				this.tbSlaveMachine.SelectAll();
			}
		}

		private void lvSlaveMachines_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (this.lvSlaveMachines.SelectedItems.Count > 0)
			{
				ListViewItem item = this.lvSlaveMachines.SelectedItems[0];

				this.grbBoxSlaveCommands.Enabled = true;
				
				if (this.currentSlave != null)
					SaveAssetConfig( (SlaveData)this.currentSlave.Tag );

				this.currentSlave = item;

				PopulateAssetConfig( (SlaveData)item.Tag );
			}
			else
			{
				this.grbBoxSlaveCommands.Enabled = false;
			}
		}

		private void lvAssetConfig_DoubleClick(object sender, System.EventArgs e)
		{
			if (this.lvAssetConfig.SelectedItems.Count > 0)
			{
				ListViewItem item = this.lvAssetConfig.SelectedItems[0];
				if (item.SubItems[1].Text == "Yes")
				{
					item.SubItems[1].Text = "No";
				}
				else
				{
					item.SubItems[1].Text = "Yes";
				}
			}
		}

		private void cmAssetConfig_Popup(object sender, EventArgs e)
		{

		}
		#endregion
	}


	class SlaveData 
	{
		public bool willBuild;
		public bool willPackage;
		public ArrayList assets;

		public SlaveData(bool willBuild, bool willPackage, ArrayList assets)
		{
			this.willBuild = willBuild;
			this.willPackage = willPackage;
			this.assets = assets;
		}
	}
}




/*
#region User definitions
#endregion
#region System definitions
#endregion
#region Constructors
#endregion
#region Member functions
#endregion
#region Event handlers
#endregion
*/


