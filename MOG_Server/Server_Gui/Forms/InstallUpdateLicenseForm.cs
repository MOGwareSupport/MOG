using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MOG;
using MOG.CONTROLLER.CONTROLLERSYSTEM;
using System.IO;
using Server_Gui;
using MOG.PROMPT;

namespace MOG_Server.Server_Gui.Forms
{
	public partial class InstallUpdateLicenseForm : Form
	{
		string mLicenseFile = "";
		
		public InstallUpdateLicenseForm()
		{
			InitializeComponent();

			string license = Path.Combine(MOG_Main.GetExecutablePath_StripCurrentDirectory(), "mog_license");
			if (File.Exists(license))
			{
				MOG_TimeBomb timeBomb = new MOG_TimeBomb(license);
				PopulateLicenseInfo(timeBomb, "MOG_License");
			}
			else
			{
				LicenseRichTextBox.Text = "";
				LicenseRichTextBox.Text += "UnLicenced Server\n";
				LicenseRichTextBox.Text += "Total UnLicensed Connections: 4 \n\n";
			}
		}

		private void LoadLicenseFile()
		{
			if (LicenseOpenFileDialog.ShowDialog() == DialogResult.OK)
			{
				MOG_TimeBomb timeBomb = new MOG_TimeBomb(LicenseOpenFileDialog.FileName);
				if (PopulateLicenseInfo(timeBomb, LicenseOpenFileDialog.FileName))
				{
					try
					{
						string installedLicense = Path.Combine(MOG_Main.GetExecutablePath_StripCurrentDirectory(), "mog_license");
						File.Copy(LicenseOpenFileDialog.FileName, installedLicense, true);

						MessageBox.Show(string.Format("Restart your server for this new license file to take effect"), "MOG License");
					}
					catch (Exception e)
					{
						MessageBox.Show(string.Format("Unable to deploy license file with error {0}!", e.Message), "MOG License" );
					}
				}
			}
		}

		private bool PopulateLicenseInfo(MOG_TimeBomb timeBomb, string licenseFilename)
		{
			if (timeBomb.IsValid())
			{
				mLicenseFile = licenseFilename;
				LicenseRichTextBox.Enabled = true;

				LicenseRichTextBox.Text = "";
				LicenseRichTextBox.Text += "Licence file: " + licenseFilename + "\n\n";
				LicenseRichTextBox.Text += "Licenced server MAC address:" + timeBomb.GetRegisteredMacAddress() + "\n";
				LicenseRichTextBox.Text += "License Creation Date:" + timeBomb.GetInstallDate().ToString() + "\n";
				LicenseRichTextBox.Text += "License Expiration Date:" + timeBomb.GetExpireDate().ToString() + "\n\n";
				LicenseRichTextBox.Text += "Total Licenses:" + timeBomb.GetClientLicenseCount().ToString() + "\n\n";
				LicenseRichTextBox.Text += "Disabled Features:" + timeBomb.GetDisabledFeatureList().Trim("[]".ToCharArray()) + "\n";

				return true;
			}
			else
			{
				LicenseRichTextBox.Text = "UnLicenced Server\n";
				LicenseRichTextBox.Text += "Total UnLicensed Connections: 4 \n\n";
				return false;
			}
		}

		private void LicenseLoadButton_Click(object sender, EventArgs e)
		{
			LoadLicenseFile();
		}

		private void LicenseCloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}