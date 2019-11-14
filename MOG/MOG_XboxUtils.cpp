

//#include "MOG_XboxUtils.h"
//#include "MOG_DosUtils.h"
//#include "xboxdbg.h"

//-----------------------------------------------------------------------------
// Global variables
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
// Name: RCmdSendFile()
// Desc: Handles the sendfile command by sending the arguments along to
//       DmSendFile
//-----------------------------------------------------------------------------
//bool MOG_Xbox::FileXboxCopy( String *source, String *target)
//{
//    return SUCCEEDED( DmSendFile( StringToCharPtr(source), StringToCharPtr(target) ) );
//}


/*


//-----------------------------------------------------------------------------
// Name: RCmdGetFile()
// Desc: Handles the getfile command by sending the arguments along to
//       DmReceiveFile
//-----------------------------------------------------------------------------
BOOL RCmdGetFile( int argc, char *argv[] )
{
    if( argc != 3 )
        return TRUE;

    return SUCCEEDED( DmReceiveFile( argv[2], argv[1] ) );
}


//-----------------------------------------------------------------------------
// Name: DisplayError()
// Desc: Display friendly error by translating the hr to a message
//-----------------------------------------------------------------------------
VOID DisplayError( const CHAR* strResponse, const CHAR* strApiName, HRESULT hr )
{
    CHAR strError[100];
    if( FAILED( DmTranslateError( hr, strError, 100) ) )
        return;

    if( hr == XBDM_UNDEFINED )
        strcpy( strError, strResponse );

    if( strError )
        ConsoleWindowPrintf( RGB(255,0,0), "%s failed: '%s'\n", strApiName, strError );
    else
        ConsoleWindowPrintf( RGB(255,0,0), "%s failed: 0x%08lx\n", strApiName, hr );
}
*/