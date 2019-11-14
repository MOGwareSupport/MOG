using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using MOG;
using MOG.INI;
using MOG.TIME;
using MOG.DOSUTILS;

//test 

namespace MOG_Post
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class MOG_Post
	{
		private bool mVerbose			= false;
		public string mBuildType		= "Release";
		public string mSourceBinaries	= "Updates";
		public string mArchiveVersion	= "1";
		
		private string mClientLabel = "Client";
		private string mClientMajorVersion = "3";
		private string mClientMinorVersion = "1";

		private string mServerLabel = "Server";
		private string mServerMajorVersion = "3";
		private string mServerMinorVersion = "1";

		#region Version creation stuff
		public void PopulateVersions()
		{
			if (DosUtils.FileExist(MOG.MOG_Main.GetExecutablePath() + "\\PostVersions.info"))
			{
				MOG_Ini versions = new MOG_Ini(MOG.MOG_Main.GetExecutablePath() + "\\PostVersions.info");

				// Client
				if (versions.SectionExist("Client"))
				{
					if (versions.KeyExist("Client", "MajorVersion"))
					{
						mClientMajorVersion = versions.GetString("Client", "MajorVersion");						
					}

					if (versions.KeyExist("Client", "MinorVersion"))
					{
						mClientMinorVersion = versions.GetString("Client", "MinorVersion");						
					}

					if (versions.KeyExist("Client", "Name"))
					{
						mClientLabel = versions.GetString("Client", "Name");						
					}
				}

				// Server
				if (versions.SectionExist("Server"))
				{
					if (versions.KeyExist("Server", "MajorVersion"))
					{
						mServerMajorVersion = versions.GetString("Client", "MajorVersion");						
					}

					if (versions.KeyExist("Server", "MinorVersion"))
					{
						mServerMinorVersion = versions.GetString("Client", "MinorVersion");						
					}

					if (versions.KeyExist("Server", "Name"))
					{
						mServerLabel = versions.GetString("Client", "Name");						
					}
				}
			}

			if (mClientMajorVersion.ToLower() == "<auto>") mClientMajorVersion = GetAutoVersion();
			if (mClientMinorVersion.ToLower() == "<auto>") mClientMinorVersion = GetAutoVersion();
			if (mServerMajorVersion.ToLower() == "<auto>") mServerMajorVersion = GetAutoVersion();			
			if (mServerMinorVersion.ToLower() == "<auto>") mServerMinorVersion = GetAutoVersion();
		}

		private string GetAutoVersion()
		{
			return MOG_Time.GetVersionTimestamp();
		}

		#endregion

		private string DisplayMenu()
		{
			Console.WriteLine("----------------------------------------");
			Console.WriteLine("MOG 1.8 Downloadable Version Updater:");
			Console.WriteLine("	BuildType:" + mBuildType);
			Console.WriteLine("	TargetFolder:" + mArchiveVersion);
			Console.WriteLine("----------------------------------------");
			
			Console.WriteLine("1 - Post Client");
			Console.WriteLine("2 - Post Server");
			Console.WriteLine("3 - Post MOG Bridge");
			Console.WriteLine("4 - Post All");
			Console.WriteLine("5 - Toggle Debug/Release");
			Console.WriteLine("6 - Post for MOG_Tools");
			Console.WriteLine("7 - Post Current MOG_Tools");
			Console.WriteLine("0 - Exit");

			Console.Write("Enter Selection:");
			return Console.ReadLine();
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            Control.CheckForIllegalCrossThreadCalls = false;

            MOG_Post poster = new MOG_Post();
			poster.PopulateVersions();

			MOG_Time current = new MOG_Time();
			poster.mArchiveVersion = current.FormatString("{month.2}.{day.2}.{year.2} {hour.2}.{minute.2}.{second.2} {AMPM}");

			string selection = "";
			while(selection != "0")
			{
				selection = poster.DisplayMenu();
			
				switch(selection)
				{
					case "1":	// Client
						poster.PostClient();
						break;
					case "2":	// Server
						poster.PostServer();
						break;
					case "3":	// Bridge
						poster.PostBridge();
						break;
					case "4":	// All
						poster.PostClient();
						poster.PostBridge();
						poster.PostServer();
						break;
					case "5":	// Toggle type
						if (poster.mBuildType == "Release") poster.mBuildType = "Debug";
						else poster.mBuildType = "Release";
						break;
					case "6":
						poster.PostToMOGTools();
						break;
					case "7":
						break;
					case "0":
						break;
				}
			}
			return;
		}

		private void PostToMOGTools()
		{
			MogToolsMenu:
				Console.WriteLine("----------------------------------------");
				Console.WriteLine("1) Server");
				Console.WriteLine("2) Client");
				Console.WriteLine("3) All");
				Console.WriteLine("----------------------------------------");

				Console.WriteLine("Enter Selection:");
				string choice = Console.ReadLine();
	            			
			GetAccount:
				Console.WriteLine("Enter Target MOGtools account:");
				string target = Console.ReadLine();
				if (target.Length == 0)
					goto GetAccount;

			mSourceBinaries = "M:\\MOG\\MOGTools\\" + target;

			string clientSource, serverSource;
			string output = "";

			string arg1;
			string arg2;

			switch (choice)
			{
				case "1":		
					serverSource = PostServer();
					arg1 = "\"" + mSourceBinaries + "\\" + "Server_" + mArchiveVersion + ".rar\"";
					arg2 = "\"" + serverSource + "\\*.* \" -ep";
					
					this.Shell(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\WinRar\\WinRar", 
					" a " + arg1 + " " + arg2 , ProcessWindowStyle.Normal, ref output, true);
					break;
				case "2":
					clientSource = PostClient();
					arg1 = "\"" + mSourceBinaries + "\\" + "Client_" + mArchiveVersion + ".rar \"";
					arg2 = "\"" + clientSource + "\\*.* \" -ep";
					
					this.Shell(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\WinRar\\WinRar", 
						" a " + arg1 + " " + arg2 , ProcessWindowStyle.Normal, ref output, true);
					break;
				case "3":
					serverSource = PostServer();
					clientSource = PostClient();

					arg1 = "\"" + mSourceBinaries + "\\" + "Server_" + mArchiveVersion + ".rar\"";
					arg2 = "\"" + serverSource + "\\*.* \" -ep";
					
					this.Shell(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\WinRar\\WinRar", 
						" a " + arg1 + " " + arg2 , ProcessWindowStyle.Normal, ref output, true);


					arg1 = "\"" + mSourceBinaries + "\\" + "Client_" + mArchiveVersion + ".rar \"";
					arg2 = "\"" + clientSource + "\\*.* \" -ep";
					
					this.Shell(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\WinRar\\WinRar", 
						" a " + arg1 + " " + arg2 , ProcessWindowStyle.Normal, ref output, true);
					break;
				default:
					goto MogToolsMenu;
			}
		}

		private string PostClient()
		{
			MOG_PropertiesIni clientFiles = new MOG_PropertiesIni();

			clientFiles.PutFile("client.info");
			clientFiles.PutFile("Slave.info");
			clientFiles.PutFile("Commandline.info");

			MOG_PropertiesIni files = Post("Client", clientFiles);
			PostVersionFile("Client");
			PostWhatsNew("Client");
			PostFileList("Client", "MOG_Client.exe", files);

			return mSourceBinaries + "\\" + "Client" + "\\" + mArchiveVersion;
		}

		private string PostServer()
		{
			MOG_PropertiesIni serverFiles = new MOG_PropertiesIni();

			serverFiles.PutFile("server.info");

			MOG_PropertiesIni sfiles = Post("Server", serverFiles);
			PostVersionFile("Server");
			PostWhatsNew("Server");
			PostFileList("Server", "MOG_Server.exe", sfiles);

			return mSourceBinaries + "\\" + "Server" + "\\" + mArchiveVersion;
		}

		private string PostBridge()
		{
			MOG_PropertiesIni Files = new MOG_PropertiesIni();

			Files.PutFile("bridge.info");

			MOG_PropertiesIni files = Post("MOGBridge", Files);
			PostVersionFile("MOGBridge");
			PostWhatsNew("MOGBridge");
			PostFileList("MOGBridge", "MOG_Bridge.dll", files);

			return mSourceBinaries + "\\" + "MOG_Bridge.dll" + "\\" + mArchiveVersion;
		}

		#region Work Routines
		private MOG_PropertiesIni Post(string targetFolder, MOG_PropertiesIni fileList)
		{
			bool title = false;
			MOG_PropertiesIni copiedFiles = new MOG_PropertiesIni();
			string fullTargetPath = mSourceBinaries + "\\" + targetFolder + "\\" + mArchiveVersion;

			Console.WriteLine("----------------------------------------");
			Console.WriteLine("Create " + targetFolder + " VERSION: " + mArchiveVersion);

			// Make our target directory
			DosUtils.DirectoryCreate(fullTargetPath);

			Console.WriteLine("---------------------------");
			Console.WriteLine("Posting new " + targetFolder + " version");
			Console.WriteLine("---------------------------");

			if (fileList.SectionExist("Files"))
			{
				for (int i = 0; i < fileList.CountKeys("Files"); i++)
				{
					string label = fileList.GetKeyByIndexSLOW("Files", i);
					string filename = fileList.GetKeyNameByIndexSLOW("Files", i);
					string path = MOG_Tokens.GetFormattedString(Path.GetDirectoryName(filename), "{BuildType}=" + mBuildType);
					string file = Path.GetFileName(filename);					

					if (!title && mVerbose)
					{
						Console.WriteLine(label);
						title = true;
					}

					// Copy the files
					foreach (FileInfo fileHandle in DosUtils.FileGetList(path, file))
					{
						if (!title && !mVerbose)
						{
							Console.WriteLine(label);
							title = true;
						}
						
						try
						{
							fileHandle.CopyTo(fullTargetPath + "\\" + fileHandle.Name, true);
							copiedFiles.PutSectionString("FILES", fullTargetPath + "\\" + fileHandle.Name);
							Console.WriteLine("	POSTED: " + 
								"<" +
								fileHandle.LastWriteTime.ToShortDateString().ToString() + 
								" " +
								fileHandle.LastWriteTime.ToShortTimeString().ToString() + 
								">\t" +
								fileHandle.Name);
						}
						catch(Exception e)
						{
							Console.WriteLine("	ERROR POSTING: " + fileHandle.Name);
							Console.WriteLine("		ERROR MESSAGE: " + e.Message);
						}
					}

					title = false;
					Console.WriteLine("");
				}
			}

			return copiedFiles;
		}
				
		/// <summary>
		/// Write version info file
		/// </summary>
		/// <param name="targetFolder"></param>
		/// <param name="fileList"></param>
		private void PostVersionFile(string targetFolder)
		{
			string versionFilename = mSourceBinaries + "\\" + targetFolder + "\\" + mArchiveVersion + "\\VERSION.INI";
			FileStream versionFile = new FileStream(versionFilename, FileMode.CreateNew, FileAccess.Write);
			
			if (versionFile != null)
			{
				StreamWriter versionFileWriter = new StreamWriter(versionFile);
				if (versionFileWriter != null)
				{
					versionFileWriter.WriteLine("[INFO]");

					versionFileWriter.WriteLine("Name=" + mClientLabel);
					versionFileWriter.WriteLine("MajorVersion=" + mClientMajorVersion);
					versionFileWriter.WriteLine("MinorVersion=" + mClientMinorVersion);
					versionFileWriter.WriteLine("Type=" + targetFolder);

					versionFileWriter.WriteLine("");

					versionFileWriter.WriteLine("[SERVERCOMPATABILITY]");
					versionFileWriter.WriteLine("ServerMajorVersion=" + mServerMajorVersion);
					versionFileWriter.WriteLine("ServerMinorVersion=" + mServerMinorVersion);

					versionFileWriter.Close();
				}
				versionFile.Close();
			}
		}
	
		private void PostWhatsNew(string targetFolder)
		{
			string versionFilename = mSourceBinaries + "\\" + targetFolder + "\\" + mArchiveVersion + "\\WhatsNew.txt";

			WhatsNewLogForm log = new WhatsNewLogForm();

			MOG_Time now = new MOG_Time();
			log.WhatsNewRichTextBox.RichTextBox.Text = "------------------------------------------------\n" + now.FormatString("") + "\n------------------------------------------------\n";
			
			log.WhatsNewRichTextBox.RichTextBox.Select(log.WhatsNewRichTextBox.RichTextBox.Text.Length, 1);
			log.TopMost = true;
			log.ShowDialog();			
                        
			DosUtils.AppendTextToFile(versionFilename, log.WhatsNewRichTextBox.RichTextBox.Text.Replace("\n","\r\n"));
			log.WhatsNewRichTextBox.RichTextBox.SaveFile(mSourceBinaries + "\\" + targetFolder + "\\" + mArchiveVersion + "\\WhatsNew.rtf", System.Windows.Forms.RichTextBoxStreamType.RichText);
		}

		private void PostFileList(string targetFolder, string executable, MOG_PropertiesIni files)
		{
			string fileListName = mSourceBinaries + "\\" + targetFolder + "\\" + mArchiveVersion + "\\FileList.ini";
			StreamWriter fileList = new StreamWriter(fileListName);
			if (fileList != null)
			{
				fileList.WriteLine("[" + targetFolder + "]");
				fileList.WriteLine("exe=" + executable);

				fileList.WriteLine("");

				fileList.WriteLine("[FILES]");
				if (files.SectionExist("FILES"))
				{
					for (int i = 0; i < files.CountKeys("FILES"); i++)
					{
						string file = files.GetKeyNameByIndexSLOW("FILES", i);
						fileList.WriteLine(file);
					}
				}

				fileList.Close();
			}
		}

		public int Shell(string command, string arguments, ProcessWindowStyle window, ref string output, bool WaitForExit)
		{
			int rc = -1;
			// Run the command
			Process p = new Process();
			p.StartInfo.FileName = command;
			p.StartInfo.Arguments = arguments;
			p.StartInfo.WorkingDirectory = Path.GetDirectoryName(command);

			p.StartInfo.WindowStyle = window;

			if (window == ProcessWindowStyle.Hidden)
			{
				p.StartInfo.CreateNoWindow = true;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.UseShellExecute = false;
			}


			try
			{
				p.Start();
			}
			catch(Exception e)
			{
				MessageBox.Show("Command:" + p.StartInfo.FileName + "\n" +
					"Args:" + p.StartInfo.Arguments + "\n" +
					e.Message, "ShellExecute", MessageBoxButtons.OK);
				return -1;
			}

			if (window == ProcessWindowStyle.Hidden)
			{
				output = string.Concat("StdOut", p.StandardOutput.ReadToEnd());
				output = string.Concat(output, " StdError:", p.StandardError.ReadToEnd());
				rc = p.ExitCode;
			}


			try
			{
				if (WaitForExit)
				{
					p.WaitForExit();
				}
				rc = p.ExitCode;
				p.Close();
			}
			catch
			{
				p.Close();
			}


			return rc;
		}
		#endregion
	}
}
