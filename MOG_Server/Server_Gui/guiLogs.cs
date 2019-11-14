using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using Server_Gui;
using MOG_Server.Server_Gui.guiConfigurationsHelpers;

using MOG;
using MOG.TIME;
using MOG.REPORT;

namespace MOG_Server.Server_Gui
{
	public class guiLogs
	{
		private string defaultLogName;
		private string defaultLogFullName;
		private string logpath;
		private bool logLoaded;

		private RichTextBox LogLogDisplayRichTextBox;
		private ComboBox LogComboBox;
		private Label LogLogFilenameLabel;
		private FormMainSMOG mainForm;

		public enum EVENT_LOG {TYPE, DATE, MACHINE};

		public bool LogLoaded { get{return logLoaded;} }
		
		public guiLogs(FormMainSMOG parentForm) 
		{
			defaultLogName=MOG_Report.GetLogFileName();

			//TODO JKB: Function Request: MOG_REPORT.GetLogPath()
			logpath = MOG_Report.GetLogFileName().Substring(0, MOG_Report.GetLogFileName().LastIndexOf("\\"));
			defaultLogFullName = MOG_Report.GetLogFileName();
			logLoaded = false;

			mainForm = parentForm;
			LogLogDisplayRichTextBox = new RichTextBox();//mainForm.LogLogDisplayRichTextBox;
			LogLogFilenameLabel = new Label();//mainForm.LogLogFilenameLabel;
			LogComboBox = new ComboBox();
		}
	
		public void MakeNewLog() 
		{
			// make a copy of the default log file
			StreamWriter f=null;
			if (!File.Exists(defaultLogFullName)) 
			{
				// create an empty log file to copy
				f=new StreamWriter(defaultLogFullName,false);
				f.Write("");
				f.Close();
			}
			string[] sourceNameElems=defaultLogName.Split(".".ToCharArray(),200);
			string destName=sourceNameElems[0];

			destName += ( string.Concat(".", DateTime.Now.ToString("ddMMMyy_hh-mm-ss"), ".log") );

			string destFilename=string.Concat( logpath, "\\"+destName );
			
			DialogResult result=MessageBox.Show( string.Concat("Saving current log (",defaultLogName,") to ", destName), "Logfile copy", MessageBoxButtons.OKCancel );
			if (result==DialogResult.Cancel)
			{
				return;
			}
			
			if (File.Exists(destFilename))
			{
				File.Delete(destFilename);
			}
			
			File.Copy(defaultLogFullName, destFilename);
			
			// recreate the default log file
			f=new StreamWriter(defaultLogFullName,false);
			f.Write("");
			f.Close();

			ReloadLogComboBox();
		}
		
		public void DeleteLog() 
		{
			if ( LogComboBox.Text == "" )
				return;

			string fname=string.Concat( logpath, "\\", (string)LogComboBox.SelectedItem );
			DialogResult result=MessageBox.Show( string.Concat("Are you sure you want to delete ", (string)LogComboBox.SelectedItem, "?"), "Confirm delete", MessageBoxButtons.YesNo);
			if ( result==DialogResult.Yes && File.Exists(fname) ) 
			{
				File.Delete(fname);
				RepopulateLogComboBox();
			}
		}
		
		public void RefreshLog() 
		{
			if ( LogComboBox.Text == "" )
				return;
			
			string fname=string.Concat( logpath, "\\", (string)LogComboBox.Text );
			if (!File.Exists(fname)) 
			{
				MessageBox.Show( string.Concat((string)LogComboBox.Text," no longer exists"),"Missing file");
				RepopulateLogComboBox();
				return;
			}

			LoadFile(fname);
			RepopulateLogComboBox();
		}
		
		public void LoadFile(string fname) 
		{
			string fullname=(new FileInfo(fname)).FullName;
			if (!File.Exists(fullname)) 
			{
				MessageBox.Show( string.Concat( (new FileInfo(fname)).Name," does not exist"),"Missing file");
				RepopulateLogComboBox();

				// Load the event nodes
				LoadEventNodes(fname);

				return;
			}
			
			LogLogDisplayRichTextBox.LoadFile(fullname, RichTextBoxStreamType.PlainText);
			LogLogFilenameLabel.Text=fullname;
			logLoaded=true;
		}


		private void LoadEventNodes(string filename)
		{
			FileInfo file = new FileInfo(filename);

			using (StreamReader sr = file.OpenText())
            {
                while (sr.Peek() >= 0) 
                {
                    string line = sr.ReadLine();

					// This is a new object, create the node
					//mainForm.LogEventsListView.Items.Add(CreateEventNode(sr));
                }
            }
		}

		private ListViewItem CreateEventNode(StreamReader sr)
		{
			ListViewItem item = new ListViewItem();

			item.SubItems.Add("");
			item.SubItems.Add("");
			item.SubItems.Add("");

			while (sr.Peek() >= 0) 
			{
				string line = sr.ReadLine();

				if ( line.IndexOf("Title") != -1)
				{
					//item.SubItems[(int)EVENT_LOG.TITLE].Text = 
				}
			}

			return null;
		}
		
		
		public void RepopulateLogComboBox() 
		{
			// Get a list of all .LOG files in the directory reported by MOG_REPORT
			string[] logfiles = Directory.GetFiles(logpath,"*.log");
			
			string selectedFile=LogComboBox.Text;
			
			// Add em to the drop box
			LogComboBox.Items.Clear();
			foreach (string logfilename in logfiles) 
				LogComboBox.Items.Add( (new FileInfo(logfilename)).Name );
			
			IEnumerator itr=LogComboBox.Items.GetEnumerator();
			int i=0;
			LogComboBox.SelectedIndex=-1;
			while (itr.MoveNext()) 
			{
				if ( ((string)itr.Current).ToUpper() == selectedFile.ToUpper() ) 
				{
					LogComboBox.SelectedIndex=i;
					break;
				}
				++i;
			}
			if (LogComboBox.SelectedIndex==-1)
			{
				LogComboBox.Text="";
				LogLogDisplayRichTextBox.Text=">> no log loaded <<";
			}
		}
		
		public void ReloadLogComboBox() 
		{
			if (!Directory.Exists(logpath))
			{
				return;
			}

			// get a list of all .LOG files in the directory reported by MOG_REPORT
			string[] logfiles = Directory.GetFiles(logpath,"*.log");
			
			// add em to the drop box
			LogComboBox.Items.Clear();
			foreach (string logfilename in logfiles) 
				LogComboBox.Items.Add( (new FileInfo(logfilename)).Name );
			IEnumerator itr=LogComboBox.Items.GetEnumerator();
			int i=0;
			LogComboBox.SelectedItem=null;
			LogComboBox.SelectedIndex=-1;
			LogComboBox.Text="";
			LogLogDisplayRichTextBox.Text=">> no log loaded <<";
			logLoaded=false;
			while (itr.MoveNext()) 
			{
				if ( ((string)itr.Current).ToUpper() == defaultLogName.ToUpper() ) 
				{
					LogComboBox.SelectedIndex=i;
					LoadFile( (string)itr.Current );
					break;
				}
				++i;
			}
		}
	}
}


