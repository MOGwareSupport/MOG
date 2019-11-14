using System;
using System.Collections.Generic;
using System.Text;
using MOG.DOSUTILS;
using MOG;

namespace MOG_LicenseGenerator
{
	public class Generator
	{
		public static bool Generate(string filename, string macAddress, int days, int clientLicenseCount)
		{
			return Generate(filename, macAddress, DateTime.Now.AddDays(days), clientLicenseCount);
		}

		public static bool Generate(string filename, string macAddress, DateTime expiration, int clientLicenseCount)
		{
			return Generate(filename, macAddress, expiration, clientLicenseCount, MOG_TimeBomb.DisabledFeature_None);
		}

		public static bool Generate(string filename, string macAddress, DateTime expiration, int clientLicenseCount, string disabledFeatureList)
		{
			DateTime installDate = DateTime.Now;

			string output = String.Concat(macAddress, "$", installDate.ToString(), "$", expiration.ToString(), "$", clientLicenseCount.ToString(), "$", disabledFeatureList, "$");
			
			Random rand = new Random(DateTime.Now.Second);
			for (int i = 0; i < 1024; i++)
			{
				char ch = (char)rand.Next('0', '9');
				output = String.Concat(output, ch);
			}

			Int64 checksum = 0;
			for (int i = 0; i < output.Length; i++)
			{
				checksum += Convert.ToInt64(Char.GetNumericValue(output[i]));
			}

			output = String.Concat(checksum.ToString(), "$", output);

			output = MOG_Encryption.Encrypt(output);
			return DosUtils.FileWrite(filename, output);
		}
	}
}
