using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using MOG_ControlsLibrary.MogUtils_Settings;

namespace MOG_RemoteServerManager
{
	[TypeConverter(typeof(SyncLocationConverter))]
	public class SyncLocation
	{
		public enum SyncScheduleTypes
		{
			Once,
			Daily,
			Weekly,
			Monthly,
			Custom,
		}

		public enum SyncTypes
		{
			Push,
			Pull,
			Full
		}

		public SyncLocation()
		{
			Enable = true;
			Port = 7655;
			TimeOut = new TimeSpan(0, 1, 0);
			PacketSize = 1024;
			Name = "New remote server";
			SyncType = SyncTypes.Full;
			SyncSchedule = SyncScheduleTypes.Weekly;			
		}

		private string mIpAddress;

		public string IpAddress
		{
			get { return mIpAddress; }
			set { mIpAddress = value; }
		}

		private bool mEnable;

		public bool Enable
		{
			get { return mEnable; }
			set { mEnable = value; }
		}

		private int mPort;

		public int Port
		{
			get { return mPort; }
			set { mPort = value; }
		}

		private string mEncryptionKey;

		public string EncryptionKey
		{
			get { return mEncryptionKey; }
			set { mEncryptionKey = value; }
		}

		private TimeSpan mTimeOut;

		public TimeSpan TimeOut
		{
			get { return mTimeOut; }
			set { mTimeOut = value; }
		}

		private int mPacketSize;

		public int PacketSize
		{
			get { return mPacketSize; }
			set { mPacketSize = value; }
		}

		private string mName;

		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}

		private string mLocation;

		public string Location
		{
			get { return mLocation; }
			set { mLocation = value; }
		}
								
		private SyncTypes mSyncType;
		public SyncTypes SyncType
		{
			get { return mSyncType; }
			set { mSyncType = value; }
		}
		
		private SyncScheduleTypes mSyncSchedule;
		public SyncScheduleTypes SyncSchedule
		{
			get { return mSyncSchedule; }
			set { mSyncSchedule = value; }
		}

		private TimeSpan mSyncTimeOffset;
		public TimeSpan SyncTimeOffset
		{
			get { return mSyncTimeOffset; }
			set { mSyncTimeOffset = value; }
		}	
	}

	// This is a special type converter which will be associated with the Employee class.
	// It converts an Employee object to string representation for use in a property grid.
	internal class SyncLocationConverter : ExpandableObjectConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
		{
			if (destType == typeof(string) && value is SyncLocation)
			{
				// Cast the value to an Employee type
				SyncLocation emp = (SyncLocation)value;

				// Return department and department role separated by comma.
				return emp.IpAddress + ", " + emp.Name;
			}
			return base.ConvertTo(context, culture, value, destType);
		}
	}

}
