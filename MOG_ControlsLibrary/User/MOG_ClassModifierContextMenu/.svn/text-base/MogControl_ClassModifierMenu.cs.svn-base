using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG.INI;
using MOG.DOSUTILS;
using MOG.CONTROLLER.CONTROLLERPROJECT;

namespace MOG_ClientControlsLibrary.MOG_ClassModifierContextMenu
{
	/// <summary>
	/// Summary description for MOG_ClassModifierMenu.
	/// </summary>
	public class MOG_ClassModifierMenu : System.Windows.Forms.ContextMenu
	{
		/// <summary>
		/// Base delagate for assigning a click event to a ClassModifier MenuItem
		/// </summary>
		public delegate void MogMenuItem_Click(object sender, System.EventArgs e);

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Base Constructor
		/// </summary>
		public MOG_ClassModifierMenu()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		#region System calls
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
		#endregion

		private void PopulateChangePropertiesToolsMenu(ContextMenu parent, string classification, MogMenuItem_Click method)
		{
			MenuItem menuRoot = null;

			// Find the change properties menuItem
			foreach (MenuItem item in parent.MenuItems)
			{
				if (MOG.StringUtils.StringCompare(item.Text, "Change Properties*"))
				{
					menuRoot = item;
					break;
				}
			}

			if (menuRoot != null)
			{
				// Check to see if there is a special menu options file
				string location = string.Concat(MOG_ControllerProject.GetProject().GetProjectToolsPath(), "\\ClassModifiers\\", classification, ".Menu");
				if (classification != "*" && DosUtils.FileExist(location))
				{
					// Enable our menu
					menuRoot.Enabled = true;
					menuRoot.Text = "Change Properties (" + classification + ")";

					MOG_Ini specialMenuIni = new MOG_Ini(location);
			
					if (specialMenuIni.SectionExist("ClassModifiers.GlobalMenu"))
					{
						// Clear the list
						menuRoot.MenuItems.Clear();
						PopulateChangePropertiesSubMenu(specialMenuIni, "ClassModifiers.GlobalMenu", menuRoot, method);						
					}
				}
				else
				{
					menuRoot.Text = "Change Properties (N/A)";
					menuRoot.Enabled = false;
				}
			}
		}

		private void PopulateChangePropertiesSubMenu(MOG_Ini specialMenuIni, string menu, MenuItem menuRoot, MogMenuItem_Click method)
		{
			// Loop through all the keys of this section and recursivly add all of them to the menu
			for (int x =0; x < specialMenuIni.CountProperty(menu, "MenuItem"); x++)
			{
				string option = specialMenuIni.GetKeyPropertyNameByIndex(menu, "MenuItem", x);

				// Check if this menu item is an actual item or a pointer to another menu.
				// Do this by checking for a matching section?
				if (specialMenuIni.SectionExist(option))
				{
					// Create the subMenu
					MenuItem subMenu = new MenuItem();
					subMenu.Text = option.ToLower().Replace("menu", "");
					PopulateChangePropertiesSubMenu(specialMenuIni, option, subMenu, method);
					menuRoot.MenuItems.Add(subMenu);
				}
				else
				{
					// Add menu item
					menuRoot.MenuItems.Add(AddChangePropertiesToolsMenu(specialMenuIni, option, method));
				}
			}
		}

		private MenuItem AddChangePropertiesToolsMenu(MOG_Ini specialMenuIni, string option, MogMenuItem_Click method)
		{
			MenuItem subItem = new MenuItem();
			subItem.Text = option;

			if (specialMenuIni.SectionExist(option))
			{				
				for (int x=0; x<specialMenuIni.CountKeys(option); x++)
				{
					string label = specialMenuIni.GetKeyNameByIndex(option, x);
					MenuItem child = AddChangePropertiesToolsMenu(specialMenuIni, label, method);
					if (child != null)
					{
						subItem.MenuItems.Add(child);
					}
				}

				return subItem;
			}
			else
			{
				subItem.Click += new EventHandler(method);
			}
	
			return subItem;
		}
	}
}
