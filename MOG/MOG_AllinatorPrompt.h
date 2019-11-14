//--------------------------------------------------------------------------------
//	MOG_AllinatorPrompt.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_AllinatorPrompt_H__
#define __MOG_AllinatorPrompt_H__

#include "MOG_Define.h"
#include "MOG_Prompt.h"


namespace MOG {
namespace UTILITIES {

public __gc class MOG_AllinatorPrompt
{
public:
	__property bool get_YesToAll()					{ return mYesToAll; }
	__property void set_YesToAll( bool value )		{ mYesToAll = value; }
	__property bool get_NoToAll()					{ return mNoToAll; }
	__property void set_NoToAll( bool value )		{ mNoToAll = value; }

	MOG_AllinatorPrompt();

	void Reset();

	MOGPromptResult PromptResponse(String *title, String *message);
	MOGPromptResult PromptResponse(String *title, String *message, MOGPromptButtons buttons);
	MOGPromptResult PromptResponse(String *title, String *message, String *stackTrace, MOGPromptButtons buttons);
	MOGPromptResult PromptResponse(String *title, String *message, String *stackTrace, MOGPromptButtons buttons, MOG_ALERT_LEVEL alertLevel);


private:
	bool mYesToAll;
	bool mNoToAll;
};


} // end ns PROFILE
} // end ns MOG

using namespace MOG::UTILITIES;

#endif	// __MOG_AllinatorPrompt_H__




