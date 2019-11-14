using System;
using System.Collections.Generic;
using System.Text;

namespace MOG_Client.Client_Mog_Utilities
{
	public class MainMenuAboutClass
	{
		internal static void SetMogLibraryMode(MogMainForm mogMainForm)
		{
			mogMainForm.aboutMOGToolStripMenuItem.Text = "About Mog Library...";

			mogMainForm.issueTrackerToolStripMenuItem.Visible = false;
			mogMainForm.toolStripSeparator12.Visible = false;
		}
	}
}
