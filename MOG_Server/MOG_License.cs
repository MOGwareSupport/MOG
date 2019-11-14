using System;
using System.Collections.Generic;
using System.Text;
using MOG.COMMAND;
using System.Collections;
using MOG.TIME;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG;

namespace MOG_Server
{
	public class MOG_License
	{
		MOG_TimeBomb mTimeBomb;

		public MOG_License()
		{
			mTimeBomb = new MOG_TimeBomb(String.Concat(MOG_Main.GetExecutablePath_StripCurrentDirectory(), "\\mog_license"));
		}


		public bool EnsureValidLicense()
		{
			bool bRenewLicense = false;

			// Check if this license has expired?  or
			// Check if this license will expire soon?  or
			// Check if this license is out of seats?
			if (mTimeBomb.HasExpired() ||
				mTimeBomb.WillExpireSoon() ||
				!IsLicenseAvailable())
			{
				// Unload this license so we can attempt to renew
				bRenewLicense = true;
			}

			if (bRenewLicense)
			{
				// Always attempt to renew our license file
				if (mTimeBomb.RenewMogwareLicense())
				{
					// Reload the license in hopes it will be better now
					mTimeBomb.Load();
				}
			}

			return mTimeBomb.HasLicenseFile();
		}

		bool IsLicenseAvailable()
		{
			if (GetAllCurrentLicenses().Count < GetMaxLicenses())
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		internal bool ConfirmLicense(MOG_Command pCommand)
		{
			bool bLicense = false;

			// Check if we are a client?
			MOG_CommandServerCS commandServer = (MOG_CommandServerCS)(MOG_ControllerSystem.GetCommandManager());
			if (commandServer != null)
			{
				// Check if this client is already registered?
				if (commandServer.LocateClientByID(pCommand.GetNetworkID()) != null)
				{
					// JohnRen - Bummer, we can't check this because they may have just expired and now be legitamately running on the 4 connections
					//			// Now double check the date of this command to ensure it hasn't expired
					//			DateTime commandDate = MOG_Time.GetDateTimeFromTimeStamp(pCommand.GetCommandTimeStamp());
					//			if (!mTimeBomb.HasExpired(commandDate))
					//			{
					// Indicate this client is licensed
					bLicense = true;
					//			}
				}
				else
				{
					// Let's try to obtain a license
					if (CanObtainLicense(pCommand))
					{
						// Indicate this client got a license
						bLicense = true;
					}
				}
			}

			return bLicense;
		}

		bool CanObtainLicense(MOG_Command pCommand)
		{
			bool bLicense = false;

			// Get our server command manager?
			MOG_CommandServerCS commandServer = (MOG_CommandServerCS)(MOG_ControllerSystem.GetCommandManager());
			if (commandServer != null)
			{
				// This client is not accounted for, check the license file to see if he can fit into the current situation
				if (EnsureValidLicense())
				{
					// We have a license file
					if (!mTimeBomb.IsValidMacAddress())
					{
						//The Mac address is bad
						String message = String.Concat( "This MOG Server's machine hasn't been properly licensed.\n",
														"You will be limited to 2 client connections.\n",
														"Contact Mogware to transfer this license." );

						MOG_Command pBroadcast = MOG_CommandFactory.Setup_NetworkBroadcast("", message);
						commandServer.SendToConnection(pCommand.GetNetworkID(), pBroadcast);
					}
					else
					{
						// Since the Mac address is ok, what about the expiration?
						DateTime commandDate = MOG_Time.GetDateTimeFromTimeStamp(pCommand.GetCommandTimeStamp());
						if (mTimeBomb.HasExpired() ||
							mTimeBomb.HasExpired(commandDate))
						{
							//Oh man this license file is expired
							String message = String.Concat( "The MOG Server has been unable to renew the MOG License.\n",
															"You will be limited to 2 client connections.\n",
															"Please make sure the MOG Server machine has internet access." );

							MOG_Command pBroadcast = MOG_CommandFactory.Setup_NetworkBroadcast("", message);
							commandServer.SendToConnection(pCommand.GetNetworkID(), pBroadcast);
						}
						// Check if we are going to expire soon?
						else if (mTimeBomb.WillExpireSoon())
						{
							// Inform the user of our impending doom
							String message = String.Concat( "The MOG Server has been unable to renew the MOG License.\n",
															"EXPIRATION DATE: ", mTimeBomb.GetExpireDate().ToString(), "\n",
															"Please make sure the MOG Server machine has internet access." );
							MOG_Command pBroadcast = MOG_CommandFactory.Setup_NetworkBroadcast("", message);
							commandServer.SendToConnection(pCommand.GetNetworkID(), pBroadcast);
						}
					}
				}
				else
				{
					// There is no license file at all
					String message = String.Concat( "You're using an unlicensed server.\n",
													"You will be limited to 2 client connections.\n",
													"Licensing can be performed in the ServerManager.");

					MOG_Command pBroadcast = MOG_CommandFactory.Setup_NetworkBroadcast("", message);
					commandServer.SendToConnection(pCommand.GetNetworkID(), pBroadcast);
				}

				// Call this to see if any more clients can be licensed
				if (IsLicenseAvailable())
				{
					bLicense = true;
				}
				else
				{
					// Notify client of failed command request
					string message = String.Concat( "No more available licenses on the server.\n\n",
													"This client cannot be launched until another client is closed.\n",
													"Additional seats can be licensed in the ServerManager.");
					MOG_Command pBroadcast = MOG_CommandFactory.Setup_NetworkBroadcast("", message);
					commandServer.SendToConnection(pCommand.GetNetworkID(), pBroadcast);
				}
			}

			return bLicense;
		}


		ArrayList GetAllCurrentLicenses()
		{
			// Check if we are a client?
			MOG_CommandServerCS commandServer = (MOG_CommandServerCS)(MOG_ControllerSystem.GetCommandManager());
			if (commandServer != null)
			{
				return commandServer.GetRegisteredClients();
			}

			return null;
		}


		public int GetMaxLicenses()
		{
			// Verify our TimeBomb
			if (mTimeBomb != null && mTimeBomb.IsValid())
			{
				return mTimeBomb.GetClientLicenseCount();
			}
			else
			{
				return 2;
			}
		}


		internal string GetDisabledFeatureList()
		{
			if (mTimeBomb != null)
			{
				return mTimeBomb.GetDisabledFeatureList();
			}
			else
			{
				return MOG_TimeBomb.DisabledFeature_NoLicenseFile;
			}
		}
	}
}
