#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;

#include "MOG_ControllerProject.h"

namespace MOG
{
	public __value enum NODE_IMAGES
	{
		WINDOWS,
		SOURCE,
		ALLCOMPUTER,
	};
	/// <summary> 
	/// Summary for MOG_PromptMoreDialog
	///
	/// WARNING: If you change the name of this class, you will need to change the 
	///          'Resource File Name' property for the managed resource compiler tool 
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public __gc class MOG_PromptMoreDialog : public System::Windows::Forms::Form
	{
	public: 
		MOG_PromptMoreDialog(void);
		void SetStackTrace(String *stackTrace);
		void SetInfo(String *title, String *message);

		void CopyMessageToClipboard();
		void CopyMessageToEmail();
	
	protected: 
		void Dispose(Boolean disposing);

		void CopyMessageToEmail_Worker(Object* sender, DoWorkEventArgs* e);

	private:
		String *mFullStackTrace;
		String *mFullMessage;
	private: System::Windows::Forms::RichTextBox*  PromptMoreErrorMessageLabel;

		String *mFullTitle;
		
		void SetMessageText(String *text);
		String *FormatErrorMessage();
	    
	public: System::Windows::Forms::Label *  PromptMoreErrorLabel;
	public: System::Windows::Forms::RichTextBox *  PromptMoreMessageLabel;
	private: System::Windows::Forms::Button *  PromptMoreOKButton;
	private: System::Windows::Forms::TreeView *  PromptMoreTreeView;
	private: System::Windows::Forms::Panel *  PromptPanel;
	private: System::Windows::Forms::Label *  PromptCallstackLabel;
	private: System::Windows::Forms::Panel *  PromptCallstackPanel;
	private: System::Windows::Forms::ImageList *  PromptCallbackImageList;
	private: System::Windows::Forms::ToolTip *  PromptMoreToolTip;
	private: System::Windows::Forms::Splitter *  PromptTreeViewSplitter;
	private: System::Windows::Forms::Button *  PromptCopyButton;
	private: System::ComponentModel::IContainer *  components;
	private: System::Windows::Forms::Button *  PromptEmailButton;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (new System::ComponentModel::Container());
			System::ComponentModel::ComponentResourceManager*  resources = (new System::ComponentModel::ComponentResourceManager(__typeof(MOG_PromptMoreDialog)));
			this->PromptMoreOKButton = (new System::Windows::Forms::Button());
			this->PromptMoreTreeView = (new System::Windows::Forms::TreeView());
			this->PromptCallbackImageList = (new System::Windows::Forms::ImageList(this->components));
			this->PromptMoreErrorLabel = (new System::Windows::Forms::Label());
			this->PromptPanel = (new System::Windows::Forms::Panel());
			this->PromptMoreMessageLabel = (new System::Windows::Forms::RichTextBox());
			this->PromptTreeViewSplitter = (new System::Windows::Forms::Splitter());
			this->PromptCallstackPanel = (new System::Windows::Forms::Panel());
			this->PromptCallstackLabel = (new System::Windows::Forms::Label());
			this->PromptCopyButton = (new System::Windows::Forms::Button());
			this->PromptMoreToolTip = (new System::Windows::Forms::ToolTip(this->components));
			this->PromptEmailButton = (new System::Windows::Forms::Button());
			this->PromptMoreErrorMessageLabel = (new System::Windows::Forms::RichTextBox());
			this->PromptPanel->SuspendLayout();
			this->PromptCallstackPanel->SuspendLayout();
			this->SuspendLayout();
			// 
			// PromptMoreOKButton
			// 
			this->PromptMoreOKButton->Anchor = (System::Windows::Forms::AnchorStyles)(System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right);
			this->PromptMoreOKButton->DialogResult = System::Windows::Forms::DialogResult::OK;
			this->PromptMoreOKButton->FlatStyle = System::Windows::Forms::FlatStyle::System;
			this->PromptMoreOKButton->Location = System::Drawing::Point(440, 288);
			this->PromptMoreOKButton->Name = S"PromptMoreOKButton";
			this->PromptMoreOKButton->Size = System::Drawing::Size(75, 23);
			this->PromptMoreOKButton->TabIndex = 1;
			this->PromptMoreOKButton->Text = S"Ok";
			this->PromptMoreOKButton->Click += new System::EventHandler(this, &MOG_PromptMoreDialog::PromptMoreOKButton_Click);
			// 
			// PromptMoreTreeView
			// 
			this->PromptMoreTreeView->Dock = System::Windows::Forms::DockStyle::Fill;
			this->PromptMoreTreeView->FullRowSelect = true;
			this->PromptMoreTreeView->HotTracking = true;
			this->PromptMoreTreeView->ImageIndex = 1;
			this->PromptMoreTreeView->ImageList = this->PromptCallbackImageList;
			this->PromptMoreTreeView->Location = System::Drawing::Point(0, 16);
			this->PromptMoreTreeView->Name = S"PromptMoreTreeView";
			this->PromptMoreTreeView->SelectedImageIndex = 1;
			this->PromptMoreTreeView->Size = System::Drawing::Size(232, 192);
			this->PromptMoreTreeView->TabIndex = 2;
			this->PromptMoreTreeView->AfterSelect += new System::Windows::Forms::TreeViewEventHandler(this, &MOG_PromptMoreDialog::PromptMoreTreeView_AfterSelect);
			// 
			// PromptCallbackImageList
			// 
			this->PromptCallbackImageList->ImageStream = (__try_cast<System::Windows::Forms::ImageListStreamer*  >(resources->GetObject(S"PromptCallbackImageList.ImageStream")));
			this->PromptCallbackImageList->TransparentColor = System::Drawing::Color::Transparent;
			this->PromptCallbackImageList->Images->SetKeyName(0, S"");
			this->PromptCallbackImageList->Images->SetKeyName(1, S"");
			this->PromptCallbackImageList->Images->SetKeyName(2, S"");
			// 
			// PromptMoreErrorLabel
			// 
			this->PromptMoreErrorLabel->Dock = System::Windows::Forms::DockStyle::Top;
			this->PromptMoreErrorLabel->Font = (new System::Drawing::Font(S"Microsoft Sans Serif", 8.25F, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point, 
				(System::Byte)0));
			this->PromptMoreErrorLabel->Location = System::Drawing::Point(232, 0);
			this->PromptMoreErrorLabel->Name = S"PromptMoreErrorLabel";
			this->PromptMoreErrorLabel->Size = System::Drawing::Size(272, 16);
			this->PromptMoreErrorLabel->TabIndex = 3;
			this->PromptMoreErrorLabel->Text = S"Details:";
			// 
			// PromptPanel
			// 
			this->PromptPanel->Anchor = (System::Windows::Forms::AnchorStyles)(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right);
			this->PromptPanel->Controls->Add(this->PromptMoreMessageLabel);
			this->PromptPanel->Controls->Add(this->PromptTreeViewSplitter);
			this->PromptPanel->Controls->Add(this->PromptMoreErrorLabel);
			this->PromptPanel->Controls->Add(this->PromptCallstackPanel);
			this->PromptPanel->Location = System::Drawing::Point(8, 72);
			this->PromptPanel->Name = S"PromptPanel";
			this->PromptPanel->Size = System::Drawing::Size(504, 208);
			this->PromptPanel->TabIndex = 4;
			// 
			// PromptMoreMessageLabel
			// 
			this->PromptMoreMessageLabel->BackColor = System::Drawing::SystemColors::Control;
			this->PromptMoreMessageLabel->Dock = System::Windows::Forms::DockStyle::Fill;
			this->PromptMoreMessageLabel->Location = System::Drawing::Point(240, 16);
			this->PromptMoreMessageLabel->Name = S"PromptMoreMessageLabel";
			this->PromptMoreMessageLabel->Size = System::Drawing::Size(264, 192);
			this->PromptMoreMessageLabel->TabIndex = 6;
			this->PromptMoreMessageLabel->Text = S"";
			this->PromptMoreMessageLabel->LinkClicked += new System::Windows::Forms::LinkClickedEventHandler(this, &MOG_PromptMoreDialog::PromptMoreMessageLabel_LinkClicked);
			// 
			// PromptTreeViewSplitter
			// 
			this->PromptTreeViewSplitter->BorderStyle = System::Windows::Forms::BorderStyle::Fixed3D;
			this->PromptTreeViewSplitter->Location = System::Drawing::Point(232, 16);
			this->PromptTreeViewSplitter->Name = S"PromptTreeViewSplitter";
			this->PromptTreeViewSplitter->Size = System::Drawing::Size(8, 192);
			this->PromptTreeViewSplitter->TabIndex = 4;
			this->PromptTreeViewSplitter->TabStop = false;
			// 
			// PromptCallstackPanel
			// 
			this->PromptCallstackPanel->Controls->Add(this->PromptMoreTreeView);
			this->PromptCallstackPanel->Controls->Add(this->PromptCallstackLabel);
			this->PromptCallstackPanel->Dock = System::Windows::Forms::DockStyle::Left;
			this->PromptCallstackPanel->Location = System::Drawing::Point(0, 0);
			this->PromptCallstackPanel->Name = S"PromptCallstackPanel";
			this->PromptCallstackPanel->Size = System::Drawing::Size(232, 208);
			this->PromptCallstackPanel->TabIndex = 5;
			// 
			// PromptCallstackLabel
			// 
			this->PromptCallstackLabel->Dock = System::Windows::Forms::DockStyle::Top;
			this->PromptCallstackLabel->Font = (new System::Drawing::Font(S"Microsoft Sans Serif", 8.25F, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point, 
				(System::Byte)0));
			this->PromptCallstackLabel->Location = System::Drawing::Point(0, 0);
			this->PromptCallstackLabel->Name = S"PromptCallstackLabel";
			this->PromptCallstackLabel->Size = System::Drawing::Size(232, 16);
			this->PromptCallstackLabel->TabIndex = 5;
			this->PromptCallstackLabel->Text = S"Message component:";
			// 
			// PromptCopyButton
			// 
			this->PromptCopyButton->Anchor = (System::Windows::Forms::AnchorStyles)(System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left);
			this->PromptCopyButton->FlatStyle = System::Windows::Forms::FlatStyle::Flat;
			this->PromptCopyButton->ForeColor = System::Drawing::SystemColors::Control;
			this->PromptCopyButton->Image = (__try_cast<System::Drawing::Image*  >(resources->GetObject(S"PromptCopyButton.Image")));
			this->PromptCopyButton->Location = System::Drawing::Point(256, 288);
			this->PromptCopyButton->Name = S"PromptCopyButton";
			this->PromptCopyButton->Size = System::Drawing::Size(24, 23);
			this->PromptCopyButton->TabIndex = 5;
			this->PromptMoreToolTip->SetToolTip(this->PromptCopyButton, S"Copy message text to clipboard");
			this->PromptCopyButton->Click += new System::EventHandler(this, &MOG_PromptMoreDialog::PromptEmailButton_Click);
			// 
			// PromptEmailButton
			// 
			this->PromptEmailButton->Anchor = (System::Windows::Forms::AnchorStyles)(System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left);
			this->PromptEmailButton->FlatStyle = System::Windows::Forms::FlatStyle::Popup;
			this->PromptEmailButton->ForeColor = System::Drawing::SystemColors::ControlText;
			this->PromptEmailButton->Image = (__try_cast<System::Drawing::Image*  >(resources->GetObject(S"PromptEmailButton.Image")));
			this->PromptEmailButton->ImageAlign = System::Drawing::ContentAlignment::MiddleLeft;
			this->PromptEmailButton->Location = System::Drawing::Point(8, 282);
			this->PromptEmailButton->Name = S"PromptEmailButton";
			this->PromptEmailButton->Size = System::Drawing::Size(160, 32);
			this->PromptEmailButton->TabIndex = 6;
			this->PromptEmailButton->Text = S"Send report to MOGware";
			this->PromptEmailButton->TextAlign = System::Drawing::ContentAlignment::MiddleRight;
			this->PromptMoreToolTip->SetToolTip(this->PromptEmailButton, S"Send error report to MOGware");
			this->PromptEmailButton->Click += new System::EventHandler(this, &MOG_PromptMoreDialog::PromptEmailButton_Click);
			// 
			// PromptMoreErrorMessageLabel
			// 
			this->PromptMoreErrorMessageLabel->BackColor = System::Drawing::SystemColors::Control;
			this->PromptMoreErrorMessageLabel->BorderStyle = System::Windows::Forms::BorderStyle::None;
			this->PromptMoreErrorMessageLabel->Location = System::Drawing::Point(8, 2);
			this->PromptMoreErrorMessageLabel->Name = S"PromptMoreErrorMessageLabel";
			this->PromptMoreErrorMessageLabel->Size = System::Drawing::Size(507, 67);
			this->PromptMoreErrorMessageLabel->TabIndex = 7;
			this->PromptMoreErrorMessageLabel->Text = S"Error Message";
			// 
			// MOG_PromptMoreDialog
			// 
			this->AcceptButton = this->PromptMoreOKButton;
			this->AutoScaleBaseSize = System::Drawing::Size(5, 13);
			this->ClientSize = System::Drawing::Size(520, 317);
			this->Controls->Add(this->PromptMoreErrorMessageLabel);
			this->Controls->Add(this->PromptEmailButton);
			this->Controls->Add(this->PromptCopyButton);
			this->Controls->Add(this->PromptPanel);
			this->Controls->Add(this->PromptMoreOKButton);
			this->Icon = (__try_cast<System::Drawing::Icon*  >(resources->GetObject(S"$this.Icon")));
			this->MinimizeBox = false;
			this->MinimumSize = System::Drawing::Size(424, 168);
			this->Name = S"MOG_PromptMoreDialog";
			this->ShowInTaskbar = false;
			this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
			this->Text = S"More Info Title:";
			this->PromptPanel->ResumeLayout(false);
			this->PromptCallstackPanel->ResumeLayout(false);
			this->ResumeLayout(false);

		}		
	private: System::Void PromptMoreTreeView_AfterSelect(System::Object *  sender, System::Windows::Forms::TreeViewEventArgs *  e);
	private: System::Void PromptMoreMessageLabel_LinkClicked(System::Object *  sender, System::Windows::Forms::LinkClickedEventArgs *  e);
	private: System::Void PromptCopyButton_Click(System::Object *  sender, System::EventArgs *  e);
	private: System::Void PromptEmailButton_Click(System::Object *  sender, System::EventArgs *  e);
	private: System::Void PromptMoreOKButton_Click(System::Object *  sender, System::EventArgs *  e);
};
}

using namespace MOG;