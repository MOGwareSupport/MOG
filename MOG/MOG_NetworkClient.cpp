//--------------------------------------------------------------------------------
//	MOG_NetworkClient.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "stdafx.h"


#include "MOG_Network.h"
#include "MOG_NetworkClient.h"
#include "MOG_ControllerSystem.h"
#include "MOG_Prompt.h"

using namespace System::Net;


MOG_NetworkClient::MOG_NetworkClient()
{
	mNetworkSocket = NULL;
}


bool MOG_NetworkClient::Initialize()
{
	mID = INVALID_NETWORK_ID;
	mComputerName = Environment::MachineName;
	mIP = NetworkSocket::GetIPAddress(MOG_ControllerSystem::GetSystem()->ServerIP);
	mMacAddress = NetworkSocket::GetMacAddress(mIP);

	// Check the MOG_MAJOR_VERSION
	if (MOG_ControllerSystem::GetSystem()->ServerMajorVersion != MOG_MAJOR_VERSION)
	{
		MOG_Prompt::PromptMessage(S"Incompatible Server", String::Concat(S"Server - Major: ", Convert::ToString(MOG_ControllerSystem::GetSystem()->ServerMajorVersion), S"   Minor: ", Convert::ToString(MOG_ControllerSystem::GetSystem()->ServerMinorVersion),
									S"\nYour - Major: ", Convert::ToString(MOG_MAJOR_VERSION), S"   Minor:", Convert::ToString(MOG_MINOR_VERSION),
									S"\nThe server is not compatible with this client.  Please restart your client so you will be updated to the newly deployed version."), S"");
		MOG_Main::Shutdown();
		return false;
	}
	// Check the MOG_MINOR_VERSION
	if (MOG_ControllerSystem::GetSystem()->ServerMinorVersion != MOG_MINOR_VERSION)
	{
		MOG_Prompt::PromptMessage(S"Incompatible Server", String::Concat(S"Server - Major: ", Convert::ToString(MOG_ControllerSystem::GetSystem()->ServerMajorVersion), S"   Minor: ", Convert::ToString(MOG_ControllerSystem::GetSystem()->ServerMinorVersion),
									S"\nYour - Major: ", Convert::ToString(MOG_MAJOR_VERSION), S"   Minor: ", Convert::ToString(MOG_MINOR_VERSION),
									S"\nThe server is still compatible with this client but it is recomended that you update."), S"");
	}

	return ConnectToServer(false);
}

//return 1 on success
bool MOG_NetworkClient::ConnectToServer(int tryUntilConnected)
{
	int done = 0;

	// Eat all commands when we are offline
	if (MOG_ControllerSystem::GetOffline())
	{
		return true;
	}

	while(!done)
	{
		try
		{
			IPAddress *serverIP = IPAddress::Parse(MOG_ControllerSystem::GetSystem()->ServerIP);

			IPEndPoint *lep = new IPEndPoint(serverIP, MOG_ControllerSystem::GetSystem()->ServerPort);
			Socket* pSocket = new Socket(lep->Address->AddressFamily, SocketType::Stream, ProtocolType::Tcp);

			// Set the timeout option on the blocking connect call
			pSocket->SetSocketOption(SocketOptionLevel::Socket, SocketOptionName::SendTimeout, 100);
			
			// Set the buffer sizes to be extra large
			// MOG_NetworkSockets currently requires larger buffers because it must read/write an entire packet from the buffer.
			pSocket->SetSocketOption(SocketOptionLevel::Socket, SocketOptionName::SendBuffer, 50*1024);
			pSocket->SetSocketOption(SocketOptionLevel::Socket, SocketOptionName::ReceiveBuffer, 50*1024);

			// Connect
			pSocket->Connect(lep);

			// Shut off the blocking
			pSocket->Blocking = false;

// Removed because this didn't fix the problem with CommandLines sending their final packets before shutdown, but left it in for future reference
//			// The socket will linger for 10 seconds after
//			// Socket.Close is called.
//			pSocket->LingerState = new LingerOption(true, 10);

			if (mNetworkSocket)
			{
				mNetworkSocket->ChangeSocket(pSocket);
			}
			else
			{
				mNetworkSocket = new NetworkSocket(pSocket);
			}

			return mNetworkSocket->IsConnected();
		}
		catch (Exception *e)
		{
			String *error = e->ToString();

			if (!tryUntilConnected)
				return mNetworkSocket ? mNetworkSocket->IsConnected() : false;	//failed.
		}
	}

	return mNetworkSocket->IsConnected();
}

bool MOG_NetworkClient::CloseConnectionToServer()
{
	try
	{
		if (mNetworkSocket)
		{
			mNetworkSocket->Shutdown();
		}
	}
	catch (...)
	{
	}

	// Remove the connection to the server
	mID = 0;

	return true;
}

bool MOG_NetworkClient::SendToServer(NetworkPacket *packet)
{
	// Eat all commands when we are offline
	if (MOG_ControllerSystem::GetOffline())
	{
		return true;
	}

	// Send the packet to the server
	mNetworkSocket->WritePacket(packet);

	// Always attempt to immediately push it out so we won't wait for the next tick
	SendPendingPackets();

	return true;
}

ArrayList *MOG_NetworkClient::ReadPackets(void)
{
	// Eat all commands when we are offline
	if (MOG_ControllerSystem::GetOffline())
	{
		return new ArrayList();
	}

	ArrayList *packets = mNetworkSocket->ReadPackets();
	if (!packets)
	{
		// Indicate we are disconnected
		mNetworkSocket->Shutdown();
		mID = 0;
	}

	return packets;
}
