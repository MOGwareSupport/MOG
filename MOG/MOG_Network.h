//--------------------------------------------------------------------------------
//	MOG_Network.h
//	
//	
//--------------------------------------------------------------------------------
#ifndef __MOG_NETWORK_H__
#define __MOG_NETWORK_H__

#include <stdlib.h>

#include "MOG_NetworkSocket.h"

using namespace System;

public __gc class MOG_Network
{
public:
				MOG_Network();

	virtual int SendPendingPackets();
	virtual bool SendDummyPacket();

	String*		GetIP()				{ return mIP; };
	String*		GetMacAddress()		{ return mMacAddress; };
	int			GetID()				{ return mID; };
	String*		GetComputerName()	{ return mComputerName; };

	void		SetID(Int32 id);

protected:
	NetworkSocket*	mNetworkSocket;
	DateTime	mLastDummyPacketSentTime;

	// Network Information
	int			mID;				//the network ID	- set to 1 for the server.
	String*		mIP;				//the computers IP
	String*		mMacAddress;		//the computers mac address
	String*		mComputerName;		//the computers name
};

#endif

