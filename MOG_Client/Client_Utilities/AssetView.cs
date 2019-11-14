using System;
using System.Diagnostics;
using System.Windows.Forms;

using MOG;
using MOG.COMMAND;
using MOG.FILENAME;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG_Client.Client_Mog_Utilities;



namespace MOG_Client.Client_Utilities
{
	/// <summary>
	/// Summary description for AssetView.
	/// </summary>
	public class AssetView
	{
		private string mViewer;
		private string mBinary;
		private MOG_Filename mAsset;

		public string Viewer {get{return mViewer;} set {mViewer = value;}}
		public string Binary {get{return mBinary;} set {mBinary = value;}}
		public MOG_Filename Asset {get{return mAsset;} set {mAsset = value;}}

		public AssetView()
		{
		}
		public void ShellSpawnWithLock()
		{
			if (mBinary == null || mAsset == null)
			{
				MOG_Prompt.PromptMessage("Spawn Viewer Error!", "One of the following was not initialized: Viewer, Binary, Asset", Environment.StackTrace);
				return;
			}
			else
			{
				MOG_Command command = new MOG_Command();
			
				// Get Asset Lock
				command = MOG.COMMAND.MOG_CommandFactory.Setup_LockReadRequest(mAsset.GetOriginalFilename(), "Asset View - Open Asset");
				if (MOG_ControllerSystem.GetCommandManager().CommandProcess(command))
				{
					string output = "";
					if (mViewer != null && mViewer.Length == 0)
					{
						guiCommandLine.ShellExecute(mBinary);
					}
					else
					{
						guiCommandLine.ShellExecute(mViewer, mBinary, ProcessWindowStyle.Normal, ref output);
					}

					command = MOG.COMMAND.MOG_CommandFactory.Setup_LockReadRelease(mAsset.GetOriginalFilename());
					MOG_ControllerSystem.GetCommandManager().CommandProcess(command);
				}
			}
		}
	}
}
