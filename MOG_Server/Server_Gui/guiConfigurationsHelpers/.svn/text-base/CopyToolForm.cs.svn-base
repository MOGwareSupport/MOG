using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.DOSUTILS;

namespace MOG_Server.Server_Gui.guiConfigurationsHelpers
{
	public class CopyToolForm : System.Windows.Forms.Form
	{
		#region System definitions

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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.toolTips = new System.Windows.Forms.ToolTip(this.components);
			this.btnAddFolder = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnAddFile = new System.Windows.Forms.Button();
			this.FilesTreeView = new System.Windows.Forms.TreeView();
			this.panel1 = new System.Windows.Forms.Panel();
			this.FilesListViewLabel = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel2 = new System.Windows.Forms.Panel();
			this.ToolsTreeInfoGroupBox = new System.Windows.Forms.GroupBox();
			this.DestFolderLabel = new System.Windows.Forms.Label();
			this.CreateSubdirTextBox = new System.Windows.Forms.TextBox();
			this.CreateSubdirCheckbox = new System.Windows.Forms.CheckBox();
			this.btnToolFolderBrowse = new System.Windows.Forms.Button();
			this.CopyToToolsCheckBox = new System.Windows.Forms.CheckBox();
			this.btnDefault = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnExecutableBrowse = new System.Windows.Forms.Button();
			this.btnGetExecutable = new System.Windows.Forms.Button();
			this.CmdLineLabel = new System.Windows.Forms.Label();
			this.ExecutableTextBox = new System.Windows.Forms.TextBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.ToolsTreeInfoGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnAddFolder
			// 
			this.btnAddFolder.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnAddFolder.Location = new System.Drawing.Point(140, 248);
			this.btnAddFolder.Name = "btnAddFolder";
			this.btnAddFolder.TabIndex = 37;
			this.btnAddFolder.Text = "Add &Folder";
			this.toolTips.SetToolTip(this.btnAddFolder, "Click to select and add a folder and all its contents  to the Tool Components box" +
				"");
			this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnRemove.Location = new System.Drawing.Point(236, 248);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.TabIndex = 36;
			this.btnRemove.Text = "&Remove";
			this.toolTips.SetToolTip(this.btnRemove, "Click to remove the selected file(s)/folder(s)");
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnAddFile
			// 
			this.btnAddFile.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnAddFile.Location = new System.Drawing.Point(44, 248);
			this.btnAddFile.Name = "btnAddFile";
			this.btnAddFile.TabIndex = 35;
			this.btnAddFile.Text = "&Add Files";
			this.toolTips.SetToolTip(this.btnAddFile, "Click to select and add files to the Tool Components box");
			this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
			// 
			// FilesTreeView
			// 
			this.FilesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.FilesTreeView.ImageIndex = -1;
			this.FilesTreeView.Location = new System.Drawing.Point(8, 24);
			this.FilesTreeView.Name = "FilesTreeView";
			this.FilesTreeView.SelectedImageIndex = -1;
			this.FilesTreeView.Size = new System.Drawing.Size(344, 208);
			this.FilesTreeView.TabIndex = 34;
			this.toolTips.SetToolTip(this.FilesTreeView, "The Tool Components box shows all files and folders that are currently a part of " +
				"the tool");
			this.FilesTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FilesTreeView_MouseDown);
			this.FilesTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FilesTreeView_MouseUp);
			this.FilesTreeView.DoubleClick += new System.EventHandler(this.FilesTreeView_DoubleClick);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnAddFolder);
			this.panel1.Controls.Add(this.btnRemove);
			this.panel1.Controls.Add(this.btnAddFile);
			this.panel1.Controls.Add(this.FilesTreeView);
			this.panel1.Controls.Add(this.FilesListViewLabel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(360, 286);
			this.panel1.TabIndex = 37;
			// 
			// FilesListViewLabel
			// 
			this.FilesListViewLabel.Location = new System.Drawing.Point(8, 8);
			this.FilesListViewLabel.Name = "FilesListViewLabel";
			this.FilesListViewLabel.Size = new System.Drawing.Size(464, 16);
			this.FilesListViewLabel.TabIndex = 12;
			this.FilesListViewLabel.Text = "Tool Components";
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(360, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 286);
			this.splitter1.TabIndex = 38;
			this.splitter1.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.ToolsTreeInfoGroupBox);
			this.panel2.Controls.Add(this.CopyToToolsCheckBox);
			this.panel2.Controls.Add(this.btnDefault);
			this.panel2.Controls.Add(this.btnOK);
			this.panel2.Controls.Add(this.btnExecutableBrowse);
			this.panel2.Controls.Add(this.btnGetExecutable);
			this.panel2.Controls.Add(this.CmdLineLabel);
			this.panel2.Controls.Add(this.ExecutableTextBox);
			this.panel2.Controls.Add(this.btnCancel);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(363, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(509, 286);
			this.panel2.TabIndex = 39;
			// 
			// ToolsTreeInfoGroupBox
			// 
			this.ToolsTreeInfoGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.ToolsTreeInfoGroupBox.Controls.Add(this.DestFolderLabel);
			this.ToolsTreeInfoGroupBox.Controls.Add(this.CreateSubdirTextBox);
			this.ToolsTreeInfoGroupBox.Controls.Add(this.CreateSubdirCheckbox);
			this.ToolsTreeInfoGroupBox.Controls.Add(this.btnToolFolderBrowse);
			this.ToolsTreeInfoGroupBox.Location = new System.Drawing.Point(8, 56);
			this.ToolsTreeInfoGroupBox.Name = "ToolsTreeInfoGroupBox";
			this.ToolsTreeInfoGroupBox.Size = new System.Drawing.Size(496, 112);
			this.ToolsTreeInfoGroupBox.TabIndex = 44;
			this.ToolsTreeInfoGroupBox.TabStop = false;
			this.ToolsTreeInfoGroupBox.Text = "Tool Tree Info";
			// 
			// DestFolderLabel
			// 
			this.DestFolderLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.DestFolderLabel.Location = new System.Drawing.Point(24, 80);
			this.DestFolderLabel.Name = "DestFolderLabel";
			this.DestFolderLabel.Size = new System.Drawing.Size(440, 16);
			this.DestFolderLabel.TabIndex = 17;
			this.DestFolderLabel.Text = "Destination folder: C:\\MOGDRIVE\\MOG\\Projects\\Advent\\Tools";
			// 
			// CreateSubdirTextBox
			// 
			this.CreateSubdirTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.CreateSubdirTextBox.Enabled = false;
			this.CreateSubdirTextBox.Location = new System.Drawing.Point(16, 32);
			this.CreateSubdirTextBox.Name = "CreateSubdirTextBox";
			this.CreateSubdirTextBox.Size = new System.Drawing.Size(368, 20);
			this.CreateSubdirTextBox.TabIndex = 16;
			this.CreateSubdirTextBox.Text = "";
			this.CreateSubdirTextBox.TextChanged += new System.EventHandler(this.CreateSubdirTextBox_TextChanged);
			// 
			// CreateSubdirCheckbox
			// 
			this.CreateSubdirCheckbox.Location = new System.Drawing.Point(24, 56);
			this.CreateSubdirCheckbox.Name = "CreateSubdirCheckbox";
			this.CreateSubdirCheckbox.Size = new System.Drawing.Size(184, 16);
			this.CreateSubdirCheckbox.TabIndex = 14;
			this.CreateSubdirCheckbox.Text = "Create/specify dedicated folder";
			this.CreateSubdirCheckbox.CheckedChanged += new System.EventHandler(this.CreateSubdirCheckbox_CheckedChanged);
			// 
			// btnToolFolderBrowse
			// 
			this.btnToolFolderBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnToolFolderBrowse.Enabled = false;
			this.btnToolFolderBrowse.Location = new System.Drawing.Point(392, 32);
			this.btnToolFolderBrowse.Name = "btnToolFolderBrowse";
			this.btnToolFolderBrowse.TabIndex = 35;
			this.btnToolFolderBrowse.Text = "Browse";
			this.btnToolFolderBrowse.Click += new System.EventHandler(this.btnToolFolderBrowse_Click);
			// 
			// CopyToToolsCheckBox
			// 
			this.CopyToToolsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.CopyToToolsCheckBox.Checked = true;
			this.CopyToToolsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CopyToToolsCheckBox.Location = new System.Drawing.Point(8, 40);
			this.CopyToToolsCheckBox.Name = "CopyToToolsCheckBox";
			this.CopyToToolsCheckBox.Size = new System.Drawing.Size(488, 16);
			this.CopyToToolsCheckBox.TabIndex = 43;
			this.CopyToToolsCheckBox.Text = "Copy tool to project \'Tools\' tree";
			this.CopyToToolsCheckBox.CheckedChanged += new System.EventHandler(this.CopyToToolsCheckBox_CheckedChanged);
			// 
			// btnDefault
			// 
			this.btnDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDefault.Location = new System.Drawing.Point(216, 248);
			this.btnDefault.Name = "btnDefault";
			this.btnDefault.TabIndex = 42;
			this.btnDefault.Text = "Default";
			this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(392, 248);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 41;
			this.btnOK.Text = "OK";
			// 
			// btnExecutableBrowse
			// 
			this.btnExecutableBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnExecutableBrowse.Location = new System.Drawing.Point(304, 208);
			this.btnExecutableBrowse.Name = "btnExecutableBrowse";
			this.btnExecutableBrowse.TabIndex = 40;
			this.btnExecutableBrowse.Text = "Browse";
			this.toolTips.SetToolTip(this.btnExecutableBrowse, "Click to browse for an executable");
			this.btnExecutableBrowse.Click += new System.EventHandler(this.btnExecutableBrowse_Click);
			// 
			// btnGetExecutable
			// 
			this.btnGetExecutable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnGetExecutable.Enabled = false;
			this.btnGetExecutable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnGetExecutable.Location = new System.Drawing.Point(16, 208);
			this.btnGetExecutable.Name = "btnGetExecutable";
			this.btnGetExecutable.Size = new System.Drawing.Size(40, 23);
			this.btnGetExecutable.TabIndex = 39;
			this.btnGetExecutable.Text = "---->";
			this.toolTips.SetToolTip(this.btnGetExecutable, "Click to set the file currently selected in the Tool Components box as the startu" +
				"p executable");
			this.btnGetExecutable.Click += new System.EventHandler(this.btnGetExecutable_Click);
			// 
			// CmdLineLabel
			// 
			this.CmdLineLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.CmdLineLabel.Location = new System.Drawing.Point(64, 192);
			this.CmdLineLabel.Name = "CmdLineLabel";
			this.CmdLineLabel.Size = new System.Drawing.Size(240, 16);
			this.CmdLineLabel.TabIndex = 38;
			this.CmdLineLabel.Text = "Startup Executable";
			// 
			// ExecutableTextBox
			// 
			this.ExecutableTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.ExecutableTextBox.Location = new System.Drawing.Point(64, 208);
			this.ExecutableTextBox.Name = "ExecutableTextBox";
			this.ExecutableTextBox.Size = new System.Drawing.Size(232, 20);
			this.ExecutableTextBox.TabIndex = 37;
			this.ExecutableTextBox.Text = "";
			this.toolTips.SetToolTip(this.ExecutableTextBox, "The startup executable is the file that launches the tool (usually ends with .EXE" +
				" or .BAT)");
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(304, 248);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 36;
			this.btnCancel.Text = "Cancel";
			// 
			// CopyToolForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(872, 286);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.MinimumSize = new System.Drawing.Size(880, 320);
			this.Name = "CopyToolForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Tool Components";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ToolsTreeInfoGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion		
		
		#endregion

		#region User definitions
		private BASE mog;
		private ImageList imageList;
		private string executablePath;
		private ToolTreeNode executableNode;
		private ToolTreeNode selectedNode;
		private System.Windows.Forms.ToolTip toolTips;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnAddFolder;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnAddFile;
		private System.Windows.Forms.TreeView FilesTreeView;
		private System.Windows.Forms.Label FilesListViewLabel;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.GroupBox ToolsTreeInfoGroupBox;
		private System.Windows.Forms.Label DestFolderLabel;
		private System.Windows.Forms.TextBox CreateSubdirTextBox;
		private System.Windows.Forms.CheckBox CreateSubdirCheckbox;
		private System.Windows.Forms.Button btnToolFolderBrowse;
		private System.Windows.Forms.CheckBox CopyToToolsCheckBox;
		private System.Windows.Forms.Button btnDefault;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnExecutableBrowse;
		private System.Windows.Forms.Button btnGetExecutable;
		private System.Windows.Forms.Label CmdLineLabel;
		private System.Windows.Forms.TextBox ExecutableTextBox;
		private System.Windows.Forms.Button btnCancel;
		private FileCopyNotifierForm fcnf;

		public string ExecutablePath { get{return executablePath;} }
		#endregion

		#region Constructor
		public CopyToolForm(CopyToolType type, BASE mog)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.mog = mog;

			this.imageList = new ImageList();
			this.imageList.Images.Add( (Image)new Bitmap( "C:\\Documents and Settings\\jbianchi\\My Documents\\work\\MOG.Net\\MOG_Server\\Server_Gui\\guiConfigurationsHelpers\\fileicon.bmp" ) );
			this.imageList.Images.Add( (Image)new Bitmap( "C:\\Documents and Settings\\jbianchi\\My Documents\\work\\MOG.Net\\MOG_Server\\Server_Gui\\guiConfigurationsHelpers\\foldericon.bmp" ) );
			this.FilesTreeView.ImageList = this.imageList;
			this.btnGetExecutable.Enabled = false;
			this.selectedNode = null;
			this.executableNode = null;

			switch (type) 
			{
				case CopyToolType.Ripper:
					this.Text = "Select Ripper Components";
					this.FilesListViewLabel.Text = "Ripper components";
					this.CopyToToolsCheckBox.Text = "Copy ripper to project \"Tools\" tree";
					break;
				case CopyToolType.RipTasker:
					this.Text = "Select Rip Tasker Components";
					this.FilesListViewLabel.Text = "Rip tasker components";
					this.CopyToToolsCheckBox.Text = "Copy rip tasker to project \"Tools\" tree";
					break;
				case CopyToolType.Viewer:
					this.Text = "Select Viewer Components";
					this.FilesListViewLabel.Text = "Viewer components";
					this.CopyToToolsCheckBox.Text = "Copy viewer to project \"Tools\" tree";
					break;
				case CopyToolType.Generic:
					this.Text = "Select Tool Components";
					this.FilesListViewLabel.Text = "Tool components";
					this.CopyToToolsCheckBox.Text = "Copy tool to project \"Tools\" tree";
					break;
			}

			updateDestFolderLabel();
		}
		#endregion

		#region Member functions
		private void CopySpecifiedFiles(TreeNodeCollection nodes, string copyPath) 
		{
			foreach (ToolTreeNode curNode in nodes) 
			{
				if (curNode.type == ToolTreeNodeType.File) 
				{
					this.fcnf.LabelText = string.Concat("Copying ", curNode.name, " to ", copyPath, "\\", curNode.name);
					File.Copy(curNode.fullPath, string.Concat(copyPath, "\\", curNode.name), true);
					this.fcnf.step();
					if (this.fcnf.Aborted) 
					{
						throw new FileCopyNotifierFormAbortedByUserException("File copy abort requested by user");
					}
				}
				else if (curNode.type == ToolTreeNodeType.Directory) 
				{
					string dirName = string.Concat(copyPath, "\\", curNode.name);
					if (!Directory.Exists(dirName)) 
					{
						Directory.CreateDirectory( dirName );
					}
					
					CopySpecifiedFiles( curNode.Nodes, string.Concat(copyPath, "\\", curNode.name) );
				}
			}
		}

		private void SelectNode( ToolTreeNode node ) 
		{
			this.FilesTreeView.SelectedNode = node;
			this.selectedNode = node;

			this.btnGetExecutable.Enabled = false;
			if ( node != null && node.type == ToolTreeNodeType.File) 
			{
				btnGetExecutable.Enabled = true;
			}
		}

		private void updateDestFolderLabel() 
		{
			if (CreateSubdirCheckbox.Checked && CreateSubdirTextBox.Text != "") 
			{
				DestFolderLabel.Text = string.Concat( "Destination folder:  ", this.mog.GetProject().GetProjectToolsPath(), "\\", CreateSubdirTextBox.Text );
			}
			else 
			{
				DestFolderLabel.Text = string.Concat( "Destination folder:  ", this.mog.GetProject().GetProjectToolsPath() );
			}
		}
		#endregion
		
		#region Events
		private void CreateSubdirCheckbox_CheckedChanged(object sender, System.EventArgs e)
		{
			if (CreateSubdirCheckbox.Checked) 
			{
				this.CreateSubdirTextBox.Enabled = true;
                this.btnToolFolderBrowse.Enabled = true;
			}
			else 
			{
				this.CreateSubdirTextBox.Enabled = false;
				this.btnToolFolderBrowse.Enabled = false;
			}
			
			updateDestFolderLabel();
		}
		
		private void CreateSubdirTextBox_TextChanged(object sender, System.EventArgs e)
		{
			updateDestFolderLabel();
		}
		
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if (this.executablePath == null || this.executablePath == "" || this.ExecutableTextBox.Text == "") 
			{
				MessageBox.Show("Please specify the executable that starts this tool", "Missing executable");
				return;
			}
			
			if (CopyToToolsCheckBox.Checked) 
			{
				//
				// copy the tool to the Tools tree
				//
				
				string copyPath = this.mog.GetProject().GetProjectToolsPath();
				while (copyPath.EndsWith("\\")) 
				{
					copyPath = copyPath.Substring(0, copyPath.Length-1);
				}

				try 
				{
					if (CreateSubdirCheckbox.Checked) 
					{
						// we need to create a special subdir for this tool in the Tools tree
						if (CreateSubdirTextBox.Text != "") 
						{
							copyPath = string.Concat(copyPath, "\\", CreateSubdirTextBox.Text);
							if (!Directory.Exists(copyPath)) 
							{
								if ( !Directory.CreateDirectory(copyPath).Exists ) 
								{
									MessageBox.Show("Couldn't create specified tool directory, please re-enter it", "Directory creation error");
								}
							}
						}
					}
				} 
				catch (Exception) { MessageBox.Show("Couldn't create specified tool directory, please re-enter it", "Directory creation error"); return; }

				// okay, now we're ready to copy the files to 'copyPath'
				try 
				{
					int numFiles = 0;
					foreach (ToolTreeNode ttn in this.FilesTreeView.Nodes) 
					{
						numFiles += ttn.GetNumFilesRecursive();
					}
					
					try 
					{
						this.fcnf = new FileCopyNotifierForm( numFiles );
						this.fcnf.TitleText = "File Copy Progress";
						this.fcnf.Show();
						this.Enabled = false;
						CopySpecifiedFiles(this.FilesTreeView.Nodes, copyPath);
						this.Enabled = true;
						this.fcnf.Close();
						this.fcnf = null;
					}
					catch (FileCopyNotifierFormAbortedByUserException) 
					{ 
						this.Enabled = true;
						fcnf.Close();
						fcnf = null;
						MessageBox.Show("Tool copy cancelled", "Copy Abort");
						return;
					}
				}
				catch (Exception excp) 
				{
					MessageBox.Show(string.Concat("An error occured in copying the files/folders specified:\n\n", excp.Message), "File copy error");
				}
                
				// build the proper executable path backwards, starting from the filename and working
				//  through its parent dirs
				this.executablePath = this.executableNode.name;
				ToolTreeNode exNode = (ToolTreeNode)this.executableNode.Parent;
				while (exNode != null) 
				{
					this.executablePath = string.Concat(exNode.name, "\\", this.executablePath);
					exNode = (ToolTreeNode)exNode.Parent;
				}
				this.executablePath = string.Concat(copyPath, "\\", this.executablePath);
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnAddFile_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Select files";
			ofd.Multiselect = true;
			if (ofd.ShowDialog() == DialogResult.OK) 
			{
				foreach (string filename in ofd.FileNames) 
				{
					ToolTreeNode newNode = new ToolTreeNode(filename, ToolTreeNodeType.File);
					newNode.ImageIndex = 0;
					newNode.SelectedImageIndex = 0;
					this.FilesTreeView.Nodes.Add( newNode );
				}
			}
		}

		private void btnAddFolder_Click(object sender, System.EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.Description = "Select tool folder";

			if (fbd.ShowDialog() == DialogResult.OK) 
			{
				ToolTreeNode newNode = ToolTreeNode.EncodeDirectory( fbd.SelectedPath );
				newNode.ImageIndex = 1;
				newNode.SelectedImageIndex = 1;
				this.FilesTreeView.Nodes.Add( newNode );
			}
		}

		private void btnGetExecutable_Click(object sender, System.EventArgs e)
		{
			if (this.selectedNode != null) 
			{
				if ( this.selectedNode.type == ToolTreeNodeType.File ) 
				{
					this.executableNode = this.selectedNode;
					this.executablePath = this.selectedNode.fullPath;
					this.ExecutableTextBox.Text = this.selectedNode.Text;
				}
			}
			else 
			{
				this.executableNode = null;
				this.executablePath = "";
				this.ExecutableTextBox.Text = "";
			}
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			if (this.selectedNode != null) 
			{
				if ( this.selectedNode.ContainsNodeRecursive(this.executableNode) )
				{
					if (MessageBox.Show("This action will remove the specified startup executable. Do you want to proceed?", "Remove?", MessageBoxButtons.YesNo) == DialogResult.No) 
					{
						return;
					}
					this.executableNode = null;
					this.executablePath = "";
					this.ExecutableTextBox.Text = "";
				}
				this.FilesTreeView.SelectedNode.Remove();
			}
		}

		private void CopyToToolsCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if (CopyToToolsCheckBox.Checked) 
			{
				ToolsTreeInfoGroupBox.Enabled = true;
			}
			else 
			{
				ToolsTreeInfoGroupBox.Enabled = false;
			}
		}

		private void FilesTreeView_DoubleClick(object sender, System.EventArgs e)
		{
			btnGetExecutable_Click(sender, e);
			//			if (this.FilesTreeView.SelectedNode != null && ((ToolTreeNode)this.FilesTreeView.SelectedNode).type == ToolTreeNodeType.File)
			//			{
			//				this.executablePath = ((ToolTreeNode)FilesTreeView.SelectedNode).fullPath;
			//				this.CmdLineTextBox.Text = FilesTreeView.SelectedNode.Text;
			//			}		
		}

		private void FilesTreeView_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch (e.KeyData) 
			{
				case Keys.Delete:
				case Keys.D:
				case Keys.R:
					btnRemove_Click(sender, new System.EventArgs());
					break;
				case Keys.Insert:
				case Keys.A:
					btnAddFile_Click(sender, new System.EventArgs());
					break;
				case Keys.F:
					btnAddFolder_Click(sender, new System.EventArgs());
					break;
			}
		}

		private void FilesTreeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			SelectNode( (ToolTreeNode)this.FilesTreeView.GetNodeAt( e.X, e.Y ) );
			
			/*
			this.FilesTreeView.SelectedNode = this.FilesTreeView.GetNodeAt( e.X, e.Y );
			this.selectedNode = (ToolTreeNode)this.FilesTreeView.SelectedNode;
			this.btnGetExecutable.Enabled = true;

			this.btnGetExecutable.Enabled = false;
			if ( this.selectedNode != null && this.selectedNode.type == ToolTreeNodeType.File) 
			{
				btnGetExecutable.Enabled = true;
			}
			*/
		}

		private void FilesTreeView_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) 
			{
				MenuItem[] items = new MenuItem[4];
				items[0] = new MenuItem("Add file", new System.EventHandler(btnAddFile_Click));
				items[1] = new MenuItem("Add folder", new System.EventHandler(btnAddFolder_Click));
				items[2] = new MenuItem("Remove", new System.EventHandler(btnRemove_Click));
				items[3] = new MenuItem("Set as startup executable", new System.EventHandler(btnGetExecutable_Click));
				ContextMenu cm = new ContextMenu(items);

				if (this.selectedNode != null)
				{
					items[0].Enabled = false;
					items[1].Enabled = false;
					if (this.selectedNode.type == ToolTreeNodeType.File) 
					{
						items[3].Visible = this.btnGetExecutable.Enabled;
					}
				}
				else 
				{
					items[2].Enabled = false;
					items[3].Visible = false;
				}

				cm.Show(this.FilesTreeView, new Point(e.X, e.Y));
			}
		}

		private void btnExecutableBrowse_Click(object sender, System.EventArgs e)
		{
			// add the executable to FilesListView so it can be copied correctly
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Select startup executable";
			ofd.Multiselect = false;
			if (ofd.ShowDialog() == DialogResult.OK) 
			{
				ToolTreeNode newNode = new ToolTreeNode(ofd.FileName, ToolTreeNodeType.File);
				this.FilesTreeView.Nodes.Add( newNode );
				SelectNode( newNode );
				btnGetExecutable_Click(sender, e);
			}
		}
		
		private void btnToolFolderBrowse_Click(object sender, System.EventArgs e)
		{
			// add the executable to FilesListView so it can be copied correctly
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.Description = "Select or create a folder for the tool";
			fbd.SelectedPath = this.mog.GetProject().GetProjectToolsPath();
			fbd.ShowNewFolderButton = true;

			if (fbd.ShowDialog() == DialogResult.OK) 
			{
				string toolsPathLowerCase = this.mog.GetProject().GetProjectToolsPath().ToLower();
				if (fbd.SelectedPath.ToLower() == toolsPathLowerCase) 
				{
					return;
				}
				if (fbd.SelectedPath.ToLower().StartsWith( toolsPathLowerCase )) 
				{
					this.CreateSubdirTextBox.Text = fbd.SelectedPath.ToLower().Replace(this.mog.GetProject().GetProjectToolsPath().ToLower() + "\\", "");
				}
				else 
				{
					MessageBox.Show("Please select or create a folder from within the project Tools folder tree", "Invalid folder selected");
				}
			} 
		}
		
		private void btnDefault_Click(object sender, System.EventArgs e)
		{
			this.FilesTreeView.Nodes.Clear();
			ToolTreeNode defaultNode = new ToolTreeNode("[ProjectPath]\\MOG\\Tools\\PC\\LaunchExplorerWin\\LaunchExplorerWin.exe", ToolTreeNodeType.File);
			this.FilesTreeView.Nodes.Add( defaultNode );
			SelectNode( defaultNode );
			btnGetExecutable_Click(sender, e);
			this.CopyToToolsCheckBox.Checked = false;
			btnOK_Click(sender, e);
		}
		#endregion
	}
	
	#region Enums
	public enum CopyToolType { Ripper, RipTasker, Viewer, Generic }
	#endregion

	#region Supporting classes
	enum ToolTreeNodeType { File, Directory, Invalid }
	class ToolTreeNode : TreeNode
	{
		public ToolTreeNodeType type;
		public string fullPath;
		public string name;

		public ToolTreeNode() : base()
		{
			this.type = ToolTreeNodeType.File;
			this.ImageIndex = 0;
			this.SelectedImageIndex = 0;
			this.fullPath = "";
			this.name = "";
		}
		public ToolTreeNode(string fullPath, ToolTreeNodeType type) : base( (new DirectoryInfo(fullPath)).Name )
		{
			this.type = type;
			this.ImageIndex = 0;
			this.SelectedImageIndex = 0;
			if (type == ToolTreeNodeType.Directory) 
			{
				this.ImageIndex = 1;
				this.SelectedImageIndex = 1;
			}
			this.fullPath = fullPath;
			this.name = (new FileInfo(fullPath)).Name;
		}

		public int GetNumFilesRecursive() 
		{
			int numFiles = 0;
			foreach (ToolTreeNode ttn in this.Nodes) 
			{
				if (ttn.type == ToolTreeNodeType.File) 
				{
					++numFiles;
				}
				else if (ttn.type == ToolTreeNodeType.Directory) 
				{
					numFiles += ttn.GetNumFilesRecursive();
				}
			}
			
			return numFiles;
		}

		public void AddFile(string fullPath) 
		{
			FileInfo finfo = new FileInfo(fullPath);
			if (finfo.Exists) 
			{
				this.Nodes.Add( new ToolTreeNode(fullPath, ToolTreeNodeType.File) );
			}
		}

		public void AddSubdir(string fullPath) 
		{
			DirectoryInfo dinfo = new DirectoryInfo(fullPath);
			if (dinfo.Exists)
			{
				this.Nodes.Add( new ToolTreeNode(fullPath, ToolTreeNodeType.Directory) );
			}
		}

		public override bool Equals(object obj)
		{
			ToolTreeNode ttn = (ToolTreeNode)obj;
			if (this.type == ttn.type && this.fullPath.ToLower() == ttn.fullPath.ToLower()) 
			{
				return true;
			}

			return false;
		}
		public override int GetHashCode(){return base.GetHashCode ();}


		public bool ContainsNodeRecursive(ToolTreeNode node)
		{
			if (this == node) 
			{
				return true;
			}
			
			foreach (ToolTreeNode ttn in this.Nodes) 
			{
				if ( ttn.ContainsNodeRecursive(node) )
				{
					return true;
				}
			}

			return false;
		}

		public static ToolTreeNode EncodeDirectory(string path) 
		{
			DirectoryInfo dinfo = new DirectoryInfo(path);
			if (dinfo.Exists) 
			{
				ToolTreeNode ttn = new ToolTreeNode(path, ToolTreeNodeType.Directory);
				foreach (DirectoryInfo dir in dinfo.GetDirectories()) 
				{
					ttn.Nodes.Add( ToolTreeNode.EncodeDirectory( dir.FullName ) );
				}
				foreach (FileInfo file in dinfo.GetFiles()) 
				{
					ttn.Nodes.Add( new ToolTreeNode(file.FullName, ToolTreeNodeType.File) );
				}

				return ttn;
			}
			return null;
		}

	}
	#endregion

}
