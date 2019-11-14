using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;

using MOG;
using MOG.INI;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.COMMAND;
using MOG.REPORT;

namespace MOG_ControlsLibrary.Common.MogControl_PriviledgesChangeForm
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class PriviledgesForm : System.Windows.Forms.Form
	{
		private const int OptionsPanel_MinWidth = 280;
		private int OldWidth;
		private Thread mMogProcess;
		private MOG_Priviledges mPriviledges;
		private TreeNode mGroupNode;

		private System.Windows.Forms.Panel PriviledgesMainPanel;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Panel OptionsPanel;
		private System.Windows.Forms.Panel MainStatusBarPanel;
		private System.Windows.Forms.StatusBar MainStatusBar;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel ButtonsPanel;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Button CancelButton1;
		private System.Windows.Forms.Button ApplyButton;
		private System.Windows.Forms.Splitter MainSplitter;
		private System.Windows.Forms.TreeView MainTreeView;
		private System.Windows.Forms.ContextMenu MainContextMenu;
		private System.Windows.Forms.MenuItem AddGroupMenuItem;
		private System.Windows.Forms.MenuItem DeleteGroupMenuItem;
		private System.Windows.Forms.MenuItem SaveChangesMenuItem;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.Label SelectedGroupLabel;
		private System.Windows.Forms.PropertyGrid MainPropertyGrid;
		private System.ComponentModel.IContainer components;

		public PriviledgesForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.OldWidth = this.Width;
			MOG_Main.Init_Client("");
			mMogProcess = new Thread(new ThreadStart(this.MogProcess));
			mMogProcess.Start();
			MOG_ControllerProject.LoginProject("BlackJupiter", "CURRENT");
			MOG_ControllerProject.LoginUser("Admin");
			mPriviledges = MOG_ControllerProject.GetPriviledges();

			InitializeMainTreeView( mPriviledges );
		}

		private void InitializeMainTreeView( MOG_Priviledges priviledges )
		{
			// Create our groupNode and set our global groupNode
			TreeNode groupNode = new TreeNode( "Groups" );
			this.mGroupNode = groupNode;

			//TreeNode userNode = new TreeNode( "Users" );
			this.MainTreeView.Nodes.Add( groupNode );
			//this.MainTreeView.Nodes.Add( userNode );

			SortedList users = priviledges.UsersList;
			ArrayList groups = priviledges.GroupsList;
			foreach( string groupName in groups )
			{
				TreeNode groupNameNode = groupNode.Nodes.Add( groupName );
				foreach( DictionaryEntry user in users )
				{
					string userGroupName =  user.Value.ToString();
					if( userGroupName == groupName )
					{
						groupNameNode.Nodes.Add( user.Key.ToString() );
					}
				}
			}

			groupNode.Expand();

//			foreach( DictionaryEntry user in users )
//			{
//				TreeNode userNameNode = userNode.Nodes.Add( user.Key.ToString() );
//				userNameNode.Nodes.Add( user.Value.ToString() );
//				userNameNode.Expand();
//			}
		}

		private void RefreshMainTreeView()
		{
			if( this.MainTreeView.Nodes.Count > 0 )
			{
				MainTreeView.Nodes.Clear();
			}
			InitializeMainTreeView( this.mPriviledges );
		}

		private void MogProcess()
		{
			while(!MOG_Main.isShutdown())
			{
				try
				{
					MOG_Main.Process();
				}
				catch(Exception ex)
				{
					MOG_Command pNotify = new MOG_Command();
					pNotify.Setup_NotifySystemException("MogProcess:Exception Error durring Mog.Process" + "\n\nMessage: " + ex.ToString() + "\n\nStackTrace:" + ex.StackTrace.ToString());
					MOG_ControllerSystem.GetCommandManager().CommandProcess(pNotify);

					MOG_REPORT.LogError("MogProcess", "Exception Error durring Mog.Process");
					MOG_REPORT.LogError("MogProcess", ex.ToString());
					MOG_REPORT.LogError("MogProcess", ex.StackTrace.ToString());
				}
			
				Thread.Sleep(100);
			}

			Application.Exit();
			mMogProcess.Abort();
			mMogProcess = null;			
		}

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
			MOG_Main.Shutdown( true );
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
			this.PriviledgesMainPanel = new System.Windows.Forms.Panel();
			this.MainSplitter = new System.Windows.Forms.Splitter();
			this.OptionsPanel = new System.Windows.Forms.Panel();
			this.MainPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.ButtonsPanel = new System.Windows.Forms.Panel();
			this.ApplyButton = new System.Windows.Forms.Button();
			this.CancelButton1 = new System.Windows.Forms.Button();
			this.OKButton = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.SelectedGroupLabel = new System.Windows.Forms.Label();
			this.MainTreeView = new System.Windows.Forms.TreeView();
			this.MainContextMenu = new System.Windows.Forms.ContextMenu();
			this.AddGroupMenuItem = new System.Windows.Forms.MenuItem();
			this.DeleteGroupMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.SaveChangesMenuItem = new System.Windows.Forms.MenuItem();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.MainStatusBarPanel = new System.Windows.Forms.Panel();
			this.MainStatusBar = new System.Windows.Forms.StatusBar();
			this.PriviledgesMainPanel.SuspendLayout();
			this.OptionsPanel.SuspendLayout();
			this.ButtonsPanel.SuspendLayout();
			this.panel1.SuspendLayout();
			this.MainStatusBarPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// PriviledgesMainPanel
			// 
			this.PriviledgesMainPanel.Controls.Add(this.MainSplitter);
			this.PriviledgesMainPanel.Controls.Add(this.OptionsPanel);
			this.PriviledgesMainPanel.Controls.Add(this.MainTreeView);
			this.PriviledgesMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PriviledgesMainPanel.Location = new System.Drawing.Point(0, 0);
			this.PriviledgesMainPanel.Name = "PriviledgesMainPanel";
			this.PriviledgesMainPanel.Size = new System.Drawing.Size(696, 389);
			this.PriviledgesMainPanel.TabIndex = 2;
			// 
			// MainSplitter
			// 
			this.MainSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.MainSplitter.Location = new System.Drawing.Point(232, 0);
			this.MainSplitter.Name = "MainSplitter";
			this.MainSplitter.Size = new System.Drawing.Size(8, 389);
			this.MainSplitter.TabIndex = 3;
			this.MainSplitter.TabStop = false;
			// 
			// OptionsPanel
			// 
			this.OptionsPanel.Controls.Add(this.MainPropertyGrid);
			this.OptionsPanel.Controls.Add(this.ButtonsPanel);
			this.OptionsPanel.Controls.Add(this.panel1);
			this.OptionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OptionsPanel.Location = new System.Drawing.Point(232, 0);
			this.OptionsPanel.Name = "OptionsPanel";
			this.OptionsPanel.Size = new System.Drawing.Size(464, 389);
			this.OptionsPanel.TabIndex = 2;
			this.OptionsPanel.Layout += new System.Windows.Forms.LayoutEventHandler(this.OptionsPanel_Layout);
			// 
			// MainPropertyGrid
			// 
			this.MainPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.MainPropertyGrid.CommandsVisibleIfAvailable = true;
			this.MainPropertyGrid.LargeButtons = false;
			this.MainPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.MainPropertyGrid.Location = new System.Drawing.Point(8, 32);
			this.MainPropertyGrid.Name = "MainPropertyGrid";
			this.MainPropertyGrid.Size = new System.Drawing.Size(456, 320);
			this.MainPropertyGrid.TabIndex = 3;
			this.MainPropertyGrid.Text = "propertyGrid1";
			this.MainPropertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.MainPropertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// ButtonsPanel
			// 
			this.ButtonsPanel.Controls.Add(this.ApplyButton);
			this.ButtonsPanel.Controls.Add(this.CancelButton1);
			this.ButtonsPanel.Controls.Add(this.OKButton);
			this.ButtonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ButtonsPanel.Location = new System.Drawing.Point(0, 349);
			this.ButtonsPanel.Name = "ButtonsPanel";
			this.ButtonsPanel.Size = new System.Drawing.Size(464, 40);
			this.ButtonsPanel.TabIndex = 2;
			// 
			// ApplyButton
			// 
			this.ApplyButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.ApplyButton.Enabled = false;
			this.ApplyButton.Location = new System.Drawing.Point(328, 8);
			this.ApplyButton.Name = "ApplyButton";
			this.ApplyButton.TabIndex = 2;
			this.ApplyButton.Text = "ApplyNoGo";
			this.ApplyButton.Visible = false;
			this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
			// 
			// CancelButton1
			// 
			this.CancelButton1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.CancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton1.Location = new System.Drawing.Point(240, 8);
			this.CancelButton1.Name = "CancelButton1";
			this.CancelButton1.TabIndex = 1;
			this.CancelButton1.Text = "Cancel";
			this.CancelButton1.Click += new System.EventHandler(this.CancelButton1_Click);
			// 
			// OKButton
			// 
			this.OKButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKButton.Location = new System.Drawing.Point(152, 8);
			this.OKButton.Name = "OKButton";
			this.OKButton.TabIndex = 0;
			this.OKButton.Text = "OK";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.SelectedGroupLabel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(464, 32);
			this.panel1.TabIndex = 1;
			// 
			// SelectedGroupLabel
			// 
			this.SelectedGroupLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.SelectedGroupLabel.Location = new System.Drawing.Point(96, 8);
			this.SelectedGroupLabel.Name = "SelectedGroupLabel";
			this.SelectedGroupLabel.Size = new System.Drawing.Size(280, 16);
			this.SelectedGroupLabel.TabIndex = 0;
			this.SelectedGroupLabel.Text = "No Group Selected";
			this.SelectedGroupLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// MainTreeView
			// 
			this.MainTreeView.ContextMenu = this.MainContextMenu;
			this.MainTreeView.Dock = System.Windows.Forms.DockStyle.Left;
			this.MainTreeView.ImageIndex = -1;
			this.MainTreeView.Location = new System.Drawing.Point(0, 0);
			this.MainTreeView.Name = "MainTreeView";
			this.MainTreeView.SelectedImageIndex = -1;
			this.MainTreeView.Size = new System.Drawing.Size(232, 389);
			this.MainTreeView.TabIndex = 1;
			this.toolTip.SetToolTip(this.MainTreeView, "Select or right click on an item inside this panel to change it");
			this.MainTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainTreeView_MouseDown);
			// 
			// MainContextMenu
			// 
			this.MainContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							this.AddGroupMenuItem,
																							this.DeleteGroupMenuItem,
																							this.menuItem4,
																							this.SaveChangesMenuItem});
			// 
			// AddGroupMenuItem
			// 
			this.AddGroupMenuItem.Index = 0;
			this.AddGroupMenuItem.Text = "Add Group";
			this.AddGroupMenuItem.Click += new System.EventHandler(this.AddGroupMenuItem_Click);
			// 
			// DeleteGroupMenuItem
			// 
			this.DeleteGroupMenuItem.Index = 1;
			this.DeleteGroupMenuItem.Text = "Delete Group";
			this.DeleteGroupMenuItem.Click += new System.EventHandler(this.DeleteGroupMenuItem_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 2;
			this.menuItem4.Text = "-";
			// 
			// SaveChangesMenuItem
			// 
			this.SaveChangesMenuItem.Index = 3;
			this.SaveChangesMenuItem.Text = "Save Changes";
			this.SaveChangesMenuItem.Click += new System.EventHandler(this.ApplyButton_Click);
			// 
			// MainStatusBarPanel
			// 
			this.MainStatusBarPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.MainStatusBarPanel.Controls.Add(this.MainStatusBar);
			this.MainStatusBarPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.MainStatusBarPanel.Location = new System.Drawing.Point(0, 389);
			this.MainStatusBarPanel.Name = "MainStatusBarPanel";
			this.MainStatusBarPanel.Size = new System.Drawing.Size(696, 24);
			this.MainStatusBarPanel.TabIndex = 3;
			// 
			// MainStatusBar
			// 
			this.MainStatusBar.Location = new System.Drawing.Point(0, -2);
			this.MainStatusBar.Name = "MainStatusBar";
			this.MainStatusBar.Size = new System.Drawing.Size(692, 22);
			this.MainStatusBar.TabIndex = 0;
			// 
			// PriviledgesForm
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.CancelButton1;
			this.ClientSize = new System.Drawing.Size(696, 413);
			this.Controls.Add(this.PriviledgesMainPanel);
			this.Controls.Add(this.MainStatusBarPanel);
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(528, 256);
			this.Name = "PriviledgesForm";
			this.Text = "Change MOG Group Priviledges";
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.PriviledgesForm_Layout);
			this.PriviledgesMainPanel.ResumeLayout(false);
			this.OptionsPanel.ResumeLayout(false);
			this.ButtonsPanel.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.MainStatusBarPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new PriviledgesForm());
		}

		private void CancelButton1_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Resize priviledges form based on width of OptionsPanel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PriviledgesForm_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
		{
			this.MainSplitter.SplitPosition -= this.OldWidth - this.Width;
			if( this.Width < this.MainTreeView.Width + this.MainSplitter.Width + OptionsPanel_MinWidth )
			{
				this.Width = OptionsPanel_MinWidth + MainSplitter.Width + MainTreeView.Width;
				this.OptionsPanel.Width = OptionsPanel_MinWidth;
				this.OptionsPanel.Location = new Point( this.Width - OptionsPanel_MinWidth, 
					this.OptionsPanel.Location.Y );
			}
			this.OldWidth = this.Width;
		}

		/// <summary>
		/// If our OptionsPanel size is changed, we also update our PriviledgesForm
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OptionsPanel_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
		{
			if( this.Width < this.MainTreeView.Width + this.MainSplitter.Width + OptionsPanel_MinWidth )
			{
				PriviledgesForm_Layout( sender, e );
			}
		}

		/// <summary>
		/// TODO: Save settings out to Ini
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ApplyButton_Click(object sender, System.EventArgs e)
		{
			e.ToString();
		}

		/// <summary>
		/// TODO: Delete a group from the tree and from MOG_Priviledges
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeleteGroupMenuItem_Click(object sender, System.EventArgs e)
		{
			TreeNode selectedNode = this.MainTreeView.SelectedNode;
			// If we have a node selected AND it is at the groupNode-level of the treeView...
			if( selectedNode != null && this.mGroupNode.Nodes.Contains( selectedNode ) )
			{
				// Remove the group, if we have it
				if( this.mPriviledges.RemoveGroup( selectedNode.Text ) )
				{
					this.RefreshMainTreeView();
				}
					// Else, report that we had an error
				else
				{
					MessageBox.Show( this, "Unable to remove group. " );					
				}
			}
			else
			{
				string nodeText = "<No node selected>";
				if( selectedNode != null )
				{
					nodeText = "<" + selectedNode.Text + ">";
				}
				MessageBox.Show( this, "The node, " + nodeText + ", is not valid.  Please select a Group Node." );
			}
		}

		/// <summary>
		/// TODO: Add a group to the tree and to MOG_Priviledges
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddGroupMenuItem_Click(object sender, System.EventArgs e)
		{
			AddGroupForm agf = new AddGroupForm();
			if( agf.ShowDialog( this ) == DialogResult.OK )
			{
				string newGroupName = agf.GroupNameTextBox.Text;
				this.mPriviledges.AddGroup( newGroupName );
				this.RefreshMainTreeView();
			}			
		}

		private void MainTreeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			TreeView tv = (TreeView)sender;
			try
			{
				tv.SelectedNode = (TreeNode)tv.GetNodeAt( e.X, e.Y );
				if( tv.SelectedNode != null )
				{
					UpdateMainPropertyGrid( tv.SelectedNode );
				}
			}
				// If we get exceptions, we eat them
			catch( Exception ex )
			{
				ex.ToString();
			}
			catch
			{
				e.ToString();
			}
		}

		private void UpdateMainPropertyGrid( TreeNode node )
		{
			// If this is one of our Group nodes
			if( this.mGroupNode.Nodes.Contains( node ) )
			{
				Mog_PriviledgesWrapper wrapper = new Mog_PriviledgesWrapper( this.mPriviledges );
				wrapper.InitializeWrapperToGroup( node.Text );
				this.MainPropertyGrid.SelectedObject = wrapper;
				SelectedGroupLabel.Text = node.Text;
			}
		}

		private void MainListView_DoubleClick(object sender, System.EventArgs e)
		{
			ListView view = (ListView)sender;
		}
	} // end class

	public class StringWrapper
	{
		private string mStringer;
		public string Stringer
		{
			get
			{
				return mStringer;
			}
			set
			{
				this.mStringer = value;
			}
		}

		public StringWrapper()
		{
			mStringer = "";
		}
	}
}// end namespace
