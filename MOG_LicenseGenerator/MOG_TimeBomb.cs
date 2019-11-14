using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using MOG;
using MOG.DOSUTILS;
using System.Management;

namespace MOG_LicenseGenerator
{

	public class MOG_TimeBomb
	{
		protected bool mIsLoaded;
		protected DateTime mInstallDate;
		protected DateTime mExpireDate;
		protected string mMacAddress;
		protected string mFilename;
		protected int mClientLicenseCount;
		protected string mDisabledFeatureList;

		public static string DisabledFeature_None = "|None|";
		public static string DisabledFeature_NoLicenseFile = "|NoLicenseFile|";

	
		public MOG_TimeBomb(string filename)
		{
			mIsLoaded = false;
			mMacAddress = "";
			mInstallDate = DateTime.MinValue;
			mExpireDate = DateTime.MinValue;
			mFilename = filename;
			mClientLicenseCount = 1;
			mDisabledFeatureList = DisabledFeature_NoLicenseFile;
		}

		public bool Load()
		{
			try
			{
				if (DosUtils.FileExistFast(mFilename))
				{
					string contents = DosUtils.FileRead(mFilename);
					if (contents.Length > 0)
					{
						contents = MOG_Encryption.Decrypt(contents);

						string[] parts = contents.Split("$".ToCharArray());

						if (parts.Length >= 5)
						{
							Int64 savedChecksum = Int64.Parse(parts[0]);
							mMacAddress = parts[1];
							mInstallDate = DateTime.Parse(parts[2]);
							mExpireDate = DateTime.Parse(parts[3]);
							mClientLicenseCount = Int32.Parse(parts[4]);
							if (parts.Length >= 6)
							{
								mDisabledFeatureList = parts[5];
							}

							//generate a new checksum and compare it with the one we saved out
							contents = contents.Substring(parts[0].Length + 1);
							Int64 checksum = 0;
							for (int i = 0; i < contents.Length; i++)
							{
								checksum += Convert.ToInt64(Char.GetNumericValue(contents[i]));
							}

							if (savedChecksum == checksum)
								mIsLoaded = true;
						}
					}
				}
			}
			catch
			{
				mIsLoaded = false;
			}

			return mIsLoaded;
		}

		public bool IsValid()
		{
			bool bValid = true;

			if (HasLicenseFile())
			{
				if (!IsValidMacAddress())
				{
					bValid = false;
				}
				else if (HasExpired())
				{
					bValid = false;
				}
			}
			else
			{
				bValid = false;
			}

			return bValid;
		}

		public bool HasLicenseFile()
		{
			// Make sure we are fully loaded before we proceed
			if (!mIsLoaded)
			{
				Load();
			}

			return mIsLoaded;
		}

		public bool HasExpired()
		{
			return HasExpired(DateTime.Now);
		}

		public bool HasExpired(DateTime testDate)
		{
			bool bHasExpired = false;

			// Check if we were fully loaded?
			if (mIsLoaded)
			{
				// Check if we have expired?
				if (mInstallDate > mExpireDate || testDate > mExpireDate)
				{
					bHasExpired = true;
				}
			}

			return bHasExpired;
		}

		public bool WillExpireSoon()
		{
			bool bWillExpireSoon = false;

			// Check if we were fully loaded?
			if (mIsLoaded)
			{
				// Check if we are going to expire soon?
				TimeSpan diff = mExpireDate.Subtract(DateTime.Now);
				if (diff.Days > 0 && diff.Days < 5)
				{
					bWillExpireSoon = true;
				}
			}

			return bWillExpireSoon;
		}

		public bool IsValidMacAddress()
		{
			bool bIsValidMacAddress = false;

			// Are we licensed?
			if (HasLicenseFile())
			{
				// Check if our MAC Address is valid?
				if (IsValidMacAddress(mMacAddress))
				{
					bIsValidMacAddress = true;
				}
			}

			return bIsValidMacAddress;
		}

		public int GetClientLicenseCount() { return mClientLicenseCount; }
		public string GetDisabledFeatureList() { return mDisabledFeatureList; }
		public string GetRegisteredMacAddress() { return mMacAddress; }
		public DateTime GetInstallDate() { return mInstallDate; }
		public DateTime GetExpireDate() { return mExpireDate; }

		protected bool IsValidMacAddress(string mac)
		{
			if (mac.Length > 0)
			{
				try
				{
					//00:00:00:00:00:00 is a valid address
					if (String.Compare(mac, "00:00:00:00:00:00", true) == 0)
					{
						return true;
					}

					ArrayList addresses = DetectLocalMacAddresses();
					for (int i = 0; i < addresses.Count; i++)
					{
						Object obj = addresses[i];
						if (obj != null && String.Compare(obj.ToString(), mac, true) == 0)
						{
							return true;
						}
					}
				}
				catch (Exception e)
				{
					e.ToString();
				}				
			}

			return false;
		}

		protected ArrayList DetectLocalMacAddresses()
		{
			ArrayList addresses = new ArrayList();

			try
			{
				ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
				ManagementObjectCollection objects = mc.GetInstances();

				ManagementObjectCollection.ManagementObjectEnumerator enumerator = objects.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ManagementObject mobj = (ManagementObject)(enumerator.Current);
					if (mobj != null)
					{
						Object obj = mobj["MacAddress"];
						if (obj != null)
						{
							addresses.Add(obj.ToString());
						}
					}
				}
			}
			catch (Exception e)
			{
				e.ToString();
			}

			return addresses;
		}
	}			
}

	