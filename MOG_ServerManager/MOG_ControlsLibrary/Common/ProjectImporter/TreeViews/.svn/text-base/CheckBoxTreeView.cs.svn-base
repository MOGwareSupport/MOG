using System;
using System.Windows.Forms;

using MOG.PROGRESS;
using MOG_ControlsLibrary;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for CheckBoxTreeView.
	/// </summary>
	public class CheckBoxTreeView : CodersLab.Windows.Controls.TreeView
	{
		#region Member vars
		private cbtvCheckStyle checkStyle = cbtvCheckStyle.Normal;
		protected bool disableChecking = false;

		#endregion
		#region Properties
		public bool DisableChecking
		{
			get { return this.disableChecking; }
		}

		public cbtvCheckStyle CheckStyle
		{
			get { return this.checkStyle; }
			set 
			{
				this.checkStyle = value;
				
				if (value == cbtvCheckStyle.None)
					this.CheckBoxes = false;
				else
					this.CheckBoxes = true;
			}
		}
		#endregion
		#region Enums
		public enum cbtvCheckStyle { None, Normal, Recursive };
		#endregion
		#region Constructor
		public CheckBoxTreeView()
		{
			this.HideSelection = false;
			this.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(CheckBoxTreeView_AfterCheck);
		}
		#endregion
		#region Private functions
		private void PropagateCheckStateDown(AssetTreeNode tn)
		{
			foreach (AssetTreeNode subNode in tn.Nodes)
			{
				subNode.Checked = tn.Checked;
				PropagateCheckStateDown(subNode);
			}
		}

		public void PropagateCheckStateUp(AssetTreeNode tn)
		{
			AssetTreeNode parentNode = tn.Parent as AssetTreeNode;
			while (parentNode != null)
			{
				parentNode.Checked = tn.Checked;
				parentNode = parentNode.Parent as AssetTreeNode;
			}
		}

		#endregion
		#region Event handlers
		private void CheckBoxTreeView_AfterCheck(object sender, TreeViewEventArgs args)
		{
			if (this.checkStyle == cbtvCheckStyle.Recursive  &&  !this.disableChecking)
			{
// JohnRen - Removed because this was seriously slowing down the importation of larger 100k+ file projects
//				this.BeginUpdate();
				Cursor.Current = Cursors.WaitCursor;
				this.disableChecking = true;	// make sure we don't flag more AfterCheck events
				
				PropagateCheckStateDown(args.Node as AssetTreeNode);
				if (args.Node.Checked)
					PropagateCheckStateUp(args.Node as AssetTreeNode);
			
				Cursor.Current = Cursors.Default;
				this.disableChecking = false;
// JohnRen - Removed because this was seriously slowing down the importation of larger 100k+ file projects
//				this.EndUpdate();
			}
		}
		#endregion
	}
}


