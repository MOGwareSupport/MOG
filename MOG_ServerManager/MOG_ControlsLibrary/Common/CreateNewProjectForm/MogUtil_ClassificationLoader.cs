using System;
using System.IO;
using System.Collections;

using MOG.INI;
using MOG.PROPERTIES;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for MogUtil_ClassificationLoader.
	/// </summary>
	public class MogUtil_ClassificationLoader
	{
		private string defaultClassesPath = "";
		private string projectName = "";

		public string ProjectName
		{
			get { return this.projectName; }
			set { this.projectName = value; }
		}

		public MogUtil_ClassificationLoader()
		{
			// Attempt to use the normal default template
			defaultClassesPath = MOG_ControllerSystem.LocateInstallItem("ProjectTemplates\\Default");
			if (defaultClassesPath.Length == 0)
			{
				// Fallback to using the library template
				defaultClassesPath = MOG_ControllerSystem.LocateInstallItem("ProjectTemplates\\Library");
			}
		}

		public MogUtil_ClassificationLoader(string projectName)
		{
			this.projectName = projectName;
		}

		public ArrayList GetClassFullNamesListFromFiles()
		{
			return GetClassFullNamesListFromFiles(this.defaultClassesPath);
		}

		public ArrayList GetClassFullNamesListFromFiles(string classesPath)
		{
			ArrayList nameList = new ArrayList();
			if (!Directory.Exists(classesPath))
				return nameList;

			foreach (string filename in Directory.GetFiles(classesPath, "{ProjectName}*.info"))
			{
				string className = Path.GetFileNameWithoutExtension( filename );
				className = filename.Replace("{ProjectName}", this.projectName);
				nameList.Add(className);
			}

			return nameList;
		}

		#region Get a class tree from a directory tree
		public ArrayList GetClassPropertiesListFromDirectoryTree(string path)
		{
			ArrayList propList = new ArrayList();
			if (!Directory.Exists(path))
				return propList;

			string className = Path.GetFileName(path);
			propList.Add(className);

			// recurse on subdirs
			foreach (string subdirPath in Directory.GetDirectories(path))
				propList.AddRange( GetClassPropertiesListFromDirectoryTree_Helper(subdirPath, className) );

			return propList;
		}

		public ArrayList GetClassPropertiesListFromDirectoryTree_Helper(string path, string baseClassName)
		{
			ArrayList propList = new ArrayList();
			if (!Directory.Exists(path))
				return propList;

			string className = baseClassName + "~" + Path.GetFileName(path);
			className = className.Trim("~".ToCharArray());
			propList.Add(className);

			// recurse on subdirs
			foreach (string subdirPath in Directory.GetDirectories(path))
				propList.AddRange( GetClassPropertiesListFromDirectoryTree_Helper(subdirPath, className) );

			return propList;
		}
		#endregion

		public ArrayList GetClassPropertiesListFromFiles()
		{
			return GetClassPropertiesListFromFiles(this.defaultClassesPath);
		}

		public ArrayList GetClassPropertiesListFromFiles(string classesPath)
		{
			ArrayList propList = new ArrayList();
			if (!Directory.Exists(classesPath))
				return propList;

			foreach (string filename in Directory.GetFiles(classesPath, "{ProjectName}*.info"))
			{
				MOG_PropertiesIni classIni = new MOG_PropertiesIni(filename);
				if (classIni != null)
				{
					MOG_Properties props = MOG_Properties.OpenFileProperties(classIni);
					string pathFilename = Path.GetFileNameWithoutExtension(filename);
					string className = pathFilename.Replace("{ProjectName}", this.projectName);
					props.Classification = className;	// extract just class name from filename
					propList.Add(props);
				}
			}

			return propList;
		}
	}
}
