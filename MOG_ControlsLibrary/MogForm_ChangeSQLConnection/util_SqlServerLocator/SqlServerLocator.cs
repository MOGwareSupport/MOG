using System;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using MOG.PROMPT;

namespace MOG_ControlsLibrary.Forms
{
	/// <summary>
	/// Summary description for SqlServerLocator.
	/// </summary>
	public class SqlServerLocator
	{
		#region Member vars

		public static bool IntegratedSecurity = true;
		public static string UserId = "";
		public static string Password = "";
		public static int TimeOut = 1;

		#endregion

		#region DLL Imports
		[DllImport("odbc32.dll")]
		private static extern short SQLAllocHandle(short hType, IntPtr inputHandle, out IntPtr outputHandle);
		[DllImport("odbc32.dll")]
		private static extern short SQLSetEnvAttr(IntPtr henv, int attribute, IntPtr valuePtr, int strLength);
		[DllImport("odbc32.dll")]
		private static extern short SQLFreeHandle(short hType, IntPtr handle); 
		[DllImport("odbc32.dll",CharSet=CharSet.Ansi)]
		private static extern short SQLBrowseConnect(IntPtr hconn, StringBuilder inString, 
			short inStringLength, StringBuilder outString, short outStringLength,
			out short outLengthNeeded);
		#endregion

		#region Constants
		private const short SQL_HANDLE_ENV = 1;
		private const short SQL_HANDLE_DBC = 2;
		private const int SQL_ATTR_ODBC_VERSION = 200;
		private const int SQL_OV_ODBC3 = 3;
		private const short SQL_SUCCESS = 0;
		
		private const short SQL_NEED_DATA = 99;
		private const short DEFAULT_RESULT_SIZE = 1024;
		private const string SQL_DRIVER_STR = "DRIVER=SQL SERVER";
		#endregion

		#region Functions
		public static bool ConnectionValid(string serverName)
		{
			try
			{
				OleDbConnection myConnection = GetConnection(serverName);
				myConnection.Open();
				myConnection.Close();
			}
			catch
			{
				return false;
			}

			return true;
		}

		public static ArrayList FindDatabases(string serverName)
		{
			ArrayList databases = new ArrayList();

			try
			{
				TimeOut = 1;
				OleDbConnection myConnection = GetConnection(serverName);
				myConnection.Open();
				DataTable schemaTable = myConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Catalogs,
					null);
				
				myConnection.Close();
				foreach (DataRow dr in schemaTable.Rows)
				{
					databases.Add(dr[0] as string);
				}
			}
			catch
			{
				MOG_Prompt.PromptResponse("Open SQL Connection Error!", "Couldn't access server " + serverName + "\nMake sure that this is not an instanced server that would require the instance name.\ni.e MyServer\\InstanceName", "", MOGPromptButtons.OK, MOG_ALERT_LEVEL.ERROR);
			}


			return databases;
		}

		private static OleDbConnection GetConnection(string serverName)
		{
//			string connString = IntegratedSecurity ?
//				String.Format("Provider=SQLOLEDB;Data Source={0};Integrated Security=SSPI;Connect Timeout={1}", serverName, TimeOut)
//				: String.Format("Provider=SQLOLEDB;Data Source={0};User Id={1};Password={2};Connect Timeout={3}",
//				serverName, UserId, Password, TimeOut);

			string connString = IntegratedSecurity ?
				String.Format("Provider=SQLOLEDB;Data Source={0};Integrated Security=SSPI;Connect Timeout=1", serverName)
				: String.Format("Provider=SQLOLEDB;Data Source={0};User Id={1};Password={2};Connect Timeout=1",
				serverName, UserId, Password);


			return new OleDbConnection(connString);
		}

		public static ArrayList FindServers()
		{
			ArrayList retval = null;
			string txt = string.Empty;
			IntPtr henv = IntPtr.Zero;
			IntPtr hconn = IntPtr.Zero;
			StringBuilder inString = new StringBuilder(SQL_DRIVER_STR);
			StringBuilder outString = new StringBuilder(DEFAULT_RESULT_SIZE);
			short inStringLength = (short) inString.Length;
			short lenNeeded = 0;

			try
			{
				if (SQL_SUCCESS == SQLAllocHandle(SQL_HANDLE_ENV, henv, out henv))
				{
					if (SQL_SUCCESS == SQLSetEnvAttr(henv,SQL_ATTR_ODBC_VERSION,(IntPtr)SQL_OV_ODBC3,0))
					{
						if (SQL_SUCCESS == SQLAllocHandle(SQL_HANDLE_DBC, henv, out hconn))
						{
							if (SQL_NEED_DATA ==  SQLBrowseConnect(hconn, inString, inStringLength, outString, 
								DEFAULT_RESULT_SIZE, out lenNeeded))
							{
								if (DEFAULT_RESULT_SIZE < lenNeeded)
								{
									outString.Capacity = lenNeeded;
									if (SQL_NEED_DATA != SQLBrowseConnect(hconn, inString, inStringLength, outString, 
										lenNeeded,out lenNeeded))
									{
										throw new ApplicationException("Unabled to aquire SQL Servers from ODBC driver.");
									}	
								}
								txt = outString.ToString();
								int start = txt.IndexOf("{") + 1;
								int len = txt.IndexOf("}") - start;
								if ((start > 0) && (len > 0))
								{
									txt = txt.Substring(start,len);
								}
								else
								{
									txt = string.Empty;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				//Throw away any error if we are not in debug mode
				string message = ex.Message;
#if (DEBUG)
				MessageBox.Show(message,"Acquire SQL Servier List Error");
#endif 
				txt = string.Empty;
			}
			finally
			{
				if (hconn != IntPtr.Zero)
				{
					SQLFreeHandle(SQL_HANDLE_DBC,hconn);
				}
				if (henv != IntPtr.Zero)
				{
					SQLFreeHandle(SQL_HANDLE_ENV,hconn);
				}
			}
	
			if (txt.Length > 0)
			{
				retval = new ArrayList(txt.Split(",".ToCharArray()));
			}

			return retval;
		}
	
		#endregion
	}

}
