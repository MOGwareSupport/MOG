//--------------------------------------------------------------------------------
//	MOG_NetworkSocket.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_NETWORKSOCKET_H__
#define __MOG_NETWORKSOCKET_H__


#using <mscorlib.dll>
#using <system.dll>

#include "stdlib.h"
#include "Mog_define.h"


using namespace System::Net::Sockets;


#define INVALID_NETWORK_ID			0


#ifdef NETWORK_WRITE_TO_CONSOLE
#define NetworkConsoleMessage			MOG_Report::ReportSilent		//write out messages to the console
#define NetworkConsoleError				MOG_Report::ReportSilent
#else
#define NetworkConsoleMessage
#define NetworkConsoleError
#endif

public __gc struct NetworkPacket
{
	int size;
	Byte buffer[];
};


public __gc class NetworkSocket
{
public:
						NetworkSocket(Socket* pSocket);
						~NetworkSocket();

	void				Shutdown();

	Socket*				Accept();
	bool				IsConnected();
	void				ChangeSocket(Socket* pSocket);

	static String*		GetIPAddress(String* serverIPAddress);
	static String*		GetMacAddress(String *localIPAddress);
	
	bool				SendDummyPacket();

	bool				WritePacket(NetworkPacket *packet);
	ArrayList*			ReadPackets();

	int					SendPendingPackets();
	bool				UpdatePendingPacketNetworkIDs(int id);

private:
	Socket*		mSocket;
	ArrayList*	mPendingPackets;
	Mutex*		mMutex;
	DateTime	mLastPacketSentTime;
};

int SerializeString(NetworkPacket *packet, int offset, String *string);
int SerializeBool(NetworkPacket *packet, int offset, bool value);
int SerializeInt32(NetworkPacket *packet, int offset, Int32 value);
int SerializeUInt32(NetworkPacket *packet, int offset, UInt32 value);

int DeserializeString(NetworkPacket *packet, int offset, String **string);
int DeserializeBool(NetworkPacket *packet, int offset, bool __gc *value);
int DeserializeInt32(NetworkPacket *packet, int offset, Int32 *value);
int DeserializeUInt32(NetworkPacket *packet, int offset, UInt32 *value);

#endif


