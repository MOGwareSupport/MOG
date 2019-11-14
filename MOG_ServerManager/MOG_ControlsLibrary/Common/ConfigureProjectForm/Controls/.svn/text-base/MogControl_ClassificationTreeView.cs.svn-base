using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG.INI;
using MOG.TIME;
using MOG.PROJECT;
using MOG.DATABASE;
using MOG.PROPERTIES;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.FILENAME;
using MOG.PROMPT;

using MOG_ServerManager.Utilities;
using MOG_ControlsLibrary;

namespace MOG_ServerManager
{
	/// <summary>
	/// A TreeView that knows how to load and modify classifications
	/// </summary>
	public class MogControl_ClassificationTreeView : System.Windows.Forms.UserControl
	{
		#region System defs
		protected CodersLab.Windows.Controls.TreeView tvClassifications;
		private System.Windows.Forms.ContextMenuStrip cmClassifications;
		private System.Windows.Forms.ToolStripMenuItem miAddClassification;
		private System.Windows.Forms.ToolStripMenuItem miRemove;
		private System.Windows.Forms.ToolStripMenuItem miShowConfiguration;
		private System.Windows.Forms.Splitter splitter1;
		protected System.Windows.Forms.PropertyGrid propertyGrid;
		protected System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.ToolStripMenuItem menuItem1;
		private System.Windows.Forms.ToolStripMenuItem menuItem2;
		private System.Windows.Forms.ToolStripMenuItem menuItem3;
		protected System.Windows.Forms.ImageList imageList;
		private System.ComponentModel.IContainer components;

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MogControl_ClassificationTreeView));
			this.tvClassifications = new CodersLab.Windows.Controls.TreeView();
			this.cmClassifications = new System.Windows.Forms.ContextMenuStrip();
			this.miAddClassification = new System.Windows.Forms.ToolStripMenuItem();
			this.miRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.miShowConfiguration = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel2 = new System.Windows.Forms.Panel();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvClassifications
			// 
			this.tvClassifications.ContextMenuStrip = this.cmClassifications;
			this.tvClassifications.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvClassifications.HideSelection = false;
			this.tvClassifications.ImageList = this.imageList;
			this.tvClassifications.Location = new System.Drawing.Point(0, 0);
			this.tvClassifications.Name = "tvClassifications";
			this.tvClassifications.PathSeparator = "~";
			this.tvClassifications.SelectedImageIndex = 1;
			this.tvClassifications.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			this.tvClassifications.SelectionMode = CodersLab.Windows.Controls.TreeViewSelectionMode.SingleSelect;
			this.tvClassifications.ShowLines = false;
			this.tvClassifications.Size = new System.Drawing.Size(597, 536);
			this.tvClassifications.TabIndex = 0;
			this.tvClassifications.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvClassifications_MouseDown);
			this.tvClassifications.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvClassifications_AfterSelect);
			this.tvClassifications.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvClassifications_AfterLabelEdit);
			this.tvClassifications.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvClassifications_KeyUp);
			// 
			// cmClassifications
			// 
			this.cmClassifications.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
																							  this.miAddClassification,
																							  this.miRemove,
																							  this.miShowConfiguration,
																							  this.menuItem1,
																							  this.menuItem2,
																							  this.menuItem3});
			// 
			// miAddClassification
			// 
			this.miAddClassification.Text = "Add Classification";
			this.miAddClassification.Click += new System.EventHandler(this.miAddClassification_Click);
			// 
			// miRemove
			// 
			this.miRemove.Text = "Remove";
			this.miRemove.Click += new System.EventHandler(this.miRemove_Click);
			// 
			// miShowConfiguration
			// 
			this.miShowConfiguration.Checked = true;
			this.miShowConfiguration.Text = "Show Configuration";
			this.miShowConfiguration.Click += new System.EventHandler(this.miShowConfiguration_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Enabled = false;
			this.menuItem1.Text = "Load Default";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Enabled = false;
			this.menuItem2.Text = "Load Project";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Enabled = false;
			this.menuItem3.Text = "Save";
			this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
			// 
			// imageList
			// 
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// propertyGrid
			// 
			this.propertyGrid.CommandsVisibleIfAvailable = true;
			this.propertyGrid.Cursor = System.Windows.Forms.Cursors.HSplit;
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.LargeButtons = false;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(320, 520);
			this.propertyGrid.TabIndex = 1;
			this.propertyGrid.Text = "propertyGrid";
			this.propertyGrid.ToolbarVisible = false;
			this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(597, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 536);
			this.splitter1.TabIndex = 3;
			this.splitter1.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.tvClassifications);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(597, 536);
			this.panel2.TabIndex = 4;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(0, 533);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(336, 3);
			this.splitter2.TabIndex = 3;
			this.splitter2.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panel3);
			this.panel1.Controls.Add(this.splitter2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(600, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(336, 536);
			this.panel1.TabIndex = 2;
			// 
			// panel3
			// 
			this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel3.Controls.Add(this.propertyGrid);
			this.panel3.Location = new System.Drawing.Point(8, 8);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(320, 520);
			this.panel3.TabIndex = 4;
			// 
			// MogControl_ClassificationTreeView
			// 
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.Name = "MogControl_ClassificationTreeView";
			this.Size = new System.Drawing.Size(936, 536);
			this.panel2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Member vars
		protected MOG_Properties defaultProps = null;
		private int oldWidth = -1;
		private int oldHeight = -1;
		private ArrayList newClasses = new ArrayList();
		private ArrayList removedClasses = new ArrayList();
		private ArrayList imageArrayList = new ArrayList();

		//protected ProgressBarForm progressBarForm;
		
		protected bool immediateMode = false;
		#endregion
		#region Properties
		public bool ImmediateMode
		{
			get { return this.immediateMode; }
			set { this.immediateMode = value; }
		}

		public int NumClassifications
		{
			get { return CountClassifications(); }
		}

		public ArrayList AllNodesFullPathList
		{
			get 
			{
				return GetAllNodesFullPathList(this.tvClassifications.Nodes);
			}
		}

		public ArrayList RootMOG_Properties
		{
			get
			{
				ArrayList props = new ArrayList();
				foreach (classTreeNode tn in this.tvClassifications.Nodes)
				{
					// Make sure we have our props?
					if (tn.props != null)
					{
						tn.props.Classification = tn.FullPath;	// make sure class name is set
						props.Add(tn.props);		// get each MOG_Property object
					}
				}

				return props;
			}
		}

		public bool ConfigurationVisible
		{
			get { return this.miShowConfiguration.Checked; }
			set 
			{
				this.miShowConfiguration.Checked = value;
				if (this.miShowConfiguration.Checked)
					ShowConfiguration();
				else
					HideConfiguration();
			}
		}

		public bool ShowConfigurationMenuItemVisible
		{
			get { return this.miShowConfiguration.Visible; }
			set { this.miShowConfiguration.Visible = value; }
		}

		public bool AddClassificationMenuItemVisible
		{
			get { return this.miAddClassification.Visible; }
			set { this.miAddClassification.Visible = value; }
		}

		public bool RemoveClassificationMenuItemVisible
		{
			get { return this.miRemove.Visible; }
			set { this.miRemove.Visible = value; }
		}
		#endregion
		#region Constants
		protected const int BLUEDOT_INDEX	= 0;
		protected const int ARROW_INDEX	= 1;
		#endregion
		#region Constructor
		public MogControl_ClassificationTreeView()
		{
			InitializeComponent();

			MOG_Properties.PropertyGrid = this.propertyGrid;
		}
		#endregion
		#region Private functions
		// recursively counts all the classes currently loaded in teh tree view
		private int CountClassifications()
		{
			int count = 0;
			foreach (classTreeNode tn in this.tvClassifications.Nodes)
				count += CountClassifications_helper(tn);

			return count;
		}

		// helps CountClassifications()
		private int CountClassifications_helper(classTreeNode tn)
		{
			int count = 0;

			if (!tn.IsAssetFilenameNode)
				count = 1;		// count tn

			foreach (classTreeNode subNode in tn.Nodes)
				count += CountClassifications_helper(subNode);
			
			return count;
		}

		// recursively builds a list of the FullPath member of every node in nodes
		private ArrayList GetAllNodesFullPathList(TreeNodeCollection nodes)
		{
			ArrayList fullNameList = new ArrayList();

			if (nodes == null)
				return fullNameList;

			foreach (TreeNode tn in nodes)
			{
				if (tn.TreeView != null)
					fullNameList.Add(tn.FullPath);

				// recurse
				foreach (TreeNode subNode in tn.Nodes)
					fullNameList.AddRange( GetAllNodesFullPathList(subNode.Nodes) );
			}

			return fullNameList;
		}

		// places a new classification into parentNode's node list, or in the root of the tree view if parentNode is null
		protected virtual void AddClassificationNode(classTreeNode parentNode)
		{
			// get the TreeNodeCollection to which to add the new class node
			TreeNodeCollection nodes = this.tvClassifications.Nodes;
			if (parentNode != null)
				nodes = parentNode.Nodes;

			// create the node NewClassificationX where X is the number of nodes called NewClassification at the insertion level (to make the name unique)
			classTreeNode newClassNode = new classTreeNode("NewClassification" + Utils.CountNodesThatStartWithString(nodes, "NewClassification").ToString());
			newClassNode.ForeColor = Color.Blue;
			newClassNode.IsNewClass = true;

			nodes.Add(newClassNode);
			
			if (newClassNode.Parent == null)
			{
				newClassNode.ImageIndex = BLUEDOT_INDEX;
				newClassNode.SelectedImageIndex = BLUEDOT_INDEX;
			}
			else 
			{
				newClassNode.ImageIndex = ARROW_INDEX;
				newClassNode.SelectedImageIndex = ARROW_INDEX;
			}

			this.newClasses.Add(newClassNode);			// mark it as a new class
			newClassNode.EnsureVisible();
			this.tvClassifications.LabelEdit = true;	// have the user edit its name inline
			newClassNode.BeginEdit();
		}

		// loop through SelectedNodes deleting as we go
		private void RemoveSelectedClassNodes()
		{
			foreach (classTreeNode tn in this.tvClassifications.SelectedNodes)
				RemoveClassificationNode(tn);
		}

		// recursively check baseClass to see if it or any of its child classes contain assets
		private bool ClassTreeContainsAssets(string baseClass)
		{
			if (MOG_DBAssetAPI.GetAllAssetsByClassification(baseClass).Count > 0)
				return true;

			// recurse by looking up child classes
			MOG_Project proj = MOG_ControllerProject.GetProject();
			if (proj != null)
			{
				foreach (string className in proj.GetSubClassificationNames(baseClass))
				{
					if (ClassTreeContainsAssets(baseClass + "~" + className))
						return true;
				}
			}

			return false;
		}

		// remove a class node
		private void RemoveClassificationNode(classTreeNode node)
		{
			if (node == null  ||  node.TreeView == null)
				return;

			if (ClassTreeContainsAssets(node.FullPath))
			{
				// can't remove it because it's got children
				MOG_Prompt.PromptResponse("Unable to Remove Classification", "This Classification cannot be removed becuase it or its Subclassifications contain Assets.\nThese Assets must be removed before the Classification can be deleted.\nYou can use the 'Generate Report' feature in the MOG Client to remove this Classification's Assets.", "", MOGPromptButtons.OK, MOG_ALERT_LEVEL.CRITICAL);
				return;
			}

			// make sure they know what they're doing
			if (Utils.ShowMessageBoxConfirmation("Are you sure you want to remove this Classification and all its Subclassifications (if any)?", "Confirm Classification Removal") != MOGPromptResult.Yes)
				return;

			if (this.immediateMode)
			{
				// kill it no matter what
				MOG_ControllerProject.GetProject().ClassificationRemove(node.FullPath);
				node.Remove();
				return;
			}


			if (node.IsNewClass)
			{
				// this is a new node, so it hasn't been saved to the DB yet -- so just remove its node and that's that
				node.Remove();
				if (this.newClasses.Contains(node))
					this.newClasses.Remove(node);
			}
			else
			{
				// add it to the remove list so we'll remove it from the databse/INIs
				node.props.Classification = node.FullPath;
				this.removedClasses.Add( node );
				node.Remove();
			}
		}

		private void ShowConfiguration()
		{
			this.panel1.Visible = true;
			if (this.oldWidth > 0  &&  this.oldHeight > 0)
			{
				this.Width = this.oldWidth;
				this.Height = this.oldHeight;

				this.oldWidth = -1;
				this.oldHeight = -1;
			}
		}

		private void HideConfiguration()
		{
			this.oldWidth = this.Width;
			this.oldHeight = this.Height;

			this.Width -= this.panel1.Width;

			this.panel1.Visible = false;
		}

		private int CountTildes(string searchString)
		{
			int count = 0;

			foreach (char c in searchString.ToCharArray())
			{
				if (c == '~')
					++count;
			}

			return count;
		}

		private string GetParentClassName(string fullClassName)
		{
			if (fullClassName.IndexOf("~") == -1)
				return "";			// has no parent, so return empty string
			else
				return fullClassName.Substring( 0, fullClassName.LastIndexOf("~") );	// return everything before the last ~
		}

		private string GetLeafClassName(string fullClassName)
		{
			if (fullClassName.IndexOf("~") == -1)
				return fullClassName;		// this is a leaf (or a root), so return the full name
			else
				return fullClassName.Substring( fullClassName.LastIndexOf("~")+1 );		// return everything after the last ~
		}

		private ArrayList BuildClassTreeFromClassNames(ArrayList classNames)
		{
			ArrayList classNodes = new ArrayList();

			if (classNames == null)
				return classNodes;

			// now, sort them into ArrayLists based on how many tildes (~) they have
			int numTildes = 0;		// start with no tildes (i.e., the root classifications)
			ArrayList classLists = new ArrayList();
			while (classNames.Count > 0)
			{
				ArrayList classList = new ArrayList();
				foreach (string className in classNames)
				{
					if (CountTildes(className) == numTildes)
						classList.Add(className);
				}

				// now, remove all the class names with 'numTildes' tildes from allClassNames
				foreach (string className in classList)
					classNames.Remove(className);

				// add classList to our list of lists
				classLists.Add(classList);

				++numTildes;
			}


			// okay, now we have an ArrayList of ArrayLists, each of which contains the class names with the number of blah blah blah
			util_MapList nodeMap = new util_MapList();
			for (int i = 0; i < classLists.Count; i++)
			{
				ArrayList classList = (ArrayList)classLists[i];
				foreach (string className in classList)
				{
					if (i == 0)
					{
						// root nodes are a special case
						classTreeNode rootNode = new classTreeNode(className);
						rootNode.ImageIndex = BLUEDOT_INDEX;
						rootNode.SelectedImageIndex = BLUEDOT_INDEX;

						rootNode.ForeColor = Color.Red;
						nodeMap.Add(className.ToLower(), rootNode);		// add so we can look up the node later by its classification
						classNodes.Add(rootNode);				// add it to return value
					}
					else
					{
						// try to look up parent node
						string parentClassName = GetParentClassName(className);
						string classLeafName = GetLeafClassName(className);

						classTreeNode parentNode = nodeMap.Get( parentClassName.ToLower() ) as classTreeNode;

						if (parentNode != null)
						{
							// if we found a parent node, add a new node representing className to it, otherwise ignore
							classTreeNode classNode = new classTreeNode(classLeafName);
							classNode.ImageIndex = ARROW_INDEX;
							classNode.SelectedImageIndex = ARROW_INDEX;
							classNode.ForeColor = Color.Red;
							parentNode.Nodes.Add(classNode);
							nodeMap.Add(className.ToLower(), classNode);		// add for lookup later (to add children to this node)
						}
					}
				}
			}
			
			return classNodes;
		}

		private ArrayList BuildClassificationTree(MOG_Project proj)
		{
			ArrayList nodeList = new ArrayList();
			foreach (string rootName in proj.GetRootClassificationNames())
			{
				classTreeNode rootNode = BuildClassificationTree_Helper(rootName, proj);
				nodeList.Add( rootNode );
			}

			return nodeList;
		}

		private classTreeNode BuildClassificationTree_Helper(string className, MOG_Project proj)
		{
			// create a new class treenode
			classTreeNode classNode = new classTreeNode( className.Substring(className.LastIndexOf("~")+1) );	// extract only last part of the name

			foreach (string subclassName in proj.GetSubClassificationNames(className))
				classNode.Nodes.Add( BuildClassificationTree_Helper(className + "~" + subclassName, proj) );

			return classNode;
		}

		// extract all classifications and subclassifications in proj and put them into an ArrayList of TreeNodes
		private ArrayList LookupClassifications()
		{
			ArrayList classNames = new ArrayList();
			foreach (string parentName in MOG.DATABASE.MOG_DBAssetAPI.GetClassificationChildren("", "Current"))
				 classNames.Add( parentName );//MOG.DATABASE.MOG_DBAssetAPI.GetClassificationChildren(parentName, null) );

			return BuildClassTreeFromClassNames(classNames);
		}

		private void LoadClassTreeConfigs(TreeNodeCollection nodes)
		{
			if (nodes == null)
				return;

			foreach (classTreeNode tn in nodes)
			{
				// load the properties for tn
				tn.props = MOG_ControllerProject.GetClassificationProperties(tn.FullPath);
				tn.props.SetImmeadiateMode(true);

//				tn.props = new MOG_Properties(tn.FullPath);

				// recurse on tn's children
				LoadClassTreeConfigs(tn.Nodes);
			}
		}

		private void LoadClassTreeConfigs_old(TreeNodeCollection nodes, MOG_Project proj)
		{
			if (nodes == null  ||  proj == null)
				return;

			foreach (classTreeNode tn in nodes)
			{
				MOG_Properties props = proj.GetClassificationProperties(tn.FullPath);
				props.SetImmeadiateMode(true);
				tn.props = props;

				// set icon
				string iconFilename = proj.GetProjectToolsPath() + "\\" + tn.props.ClassIcon;
				if (File.Exists(iconFilename))
				{
					if (!this.imageArrayList.Contains(iconFilename.ToLower()))
					{
						// this image hasn't been loaded before
						Image img = Image.FromFile(iconFilename);
						this.imageList.Images.Add(img);
						this.imageArrayList.Add(iconFilename.ToLower());
						//tn.ImageIndex = this.imageArrayList.Count - 1;
						//tn.SelectedImageIndex = this.imageArrayList.Count - 1;
					}
					else
					{
						// this image has already been loaded
						//tn.ImageIndex = this.imageArrayList.IndexOf(iconFilename.ToLower());
						//tn.SelectedImageIndex = this.imageArrayList.IndexOf(iconFilename.ToLower());
					}

				}

				LoadClassTreeConfigs(tn.Nodes);
			}
		}

		private void SaveConfigTree(TreeNodeCollection nodes, MOG_Project proj)
		{
			if (nodes == null  ||  proj == null)
				return;

			foreach (classTreeNode tn in nodes)
			{
				if (!tn.IsAssetFilenameNode)
				{

					//tn.props.SetClassification(tn.FullPath);
					//proj.ClassificationUpdate(tn.props);
				
					SaveConfigTree(tn.Nodes, proj);
				}
			}
		}

		private ArrayList BuildAllClassNodesList(TreeNodeCollection nodes)
		{
			ArrayList nodeList = new ArrayList();
			if (nodes == null)
				return nodeList;

			foreach(classTreeNode tn in nodes)
			{
				nodeList.Add(tn);

				// add child trees
				nodeList.AddRange( BuildAllClassNodesList(tn.Nodes) );
			}

			return nodeList;
		}

		#endregion
		#region Public functions
		public classTreeNode GetNodeFromFullName(string fullName)
		{
			return GetNodeFromFullName(this.tvClassifications.Nodes, fullName);
		}

		private classTreeNode GetNodeFromFullName(TreeNodeCollection nodes, string fullName)
		{
			if (nodes == null  ||  fullName == null)
				return null;

			string nextNodeName = fullName;
			if (fullName.IndexOf("~") != -1)
				nextNodeName = fullName.Split("~".ToCharArray())[0];

			foreach (classTreeNode tn in nodes)
			{
				if (tn.Text.ToLower() == nextNodeName.ToLower())
				{
					if (fullName.IndexOf("~") == -1)
						return tn;		// we're done
					else
						return GetNodeFromFullName(tn.Nodes, fullName.Substring( fullName.IndexOf("~")+1 ));	// keep looking
				}
			}

			return null;		// not found
		}

		public void CopyIcons(string imagesDir)
		{
			foreach (classTreeNode tn in this.tvClassifications.Nodes)
				CopyIcons(tn, imagesDir);
		}
		
		public void CopyIcons(classTreeNode tn, string imagesDir)
		{
			// Don't bother unless we have props?
			if (tn.props != null)
			{
				if (!File.Exists(imagesDir + "\\" + Path.GetFileName(tn.props.ClassIcon)))
				{
					if (File.Exists(tn.props.ClassIcon))
					{
						File.Copy(tn.props.ClassIcon, imagesDir + "\\" + Path.GetFileName(tn.props.ClassIcon));
						tn.props.ClassIcon = Path.GetFileName(imagesDir) + "\\" + Path.GetFileName(tn.props.ClassIcon);
					}
					else
						tn.props.ClassIcon = "NONE";
				}
				else
					tn.props.ClassIcon = Path.GetFileName(imagesDir) + "\\" + Path.GetFileName(tn.props.ClassIcon);
			}

			foreach (classTreeNode subNode in tn.Nodes)
				CopyIcons(subNode, imagesDir);
		}

		#region NEW CLASS LOADING CODE
		
		private void tvClassifications_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			classTreeNode ctn = e.Node as classTreeNode;
			if (ctn != null  &&  !ctn.FilledIn)
			{
				Cursor.Current = Cursors.WaitCursor;
				PopulateClassNode(ctn);
				Cursor.Current = Cursors.Default;
			}
		}

		private void tvClassifications_AfterSelect2(object sender, TreeViewEventArgs e)
		{
			classTreeNode newSelectedNode = e.Node as classTreeNode;
			PopulateClassNodeProps(newSelectedNode);
		}

		private void tvClassifications_MouseDown2(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			classTreeNode newSelectedNode = this.tvClassifications.GetNodeAt(e.X, e.Y) as classTreeNode;
			if (newSelectedNode == null)
				return;

			this.tvClassifications.SelectedNodes.Clear();
			this.tvClassifications.SelectedNodes.Add(newSelectedNode);

			PopulateClassNodeProps(newSelectedNode);
		}

		private void PopulateClassNodeProps(classTreeNode ctn)
		{
			if (ctn != null)
			{
				// Get the classification's properties that we can edit
				ctn.props = MOG_Properties.OpenClassificationProperties(ctn.FullPath);
				ctn.props.SetImmeadiateMode(true);
				
				this.propertyGrid.SelectedObject = ctn.props;
// MIGRATION: whatever
//                if (ctn.IsNewClass)
//					ctn = ctn;
			}
		}

		private void PopulateClassNode(classTreeNode ctn)
		{
			if (!ctn.FilledIn)
			{
				// fill it in
				
				// get rid of dummy node
				ctn.Nodes.Clear();

				// fill in children
				foreach (string subClassName in MOG_ControllerProject.GetProject().GetSubClassificationNames(ctn.FullPath))
				{
					// create subnode and add it to the parent node
					classTreeNode subNode = new classTreeNode(subClassName);
					ctn.Nodes.Add(subNode);
					
					// add dummy if needs be
					ArrayList subClassNames = MOG_ControllerProject.GetProject().GetSubClassificationNames(subNode.FullPath);
					if (subClassNames != null  &&  subClassNames.Count > 0)
						subNode.Nodes.Add( new classTreeNode("DUMMY_NODE") );
				}

				ctn.FilledIn = true;
			}
		}

		public void LoadProjectClassifications2()
		{
			LoadProjectClassifications2(MOG_ControllerProject.GetProject());
		}

		public void LoadProjectClassifications2(MOG_Project proj)
		{
			if (proj != null)
			{
				this.tvClassifications.Nodes.Clear();
				this.newClasses.Clear();

				this.imageList = new ImageList();
				this.imageArrayList = new ArrayList();

				this.tvClassifications.Enabled = true;


				foreach (string rootClassName in proj.GetRootClassificationNames())
				{
					classTreeNode rootClassNode = new classTreeNode(rootClassName);
					if (proj.GetSubClassificationNames(rootClassName).Count > 0)
						rootClassNode.Nodes.Add( new classTreeNode("DUMMY_NODE") );

					this.tvClassifications.Nodes.Add(rootClassNode);
				}

				// setup events
				this.tvClassifications.MouseDown -= new MouseEventHandler(tvClassifications_MouseDown);
				this.tvClassifications.MouseDown += new MouseEventHandler(tvClassifications_MouseDown2);
				this.tvClassifications.AfterSelect += new TreeViewEventHandler(tvClassifications_AfterSelect2);
				this.tvClassifications.BeforeExpand += new TreeViewCancelEventHandler(tvClassifications_BeforeExpand);

				// expand the first level of nodes
				foreach (TreeNode tn in this.tvClassifications.Nodes)
					tn.Expand();
			}
			else
			{
				// disable everything
				this.tvClassifications.Nodes.Add(new classTreeNode("<NO CURRENT PROJECT>"));
				this.tvClassifications.Enabled = false;
			}
		}

		#endregion
		
		public void LoadProjectClassifications()
		{
			LoadProjectClassifications(MOG_ControllerProject.GetProject());
		}

		public void LoadProjectClassifications(MOG_Project proj)
		{
			this.tvClassifications.Nodes.Clear();
			this.newClasses.Clear();

			this.imageList = new ImageList();
			this.imageArrayList = new ArrayList();

			if (proj != null)
			{
				this.tvClassifications.Enabled = true;

				ArrayList classNodes = BuildClassificationTree(proj);
				foreach (classTreeNode tn in classNodes)
					this.tvClassifications.Nodes.Add(tn);

//				MogUtil_ClassificationLoader classLoader = new MogUtil_ClassificationLoader(MOG_ControllerProject.GetProjectName());
//				ArrayList classes = classLoader.GetClassFullNamesListFromFiles();
//				foreach (classTreeNode tn in BuildClassTreeFromClassNames(classes))
//					this.tvClassifications.Nodes.Add(tn);
				
				//LoadClassTreeConfigs(this.tvClassifications.Nodes);

				// expand the first level of nodes
				foreach (TreeNode tn in this.tvClassifications.Nodes)
					tn.Expand();
			}
			else
			{
				// disable everything
				this.tvClassifications.Nodes.Add(new classTreeNode("<NO CURRENT PROJECT>"));
				this.tvClassifications.Enabled = false;
			}
		}


		public void CreateAll()
		{
			MOG_Project proj = MOG_ControllerProject.GetProject();
			if (proj != null)
			{
				foreach (classTreeNode tn in BuildAllClassNodesList(this.tvClassifications.Nodes))
				{
					if (!tn.IsAssetFilenameNode)
					{
						proj.ClassificationAdd( tn.FullPath );
						MOG_Properties props = new MOG_Properties(tn.FullPath);
						props.SetProperties(tn.props.GetPropertyList());
					}
				}
			}
		}

		public void CreateAllExtantClasses()
		{
			foreach (classTreeNode ctn in this.tvClassifications.Nodes)
				CreateAllExtantClasses_Helper(MOG_ControllerProject.GetProject(), ctn);
		}

		private void CreateAllExtantClasses_Helper(MOG_Project proj, classTreeNode ctn)
		{
			if (ctn == null  ||  ctn.TreeView == null)
				return;
			
			string fullClass = ctn.FullPath.Replace( ctn.TreeView.PathSeparator, "~" );
			proj.ClassificationAdd(fullClass);

			foreach (classTreeNode subNode in ctn.Nodes)
				CreateAllExtantClasses_Helper(proj, subNode);
		}

		public void SaveChanges()
		{
			MOG_Project proj = MOG_ControllerProject.GetProject();
			if (proj != null)
			{
				// removed nodes
				foreach (classTreeNode tn in this.removedClasses)
				{
					proj.ClassificationRemove(tn.props.Classification);
				}
				this.removedClasses.Clear();

				// new nodes
				foreach (classTreeNode tn in this.newClasses)
				{
					if (!tn.IsAssetFilenameNode)
					{
						tn.props.Classification = tn.FullPath;
						proj.ClassificationAdd( tn.FullPath );
					}
				}
				this.newClasses.Clear();

				SaveConfigTree(this.tvClassifications.Nodes, proj);
			}
		}

		/// <summary>
		/// Load all the default classifications from the setup or internal L: drive and populate our tree
		/// </summary>
		public void LoadDefaultClassifications()
		{
			this.tvClassifications.Nodes.Clear();
			this.newClasses.Clear();

			this.imageList = null;
			this.imageArrayList = null;

//			MogUtil_ClassificationLoader classLoader = new MogUtil_ClassificationLoader("Project");
//			ArrayList classes = classLoader.GetClassFullNamesListFromFiles();
//			foreach (classTreeNode tn in BuildClassTreeFromClassNames(classes))
//				this.tvClassifications.Nodes.Add(tn);
		}

		public void LoadDefaultClassifications(string projectName)
		{
			this.tvClassifications.Nodes.Clear();
			this.newClasses.Clear();

			this.imageList = null;
			this.imageArrayList = null;
//
//			MogUtil_ClassificationLoader classLoader = new MogUtil_ClassificationLoader(projectName);
//			foreach (classTreeNode tn in BuildClassTreeFromClassNames(classLoader.GetClassFullNamesListFromFiles()))
//				this.tvClassifications.Nodes.Add(tn);
		}
		#endregion
		#region Event handlers
		private void miAddClassification_Click(object sender, System.EventArgs e)
		{
			if (this.tvClassifications.SelectedNodes.Count > 0)
				AddClassificationNode( this.tvClassifications.SelectedNodes[0] as classTreeNode );
			else
				AddClassificationNode( null );
		}

		private void miRemove_Click(object sender, System.EventArgs e)
		{
			if (this.tvClassifications.SelectedNodes.Count > 0)
				RemoveClassificationNode( this.tvClassifications.SelectedNodes[0] as classTreeNode );
		}

		private void miShowConfiguration_Click(object sender, System.EventArgs e)
		{
			// toggle
			this.miShowConfiguration.Checked = !this.miShowConfiguration.Checked;
			
			// and display change
			if (this.miShowConfiguration.Checked)
				ShowConfiguration();
			else
				HideConfiguration();
		}

		private void tvClassifications_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			classTreeNode newSelectedNode = this.tvClassifications.GetNodeAt(e.X, e.Y) as classTreeNode;
			this.tvClassifications.SelectedNodes.Clear();
			this.tvClassifications.SelectedNodes.Add(newSelectedNode);

			if (newSelectedNode != null)
			{
				this.propertyGrid.SelectedObject = newSelectedNode.props;
				//if (newSelectedNode.IsNewClass)
				//	newSelectedNode = newSelectedNode;
			}
			else
			{
				MOG_Prompt.PromptResponse("", "Selected node has no properties object");
			}

			//if (this.tvClassifications.SelectedNodes[0] != null  &&  this.tvClassifications.SelectedNodes[0] is classTreeNode)
			//	this.propertyGrid.SelectedObject = ((classTreeNode)this.tvClassifications.SelectedNodes[0]).props;		
		}

		private void tvClassifications_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			TreeNodeCollection nodes = this.tvClassifications.Nodes;
			if (e.Node.Parent != null)
				nodes = e.Node.Parent.Nodes;

			if (e.Label == ""  ||  Utils.NodesCollectionContainsSubNode(nodes, e.Label))
			{
				MOG_Prompt.PromptResponse("Alert", "Invalid Classification Name", "", MOGPromptButtons.OK, MOG_ALERT_LEVEL.ALERT);
				e.CancelEdit = true;
			}

			if (this.immediateMode)
			{
				string className = e.Node.FullPath.Substring(0, e.Node.FullPath.LastIndexOf("~")) + "~" + e.Label;
				if (e.Label == null)
					className = e.Node.FullPath;

				if (MOG_ControllerProject.GetProject().ClassificationAdd(className))
				{
					((classTreeNode)e.Node).props = MOG_ControllerProject.GetClassificationProperties(className);
					((classTreeNode)e.Node).props.SetImmeadiateMode(true);
				}
				else
				{
					e.CancelEdit = true;
				}
			}

			this.tvClassifications.LabelEdit = false;
		}

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			this.LoadDefaultClassifications();
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			this.LoadProjectClassifications();
		}

		private void menuItem3_Click(object sender, System.EventArgs e)
		{
			SaveChanges();
		}

		private void tvClassifications_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.F2  &&  this.tvClassifications.SelectedNodes[0] != null  &&  this.tvClassifications.SelectedNodes[0] is classTreeNode  &&  ((classTreeNode)this.tvClassifications.SelectedNodes[0]).IsNewClass)
			{
				this.tvClassifications.LabelEdit = true;
				this.tvClassifications.SelectedNodes[0].BeginEdit();
			}
// JohnRen - This doesn't rebuild the list correctly when we are importing a project
//			else if (e.KeyData == Keys.F5)
//				this.LoadProjectClassifications();
			else if (e.KeyData == Keys.Delete)
				RemoveSelectedClassNodes();
		}

		private void tvClassifications_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			classTreeNode newSelectedNode = e.Node as classTreeNode;

			if (newSelectedNode != null)
			{
				this.propertyGrid.SelectedObject = newSelectedNode.props;
// MIGRATION: why would you want to do this?
//				if (newSelectedNode.IsNewClass)
//					newSelectedNode = newSelectedNode;
			}
		}
	}
		#endregion


	#region Helpers
	public class classTreeNode : TreeNode
	{
		public MOG_Properties props = null;
		public bool IsNewClass = false;
		
		public bool IsAssetFilenameNode = false;
		public ArrayList importFiles = new ArrayList();

		public bool FilledIn = false;
		public AssetTreeNode assetTreeNode = null;

		public classTreeNode(string text) : base(text)
		{
		}
	}
	#endregion
}


