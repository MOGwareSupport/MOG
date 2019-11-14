using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace MOG_Server
{
	public class MOG_Encryption
	{
		public static string Encrypt(string text)
		{
			return Encrypt(text, "*&$%#@!)");
		}

		public static string Encrypt(string text, string encodeKey)
		{
			Byte[] byKey;
			Byte[] IV = { (byte)'a', (byte)'a', (byte)'a', (byte)'a', (byte)'a', (byte)'a', (byte)'a', (byte)'a' };
			
			try
			{
				// Get our encode string
				if (encodeKey.Length > 8)
				{
					encodeKey = encodeKey.Substring(0, 8);
				}
				UTF8Encoding utf8 = new UTF8Encoding();
				byKey = utf8.GetBytes(encodeKey);

				// Prepare our string for encoding
				Byte[] inputByteArray = utf8.GetBytes(text);

				// Create our encoder
				DESCryptoServiceProvider des = new DESCryptoServiceProvider();
				MemoryStream ms = new MemoryStream();
				CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);

				// Encode the string
				cs.Write(inputByteArray, 0, inputByteArray.Length);
				cs.FlushFinalBlock();

				// Return the encoded string
				return Convert.ToBase64String(ms.ToArray());
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}

		public static string Decrypt(string text)
		{
			return Decrypt(text, "*&$%#@!)");
		}

		public static string Decrypt(string text, string decodeKey)
		{
			Byte[] byKey;
			Byte[] IV = { (byte)'a', (byte)'a', (byte)'a', (byte)'a', (byte)'a', (byte)'a', (byte)'a', (byte)'a' };
			
			try
			{
				// Get our 8 character key
				if (decodeKey.Length > 8)
				{
					decodeKey = decodeKey.Substring(0, 8);
				}
				UTF8Encoding utf8 = new UTF8Encoding();
				byKey = utf8.GetBytes(decodeKey);

				// Prepate our encoded string
				Byte[] inputByteArray = Convert.FromBase64String(text);

				// Create our decoders
				DESCryptoServiceProvider des = new DESCryptoServiceProvider();
				MemoryStream ms = new MemoryStream();
				CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);

				// Decode the string
				cs.Write(inputByteArray, 0, inputByteArray.Length);
				cs.FlushFinalBlock();

				// Return the string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(ms.ToArray());
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}
	}
}
