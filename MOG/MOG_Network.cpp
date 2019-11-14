//--------------------------------------------------------------------------------
//	MOG_Network.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"
#include "MOG_Network.h"


MOG_Network::MOG_Network()
{
	mNetworkSocket = NULL;
	mID = 0;
	mIP = "";
	mComputerName = "";
}

void MOG_Network::SetID(Int32 id)
{
	mID = id;

	mNetworkSocket->UpdatePendingPacketNetworkIDs(id);
}


bool MOG_Network::SendDummyPacket()
{
	// Determin how long it has been since we last sent a dummy packet?
	TimeSpan timeSpan = DateTime::Now - mLastDummyPacketSentTime;
	if (timeSpan.TotalSeconds > 5)
	{
		// Attempt to send them a dummy packet to ensure connectivity
		mNetworkSocket->SendDummyPacket();

		// Restamp our mLastDummyPacketSentTime
		mLastDummyPacketSentTime = DateTime::Now;
		return true;
	}

	return false;
}


int MOG_Network::SendPendingPackets()
{
	int packets = 0;

	if (mNetworkSocket)
	{
		packets = mNetworkSocket->SendPendingPackets();
		if (packets == -1)
		{
			// lost connection
			// Indicate we are disconnected
			mNetworkSocket->Shutdown();
			mID = 0;
		}
		// Check when no packets were sent
		if (packets == 0)
		{
			// Consider sending a dummy packet instead
			SendDummyPacket();
		}
	}

	return packets;
}
