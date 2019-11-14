#ifndef __MOG_PROGRESS__
#define __MOG_PROGRESS__

#include "MOG_Prompt.h"

using namespace System;
using namespace System::Windows::Forms;
using namespace System::Threading;

//Forward declaration of MOG_ProgressDialog
namespace MOG
{
	public __gc class MOG_ProgressDialog;
}

namespace MOG
{	
	namespace PROGRESS
	{
		public __gc class MOG_Progress
		{
		public:
			static void SecondaryProgressBarSetup(int dialogId);
			static void SecondaryProgressBarSetup(int dialogId, int max);
			static void SecondaryProgressBarSetup(int dialogId, int min, int max);
			static void ProgressSetMinMax(int dialogId, int min, int max);
			static void ProgressInitGraphic(int dialogId);
			static MOGPromptResult ProgressUpdateGraphic(int dialogID, String *fileMessage);
			static void SecondaryProgressBarClose(int dialogId);
			static System::Windows::Forms::Form *ClientForm;
			static bool IsDialogVisible()  { return mVisible > 0; };
			static int ProgressSetup(String *title, String *message);
			static int ProgressSetup(String *title, String *message, int progressMax);
			static int ProgressSetup(String *title, String *message, int progressMin, int progressMax);
			static void ProgressClose(int dialogId);
			static MOGPromptResult ProgressStatus(int dialogID);
			static MOGPromptResult ProgressUpdate(int dialogID, String *fileMessage);
			static MOGPromptResult ProgressUpdate(int dialogId, String *message, String *fileMessage);
			static MOGPromptResult ProgressUpdatePercent( int dialogID, String *fileMessage, int percent );
			static MOGPromptResult ProgressUpdatePercent( int dialogID, String *message, String *fileMessage, int percent );
			static MOGPromptResult ProgressUpdateStep(int dialogID, String *fileMessage);
			static MOGPromptResult ProgressUpdateStep(int dialogId, String *message, String *fileMessage);
			static MOGPromptResult ProgressUpdateStep(int dialogID, String *fileMessage, int step);
			static MOGPromptResult ProgressUpdateStep(int dialogId, String *message, String *fileMessage, int step);
		private:
			static int CreateDialog(String *title, String *message, int progressMin, int progressMax);
			static MOG_ProgressDialog *GetDialog(int dialogId) { return GetDialog(dialogId, false); }
			static MOG_ProgressDialog *GetDialog(int dialogId, bool explicitMatch);
			static void TerminateDialog(int dialogId);

			static ArrayList *mDialogs;
			static int mVisible = 0;			
		};
	}
}

using namespace MOG::PROGRESS;

#endif __MOG_PROGRESS__