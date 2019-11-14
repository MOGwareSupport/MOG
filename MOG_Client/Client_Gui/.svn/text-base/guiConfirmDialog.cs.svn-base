using System;
using System.Collections;
using System.Windows.Forms;

using MOG_Client.Forms;

using MOG.REPORT;

namespace MOG_Client.Client_Mog_Utilities.AssetOptions
{
	/// <summary>
	/// Summary description for guiConfirmDialog.
	/// </summary>
	public class guiConfirmDialog
	{

		static private ArrayList mSelectedItems;
		static public ArrayList SelectedItems { get{return mSelectedItems;} }
				
		public guiConfirmDialog()
		{
		}

		static public DialogResult ConfirmDialog(string title, string action, string assets, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return MessageBox.Show(action + "\n\n" + assets, title, buttons, icon);
		}

		static public DialogResult ConfirmModalDialog(MogMainForm app, string title, string action, string assets, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return MessageBox.Show(app, action + "\n\n" + assets, title, buttons, icon);
		}

		static public DialogResult MessageBoxDialog(string title, string message, string parentPath, ArrayList assets, ArrayList optionalAssetLabels, string optionalLabelDelimeter, MessageBoxButtons buttons)
		{
			DialogResult rc = DialogResult.Cancel;
			string buttonString;

			// Prepare the buttons
			switch (buttons)
			{
				case MessageBoxButtons.AbortRetryIgnore:
					buttonString = "Abort/Retry/Ignore";
					break;
				case MessageBoxButtons.OK:
					buttonString = "Ok";
					break;
				case MessageBoxButtons.OKCancel:
					buttonString = "Ok/Cancel";
					break;
				case MessageBoxButtons.RetryCancel:
					buttonString = "Retry/Cancel";
					break;
				case MessageBoxButtons.YesNo:
					buttonString = "Yes/No";
					break;
				case MessageBoxButtons.YesNoCancel:
					buttonString = "Yes/No/Cancel";
					break;
				default:
					buttonString = "Unknown";
					break;
			}

			// Display the string.
			FormConfirmSpecial question = new FormConfirmSpecial();
			if (optionalLabelDelimeter != null)
			{
				question.LabelTreeDelimiter = optionalLabelDelimeter;
			}

			question.DialogInitialize(title, message, parentPath, assets, optionalAssetLabels, buttonString);
			question.DialogShowModal();

			switch (buttons)
			{
				case MessageBoxButtons.AbortRetryIgnore:
				switch (question.result)
				{
					case 0:
						rc = DialogResult.Abort;
						break;
					case 1:
						rc = DialogResult.Retry;
						break;
					case 2:
						rc = DialogResult.Ignore;
						break;
				}
					break;
				case MessageBoxButtons.OK:
				switch (question.result)
				{
					case 0:
						rc = DialogResult.OK;
						break;
				}
					break;
				case MessageBoxButtons.OKCancel:
				switch (question.result)
				{
					case 0:
						rc = DialogResult.OK;
						break;
					case 1:
						rc = DialogResult.Cancel;
						break;
				}
					break;
				case MessageBoxButtons.RetryCancel:
				switch (question.result)
				{
					case 0:
						rc = DialogResult.Retry;
						break;
					case 1:
						rc = DialogResult.Cancel;
						break;
				}
					break;
				case MessageBoxButtons.YesNo:
				switch (question.result)
				{
					case 0:
						rc = DialogResult.Yes;
						break;
					case 1:
						rc = DialogResult.No;
						break;
				}
					break;
				case MessageBoxButtons.YesNoCancel:
				switch (question.result)
				{
					case 0:
						rc = DialogResult.Yes;
						break;
					case 1:
						rc = DialogResult.No;
						break;
					case 2:
						rc = DialogResult.Cancel;
						break;
				}
					break;
				default:
					break;
			}

			question.DialogKill();
			
			// Get the selected items
			mSelectedItems = question.SelectedItems;

			question = null;

			
			return rc;
		}

	}
}
