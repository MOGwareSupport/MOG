using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using MOG.DOSUTILS;
using MOG.PROMPT;
using MOG.REPORT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.PROPERTIES;
using MOG;
using MOG.INI;
using MOG.CONTROLLER.CONTROLLERASSET;

namespace MOG_ControlsLibrary.Wizards
{
	public partial class AddRipPropertyWizard : Form
	{
		MOG_Properties mProperties;
		private string mPropertyMenu;
		public string PropertyMenu
		{
			get { return mPropertyMenu; }			
		}

		private enum TreeImages
		{
			ROOT_IMAGE,
			FOLDER_IMAGE,
			PROPERTY_IMAGE,
		}
	

		public AddRipPropertyWizard(MOG_Properties properties)
		{
			InitializeComponent();

			mProperties = properties;
		}

		#region "Support functions"

		#endregion "Support functions"

		#region "Wizard tab events"
		
		private void FolderStartWizardPage_CloseFromNext(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			e.Page = PropertyNameWizardPage;
		}

		private void PropertyNameWizardPage_ShowFromNext(object sender, EventArgs e)
		{
			// Always reset our tree
			TreeNode root = PropertyContextMenuTreeView.Nodes[0];
			root.Nodes.Clear();

			// We need to initialize any previously existing menu
			if (mProperties.PropertyMenu.Length != 0)
			{
				string PropertyMenuFullName = MOG_Tokens.GetFormattedString(mProperties.PropertyMenu, MOG_Tokens.GetProjectTokenSeeds(MOG_ControllerProject.GetProject()));

				// Make sure we got a filename with a full path, if we didn't it is probably a relational path
				if (!Path.IsPathRooted(PropertyMenuFullName))
				{
					PropertyMenuFullName = MOG_ControllerProject.GetProject().GetProjectToolsPath() + "\\" + PropertyMenuFullName;
				}

				// Find and open the menu
				if (DosUtils.FileExist(PropertyMenuFullName))
				{
					MOG_PropertiesIni ripMenu = new MOG_PropertiesIni(PropertyMenuFullName);
					if (ripMenu.SectionExist("Property.Menu"))
					{
						string property = "";

						// Create the sub menu
						CreateChangePropertiesSubMenu(property, PropertyContextMenuTreeView.Nodes[0], "Property.Menu", ripMenu);
					}
				}
			}
		}

		private void PropertyFilenameWizardPage_ShowFromNext(object sender, EventArgs e)
		{
			// Create a default name
			string defaultName = "";

			// Do we already have a name for this property menu?
			if (mProperties != null && mProperties.PropertyMenu.Length != 0)
			{
				// Get it
				defaultName = mProperties.PropertyMenu;
			}
			else
			{
				// Is this an asset?
				if (mProperties.GetAssetFilename().GetFullFilename() != "")
				{
					defaultName = mProperties.GetAssetFilename().GetAssetLabel() + ".Menu.Info";
				}
				else
				{
					defaultName = mProperties.GetClassificationName() + ".Menu.Info";
				}				
			}

			// Set the textBox
			PropertyFilenameTextBox.Text = defaultName;
		}

		private void PropertyFilenameWizardPage_CloseFromNext(object sender, MOG_ControlsLibrary.Utils.PageEventArgs e)
		{
			if (PropertyFilenameTextBox.Text.Length > 0)
			{
				string PropertyMenuFullName = MOG_Tokens.GetFormattedString(PropertyFilenameTextBox.Text, MOG_Tokens.GetProjectTokenSeeds(MOG_ControllerProject.GetProject()));

				string LocatedPropertyMenu = MOG_ControllerSystem.LocateTool(PropertyMenuFullName);

				if (LocatedPropertyMenu.Length == 0)
				{
					// Make sure we got a filename with a full path, if we didn't it is probably a relational path
					if (!Path.IsPathRooted(PropertyMenuFullName))
					{
						PropertyMenuFullName = MOG_ControllerProject.GetProject().GetProjectToolsPath() + "\\Property.Menus\\" + PropertyMenuFullName;
					}
				}
				else
				{
					PropertyMenuFullName = LocatedPropertyMenu;
				}

				// Create or open the existing properties
				MOG_PropertiesIni ripMenu = new MOG_PropertiesIni(PropertyMenuFullName);
				
				// Reset our menu
				ripMenu.Empty();

				// Add the properties
				ripMenu.PutSection("Property.Menu");
				SaveOutMenus(ripMenu, PropertyContextMenuTreeView.Nodes[0].Nodes);

				// Save the properties
				ripMenu.Save();

				// Save our new propertyMenu filename
				mPropertyMenu = PropertyMenuFullName;
			}

			// Skip to the end
			e.Page = PropertyEndWizardPage;
		}

		private void PropertyTestWizardPage_ShowFromNext(object sender, EventArgs e)
		{
			//PropertyContextMenuStrip 
		}	
		#endregion "Wizard tab events"

		#region "Support methods"

		private void CreateChangePropertiesSubMenu(string propertyPath, TreeNode parentNode, string section, MOG_PropertiesIni ripMenu)
		{
			for (int i = 0; i < ripMenu.CountProperties(section, "MenuItem"); i++)
			{
				MOG_Property property = ripMenu.GetPropertyByIndex(section, "MenuItem", i);
				if (property != null)
				{
					string menuItem = property.mPropertyKey;
					if (propertyPath.Length == 0)
					{
						CreateChangePropertiesMenuItem(menuItem, parentNode, section, menuItem, ripMenu);
					}
					else
					{
						CreateChangePropertiesMenuItem(propertyPath + "/" + menuItem, parentNode, section, menuItem, ripMenu);
					}
				}
			}
		}

		private void CreateChangePropertiesMenuItem(string propertyPath, TreeNode parentNode, string section, string itemName, MOG_PropertiesIni ripMenu)
		{
			string command = ripMenu.GetPropertyString(section, "MenuItem", itemName);

			// Check to see if this command is another sub menu
			if (ripMenu.SectionExist(command))
			{
				TreeNode propertyMenuItem = CreatePropertyMenuNode(parentNode, itemName);
				CreateChangePropertiesSubMenu(propertyPath, propertyMenuItem, command, ripMenu);
			}
			else
			{
				string globalSection, propertySection, key, val;
				string[] leftParts = command.Split("=".ToCharArray());
				if (leftParts.Length < 2)
				{
					string title = "Change Properties Failed";
					string message = "Invalid property format specified.\n" +
									 "Specified Format: " + command + "\n" +
									 "   Proper Format: [Section]{PropertyGroup}PropertyName=PropertyValue";
					MOG_Prompt.PromptMessage(title, message);
					return;
				}
				else
				{
					string[] testParts = leftParts[0].Split("[]{}".ToCharArray());
					if (testParts.Length != 5)
					{
						string title = "Change Properties Failed";
						string message = "Invalid property format specified.\n" +
										 "Specified Format: " + command + "\n" +
										 "   Proper Format: [Section]{PropertyGroup}PropertyName=PropertyValue";
						MOG_Prompt.PromptMessage(title, message);
						return;
					}
					else
					{
						try
						{
							globalSection = testParts[1];
							propertySection = testParts[3];
							key = testParts[4];
							val = command.Substring(command.IndexOf("=") + 1);
						}
						catch
						{
							string title = "Change Properties Failed";
							string message = "Invalid property format specified.\n" +
											 "Specified Format: " + command + "\n" +
											 "   Proper Format: [Section]{PropertyGroup}PropertyName=PropertyValue";
							MOG_Prompt.PromptMessage(title, message);
							return;
						}
					}

					CreatePropertyNode(parentNode, itemName, globalSection, propertySection, key, val);
				}
			}			
		}

		private void SaveOutMenus(MOG_PropertiesIni ripMenu, TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				PropertyProperty property = node.Tag as PropertyProperty;
				if (property != null)
				{
					if (node.Parent.Text == "Root")
					{
						ripMenu.PutString("Property.Menu", "{MenuItem}" + property.MenuText, "[" + property.PropertyCategory + "]" + "{" + property.PropertySubCategory + "}" + property.PropertyName + "=" + property.PropertyValue);
					}
					else
					{
						ripMenu.PutString(node.Parent.Text, "{MenuItem}" + property.MenuText, "[" + property.PropertyCategory + "]" + "{" + property.PropertySubCategory + "}" + property.PropertyName + "=" + property.PropertyValue);
					}
				}
				else
				{
					PropertyMenu menu = node.Tag as PropertyMenu;
					if (menu != null)
					{
						if (node.Parent.Text == "Root")
						{
							ripMenu.PutString("Property.Menu", "{MenuItem}" + menu.MenuText, menu.MenuText);
						}
						else
						{
							ripMenu.PutString(node.Parent.Text, "{MenuItem}" + menu.MenuText, menu.MenuText);
						}

						// Menus ALWAYS require a section...even if it is blank!
						ripMenu.PutSection(menu.MenuText);
					}
				}

				if (node.Nodes.Count > 0)
				{
					SaveOutMenus(ripMenu, node.Nodes);
				}
			}
		}

		private TreeNode CreatePropertyNode(TreeNode root, string menuName, string category, string subcategory, string variable, string value)
		{
			PropertyProperty property = new PropertyProperty(menuName, category, subcategory, variable, value);
			TreeNode node = new TreeNode(property.MenuText);

			property.node = node;
			node.ImageIndex = (int)TreeImages.PROPERTY_IMAGE;
			node.SelectedImageIndex = (int)TreeImages.PROPERTY_IMAGE;
			node.Name = property.MenuText;
			node.Tag = property;

			AddNode(root, node);
			return node;
		}

		private TreeNode CreatePropertyNode(TreeNode root)
		{
			string className = mProperties.GetClassificationName();

			try
			{
				// Try and trim this to be just the class name and not its entire lineage
				if (className.IndexOf("~") != -1)
				{
					className = className.Substring(className.LastIndexOf("~") + 1);
				}
			}
			catch
			{
				className = mProperties.GetClassificationName();
			}

			// Make sure this is a valid unique name
			string newName = GenerateUniqueName("MyProperty", root.TreeView);
			return CreatePropertyNode(root, newName, MOG_ControllerProject.GetProjectName(), className, "MyProperty", "MyValue");
		}

		private TreeNode CreatePropertyMenuNode(TreeNode root, string menuName)
		{
			PropertyMenu menu = new PropertyMenu(menuName);
			TreeNode node = new TreeNode(menu.MenuText);

			menu.node = node;
			node.ImageIndex = (int)TreeImages.FOLDER_IMAGE;
			node.SelectedImageIndex = (int)TreeImages.FOLDER_IMAGE;
			node.Name = menu.MenuText;
			node.Tag = menu;

			AddNode(root, node);

			return node;
		}

		private TreeNode CreatePropertyMenuNode(TreeNode root)
		{
			// Make sure this is a valid unique name
			string newName = GenerateUniqueName("New Menu", root.TreeView);
			return CreatePropertyMenuNode(root, newName);
		}

		private void AddNode(TreeNode root, TreeNode node)
		{
			if (root == null)
			{
				//PropertyContextMenuTreeView.Nodes.Clear();

				// Add the root
				PropertyContextMenuTreeView.Nodes.Add(node);
				PropertyContextMenuTreeView.ExpandAll();
			}
			else
			{
				// Add to the selected node
				root.Nodes.Add(node);
				root.ExpandAll();
			}
		}

		#endregion "Support methods"

		#region "Control events"

		private void addToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			TreeNode root = PropertyContextMenuTreeView.SelectedNode;
			CreatePropertyMenuNode(root);			
		}

		private void removeToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			TreeNode root = PropertyContextMenuTreeView.SelectedNode;
			if (root != null)
			{
				root.Remove();
			}
		}

		private void addPropertyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode root = PropertyContextMenuTreeView.SelectedNode;
			CreatePropertyNode(root);			
		}

		private void PropertyContextMenuTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNode root = PropertyContextMenuTreeView.SelectedNode;
			if (root != null && root.Tag != null)
			{
				PropertyPropertyGrid.SelectedObject = root.Tag;
			}
		}

		private void PropertyPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			// Check if the 'MenuText' just changed?
			if (e.ChangedItem.Label == "MenuText")
			{
				// Check if this is a property?
				PropertyProperty prop = PropertyPropertyGrid.SelectedObject as PropertyProperty;
				if (prop != null)
				{
					// Make sure this is a valid unique name
					string validName = GenerateUniqueName(prop.MenuText, prop.node.TreeView);
					// Update the tree node
					prop.MenuText = validName;
					prop.node.Text = validName;
					prop.node.Name = validName;
				}
				else
				{
					// Check if this is a menu?
					PropertyMenu menu = PropertyPropertyGrid.SelectedObject as PropertyMenu;
					if (menu != null)
					{
						// Make sure this is a valid unique name
						string validName = GenerateUniqueName(menu.MenuText, menu.node.TreeView);
						// Update the tree node
						menu.MenuText = validName;
						menu.node.Text = validName;
						menu.node.Name = validName;
					}
				}
			}
		}

		private string GenerateUniqueName(string desiredName, TreeView treeView)
		{
			string newName = desiredName;

			// Make sure we have a parentNode to search
			if (treeView != null)
			{
				// Loop while we try and find a unique value
				int count = 1;
				while (treeView.Nodes.Find(newName, true).Length > 0)
				{
					// Create a new name with a count index
					newName = desiredName + count.ToString();
					// Increament our count
					count++;
				}
			}

			return newName;
		}

		private void PropertyTreeContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			TreeNode root = PropertyContextMenuTreeView.SelectedNode;
			if (root == null)
			{
				foreach (ToolStripItem item in PropertyTreeContextMenuStrip.Items)
				{
					item.Enabled = false;
				}
			}
			else
			{
				foreach (ToolStripItem item in PropertyTreeContextMenuStrip.Items)
				{
					item.Enabled = true;
				}

				PropertyProperty propertyNode = root.Tag as PropertyProperty;
				if (propertyNode != null)
				{
					addPropertyToolStripMenuItem.Enabled = false;
				}
			}
		}

		private void PropertyContextMenuTreeView_MouseDown(object sender, MouseEventArgs e)
		{
			PropertyContextMenuTreeView.SelectedNode = PropertyContextMenuTreeView.GetNodeAt(e.X, e.Y);
		}

	#endregion "Control events"
				
	}

	internal class PropertyMenu
	{
		public TreeNode node;

		private string mMenuText;
		[Category("Menu item"), Description("This is the name of the menu item that users will see")]
		public string MenuText
		{
			get { return mMenuText; }
			set { mMenuText = value; }
		}

		public PropertyMenu()
		{
		}

		public PropertyMenu(string menuText)
		{
			mMenuText = menuText;
		}
	}

	internal class PropertyProperty : PropertyMenu
	{
		private string mCategory;
		[Category("Property window"), Description("This will specify the root node that this property will be placed when viewing the properties for this asset")]		
		public string PropertyCategory
		{
			get { return mCategory; }
			set { mCategory = value; }
		}

		private string mSubCategory;
		[Category("Property window"), Description("This will specify sub node that this property will be placed when viewing the properties for this asset")]
		public string PropertySubCategory
		{
			get { return mSubCategory; }
			set { mSubCategory = value; }
		}

		private string mPropertyName;
		[Category("Property"), Description("This is the name of the property that can be referenced by conversion scripts (Rippers)")]
		public string PropertyName
		{
			get { return mPropertyName; }
			set { mPropertyName = value; }
		}

		private string mValue;
		[Category("Property"), Description("This value of the property that can be referenced by conversion scripts (Rippers)")]
		public string PropertyValue
		{
			get { return mValue; }
			set { mValue = value; }
		}
	
		public PropertyProperty(string menuName, string category, string subCategory, string variable, string value) 
		{
			MenuText = menuName;
			mCategory = category;
			mSubCategory = subCategory;
			mPropertyName = variable;
			mValue = value;
		}
	}
}