using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Client.Forms.ToolBoxForms.ToolBoxHelpers
{
	/// <summary>
	/// Summary description for ToolBoxSaveLocationForm.
	/// </summary>
	public class ToolBoxSaveLocationForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Button CancelButton1;
		private System.Windows.Forms.ToolTip toolTip;
		public System.Windows.Forms.ComboBox LocationComboBox;
		private System.ComponentModel.IContainer components;

		public ToolBoxSaveLocationForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Initialize our ToolBoxSaveLocationForm with the ability to
		///		save to Project, OR save to Department.  (At least on of these
		///		values should be true, if you want to use this form--otherwise,
		///		what's the point?)
		/// </summary>
		/// <param name="saveToProject">True if user has right to save to Project</param>
		/// <param name="saveToDepartment">True if use has right to save to Department</param>
		public ToolBoxSaveLocationForm( bool saveToProject, bool saveToDepartment )
			:this()
		{
			if( this.LocationComboBox.Items.Count > 0 )
				this.LocationComboBox.Items.Clear();
			this.LocationComboBox.Items.Add( ToolBoxControlLocation.User.ToString() );

			if( saveToDepartment )
			{
				LocationComboBox.Items.Add( ToolBoxControlLocation.Department.ToString() );
			}
			if( saveToProject )
			{
				LocationComboBox.Items.Add( ToolBoxControlLocation.Project.ToString() );
			}

			// Go ahead and select our first item (User)
			this.LocationComboBox.SelectedIndex = 0;
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
			this.components = new System.ComponentModel.Container();
			this.LocationComboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.OKButton = new System.Windows.Forms.Button();
			this.CancelButton1 = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// LocationComboBox
			// 
			this.LocationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.LocationComboBox.Location = new System.Drawing.Point(8, 40);
			this.LocationComboBox.Name = "LocationComboBox";
			this.LocationComboBox.Size = new System.Drawing.Size(264, 21);
			this.LocationComboBox.TabIndex = 0;
			this.toolTip.SetToolTip(this.LocationComboBox, "Select a location here.  User is for user-only, Department will add this control " +
				"to the whole department, Project will add it to the entire project");
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(264, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Please Select a Save Location:";
			// 
			// OKButton
			// 
			this.OKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKButton.Location = new System.Drawing.Point(60, 72);
			this.OKButton.Name = "OKButton";
			this.OKButton.TabIndex = 2;
			this.OKButton.Text = "OK";
			// 
			// CancelButton1
			// 
			this.CancelButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.CancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton1.Location = new System.Drawing.Point(148, 72);
			this.CancelButton1.Name = "CancelButton1";
			this.CancelButton1.TabIndex = 3;
			this.CancelButton1.Text = "Cancel";
			// 
			// ToolBoxSaveLocationForm
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.CancelButton1;
			this.ClientSize = new System.Drawing.Size(280, 109);
			this.Controls.Add(this.CancelButton1);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.LocationComboBox);
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(288, 136);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(288, 136);
			this.Name = "ToolBoxSaveLocationForm";
			this.Text = "Save Control To Selected Location...";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
