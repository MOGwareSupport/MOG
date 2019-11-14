using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

using MOG.DOSUTILS;

namespace MOG_ServerManager.Utilities
{
	/// <summary>
	/// Summary description for MogUtil_HelpManager.
	/// </summary>
	public class MogUtil_HelpManager
	{
		static private string mHelpDoc = "";

		public MogUtil_HelpManager()
		{
		}
		
		static public string DetectValidHotSpot(MOG_ServerManagerMainForm mainForm)
		{
			try
			{				
				mainForm.StartPageInfoRichTextBox.SelectAll();
				mainForm.StartPageInfoRichTextBox.SelectionColor = System.Drawing.Color.SteelBlue;				
				
				Point loc = mainForm.StartPageInfoRichTextBox.PointToClient(Cursor.Position);
			
				int chIndex;
				int i = chIndex = mainForm.StartPageInfoRichTextBox.GetCharIndexFromPosition(loc);
			
				while (mainForm.StartPageInfoRichTextBox.Text.Length -1 > i && mainForm.StartPageInfoRichTextBox.Text[++i] != ' ');
				while (chIndex != 0 && mainForm.StartPageInfoRichTextBox.Text[--chIndex] != ' ');

				mainForm.StartPageInfoRichTextBox.Select(chIndex, i-chIndex);

				string str = mainForm.StartPageInfoRichTextBox.SelectedText.Trim(". ,/?;':\"!@#$%^&*()-=_+{}[]\\|`~".ToCharArray());				
				
				return str;
			}
			catch(Exception ex)
			{
				ex.ToString();
			}
			return "";
		}

		static public void LoadHelpText(MOG_ServerManagerMainForm mainForm, string doc)
		{			
			if (DosUtils.FileExistFast(doc))
			{
				mainForm.StartPageInfoRichTextBox.LoadFile(doc, RichTextBoxStreamType.RichText);
				mHelpDoc = doc;
				mainForm.StartPageInfoRichTextBox.SelectAll();
				mainForm.StartPageInfoRichTextBox.SelectionColor = System.Drawing.Color.SteelBlue;
			}
		}

		static private bool StringInHotspotList(string []hotspots, string str)
		{
			foreach (string val in hotspots)
			{
				if (val == str)
					return true;
			}

			return false;
		}

		static private void HilightControl(Control ctrl, int flashLoop, int millisecondDelay)
		{
			for (int i = 0; i < flashLoop; i++)
			{
				ctrl.BackColor = SystemColors.ControlDark;
				Application.DoEvents();
				Thread.Sleep(millisecondDelay);				
			}	
		
			ctrl.BackColor = SystemColors.Window;				
		}

		static public void HelpTextAction(MOG_ServerManagerMainForm mainForm, bool clicked)
		{
			string HotSpotLabel = DetectValidHotSpot(mainForm).ToLower();

			switch(mHelpDoc)
			{
				case "WelcomeServerManager.rtf":
					#region Home events
					if (StringInHotspotList( new string[] {"created", "monitored", "deployed", "projects"}, HotSpotLabel))
					{
						Cursor.Current = Cursors.Hand;
						if (clicked)
						{
							switch(HotSpotLabel)
							{
								case "created":
									mainForm.MOGMainTabControl.SelectedIndex = 1;
									break;
								case "monitored":
									mainForm.MOGMainTabControl.SelectedIndex = 2;
									break;
								case "deployed":
									mainForm.MOGMainTabControl.SelectedIndex = 3;
									break;
								case "projects":
									mainForm.MOGMainTabControl.SelectedIndex = 1;
									break;
							}
						}
					}
					else
					{
						Cursor.Current = Cursors.Default;
					}
					break;
					#endregion
				case "ProjectConfiguration.rtf":
					#region Projects events
					if (StringInHotspotList( new string[] {"create", "configure", "demo"}, HotSpotLabel))
					{
						Cursor.Current = Cursors.Hand;
						if (clicked)
						{
							switch(HotSpotLabel)
							{
								case "create":
									mainForm.miNewProject_Click(mainForm, null);
									break;
								case "configure":
									mainForm.miConfigureProject_Click(mainForm, null); 
									break;
								case "demo":
									HilightControl(mainForm.lvDemoProjects, 3, 100);
									break;								
							}
						}						
					}
					else
					{
						Cursor.Current = Cursors.Default;
					}
					break;
					#endregion
				case "TrashPolicy.rtf":
					#region Trash events
					if (StringInHotspotList( new string[] {"purge"}, HotSpotLabel))
					{
						Cursor.Current = Cursors.Hand;
						if (clicked)
						{
							switch(HotSpotLabel)
							{
								case "purge":
									mainForm.trashPolicyEditorControl1.btnPurge_Click(mainForm, null);
									break;								
							}
						}
					}
					else
					{
						Cursor.Current = Cursors.Default;
					}
					break;
					#endregion
				case "VersionManager.rtf":
					#region Version events
					if (StringInHotspotList( new string[] {"deployed"}, HotSpotLabel))
					{
						Cursor.Current = Cursors.Hand;
						if (clicked)
						{
							switch(HotSpotLabel)
							{
								case "deployed":
									mainForm.VersionDeployButton_Click(mainForm, null);
									break;								
							}
						}
					}
					else
					{
						Cursor.Current = Cursors.Default;
					}
					break;
					#endregion
				case "RemoteServers.rtf":
					#region Remote servers events
					if (StringInHotspotList(new string[] { "enabling", "create" }, HotSpotLabel))
					{
						Cursor.Current = Cursors.Hand;
						if (clicked)
						{
							switch (HotSpotLabel)
							{
								case "enabling":
									mainForm.RemoteServerSettings.EnableRemoteServers = true;
									break;
								case "create":
									//mainForm.VersionDeployButton_Click(mainForm, null);
									break;								
							}
						}
					}
					else
					{
						Cursor.Current = Cursors.Default;
					}
					break;
					#endregion
			}

			if (Cursor.Current == Cursors.Hand)
			{
				mainForm.StartPageInfoRichTextBox.SelectionColor = Color.Blue;
			}
			else 
			{
				mainForm.StartPageInfoRichTextBox.SelectionColor = Color.SteelBlue;
			}

			if (clicked)
			{
				mainForm.MOGMainTabControl.Focus();
			}
		}
	}
}
