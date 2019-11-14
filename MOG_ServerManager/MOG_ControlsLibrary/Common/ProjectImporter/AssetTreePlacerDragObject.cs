using System;
using System.Collections;
using System.Windows.Forms;

namespace MOG_ServerManager
{
	/// <summary>
	/// Summary description for AssetTreePlacerDragObject.
	/// </summary>
	public class AssetTreePlacerDragObject
	{
		#region Member vars
		public MouseButtons button;
		public ArrayList nodeList;
		public DragObjectSource source;
		
		public AssetImportPlacer assetImportPlacer;
		#endregion
		#region Enums
		public enum DragObjectSource { DISKTREEVIEW, SELECTTREEVIEW, ASSETTREEVIEW };
		#endregion
		#region Constructor
		public AssetTreePlacerDragObject(DragObjectSource source)
		{
			this.source = source;
		}
		#endregion
		#region Public functions

		#endregion
	}
}




