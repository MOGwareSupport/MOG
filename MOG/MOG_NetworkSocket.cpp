//--------------------------------------------------------------------------------
//	MOG_NetworkSocket.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "stdafx.h"

#include "MOG_NetworkSocket.h"
#include "MOG_Report.h"
#include "MOG_Command.h"
#include "MOG_StringUtils.h"


//#define REPORT_READ_AND_WRITES

// Very slow if you define VERIFY_WRITES.  This is to check and make sure that we are not losing packets.
// #define VERIFY_WRITES
#ifdef VERIFY_WRITES
#include "mog_command.h"
#endif


using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Threading;
using namespace System::Net::NetworkInformation;


NetworkSocket::NetworkSocket(Socket* pSocket)
{
	// Set the buffer sizes to be extra large
	// MOG_NetworkSockets currently requires larger buffers because it must read/write an entire packet from the buffer.
	pSocket->SetSocketOption(SocketOptionLevel::Socket, SocketOptionName::SendBuffer, 50*1024);
	pSocket->SetSocketOption(SocketOptionLevel::Socket, SocketOptionName::ReceiveBuffer, 50*1024);

	mSocket = pSocket;
	mPendingPackets = new ArrayList;
	mMutex = new Mutex;
}

NetworkSocket::~NetworkSocket()
{
}

void NetworkSocket::Shutdown()
{
	if (mSocket && mSocket->Connected)
	{
		// Make sure we send all our pending packets before we close the connection
		if (SendPendingPackets() < 0)
		{
			// This might be a good time to error or warn the user that all pending packets were not sent
			// We didn't want to do it in a while because it could be problematic if the socket kept failing forever.
		}

		// Shutdown and close the socket
		mSocket->Shutdown(SocketShutdown::Both);
		mSocket->Close();
	}
}

Socket* NetworkSocket::Accept()
{
	try
	{
		if (mSocket)
		{
			return mSocket->Accept();
		}
	}
	catch (SocketException*)
	{
		//The Accept method throws this exception if you're in non-blocking mode and it can't accept anything right away
		//Apparently this is how MOG was designed to work until further notice, so don't worry about seeing this exception happen a lot!
	}

	return NULL;
}

bool NetworkSocket::IsConnected()
{
	return (mSocket && mSocket->Connected);
}

void NetworkSocket::ChangeSocket(Socket* pSocket)
{
	Shutdown();

	mSocket = pSocket;
}


String* NetworkSocket::GetIPAddress(String* serverIPAddress)
{
	String *localAddress = String::Empty;
	IPHostEntry *hostEntry = Dns::GetHostEntry(Dns::GetHostName());

    try
    {
        if (hostEntry->AddressList->Length > 0)
        {
			// Assume the first one is best
			String *bestIPAddress = "0.0.0.0";
			int bestLikeness = -1;

			for (int i = 0; i < hostEntry->AddressList->Count; i++)
            {
				IPAddress *ipAddress = dynamic_cast<IPAddress*>(hostEntry->AddressList->Item[i]);

				String* thisIPAddress = ipAddress->ToString();
				int thisLikeness = StringUtils::CompareStartsWithLikeness(thisIPAddress, serverIPAddress);
				if (thisLikeness > bestLikeness)
				{
					bestLikeness = thisLikeness;
					bestIPAddress = thisIPAddress;
	            }
			}

			localAddress = bestIPAddress;
        }
    }
    catch (...)
    {
        // Log any errors
    }

    return localAddress;
}

String *NetworkSocket::GetMacAddress(String *localIPAddress)
{
	// Now here is the code to enumerate and write the ->
	// MAC Address of each Network Interface on your system.
	String *mac = S"";

	// This gets all the Network Interfaces in an array
	NetworkInterface* nics[] = NetworkInterface::GetAllNetworkInterfaces();

	// Now enumerate all the interfaces
	for (int i = 0; i < nics->Count; i++)
	{
		NetworkInterface *nic = dynamic_cast<NetworkInterface*>(nics->Item[i]);

		// Return the byte[] Physical Address
		// This will hold the MAC Address
		Byte bytes[] = nic->GetPhysicalAddress()->GetAddressBytes();
		// Convert the bytes into a string and append the mac variable
		for (int i = 0; i < bytes->Length; i++)
		{
			mac = String::Concat(mac, bytes[i].ToString("X2"));
			// Add the seperator
			if (i != bytes->Length - 1)
			{
				mac = String::Concat(mac, ":");
			}
		}

		// Check if we have obtained a MacAddress?
		if (mac->Length)
		{
			// We need to be better than but for now, stop after the 1st Mac Address
			break;
		}
	}

	return mac;
}


ArrayList *NetworkSocket::ReadPackets()
{
	// Allocate the array of packets
	ArrayList *packets = new ArrayList;
	Byte buffer[] = new Byte[4];
	int packetLength;
	bool connected;

	// Make sure we have a valid socket?
	if (mSocket)
	{
		// Check if the socket is still connected?
		connected = mSocket->Connected;
		while(connected && mSocket->Connected)
		{
			// Test the connection by attempting to read the first 4 bytes
			try
			{
				// Make sure no one EVER enters this in another thread while we are reading packates
				Monitor::Enter(mSocket);
				try
				{
					//Don't try to receive unless there is actually data present or it will throw an exception
					// Attempt to peek the packet length from the socket
					if (mSocket->Available >= 4 && mSocket->Receive(buffer, 0, 4, SocketFlags::Peek) == 4)
					{
						// Calculate the specified length
						packetLength = buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24);

						NetworkPacket *packet = new NetworkPacket;
						packet->buffer = new Byte[packetLength];

// JohnRen - Tried to fix packets larger then 8k (buffer size) but this code was causing network instability with packet reads getting out of sync...reverted back to old way
//						// Read the packet's data from the socket's buffer
//						int remainingBytes = packetLength;
//						int readBytes = 0;
//						while (remainingBytes)
//						{
//							// Check the available bytes in the socket
//							int availableBytes = mSocket->Available;
//							if (availableBytes > remainingBytes)
//							{
//								availableBytes = remainingBytes;
//							}
//
//							// Read the available bytes from the socket
//							mSocket->Receive(packet->buffer, readBytes, availableBytes, SocketFlags::Partial);
//							remainingBytes -= availableBytes;
//							readBytes += availableBytes;
//						}
//
//						// Add the packet to the array of packets
//						packets->Add(packet);

				        // Make sure the whole packet is available before we read it?
				        if (mSocket->Available >= packetLength)
				        {
					        NetworkPacket *packet = new NetworkPacket;
					        packet->buffer = new Byte[packetLength];
    
					        // Read the entire packet from the buffer
					        mSocket->Receive(packet->buffer, packetLength, SocketFlags::None);

					        // Add the packet to the array of packets
					        packets->Add(packet);
					    }
					}
					else
					{
						//No packets present...keep waiting for more data
						break;
					}
				}
				__finally
				{
					Monitor::Exit(mSocket);
				}
			}
			catch (ObjectDisposedException *e)
			{
				// Indicate this connection was closed
				e->ToString();
				connected = false;
				break;
			}
			catch (SocketException *e)
			{
				// This code is so we can set a breakpoint
				e->ToString();
				connected = connected;
				break;
			}
			catch (ThreadAbortException *e)
			{
				// Eat this error, since it means the user is shutting us down
				e->ToString();
				return NULL;
			}
			catch (Exception *e)
			{
				// This code is so we can set a breakpoint
				e->ToString();
				connected = connected;
				break;
			}
		}

#ifdef MOG_DEBUG_NETWORK
		if (packets->Count)
		{
			// Used for tracking the network packets
			MOG_Report::LogComment(String::Concat(S"NETWORK - Received ", packets->Count.ToString(), S" Packets"));
		}
#endif

		// Check if the socket's connection is still valid?
		if (connected && mSocket->Connected)
		{
			// Return a valid packets array of any packets collected during this read
			return packets;
		}
	}

	// Looking like we might need to fail...
	// Check if we have collected any packets before failure?
	if (packets->Count)
	{
		// Return the packets collected during this read and plan on failing again next time
		return packets;
	}

	// Return NULL so that the connection will be terminated
	return NULL;
}


bool NetworkSocket::SendDummyPacket()
{
	// We're basically just checking if the socket is still connected
	if (mSocket->Connected)
	{
		// Determin how long it has been since we last sent a packet?
		TimeSpan timeSpan = DateTime::Now - mLastPacketSentTime;
		if (timeSpan.TotalSeconds > 3)
		{
			//Make a new packet with a dummy command in it
			MOG_Command* command = new MOG_Command();
			command->SetCommandType(MOG_COMMAND_None);
			NetworkPacket* packet = command->Serialize();

			//Send this packet out 
			if (WritePacket(packet))
			{
				return true;
			}
		}
	}

	return false;
}


bool NetworkSocket::WritePacket(NetworkPacket *packet)
{
	mMutex->WaitOne();

	//Add the packet to the list so we can properly send them in the right order later regardless of whether we get an error now or not
	mPendingPackets->Add(packet);

	mMutex->ReleaseMutex();
	
	return true;
}

int NetworkSocket::SendPendingPackets()
{
	int packets = 0;

	if (mSocket && mPendingPackets && mMutex)
	{
		try
		{
			// This will ensure thread safety
			mMutex->WaitOne();

			//Go through the send packet list and send everything
			while (mPendingPackets->Count > 0)
			{
				try
				{
					NetworkPacket* packet = __try_cast<NetworkPacket*>(mPendingPackets->Item[0]);

					// Send the full packet
					if (mSocket->Send(packet->buffer, packet->size, SocketFlags::None) == packet->size)
					{
						//Packet was successfully sent
#ifdef MOG_DEBUG_NETWORK
						// Used for tracking the network packets
						MOG_Report::LogComment("NETWORK - Packet Sent");
#endif
						mPendingPackets->RemoveAt(0);
						packets++;
						mLastPacketSentTime = DateTime::Now;
					}
				}
				catch (SocketException *e)
				{
					String* error = e->Message;

					switch (e->ErrorCode)
					{
					case 10035: //WSAEWOULDBLOCK
						error = S"Resource temporarily unavailable";
						break;
					case 10040: //WSAEMSGSIZE
						error = S"Message too long";
						break;
					case 10050: //WSAENETDOWN
						error = S"Network is down";
						break;
					case 10051: //WSAENETUNREACH
						error = S"Network is unreachable";
						break;
					case 10053: //WSAECONNABORTED
						error = S"Connection Aborted";
						return -1;
					case 10055: //WSAENOBUFS
						error = S"No Buffer Space available";
						break;
					case 10091: //WSASYSNOTREADY
						error = S"Network subsystem is unavailable";
						break;
					case 11003: //WSANO_RECOVERY
						error = S"This is a non-recoverable error";
						break;
					}
					break;
				}
				catch (Exception *e)
				{
					//we did not send all the packets in the list
					MOG_Report::ReportMessage(S"Network Write Packet Error", e->Message, e->StackTrace, MOG_ALERT_LEVEL::CRITICAL);
					break;
				}
			}
		}
		__finally
		{
			mMutex->ReleaseMutex();
		}
	}

	return packets;
}

bool NetworkSocket::UpdatePendingPacketNetworkIDs(int id)
{
	mMutex->WaitOne();

	ArrayList* oldPackets = mPendingPackets;
	mPendingPackets = new ArrayList;

	for (int i = 0; i < oldPackets->Count; i++)
	{
		NetworkPacket* pPacket = __try_cast<NetworkPacket*>(oldPackets->Item[i]);
		if (pPacket)
		{
			MOG_Command* pCommand = MOG_Command::Deserialize(pPacket);
			pCommand->SetNetworkID(id);
			mPendingPackets->Add(pCommand->Serialize());
		}
	}

	mMutex->ReleaseMutex();

	return true;
}

int SerializeString(NetworkPacket *packet, int offset, String *string)
{
	if (string)
	{
		// Write the length of the string
		offset = SerializeInt32(packet, offset, string->Length);

		CharEnumerator *thisChar = string->GetEnumerator();
		while(thisChar->MoveNext())
		{
			packet->buffer[offset++] = (char)thisChar->Current;
		}
	}
	else
	{
		// Write the length of the string
		offset = SerializeInt32(packet, offset, 0);
	}

	return offset;
}


int SerializeBool(NetworkPacket *packet, int offset, bool value)
{
	packet->buffer[offset++] = (unsigned char)value;

	return offset;
}


int SerializeInt32(NetworkPacket *packet, int offset, Int32 value)
{
	char *ptr = (char *)&value;
	packet->buffer[offset++] = *ptr++;
	packet->buffer[offset++] = *ptr++;
	packet->buffer[offset++] = *ptr++;
	packet->buffer[offset++] = *ptr++;

	return offset;
}


int SerializeUInt32(NetworkPacket *packet, int offset, UInt32 value)
{
	char *ptr = (char *)&value;
	packet->buffer[offset++] = *ptr++;
	packet->buffer[offset++] = *ptr++;
	packet->buffer[offset++] = *ptr++;
	packet->buffer[offset++] = *ptr++;

	return offset;
}


int DeserializeString(NetworkPacket *packet, int offset, String **string)
{
	int len;

	// Get the length of the string
	offset = DeserializeInt32(packet, offset, &len);

	// Scan in the characters of the string
	char *tempBuffer = new char[len + 1];
	int c = 0;
	for(; c < len; c++)
	{
		tempBuffer[c] = (unsigned char)packet->buffer[offset++];
	}
	tempBuffer[c] = 0;

	*string = tempBuffer;

	return offset;
}


int DeserializeBool(NetworkPacket *packet, int offset, bool __gc *value)
{
	unsigned char byte = packet->buffer[offset++];

	if (!byte)
	{
		*value = false;
	}
	else
	{
		*value = true;
	}

	return offset;
}


int DeserializeInt32(NetworkPacket *packet, int offset, Int32 *value)
{
	UInt32 temp = 0;
	char *ptr = (char *)&temp;
	*ptr++ = packet->buffer[offset++];
	*ptr++ = packet->buffer[offset++];
	*ptr++ = packet->buffer[offset++];
	*ptr++ = packet->buffer[offset++];

	*value = temp;

	return offset;
}


int DeserializeUInt32(NetworkPacket *packet, int offset, UInt32 *value)
{
	UInt32 temp = 0;
	char *ptr = (char *)&temp;
	*ptr++ = packet->buffer[offset++];
	*ptr++ = packet->buffer[offset++];
	*ptr++ = packet->buffer[offset++];
	*ptr++ = packet->buffer[offset++];

	*value = temp;

	return offset;
}


