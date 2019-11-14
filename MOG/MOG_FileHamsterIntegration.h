//--------------------------------------------------------------------------------
//	MOG_FileHamsterIntegration.h
//	
//	
//--------------------------------------------------------------------------------

#ifndef __MOG_FileHamsterIntegration_H__
#define __MOG_FileHamsterIntegration_H__

#include "MOG_Define.h"


using namespace System::Collections::Specialized;


namespace MOG {
namespace UTILITIES {

public __gc class MOG_FileHamsterIntegration
{
public:
	static bool Initialize();
	static void FileHamsterPause(String *directory);
	static void FileHamsterUnPause();

private:
	static HybridDictionary *mGetLatestFolders = new HybridDictionary();
};


} // end ns PROFILE
} // end ns MOG

using namespace MOG::UTILITIES;

#endif	// __MOG_FileHamsterIntegration__




