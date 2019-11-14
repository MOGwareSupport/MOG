//--------------------------------------------------------------------------------
//	MOG_REPORT.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_REPORT_H__
#define __MOG_REPORT_H__

#using <mscorlib.dll>
#using <system.dll>
#using <system.windows.forms.dll>

#include "stdlib.h"
#include "MOG_Prompt.h"

using namespace System;
using System::Windows::Forms::MessageBox;
using System::Windows::Forms::MessageBoxButtons;
using System::Windows::Forms::MessageBoxIcon;
using System::Windows::Forms::DialogResult;
using System::IO::FileStream;

namespace MOG {
namespace REPORT {
public __gc class MOG_Report {

private:
	static String *mLogFilename = "";

public:
	static void SetLogFileName(String *string);
	static String *GetLogFileName() { return mLogFilename; };

	static bool ReportSilent(String *title, String *message, String *stackTrace);
	static bool ReportSilent(String *title, String *message, String *stackTrace, MOG_ALERT_LEVEL criticalLevel);
	static bool ReportMessage(String *title, String *message, String *stackTrace, MOG_ALERT_LEVEL criticalLevel);
	static MOGPromptResult ReportResponse(String *title, String *message, String *stackTrace, MOGPromptButtons buttons, MOG_ALERT_LEVEL criticalLevel);

	// REMOVE TEST CODE BELOW:
	static bool TestOnCPlusSide();
};

}
}

using namespace MOG::REPORT;


#endif