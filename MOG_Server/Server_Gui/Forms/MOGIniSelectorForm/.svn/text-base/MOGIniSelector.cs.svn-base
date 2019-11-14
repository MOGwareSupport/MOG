using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MOG_Server.Server_Gui
{
	/// <summary>
	/// Summary description for MOGIniSelector.
	/// </summary>
	public class MOGIniSelectorForm : System.Windows.Forms.Form
	{
		#region System definitions
		private MOG_Server.Server_Gui.MOGIniSelector mogIniSelector;
		private System.Windows.Forms.CheckBox chbxShowAgain;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MOGIniSelectorForm));
			this.mogIniSelector = new MOG_Server.Server_Gui.MOGIniSelector();
			this.chbxShowAgain = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// mogIniSelector
			// 
			this.mogIniSelector.CancelButtonVisible = true;
			this.mogIniSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mogIniSelector.LocateButtonVisible = true;
			this.mogIniSelector.Location = new System.Drawing.Point(0, 0);
			this.mogIniSelector.Name = "mogIniSelector";
			this.mogIniSelector.OKButtonVisible = true;
			this.mogIniSelector.RemoveButtonVisible = true;
			this.mogIniSelector.Size = new System.Drawing.Size(520, 230);
			this.mogIniSelector.TabIndex = 0;
			// 
			// chbxShowAgain
			// 
			this.chbxShowAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chbxShowAgain.Checked = true;
			this.chbxShowAgain.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chbxShowAgain.Location = new System.Drawing.Point(192, 200);
			this.chbxShowAgain.Name = "chbxShowAgain";
			this.chbxShowAgain.Size = new System.Drawing.Size(152, 24);
			this.chbxShowAgain.TabIndex = 1;
			this.chbxShowAgain.Text = "Show this dialog again";
			// 
			// MOGIniSelectorForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(520, 230);
			this.Controls.Add(this.chbxShowAgain);
			this.Controls.Add(this.mogIniSelector);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MOGIniSelectorForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Repository and INI File";
			this.ResumeLayout(false);

		}
		#endregion
		#endregion
		#region Member vars
		#endregion
		#region Properties
		public string SelectedINIFilename
		{
			get { return this.mogIniSelector.SelectedINIFilename; }
		}
		public string SelectedINIPath
		{
			get { return this.mogIniSelector.SelectedINIPath; }
		}
		public string SelectedRepositoryPath
		{
			get { return this.mogIniSelector.SelectedRepositoryPath; }
		}
		public bool ShowAgainChecked
		{
			get { return this.chbxShowAgain.Checked; }
		}
		public bool CancelButtonVisible
		{
			get { return this.mogIniSelector.CancelButtonVisible; }
			set { this.mogIniSelector.CancelButtonVisible = value; }
		}
		public bool OKButtonVisible
		{
			get { return this.mogIniSelector.OKButtonVisible; }
			set { this.mogIniSelector.OKButtonVisible = value; }
		}

		#endregion
		#region Constructors
		public MOGIniSelectorForm()
		{
			InitializeComponent();
			this.mogIniSelector.OKClicked += new EventHandler(mogIniSelector_OKClicked);
			this.mogIniSelector.CancelClicked += new EventHandler(mogIniSelector_CancelClicked);
		}
		public MOGIniSelectorForm(ArrayList names, ArrayList paths)
		{
			InitializeComponent();

			this.mogIniSelector.OKClicked += new EventHandler(mogIniSelector_OKClicked);
			this.mogIniSelector.CancelClicked += new EventHandler(mogIniSelector_CancelClicked);
			if (names.Count == paths.Count)
			{
				for (int i=0; i<names.Count; i++)
					AddRepository((string)names[i], (string)paths[i]);
			}
		}
		#endregion
		#region Public functions
		public void AddShallowRepositories()
		{
			this.mogIniSelector.AddShallowRepositories();
		}

		public void LoadShallowRepositories()
		{
			this.mogIniSelector.LoadShallowRepositories();
		}

		public void AddRepository(string name, string path)
		{
			this.mogIniSelector.AddRepository(name, path);
		}
		
		public bool SaveRepositories(string iniFilename)
		{
			return this.mogIniSelector.SaveRepositories(iniFilename);
		}

		public bool LoadRepositories(string iniFilename)
		{
			return this.mogIniSelector.LoadRepositories(iniFilename);
		}
		#endregion
		#region Event handlers
		private void mogIniSelector_OKClicked(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Visible = false;
		}
		
		private void mogIniSelector_CancelClicked(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Visible = false;
		}
		#endregion
	}
}

/*
#region System definitions
#endregion
#region Member vars
#endregion
#region Properties
#endregion
#region Constructors
#endregion
#region Public functions
#endregion
#region Private functions
#endregion
#region Event handlers
#endregion
*/ 
