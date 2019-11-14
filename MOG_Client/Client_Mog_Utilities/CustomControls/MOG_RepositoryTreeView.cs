using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using MOG.CONTROLLER.CONTROLLERPROJECT;

using MOG_Client.Client_Mog_Utilities.AssetOptions;

using MOG;
using MOG.FILENAME;
using MOG.PROPERTIES;
using MOG.CONTROLLER.CONTROLLERSYSTEM;



namespace MOG_Client.Client_Mog_Utilities.CustomControls
{
	/// <summary>
	/// Summary description for MOG_RepositoryTreeView.
	/// </summary>
	public class MOG_RepositoryTreeView : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TreeView ProjectTreeView;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MOG_RepositoryTreeView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
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
			this.ProjectTreeView = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// ProjectTreeView
			// 
			this.ProjectTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.ProjectTreeView.ImageIndex = -1;
			this.ProjectTreeView.Location = new System.Drawing.Point(0, 8);
			this.ProjectTreeView.Name = "ProjectTreeView";
			this.ProjectTreeView.SelectedImageIndex = -1;
			this.ProjectTreeView.Size = new System.Drawing.Size(232, 224);
			this.ProjectTreeView.TabIndex = 0;
			this.ProjectTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.ProjectTreeView_BeforeExpand);
			// 
			// MOG_RepositoryTreeView
			// 
			this.Controls.Add(this.ProjectTreeView);
			this.Name = "MOG_RepositoryTreeView";
			this.Size = new System.Drawing.Size(232, 240);
			this.ResumeLayout(false);

		}
		#endregion

		public void Initialize()
		{
			// Populate tree
			PopulateTree();
		}

		private void PopulateTree()
		{
			ProjectTreeView.ImageList = guiAssetIcon.Images;
			BuildAssetTree();
		}

		private void BuildAssetTree()
		{
			TreeView tree = ProjectTreeView;

			// Clear the tree
			tree.Nodes.Clear();
			
			// Create the root and get its first round of children
			TreeNode root = new TreeNode(MOG_ControllerProject.GetProject().GetProjectName());
			root.Tag = "";
			PopulateClassificationChildren(root);
			tree.Nodes.Add(root);
			

			// Expand the current projects tree
			foreach (TreeNode node in tree.Nodes)
			{
				if (string.Compare(MOG_ControllerProject.GetProject().GetProjectKey(), node.Text, true) == 0)
				{
					ExpandTreeDown(tree, node);
					node.Expand();
				}
			}
		}

		private void PopulateClassificationChildren(TreeNode parentNode)
		{
			// Create a name for our lookup into the DB
			string classificationName = (string)parentNode.Tag;
			
			// Get a list of the root classes from the clasification database
			ArrayList classifications = MOG.DATABASE.MOG_DBAssetAPI.GetClassificationChildren(classificationName);


			//Temp
//			switch (classificationName)
//			{
//				case "":
//					classifications.Add("Textures");
//					classifications.Add("Animations");
//					classifications.Add("Models");
//					classifications.Add("Packages");
//					classifications.Add("Tools");
//					classifications.Add("Sounds");
//					classifications.Add("Music");
//					break;
//				case "Textures":
//					classifications.Add("Human");
//					classifications.Add("Seeker");
//					classifications.Add("Other");
//					break;
//				case "Animations":
//					classifications.Add("Human");
//					classifications.Add("Seeker");
//					classifications.Add("Other");
//					break;
//				case "Models":
//					classifications.Add("Edumea");
//					classifications.Add("Algothia");
//					classifications.Add("Mingolia");
//					break;
//				case "Packages":
//					classifications.Add("pak1");
//					classifications.Add("pak2");
//					classifications.Add("pak3");
//					break;
//				case "Sounds":
//					classifications.Add("Hit");
//					classifications.Add("Dialog");
//					classifications.Add("Ambient");
//					break;
//				case "Music":
//					classifications.Add("Level 1");
//					classifications.Add("Level 2");
//					classifications.Add("Level 3");
//					classifications.Add("Level 4");
//					classifications.Add("Level 5");
//					classifications.Add("Level 6");
//					classifications.Add("Level 7");
//					classifications.Add("Level 8");
//					break;
//			}

			
			// Create our root tree
			if (classifications != null)
			{
				foreach (string classification in classifications)
				{
					string subClassificationName;
					TreeNode classificationNode = new TreeNode();

					// Determine our subclass name
					if (classificationName.Length == 0)
					{
						subClassificationName = classification;
					}
					else
					{
						subClassificationName = classificationName + "~" + classification;
					}

					// Get an assetInfo object on this class
					//MOG_Properties properties = new MOG_Properties(new MOG_Filename(subClassificationName));

					classificationNode.Text = classification;
					classificationNode.ImageIndex = guiAssetIcon.SetAssetIcon(subClassificationName);
					classificationNode.Tag = subClassificationName;

					// Check for children
					if (MOG.DATABASE.MOG_DBAssetAPI.GetClassificationChildren(classification).Count != 0)
					{
						classificationNode.Nodes.Add("<BLANK>");
					}

					parentNode.Nodes.Add(classificationNode);
				}
			}
		}

		// Handle each click to expand down
		private void ExpandTreeDown(TreeView tree, TreeNode node)
		{
			// Check if this is the first time we are expanding this tree item
			if (node.Nodes.Count == 1  && node.Nodes[0].Text == "<BLANK>")
			{
				// Save our current mouse Cursor with a new pointer/object
				// Cursor oldCurrent = new Cursor(Cursor.Current.Handle);
				// Change to mouse to hourglass
				tree.Cursor = Cursors.WaitCursor;

				// Clean out the temp(s)
				node.Nodes.Clear();

				// Get the children of the node we are expanding
				PopulateClassificationChildren(node);
					
				// Change mouse cursor back to what it was
				tree.Cursor = Cursors.Default;
			} // End if on nodes count
		}

		private void ProjectTreeView_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			this.ExpandTreeDown(this.ProjectTreeView, e.Node);
		}	
	}
}
