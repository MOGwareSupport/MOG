//--------------------------------------------------------------------------------
//	MOG_NetworkServer.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "stdafx.h"

#include "MOG_NetworkSocket.h"
#include "MOG_Network.h"
#include "MOG_NetworkServer.h"
#include "MOG_Command.h"
#include "MOG_ControllerSystem.h"

using namespace System::Net;

MOG_NetworkServer::MOG_NetworkServer()
{
	mConnections = new HybridDictionary();
	mLastDummyPacketSentTime = DateTime::Now;
}


bool MOG_NetworkServer::Initialize(int serverPort)
{
	mServerNextID = 2;		//next ID to give out.

	mID = 1;
	mComputerName = Environment::MachineName;
	mIP = NetworkSocket::GetIPAddress(MOG_ControllerSystem::GetSystem()->ServerIP);
	mMacAddress = NetworkSocket::GetMacAddress(mIP);

	mServerPort = serverPort;

	IPAddress *address = NULL;
	IPEndPoint *localEndPoint = NULL;

	// Establish the server's socket connection
	try
	{
		address = IPAddress::Parse(mIP);
		localEndPoint = new IPEndPoint(address, serverPort);
	}
	catch(Exception *ea)
	{
		MOG_ASSERT_THROW(false, MOG_Exception::MOG_EXCEPTION_Generic, String::Concat(S"MOG_NetworkServer::Initialize - Did not get a valid ip or localEndpoint that are used to setup our tcp socket!\nMessage = (", ea->Message->ToString(), S")\n\nIs your server port (", Convert::ToString(serverPort), S") within a valid range?"));
		return false;
	}
	
	// Do I have a valid localEndPoint and Address?
	if (localEndPoint && address)
	{
		try
		{
			Socket* pSocket = new Socket(localEndPoint->Address->AddressFamily, SocketType::Stream, ProtocolType::Tcp);
			pSocket->Blocking = false;
			pSocket->Bind(localEndPoint);
			pSocket->Listen(1000);

			mNetworkSocket = new NetworkSocket(pSocket);
		}
		catch (Exception *e)
		{
			e->ToString();
			MOG_ASSERT_THROW(false, MOG_Exception::MOG_EXCEPTION_Generic, String::Concat(S"MOG_NetworkServer::Initialize - Could not create server tcp socket!\nMessage = ", e->Message->ToString()));
			return false;
		}
	}

	return true;
}


int MOG_NetworkServer::CheckNewConnections(void)
{
	try
	{
		Socket *newConnectionSocket = mNetworkSocket->Accept();
		if (newConnectionSocket)
		{
			NetworkConsoleMessage("CheckNewConnections", "Recieved new connection request.", Environment::StackTrace);

			newConnectionSocket->Blocking = false;

			// Initialize the connection IP & ID
			MOG_NetworkConnection *newConnection = new MOG_NetworkConnection();
			newConnection->mID = mServerNextID++;	// Increment to the next ID to give out.
			newConnection->mIP = "Invalid";
			newConnection->mNetworkSocket = new NetworkSocket(newConnectionSocket);
			// Add this new connection
			mConnections->Add(newConnection->mID.ToString(), newConnection);
			NetworkConsoleMessage("CheckNewConnections", "Connection accepted.", Environment::StackTrace);

			// Return the assigned NetworkID of this new connection
			return newConnection->mID;
		}
	}
	// No more connections to be added
	catch (...)
	{
	}

	return 0;
}


ArrayList *MOG_NetworkServer::GetAllConnectionsArray(void)
{
	ArrayList *connections = new ArrayList();

	// Enumerate through our connectiosn and build us an independant array
	IDictionaryEnumerator* myEnumerator = mConnections->GetEnumerator();
	while ( myEnumerator->MoveNext() )
	{
		connections->Add(__try_cast<MOG_NetworkConnection *>(myEnumerator->Value));
	}

	return connections;
}


ArrayList *MOG_NetworkServer::ReadNewConnections(void)
{
	ArrayList *packets = new ArrayList();

	// Loop through all of our connections
	ArrayList *connections = GetAllConnectionsArray();
	for (int i = 0; i < connections->Count; i++)
	{
		MOG_NetworkConnection *connection = __try_cast<MOG_NetworkConnection *>(connections->Item[i]);

		// Read the packets from this connection
		ArrayList *array = ReadPackets(connection);
		// if we have recieved a packet add it to the actual list.
		if (array && array->Count > 0)
		{
			packets->AddRange(array);
		}
	}

	return packets;
}


bool MOG_NetworkServer::CloseAllConnections()
{
	ArrayList *packets = NULL;

	try
	{
		// Loop through all of our connections
		ArrayList *connections = GetAllConnectionsArray();
		for (int i = 0; i < connections->Count; i++)
		{
			MOG_NetworkConnection *connection = __try_cast<MOG_NetworkConnection *>(connections->Item[i]);
			if (connection)
			{
				connection->mNetworkSocket->Shutdown();
			}
		}
	}
	catch (...)
	{
	}

	// Clear all of our connections
	mConnections = new HybridDictionary;

	return true;
}


bool MOG_NetworkServer::CloseConnection(int networkID)
{
	ArrayList *packets = NULL;

	// Get the connection for this networkID
	MOG_NetworkConnection *connection = __try_cast<MOG_NetworkConnection *>(mConnections->Item[networkID.ToString()]);
	if (connection)
	{
		// Shut down the socket
		connection->mNetworkSocket->Shutdown();

		// Remove the connection from the list
		mConnections->Remove(networkID.ToString());
		return true;
	}

	return false;
}


bool MOG_NetworkServer::SendPacketToConnection(NetworkPacket *packet, int networkID)
{
	// Get the connection for this networkID
	MOG_NetworkConnection *connection = __try_cast<MOG_NetworkConnection *>(mConnections->Item[networkID.ToString()]);
	if (connection)
	{
		// Write the packet to the connection
		if (connection->mNetworkSocket)
		{
			if (connection->mNetworkSocket->WritePacket(packet))
			{
				return true;
			}
		}
	}

	return false;
}

ArrayList *MOG_NetworkServer::ReadPackets(int networkID)
{
	// Read packets from the connection  for this networkID
	MOG_NetworkConnection *connection = __try_cast<MOG_NetworkConnection *>(mConnections->Item[networkID.ToString()]);
	if (connection)
	{
		return ReadPackets(connection);
	}

	return NULL;
}

ArrayList *MOG_NetworkServer::ReadPackets(MOG_NetworkConnection *connection)
{
	ArrayList *packets = NULL;

	// Make sure we have a valid connection?
	if (connection)
	{
		// Read the packets from the specified connection
		packets = connection->mNetworkSocket->ReadPackets();
		if (!packets)
		{
			// There was a problem, remove the connection from the server
			// Shut down the socket
			connection->mNetworkSocket->Shutdown();

			// Remove the connection from the list
			mConnections->Remove(connection->mID.ToString());
		}
	}

	return packets;
}


bool MOG_NetworkServer::SendPacketToAll(NetworkPacket *packet)
{
	// Loop through all of our connections
	ArrayList *connections = GetAllConnectionsArray();
	for (int i = 0; i < connections->Count; i++)
	{
		MOG_NetworkConnection *connection = __try_cast<MOG_NetworkConnection *>(connections->Item[i]);
		if (connection)
		{
			return SendPacketToConnection(packet, connection->mID);
		}
	}

	return 0;
}


bool MOG_NetworkServer::SendDummyPacket()
{
	// Allow our parent to send the default dummypacket
	bool bDummyPacketSent = MOG_Network::SendDummyPacket();
	if (bDummyPacketSent)
	{
		// Loop through all of our connections
		ArrayList *connections = GetAllConnectionsArray();
		for (int i = 0; i < connections->Count; i++)
		{
			MOG_NetworkConnection *connection = __try_cast<MOG_NetworkConnection *>(connections->Item[i]);
			if (connection)
			{
				// Attempt to send each one of them a dummy packet to ensure connectivity
				connection->mNetworkSocket->SendDummyPacket();
			}
		}

		return true;
	}

	return false;
}


int MOG_NetworkServer::SendPendingPackets()
{
	int packets = MOG_Network::SendPendingPackets();

	// Loop through all of our connections
	ArrayList *connections = GetAllConnectionsArray();
	for (int i = 0; i < connections->Count; i++)
	{
		MOG_NetworkConnection *connection = __try_cast<MOG_NetworkConnection *>(connections->Item[i]);
		if (connection)
		{
			int result = connection->mNetworkSocket->SendPendingPackets();
			if (result == -1)
			{
				// Shut down the socket
				connection->mNetworkSocket->Shutdown();

				// Remove the connection from the list
				mConnections->Remove(connection->mID.ToString());
			}
			else
			{
				packets += result;
			}
		}
	}

	return packets;
}
