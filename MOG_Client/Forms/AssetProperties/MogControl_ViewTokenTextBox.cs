using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MOG;
using MOG.TIME;
using MOG.FILENAME;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.CONTROLLER.CONTROLLERPACKAGE;

namespace MOG_Client.Forms.AssetProperties
{
	/// <summary>
	/// Summary description for MogControl_ViewTokenTextBox.
	/// </summary>
	public class MogControl_ViewTokenTextBox : System.Windows.Forms.UserControl
	{
		private ComboBox mComboBox;
		private TextBox mTextBox;
		private MOG_Filename mAssetFilename;
		private System.Windows.Forms.TextBox ViewTokenTextBox;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		[Category("SourceTokenControl"), Description("Source control to de-tokenize")]
		public ComboBox SourceComboBox
		{
			set 
			{ 
				this.mComboBox = value;
				if (mComboBox != null)
				{
					mComboBox.TextChanged += new EventHandler(mComboBox_TextChanged);
				}
			}
			get { return this.mComboBox;}
		}

		[Category("SourceTokenControl"), Description("Source control to de-tokenize")]
		public TextBox SourceTextBox
		{
			set 
			{ 
				this.mTextBox = value;
				if (mTextBox != null)
				{
					mTextBox.TextChanged += new EventHandler(mTextBox_TextChanged);
				}
			}
			get { return this.mTextBox;}
		}

		public MOG_Filename MOGAssetFilename
		{
			set 
			{ 
				this.mAssetFilename = value;				
			}
			get { return this.mAssetFilename;}
		}

		public MogControl_ViewTokenTextBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ViewTokenTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// ViewTokenTextBox
			// 
			this.ViewTokenTextBox.BackColor = System.Drawing.SystemColors.InactiveBorder;
			this.ViewTokenTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ViewTokenTextBox.Location = new System.Drawing.Point(0, 0);
			this.ViewTokenTextBox.Name = "ViewTokenTextBox";
			this.ViewTokenTextBox.Size = new System.Drawing.Size(232, 20);
			this.ViewTokenTextBox.TabIndex = 0;
			this.ViewTokenTextBox.Text = "";
			// 
			// MogControl_ViewTokenTextBox
			// 
			this.Controls.Add(this.ViewTokenTextBox);
			this.Name = "MogControl_ViewTokenTextBox";
			this.Size = new System.Drawing.Size(232, 24);
			this.ResumeLayout(false);

		}
		#endregion

		private string GetFormattedString(string formatedSource)
		{
			string seeds = "";
			if (MOG_ControllerProject.GetUser() != null)
			{
				seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetPackageTokenSeeds("Game~Packages{All}PackageFile.Pak", "All", "", "", "Packages\\PackageFile.Pak"));
			}
			
			if (MOG_ControllerProject.GetProject() != null)
			{
				seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetProjectTokenSeeds(MOG_ControllerProject.GetProject()));
			}
			
			if (mAssetFilename != null)
			{
				seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetFilenameTokenSeeds(mAssetFilename));
				seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetRipperTokenSeeds(mAssetFilename.GetEncodedFilename() + "\\Files.Imported", "*.*", mAssetFilename.GetEncodedFilename() + "\\Files.All"));
			}
			
			seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetSystemTokenSeeds());
			seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetOSTokenSeeds());
			seeds = MOG_Tokens.AppendTokenSeeds(seeds, MOG_Tokens.GetTimeTokenSeeds(new MOG_Time()));


			return MOG_Tokens.GetFormattedString(formatedSource, seeds);
		}

		private void mComboBox_TextChanged(object sender, EventArgs e)
		{	
			this.ViewTokenTextBox.Text = GetFormattedString(mComboBox.Text);
		}

		private void mTextBox_TextChanged(object sender, EventArgs e)
		{
			this.ViewTokenTextBox.Text = GetFormattedString(mTextBox.Text);
		}
	}
}
