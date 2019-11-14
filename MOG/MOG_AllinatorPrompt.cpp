//--------------------------------------------------------------------------------
//	MOG_AllinatorPrompt.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_AllinatorPrompt.h"
#include "MOG_ControllerSystem.h"


MOG_AllinatorPrompt::MOG_AllinatorPrompt()
{
	Reset();
}


void MOG_AllinatorPrompt::Reset()
{
	mYesToAll = false;
	mNoToAll = false;
}


MOGPromptResult MOG_AllinatorPrompt::PromptResponse(String *title, String *message)
{
	return PromptResponse(title, message, "", MOGPromptButtons::YesNoYesToAllNoToAll, MOG_ALERT_LEVEL::MESSAGE);
}

// Overload of MOGPromptResult MOG_Prompt::PromptResponse(String *title, String *message, String *stackTrace, MOGPromptButtons buttons, MOG_ALERT_LEVEL alertLevel)
MOGPromptResult MOG_AllinatorPrompt::PromptResponse(String *title, String *message, MOGPromptButtons buttons)
{
	return PromptResponse(title, message, "", buttons, MOG_ALERT_LEVEL::MESSAGE );
}

// Overload of MOGPromptResult MOG_Prompt::PromptResponse(String *title, String *message, String *stackTrace, MOGPromptButtons buttons, MOG_ALERT_LEVEL alertLevel)
MOGPromptResult MOG_AllinatorPrompt::PromptResponse(String *title, String *message, String *stackTrace, MOGPromptButtons buttons)
{
	return PromptResponse(title, message, stackTrace, buttons, MOG_ALERT_LEVEL::MESSAGE );
}

MOGPromptResult MOG_AllinatorPrompt::PromptResponse(String *title, String *message, String *stackTrace, MOGPromptButtons buttons, MOG_ALERT_LEVEL alertLevel)
{
	MOGPromptResult result = MOGPromptResult::Yes;

	if (MOG_ControllerSystem::isSuppressMessageBox() == false)
	{
		if (YesToAll)
		{
			result = MOGPromptResult::Yes;
		}
		else if(NoToAll)
		{
			result = MOGPromptResult::No;
		}
		else
		{
			// Ask the user what MOG should do here?
			result = MOG_Prompt::PromptResponse(title, message, buttons);
			switch(result)
			{
				case MOGPromptResult::YesToAll:
					YesToAll = true;
					result = MOGPromptResult::Yes;
					break;
				case MOGPromptResult::Yes:
					break;

				case MOGPromptResult::NoToAll:
					NoToAll = true;
					result = MOGPromptResult::No;
					break;
				case MOGPromptResult::No:
					break;
			}
		}
	}

	return result;
}

