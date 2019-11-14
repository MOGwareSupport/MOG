using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using MOG.CONTROLLER.CONTROLLERSYSTEM;

using MOG;
using MOG.REPORT;
using MOG.PROMPT;
using MOG.ASSET_STATUS;

using Tst;

namespace MOG_Client.Client_Mog_Utilities.AssetOptions
{
		/// <summary>
		/// Location for all status information pertaining to ListViewItems
		/// in AssetManager.  
		/// 
		/// Returns MOGAssetStatus.StatusInfo objects
		/// based on string 'status' in method GetStatusInfo(string)
		/// </summary>
	public class MOGAssetStatus
	{
		private ImageList stateImageList;
		private TstDictionary mStateTypes	= new TstDictionary();	

 		public ImageList StateImageList
		{
			get {	return this.stateImageList; }
			set {	this.stateImageList = value; }
		}

        		/// <summary>
		/// Member class containing all information
		/// for each possible AssetManagerInbox statuses
		/// </summary>
		public class StatusInfo
		{
			private string text;
			private string eventSound;
			private int index;
			private MOG.ASSET_STATUS.StateIcon iconIndex;

			public StatusInfo(string text, string eventSound,
				MOG.ASSET_STATUS.StateIcon iconindex, int index)
			{
				this.text = text;
				this.eventSound = eventSound;
				this.iconIndex = iconindex;
				this.index = index;
			}

			#region Public properties
			/// These allow for us to check that
			/// private variables are being assigned 
			/// correctly.  NO CHECKING IMPLEMENTED,
			/// though.
			
			public string Text
			{
				get {	return this.text;	}
				set {	this.text = value;	}
			}
			public string EventSound
			{
				get {	return this.eventSound;		}
				set {	this.eventSound = value;	}
			}			
			public int Index
			{
				get {	return this.index;	}
				set	{	this.index = value;	}
			}
			public int IconIndex
			{
				get {	return (int)iconIndex;	}
				set	{	iconIndex = (MOG.ASSET_STATUS.StateIcon)value;	}
			}
			#endregion Public properties
		}

		/// <summary>
		/// Initializes MOGAssetStatus.statusInfos for use
		/// throughout MOG
		/// </summary>
		public MOGAssetStatus()
		{
			this.StateImageList = new ImageList();
			this.StateImageList.ColorDepth = ColorDepth.Depth24Bit;
			int counter = 0;

			// Load all our potential icons
			// WARNING!! Can only have 15 or less
			foreach(string filename in MOG_AssetStatus.GetStateIconFiles())
			{
				Bitmap icon = SetIcon(filename);
				
				// If we have a MOGMainForm, use it...
				if( icon != null)
				{
					stateImageList.Images.Add((Image)icon);
				}
			}

			/// Fill ArrayList, statusInfos, with 
			/// StatusInfo objects.
			foreach(MOG_AssetStatusInfo status in MOG_AssetStatus.GetAssetStatusInfos())
			{
				/// Create a new MOGAssetStatus, and 
				/// place it in the statusInfos ArrayList				
				StatusInfo current = null;

				current = new StatusInfo(	status.Status, 
											"eventSound",
											status.StateIconIndex,
											stateImageList.Images.Count - 1);
				
				this.mStateTypes.Add(status.Status.ToLower(), current);
				counter++;
			}
		}

		private System.Drawing.Bitmap SetIcon(string filename)
		{
			try
			{				
				return new System.Drawing.Bitmap(MOG_ControllerSystem.LocateTool("\\Images\\States", filename));
			}
			catch(Exception e1)
			{

				MOG_Report.ReportSilent("Initialize state icon", "Icon:\n" + "\\Images\\States\\" + filename +
										"\nCould not be loaded\n\n" + e1.Message, e1.StackTrace, MOG_ALERT_LEVEL.ERROR);

				return null;
			}
		}

		/// <summary>
		/// Gets the MOGAssetStatus for a given status name
		/// </summary>
		public StatusInfo GetStatusInfo(string status)
		{
			// Loops through statusInfos ArrayList in order
			// to find StatusInfo.Text that matches parameter 
			// 'status'
			try
			{
				TstDictionaryEntry node = this.mStateTypes.Find(status.ToLower());
				if (node != null && node.IsKey)
				{
					return (StatusInfo)node.Value;
				}
			}
			catch
			{
			}
 
			// If there was no match, return error
			return new StatusInfo("Error", string.Empty, 0, 0);
		} 
	} // end class MOGAssetStatus	
}
