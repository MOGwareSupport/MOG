using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MOG_Client
{
	public class MogControl_ColumnContextMenu : ContextMenuStrip
	{
		private ListView mView;

		public MogControl_ColumnContextMenu(ListView view)
		{
			mView = view;

			// Create a menu item for each column in the columns array
			foreach (ColumnHeader header in view.Columns)
			{
				ToolStripMenuItem menuItem = new ToolStripMenuItem(header.Text, null, new EventHandler(ColumnMenuItem_Click));

				// Check it if that column is greater than 0
				if (header.Width > 0)
				{
					menuItem.Checked = true;
				}
				else
				{
					menuItem.Checked = false;
				}

				// Add this item
				Items.Add(menuItem);
			}
		}

		public void ColumnMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			if (item != null)
			{
				item.Checked = !item.Checked;
				foreach (ColumnHeader header in mView.Columns)
				{
					if (String.Compare(item.Text, header.Text, true) == 0)
					{
						if (!item.Checked)
						{
							header.Width = 0;
						}
						else
						{
							header.Width = 120;
						}
						return;
					}
				}
			}
		}
	}
}
