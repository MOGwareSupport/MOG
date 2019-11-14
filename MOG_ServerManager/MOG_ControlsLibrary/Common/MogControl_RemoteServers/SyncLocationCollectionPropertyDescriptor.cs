using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace MOG_ServerManager
{
	class SyncLocationCollectionPropertyDescriptor : PropertyDescriptor
	{
		private SyncLocationCollection collection = null;
		private int index = -1;

		public SyncLocationCollectionPropertyDescriptor(SyncLocationCollection coll, int idx)
			:
			base("#" + idx.ToString(), null)
		{
			this.collection = coll;
			this.index = idx;
		}

		public override AttributeCollection Attributes
		{
			get
			{
				return new AttributeCollection(null);
			}
		}

		public override bool CanResetValue(object component)
		{
			return true;
		}

		public override Type ComponentType
		{
			get
			{
				return this.collection.GetType();
			}
		}

		public override string DisplayName
		{
			get
			{
				SyncLocation emp = this.collection[index];
				if (emp != null)
				{
					return emp.Location + " " + emp.Name;
				}
				else
				{
					return "";
				}
			}
		}

		public override string Description
		{
			get
			{
				SyncLocation emp = this.collection[index];
				StringBuilder sb = new StringBuilder();
				sb.Append(emp.Location);
				sb.Append(",");
				sb.Append(emp.Name);
				sb.Append(",");
				sb.Append(emp.IpAddress);				

				return sb.ToString();
			}
		}

		public override object GetValue(object component)
		{
			return this.collection[index];
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override string Name
		{
			get { return "#" + index.ToString(); }
		}

		public override Type PropertyType
		{
			get { return this.collection[index].GetType(); }
		}

		public override void ResetValue(object component)
		{
		}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}

		public override void SetValue(object component, object value)
		{
			// this.collection[index] = value;
		}
	}
}
