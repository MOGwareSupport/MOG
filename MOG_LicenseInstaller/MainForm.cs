using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MOG;

namespace MOG_LicenseInstaller
{
	public partial class MainForm : Form
	{
		string mLicenseFile = "";
		string mServerPath = "";
		string mLicenseInstallPath = "";

		public MainForm()
		{
			InitializeComponent();
			
			InitializeServerPath();
		}

		private void InitializeServerPath()
		{
			LicenseInstallButton.Enabled = false;
			FindServerPath();

			if (mServerPath.Length > 0)
			{
				string installedServerPath = Path.GetDirectoryName(mServerPath);
				mLicenseInstallPath = installedServerPath.Substring(0, installedServerPath.LastIndexOf("\\"));

				LicenseServerLabel.Text = mServerPath;
				LicenseInstallButton.Enabled = true;
			}
			else
			{
				LicenseServerLabel.Text = "Could not locate valid installed MOG Server...";
			}
		}

		private void FindServerPath()
		{
			string mogPath = Environment.GetEnvironmentVariable("MOG_PATH");
			if (ValidateServerFolder(mogPath).Length != 0)
			{
				mServerPath = ValidateServerFolder(mogPath);
			}
			else
			{
				string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				string mogwarePath = Path.Combine(programFiles, "Mogware");
				if (Directory.Exists(mogwarePath))
				{
					foreach (string path in Directory.GetDirectories(mogwarePath))
					{
						string serverPath = ValidateServerFolder(path);
						if (serverPath.Length > 0)
						{
							mServerPath = serverPath;
							return;
						}
					}
				}
				if (MessageBox.Show("Could not locate your installed MOG Server\n\nWould you like to manually locate it?", "License installation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
				{
				AskUserForDir:
					// We could not find it, Ask the user...
					FolderBrowserDialog targetDir = new FolderBrowserDialog();
					targetDir.ShowNewFolderButton = false;
					targetDir.RootFolder = Environment.SpecialFolder.ProgramFiles;
					targetDir.Description = "Browse to your MOG Server installation directory:";
					if (targetDir.ShowDialog(this) == DialogResult.OK)
					{
						string serverPath = ValidateServerFolder(targetDir.SelectedPath);
						if (serverPath.Length > 0)
						{
							mServerPath = serverPath;
							return;
						}
						else
						{
							MessageBox.Show("Could not locate MOG Server.exe at that path\nPlease try again.", "License installation", MessageBoxButtons.OK);
							goto AskUserForDir;
						}
					}
				}
			}
		}

		private string ValidateServerFolder(string path)
		{
			string MogServerExe = Path.Combine(path, "Current\\Mog_Server.Exe");
			if (File.Exists(MogServerExe))
			{
				// Set target server
				return MogServerExe;
			}

			// Check if we can find it without using current
			MogServerExe = Path.Combine(path, "Mog_Server.Exe");
			if (File.Exists(MogServerExe))
			{
				// Set target server
				return MogServerExe;
			}


			return "";
		}

		private void LicenseLoadButton_Click(object sender, EventArgs e)
		{
			LoadLicenseFile();
		}

		private void LoadLicenseFile()
		{
			if (LicenseOpenFileDialog.ShowDialog() == DialogResult.OK)
			{
				MOG_TimeBomb timeBomb = new MOG_TimeBomb(LicenseOpenFileDialog.FileName);
				if (timeBomb.IsValid())
				{
					mLicenseFile = LicenseOpenFileDialog.FileName;
					LicenseRichTextBox.Enabled = true;

					LicenseRichTextBox.Text = "";
					LicenseRichTextBox.Text += "Licence file: " + LicenseOpenFileDialog.FileName + "\n\n";
					LicenseRichTextBox.Text += "Licenced server MAC address:" + timeBomb.GetRegisteredMacAddress() + "\n";
					LicenseRichTextBox.Text += "License Creation Date:" + timeBomb.GetInstallDate().ToString() + "\n";
					LicenseRichTextBox.Text += "License Expiration Date:" + timeBomb.GetExpireDate().ToString() + "\n\n";
					LicenseRichTextBox.Text += "Total Licenses:" + timeBomb.GetClientLicenseCount().ToString() + "\n\n";
					LicenseRichTextBox.Text += "Disabled Features:" + timeBomb.GetDisabledFeatureList().Trim("[]".ToCharArray()) + "\n";
				}
				else
				{
					MessageBox.Show("File (" + LicenseOpenFileDialog.FileName + ")\ndoes not seem to be a valid license file", "Incompatible License!", MessageBoxButtons.OK);
					LicenseRichTextBox.Enabled = false;
				}
			}
		}

		private void LicenseInstallButton_Click(object sender, EventArgs e)
		{
			installLicense:

			if (mLicenseFile.Length > 0 && File.Exists(mLicenseFile))
			{
				if (MessageBox.Show("Are you sure you want to install this license file onto this server?", "License installation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
				{
					if (CopyLicenseFile(mLicenseInstallPath) == false)
					{
						goto installLicense;
					}

					MessageBox.Show("Complete!\nRestart your MOG Server for the new license to take effect.", "Installation successfull!", MessageBoxButtons.OK);
				}					
			}
			else
			{
				MessageBox.Show("Your selected license file either does not exist or has not been loaded.", "Install Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private bool CopyLicenseFile(string targetDir)
		{
			// Check for valid server directory
			if (Directory.Exists(targetDir))
			{
				string targetLicenseFile = Path.Combine(targetDir, "mog_license");

				bool overwrite = false;
				if (File.Exists(targetLicenseFile))
				{
					if (MessageBox.Show("MOG License already exists. \n\nDo you want to overwrite this license?", "License installation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
					{
						overwrite = true;
					}
					else
					{
						return false;
					}
				}
				// All good, do the copy
				try
				{
					File.Copy(mLicenseFile, targetLicenseFile, overwrite);
					return true;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Install Error (Licence file could not be copied!)", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show("The path you selected is not a valid Mog Server path!", "Copy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return false;
		}

		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void searchForServerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			InitializeServerPath();
		}		
	}
}