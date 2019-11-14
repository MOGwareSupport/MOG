using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

namespace MOG_Server.MOG_ControlsLibrary.Common
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public class Utils
	{
		public Utils()
		{
		}

		public static bool CopyDirectory(string source, string dest)
		{
			if (!Directory.Exists(source))
				return false;
			
			// remove trailing backslashes if any
			source = source.Trim("\\".ToCharArray());
			dest = dest.Trim("\\".ToCharArray());

			if (!Directory.Exists(dest))
			{
				if ( !Directory.CreateDirectory(dest).Exists )
					return false;
			}

			// copy files and subdirs
			string[] members = Directory.GetFileSystemEntries(source);	// get full names of all files and directories
			foreach (string member in members)
			{
				if (Directory.Exists(member))	// if its a directory
				{
					if (!CopyDirectory(member, dest + "\\" + Path.GetFileName(member)))
						return false;
				}
				else
					File.Copy(member, dest + "\\" + Path.GetFileName(member), true);
			}

			return true;
		}

		public static bool ArrayListTypeCheck(ArrayList list, System.Type type)
		{
			if (list == null  ||  type == null)
				return false;

			foreach (object obj in list)
			{
				if (obj.GetType() != type)
					return false;
			}

			return true;
		}

		public static ArrayList GetUniqueRootNodes(ArrayList nodes)
		{
			ArrayList roots = new ArrayList();
			if (nodes == null)
				return roots;

			foreach (TreeNode tn in nodes)
			{
				TreeNode root = GetRootNode(tn);
				if (!roots.Contains(root))
					roots.Add(root);
			}

			return roots;
		}

		public static TreeNode GetRootNode(TreeNode tn)
		{
			TreeNode parent = tn;
			while (parent.Parent != null)
				parent = parent.Parent;

			return parent;
		}

		public static int CountNodesRecursive(TreeNodeCollection nodes)
		{
			int count = 0;
			count += nodes.Count;
			
			foreach (TreeNode tn in nodes)
				count += CountNodesRecursive(tn.Nodes);

			return count;
		}

		public static void CenterForm(Form form, Control parentControl)
		{
			int x = parentControl.Location.X + ((parentControl.Width - form.Width) / 2);
			int y = parentControl.Location.Y + ((parentControl.Height - form.Height) / 2);
			form.Location = new System.Drawing.Point(x, y);
		}

		public static bool NodesCollectionContainsSubNode(TreeNodeCollection nodes, string text)
		{
			if (nodes == null  ||  text == null)
				return false;

			foreach (TreeNode tn in nodes)
			{
				if (tn.Text.ToLower() == text.ToLower())
					return true;
			}

			return false;
		}

		public static string ArrayListToString(ArrayList list, string prefix)
		{
			if (list == null)
				return "";

			string msg = "";
			foreach (object obj in list)
				msg += prefix + obj.ToString() + "\n";

			return msg;
		}

		public static int CountNodesThatStartWithString(TreeNodeCollection nodes, string text)
		{
			int count = 0;
			foreach (TreeNode node in nodes)
			{
				if (node.Text.ToLower().StartsWith( text.ToLower() ))
					++count;
			}

			return count;
		}

		public static void SetListViewItemColor(ListViewItem item, System.Drawing.Color color)
		{
			item.ForeColor = color;
			for (int i=0; i<item.SubItems.Count; i++)
				item.SubItems[i].ForeColor = color;
		}

		public static void SelectListViewItem(ListViewItem item)
		{
			if (item == null  ||  item.ListView == null)
				return;

			foreach (ListViewItem lvi in item.ListView.Items)
				lvi.Selected = false;

			item.Selected = true;
		}

		public static ArrayList GetLeafNodes(TreeNode tn)
		{
			ArrayList nodes = new ArrayList();

			if (tn == null)
				return nodes;

			if (tn.Nodes.Count == 0)
				nodes.Add(tn);

			foreach (TreeNode subNode in tn.Nodes)
				nodes.AddRange( GetLeafNodes(subNode) );

			return nodes;
		}

		public static bool IsSubNode(TreeView tv, TreeNode child)
		{
			if (tv == null  ||  child == null)
				return false;

			if (tv.Nodes.Contains(child))
				return true;

			foreach (TreeNode n in tv.Nodes)
			{
				if (IsSubNode(n, child))
					return true;
			}

			return false;
		}

		public static bool IsSubNode(TreeNode parent, TreeNode child)
		{
			if (parent == null  ||  child == null)
				return false;

			if (parent.Nodes.Contains(child))
				return true;

			foreach (TreeNode n in parent.Nodes)
			{
				if (IsSubNode(n, child))
					return true;
			}

			return false;
		}

		public static void SetListViewColumns(ListView listView, ArrayList columns)
		{
			if (listView == null  ||  columns == null)
				return;

			listView.Columns.Clear();
			foreach (string colName in columns)
				listView.Columns.Add( colName, 100, HorizontalAlignment.Left );
		}

		public static void AppendLineToFile(string filename, string line)
		{
			if (Directory.Exists( Path.GetDirectoryName(filename) ))
			{
				StreamWriter sw = new StreamWriter( File.Open(filename, FileMode.Append) );
				sw.WriteLine(line);
				sw.Close();
			}
		}

		public static DialogResult ShowMessageBoxExclamation(string text, string title)
		{
			return MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
		}

		public static DialogResult ShowMessageBoxConfirmation(string text, string title)
		{
			return MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
		}

		public static DialogResult ShowMessageBoxInfo(string text, string title)
		{
			return MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
		}
		
		public static bool ListViewContainsText(ListView listView, string text)
		{
			foreach (ListViewItem item in listView.Items)
			{
				if (text.ToLower() == item.Text.ToLower())
					return true;
			}
			
			return false;
		}

		public static ListViewItem GetListViewItem(ListView listView, string text)
		{
			foreach (ListViewItem item in listView.Items)
			{
				if (item.Text.ToLower() == text.ToLower())
					return item;
			}

			return null;
		}

		public static ListViewItem GetListViewItem(ListView listView, string text, string columnName)
		{
			if (listView == null  ||  text == null  ||  columnName == null)
				return null;

			foreach (ListViewItem item in listView.Items)
			{
				foreach (ColumnHeader col in listView.Columns)
				{
					if (col.Text.ToLower() == columnName.ToLower())
					{
						if (item.SubItems[col.Index].Text.ToLower() == text.ToLower())
							return item;
					}
				}
			}

			return null;
		}

		public static ListViewItem GetListViewItemFromData(ListView listView, ArrayList columns, ArrayList vals) 
		{
			if (columns.Count != vals.Count)
				return null;

			for (int i=0; i<columns.Count; i++)
			{
				if ( !(columns[i] is string)  ||  !(vals[i] is string) )
					return null;
			}

			foreach (ListViewItem item in listView.Items)
			{
				bool match = true;
				for (int i=0; i<columns.Count; i++)
				{
					string col = (string)columns[i];
					string val = (string)vals[i];

					if (GetListViewData(listView, item, col) != val)
						match = false;
				}

				if (match)
					return item;
			}

			return null;
		}

		public static ListViewItem FormatListViewItem(ListView listView, ArrayList allColumns, ArrayList allValues)
		{
			// must have same number of elements
			if (allColumns.Count != allValues.Count)
				return null;
			
			// if both don't contain all strings, bail
			for (int i=0; i<allColumns.Count; i++)
			{
				if ( !(allColumns[i] is string)  ||  !(allValues[i] is string) )
					return null;
			}

			int index = 0;
			ListViewItem item = new ListViewItem();
			foreach (ColumnHeader h in listView.Columns)
			{
				if ( allColumns.Contains(h.Text) )
				{
					if (index == 0)
						item.Text = (string)allValues[allColumns.IndexOf(h.Text)];
					else
						item.SubItems.Add( (string)allValues[allColumns.IndexOf(h.Text)] );

				}
				else
				{
					if (index == 0)
						item.Text = "";
					else
						item.SubItems.Add( "" );

				}

				++index;
			}

			return item;
		}

		public static void AutoSizeListViewColumns(ListView listView)
		{
			// resize all column widths to -2 (autosize to largest element)
			foreach (ColumnHeader h in listView.Columns)
			{
				h.Width = -2;

				// sanity check
				if (h.Width > 300)
					h.Width = 300;
			}
		}

		public static string GetListViewData(ListView listView, ListViewItem item, string columnName)
		{
			if (listView.Columns.Count < item.SubItems.Count)
				return null;

			int colNum = 0;
			foreach (ColumnHeader colHeader in listView.Columns)
			{
				if (colHeader.Text.ToLower() == columnName.ToLower())
					return item.SubItems[colNum].Text;

				++colNum;
			}

			return null;
		}

		public static long DirSizeInBytes(string path)
		{
			if (!Directory.Exists(path))
				return 0;

			long size = 0;
			DirectoryInfo dInfo = new DirectoryInfo(path);
			foreach (FileInfo fInfo in dInfo.GetFiles())
				size += fInfo.Length;

			foreach (DirectoryInfo subdirInfo in dInfo.GetDirectories())
				size += Utils.DirSizeInBytes(subdirInfo.FullName);

			return size;
		}

		public static bool RunCommand(string cmd, string args)
		{
			// Run the command
			Process p = new Process();
			p.StartInfo.FileName = cmd;
			p.StartInfo.Arguments = args;

			p.StartInfo.UseShellExecute = false;
			try
			{
				p.Start();
			}
			catch(Exception e)
			{
				ShowMessageBoxExclamation(String.Concat("Utils.RunCommand():  Could not start (", e.ToString(), ")\n", e.StackTrace), "SpawnCommand Error");
				return false;
			}
	
			p.WaitForExit();
			bool retVal = (p.ExitCode == 0);
			p.Close();
			p = null;
			
			return retVal;
		}
	}
}


