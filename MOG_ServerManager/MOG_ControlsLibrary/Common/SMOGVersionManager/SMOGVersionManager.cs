using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG;
using MOG.COMMAND;
using MOG.DOSUTILS;
using MOG.INI;
using MOG.REPORT;
using MOG.PROGRESS;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using EV.Windows.Forms;
using MOG_CoreControls;
using System.Collections.Generic;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for blaForm.
	/// </summary>
	public class VersionManagerClass
	{
		public const int SERVER_ICON		= 0;
		public const int CLIENT_ICON		= 1;
		public const int SLAVE_ICON			= 2;
		public const int COMMANDLINE_ICON	= 3;
		public const int BRIDGE_ICON		= 3;

		private enum VERSION_TYPE {SERVER, CLIENT, BRIDGE};

        public string DeploymentDirectory;
		public string DeploymentTarget = "Current";
		public bool bCheckOverride = false;

		ArrayList mServerVersions = new ArrayList();
		ArrayList mClientVersions = new ArrayList();
		ArrayList mBridgeVersions = new ArrayList();
		MOG_ServerManagerMainForm mainForm;

		private ArrayList mListViewSort_Manager = new ArrayList();

		public VersionManagerClass(MOG_ServerManagerMainForm main)
		{
			mainForm = main;
			mainForm.VersionFilesListView.SmallImageList = MogUtil_AssetIcons.Images;
			ListViewSortManager serverVersions = new ListViewSortManager(mainForm.ServerListView, new Type[] {
																												 typeof(ListViewTextCaseInsensitiveSort),
																												 typeof(ListViewTextCaseInsensitiveSort)
																											 });
			ListViewSortManager clientVersions = new ListViewSortManager(mainForm.ClientListView, new Type[] {
																												 typeof(ListViewTextCaseInsensitiveSort),
																												 typeof(ListViewTextCaseInsensitiveSort)
																											 });
			
			mListViewSort_Manager.Add(serverVersions);
			mListViewSort_Manager.Add(clientVersions);
			mListViewSort_Manager.Add(new ListViewSortManager(mainForm.VersionFilesListView, new Type[] {
																									  typeof(ListViewTextCaseInsensitiveSort),
																									  typeof(ListViewDateSort),
																									  typeof(ListViewInt64Sort)
																								}
																								));

			serverVersions.Sort(0, SortOrder.Descending);
			clientVersions.Sort(0, SortOrder.Descending);

            DeploymentDirectory = MOG_ControllerSystem.GetSystem().GetConfigFile().GetString("MOG", "Updates");
            DeploymentDirectory = DeploymentDirectory.ToLower().Replace(MOG_Tokens.GetSystemRepositoryPath().ToLower(), MOG_ControllerSystem.GetSystemRepositoryPath());

		}

		//----------------------------------------------------------
		// Update all the file version information
		public void Update()
		{
			//System.Console.WriteLine("TEST");
			LoadNewData();
		}

		/// <summary>
		/// Get all the versions within the specified directory, and add them to either the server array or the clients array
		/// </summary>
		/// <param name="FullName"></param>
		private void GetAllVersions(string FullName)
		{
			ArrayList fileList = new ArrayList();
			try
			{
				DirectoryInfo updateDir = new DirectoryInfo(FullName);
				// Scan all the folders in this specified directory
				foreach(DirectoryInfo dir in updateDir.GetDirectories())
				{
					// Ommit the 'current' directory unless that is the only directory
					if (string.Compare(dir.Name, DeploymentTarget, true) != 0  || updateDir.GetDirectories().Length == 1)
					{
						// Create the version node
						VersionNode node = this.CreateVersionNode(dir.Name, dir.FullName);
						if (node != null)
						{
							// Assign it to the correct arrayList
							switch(node.Type.ToUpper())
							{
								case "SERVER":
									this.mServerVersions.Add(node);
									break;
								default:
									this.mClientVersions.Add(node);
									break;
							}
						}
					}
				}
			}
			catch(Exception e)
			{
				MOG_Report.ReportMessage("Build version list", e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
			}
		}

		/// <summary>
		/// Fromt he given directory, make a versionNode and return it
		/// </summary>
		/// <param name="directoryName"></param>
		/// <param name="directoryFullName"></param>
		/// <returns></returns>
		private VersionNode CreateVersionNode(string directoryName, string directoryFullName)
		{
			// Can we find a valid version descriptor and the target dir is not a 'current' dir
			if (DosUtils.FileExist(directoryFullName + "\\VERSION.INI"))
			{
				MOG_Ini info = new MOG_Ini(directoryFullName + "\\VERSION.INI");

				// get basic info
				string Name			= info.GetString("INFO", "Name").Replace("\"", "").Trim();
				long MajorVer		= Convert.ToInt64( info.GetString("INFO", "MajorVersion") );
				long MinorVer		= 0;

				// Add code to handle new code drops
				if (info.GetString("INFO", "MinorVersion").Contains("."))
				{
					MinorVer = Convert.ToInt64(info.GetString("INFO", "MinorVersion").Replace(".", ""));
				}
				else
				{
					MinorVer = Convert.ToInt64(info.GetString("INFO", "MinorVersion"));
				}
			
				string type			= info.GetString("INFO", "Type").Trim();

				VersionNode versionNode = new VersionNode(Name);			

				// Create the version node
				versionNode.FolderName = directoryName;
				versionNode.MajorVersion = MajorVer;
				versionNode.MinorVersion = MinorVer;
				versionNode.SourceDirectory = directoryFullName;
				versionNode.Type = type;

				// Apply some special settings based on type
				switch(type.ToUpper())
				{
					case "SERVER":
						versionNode.ImageIndex = SERVER_ICON;
						versionNode.ServerMajorVersion = 0;
						versionNode.ServerMinorversion = 0;
						break;
					case "CLIENT":
						long SMajorVer		= Convert.ToInt64( info.GetString("SERVERCOMPATABILITY", "ServerMajorVersion") );
						long SMinorVer = 0;

						// Add code to handle new code drops
						if (info.GetString("SERVERCOMPATABILITY", "ServerMinorversion").Contains("."))
						{
							SMinorVer = Convert.ToInt64(info.GetString("SERVERCOMPATABILITY", "ServerMinorversion").Replace(".", ""));
						}
						else
						{
							SMinorVer = Convert.ToInt64(info.GetString("SERVERCOMPATABILITY", "ServerMinorversion"));
						}

						versionNode.ServerMajorVersion = SMajorVer;
						versionNode.ServerMinorversion = SMinorVer;
						versionNode.ImageIndex = CLIENT_ICON;
						break;
					case "BRIDGE":
						long BMajorVer		= Convert.ToInt64( info.GetString("SERVERCOMPATABILITY", "ServerMajorVersion") );
						long BMinorVer		= 0;

						// Add code to handle new code drops
						if (info.GetString("SERVERCOMPATABILITY", "ServerMinorversion").Contains("."))
						{
							BMinorVer = Convert.ToInt64(info.GetString("SERVERCOMPATABILITY", "ServerMinorversion").Replace(".", ""));
						}
						else
						{
							BMinorVer = Convert.ToInt64(info.GetString("SERVERCOMPATABILITY", "ServerMinorversion"));							
						}

						versionNode.ServerMajorVersion = BMajorVer;
						versionNode.ServerMinorversion = BMinorVer;
						versionNode.ImageIndex = BRIDGE_ICON;
						break;
					default:
						long dMajorVer		= Convert.ToInt64( info.GetString("SERVERCOMPATABILITY", "ServerMajorVersion") );
						long dMinorVer = 0;

						// Add code to handle new code drops
						if (info.GetString("SERVERCOMPATABILITY", "ServerMinorversion").Contains("."))
						{
							dMinorVer = Convert.ToInt64(info.GetString("SERVERCOMPATABILITY", "ServerMinorversion").Replace(".", ""));
						}
						else
						{
							dMinorVer = Convert.ToInt64(info.GetString("SERVERCOMPATABILITY", "ServerMinorversion"));
						}


						versionNode.ServerMajorVersion = dMajorVer;
						versionNode.ServerMinorversion = dMinorVer;
						versionNode.ImageIndex = BRIDGE_ICON;
						break;
				}

				return versionNode;
			}

			return null;
		}


		//----------------------------------------------------------
		// Load the data off disk
		void LoadNewData()
		{
			mServerVersions.Clear();
			mClientVersions.Clear();
			mBridgeVersions.Clear();

			// Obtain the file system entries in the directory path.
			if(Directory.Exists(DeploymentDirectory) == false)
			{
				return;
			}

			DirectoryInfo Updates = new DirectoryInfo(DeploymentDirectory);
			DirectoryInfo []directoryEntries =	Updates.GetDirectories(); 

			foreach (DirectoryInfo dir in directoryEntries) 
			{
				GetAllVersions(dir.FullName);
			}
		}

		//--------------------------------------------------------------------
		// DVC Initial entry point to load and populate the versions list
		public void BuildVersionsList()
		{
			Update();
			ArrayList serverList = GetServerList();

			mainForm.ServerListView.Items.Clear();
			mainForm.ClientListView.Items.Clear();

			foreach(VersionNode version in serverList)
			{
				ListViewItem item = new ListViewItem(version.Name);
				item.SubItems.Add(version.Version);

				item.ImageIndex = version.ImageIndex;
				item.Tag = version;
				
				mainForm.ServerListView.Items.Add(item);
			}

			MarkCurrentVersion(mainForm.ServerListView, "Server");			
		}
		

		//----------------------------------------------------------
		// Returns the server name list
		public ArrayList GetServerList()
		{
			return mServerVersions;
		}

		//----------------------------------------------------------
		// returns a list of available client versions
		public ArrayList GetCompatibleBuilds(ListViewItem server)
		{
			try
			{
				VersionNode serverVersion = (VersionNode)server.Tag;
				ArrayList compatibleBuilds = new ArrayList();
				foreach(VersionNode version in mClientVersions)		// go through client list
				{
					// major versions must match!
					if(version.MajorVersion == serverVersion.MajorVersion)
					{
						//if(version.MinorVersion >= serverVersion.MinorVersion)
						{
							compatibleBuilds.Add( version );
						}
					}
				}

				return compatibleBuilds;
			}
			catch(Exception e)
			{
				MOG_Report.ReportMessage("Get Compatible Builds", e.Message, e.StackTrace, MOG.PROMPT.MOG_ALERT_LEVEL.ERROR);
				return new ArrayList();
			}
		}

		public void MarkCurrentVersion(ListView list, string type)
		{
			// Check our current versions version
			if (DosUtils.DirectoryExist(DeploymentDirectory + "\\" + type + "\\" +DeploymentTarget))
			{
				VersionNode current = this.CreateVersionNode(DeploymentTarget, DeploymentDirectory + "\\" + type + "\\" + DeploymentTarget);

				if (current != null)
				{
					//this.bCheckOverride = true;
					// Walk all the items of this list and check or bold the items that match
					foreach (ListViewItem item in list.Items)
					{
						// Get our version node from this item
						VersionNode target = (VersionNode)item.Tag;

						if (string.Compare(target.Name, current.Name, true) == 0 &&
							target.MajorVersion == current.MajorVersion &&
							target.MinorVersion == current.MinorVersion)
						{
							item.ForeColor = Color.DarkGreen;
							item.Font = new Font(item.Font.FontFamily, item.Font.Size, FontStyle.Bold);
							item.Checked = true;
						}						
					}

					//this.bCheckOverride = false;
				}
			}
		}

		//----------------------------------------------------------
		// DVC - loads a file
		static internal string LoadFileByLines(string filepath)
		{
			string data = "";

			try 
			{
				// Create an instance of StreamReader to read from a file.
				// The using statement also closes the StreamReader.
				using (StreamReader sr = new StreamReader(filepath)) 
				{
					String line;
					// Read and display lines from the file until the end of 
					// the file is reached.
					while ((line = sr.ReadLine()) != null) 
					{
						data = data + line + "\n";
					}
				}
			}
			catch (Exception e) 
			{
				// Let the user know what went wrong.
				//	Console.WriteLine("The file could not be read:");
				//	Console.WriteLine(e.Message);
				e.ToString();
				data = "";
			}

			return data;
		}

		/// <summary>
		/// Get all the files associated with this version and populate the info windows
		/// </summary>
		/// <param name="version"></param>
		public void GetFileList(VersionNode version)
		{
			try
			{
				mainForm.VersionFilesListView.Items.Clear();
				mainForm.VersionFilesListView.BeginUpdate();

				DirectoryInfo dir = new DirectoryInfo(version.SourceDirectory);
				foreach (FileInfo file in dir.GetFiles())
				{
					ListViewItem item = new ListViewItem(file.Name);
					item.SubItems.Add(file.LastWriteTime.ToShortDateString() + " " + file.LastWriteTime.ToShortTimeString());
					item.SubItems.Add(file.Length.ToString());
					item.ImageIndex = MogUtil_AssetIcons.GetFileIconIndex(file.FullName);

					mainForm.VersionFilesListView.Items.Add(item);
				}

				mainForm.VersionFilesListView.EndUpdate();
			}
			catch
			{
			}
		}	

		/// <summary>
		/// Perform the actual copies in the deployment process
		/// </summary>
		public void DeployCheckedItems(ListView list, string deploymentDirectory, string deploymentTarget)
		{
			List<VersionNode> versionNodes = new List<VersionNode>();
			foreach (ListViewItem item in list.CheckedItems)
			{
				versionNodes.Add(item.Tag as VersionNode);
			}

			List<object> args = new List<object>();
			args.Add(versionNodes);
			args.Add(deploymentDirectory);
			args.Add(deploymentTarget);

			ProgressDialog progress = new ProgressDialog("Deploying build", "Please wait while MOG Deploys the build...", DeployCheckedItems_Worker, args, true);
			progress.ShowDialog();
		}

		private void DeployCheckedItems_Worker(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<object> args = e.Argument as List<object>;
			List<VersionNode> versionNodes = args[0] as List<VersionNode>;
			string deploymentDirectory = args[1] as string;
			string deploymentTarget = args[2] as string;

			// We are going to attemp to copy all check clients to their respective deployment directories
			for (int i = 0; i < versionNodes.Count && !worker.CancellationPending; i++)
			{
				VersionNode target = versionNodes[i];

				// Set our current directory in the subDirectory of this versions type
				string deployTarget = target.Type + "\\" + deploymentTarget;

				// Clear out any previous deployment
				if (DosUtils.DirectoryExist(deploymentDirectory + "\\" + deployTarget))
				{
					worker.ReportProgress(i * 100 / versionNodes.Count, "Removing previous (" + target.Type + ") build");

					if (!DosUtils.DirectoryDelete(deploymentDirectory + "\\" + deployTarget))
					{
						throw new Exception(DosUtils.GetLastError());
					}
				}

				worker.ReportProgress(i * 100 / versionNodes.Count, "Copying new(" + target.Type + ") build named:" + target.Name);

				// Copy our new deployment
				if (!DosUtils.DirectoryCopyFast(target.SourceDirectory, deploymentDirectory + "\\" + deployTarget, true))
				{
					throw new Exception(DosUtils.GetLastError());
				}

				worker.ReportProgress(i * 100 / versionNodes.Count, target.Type + " named:" + target.Name + " has been deployed!");
			}
		}
	}

	//---------------------------------------------
	// node class to hold infos
	public class VersionNode
	{
		public string Name;
		public string FolderName;
		public string Description;
		public long MajorVersion;
		public long MinorVersion;
		public long ServerMajorVersion;
		public long ServerMinorversion;
		public int ImageIndex;
		public string SourceDirectory;
		public string Type;

		public string Version
		{
			get { return string.Format("{0}.{1}",MajorVersion,MinorVersion); }
		}

		public VersionNode(string name)
		{
			Name = name;
		}
	}
}

