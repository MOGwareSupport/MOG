using System;
using System.Windows.Forms;

using MOG;
using MOG.TIME;
using MOG.COMMAND;
using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;


namespace MOG_Client.Client_Mog_Utilities.AssetOptions
{
	/// <summary>
	/// Summary description for guiAssetSourceLock.
	/// </summary>
	public class guiAssetSourceLock
	{
		static public MOG_Command lockHolder;

		public guiAssetSourceLock()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static private DialogResult LockMessage(string message, MOG_Command desiredLockInfo, MOG_Command currentLockHolderInfo)
		{
			MOG_Time desiredLockTime = new MOG_Time();
			desiredLockTime.SetTimeStamp(desiredLockInfo.GetCommand().GetCommandTimeStamp());

			string desiredLockName = desiredLockInfo.GetAssetFilename().GetAssetFullName();

			// Check if this asset is a classification
			if (desiredLockName.Length == 0)
			{
				desiredLockName = desiredLockInfo.GetAssetFilename().GetOriginalFilename();
			}

			string desiredUser = desiredLockInfo.GetCommand().GetUserName().Length != 0 ? desiredLockInfo.GetCommand().GetUserName() : desiredLockInfo.GetCommand().GetComputerName();
            string desiredMachine = desiredLockInfo.GetCommand().GetComputerName();
			string desiredDescription = desiredLockInfo.GetCommand().GetDescription().Trim();

			// Current lock holder
			MOG_Time currentLockTime = new MOG_Time();
			currentLockTime.SetTimeStamp(currentLockHolderInfo.GetCommandTimeStamp());

			string currentLockName = currentLockHolderInfo.GetAssetFilename().GetAssetFullName();

			// Check if this asset is a classification
			if (currentLockName.Length == 0)
			{
				currentLockName = currentLockHolderInfo.GetAssetFilename().GetOriginalFilename();
			}

			string currentLockUser = currentLockHolderInfo.GetUserName().Length != 0 ? currentLockHolderInfo.GetUserName() : currentLockHolderInfo.GetComputerName();
			string currentLockMachine = currentLockHolderInfo.GetComputerName();
			string currentLockDescription = currentLockHolderInfo.GetDescription().Trim();


			return MessageBox.Show(message + "\n\n" + 
				desiredLockName + "\n" +
				"    USER: " + desiredUser + "\n" +
                "    MACHINE: " + desiredMachine + "\n" + 
				"    DESCRIPTION: " + desiredDescription + "\n" +
				"    TIME: " + desiredLockTime.FormatString("") + 

				"\n\n Colliding Lock:\n\n" +

				currentLockName + "\n" +
				"    USER: " + currentLockUser + "\n" +
				"    MACHINE: " + currentLockMachine + "\n" +
				"    DESCRIPTION: " + currentLockDescription + "\n" +
				"    TIME: " + currentLockTime.FormatString(""),

				"Asset Locked!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question);
		}

		static public bool IsLockedWithRetry(string assetFullName)
		{
			if (assetFullName.Length != 0)
			{
			TryGetLock:
				MOG_Command sourceLock = MOG_ControllerProject.PersistentLock_Query(assetFullName);
				
				lockHolder = sourceLock.GetCommand();
				// Now check if someone had a lock on this asset
				if (sourceLock.IsCompleted() &&
					lockHolder != null)
				{
					//The asset is locked
					switch (LockMessage("This asset is currently locked!", sourceLock, lockHolder))
					{
						case DialogResult.Retry:
							goto TryGetLock;
					}
					return true;
				}
			}
			return false;
		}

		static public bool RequestPersistentLockWithRetry(string assetFullName, string description)
		{
			if (!guiAssetSourceLock.IsLockedWithRetry(assetFullName))
			{
			SourceLockGet:
				MOG_Command sourceLock = MOG_ControllerProject.PersistentLock_Request(assetFullName, description);
				
				lockHolder = sourceLock.GetCommand();
				if (sourceLock.IsCompleted() &&
					lockHolder != null)
				if (!sourceLock.IsCompleted() ||
					lockHolder == null)
				{
					switch (LockMessage("Could not aquire this lock!", sourceLock, lockHolder))
					{
						case DialogResult.Retry:
							goto SourceLockGet;
					}
				}

				return sourceLock.IsCompleted();
			}

			return false;
		}

		static public bool ReleasePersistentLockWithRetry(string assetFullName)
		{
			SourceLockRelease:
				// Try to release the lock
				if (!MOG_ControllerProject.PersistentLock_Release(assetFullName))
				{
					// If we can't, tell us who has it
					MOG_Command sourceLock = MOG_ControllerProject.PersistentLock_Query(assetFullName);
					
					lockHolder = sourceLock.GetCommand();
					if (sourceLock.IsCompleted() &&
						lockHolder != null)
					{
						// Now check if someone had a lock on this asset
						switch (LockMessage("This asset is currently locked by another user", sourceLock, lockHolder))
						{
							case DialogResult.Retry:
								goto SourceLockRelease;
						}
						
						return false;
					}
				}
				
			return true;
		}
	}
}
