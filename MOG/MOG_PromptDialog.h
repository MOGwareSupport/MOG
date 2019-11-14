#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::Runtime::InteropServices;
using namespace System::Threading;

#include "MOG_PromptMoreDialog.h"
#include "MOG_Report.h"
#include "MOG_Prompt.h"
#include "MOG_Progress.h"

namespace MOG
{
	public __value enum MessageBeepType
	{
		Default = -1,
		Ok = 0x00000000,
		Error = 0x00000010,
		Question = 0x00000020,
		Warning = 0x00000030,
		Information = 0x00000040,
	};

	private __value enum MessageIcons
	{
		IMAGE_ALERT,
		IMAGE_ERROR,
		IMAGE_EXCEPTION,
	};

	/// <summary> 
	/// Summary for MOG_PromptDialog
	///
	/// WARNING: If you change the name of this class, you will need to change the 
	///          'Resource File Name' property for the managed resource compiler tool 
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public __gc class MOG_PromptDialog : public System::Windows::Forms::Form
	{
		public:
			__property MOGPromptResult get_MOGPromptResult();
			MOG_PromptDialog(void);

			[DllImport("user32.dll", SetLastError=true)]
			static int MessageBeep(MessageBeepType type);

			void CheckIfShowing()
			{	
				// If there is no Active Form, display this form (this only should occur when Mog closes with errors)
				if(!this->ActiveForm)
				{
					this->Show();
				}
			}

		public protected:
			void SetupButtons(MOGPromptButtons buttons);
			void SetupImage(MOG_ALERT_LEVEL alertLevel, bool playSound);
			void SetupStackTrace(String *stacktrace);
			void SetupMessage(String *message);
			void SetupTitle(String *title);

		protected: 
			void Dispose(Boolean disposing);

		private:
 			String *mStackTrace;
			MOG::PROMPT::MOGPromptResult mMOGPromptResult;
			
			void LoadMessageFromListView(ListViewItem *selectedItem);
			void CreateButtons(String *buttons);
			void LoadAlertIcon(int index);
			
			MOG::PROMPT::MOGPromptResult StringToMOGPromptResult(String *dialogResult);
			
		private: System::Void PomptMoreButton_Click(System::Object *  sender, System::EventArgs *  e);
		private: System::Void PromptOKButton_Click(System::Object *  sender, System::EventArgs *  e);
		private: System::Void PromptListView_Click(System::Object *  sender, System::EventArgs *  e);
		private: System::Void PromptEmailButton_Click(System::Object *  sender, System::EventArgs *  e);
		private: System::Void PromptCopyButton_Click(System::Object *  sender, System::EventArgs *  e);
		private: System::Void PromptListView_DoubleClick(System::Object *  sender, System::EventArgs *  e);
		private: System::Void MOG_PromptDialog_Closed(System::Object *  sender, System::EventArgs *  e);
		
		private: System::Windows::Forms::Button *  PromptButton1;
		private: System::Windows::Forms::Button *  PromptButton2;
		private: System::Windows::Forms::Button *  PromptButton3;
		private: System::Windows::Forms::PictureBox *  PromptPictureBox;
		private: System::Windows::Forms::ToolTip *  PromptToolTip;
		private: System::Windows::Forms::ImageList *  PromptIconsImageList;
		private: System::Windows::Forms::Panel *  PromptDividerPanel;
		public: System::Windows::Forms::ListView *  PromptListView;
		private: System::Windows::Forms::Panel *  PromptDividerPanel2;
		private: System::Windows::Forms::ColumnHeader *  PromptListViewCountColumn;
		private: System::Windows::Forms::ColumnHeader *  PromptListViewTypeColumn;
		private: System::Windows::Forms::ColumnHeader *  PromptListViewTitleColumn;
		private: System::Windows::Forms::Panel *  PromptListViewPanel;
		private: System::Windows::Forms::ColumnHeader *  PromptListViewMessageColumn;
		private: System::Windows::Forms::Panel *  PromptMainPanel;
		private: System::Windows::Forms::PictureBox *  PromptPictureBoxAlert;
		private: System::Windows::Forms::PictureBox *  PromptPictureBoxError;
		private: System::Windows::Forms::PictureBox *  PromptPictureBoxCritical;
		private: System::Windows::Forms::ImageList *  SmallPromptIconsImageList;
		private: System::Windows::Forms::Button *  PromptEmailButton;
		private: System::Windows::Forms::Button *  PromptCopyButton;
		private: System::Windows::Forms::Button *  PromptOKButton;
		private: System::Windows::Forms::RichTextBox *  PromptMessageLabel;
		private: System::Windows::Forms::Button *  PomptMoreButton;
		private: System::ComponentModel::IContainer *  components;
		/// <summary>
		/// Required designer variable.
		/// </summary>

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = new System::ComponentModel::Container();
			System::Resources::ResourceManager *  resources = new System::Resources::ResourceManager(__typeof(MOG::MOG_PromptDialog));
			this->PromptOKButton = new System::Windows::Forms::Button();
			this->PromptMessageLabel = new System::Windows::Forms::RichTextBox();
			this->PomptMoreButton = new System::Windows::Forms::Button();
			this->PromptCopyButton = new System::Windows::Forms::Button();
			this->PromptButton1 = new System::Windows::Forms::Button();
			this->PromptButton2 = new System::Windows::Forms::Button();
			this->PromptButton3 = new System::Windows::Forms::Button();
			this->PromptDividerPanel = new System::Windows::Forms::Panel();
			this->PromptPictureBox = new System::Windows::Forms::PictureBox();
			this->PromptToolTip = new System::Windows::Forms::ToolTip(this->components);
			this->PromptEmailButton = new System::Windows::Forms::Button();
			this->PromptIconsImageList = new System::Windows::Forms::ImageList(this->components);
			this->PromptListView = new System::Windows::Forms::ListView();
			this->PromptListViewCountColumn = new System::Windows::Forms::ColumnHeader();
			this->PromptListViewTypeColumn = new System::Windows::Forms::ColumnHeader();
			this->PromptListViewTitleColumn = new System::Windows::Forms::ColumnHeader();
			this->PromptListViewMessageColumn = new System::Windows::Forms::ColumnHeader();
			this->SmallPromptIconsImageList = new System::Windows::Forms::ImageList(this->components);
			this->PromptDividerPanel2 = new System::Windows::Forms::Panel();
			this->PromptListViewPanel = new System::Windows::Forms::Panel();
			this->PromptMainPanel = new System::Windows::Forms::Panel();
			this->PromptPictureBoxAlert = new System::Windows::Forms::PictureBox();
			this->PromptPictureBoxError = new System::Windows::Forms::PictureBox();
			this->PromptPictureBoxCritical = new System::Windows::Forms::PictureBox();
			this->PromptListViewPanel->SuspendLayout();
			this->PromptMainPanel->SuspendLayout();
			this->SuspendLayout();
			// 
			// PromptOKButton
			// 
			this->PromptOKButton->Anchor = (System::Windows::Forms::AnchorStyles)(System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right);
			this->PromptOKButton->DialogResult = System::Windows::Forms::DialogResult::OK;
			this->PromptOKButton->FlatStyle = System::Windows::Forms::FlatStyle::System;
			this->PromptOKButton->Location = System::Drawing::Point(367, 98);
			this->PromptOKButton->Name = S"PromptOKButton";
			this->PromptOKButton->Size = System::Drawing::Size(85, 23);
			this->PromptOKButton->TabIndex = 0;
			this->PromptOKButton->Text = S"Ok";
			this->PromptOKButton->Click += new System::EventHandler(this, &MOG::MOG_PromptDialog::PromptOKButton_Click);
			// 
			// PromptMessageLabel
			// 
			this->PromptMessageLabel->Anchor = (System::Windows::Forms::AnchorStyles)((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right);
			this->PromptMessageLabel->Location = System::Drawing::Point(72, 0);
			this->PromptMessageLabel->Name = S"PromptMessageLabel";
			this->PromptMessageLabel->Size = System::Drawing::Size(376, 85);
			this->PromptMessageLabel->TabIndex = 10;
			this->PromptMessageLabel->BackColor = System::Drawing::SystemColors::Control;			
			this->PromptMessageLabel->BorderStyle = System::Windows::Forms::BorderStyle::None;
			// 
			// PomptMoreButton
			// 
			this->PomptMoreButton->FlatStyle = System::Windows::Forms::FlatStyle::Flat;
			this->PomptMoreButton->ForeColor = System::Drawing::SystemColors::Control;
			this->PomptMoreButton->Image = (__try_cast<System::Drawing::Image *  >(resources->GetObject(S"PomptMoreButton.Image")));
			this->PomptMoreButton->Location = System::Drawing::Point(8, 98);
			this->PomptMoreButton->Name = S"PomptMoreButton";
			this->PomptMoreButton->Size = System::Drawing::Size(24, 23);
			this->PomptMoreButton->TabIndex = 2;
			this->PromptToolTip->SetToolTip(this->PomptMoreButton, S"Additional information");
			this->PomptMoreButton->Visible = false;
			this->PomptMoreButton->Click += new System::EventHandler(this, &MOG::MOG_PromptDialog::PomptMoreButton_Click);
			// 
			// PromptCopyButton
			// 
			this->PromptCopyButton->FlatStyle = System::Windows::Forms::FlatStyle::Flat;
			this->PromptCopyButton->ForeColor = System::Drawing::SystemColors::Control;
			this->PromptCopyButton->Image = (__try_cast<System::Drawing::Image *  >(resources->GetObject(S"PromptCopyButton.Image")));
			this->PromptCopyButton->Location = System::Drawing::Point(32, 98);
			this->PromptCopyButton->Name = S"PromptCopyButton";
			this->PromptCopyButton->Size = System::Drawing::Size(24, 23);
			this->PromptCopyButton->TabIndex = 3;
			this->PromptToolTip->SetToolTip(this->PromptCopyButton, S"Copy error to clipboard");
			this->PromptCopyButton->Click += new System::EventHandler(this, &MOG::MOG_PromptDialog::PromptCopyButton_Click);
			// 
			// PromptButton1
			// 
			this->PromptButton1->Anchor = (System::Windows::Forms::AnchorStyles)(System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right);
			this->PromptButton1->DialogResult = System::Windows::Forms::DialogResult::OK;
			this->PromptButton1->FlatStyle = System::Windows::Forms::FlatStyle::System;
			this->PromptButton1->Location = System::Drawing::Point(274, 98);
			this->PromptButton1->Name = S"PromptButton1";
			this->PromptButton1->Size = System::Drawing::Size(85, 23);
			this->PromptButton1->TabIndex = 4;
			this->PromptButton1->Text = S"Buttons";
			this->PromptButton1->Visible = false;
			this->PromptButton1->Click += new System::EventHandler(this, &MOG::MOG_PromptDialog::PromptOKButton_Click);
			// 
			// PromptButton2
			// 
			this->PromptButton2->Anchor = (System::Windows::Forms::AnchorStyles)(System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right);
			this->PromptButton2->DialogResult = System::Windows::Forms::DialogResult::OK;
			this->PromptButton2->FlatStyle = System::Windows::Forms::FlatStyle::System;
			this->PromptButton2->Location = System::Drawing::Point(181, 98);
			this->PromptButton2->Name = S"PromptButton2";
			this->PromptButton2->Size = System::Drawing::Size(85, 23);
			this->PromptButton2->TabIndex = 5;
			this->PromptButton2->Text = S"Buttons";
			this->PromptButton2->Visible = false;
			this->PromptButton2->Click += new System::EventHandler(this, &MOG::MOG_PromptDialog::PromptOKButton_Click);
			// 
			// PromptButton3
			// 
			this->PromptButton3->Anchor = (System::Windows::Forms::AnchorStyles)(System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right);
			this->PromptButton3->DialogResult = System::Windows::Forms::DialogResult::OK;
			this->PromptButton3->FlatStyle = System::Windows::Forms::FlatStyle::System;
			this->PromptButton3->Location = System::Drawing::Point(88, 98);
			this->PromptButton3->Name = S"PromptButton3";
			this->PromptButton3->Size = System::Drawing::Size(85, 23);
			this->PromptButton3->TabIndex = 6;
			this->PromptButton3->Text = S"Buttons";
			this->PromptButton3->Visible = false;
			this->PromptButton3->Click += new System::EventHandler(this, &MOG::MOG_PromptDialog::PromptOKButton_Click);
			// 
			// PromptDividerPanel
			// 
			this->PromptDividerPanel->Anchor = (System::Windows::Forms::AnchorStyles)((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right);
			this->PromptDividerPanel->BorderStyle = System::Windows::Forms::BorderStyle::FixedSingle;
			this->PromptDividerPanel->ForeColor = System::Drawing::SystemColors::ControlText;
			this->PromptDividerPanel->Location = System::Drawing::Point(0, 89);
			this->PromptDividerPanel->Name = S"PromptDividerPanel";
			this->PromptDividerPanel->Size = System::Drawing::Size(456, 3);
			this->PromptDividerPanel->TabIndex = 11;
			// 
			// PromptPictureBox
			// 
			this->PromptPictureBox->Image = (__try_cast<System::Drawing::Image *  >(resources->GetObject(S"PromptPictureBox.Image")));
			this->PromptPictureBox->Location = System::Drawing::Point(8, 16);
			this->PromptPictureBox->Name = S"PromptPictureBox";
			this->PromptPictureBox->Size = System::Drawing::Size(48, 48);
			this->PromptPictureBox->TabIndex = 12;
			this->PromptPictureBox->TabStop = false;
			// 
			// PromptEmailButton
			// 
			this->PromptEmailButton->Anchor = (System::Windows::Forms::AnchorStyles)(System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left);
			this->PromptEmailButton->FlatStyle = System::Windows::Forms::FlatStyle::Flat;
			this->PromptEmailButton->ForeColor = System::Drawing::SystemColors::Control;
			this->PromptEmailButton->Image = (__try_cast<System::Drawing::Image *  >(resources->GetObject(S"PromptEmailButton.Image")));
			this->PromptEmailButton->Location = System::Drawing::Point(59, 99);
			this->PromptEmailButton->Name = S"PromptEmailButton";
			this->PromptEmailButton->Size = System::Drawing::Size(24, 23);
			this->PromptEmailButton->TabIndex = 7;
			this->PromptEmailButton->TextAlign = System::Drawing::ContentAlignment::MiddleRight;
			this->PromptToolTip->SetToolTip(this->PromptEmailButton, S"Send report to MOGware");
			this->PromptEmailButton->Click += new System::EventHandler(this, &MOG::MOG_PromptDialog::PromptEmailButton_Click);
			// 
			// PromptIconsImageList
			// 
			this->PromptIconsImageList->ColorDepth = System::Windows::Forms::ColorDepth::Depth32Bit;
			this->PromptIconsImageList->ImageSize = System::Drawing::Size(48, 48);
			this->PromptIconsImageList->ImageStream = (__try_cast<System::Windows::Forms::ImageListStreamer *  >(resources->GetObject(S"PromptIconsImageList.ImageStream")));
			this->PromptIconsImageList->TransparentColor = System::Drawing::Color::Transparent;
			// 
			// PromptListView
			// 
			this->PromptListView->BackColor = System::Drawing::SystemColors::Window;
			System::Windows::Forms::ColumnHeader* __mcTemp__1[] = new System::Windows::Forms::ColumnHeader*[4];
			__mcTemp__1[0] = this->PromptListViewCountColumn;
			__mcTemp__1[1] = this->PromptListViewTypeColumn;
			__mcTemp__1[2] = this->PromptListViewTitleColumn;
			__mcTemp__1[3] = this->PromptListViewMessageColumn;
			this->PromptListView->Columns->AddRange(__mcTemp__1);
			this->PromptListView->Dock = System::Windows::Forms::DockStyle::Fill;
			this->PromptListView->FullRowSelect = true;
			this->PromptListView->GridLines = true;
			this->PromptListView->Location = System::Drawing::Point(0, 0);
			this->PromptListView->MultiSelect = false;
			this->PromptListView->Name = S"PromptListView";
			this->PromptListView->Size = System::Drawing::Size(456, 0);
			this->PromptListView->SmallImageList = this->SmallPromptIconsImageList;
			this->PromptListView->TabIndex = 13;
			this->PromptListView->View = System::Windows::Forms::View::Details;
			this->PromptListView->Visible = false;
			this->PromptListView->Click += new System::EventHandler(this, &MOG::MOG_PromptDialog::PromptListView_Click);
			this->PromptListView->DoubleClick += new System::EventHandler(this, &MOG::MOG_PromptDialog::PromptListView_DoubleClick);
			this->PromptListView->SelectedIndexChanged += new System::EventHandler(this, &MOG::MOG_PromptDialog::PromptListView_Click);
			// 
			// PromptListViewCountColumn
			// 
			this->PromptListViewCountColumn->Text = S"Count";
			this->PromptListViewCountColumn->Width = 45;
			// 
			// PromptListViewTypeColumn
			// 
			this->PromptListViewTypeColumn->Text = S"Type";
			// 
			// PromptListViewTitleColumn
			// 
			this->PromptListViewTitleColumn->Text = S"Title";
			this->PromptListViewTitleColumn->Width = 86;
			// 
			// PromptListViewMessageColumn
			// 
			this->PromptListViewMessageColumn->Text = S"Message";
			this->PromptListViewMessageColumn->Width = 462;
			// 
			// SmallPromptIconsImageList
			// 
			this->SmallPromptIconsImageList->ImageSize = System::Drawing::Size(16, 16);
			this->SmallPromptIconsImageList->ImageStream = (__try_cast<System::Windows::Forms::ImageListStreamer *  >(resources->GetObject(S"SmallPromptIconsImageList.ImageStream")));
			this->SmallPromptIconsImageList->TransparentColor = System::Drawing::Color::Transparent;
			// 
			// PromptDividerPanel2
			// 
			this->PromptDividerPanel2->BorderStyle = System::Windows::Forms::BorderStyle::FixedSingle;
			this->PromptDividerPanel2->Dock = System::Windows::Forms::DockStyle::Top;
			this->PromptDividerPanel2->Location = System::Drawing::Point(0, 0);
			this->PromptDividerPanel2->Name = S"PromptDividerPanel2";
			this->PromptDividerPanel2->Size = System::Drawing::Size(456, 3);
			this->PromptDividerPanel2->TabIndex = 14;
			// 
			// PromptListViewPanel
			// 
			this->PromptListViewPanel->Controls->Add(this->PromptDividerPanel2);
			this->PromptListViewPanel->Controls->Add(this->PromptListView);
			this->PromptListViewPanel->Dock = System::Windows::Forms::DockStyle::Fill;
			this->PromptListViewPanel->Location = System::Drawing::Point(0, 128);
			this->PromptListViewPanel->Name = S"PromptListViewPanel";
			this->PromptListViewPanel->Size = System::Drawing::Size(456, 0);
			this->PromptListViewPanel->TabIndex = 15;
			// 
			// PromptMainPanel
			// 
			this->PromptMainPanel->Controls->Add(this->PromptEmailButton);
			this->PromptMainPanel->Controls->Add(this->PromptPictureBoxAlert);
			this->PromptMainPanel->Controls->Add(this->PromptButton1);
			this->PromptMainPanel->Controls->Add(this->PromptButton2);
			this->PromptMainPanel->Controls->Add(this->PromptButton3);
			this->PromptMainPanel->Controls->Add(this->PomptMoreButton);
			this->PromptMainPanel->Controls->Add(this->PromptPictureBox);
			this->PromptMainPanel->Controls->Add(this->PromptOKButton);
			this->PromptMainPanel->Controls->Add(this->PromptCopyButton);
			this->PromptMainPanel->Controls->Add(this->PromptMessageLabel);
			this->PromptMainPanel->Controls->Add(this->PromptDividerPanel);
			this->PromptMainPanel->Dock = System::Windows::Forms::DockStyle::Top;
			this->PromptMainPanel->Location = System::Drawing::Point(0, 0);
			this->PromptMainPanel->Name = S"PromptMainPanel";
			this->PromptMainPanel->Size = System::Drawing::Size(456, 128);
			this->PromptMainPanel->TabIndex = 16;
			// 
			// PromptPictureBoxAlert
			// 
			this->PromptPictureBoxAlert->Image = (__try_cast<System::Drawing::Image *  >(resources->GetObject(S"PromptPictureBoxAlert.Image")));
			this->PromptPictureBoxAlert->Location = System::Drawing::Point(8, 16);
			this->PromptPictureBoxAlert->Name = S"PromptPictureBoxAlert";
			this->PromptPictureBoxAlert->Size = System::Drawing::Size(48, 48);
			this->PromptPictureBoxAlert->TabIndex = 17;
			this->PromptPictureBoxAlert->TabStop = false;
			this->PromptPictureBoxAlert->Visible = false;
			// 
			// PromptPictureBoxError
			// 
			this->PromptPictureBoxError->Image = (__try_cast<System::Drawing::Image *  >(resources->GetObject(S"PromptPictureBoxError.Image")));
			this->PromptPictureBoxError->Location = System::Drawing::Point(8, 16);
			this->PromptPictureBoxError->Name = S"PromptPictureBoxError";
			this->PromptPictureBoxError->Size = System::Drawing::Size(48, 48);
			this->PromptPictureBoxError->TabIndex = 18;
			this->PromptPictureBoxError->TabStop = false;
			this->PromptPictureBoxError->Visible = false;
			// 
			// PromptPictureBoxCritical
			// 
			this->PromptPictureBoxCritical->Image = (__try_cast<System::Drawing::Image *  >(resources->GetObject(S"PromptPictureBoxCritical.Image")));
			this->PromptPictureBoxCritical->Location = System::Drawing::Point(8, 15);
			this->PromptPictureBoxCritical->Name = S"PromptPictureBoxCritical";
			this->PromptPictureBoxCritical->Size = System::Drawing::Size(48, 48);
			this->PromptPictureBoxCritical->TabIndex = 19;
			this->PromptPictureBoxCritical->TabStop = false;
			this->PromptPictureBoxCritical->Visible = false;
			// 
			// MOG_PromptDialog
			// 
			this->AutoScaleBaseSize = System::Drawing::Size(5, 13);
			this->ClientSize = System::Drawing::Size(464, 144);
			this->ControlBox = false;
			this->Controls->Add(this->PromptPictureBoxCritical);
			this->Controls->Add(this->PromptPictureBoxError);
			this->Controls->Add(this->PromptListViewPanel);
			this->Controls->Add(this->PromptMainPanel);
			this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::SizableToolWindow;
			this->Icon = (__try_cast<System::Drawing::Icon *  >(resources->GetObject(S"$this.Icon")));
//			this->MaximizeBox = false;
			this->MaximumSize = System::Drawing::Size(464, 190);
//			this->MinimizeBox = false;
			this->MinimumSize = System::Drawing::Size(464, 140);
//			this->Size = System::Drawing::Size(464, 152);
			this->Name = S"MOG_PromptDialog";
			this->StartPosition = System::Windows::Forms::FormStartPosition::CenterScreen;
			this->Text = S"Prompt Title:";
			this->Click += new System::EventHandler(this, &MOG::MOG_PromptDialog::PromptOKButton_Click);
			this->Closed += new System::EventHandler(this, &MOG::MOG_PromptDialog::MOG_PromptDialog_Closed);
			this->PromptListViewPanel->ResumeLayout(false);
			this->PromptMainPanel->ResumeLayout(false);
			this->ResumeLayout(false);

		}			



};

	public __gc struct ListViewMessage
	{
		String *mTitle;
		String *mMessage;
		String *mStackTrace;
		MOG_ALERT_LEVEL mLevel;
	};
}