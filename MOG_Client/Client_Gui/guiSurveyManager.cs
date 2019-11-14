using System;
using System.Windows.Forms;
using System.IO;

using MOG;
using MOG.INI;
using MOG.FILENAME;
using MOG.TIME;
using MOG.DOSUTILS;
using MOG.COMMAND;
using MOG.CONTROLLER.CONTROLLERSURVEY;


namespace MOG_Client.Client_Gui
{
	/// <summary>
	/// Summary description for guiSurveyManager.
	/// </summary>
	public class guiSurveyManager
	{
		private MogMainForm mainForm;
		private BASE mMog;
//		private MOG_Ini newfile;
//		private MOG_Filename fname;
		private MOG_Time mtime;

		public guiSurveyManager(MogMainForm main)
		{
			mainForm = main;
			mMog = mainForm.gMog;
		}

		public void Start()
		{
			SurveyForm tmpfrm = new SurveyForm(this);

			// Populate the priority comboBox
			tmpfrm.SurveyPriorityComboBox.Items.Add("None");
			tmpfrm.SurveyPriorityComboBox.Items.Add("Low");
			tmpfrm.SurveyPriorityComboBox.Items.Add("Medium");
			tmpfrm.SurveyPriorityComboBox.Items.Add("High");
			tmpfrm.SurveyPriorityComboBox.Items.Add("Urgent");
			tmpfrm.Show();			
		}

		public void MakeChart()
		{
			/*double [] y=new double [1];
			double [] x=new double [1];
			double [] x2=new double [1];
			double [] x3=new double [1];
			x[0]=3;
			y[0] = 0;
			x2[0]=1;x3[0]=6;
			String [] cats ={"answer 1","answer 2","answer 3"};
			//size of y array tells it how many elements
			SurveyVerticalBarChart.SetXValues(y);
			//this sets up names for all elements
			SurveyVerticalBarChart.X_Categories = cats;
			//a series can be one value or many
			SurveyVerticalBarChart.AddSeries("one",Color.Blue,x);
			SurveyVerticalBarChart.AddSeries("two",Color.Red,x2);
			SurveyVerticalBarChart.AddSeries("three",Color.Green,x3);
			//force it to repaint
			Invalidate(true);*/

		}

		public void RecieveNewSurvey(MOG_Command survey)
		{
			SurveyVote vote = new SurveyVote(this);
			vote.ShowDialog();
		}

		public void SendSurvey(SurveyForm sfrm)
		{
			//MOG_SURVEY_PRIORITY priority;
			mtime = new MOG_Time();
			mtime.UpdateTime();

			//Controler stuff
			MOG_ControllerSurvey mController = new MOG_ControllerSurvey(mMog);
			mController.SetTitle(sfrm.SurveyNameTextBox.Text);
			mController.SetCategory(sfrm.SurveyCatComboBox.Text);
			mController.SetCreateTime(mtime.GetTimeStamp());
			mController.SetDescription(sfrm.SurveyDescTextBox.Text);

			// Set the priority
			MOG_SURVEY_PRIORITY priority = MOG_SURVEY_PRIORITY.MOG_SURVEY_PRIORITY_None;
			switch(sfrm.SurveyPriorityComboBox.Text)
			{
				case "None":
					priority = MOG_SURVEY_PRIORITY.MOG_SURVEY_PRIORITY_None;
					break;
				case "Low":
					priority = MOG_SURVEY_PRIORITY.MOG_SURVEY_PRIORITY_Low;
					break;
				case "Medium":
					priority = MOG_SURVEY_PRIORITY.MOG_SURVEY_PRIORITY_Medium;
					break;
				case "High":
					priority = MOG_SURVEY_PRIORITY.MOG_SURVEY_PRIORITY_High;
					break;
				case "Urgent":
					priority = MOG_SURVEY_PRIORITY.MOG_SURVEY_PRIORITY_Urgent;
					break;
			}
			mController.SetPriority(priority);
			
			//Figure out
			string users = "";
			mController.SetSurveyUsers(users);
			mController.SetCreator(mMog.GetUser().GetUserName());
			mController.SetExpireTime(sfrm.SurveyExpireDateTimePicker.Value.ToString());
		
			mController.Create();
			mController.Send();

			// Update the surveys window
			Update();
		}

		public void Vote(int question,string comment)
		{

		}
		
		public void Update()
		{
			if (DosUtils.FileExist(string.Concat(mMog.GetProject().GetProjectPath(), "\\Surveys\\Contents.info")))
			{
				MOG_Ini contentsInfo = new MOG_Ini(string.Concat(mMog.GetProject().GetProjectPath(), "\\Surveys\\Contents.info"));
				mainForm.SurveyListView.Items.Clear();

				for (int i = 0; i < contentsInfo.CountKeys("SURVEYS"); i++)
				{
					string SurveyName = contentsInfo.GetKeyNameByIndex("SURVEYS", i);
					ListViewItem item = new ListViewItem(contentsInfo.GetString(SurveyName, "Title"));
					item.SubItems.Add(contentsInfo.GetString(SurveyName, "Time"));
					item.SubItems.Add(contentsInfo.GetString(SurveyName, "Category"));
					item.SubItems.Add(contentsInfo.GetString(SurveyName, "Status"));
					item.SubItems.Add(contentsInfo.GetString(SurveyName, "Priority"));

					mainForm.SurveyListView.Items.Add(item);
				}
			}
		}
	}
}
