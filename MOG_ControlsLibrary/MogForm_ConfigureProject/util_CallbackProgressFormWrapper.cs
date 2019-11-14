using System;
using System.Windows.Forms;

using MOG.COMMAND;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_Server.MOG_ControlsLibrary.Common
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
			this.Callbacks = MOG_ControllerSystem.GetCommandManager().GetCallbacks();

			if (this.Callbacks == null)
				return -1;

			if (bCancelVisible)
				this.Handle = this.Callbacks.mDialogInit(title, description, "Cancel");
			else
				this.Handle = this.Callbacks.mDialogInit(title, description, "");

			this.description = description;
			this.Title = title;
			this.cancelVisible = bCancelVisible;

			return this.Handle;
		}

		public bool Update(int percent, string description)
		{
			if (this.Callbacks != null)
			{
				this.Callbacks.mDialogUpdate(this.Handle, percent, description);
				this.description = description;
				this.percent = percent;
				Application.DoEvents();
				return !this.Callbacks.mDialogProcess(this.Handle);
			}

			return true;
		}

		public bool CancelClicked()
		{
			if (this.Callbacks != null)
			{
				Application.DoEvents();
				this.Callbacks.mDialogUpdate(this.Handle, this.percent, this.description);
				return !this.Callbacks.mDialogProcess(this.Handle);
			}

			return false;
		}

		public bool Update(string description)
		{
			if (this.Callbacks != null)
			{
				float fMax = (float)this.Max;
				float fValue = (float)this.Value;
				int percent = (int)((fValue/fMax)*100);

				if (percent < 0) percent = 0;
				if (percent > 100) percent = 100;

				this.Callbacks.mDialogUpdate(this.Handle, percent, description);
				return !this.Callbacks.mDialogProcess(this.Handle);
			}

			return true;
		}

		public void Kill()
		{
			if (this.Callbacks != null)
				this.Callbacks.mDialogKill(this.Handle);
		}
	}
}
