using System;
using System.Collections;

namespace MOG_ControlsLibrary
{
	/// <summary>
	/// Summary description for MapList.
	/// </summary>
	public class util_MapList
	{
		#region Member vars
		private ArrayList keys = new ArrayList();
		private ArrayList values = new ArrayList();
		#endregion
		#region Properties
		public ArrayList Keys
		{
			get { return this.keys; }
		}
		public ArrayList Values
		{
			get { return this.values; }
		}
		#endregion
		#region Constructor
		public util_MapList()
		{
		}
		#endregion
		#region Public functions
		public void Clear()
		{
			keys.Clear();
			values.Clear();
		}

		public int IndexOfKey(object key)
		{
			return keys.IndexOf(key);
		}

		public int IndexOfValue(object val)
		{
			return values.IndexOf(val);
		}

		public int Add(object key, object val)
		{
			keys.Add(key);
			values.Add(val);

			return keys.Count;
		}

		public void Remove(object key)
		{
			if (keys.Contains(key))
			{
				int index = keys.IndexOf(key);
				keys.RemoveAt(index);
				values.RemoveAt(index);
			}
		}

		public bool ContainsKey(object key)
		{
			return keys.Contains(key);
		}

		public bool ContainsValue(object val)
		{
			return values.Contains(val);
		}

		public object GetKeyByValue(object val) 
		{
			if (values.Contains(val))
				return keys[ values.IndexOf(val) ];

			return null;
		}

		public object Get(object key)
		{
			if (!keys.Contains(key))
				return null;

			return values[ keys.IndexOf(key) ];
		}

		public object GetByIndex(int index)
		{
			if (index >= keys.Count)
				return null;

			return values[index];
		}
		#endregion
	}
}
