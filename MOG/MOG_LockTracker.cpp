//--------------------------------------------------------------------------------
//	MOG_LockTracker.cpp
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Define.h"
#include "MOG_Command.h"
#include "MOG_StringUtils.h"
#include "MOG_CommandFactory.h"

#include "MOG_LockTracker.h"



MOG_LockTracker::MOG_LockTracker(MOG_CommandManager* commandManager, bool useLockWaitDialog)
{
	mCommandManager = commandManager;
	mPersistentLocks = new HybridDictionary();
	mUseLockWaitDialog = useLockWaitDialog;
}


bool MOG_LockTracker::UpdateLocks(MOG_Command *pCommand)
{
	if (pCommand)
	{
		// Ensure exclusivity
		Monitor::Enter(mPersistentLocks);
		__try
		{
			// Determin the type of lock?
			switch(pCommand->GetCommandType())
			{
				case MOG_COMMAND_LockWriteRequest:
				case MOG_COMMAND_LockReadRequest:
					mPersistentLocks->Item[pCommand->GetAssetFilename()->GetOriginalFilename()] = pCommand;
					break;
				case MOG_COMMAND_LockWriteRelease:
				case MOG_COMMAND_LockReadRelease:
					mPersistentLocks->Remove(pCommand->GetAssetFilename()->GetOriginalFilename());
					break;
			}
		}
		__finally
		{
			Monitor::Exit(mPersistentLocks);
		}
	}

	// Indicate that we received and processed the command
	return true;
}


bool MOG_LockTracker::PersistentLockQuery(MOG_Command *pCommand)
{
	MOG_Command *lockContainer = pCommand;

	// Ensure exclusivity
	Monitor::Enter(mPersistentLocks);
	__try
	{
		// Both read and write locks need to check for existing write locks
		IDictionaryEnumerator* myEnumerator = mPersistentLocks->GetEnumerator();
		while ( myEnumerator->MoveNext() )
		{
			MOG_Command *lock = __try_cast <MOG_Command*>(myEnumerator->Value);

			// By adding the "\\*" we can check all of the following at once
			//      Make sure there isn't an exact match?
			//      Make sure there are no higher-level locks?
			//      Make sure there are no lower-level locks?
			if (MOG_StringCompare(String::Concat(lock->GetAssetFilename()->GetOriginalFilename(), S"\\*"), String::Concat(pCommand->GetAssetFilename()->GetOriginalFilename(), S"\\*")))
			{
				// Imbed the colliding lock
				lockContainer->SetCommand(lock);
				lockContainer = lock;
			}
		}
	}
	__finally
	{
		Monitor::Exit(mPersistentLocks);
	}

	// Check to see if we found any collining locks
	if (pCommand->GetCommand())
	{
		return true;
	}

	return false;	
}


bool MOG_LockTracker::CommandExecute(MOG_Command *pCommand)
{
	bool processed = false;

	switch (pCommand->GetCommandType())
	{
		case MOG_COMMAND_LockCopy:
			processed = Command_LockCopy(pCommand);
			break;
		case MOG_COMMAND_LockMove:
			processed = Command_LockMove(pCommand);
			break;

		case MOG_COMMAND_LockReadRequest:
		case MOG_COMMAND_LockWriteRequest:
			processed = Command_LockRequest(pCommand);
			break;
		case MOG_COMMAND_LockReadRelease:
		case MOG_COMMAND_LockWriteRelease:
			processed = Command_LockRelease(pCommand);
			break;

		case MOG_COMMAND_LockWriteQuery:
		case MOG_COMMAND_LockReadQuery:
			processed = Command_LockQuery(pCommand);
			break;

		case MOG_COMMAND_LockPersistentNotify:
			processed = Command_LockPersistentNotify(pCommand);
			break;
	}

	return processed;
}


bool MOG_LockTracker::Command_LockCopy(MOG_Command *pCommand)
{
	MOG_Command *command = MOG_CommandFactory::Setup_LockReadRequest(pCommand->GetSource(), String::Concat("MOG_CommandManager::Command_LockCopy(Source) - ", pCommand->GetDescription()));
	bool completed = false;

	// Get the ReadLock on the source
	if (Command_LockRequest(command))
	{
		// Get the WriteLock on the target
		command = MOG_CommandFactory::Setup_LockWriteRequest(pCommand->GetDestination(), String::Concat("MOG_CommandManager::Command_LockCopy(Target) - ", pCommand->GetDescription()));
		if (Command_LockRequest(command))
		{
			// Copy the asset tree
			DosUtils::Copy(pCommand->GetSource(), pCommand->GetDestination(), false);

			// Release the destination lock
			command = MOG_CommandFactory::Setup_LockWriteRelease(pCommand->GetDestination());
			Command_LockRelease(command);

			// Indicate we successfully processed the command
			completed = true;
		}
		// Release the source lock
		command = MOG_CommandFactory::Setup_LockReadRelease(pCommand->GetSource());
		Command_LockRelease(command);
	}

	// Indicate the status of the command
	if (completed)
	{
		return true;
	}

	return false;
}


bool MOG_LockTracker::Command_LockMove(MOG_Command *pCommand)
{
	MOG_Command *command = MOG_CommandFactory::Setup_LockWriteRequest(pCommand->GetSource(), String::Concat(S"MOG_CommandManager::Command_LockMove(Source) - ", pCommand->GetDescription()));
	bool completed = false;

	// Get the WriteLock on the source
	if (Command_LockRequest(command))
	{
		// Get the WriteLock on the target
		command = MOG_CommandFactory::Setup_LockWriteRequest(pCommand->GetDestination(), String::Concat("MOG_CommandManager::Command_LockMove(Target) - ", pCommand->GetDescription()));
		if (Command_LockRequest(command))
		{
			// Move the asset
			DosUtils::Move(pCommand->GetSource(), pCommand->GetDestination());

			// Release the destination lock
			command = MOG_CommandFactory::Setup_LockWriteRelease(pCommand->GetDestination());
			Command_LockRelease(command);

			// Indicate we successfully processed the command
			completed = true;
		}
		// Release the source lock
		command = MOG_CommandFactory::Setup_LockWriteRelease(pCommand->GetSource());
		Command_LockRelease(command);
	}

	// Indicate the status of the command
	if (completed)
	{
		return true;
	}

	return false;
}


bool MOG_LockTracker::Command_LockRequest(MOG_Command *pCommand)
{
	bool bPerformLock = false;
	bool bLocked = false;

	// Check if this is a persistent lock?
	if (pCommand->IsPersistentLock())
	{
		// Allow PersistentLockQuery to populate pCommand with any obstructing locks (faster if there is a blocking lock)
		if (PersistentLockQuery(pCommand) == false)
		{
			bPerformLock = true;
		}
	}
	else
	{
		bPerformLock = true;
	}

	//Should we perform the lock?
	if (bPerformLock)
	{
		// Looks like wwe can proceed to request the lock from the server
		if (mCommandManager->SendToServerBlocking(pCommand))
		{
			bLocked = true;
			// Make sure we immediately update our local tables so we don't have to wait for the server's normal notification
			UpdateLocks(pCommand);
		}
	}

	// Check if pCommand was populated with an existing lock?
	if (!bLocked)
	{
		// Check if this LockTracker has been instructed to wait for locks to free?
		if (mUseLockWaitDialog)
		{
			// Wait for this lock to be freed
			bLocked = mCommandManager->LockWaitDialog(pCommand);
		}
	}

	return bLocked;
}


bool MOG_LockTracker::Command_LockRelease(MOG_Command *pCommand)
{
	bool bUnlocked = false;

	// Send the command to the server for release
	if (mCommandManager->SendToServerBlocking(pCommand))
	{
		// Immediately update our local tables so we don't have to wait for the server's normal notification
		UpdateLocks(pCommand);
		bUnlocked = true;
	}

	return bUnlocked;
}


bool MOG_LockTracker::Command_LockQuery(MOG_Command *pCommand)
{
	bool bSuccess = false;

	// Check if this is a persistent lock?
	if (pCommand->IsPersistentLock())
	{
		// We can simply check our local copy of all persistent locks
		if (PersistentLockQuery(pCommand))
		{
			bSuccess = true;
		}
	}
	else
	{
		// Looks like we need to request this info from the server
		bSuccess = mCommandManager->SendToServerBlocking(pCommand);
	}

	return bSuccess;
}


bool MOG_LockTracker::Command_LockPersistentNotify(MOG_Command *pCommand)
{
	// Get the lock contained within this notify command
	MOG_Command *lock = pCommand->GetCommand();
	if (lock)
	{
		// Immediately update our local tables so we don't have to wait for the server's normal notification
		UpdateLocks(lock);
	}

	// Always eat this command
	return true;
}



