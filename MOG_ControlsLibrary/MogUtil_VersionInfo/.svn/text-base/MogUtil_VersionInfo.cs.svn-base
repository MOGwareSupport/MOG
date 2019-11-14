using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

using MOG;

namespace MOG_ControlsLibrary.Utils
{
    public class MogUtil_VersionInfo
    {
        private const string FullVerionText = "{Full Version}";

		public static bool SetLightVersionControl(MenuItem menuItem)
		{
			// If we are in 'Light' mode we need to disable this menu
			if (MOG_Main.IsUnlicensed())
			{
				menuItem.Enabled = false;

				// Make sure this menu item does not already say 'Full Version'
				if (menuItem.Text.IndexOf(FullVerionText) == -1)
				{
					menuItem.Text = menuItem.Text + " " + FullVerionText;
				}

				return menuItem.Enabled;
			}

			return menuItem.Enabled;
		}

		public static bool SetLightVersionControl(ToolStripMenuItem menuItem)
        {
            // If we are in 'Light' mode we need to disable this menu
            if (MOG_Main.IsUnlicensed())
            {
                menuItem.Enabled = false;

				// Make sure this menu item does not already say 'Full Version'
                if (menuItem.Text.IndexOf(FullVerionText) == -1)
                {
                    menuItem.Text = menuItem.Text + " " + FullVerionText;
                }

                return menuItem.Enabled;
            }

            return menuItem.Enabled;
        }

		public static bool SetLightVersionControl(ContextMenuStrip menu)
		{
			// If we are in 'Light' mode we need to disable this menu
			if (MOG_Main.IsUnlicensed() && menu != null)
			{
				foreach (ToolStripItem item in menu.Items)
				{
					// Make sure this menuItem is not a separator
					if (item is ToolStripMenuItem)
					{
						ToolStripMenuItem menuItem = item as ToolStripMenuItem;
						menuItem.Enabled = false;

						// Make sure this menu item does not already say 'Full Version'
						if (menuItem.Text.IndexOf(FullVerionText) == -1)
						{
							menuItem.Text = menuItem.Text + " " + FullVerionText;
						}
					}
				}

				return false;
			}

			return true;
		}

		public static bool SetLightVersionControl(Button button)
		{
			// If we are in 'Light' mode we need to disable this menu
			if (MOG_Main.IsUnlicensed())
			{
				button.Enabled = false;

				return button.Enabled;
			}

			return button.Enabled;
		}

        public static bool SetLightVersionControl(TreeNode node)
        {
            // If we are in 'Light' mode we need to disable this menu
            if (MOG_Main.IsUnlicensed())
            {
                node.ForeColor = SystemColors.GrayText;

				// Make sure this node does not already say 'Full Version'
                if (node.Text.IndexOf(FullVerionText) == -1)
                {
                    node.Text = node.Text + " " + FullVerionText;
                }

                return false;
            }

            return true;
        }
    }
}
