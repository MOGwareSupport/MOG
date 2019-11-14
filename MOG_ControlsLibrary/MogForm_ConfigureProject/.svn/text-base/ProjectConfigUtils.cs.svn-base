using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

namespace MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for ProjectConfigUtils.
	/// </summary>
	public class ProjectConfigUtils
	{
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

		public static DialogResult ShowMessageBoxExclamation(string text, string title)
		{
			return MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
		}

		public static DialogResult ShowMessageBoxConfirmation(string text, string title)
		{
			return MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
		}
	}
}


