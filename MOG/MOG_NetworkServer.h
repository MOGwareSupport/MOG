//--------------------------------------------------------------------------------
//	MOG_NetworkServer.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_NETWORKSERVER_H__
#define __MOG_NETWORKSERVER_H__


#include <stdlib.h>

#include "MOG_Network.h"

using namespace System::Collections::Specialized;


//public __value struct Connection{
public __gc class MOG_NetworkConnection
{
public:
	int				mID;
	String*			mIP;
	NetworkSocket*	mNetworkSocket;
};


public __gc class MOG_NetworkServer : public MOG_Network
{
public:
				MOG_NetworkServer();

	bool		Initialize(int serverPort);

	ArrayList*	GetAllConnectionsArray(void);

	int			CheckNewConnections(void);
	ArrayList*	ReadNewConnections(void);

	bool		CloseAllConnections();
	bool		CloseConnection(int networkID);

	ArrayList*	ReadPackets(int networkID);
	ArrayList*	ReadPackets(MOG_NetworkConnection *connection);

	bool		SendPacketToConnection(NetworkPacket *packet, int ID);
	bool		SendPacketToAll(NetworkPacket *packet);
	int			SendPendingPackets();
	bool		SendDummyPacket();

private:
	// Server variables
	HybridDictionary *mConnections;

	int			mServerPort;

	int			mServerNextID;				//next ID to give out.
};


#endif

