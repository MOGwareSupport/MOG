using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using MOG.FILENAME;
using MOG.PROGRESS;
using MOG.CONTROLLER.CONTROLLERPROJECT;

using MOG_Client.Client_Mog_Utilities.AssetOptions;
using MOG_ControlsLibrary.Controls;
using MOG_ControlsLibrary.Utils;

using Etier.IconHelper;
using MOG_CoreControls;
using System.Collections.Generic;


namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for FormConfirmSpecial.
	/// This is a confirm dialog that can replace the simple MessageBox show that supports a treeView of assets to confirm
	/// </summary>
	public class FormConfirmSpecial : System.Windows.Forms.Form
	{
		public bool cancel;
		public int result;
		public string[]buttons;
		private bool mBuilding;
		private string mLabelTreeDelimiter = "\\";
		private ArrayList mSelectedItems;
		public string LabelTreeDelimiter { set {mLabelTreeDelimiter = value;} get {return mLabelTreeDelimiter;} }
		public ArrayList SelectedItems { get{return mSelectedItems;} }
		
//		private IconListManager mIconListManager;
//		private ImageList mSmallImageList = new ImageList();
//		private ImageList mLargeImageList = new ImageList();

		public System.Windows.Forms.TreeView ConfirmTreeView;
		private System.Windows.Forms.Label ConfirmLabel;
		public System.Windows.Forms.Button ConfirmOptional4Button;
		public System.Windows.Forms.Button ConfirmOptional3Button;
		public System.Windows.Forms.Button ConfirmOptional2Button;
		public System.Windows.Forms.Button ConfirmOptional1Button;

		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfirmSpecial()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			cancel = false;
			result = -1;
			mBuilding = false;

			//InitializeExplorerIcons();
			ConfirmTreeView.ImageList = MogUtil_AssetIcons.Images;

			mSelectedItems = new ArrayList();			
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormConfirmSpecial));
			this.ConfirmOptional4Button = new System.Windows.Forms.Button();
			this.ConfirmOptional3Button = new System.Windows.Forms.Button();
			this.ConfirmOptional2Button = new System.Windows.Forms.Button();
			this.ConfirmOptional1Button = new System.Windows.Forms.Button();
			this.ConfirmTreeView = new System.Windows.Forms.TreeView();
			this.ConfirmLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// ConfirmOptional4Button
			// 
			this.ConfirmOptional4Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ConfirmOptional4Button.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ConfirmOptional4Button.Location = new System.Drawing.Point(424, 240);
			this.ConfirmOptional4Button.Name = "ConfirmOptional4Button";
			this.ConfirmOptional4Button.Size = new System.Drawing.Size(68, 23);
			this.ConfirmOptional4Button.TabIndex = 12;
			this.ConfirmOptional4Button.Text = "Unknown";
			this.ConfirmOptional4Button.Visible = false;
			this.ConfirmOptional4Button.Click += new System.EventHandler(this.ConfirmOptionalButton_Click);
			// 
			// ConfirmOptional3Button
			// 
			this.ConfirmOptional3Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ConfirmOptional3Button.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ConfirmOptional3Button.Location = new System.Drawing.Point(352, 240);
			this.ConfirmOptional3Button.Name = "ConfirmOptional3Button";
			this.ConfirmOptional3Button.Size = new System.Drawing.Size(68, 23);
			this.ConfirmOptional3Button.TabIndex = 11;
			this.ConfirmOptional3Button.Text = "Unknown";
			this.ConfirmOptional3Button.Visible = false;
			this.ConfirmOptional3Button.Click += new System.EventHandler(this.ConfirmOptionalButton_Click);
			// 
			// ConfirmOptional2Button
			// 
			this.ConfirmOptional2Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ConfirmOptional2Button.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ConfirmOptional2Button.Location = new System.Drawing.Point(280, 240);
			this.ConfirmOptional2Button.Name = "ConfirmOptional2Button";
			this.ConfirmOptional2Button.Size = new System.Drawing.Size(68, 23);
			this.ConfirmOptional2Button.TabIndex = 10;
			this.ConfirmOptional2Button.Text = "Unknown";
			this.ConfirmOptional2Button.Visible = false;
			this.ConfirmOptional2Button.Click += new System.EventHandler(this.ConfirmOptionalButton_Click);
			// 
			// ConfirmOptional1Button
			// 
			this.ConfirmOptional1Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ConfirmOptional1Button.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.ConfirmOptional1Button.Location = new System.Drawing.Point(208, 240);
			this.ConfirmOptional1Button.Name = "ConfirmOptional1Button";
			this.ConfirmOptional1Button.Size = new System.Drawing.Size(68, 23);
			this.ConfirmOptional1Button.TabIndex = 9;
			this.ConfirmOptional1Button.Text = "Unknown";
			this.ConfirmOptional1Button.Visible = false;
			this.ConfirmOptional1Button.Click += new System.EventHandler(this.ConfirmOptionalButton_Click);
			// 
			// ConfirmTreeView
			// 
			this.ConfirmTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.ConfirmTreeView.CheckBoxes = true;
			this.ConfirmTreeView.HotTracking = true;
			this.ConfirmTreeView.ImageIndex = -1;
			this.ConfirmTreeView.Location = new System.Drawing.Point(8, 32);
			this.ConfirmTreeView.Name = "ConfirmTreeView";
			this.ConfirmTreeView.SelectedImageIndex = -1;
			this.ConfirmTreeView.Size = new System.Drawing.Size(528, 200);
			this.ConfirmTreeView.TabIndex = 13;
			this.ConfirmTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.ConfirmTreeView_AfterCheck);
			// 
			// ConfirmLabel
			// 
			this.ConfirmLabel.Location = new System.Drawing.Point(16, 8);
			this.ConfirmLabel.Name = "ConfirmLabel";
			this.ConfirmLabel.Size = new System.Drawing.Size(520, 23);
			this.ConfirmLabel.TabIndex = 14;
			this.ConfirmLabel.Text = "Message";
			// 
			// FormConfirmSpecial
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(544, 273);
			this.ControlBox = false;
			this.Controls.Add(this.ConfirmLabel);
			this.Controls.Add(this.ConfirmTreeView);
			this.Controls.Add(this.ConfirmOptional4Button);
			this.Controls.Add(this.ConfirmOptional3Button);
			this.Controls.Add(this.ConfirmOptional2Button);
			this.Controls.Add(this.ConfirmOptional1Button);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormConfirmSpecial";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Title";
			this.ResumeLayout(false);

		}
		#endregion

		public void DialogInitialize(string title, string message, string button)
		{
			Text = title;

			ConfirmLabel.Visible = false;
			ConfirmLabel.Text = message;

			if (button.Length != 0)
			{
				ConfirmOptional4Button.Text = button;
			}
			else
			{
				ConfirmOptional4Button.Visible = false;
			}

			ShowDialog(MogMainForm.MainApp);
		}

		public void DialogInitialize(string title, string message, string parentPath, ArrayList assets, ArrayList assetLabels, string button)
		{
			Text = title;

			ConfirmLabel.Text = message;
			
			// Check if the button string needs to be parsed
			if (button.IndexOf("/") != -1)
			{
				string []buttons = button.Split(new Char[] {'/'});
				this.buttons = buttons;

				for (int i = 0; i < buttons.Length; i++)
				{
					switch (i)
					{
						case 0:
							ConfirmOptional1Button.Text = buttons[i];
							ConfirmOptional1Button.Visible = true;
							break;
						case 1:
							ConfirmOptional2Button.Text = buttons[i];
							ConfirmOptional2Button.Visible = true;
							break;
						case 2:
							ConfirmOptional3Button.Text = buttons[i];
							ConfirmOptional3Button.Visible = true;
							break;
						case 3:
							ConfirmOptional4Button.Text = buttons[i];
							ConfirmOptional4Button.Visible = true;
							break;
						default:
							// Error
							break;
					}
				}
			}
			else
			{
				if (button.Length != 0)
				{
					ConfirmOptional4Button.Text = button;
					ConfirmOptional4Button.Visible = true;
				}
			}

			// Enable the multi item message window
			ConfirmTreeView.Nodes.Clear();
			ConfirmTreeView.BeginUpdate();
			mBuilding = true;

			List<Object> args = new List<Object>();
			args.Add(parentPath);
			args.Add(assets);
			args.Add(assetLabels);

			ProgressDialog progress = new ProgressDialog("Initializing...", "Populating Tree", DialogInitialize_Worker, args, true);
			progress.ShowDialog(MogMainForm.MainApp);

			ConfirmTreeView.EndUpdate();
			ConfirmTreeView.ExpandAll();
			mBuilding = false;

			Show(MogMainForm.MainApp);
		}

		private void DialogInitialize_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<Object> args = e.Argument as List<Object>;
			string parentPath = args[0] as string;
			ArrayList assets = args[1] as ArrayList;
			ArrayList assetLabels = args[2] as ArrayList;
			TreeNode parentNode = null;

			for (int counter = 0; counter < assets.Count; counter++)
			{
				string rawString = assets[counter] as string;
				string labelString = (assetLabels != null) ? assetLabels[counter] as string : "";

				worker.ReportProgress(counter * 100 / assets.Count);

				// Determin if we should use the label or the str 
				string displayString = (labelString.Length > 0) ? labelString : rawString;

				// Check if this is an official MOG_Filename? and
				// Make sure it contains no path!
				MOG_Filename filename = new MOG_Filename(displayString);
				if (filename.GetFilenameType() != MOG_FILENAME_TYPE.MOG_FILENAME_Unknown &&
					filename.GetPath().Length == 0)
				{
					if (ConfirmTreeView.ImageList != MogUtil_AssetIcons.Images)
					{
						ConfirmTreeView.ImageList = MogUtil_AssetIcons.Images;
					}

					// Build the node
					BuildMogFilenameNode(filename, rawString, labelString);
				}
				// Check if this resembles a path?
				else if (displayString.Contains("\\"))
				{
					// Is this the first node?
					if (ConfirmTreeView.Nodes.Count == 0)
					{
						// Check if we are missing a parentPath?
						if (parentPath.Length == 0)
						{
							// Use the root of the file as our parentPath
							parentPath = displayString.Substring(0, displayString.IndexOf("\\"));
						}

						// Create the start node
						parentNode = ConfirmTreeView.Nodes.Add(parentPath);
						parentNode.Checked = true;

						try
						{
							// Select file icon
							parentNode.ImageIndex = MogUtil_AssetIcons.GetFileIconIndex(parentNode.FullPath);
						}
						catch (Exception ex)
						{
							ex.ToString();
						}
					}

					// Figure out what...if any...remainingPath
					string remainingPath = "";
					if (displayString.StartsWith(parentPath, StringComparison.CurrentCultureIgnoreCase))
					{
						remainingPath = displayString.Substring(parentPath.Length).Trim("\\".ToCharArray());
					}

					// Build the Node path
					BuildWindowsFileNode(parentNode, remainingPath, rawString);
				}
				else
				{
					// Check if we have a parentNode?
					if (parentNode != null)
					{
						// Just add the basic node
						TreeNode thisNode = new TreeNode(displayString);
						thisNode.Checked = true;
						parentNode.Nodes.Add(thisNode);
					}
				}
			}
		}

		private void BuildWindowsFileNode(TreeNode parentNode, string displayString, string fullFilename)
		{
			// Split up str into the path and filename
			string[] parts = displayString.Split("\\".ToCharArray());

			TreeNode thisNode = FindNode(parentNode, parts[0]);
			if (thisNode == null)
			{
				string nodeText = parts[0];

				// Check if this is the final layer?
				if (parts.Length == 1)
				{
					nodeText = displayString;
				}

				// Create a new node
				thisNode = new TreeNode(nodeText);
				thisNode.Checked = true;
				thisNode.Tag = fullFilename;
				// Check if this is the final item?
				if (parts.Length == 1)
				{
					// Add this to the bottom because it is the final node
					parentNode.Nodes.Add(thisNode);
				}
				else
				{
					// Insert this at the top because it is a directory
					parentNode.Nodes.Insert(0, thisNode);
				}

				try
				{
					// Select file icon
					thisNode.ImageIndex = MogUtil_AssetIcons.GetFileIconIndex(thisNode.FullPath);
				}
				catch(Exception e)
				{
					e.ToString();
				}
			}

			// Check if we need to keep drilling?
			if (parts.Length > 1)
			{
				string remainingPath = displayString.Substring(parts[0].Length + 1);
				BuildWindowsFileNode(thisNode, remainingPath, fullFilename);
			}
		}


		/// Build a node within the confirm treeView of assets to confirm
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="fullFilename"></param>
		private void BuildMogFilenameNode(MOG_Filename filename, string fullFilename, string label)
		{
			TreeNode node = MogUtil_ClassificationTrees.CreateAndExpandTreeNodeFullPath(ConfirmTreeView, null, this.LabelTreeDelimiter, label);
			if (node != null)
			{
				node.Tag = fullFilename;
				node.Checked = true;
			}
		}

		private TreeNode FindNode(TreeNodeCollection parentNodes, string title)
		{
			if (parentNodes.Count > 0)
			{
				foreach(TreeNode node in parentNodes)
				{
					if (string.Compare(node.Text, title, true) == 0)
					{
						return node;
					}
				}
			}

			return null;
		}

		private TreeNode FindNode(TreeNode parentNode, string title)
		{
			if (parentNode != null)
			{
				foreach (TreeNode node in parentNode.Nodes)
				{
					if (string.Compare(node.Text, title, true) == 0)
					{
						return node;
					}
				}
			}
			return null;
		}

		public void DialogShowModal()
		{
			while(result == -1)
			{
				Application.DoEvents();
			}
		}

		public bool DialogProcess()
		{
			return cancel;
		}

		public void DialogUpdate(int percent, string description)
		{
			if (description.Length != 0)
			{
				ConfirmLabel.Visible = true;
				ConfirmLabel.Text = description;
			}

			Application.DoEvents();
		}

		public void DialogKill()
		{
			// Populate the selected array
			GetChecked(ConfirmTreeView.Nodes);
			Close();
		}

		private void ConfirmOptionalButton_Click(object sender, System.EventArgs e)
		{
			string buttonString = ((Button)sender).Text;
			for (int i = 0; i < buttons.Length; i++)
			{
				if (string.Compare(buttons[i], buttonString, true) == 0)
				{
					result =  i;
				}
			}
		}

		private void ConfirmTreeView_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (!mBuilding)
			{
				TreeNode parent = e.Node;
				foreach (TreeNode node in parent.Nodes)
				{
					node.Checked = parent.Checked;
				}
			}
		}

		private void GetChecked(TreeNodeCollection nodes)
		{
			// Walk all these nodes
			foreach (TreeNode node in nodes)
			{
				if (node.Nodes.Count > 0 )
				{
					GetChecked(node.Nodes);
				}
				else
				{
					if (node.Checked)
					{
						mSelectedItems.Add(node.Tag);
					}
				}
			}
		}
	}
}
