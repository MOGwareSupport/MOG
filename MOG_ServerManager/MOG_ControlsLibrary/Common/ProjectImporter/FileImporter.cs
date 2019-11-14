using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG.PROJECT;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for FileImporter.
	/// </summary>
	public class FileImporter : System.Windows.Forms.Form
	{
		#region System defs
		private CodersLab.Windows.Controls.TreeView tvTree;
		private CodersLab.Windows.Controls.TreeView tvClasses;
		private System.Windows.Forms.Button btnBack;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Button btnCreateDir;
		private System.Windows.Forms.Button btnDeleteDir;
		private System.Windows.Forms.Label lblPath;
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
			this.tvTree = new CodersLab.Windows.Controls.TreeView();
			this.tvClasses = new CodersLab.Windows.Controls.TreeView();
			this.btnBack = new System.Windows.Forms.Button();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnCreateDir = new System.Windows.Forms.Button();
			this.btnDeleteDir = new System.Windows.Forms.Button();
			this.lblPath = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tvTree
			// 
			this.tvTree.HideSelection = false;
			this.tvTree.ImageIndex = -1;
			this.tvTree.Location = new System.Drawing.Point(8, 56);
			this.tvTree.Name = "tvTree";
			this.tvTree.SelectedImageIndex = -1;
			this.tvTree.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			this.tvTree.SelectionMode = CodersLab.Windows.Controls.TreeViewSelectionMode.MultiSelect;
			this.tvTree.Size = new System.Drawing.Size(312, 312);
			this.tvTree.TabIndex = 0;
			this.tvTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvTree_BeforeExpand);
			this.tvTree.DoubleClick += new System.EventHandler(this.tvTree_DoubleClick);
			this.tvTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvTree_ItemDrag);
			// 
			// tvClasses
			// 
			this.tvClasses.AllowDrop = true;
			this.tvClasses.HideSelection = false;
			this.tvClasses.ImageIndex = -1;
			this.tvClasses.Location = new System.Drawing.Point(336, 16);
			this.tvClasses.Name = "tvClasses";
			this.tvClasses.PathSeparator = "~";
			this.tvClasses.SelectedImageIndex = -1;
			this.tvClasses.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			this.tvClasses.SelectionMode = CodersLab.Windows.Controls.TreeViewSelectionMode.MultiSelect;
			this.tvClasses.Size = new System.Drawing.Size(328, 352);
			this.tvClasses.TabIndex = 1;
			this.tvClasses.DragOver += new System.Windows.Forms.DragEventHandler(this.tvClasses_DragOver);
			this.tvClasses.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvClasses_DragDrop);
			this.tvClasses.DragEnter += new System.Windows.Forms.DragEventHandler(this.tvClasses_DragEnter);
			// 
			// btnBack
			// 
			this.btnBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnBack.Location = new System.Drawing.Point(24, 8);
			this.btnBack.Name = "btnBack";
			this.btnBack.Size = new System.Drawing.Size(24, 23);
			this.btnBack.TabIndex = 2;
			this.btnBack.Text = "<";
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			// 
			// btnUp
			// 
			this.btnUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnUp.Location = new System.Drawing.Point(48, 8);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(24, 23);
			this.btnUp.TabIndex = 3;
			this.btnUp.Text = "^";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnCreateDir
			// 
			this.btnCreateDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnCreateDir.Location = new System.Drawing.Point(104, 8);
			this.btnCreateDir.Name = "btnCreateDir";
			this.btnCreateDir.Size = new System.Drawing.Size(24, 23);
			this.btnCreateDir.TabIndex = 4;
			this.btnCreateDir.Text = "*";
			// 
			// btnDeleteDir
			// 
			this.btnDeleteDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnDeleteDir.Location = new System.Drawing.Point(128, 8);
			this.btnDeleteDir.Name = "btnDeleteDir";
			this.btnDeleteDir.Size = new System.Drawing.Size(24, 23);
			this.btnDeleteDir.TabIndex = 5;
			this.btnDeleteDir.Text = "X";
			// 
			// lblPath
			// 
			this.lblPath.Location = new System.Drawing.Point(8, 40);
			this.lblPath.Name = "lblPath";
			this.lblPath.Size = new System.Drawing.Size(312, 16);
			this.lblPath.TabIndex = 6;
			// 
			// FileImporter
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(680, 382);
			this.Controls.Add(this.lblPath);
			this.Controls.Add(this.btnDeleteDir);
			this.Controls.Add(this.btnCreateDir);
			this.Controls.Add(this.btnUp);
			this.Controls.Add(this.btnBack);
			this.Controls.Add(this.tvClasses);
			this.Controls.Add(this.tvTree);
			this.Name = "FileImporter";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "FileImporter";
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Member vars
		private string loadedDir = "";
		private Stack dirHistory = new Stack();
		private ArrayList placedPaths = new ArrayList();
		#endregion
		#region Constructor
		public FileImporter()
		{
			InitializeComponent();

			LoadTree("C:\\");
		}

		#endregion
		#region Member functions
		#region Class loading
		public void LoadProjectClasses(MOG_Project proj)
		{
			this.tvClasses.Nodes.Clear();

			foreach(string rootClassName in proj.GetRootClassificationNames())
			{
				imTreeNode rootNode = LoadProjectClasses_Helper(proj, rootClassName);
				this.tvClasses.Nodes.Add( rootNode );
				rootNode.Expand();
			}
		}
		
		private imTreeNode LoadProjectClasses_Helper(MOG_Project proj, string baseClass)
		{
			string baseClassName = baseClass;
			if (baseClassName.IndexOf("~") != -1)
				baseClassName = baseClass.Substring( baseClass.LastIndexOf("~")+1 );

			imTreeNode node = new imTreeNode(baseClassName, "class");

			foreach (string className in proj.GetSubClassificationNames(baseClass))
				node.Nodes.Add(LoadProjectClasses_Helper(proj, baseClass + "~" + className));

			return node;
		}

		#endregion
		#region Tree loading
		private void LoadTree(string path)
		{
			if (!Directory.Exists(path))
				return;

			this.tvTree.Nodes.Clear();

			foreach (imTreeNode tn in LoadTree_Helper(path))
				this.tvTree.Nodes.Add(tn);

			this.loadedDir = path;
			this.lblPath.Text = path;
			
			if (this.loadedDir != "")
				this.dirHistory.Push(this.loadedDir);
		}

		private ArrayList LoadTree_Helper(string path)
		{
			ArrayList nodes = new ArrayList();
			if (!Directory.Exists(path))
				return nodes;

			foreach (string dirname in Directory.GetDirectories(path))
			{
				imTreeNode dirNode = new imTreeNode(Path.GetFileName(dirname), "dir");
				dirNode.Path = dirname;

				try
				{
					if (Directory.GetDirectories(dirname).Length > 0  ||  Directory.GetFiles(dirname).Length > 0)
						dirNode.Nodes.Add(new imTreeNode("DUMMYNODE", "dummy"));
				}
				catch
				{
					dirNode.WindowsProtected = true;
					dirNode.ForeColor = Color.Red;
				}

				nodes.Add(dirNode);
			}

			foreach (string filename in Directory.GetFiles(path))
			{
				imTreeNode fileNode = new imTreeNode(Path.GetFileName(filename), "file");
				fileNode.Path = filename;
				nodes.Add(fileNode);
			}

			return nodes;
		}
		#endregion
		#region Node population
		private void PopulateDiskNode_Complete(imTreeNode tn)
		{
			if (!tn.Populated)
				PopulateDiskNode_Shallow(tn);

			foreach (imTreeNode subNode in tn.Nodes)
			{
				if (subNode.IsDirectory)
					PopulateDiskNode_Complete(subNode);
			}
		}

		private void PopulateDiskNode_Shallow(imTreeNode tn)
		{
			if (!tn.Populated)
			{
				tn.Nodes.Clear();		// kill dummy node
				foreach (imTreeNode subNode in LoadTree_Helper(tn.Path))
					tn.Nodes.Add(subNode);
				tn.Populated = true;
			}
		}
		#endregion
		#region Node creation
		private imTreeNode CreateFileNode(imTreeNode originalNode)
		{
			imTreeNode fileNode = new imTreeNode(originalNode.Text, "subasset");
			return fileNode;
		}
		private imTreeNode CreateFileNode(string text)
		{
			imTreeNode fileNode = new imTreeNode(text, "subasset");
			return fileNode;
		}

		private imTreeNode CreateAssetNode(imTreeNode originalNode)
		{
			imTreeNode assetNode = new imTreeNode("{All}"+originalNode.Text, "asset");
			assetNode.Nodes.Add(CreateFileNode(originalNode));
			return assetNode;
		}
		private imTreeNode CreateAssetNode(string text)
		{
			imTreeNode assetNode = new imTreeNode("{All}"+text, "asset");
			assetNode.Nodes.Add(CreateFileNode(text));
			return assetNode;
		}
		
		private imTreeNode CreateClassNode(imTreeNode originalNode)
		{
			imTreeNode classNode = new imTreeNode(originalNode.Text, "class");
			return classNode;
		}
		private imTreeNode CreateClassNode(string text)
		{
			imTreeNode classNode = new imTreeNode(text, "class");
			return classNode;
		}

		#endregion
		#region Node insertion
		private void InsertPathList(ArrayList paths, imTreeNode targetNode)
		{
			foreach (string path in paths)
			{
				if (Directory.Exists(path))
				{
					// path is a directory, create classification
					InsertPathAsClassTree(targetNode, path);
				}
				else if (File.Exists(path))
				{
					// path is a file, create asset
					targetNode.Nodes.Add(CreateAssetNode( Path.GetFileName(path) ));
				}
			}

			targetNode.Expand();
		}

		private void InsertPathAsClassTree(imTreeNode targetNode, string path)
		{
			imTreeNode classNode = CreateClassNode( Path.GetFileName(path) );
			foreach (string subdirpath in Directory.GetDirectories(path))
			{
				InsertPathAsClassTree(classNode, subdirpath);
			}

			foreach (string filename in Directory.GetFiles(path))
			{
				imTreeNode fileNode = CreateAssetNode( Path.GetFileName(filename) );
				classNode.Nodes.Add(fileNode);
			}
			
			targetNode.Nodes.Add(classNode);
		}

		private void InsertNodeList(ArrayList nodes, imTreeNode targetNode)
		{
			foreach (imTreeNode draggedNode in nodes)
			{
				if (draggedNode.WindowsProtected)
					continue;		// skip windows-protected stuff

				// make sure the node is fully populated (i.e., no dummy nodes)
				PopulateDiskNode_Complete(draggedNode);

				if (draggedNode.IsDirectory)
				{
					// create classification
					InsertDirTreeAsClassTree(targetNode, draggedNode);
				}
				else if (draggedNode.IsFile)
				{
					// create asset
					targetNode.Nodes.Add(CreateAssetNode(draggedNode));
					targetNode.Expand();
				}
			}
		}

		private void InsertDirTreeAsClassTree(imTreeNode targetNode, imTreeNode draggedNode)
		{
			if (targetNode == null  ||  draggedNode == null)
				return;

			imTreeNode classNode = CloneDirTreeAsClassTree(draggedNode);
			targetNode.Nodes.Add( classNode );
			targetNode.Expand();
		}

		private imTreeNode CloneDirTreeAsClassTree(imTreeNode dirNode)
		{
			imTreeNode classNode = CreateClassNode(dirNode);
			foreach (imTreeNode subNode in dirNode.Nodes)
			{
				if (subNode.IsFile)
				{
					classNode.Nodes.Add(CreateAssetNode(subNode));
				}
				else if (subNode.IsDirectory)
				{
					classNode.Nodes.Add(CloneDirTreeAsClassTree(subNode));
				}
			}

			return classNode;
		}


		#endregion
		#endregion
		#region Event handlers
		#region Disk tree view
		#region Drag-drop
		private void tvTree_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{
			ArrayList nodes = new ArrayList();
			foreach (imTreeNode tn in this.tvTree.SelectedNodes)
				nodes.Add(tn);
			this.tvTree.DoDragDrop(nodes, DragDropEffects.Link);
		}

		#endregion
		private void tvTree_DoubleClick(object sender, System.EventArgs e)
		{
			imTreeNode clickedNode = this.tvTree.GetNodeAt( this.tvTree.PointToClient(Cursor.Position) ) as imTreeNode;
			if (clickedNode != null  &&  clickedNode.IsDirectory)
			{
				LoadTree(this.loadedDir + "\\" + clickedNode.Text);
			}
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			LoadTree(Path.GetDirectoryName(this.loadedDir));
		}

		private void btnBack_Click(object sender, System.EventArgs e)
		{
			if (this.dirHistory.Count > 0)
			{
				LoadTree((string)this.dirHistory.Pop());
				this.dirHistory.Pop();
			}
		}

		private void tvTree_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			imTreeNode tn = e.Node as imTreeNode;
			if (tn != null)
			{
				if (tn.IsDirectory  &&  !tn.Populated)
				{
                    PopulateDiskNode_Shallow(tn);
				}
			}
		}
		#endregion
		#region Class tree view
		#region Drag-drop

		private void tvClasses_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(ArrayList)) ||  e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = e.AllowedEffect;
		}

		private void tvClasses_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			imTreeNode targetNode = this.tvClasses.GetNodeAt(this.tvClasses.PointToClient(new Point(e.X, e.Y))) as imTreeNode;
			if (targetNode == null)
			{
				e.Effect = DragDropEffects.None;
				this.tvClasses.SelectedNodes.Clear();
			}
			else
			{
				e.Effect = e.AllowedEffect;
				this.tvClasses.SelectedNodes.Clear();
				this.tvClasses.SelectedNodes.Add(targetNode);
			}
		}

		private void tvClasses_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			imTreeNode targetNode = this.tvClasses.GetNodeAt(this.tvClasses.PointToClient(new Point(e.X, e.Y))) as imTreeNode;
			if (targetNode == null)
				return;

			if (e.Data.GetDataPresent(typeof(ArrayList)))
			{
				ArrayList nodes = (ArrayList)e.Data.GetData(typeof(ArrayList));
				nodes = GetUniqueClassRoots(nodes);

				InsertNodeList(nodes, targetNode);
			}
			else if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData("FileDrop", false);
				InsertPathList( GetUniquePathRoots(new ArrayList(files)), targetNode );
			}
		}

		private ArrayList GetUniquePathRoots(ArrayList paths)
		{
			ArrayList uniqueRoots = new ArrayList();
			if (paths == null)
				return uniqueRoots;

			foreach (string path in paths)
			{
				string root = path;
				string parentPath = Path.GetDirectoryName(root);
				while (Directory.Exists(parentPath)  &&  paths.IndexOf(parentPath) != -1)
				{
					root = parentPath;
					parentPath = Path.GetDirectoryName(root);
				}

				if (!uniqueRoots.Contains(root))
					uniqueRoots.Add(root);
			}
			
			return uniqueRoots;
		}

		private ArrayList GetUniqueClassRoots(ArrayList nodes)
		{
			ArrayList uniqueRoots = new ArrayList();
			if (nodes == null)
				return uniqueRoots;

			foreach (imTreeNode tn in nodes)
			{
				imTreeNode root = tn;
				while (root.Parent != null  &&  nodes.Contains(root.Parent))
					root = root.Parent as imTreeNode;

				if (!uniqueRoots.Contains(root))
					uniqueRoots.Add(root);
			}
			
			return uniqueRoots;
		}

		#endregion
		#endregion 
		#endregion
	}

	class imTreeNode : TreeNode
	{
		public bool IsDirectory = false;
		public bool IsFile = false;
		public bool IsClassification = false;
		public bool IsAsset = false;
		public bool IsSubAsset = false;
		public bool IsDummy = false;

		public bool Populated = false;
		public bool WindowsProtected = false;

		public string Path = "";

		public imTreeNode(string text, string type) :base(text)
		{
			if (type.ToLower() == "dir")
			{
				this.IsDirectory = true;
				this.ForeColor = Color.Blue;
			}
			else if (type.ToLower() == "file")
			{
				this.IsFile = true;
			}
			else if (type.ToLower() == "class")
			{
				this.IsClassification = true;
				this.ForeColor = Color.Purple;
			}
			else if (type.ToLower() == "asset")
			{
				this.IsAsset = true;
				this.ForeColor = Color.Blue;
			}
			else if (type.ToLower() == "subasset")
			{
				this.IsSubAsset = true;
				this.ForeColor = Color.Salmon;
			}
			else if (type.ToLower() == "subasset")
			{
				this.IsDummy = true;
			}
		}
	}
}
