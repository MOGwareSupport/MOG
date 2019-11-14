using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;

using MOG_Client.Forms;
using MOG_Client.Client_Utilities;
using MOG_Client.Client_Mog_Utilities;

using MOG_ControlsLibrary;
using MOG_ControlsLibrary.Utils;
using MOG_ControlsLibrary.Common.MogControl_RepositoryTreeViews;

using MOG;
using MOG.INI;
using MOG.TIME;
using MOG.REPORT;
using MOG.FILENAME;
using MOG.DOSUTILS;
using MOG.CONTROLLER.CONTROLLERPROJECT;

namespace MOG_Client.Client_Mog_Utilities.AssetOptions
{
	/// <summary>
	/// Summary description for guiAssetCustomToolsMenu.
	/// </summary>
	public class guiAssetCustomToolsMenu
	{
		public delegate void MogMenuItem_Click(object sender, System.EventArgs e);
		ArrayList mClickHandlers;

		private ListView mView;
		private TreeView mTree;
		private string mCustomToolsInfo;
		public string []mColumns;
		private MogMainForm mainForm;

		// TODO We might want this for remote users
		//private MOG_Info mCustomTools;

		public ListView SetView { get{return mView;} set{mView = value; mTree = null;}}
		public TreeView SetTree { get{return mTree;} set{mTree = value; mView = null;}}

		#region Constructors
		public guiAssetCustomToolsMenu(MogMainForm main, string customToolsInfo)
		{
			mView = null;
			mTree = null;
			mCustomToolsInfo = customToolsInfo;
			mainForm = main;
		}

		public guiAssetCustomToolsMenu(MogMainForm main, string customToolsInfo, ListView view)
		{
			mView = view;
			mTree = null;
			mCustomToolsInfo = customToolsInfo;
			mainForm = main;
		}

		public guiAssetCustomToolsMenu(MogMainForm main, string customToolsInfo, TreeView tree)
		{
			mView = null;
			mTree = tree;
			mCustomToolsInfo = customToolsInfo;
			mainForm = main;
		}
		#endregion

		public void InitializeItems(MenuItem menu)
		{
			string dir = mCustomToolsInfo.Substring(0, mCustomToolsInfo.LastIndexOf("\\"));
			string wildcard = mCustomToolsInfo.Substring(mCustomToolsInfo.LastIndexOf("\\")+1, (mCustomToolsInfo.Length - mCustomToolsInfo.LastIndexOf("\\"))-1);

			foreach (FileInfo file in DosUtils.FileGetList(dir, wildcard))
			{
				MOG_Ini customTools = new MOG_Ini(file.FullName);

				// Remove the beginning of the info name
				string tmp = wildcard.Substring(0, wildcard.IndexOf("*"));
				string name = file.Name.Replace(tmp , "");

				// Remove the .info
				name = name.Replace(".info", "");
				
				// Create the main Item
				MenuItem parent = new MenuItem(name);

				if (customTools.SectionExist("USER_TOOLS"))
				{
					mClickHandlers = new ArrayList();
					for (int i = 0; i < customTools.CountKeys("USER_TOOLS"); i++)
					{
						parent.MenuItems.Add(CreateItem(customTools.GetKeyNameByIndexSLOW("USER_TOOLS", i), new MogMenuItem_Click(CustomToolMenuItem_Click)));
					}
				}

				menu.MenuItems.Add(parent);
			}
		}

		private MenuItem CreateItem(string name, MogMenuItem_Click method)
		{
			MenuItem Item = new MenuItem(name);
			Item.Click += new System.EventHandler(method);
			return Item;
		}


		#region UTILITY FUNCTIONS
		/// <summary>
		/// Locates a string in an array and returns its index
		/// </summary>
		/// <param name="cols"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private int ColumnNameFind(string []cols, string name)
		{
			for (int x = 0; x < cols.Length; x++)
			{
				if (string.Compare(cols[x], name, true) == 0)
				{
					return x;
				}
			}

			return -1;
		}

		private ArrayList ControlGetSelectedItems()
		{
			ArrayList nodes = new ArrayList();

			// Get the list nodes
			if (mView != null && mTree == null)
			{
				foreach (ListViewItem item in mView.SelectedItems)
				{
					int index = ColumnNameFind(mColumns, "Fullname");

					if (index != -1)
					{
						string text = item.SubItems[index].Text;

						nodes.Add(new guiAssetTreeTag(text, guiAssetTreeTag.TREE_FOCUS.LABEL, true));
					}
				}
			}
				// Get the tree nodes
			else if (mView == null && mTree != null)
			{
				nodes.Add((guiAssetTreeTag)mTree.SelectedNode.Tag);
			}
			else
			{
				// No valid nodes
				return null;
			}

			return nodes;
		}

		private string FormatString(guiAssetTreeTag tag, string format)
		{
			// Replace out any string options {}
			if (format.IndexOf("{projectRoot}") != -1)
			{
				format = format.Replace("{projectRoot}", MOG_ControllerProject.GetCurrentSyncDataController().GetSyncDirectory());
			}

			// Replace out any string options {}
			if (format.IndexOf("{projectName}") != -1)
			{
				format = format.Replace("{projectName}", MOG_ControllerProject.GetProjectName());
			}

			// Replace out any string options {}
			if (format.IndexOf("{LoginUserName}") != -1)
			{
				format = format.Replace("{LoginUserName}", MOG_ControllerProject.GetActiveUser().GetUserName());
			}

			// Replace out any string options {}
			if (format.IndexOf("{SelectedItem}") != -1)
			{
				format = format.Replace("{SelectedItem}", tag.FullFilename);
			}

			return format;
		}


		private void ExecucteCustomToolWindow(string toolGroup, string toolName, guiAssetTreeTag tag)
		{
			string infoFilename = mCustomToolsInfo.Replace("*", toolGroup);
			MOG_Ini customTools = new MOG_Ini(infoFilename);

			string command = "";
			string argAsset = "";
			System.Diagnostics.ProcessWindowStyle windowMode = System.Diagnostics.ProcessWindowStyle.Normal;
			bool createWindow = false;

			if (customTools.SectionExist(toolName))
			{
				// Get command
				if (customTools.KeyExist(toolName, "Command"))
				{
					command = FormatString(tag, customTools.GetString(toolName, "Command"));
				}

				// Get argAsset
				if (customTools.KeyExist(toolName, "argAsset"))
				{
					argAsset = FormatString(tag, customTools.GetString(toolName, "argAsset"));
				}

				// Get window mode
				if (customTools.KeyExist(toolName, "windowMode"))
				{
					string mode = FormatString(tag, customTools.GetString(toolName, "windowMode"));

					if (string.Compare(mode, "Hidden", true) == 0)
					{
						windowMode = System.Diagnostics.ProcessWindowStyle.Hidden;
					}
					else if (string.Compare(mode, "Maximise", true) == 0)
					{
						windowMode = System.Diagnostics.ProcessWindowStyle.Maximized;
					}
					else if (string.Compare(mode, "Minimized", true) == 0)
					{
						windowMode = System.Diagnostics.ProcessWindowStyle.Minimized;
					}
					else if (string.Compare(mode, "Normal", true) == 0)
					{
						windowMode = System.Diagnostics.ProcessWindowStyle.Normal;
					}
					else
					{
						windowMode = System.Diagnostics.ProcessWindowStyle.Normal;
					}
				}

				// Get Toggle Options
				if (customTools.KeyExist(toolName, "ToggleOptions"))
				{
					createWindow = true;
				}

				// Get Numerical Options
				if (customTools.KeyExist(toolName, "NumericalOptions"))
				{
					createWindow = true;
				}

				// Get String Options
				if (customTools.KeyExist(toolName, "StringOptions"))
				{
					createWindow = true;
				}

				if (createWindow)
				{
					//CustomToolOptionsForm form = new CustomToolOptionsForm();
					//form.Text = toolName;

					//if (form.ShowDialog() == DialogResult.OK)
					//{
						// Do stuff
					//}
				}
				else
				{
					string output="";
					Report outputForm = new Report(mainForm);
					if (windowMode == System.Diagnostics.ProcessWindowStyle.Hidden)
					{
						outputForm.LogRichTextBox.Text = "GENERATING REPORT.  PLEASE WAIT...";
						outputForm.LogOkButton.Enabled = false;

						// Load saved positions
						//mainForm.mUserPrefs.Load("ReportForm", outputForm);
						guiUserPrefs.LoadDynamic_LayoutPrefs("ReportForm", outputForm);

						outputForm.Show(mainForm);
						Application.DoEvents();
					}

					guiCommandLine.ShellSpawn(command.Trim(), argAsset.Trim(), windowMode, ref output);

					if (windowMode == System.Diagnostics.ProcessWindowStyle.Hidden)
					{							
						outputForm.LogRichTextBox.Text = output;
						outputForm.LogOkButton.Enabled = true;
					}
				}
			}
		}

		#endregion
		
		#region CLICK HANDLES
		private void CustomToolMenuItem_Click(object sender, System.EventArgs e)
		{
			ArrayList selectedItems = ControlGetSelectedItems();
			MenuItem tool = (MenuItem)sender;
			MenuItem toolGroup = (MenuItem)tool.Parent;

			foreach (guiAssetTreeTag tag in selectedItems)
			{				
				if (tag.Execute)
				{
					ExecucteCustomToolWindow(toolGroup.Text, tool.Text, tag);
				}
			}
		}
		#endregion
	}
}
