using System;
using System.Windows.Forms;

using MOG.COMMAND;
using MOG.PROGRESS;
using MOG.PROMPT;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_ServerManager.MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for util_CallbackProgressFormWrapper.
	/// </summary>
	public class util_CallbackProgressFormWrapper
	{
		public int Handle;
		public string Title;
		public MOG_Callbacks Callbacks;
		public int Max;
		public int Value;
		public string description = "";
		public int percent = 0;

		public ProgressBarForm progressForm = null;
		
		private bool cancelVisible = true;

		public util_CallbackProgressFormWrapper()
		{
		}

		public int DialogReset()
		{
			Kill();
			DialogInit(this.Title, this.description, this.cancelVisible);
			Update(this.percent, this.description);

			return this.Handle;
		}

		public int DialogInit(string title, string description, bool bCancelVisible)
		{
			this.Handle = MOG_Progress.ProgressSetup(title, description, 0, 100);
			this.Title = title;
			return this.Handle;
		}

		public bool Update(int percent, string description)
		{
			this.description = description;
			this.percent = percent;
			MOG_Progress.ProgressUpdateStep(this.Handle, description, percent);
			return CancelClicked();
		}

		public bool CancelClicked()
		{
			return (MOG_Progress.ProgressStatus(this.Handle) != MOGPromptResult.Cancel);
		}

		public bool Update(string description)
		{
			MOG_Progress.ProgressUpdate(this.Handle, description);
			this.description = description;
			return CancelClicked();
		}

		public void Kill()
		{
			MOG_Progress.ProgressClose(this.Handle);
		}
	}
}
