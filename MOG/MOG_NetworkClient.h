//--------------------------------------------------------------------------------
//	MOG_NetworkClient.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_NETWORKCLIENT_H__
#define __MOG_NETWORKCLIENT_H__

#include <stdlib.h>

#include "MOG_Network.h"

public __gc class MOG_NetworkClient : public MOG_Network
{
public:
				MOG_NetworkClient();
	bool		Initialize();

	bool		ConnectToServer(int tryUntilConnected);
	bool		CloseConnectionToServer();
	bool		Connected(void)		{ return mNetworkSocket ? mNetworkSocket->IsConnected() : false; }

	ArrayList*	ReadPackets(void);
	bool		SendToServer(NetworkPacket *packet);
};

#endif

