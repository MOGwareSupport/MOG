using System;

using MOG;
using MOG.TIME;

namespace MOG_ControlsLibrary
{
	public class MogUtils_StringVersion
	{
		static public string VersionToString(string version)
		{
			string text = "";

			if (version.Length > 0)
			{
				MOG_Time time = new MOG_Time(version);
				text = time.FormatString("");
			}

			return text;
		}
	}
}

