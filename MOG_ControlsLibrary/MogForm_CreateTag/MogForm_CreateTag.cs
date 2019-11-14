using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MOG.CONTROLLER.CONTROLLERPROJECT;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERSYNCDATA;

namespace MOG_ControlsLibrary.MogForm_CreateTag
{
	public partial class MogForm_CreateTag : Form
	{
		public MogForm_CreateTag()
		{
			InitializeComponent();
		}

		private void TagOkButton_Click(object sender, EventArgs e)
		{
			if (TagNameTextBox.Text.Length != 0)
			{
				if (MOG_ControllerProject.TagCreate(MOG_ControllerProject.GetCurrentSyncDataController(), TagNameTextBox.Text))
				{
					Close();
				}
				else
				{
					MOG_Prompt.PromptMessage("Create Tag", "Error in creating the tag");
				}
			}
		}
	}
}