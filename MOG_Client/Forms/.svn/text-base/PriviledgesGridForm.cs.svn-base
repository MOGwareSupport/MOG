using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

namespace MOG_Client.Forms
{
	/// <summary>
	/// Summary description for PropertiesGridForm.
	/// </summary>
	public class PriviledgesGridForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TreeView PropertyFormTreeView;
		private System.Windows.Forms.Panel PropertyFormTreeViewPanel;
		private System.Windows.Forms.Panel PropertyFormTextBoxPanel;
		private System.Windows.Forms.TextBox PropertyFormTreeViewTextBox;
		private System.Windows.Forms.Button PropertyFormTextBoxAddButton;
		private System.Windows.Forms.Button PropertyFormTextBoxDeleteButton;
		private System.Windows.Forms.Label PropertyFormTextBoxLabel;
		private System.Windows.Forms.Panel PropertyFormPropertyGridPanel;
		private System.Windows.Forms.PropertyGrid PropertyFormPropertyGrid;
		private System.Windows.Forms.Panel PropertyFormOKCancelButtonPanel;
		private System.Windows.Forms.Button PropertyFormExitButton;
		private System.Windows.Forms.Splitter PropertyFormSplitter;
		private System.Windows.Forms.Label PropertyFormTextBoxDisplayLabel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PriviledgesGridForm( object propertyObject, TreeView tree )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.PropertyFormPropertyGrid.SelectedObject = propertyObject;

			SwapTreeViews( this.PropertyFormTreeView, tree );
		}

		/// <summary>
		/// 'Loads' a new TreeView into a currently displayed base TreeView
		/// </summary>
		/// <param name="baseView">TreeView which will be changed</param>
		/// <param name="newView">TreeView from which to get our values</param>
		private void SwapTreeViews( TreeView baseView, TreeView newView )
		{
			Control treeParent = (Control)baseView.Parent;

			// Save all the current settings (as best as possible)
			Point currentLocation = baseView.Location;
			Size currentSize = baseView.Size;
			DockStyle currentDock = baseView.Dock;
			AnchorStyles currentAnchor = baseView.Anchor;

			// Remove current treeview from our parent
			treeParent.Controls.Remove( baseView );
			baseView.Dispose();

			// Change the object reference of our current Control
			baseView = newView;

			// Load all the saved settings.
			baseView.Location = currentLocation;
			baseView.Size = currentSize;
			baseView.Dock = currentDock;
			baseView.Anchor = currentAnchor;
			baseView.MouseUp += new MouseEventHandler( PropertyFormTreeView_MouseDown );

			// Add our control back into where it was at before
			treeParent.Controls.Add( baseView );
		}

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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.PropertyFormTreeView = new System.Windows.Forms.TreeView();
			this.PropertyFormTreeViewPanel = new System.Windows.Forms.Panel();
			this.PropertyFormTextBoxPanel = new System.Windows.Forms.Panel();
			this.PropertyFormTextBoxLabel = new System.Windows.Forms.Label();
			this.PropertyFormTextBoxDeleteButton = new System.Windows.Forms.Button();
			this.PropertyFormTextBoxAddButton = new System.Windows.Forms.Button();
			this.PropertyFormTreeViewTextBox = new System.Windows.Forms.TextBox();
			this.PropertyFormSplitter = new System.Windows.Forms.Splitter();
			this.PropertyFormPropertyGridPanel = new System.Windows.Forms.Panel();
			this.PropertyFormPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.PropertyFormOKCancelButtonPanel = new System.Windows.Forms.Panel();
			this.PropertyFormExitButton = new System.Windows.Forms.Button();
			this.PropertyFormTextBoxDisplayLabel = new System.Windows.Forms.Label();
			this.PropertyFormTreeViewPanel.SuspendLayout();
			this.PropertyFormTextBoxPanel.SuspendLayout();
			this.PropertyFormPropertyGridPanel.SuspendLayout();
			this.PropertyFormOKCancelButtonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// PropertyFormTreeView
			// 
			this.PropertyFormTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertyFormTreeView.ImageIndex = -1;
			this.PropertyFormTreeView.Location = new System.Drawing.Point(0, 0);
			this.PropertyFormTreeView.Name = "PropertyFormTreeView";
			this.PropertyFormTreeView.SelectedImageIndex = -1;
			this.PropertyFormTreeView.Size = new System.Drawing.Size(316, 252);
			this.PropertyFormTreeView.TabIndex = 4;
			this.PropertyFormTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PropertyFormTreeView_MouseDown);
			// 
			// PropertyFormTreeViewPanel
			// 
			this.PropertyFormTreeViewPanel.Controls.Add(this.PropertyFormTextBoxPanel);
			this.PropertyFormTreeViewPanel.Controls.Add(this.PropertyFormTreeView);
			this.PropertyFormTreeViewPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.PropertyFormTreeViewPanel.Location = new System.Drawing.Point(0, 0);
			this.PropertyFormTreeViewPanel.Name = "PropertyFormTreeViewPanel";
			this.PropertyFormTreeViewPanel.Size = new System.Drawing.Size(316, 309);
			this.PropertyFormTreeViewPanel.TabIndex = 5;
			// 
			// PropertyFormTextBoxPanel
			// 
			this.PropertyFormTextBoxPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.PropertyFormTextBoxPanel.Controls.Add(this.PropertyFormTextBoxDisplayLabel);
			this.PropertyFormTextBoxPanel.Controls.Add(this.PropertyFormTextBoxLabel);
			this.PropertyFormTextBoxPanel.Controls.Add(this.PropertyFormTextBoxDeleteButton);
			this.PropertyFormTextBoxPanel.Controls.Add(this.PropertyFormTextBoxAddButton);
			this.PropertyFormTextBoxPanel.Controls.Add(this.PropertyFormTreeViewTextBox);
			this.PropertyFormTextBoxPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.PropertyFormTextBoxPanel.Location = new System.Drawing.Point(0, 249);
			this.PropertyFormTextBoxPanel.Name = "PropertyFormTextBoxPanel";
			this.PropertyFormTextBoxPanel.Size = new System.Drawing.Size(316, 60);
			this.PropertyFormTextBoxPanel.TabIndex = 5;
			// 
			// PropertyFormTextBoxLabel
			// 
			this.PropertyFormTextBoxLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.PropertyFormTextBoxLabel.Location = new System.Drawing.Point(8, 4);
			this.PropertyFormTextBoxLabel.Name = "PropertyFormTextBoxLabel";
			this.PropertyFormTextBoxLabel.Size = new System.Drawing.Size(100, 16);
			this.PropertyFormTextBoxLabel.TabIndex = 3;
			this.PropertyFormTextBoxLabel.Text = "Current Selection:";
			// 
			// PropertyFormTextBoxDeleteButton
			// 
			this.PropertyFormTextBoxDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertyFormTextBoxDeleteButton.Location = new System.Drawing.Point(228, 24);
			this.PropertyFormTextBoxDeleteButton.Name = "PropertyFormTextBoxDeleteButton";
			this.PropertyFormTextBoxDeleteButton.TabIndex = 2;
			this.PropertyFormTextBoxDeleteButton.Text = "Delete";
			// 
			// PropertyFormTextBoxAddButton
			// 
			this.PropertyFormTextBoxAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertyFormTextBoxAddButton.Location = new System.Drawing.Point(188, 24);
			this.PropertyFormTextBoxAddButton.Name = "PropertyFormTextBoxAddButton";
			this.PropertyFormTextBoxAddButton.Size = new System.Drawing.Size(32, 23);
			this.PropertyFormTextBoxAddButton.TabIndex = 1;
			this.PropertyFormTextBoxAddButton.Text = "Add";
			// 
			// PropertyFormTreeViewTextBox
			// 
			this.PropertyFormTreeViewTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertyFormTreeViewTextBox.Location = new System.Drawing.Point(8, 24);
			this.PropertyFormTreeViewTextBox.Name = "PropertyFormTreeViewTextBox";
			this.PropertyFormTreeViewTextBox.Size = new System.Drawing.Size(172, 20);
			this.PropertyFormTreeViewTextBox.TabIndex = 0;
			this.PropertyFormTreeViewTextBox.Text = "";
			// 
			// PropertyFormSplitter
			// 
			this.PropertyFormSplitter.Location = new System.Drawing.Point(316, 0);
			this.PropertyFormSplitter.Name = "PropertyFormSplitter";
			this.PropertyFormSplitter.Size = new System.Drawing.Size(3, 309);
			this.PropertyFormSplitter.TabIndex = 6;
			this.PropertyFormSplitter.TabStop = false;
			// 
			// PropertyFormPropertyGridPanel
			// 
			this.PropertyFormPropertyGridPanel.Controls.Add(this.PropertyFormPropertyGrid);
			this.PropertyFormPropertyGridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PropertyFormPropertyGridPanel.Location = new System.Drawing.Point(319, 0);
			this.PropertyFormPropertyGridPanel.Name = "PropertyFormPropertyGridPanel";
			this.PropertyFormPropertyGridPanel.Size = new System.Drawing.Size(345, 309);
			this.PropertyFormPropertyGridPanel.TabIndex = 7;
			// 
			// PropertyFormPropertyGrid
			// 
			this.PropertyFormPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertyFormPropertyGrid.CommandsVisibleIfAvailable = true;
			this.PropertyFormPropertyGrid.LargeButtons = false;
			this.PropertyFormPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.PropertyFormPropertyGrid.Location = new System.Drawing.Point(0, 0);
			this.PropertyFormPropertyGrid.Name = "PropertyFormPropertyGrid";
			this.PropertyFormPropertyGrid.Size = new System.Drawing.Size(344, 268);
			this.PropertyFormPropertyGrid.TabIndex = 1;
			this.PropertyFormPropertyGrid.Tag = "";
			this.PropertyFormPropertyGrid.Text = "PropertyFormPropertyGrid";
			this.PropertyFormPropertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.PropertyFormPropertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// PropertyFormOKCancelButtonPanel
			// 
			this.PropertyFormOKCancelButtonPanel.Controls.Add(this.PropertyFormExitButton);
			this.PropertyFormOKCancelButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.PropertyFormOKCancelButtonPanel.Location = new System.Drawing.Point(319, 273);
			this.PropertyFormOKCancelButtonPanel.Name = "PropertyFormOKCancelButtonPanel";
			this.PropertyFormOKCancelButtonPanel.Size = new System.Drawing.Size(345, 36);
			this.PropertyFormOKCancelButtonPanel.TabIndex = 8;
			// 
			// PropertyFormExitButton
			// 
			this.PropertyFormExitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.PropertyFormExitButton.Location = new System.Drawing.Point(264, 8);
			this.PropertyFormExitButton.Name = "PropertyFormExitButton";
			this.PropertyFormExitButton.TabIndex = 0;
			this.PropertyFormExitButton.Text = "Exit";
			this.PropertyFormExitButton.Click += new System.EventHandler(this.PropertyFormExitButton_Click);
			// 
			// PropertyFormTextBoxDisplayLabel
			// 
			this.PropertyFormTextBoxDisplayLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.PropertyFormTextBoxDisplayLabel.Location = new System.Drawing.Point(104, 4);
			this.PropertyFormTextBoxDisplayLabel.Name = "PropertyFormTextBoxDisplayLabel";
			this.PropertyFormTextBoxDisplayLabel.Size = new System.Drawing.Size(204, 20);
			this.PropertyFormTextBoxDisplayLabel.TabIndex = 4;
			// 
			// PropertyGridForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(664, 309);
			this.Controls.Add(this.PropertyFormOKCancelButtonPanel);
			this.Controls.Add(this.PropertyFormPropertyGridPanel);
			this.Controls.Add(this.PropertyFormSplitter);
			this.Controls.Add(this.PropertyFormTreeViewPanel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(512, 284);
			this.Name = "PropertyGridForm";
			this.Text = "MOG Properties Grid";
			this.PropertyFormTreeViewPanel.ResumeLayout(false);
			this.PropertyFormTextBoxPanel.ResumeLayout(false);
			this.PropertyFormPropertyGridPanel.ResumeLayout(false);
			this.PropertyFormOKCancelButtonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void PropertyFormExitButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void PropertyFormTreeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			TreeView baseView = (TreeView)sender;
			TreeNode selectedNode = baseView.GetNodeAt( e.X, e.Y );
			this.PropertyFormPropertyGrid.SelectedObject = selectedNode.Tag;
			this.PropertyFormTextBoxDisplayLabel.Text = selectedNode.Text;
		}
	} // end class
} // end ns
