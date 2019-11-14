using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MOG_Photoshop_Exporter;
using MOG.REPORT;

using ps = Photoshop;

namespace MOG_Client
{
	/// <summary>
	/// Summary description for PhotoshopImportForm.
	/// </summary>
	public class PhotoshopImportForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Button MOG_ExportActiveButton;
		private System.Windows.Forms.Button MOG_ExportAllButton;
		private System.ComponentModel.IContainer components;

		private string mTargetDir;
		public System.Windows.Forms.Label PhotoshopLabel;
		private MogMainForm mainForm;

		public PhotoshopImportForm(string targetDir, MogMainForm main)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mainForm = main;
			mTargetDir = targetDir;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PhotoshopImportForm));
			this.MOG_ExportActiveButton = new System.Windows.Forms.Button();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.MOG_ExportAllButton = new System.Windows.Forms.Button();
			this.PhotoshopLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// MOG_ExportActiveButton
			// 
			this.MOG_ExportActiveButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.MOG_ExportActiveButton.ImageIndex = 0;
			this.MOG_ExportActiveButton.ImageList = this.imageList1;
			this.MOG_ExportActiveButton.Location = new System.Drawing.Point(3, 16);
			this.MOG_ExportActiveButton.Name = "MOG_ExportActiveButton";
			this.MOG_ExportActiveButton.Size = new System.Drawing.Size(32, 32);
			this.MOG_ExportActiveButton.TabIndex = 0;
			this.MOG_ExportActiveButton.Click += new System.EventHandler(this.MOG_ExportActiveButton_Click);
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Magenta;
			// 
			// MOG_ExportAllButton
			// 
			this.MOG_ExportAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.MOG_ExportAllButton.ImageIndex = 1;
			this.MOG_ExportAllButton.ImageList = this.imageList1;
			this.MOG_ExportAllButton.Location = new System.Drawing.Point(37, 16);
			this.MOG_ExportAllButton.Name = "MOG_ExportAllButton";
			this.MOG_ExportAllButton.Size = new System.Drawing.Size(32, 32);
			this.MOG_ExportAllButton.TabIndex = 1;
			this.MOG_ExportAllButton.Click += new System.EventHandler(this.MOG_ExportAllButton_Click);
			// 
			// PhotoshopLabel
			// 
			this.PhotoshopLabel.Location = new System.Drawing.Point(8, 0);
			this.PhotoshopLabel.Name = "PhotoshopLabel";
			this.PhotoshopLabel.Size = new System.Drawing.Size(56, 16);
			this.PhotoshopLabel.TabIndex = 2;
			this.PhotoshopLabel.Text = "PS V. 7";
			// 
			// PhotoshopImportForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(72, 48);
			this.Controls.Add(this.PhotoshopLabel);
			this.Controls.Add(this.MOG_ExportAllButton);
			this.Controls.Add(this.MOG_ExportActiveButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PhotoshopImportForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "MOG";
			this.TopMost = true;
			this.LocationChanged += new System.EventHandler(this.PhotoshopImportForm_LocationChanged);
			this.Closed += new System.EventHandler(this.PhotoshopImportForm_Closed);
			this.ResumeLayout(false);

		}
		#endregion

		private void MOG_ExportActiveButton_Click(object sender, System.EventArgs e)
		{
			if (string.Compare(this.PhotoshopLabel.Text, "P.S. V.9") == 0)
			{
				try
				{					
					ps.ApplicationClass app = new ps.ApplicationClass();
					ps.Document doc = app.ActiveDocument;
				
					ps.PhotoshopSaveOptionsClass psSaveOptions = new ps.PhotoshopSaveOptionsClass();
					psSaveOptions.AlphaChannels = true;
					psSaveOptions.Annotations = true;
					psSaveOptions.Layers = true;
					psSaveOptions.SpotColors = true;

					string FullFilename = mTargetDir + "\\" + doc.Name;
					
					doc.SaveAs(FullFilename, psSaveOptions, true, ps.PsExtensionType.psUppercase);
				}
				catch(Exception ex)
				{					
					MOG_REPORT.ShowErrorMessageBox("Photoshop CS import", "Could not initialize a compatible link between MOG and your version of Photoshop\nMessage:" + ex.ToString());
					return;
				}
			}
			else if (string.Compare(this.PhotoshopLabel.Text, "P.S. V.7") == 0)
			{
				try
				{
					MOG_Export export = new MOG_Export();
		
					export.LaunchExportActive(mTargetDir);
				}
				catch(Exception ex)
				{
					MOG_REPORT.ShowErrorMessageBox("Photoshop 7 import", "Could not initialize a compatible link between MOG and your version of Photoshop\nMessage:" + ex.ToString());
					return;
				}
			}
		}

		private void MOG_ExportAllButton_Click(object sender, System.EventArgs e)
		{
			if (string.Compare(this.PhotoshopLabel.Text, "P.S. V.9") == 0)
			{
				try
				{
					ps.ApplicationClass app = new ps.ApplicationClass();
					ps.Documents doc_arr = app.Documents;
					foreach (ps.Document doc in doc_arr)
					{
						ps.PhotoshopSaveOptionsClass psSaveOptions = new ps.PhotoshopSaveOptionsClass();
						psSaveOptions.AlphaChannels = true;
						psSaveOptions.Annotations = true;
						psSaveOptions.Layers = true;
						psSaveOptions.SpotColors = true;

						string FullFilename = mTargetDir + "\\" + doc.Name;
					
						doc.SaveAs(FullFilename, psSaveOptions, true, ps.PsExtensionType.psUppercase);
					}
				}
				catch(Exception ex)
				{
					MOG_REPORT.ShowErrorMessageBox("Photoshop CS import", "Could not initialize a compatible link between MOG and your version of Photoshop\nMessage:" + ex.ToString());
					return;
				}
			}
			else if (string.Compare(this.PhotoshopLabel.Text, "P.S. V.7") == 0)
			{
				try
				{
					MOG_Export export = new MOG_Export();
		
					export.LaunchExportAll(mTargetDir);		
				}
				catch(Exception ex)
				{
					MOG_REPORT.ShowErrorMessageBox("Photoshop 7 import", "Could not initialize a compatible link between MOG and your version of Photoshop\nMessage:" + ex.ToString());
					return;
				}
			}
		}

		private void PhotoshopImportForm_LocationChanged(object sender, System.EventArgs e)
		{
			mainForm.mStartup.mPhotoshopImportWindowX = Location.X;
			mainForm.mStartup.mPhotoshopImportWindowY = Location.Y;
		}

		private void PhotoshopImportForm_Closed(object sender, System.EventArgs e)
		{
			mainForm.mStartup.mPhotoshopImportWindow = false;
		}

	}
}
