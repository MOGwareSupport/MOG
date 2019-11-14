using System;
using System.Collections.Generic;
using System.Text;
using MOG.INI;
using MOG.DOSUTILS;

namespace MOG_ControlsLibrary
{
	public class SyncFilter
	{
		private List<string> mExclusionList = new List<string>();
		private List<string> mInclusionList = new List<string>();
		
		public SyncFilter()
		{

		}

		public bool Load(string filename)
		{
			if (DosUtils.ExistFast(filename))
			{
				MOG_Ini ini = new MOG_Ini(filename);
				if (ini != null)
				{
					Clear();
					AddExclusions(ini.GetString("Sync", "Exclusions"));
					AddInclusions(ini.GetString("Sync", "Inclusions"));
					return true;
				}
			}
			
			return false;
		}

		public bool Save(string filename)
		{
			MOG_Ini ini = new MOG_Ini(filename);
			ini.PutString("Sync", "Exclusions", GetExclusionString());
			ini.PutString("Sync", "Inclusions", GetInclusionString());
			return ini.Save();
		}

		public void Clear()
		{
			mExclusionList.Clear();
			mInclusionList.Clear();
		}

		private bool Simplify(List<string> inclusion, List<string> exclusion, string newItem)
		{
			bool simplified = false;

			List<string> remove = new List<string>();
			// Loop through all the keys
			foreach (string listItem in inclusion)
			{
				// Check to see if this key is within the checkstring and is not the checkstring
				if (listItem.Contains(newItem) && 
					listItem != newItem && 
					exclusion.Contains(newItem) == false)
				{
					// If it is, then it is a child and should be removed
					remove.Add(listItem);
					simplified = true;
				}
			}

			// Remove all the keys that were unneeded
			foreach (string removeItem in remove)
			{
				inclusion.Remove(removeItem);
			}

			return simplified;
		}

		public void AddExclusion(string exclusion)
		{
			if (mExclusionList.Contains(exclusion) == false)
			{
				mExclusionList.Add(exclusion);
				Simplify(mExclusionList, mInclusionList, exclusion);
			}
		}

		public void AddInclusion(string inclusion)
		{
			if (mInclusionList.Contains(inclusion) == false)
			{
				mInclusionList.Add(inclusion);
				Simplify(mInclusionList, mExclusionList, inclusion);
			}
		}

		public void RemoveExclusion(string exclusion)
		{
			mExclusionList.Remove(exclusion);
			Simplify(mInclusionList, mExclusionList, exclusion);
		}

		public void RemoveInclusion(string inclusion)
		{
			mInclusionList.Remove(inclusion);
			Simplify(mExclusionList, mInclusionList, inclusion);
		}

		private void AddExclusions(string exclusionString)
		{
			AddCommaSeparatedStringToList(mExclusionList, exclusionString);
		}

		private void AddInclusions(string inclusionString)
		{
			AddCommaSeparatedStringToList(mInclusionList, inclusionString);
		}

		private void AddCommaSeparatedStringToList(List<string> list, string commaSeparatedString)
		{
			string[] parts = commaSeparatedString.Split(",;".ToCharArray());
			foreach (string item in parts)
			{
				// Make sure the item we are adding is not a blank
				if (item.Length > 0)
				{
					list.Add(item);
				}
			}
		}

		public string GetExclusionString()
		{
			return ListToString(mExclusionList);
		}

		public string GetInclusionString()
		{
			return ListToString(mInclusionList);
		}

		private string ListToString(List<string> list)
		{
			StringBuilder output = new StringBuilder();

			foreach (string entry in list)
			{
				if (output.Length > 0)
				{
					output.Append(",");
				}

				output.Append(entry);
			}

			return output.ToString();
		}

		public bool IsExcluded(string test)
		{
			return mExclusionList.Contains(test);
		}

		public bool IsIncluded(string test)
		{
			return mInclusionList.Contains(test);
		}
	}
}
