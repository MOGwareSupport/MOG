using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace MOG_ServerManager
{
	public class RemoteSettings
	{

		SyncLocationCollection mSyncLocations = new SyncLocationCollection(); 
			
		[TypeConverter(typeof(SyncLocationCollectionConverter))]
		public SyncLocationCollection SyncSources
		{
			get { return mSyncLocations; }
		}		
	}

	// This is a special type converter which will be associated with the EmployeeCollection class.
	// It converts an EmployeeCollection object to a string representation for use in a property grid.
	internal class SyncLocationCollectionConverter : ExpandableObjectConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
		{
			if (destType == typeof(string) && value is SyncLocationCollection)
			{
				SyncLocationCollection col = value as SyncLocationCollection;
				if (col != null)
				{
					return col.Count.ToString() + " Sync Locations";
				}
				// Return department and department role separated by comma.
				return "Sync Locations data";
			}
			return base.ConvertTo(context, culture, value, destType);
		}
	}
}
