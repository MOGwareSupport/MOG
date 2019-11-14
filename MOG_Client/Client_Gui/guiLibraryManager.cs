using System;

using MOG.COMMAND;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG_Client.Client_Utilities;

namespace MOG_Client.Client_Gui
{
	/// <summary>
	/// Summary description for guiLibraryManager.
	/// </summary>
	public class guiLibraryManager
	{
		private MogMainForm mainForm;
		private bool mInitialized = false;
		private bool mLoaded = false;

		public guiLibraryManager(MogMainForm main)
		{
			mainForm = main;
			mInitialized = false;
		}

		public void Initialize()
		{
			mainForm.LibraryExplorer.Initialize(OnWorkerCompleteLoadLibraryExplorer);
			mInitialized = true;
		}

		private void OnWorkerCompleteLoadLibraryExplorer()
		{
			if (!mLoaded)
			{
				guiUserPrefs.LoadDynamic_LayoutPrefs("LibraryManager", mainForm);
				mLoaded = true;
			}
		}

		public void Refresh()
		{
			if (mInitialized)
			{
				//Refresh everything
				mainForm.LibraryExplorer.Refresh();
			}
			else if (MOG_ControllerProject.IsProject() && MOG_ControllerProject.IsUser())
			{
				Initialize();
			}
		}

		public void RefreshItem(MOG_Command command)
		{
			//Refresh just one item
			mainForm.LibraryExplorer.RefreshItem(command);
		}		
	}
}
