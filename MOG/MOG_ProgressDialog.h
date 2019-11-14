#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::Threading;

#include "MOG_Report.h"
#include "MOG_Time.h"

#define TOTAL_FRAME_COUNT 10

namespace MOG
{
	/// <summary> 
	/// Summary for MOG_ProgressDialog
	///
	/// WARNING: If you change the name of this class, you will need to change the 
	///          'Resource File Name' property for the managed resource compiler tool 
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public __gc class MOG_ProgressDialog : public System::Windows::Forms::Form
	{
	public: 
		int InitializingInt;

		__property bool get_IsRunning();
		__property MOGPromptResult get_MOGPromptResult();
		void AbortThread();

		MOG_ProgressDialog(void);
		void SetupProgressGraphic();
		void SecondaryProgressBarSetup(int minimum, int maximum);
		void SecondaryProgressBarClose();
		void SetupProgressBar(int minimum, int maximum);
		void Show( String *title, String *message );
		void Show( String *title, String *message, int progressMax );
		void Show( String *title, String *message, int progressMin, int progressMax);

		MOG::PROMPT::MOGPromptResult UpdateProgressValue( String *message, String *fileMessage, int value );
		MOG::PROMPT::MOGPromptResult UpdateProgress( String *message, String *fileMessage );
		MOG::PROMPT::MOGPromptResult UpdateProgressStep( String *message, String *fileMessage );
		MOG::PROMPT::MOGPromptResult UpdateProgressStep( String *message, String *fileMessage, int step );
		MOG::PROMPT::MOGPromptResult UpdateProgressGraphic(String *message);
		
	// Make it so the unknowing programmer cannot access functionality of standard Forms that we cannot use...
	private:
		static int count;
		int mGraphicFrame;
		bool bCancelled;

		String *mOpeningStackTrace;
		Thread *mDisplayThread;
		Thread *mGraphicTimer;

		// non-windows variables for thread access
		// Accessing these form variables outside of the background thread is SLOW!!!
		String *mTitle;
		String *mProgressMessage;
		bool mFileMessageVisible;
		String *mProgressFileMessage;
		bool mProgressBarVisible;
		int mProgressBarPosition;
		int mProgressBarPositionMin;
		int mProgressBarPositionMax;
		bool mSecondaryProgressBar;
		bool mProgressBar2Visible;
		int mProgressBar2Position;
		int mProgressBar2PositionMin;
		int mProgressBar2PositionMax;
		bool mProgressPictureVisible;

		private: MOG_CoreControls::MOG_XpProgressBar *  ProgressBar2;
		private: MOG_CoreControls::MOG_XpProgressBar *  ProgressBar;
		private: System::Windows::Forms::PictureBox *  pictureBox1;
		private: System::Windows::Forms::PictureBox *  pictureBox2;
		private: System::Windows::Forms::PictureBox *  pictureBox3;
		private: System::Windows::Forms::PictureBox *  pictureBox4;
		private: System::Windows::Forms::PictureBox *  pictureBox5;
		private: System::Windows::Forms::PictureBox *  pictureBox6;
		private: System::Windows::Forms::PictureBox *  pictureBox7;
		private: System::Windows::Forms::PictureBox *  pictureBox8;
		private: System::Windows::Forms::PictureBox *  pictureBox9;
			 MOG::PROMPT::MOGPromptResult mMOGPromptResult;
		__delegate void Timer();
		
	protected:
		void ShowMethod();
		virtual void Show(){}
		virtual void ShowDialog(){}
		virtual void Show( IWin32Window *window ){}
		virtual void ShowDialog( IWin32Window *window ){}

		void SetupMessage(String *message);
		void SetupTitle(String *title);
		void UpdateFileMessage(String *message);
		void UpdateMessage(String *message);
		void UpdateProgressBar();
		void UpdateProgressBarValue(int value);

	private: System::Windows::Forms::Label *  ProgressFileLabel;
	private: System::Windows::Forms::Label *  ProgressMessageLabel;
	private: System::Windows::Forms::Button *  ProgressCancelButton;

	private: System::ComponentModel::IContainer *  components;

	private: System::Windows::Forms::PictureBox *  MOGProgressPictureBox;

	private: System::Void MOG_ProgressDialog_Closing(System::Object *  sender, System::ComponentModel::CancelEventArgs *  e);
	private: System::Void ProgressCancelButton_Click(System::Object *  sender, System::EventArgs *  e);

	protected: 
		void Dispose(Boolean disposing)
		{
			if (disposing && components)
			{
				components->Dispose();
			}

			try
			{
				// Change our mutex, so the thread can close out normally
				this->bCancelled = true;
				//Thread::Sleep(10);
			}
			catch( Exception *ex )
			{
				ex->ToString();
				//MOG_Report::ReportSilent( S"Error with Progress Dialog", ex->Message, ex->StackTrace, MOG_ALERT_LEVEL::ERROR );
			}
			__super::Dispose(disposing);
		}
	
	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>


		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			System::ComponentModel::ComponentResourceManager*  resources = (new System::ComponentModel::ComponentResourceManager(__typeof(MOG_ProgressDialog)));
			this->ProgressFileLabel = (new System::Windows::Forms::Label());
			this->ProgressMessageLabel = (new System::Windows::Forms::Label());
			this->ProgressCancelButton = (new System::Windows::Forms::Button());
			this->MOGProgressPictureBox = (new System::Windows::Forms::PictureBox());
			this->ProgressBar2 = (new MOG_CoreControls::MOG_XpProgressBar());
			this->ProgressBar = (new MOG_CoreControls::MOG_XpProgressBar());
			this->pictureBox1 = (new System::Windows::Forms::PictureBox());
			this->pictureBox2 = (new System::Windows::Forms::PictureBox());
			this->pictureBox3 = (new System::Windows::Forms::PictureBox());
			this->pictureBox4 = (new System::Windows::Forms::PictureBox());
			this->pictureBox5 = (new System::Windows::Forms::PictureBox());
			this->pictureBox6 = (new System::Windows::Forms::PictureBox());
			this->pictureBox7 = (new System::Windows::Forms::PictureBox());
			this->pictureBox8 = (new System::Windows::Forms::PictureBox());
			this->pictureBox9 = (new System::Windows::Forms::PictureBox());
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->MOGProgressPictureBox))->BeginInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox1))->BeginInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox2))->BeginInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox3))->BeginInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox4))->BeginInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox5))->BeginInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox6))->BeginInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox7))->BeginInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox8))->BeginInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox9))->BeginInit();
			this->SuspendLayout();
			// 
			// ProgressFileLabel
			// 
			this->ProgressFileLabel->Anchor = (System::Windows::Forms::AnchorStyles)((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right);
			this->ProgressFileLabel->BackColor = System::Drawing::SystemColors::Control;
			this->ProgressFileLabel->Location = System::Drawing::Point(8, 56);
			this->ProgressFileLabel->Name = S"ProgressFileLabel";
			this->ProgressFileLabel->Size = System::Drawing::Size(412, 56);
			this->ProgressFileLabel->TabIndex = 7;
			// 
			// ProgressMessageLabel
			// 
			this->ProgressMessageLabel->Anchor = (System::Windows::Forms::AnchorStyles)(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right);
			this->ProgressMessageLabel->Location = System::Drawing::Point(8, 8);
			this->ProgressMessageLabel->Name = S"ProgressMessageLabel";
			this->ProgressMessageLabel->Size = System::Drawing::Size(412, 48);
			this->ProgressMessageLabel->TabIndex = 6;
			this->ProgressMessageLabel->Text = S"Message";
			// 
			// ProgressCancelButton
			// 
			this->ProgressCancelButton->Anchor = System::Windows::Forms::AnchorStyles::Bottom;
			this->ProgressCancelButton->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->ProgressCancelButton->FlatStyle = System::Windows::Forms::FlatStyle::System;
			this->ProgressCancelButton->Location = System::Drawing::Point(180, 141);
			this->ProgressCancelButton->Name = S"ProgressCancelButton";
			this->ProgressCancelButton->Size = System::Drawing::Size(66, 23);
			this->ProgressCancelButton->TabIndex = 5;
			this->ProgressCancelButton->Text = S"Cancel";
			this->ProgressCancelButton->Click += new System::EventHandler(this, &MOG_ProgressDialog::ProgressCancelButton_Click);
			// 
			// MOGProgressPictureBox
			// 
			this->MOGProgressPictureBox->BackColor = System::Drawing::Color::Transparent;
			this->MOGProgressPictureBox->Image = (__try_cast<System::Drawing::Image*  >(resources->GetObject(S"MOGProgressPictureBox.Image")));
			this->MOGProgressPictureBox->Location = System::Drawing::Point(8, 24);
			this->MOGProgressPictureBox->Name = S"MOGProgressPictureBox";
			this->MOGProgressPictureBox->Size = System::Drawing::Size(64, 64);
			this->MOGProgressPictureBox->SizeMode = System::Windows::Forms::PictureBoxSizeMode::CenterImage;
			this->MOGProgressPictureBox->TabIndex = 9;
			this->MOGProgressPictureBox->TabStop = false;
			this->MOGProgressPictureBox->Visible = false;
			// 
			// ProgressBar2
			// 
			this->ProgressBar2->Anchor = (System::Windows::Forms::AnchorStyles)(System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left);
			this->ProgressBar2->ColorBackGround = System::Drawing::SystemColors::Control;
			this->ProgressBar2->ColorBarBorder = System::Drawing::Color::FromArgb((System::Int32)(System::Byte)20, (System::Int32)(System::Byte)114, 
				(System::Int32)(System::Byte)214);
			this->ProgressBar2->ColorBarCenter = System::Drawing::Color::AliceBlue;
			this->ProgressBar2->ColorText = System::Drawing::Color::Black;
			this->ProgressBar2->ForeColor = System::Drawing::SystemColors::Control;
			this->ProgressBar2->Location = System::Drawing::Point(2, 122);
			this->ProgressBar2->Name = S"ProgressBar2";
			this->ProgressBar2->Position = 0;
			this->ProgressBar2->PositionMax = 100;
			this->ProgressBar2->PositionMin = 0;
			this->ProgressBar2->Size = System::Drawing::Size(419, 12);
			this->ProgressBar2->SteepDistance = (System::Byte)0;
			this->ProgressBar2->SteepWidth = (System::Byte)1;
			this->ProgressBar2->TabIndex = 10;
			this->ProgressBar2->Visible = false;
			// 
			// ProgressBar
			// 
			this->ProgressBar->Anchor = (System::Windows::Forms::AnchorStyles)(System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left);
			this->ProgressBar->ColorBackGround = System::Drawing::Color::White;
			this->ProgressBar->ColorBarBorder = System::Drawing::Color::DodgerBlue;
			this->ProgressBar->ColorBarCenter = System::Drawing::Color::FromArgb((System::Int32)(System::Byte)17, (System::Int32)(System::Byte)96, 
				(System::Int32)(System::Byte)183);
			this->ProgressBar->ColorText = System::Drawing::Color::Black;
			this->ProgressBar->Location = System::Drawing::Point(2, 112);
			this->ProgressBar->Name = S"ProgressBar";
			this->ProgressBar->Position = 0;
			this->ProgressBar->PositionMax = 100;
			this->ProgressBar->PositionMin = 0;
			this->ProgressBar->Size = System::Drawing::Size(419, 16);
			this->ProgressBar->SteepDistance = (System::Byte)1;
			this->ProgressBar->TabIndex = 11;
			this->ProgressBar->Visible = false;
			// 
			// pictureBox1
			// 
			this->pictureBox1->BackColor = System::Drawing::Color::Transparent;
			this->pictureBox1->Image = (__try_cast<System::Drawing::Image*  >(resources->GetObject(S"pictureBox1.Image")));
			this->pictureBox1->Location = System::Drawing::Point(8, 24);
			this->pictureBox1->Name = S"pictureBox1";
			this->pictureBox1->Size = System::Drawing::Size(64, 64);
			this->pictureBox1->SizeMode = System::Windows::Forms::PictureBoxSizeMode::CenterImage;
			this->pictureBox1->TabIndex = 12;
			this->pictureBox1->TabStop = false;
			this->pictureBox1->Visible = false;
			// 
			// pictureBox2
			// 
			this->pictureBox2->BackColor = System::Drawing::Color::Transparent;
			this->pictureBox2->Image = (__try_cast<System::Drawing::Image*  >(resources->GetObject(S"pictureBox2.Image")));
			this->pictureBox2->Location = System::Drawing::Point(8, 24);
			this->pictureBox2->Name = S"pictureBox2";
			this->pictureBox2->Size = System::Drawing::Size(64, 64);
			this->pictureBox2->SizeMode = System::Windows::Forms::PictureBoxSizeMode::CenterImage;
			this->pictureBox2->TabIndex = 13;
			this->pictureBox2->TabStop = false;
			this->pictureBox2->Visible = false;
			// 
			// pictureBox3
			// 
			this->pictureBox3->BackColor = System::Drawing::Color::Transparent;
			this->pictureBox3->Image = (__try_cast<System::Drawing::Image*  >(resources->GetObject(S"pictureBox3.Image")));
			this->pictureBox3->Location = System::Drawing::Point(8, 24);
			this->pictureBox3->Name = S"pictureBox3";
			this->pictureBox3->Size = System::Drawing::Size(64, 64);
			this->pictureBox3->SizeMode = System::Windows::Forms::PictureBoxSizeMode::CenterImage;
			this->pictureBox3->TabIndex = 14;
			this->pictureBox3->TabStop = false;
			this->pictureBox3->Visible = false;
			// 
			// pictureBox4
			// 
			this->pictureBox4->BackColor = System::Drawing::Color::Transparent;
			this->pictureBox4->Image = (__try_cast<System::Drawing::Image*  >(resources->GetObject(S"pictureBox4.Image")));
			this->pictureBox4->Location = System::Drawing::Point(8, 24);
			this->pictureBox4->Name = S"pictureBox4";
			this->pictureBox4->Size = System::Drawing::Size(64, 64);
			this->pictureBox4->SizeMode = System::Windows::Forms::PictureBoxSizeMode::CenterImage;
			this->pictureBox4->TabIndex = 15;
			this->pictureBox4->TabStop = false;
			this->pictureBox4->Visible = false;
			// 
			// pictureBox5
			// 
			this->pictureBox5->BackColor = System::Drawing::Color::Transparent;
			this->pictureBox5->Image = (__try_cast<System::Drawing::Image*  >(resources->GetObject(S"pictureBox5.Image")));
			this->pictureBox5->Location = System::Drawing::Point(8, 24);
			this->pictureBox5->Name = S"pictureBox5";
			this->pictureBox5->Size = System::Drawing::Size(64, 64);
			this->pictureBox5->SizeMode = System::Windows::Forms::PictureBoxSizeMode::CenterImage;
			this->pictureBox5->TabIndex = 16;
			this->pictureBox5->TabStop = false;
			this->pictureBox5->Visible = false;
			// 
			// pictureBox6
			// 
			this->pictureBox6->BackColor = System::Drawing::Color::Transparent;
			this->pictureBox6->Image = (__try_cast<System::Drawing::Image*  >(resources->GetObject(S"pictureBox6.Image")));
			this->pictureBox6->Location = System::Drawing::Point(8, 24);
			this->pictureBox6->Name = S"pictureBox6";
			this->pictureBox6->Size = System::Drawing::Size(64, 64);
			this->pictureBox6->SizeMode = System::Windows::Forms::PictureBoxSizeMode::CenterImage;
			this->pictureBox6->TabIndex = 17;
			this->pictureBox6->TabStop = false;
			this->pictureBox6->Visible = false;
			// 
			// pictureBox7
			// 
			this->pictureBox7->BackColor = System::Drawing::Color::Transparent;
			this->pictureBox7->Image = (__try_cast<System::Drawing::Image*  >(resources->GetObject(S"pictureBox7.Image")));
			this->pictureBox7->Location = System::Drawing::Point(8, 24);
			this->pictureBox7->Name = S"pictureBox7";
			this->pictureBox7->Size = System::Drawing::Size(64, 64);
			this->pictureBox7->SizeMode = System::Windows::Forms::PictureBoxSizeMode::CenterImage;
			this->pictureBox7->TabIndex = 18;
			this->pictureBox7->TabStop = false;
			this->pictureBox7->Visible = false;
			// 
			// pictureBox8
			// 
			this->pictureBox8->BackColor = System::Drawing::Color::Transparent;
			this->pictureBox8->Image = (__try_cast<System::Drawing::Image*  >(resources->GetObject(S"pictureBox8.Image")));
			this->pictureBox8->Location = System::Drawing::Point(8, 24);
			this->pictureBox8->Name = S"pictureBox8";
			this->pictureBox8->Size = System::Drawing::Size(64, 64);
			this->pictureBox8->SizeMode = System::Windows::Forms::PictureBoxSizeMode::CenterImage;
			this->pictureBox8->TabIndex = 19;
			this->pictureBox8->TabStop = false;
			this->pictureBox8->Visible = false;
			// 
			// pictureBox9
			// 
			this->pictureBox9->BackColor = System::Drawing::Color::Transparent;
			this->pictureBox9->Image = (__try_cast<System::Drawing::Image*  >(resources->GetObject(S"pictureBox9.Image")));
			this->pictureBox9->Location = System::Drawing::Point(8, 24);
			this->pictureBox9->Name = S"pictureBox9";
			this->pictureBox9->Size = System::Drawing::Size(64, 64);
			this->pictureBox9->SizeMode = System::Windows::Forms::PictureBoxSizeMode::CenterImage;
			this->pictureBox9->TabIndex = 20;
			this->pictureBox9->TabStop = false;
			this->pictureBox9->Visible = false;
			// 
			// MOG_ProgressDialog
			// 
			this->AutoScaleBaseSize = System::Drawing::Size(5, 13);
			this->CancelButton = this->ProgressCancelButton;
			this->ClientSize = System::Drawing::Size(426, 167);
			this->ControlBox = false;
			this->Controls->Add(this->pictureBox9);
			this->Controls->Add(this->pictureBox8);
			this->Controls->Add(this->pictureBox7);
			this->Controls->Add(this->pictureBox6);
			this->Controls->Add(this->pictureBox5);
			this->Controls->Add(this->pictureBox4);
			this->Controls->Add(this->pictureBox3);
			this->Controls->Add(this->pictureBox2);
			this->Controls->Add(this->pictureBox1);
			this->Controls->Add(this->ProgressBar);
			this->Controls->Add(this->ProgressBar2);
			this->Controls->Add(this->MOGProgressPictureBox);
			this->Controls->Add(this->ProgressMessageLabel);
			this->Controls->Add(this->ProgressCancelButton);
			this->Controls->Add(this->ProgressFileLabel);
			this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedToolWindow;
			this->Icon = (__try_cast<System::Drawing::Icon*  >(resources->GetObject(S"$this.Icon")));
			this->KeyPreview = true;
			this->MaximizeBox = false;
			this->MinimizeBox = false;
			this->Name = S"MOG_ProgressDialog";
			this->StartPosition = System::Windows::Forms::FormStartPosition::CenterScreen;
			this->Text = S"Title:";
			this->Closing += new System::ComponentModel::CancelEventHandler(this, &MOG_ProgressDialog::MOG_ProgressDialog_Closing);
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->MOGProgressPictureBox))->EndInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox1))->EndInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox2))->EndInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox3))->EndInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox4))->EndInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox5))->EndInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox6))->EndInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox7))->EndInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox8))->EndInit();
			(__try_cast<System::ComponentModel::ISupportInitialize*  >(this->pictureBox9))->EndInit();
			this->ResumeLayout(false);

		}		





};
}