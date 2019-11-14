using System;
using System.Collections.Generic;
using System.Text;

namespace MOG_Client.Client_Mog_Utilities
{
	public class MainMenuEditClass
	{
		internal static void SetMogLibraryMode(MogMainForm mogMainForm)
		{
			mogMainForm.showOldStyleWorkspaceButtonsToolStripMenuItem.Visible = false;
			mogMainForm.optionsToolStripMenuItem.Visible = false;
		}
	}
}
