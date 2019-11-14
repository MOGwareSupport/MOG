#ifndef __MOG_PROMPT__
#define __MOG_PROMPT__

using namespace System;
using namespace System::Threading;
using namespace System::Drawing;
using namespace System::Windows::Forms;
using namespace System::Collections;


namespace MOG
{
	//Forward declaration of MOG_PromptDialog
	public __gc class MOG_PromptDialog;
}

namespace MOG
{	
	namespace PROMPT
	{
		public __value enum MOG_ALERT_LEVEL
		{
			MESSAGE,
			ALERT,
			ERROR,
			CRITICAL,
		};

		public __value enum MOGPromptResult {
			Abort,
			Cancel,
			Ignore,
			No,
			None,
			OK,
			Retry,
			Yes,
			YesToAll,
			NoToAll,
		};

		public __value enum MOGPromptButtons {
			AbortRetryIgnore,
			OK,
			OKCancel,
			RetryCancel,
			YesNo,
			YesNoCancel,
			YesNoYesToAllNoToAll,
		};

		public __value enum MOG_PROMPT_MODE_TYPE
		{
			MOG_PROMPT_NONE = 0x00,
			MOG_PROMPT_CONSOLE = 0x01,
			MOG_PROMPT_FILE = 0x02,
			MOG_PROMPT_SILENT = 0x04,
		};

		public __gc class MOG_Prompt
		{			
		public:
			static Point GetCenteredLocation( Form* mainForm, Size promptSize );
			static void SetMode(MOG_PROMPT_MODE_TYPE modeFlags)		{ mModeFlags |= modeFlags; }
			static bool IsMode(MOG_PROMPT_MODE_TYPE modeFlags)		{ return (mModeFlags & modeFlags) ? true : false; }
			static void ClearMode(MOG_PROMPT_MODE_TYPE modeFlags)	{ mModeFlags &= ~modeFlags; }
			static void ClearAllModes()								{ mModeFlags = 0; }

			static System::Windows::Forms::Form *ClientForm;
			static MOG::MOG_PromptDialog *PromptMessageDialog;
			static bool CheckPromptMessageDialog();
			static bool IsDialogVisible() { return mVisible > 0; };
			static void DialogVisible(int inc) { mVisible += inc; };
			static bool PromptMessage(String *title, String *message);
			static bool PromptMessage(String *title, String *message, String *stackTrace);
			static bool PromptMessage(String *title, String *message, String *stackTrace, MOG_ALERT_LEVEL alertLevel);
			static MOGPromptResult PromptResponse(String *title, String *message);
			static MOGPromptResult PromptResponse(String *title, String *message, MOGPromptButtons buttons);
			static MOGPromptResult PromptResponse(String *title, String *message, String *stackTrace, MOGPromptButtons buttons);
			static MOGPromptResult PromptResponse(String *title, String *message, String *stackTrace, MOGPromptButtons buttons, MOG_ALERT_LEVEL alertLevel);

		private:
			static int			mCurrentListViewIndex;
			static Mutex		*mListViewMutex = new Mutex();
			static ArrayList	*mStoredListViewItems = new ArrayList();
			static int			threadNumber = 0;
			static bool			bCancelled = false;
			static const int	INVOKE_COUNT_LIMIT = 500; // 500 * 10ms == approx. 5 seconds to failure
			static int			mModeFlags = MOG_PROMPT_NONE;
			static int			mVisible = 0;
            
			static void PromptMessageThreadMethod();
			static void PromptMessageFormStoreAppend(String *title, String *message, String *stackTrace, MOG_ALERT_LEVEL alertLevel);
			static void PromptMessageFormUpdate();
			static void StartPromptMessageDialogThread(String *title, String *message, String *stackTrace, MOG_ALERT_LEVEL alertLevel );
			static void CancelPromptMessageDialog( Object *sender, System::ComponentModel::CancelEventArgs *e );

			static int AlertToImageIndex(MOG_ALERT_LEVEL alertLevel);
			static String *AlertToString(MOG_ALERT_LEVEL alertLevel);

		};
	}
}

using namespace MOG::PROMPT;

#endif __MOG_PROMPT__