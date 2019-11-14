using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;

namespace MOG_Client.Forms.ToolBoxForms
{
	/// <summary>
	/// Summary description for ToolBoxGroupBox.
	/// </summary>
	public class ToolBoxGroupBox : System.Windows.Forms.GroupBox
	{
		public const int Top_Padding = 12;
		public const int Bottom_Padding = 4;
		public const int LeftRight_Padding = 8;
		public const int RadioButtonText_Padding = 20;
		public const int HeightWidth_Increment = 2;

		public bool bHidingControls;

		// Delegate for our event...
		public delegate void SelectionChangedHandler( object sender, EventArgs e );
		/// <summary>
		/// Indicates that the user has selected a different Radio Button
		/// </summary>
		[Category("Action"), Description("Indicates that the user " + 
			"has changed which Radio Button is selected.")]
		public event SelectionChangedHandler SelectionChanged;
		/// <summary>
		/// Wrapper provided ONLY for the purpose of keeping compiler from giving a Warning
		/// </summary>
		private void selectionChangedWrapper(){ SelectionChanged( new object(), new EventArgs() );}

		private RadioButton mSelectedButton;
		/// <summary>
		/// Gets or sets the Selected RadioButton control inside this GroupBox
		/// </summary>
		public RadioButton SelectedButton
		{
			get
			{
				return this.mSelectedButton;
			}
			set
			{
				if (value != null)
				{
					this.mSelectedButton = value;
					this.mSelectedButton.Checked = true;
					if (SelectionChanged != null)
						SelectionChanged(value, new EventArgs());
				}
			}
		}

		/// <summary>
		/// Gets/Sets the RadioButtons inside this GroupBox
		/// </summary>
		[Description("Sets the RadioButtons to be displayed by this Form.")]
		public SortedList RadioButtons
		{
			get
			{
				SortedList radioButtons = new SortedList();
				// Foreach RadioButton in Controls...
				foreach( Control testControl in this.Controls )
				{
					if( testControl.GetType().AssemblyQualifiedName ==
						(new RadioButton()).GetType().AssemblyQualifiedName )
					{
						RadioButton radioButton = (RadioButton)testControl;
						string name = radioButton.Text;
						string tag = radioButton.Tag.ToString();
						radioButtons.Add( name, tag );
					}
				}
				return radioButtons;
			}
			set
			{
				// If we already have controls, clear them...
				if( this.Controls.Count > 0 )
					this.Controls.Clear();

				// Foreach DictionaryEntry, add a button
				foreach( DictionaryEntry button in value )
				{
					RadioButton newButton = new RadioButton();
					newButton.Text = (string)button.Key;
					newButton.Tag = (string)button.Value;
					this.Controls.Add( newButton );
				}
			}
		}

		public ToolBoxGroupBox()
			:base()	
		{		
			this.Layout += new LayoutEventHandler(ToolBoxGroupBox_Layout);
			this.ControlAdded += new ControlEventHandler(ToolBoxGroupBox_ControlAdded);
		}
		public ToolBoxGroupBox( SortedList radioButtons )
			:this()
		{
			this.RadioButtons = radioButtons;
		}

		/// <summary>
		/// Lays out child RadioButtons automatically in vertical columns 
		/// 
		/// Called every time this control is added to, removed from, resized.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToolBoxGroupBox_Layout(object sender, LayoutEventArgs e)
		{
			ToolBoxGroupBox_LayoutHelper();
		}

		/// <summary>
		/// Do a normal layout (no variable width or height and no recursion
		/// </summary>
		private void ToolBoxGroupBox_LayoutHelper()
		{
			ToolBoxGroupBox_LayoutHelper( false, false, this.Size );
		}

		/// <summary>
		/// Incrementally finds the 'perfect' size of this GroupBox, increasing size 
		/// (if returnDesiredWidth or -Height are true) reverse-logarithmically.  [i.e. It
		/// increases based on the number of non-visible controls.]
		/// 
		/// NOTE:  Any function that calls this recursively should suspend layout before calling
		/// </summary>
		/// <param name="returnDesiredHeight">Indicates that Height will be variable.  If true, suspend layout before calling this function.</param>
		/// <param name="returnDesiredWidth">Indicates that Width will be variable.  If true, suspend layout before calling this function.</param>
		/// <param name="newDesiredSize">The desired Size of this GroupBox.</param>
		/// <returns></returns>
		private Size ToolBoxGroupBox_LayoutHelper( bool returnDesiredHeight, bool returnDesiredWidth, Size newDesiredSize )
		{
			// Documents the lastButton we were doing the layout on
			RadioButton lastButton = null;
			int leftMargin = 0;
			int rightMargin = this.Width - LeftRight_Padding;
			int hiddenControlCount = 0;

			// Foreach radio button, do layout
			foreach( Control testControl in this.Controls )
			{
				// If (at design time) we have something other than a RadioButton, report, remove, and continue...
				if( !(testControl.GetType().AssemblyQualifiedName == (new RadioButton()).GetType().AssemblyQualifiedName) )
				{
					MessageBox.Show( this, "Invalid Windows control added.  Removing invalid control, "
						+ testControl.ToString());
					this.Controls.Remove( testControl );
					continue;
				}

				// Cast into our RadioButton
				RadioButton currentButton = (RadioButton)testControl;
				// Make sure our FlatStyle will look good on XP
				currentButton.FlatStyle = FlatStyle.System;
				// Set our width based on how much text we have
				currentButton.Width = (int)(currentButton.CreateGraphics()
					.MeasureString( currentButton.Text, currentButton.Font ).Width + 0.5f + RadioButtonText_Padding);
				// Make sure we have a minimum Width
				if( newDesiredSize.Width < currentButton.Width )
					newDesiredSize.Width = currentButton.Width;

				// If this is our first iteration...
				if( lastButton == null )
				{
					currentButton.Location = new Point( LeftRight_Padding, Top_Padding );
					leftMargin = currentButton.Location.X + currentButton.Width;
					this.bHidingControls = false;

					// Make sure our Height is a bare minimum
					if( newDesiredSize.Height < currentButton.Height )
						newDesiredSize.Height = currentButton.Height + Top_Padding + Bottom_Padding;
				}
					// Else, we are on the second control
				else
				{
					int x, y;

					// If our y is in bounds, assign it accordingly
					if( (lastButton.Location.Y + lastButton.Height + currentButton.Height) 
						< (newDesiredSize.Height - Bottom_Padding ) )
						y = lastButton.Location.Y + lastButton.Size.Height;
						// Else, y needs to be reset to the top of the GroupBox
					else
						y = Top_Padding;

					// Assign our x based on what we got for y:
					// If y has been reset, assign x as the next button over
					if( y == Top_Padding )
						x = leftMargin;
					else
						x = lastButton.Location.X;

					currentButton.Location = new Point( x, y );

					// If our button X coord and width exceed our leftMargin, document it
					if( (currentButton.Location.X + currentButton.Width)  > leftMargin )
						leftMargin = currentButton.Location.X + currentButton.Width;

					// If we are out of bounds, make anything we iterate through invisible
					if( leftMargin > newDesiredSize.Width && newDesiredSize.Width != currentButton.Width )
					{
						currentButton.Visible = false;
						bHidingControls = true;
						++hiddenControlCount;
					}
					else
					{
						currentButton.Visible = true;
						bHidingControls = false;
					}
				}

				// Document our last button for next iteration
				lastButton = currentButton;
			}

				// Now that we're done processing, return what we've found
			// If we want to return both desired variables, do so...
			//if( returnDesiredHeight && returnDesiredWidth )
			{
				if( bHidingControls )
				{
					newDesiredSize.Width += HeightWidth_Increment * hiddenControlCount;
					newDesiredSize.Height += HeightWidth_Increment * hiddenControlCount;

					// Return the size based on the value we just created for newDesiredSize
					return ToolBoxGroupBox_LayoutHelper( true, true, newDesiredSize );
				}
				else
					return newDesiredSize;
			}
				// Else, if we only want to return Height, do so...
//			else if( returnDesiredHeight  && !returnDesiredWidth )
//			{
//				if( bHidingControls )
//				{
//					newDesiredSize.Width = this.Width;
//					newDesiredSize.Height += HeightWidth_Increment * hiddenControlCount;
//					return ToolBoxGroupBox_LayoutHelper( returnDesiredHeight, returnDesiredWidth, newDesiredSize );
//				}
//				else
//					return new Size( this.Width, newDesiredSize.Height );
//			}
//				// Else, if we only want to return Width, do so...
//			else if( !returnDesiredHeight && returnDesiredWidth )
//			{
//				if( bHidingControls )
//				{
//					newDesiredSize.Width += HeightWidth_Increment * hiddenControlCount;
//					newDesiredSize.Height = this.Height;
//					return ToolBoxGroupBox_LayoutHelper( returnDesiredHeight, returnDesiredWidth, newDesiredSize );
//				}
//				else
//					return new Size( newDesiredSize.Width, this.Height );
//			}
//				// Else, set this.Size and return it...
//			else
//			{
//				this.Size = newDesiredSize;
//				return this.Size;
//			}
		}

		/*NOTE:  In order to save our RadioButtons out to a SortedList, we can
		 * have No Duplicate Names.  Any duplicate name are removed.  To help
		 * in GUI usability, a user of ToolBoxGroupBox should check input before
		 * it gets to this method, then report to end-user.
		 */
		/// <summary>
		/// Make sure that we have a RadioButton and document that it is now selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToolBoxGroupBox_ControlAdded(object sender, ControlEventArgs e)
		{
			if( e.Control.GetType().AssemblyQualifiedName == (new RadioButton()).GetType().AssemblyQualifiedName )
			{
				RadioButton button = (RadioButton)e.Control;
				// If we do not have a duplicate name...
				if( !IsDuplicateButtonName( button ) )
				{
					// Document this Control (and allow it to be added)
					this.mSelectedButton = button;
					button.MouseDown += new MouseEventHandler(RadioButton_MouseDown);
				}
				else
				{
					// If user chooses to update the existing control...
					if( MessageBox.Show( this, "You are trying to add a Radio Button that already exists.  \r\n\r\n"
						+ "Would you like to update it, instead?", "Radio Button, " + button.Text + ", already exists!",
						MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information ) == DialogResult.Yes )
					{
						// Remove the extra control first
						this.Controls.Remove( e.Control );
						RadioButton originalButton = GetButtonByText( button.Text );
                        originalButton.Text = button.Text;
						originalButton.Tag = button.Tag;
					}
					else
					{
						// Get rid of this Control
						this.Controls.Remove( e.Control );
					}
				}
			}
			else
				this.Controls.Remove( e.Control );
		}

		private void RadioButton_MouseDown(object sender, MouseEventArgs e)
		{
			this.SelectedButton = (RadioButton)sender;
		}

		/// <summary>
		/// Returns integer of Height needed to display all RadioButtons given `width`
		/// </summary>
		/// <param name="width">The integer width of the Parent Control OR 
		/// the maximum width this GroupBox can fill</param>
		/// <returns>Height needed to display all RadioButtons in this GroupBox</returns>
		public int CalculateHeightBasedOnWidth( int width )
		{
			// Do no Layout while we are changing this (StackOverflow if we do layout...)
			this.SuspendLayout();

//			// Document our current values before we continue
//			int oldWidth = this.Width;
//			int oldHeight = this.Height;
//
//			// Use a grid of 4 pixels to lay out our controls
//			int heightIncrement = HeightWidth_Increment;
//
//			// Change this.Height to zero
//			this.Height = 0;
//
//			// Set bHidingControls to true so we start our loop
//			this.bHidingControls = true;
//			// While we have controls that are hidden...
//			while( bHidingControls )
//			{
//				// Add a row and run Layout to see how it works
//                this.ToolBoxGroupBox_Layout( this, new LayoutEventArgs( new Control(), "" ) );
//
//				// If we don't have enough Height, increment it...
//				if( bHidingControls )
//					this.Height += heightIncrement;
//			}
//
//			// Save our current height value (now that we know all RadioButtons fit)
//			int newHeight = this.Height;
//
//			// Return our this.Width and Height values to what they were
//			this.Width = oldWidth;
//			this.Height = oldHeight;
			Size newSize = ToolBoxGroupBox_LayoutHelper( true, false, new Size( this.Width, 0 ) );
			// Resume our layout as normal
			this.ResumeLayout();

			// Return our new Height
//			return newHeight;
			return newSize.Height;
		}

		/// <summary>
		/// Test if a Control (after being added) is the duplicate of an existing Control
		/// </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		private bool IsDuplicateButtonName( RadioButton button )
		{
			// Count our hits, since the Control has already been added
			int hits = 0;

			// Test each control
			foreach( Control testControl in this.Controls )
			{
				if( testControl.Text == button.Text )
				{
					++hits;
					if( hits > 1 )
						return true;
				}
			}
			return false;
		}

		private RadioButton GetButtonByText( string text )
		{
			foreach( Control testControl in this.Controls )
			{
				if( testControl.Text == text )
				{
					return (RadioButton)testControl;
				}
			}
			return new RadioButton();
		}
	} // end class
}
