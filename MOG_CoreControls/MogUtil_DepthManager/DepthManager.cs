using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

using System.Diagnostics;

namespace MOG_CoreControls.MogUtil_DepthManager
{
	public enum FormDepth {	BACK, MID, TOP };
			
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class DepthManager
	{
		/// <summary>
		/// Class holder for a form object
		/// </summary>
		class FormHolder
		{
			public int mDepth;				// Current Z Depth of this form
			public FormDepth mDepthZone;	// Current zone for this depth (Back, Mid, Top)
			public Form mForm;				// Form pointer
			public bool mModal;				// Are we modal?
		}
		
		class FormMap
		{
			public Form form;
			public FormHolder holder;

			public FormMap(Form form, FormHolder holder)
			{
				this.form = form;
				this.holder = holder;
			}
		}

		private delegate void DisableParentDelegate(Form parent);

		static private SortedList[] mForms = {new SortedList(), new SortedList(), new SortedList()};

		static int mCurrentBack = 0;
		static int mCurrentMid = 5000;
		static int mCurrentTop = 20000;

		static private FormHolder mCurrentTopmost = null;
		static private Mutex mutex = new Mutex();
		static private ArrayList mFormMapList = new ArrayList();

		static private void AddHolderMap(Form form, FormHolder holder)
		{
			mFormMapList.Add(new FormMap(form, holder));
		}
			
		static private FormHolder RemoveHolderMap(Form form)
		{
			FormHolder holder = null;

			//Go through all the map entries in the list
			foreach (FormMap map in mFormMapList)
			{
				if (map.form == form)
				{
					//This is it, get the holder so we can return it, and then remove the map entry
					holder = map.holder;
					mFormMapList.Remove(map);
					break;
				}
			}

			return holder;
		}

		/// <summary>
		/// Create a formHolder object with the correct depth and event handlers
		/// </summary>
		/// <param name="form"></param>
		/// <param name="depth"></param>
		/// <returns></returns>
		static private FormHolder CreateFormHolder(Form form, FormDepth depth, bool isModal)
		{
			// Create our handle
			FormHolder holder = new FormHolder();

			// Assign it our vars
			holder.mForm = form;
			holder.mDepthZone = depth;
			holder.mModal = isModal;

			// Determine its depth base on its zone
			switch(depth)
			{
			case FormDepth.BACK:
				holder.mDepth = mCurrentBack++;
				break;
			case FormDepth.MID:
				holder.mDepth = mCurrentMid++;
				break;
			case FormDepth.TOP:
				holder.mDepth = mCurrentTop++;
				break;
			}

			// Mark down in our list that this holder goes with this form
			AddHolderMap(form, holder);

			// Override the forms activated event handle
			form.Activated += new EventHandler(form_Activated);
			form.Closed += new EventHandler(form_Closed);

			// Return the new object
			return holder;
		}

		/// <summary>
		/// Bring the highest form to the top of our window
		/// </summary>
		static private void BringNextToTop()
		{
			//Only go in there if nobody else is doing it already
			//The first person in here will go through the loop and activate all the windows in the order they should be.
			//Anytime we get here and the mutex is locked, this is being called from a form that is not
			//the original one being activated so we don't have to worry about reactivating all the windows again
			if (mutex.WaitOne(0, false))
			{
				try
				{
					//if (mCurrentTopmost == null || GetTopmostDepth() > mCurrentTopmost.mDepth)
					{
						mCurrentTopmost = GetTopmostFormHolder();
#if true
						//Don't keep activating the same form over and over
						if (mCurrentTopmost != null)
						{
							//Debug.WriteLine("+++Activate: " + mCurrentTopmost.mForm.Name);
							
							mCurrentTopmost.mForm.Enabled = true;
							mCurrentTopmost.mForm.Activate();
						}
#else
						for (int i = 0; i < mForms.Length; i++)
						{
							for (int j = 0; j < mForms[i].Count; j++)
							{
								// I assume the latest node in our sorted list is the highest because it is a sorted (by depth) list
								FormHolder holder = mForms[i].GetByIndex(j) as FormHolder;
								
								//Debug.WriteLine("+++Activate: " + holder.mForm.Name);
								
								if (holder == GetTopmostFormHolder())
								{
									holder.mForm.Enabled = true;
								}

								holder.mForm.Activate();
								holder.mForm.BringToFront();
							}
						}
#endif
					}
				}
				catch
				{
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// Bring the highest form to the top of our window
		/// </summary>
		static private Form GetTopmostForm(FormDepth zone)
		{
			// I assume the latest node in our sorted list is the highest because it is a sorted(by depth) list
			for (int z = (int)zone; z >= 0; z--)
			{
				if (mForms[z].Count > 0)
				{
					// I assume the latest node in our sorted list is the highest because it is a sorted(by depth) list
					FormHolder holder = mForms[z].GetByIndex(mForms[z].Count-1) as FormHolder;
					return holder.mForm;
				}
			}

			return null;
		}

		static private FormHolder GetTopmostFormHolder()
		{
			//Go backwards through the list of active forms to find the topmost one
			for (int i = mForms.Length-1; i >= 0; i--)
			{
				if (mForms[i].Count > 0)
				{
					// I assume the latest node in our sorted list is the highest because it is a sorted (by depth) list
					FormHolder holder = mForms[i].GetByIndex(mForms[i].Count-1) as FormHolder;
					return holder;
				}
			}

			return null;
		}

		static private Form GetTopmostForm()
		{
			FormHolder holder = GetTopmostFormHolder();

			if (holder != null)
			{
				return holder.mForm;
			}

			return null;
		}

		static private int GetTopmostDepth()
		{
			FormHolder holder = GetTopmostFormHolder();
			
			if (holder != null)
			{
				return holder.mDepth;
			}

			return -1;
		}

		/// <summary>
		/// Manage the adding of forms to our internal arrays
		/// </summary>
		/// <param name="holder"></param>
		static private void AddForm(FormHolder holder)
		{
			if (holder != null)
			{
				mForms[(int)holder.mDepthZone].Add(holder.mDepth, holder);
			}
		}

		/// <summary>
		/// Manage the removing of forms to our internal arrays
		/// </summary>
		/// <param name="holder"></param>
		static private void RemoveForm(FormHolder holder)
		{
			if (holder != null)
			{
				mForms[(int)holder.mDepthZone].Remove(holder.mDepth);
			}
		}
		
		/// <summary>
		/// Assign the initial parent form
		/// </summary>
		/// <param name="form"></param>
		/// <param name="depth"></param>
		static public void InitParent(Form form, FormDepth depth)
		{
			FormHolder holder = CreateFormHolder(form, depth, false);			
			AddForm(holder);
		}

		/// <summary>
		/// Main Show routine
		/// </summary>
		static private DialogResult Show(Form parent, Form child, FormDepth depth, bool isModal, bool isVirtual)
		{
            DialogResult result = DialogResult.None;
			FormHolder holder = CreateFormHolder(child, depth, isModal);		
			AddForm(holder);

			try
			{
				holder.mForm.StartPosition = FormStartPosition.CenterParent;
				
				if (isVirtual)
				{
					// this is a virtually modal form
					if (parent != null)
					{						
						parent.Invoke(new DisableParentDelegate(DisableParent), new object[]{parent});
					}
					
					if (!isModal)
					{
						holder.mForm.Show();
					}
				}
				else
				{
					if (isModal)
					{
						//this is a pure modal form
						result = holder.mForm.ShowDialog(parent);
						Close(holder.mForm);
					}
					else
					{
						// this is not modal nor is it virtually modal, so just show it modeless
						holder.mForm.Show();					
					}
				}
			}
			// Ignore any ThreadAbortExceptions (but keep this code here so we can observe them)
			catch( ThreadAbortException ex)
			{
				Debug.WriteLine( ex.ToString());
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				// If we've caught, re-enable our parent (just in case)
				if (parent != null)
				{
					//Debug.WriteLine(parent.Name + " - (Exception)BackForm ReEnabled", "Show");
					parent.Enabled = true;
				}
			}

            return result;
		}

		/// <summary>
		/// Allow us to disable our Parent Asynchronously so that we can keep processing...
		/// </summary>
		/// <param name="result"></param>
		private static void DisableParent(Form parent)
		{
			if (parent != null)
			{
				//Debug.WriteLine("DisableParent: " + parent.Name);
				parent.Enabled = false;
			}
		}

		/// <summary>
		/// Show functions
		/// </summary>
		/// <param name="form"></param>
		/// <param name="depth"></param>
		static public void Show(Form form, FormDepth depth)
		{
			Show(null, form, depth, false, false);
		}
		static public DialogResult ShowModal(Form parent, Form form, FormDepth depth)
		{
			return Show(parent, form, depth, true, false);
		}
		static public void ShowVirtualModal(Form parent, Form form, FormDepth depth)
		{
            Show(parent, form, depth, false, true);
		}
		static public void VirtualModal(Form parent, Form form, FormDepth depth)
		{
			Show(parent, form, depth, true, true);
		}

		/// <summary>
		/// Close function
		/// </summary>
		/// <param name="form"></param>
		static private void Close(Form form)
		{
			try
			{
				// Get our holder hande from the closing form
				FormHolder holder = RemoveHolderMap(form);

				// Remove this from our sorted set
				RemoveForm(holder);

				if (holder == mCurrentTopmost)
				{
					//This form is going to be closed, so it cannot be the topmost anymore
					mCurrentTopmost = null;
				}

				// Find out who is next in line to be enabled
				BringNextToTop();
			}
			catch
			{
			}
		}

		/// <summary>
		/// Catch all form activated events so that we can make sure to keep our order
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void form_Activated(object sender, EventArgs e)
		{
			Form form = sender as Form;

			//Debug.WriteLine("form_Activated: " + form.Name);
			BringNextToTop();
		}

		private static void form_Closed(object sender, EventArgs e)
		{
			Form form = sender as Form;

			//Debug.WriteLine("form_Closed: " + form.Name);
			Close(form);
		}
	}
}
