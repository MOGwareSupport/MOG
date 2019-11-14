//--------------------------------------------------------------------------------
//	MOG_UITypeConverters.cpp
//	
//	
//--------------------------------------------------------------------------------
#include "StdAfx.h"


#include "MOG_Project.h"
#include "MOG_DosUtils.h"
#include "MOG_ControllerProject.h"
#include "MOG_ControllerSystem.h"


using namespace System::Drawing::Design;
using namespace System::Windows::Forms::Design;


System::Drawing::Design::UITypeEditorEditStyle MOGToolTypeEditor::GetEditStyle(System::ComponentModel::ITypeDescriptorContext* context)
{
	return System::Drawing::Design::UITypeEditorEditStyle::Modal;
}

Object *MOGToolTypeEditor::EditValue(System::ComponentModel::ITypeDescriptorContext* context, System::IServiceProvider* provider, Object* value)
{
	//System::Drawing::Design::UITypeEditor::GetEditStyle(
	String* oldValue = __try_cast<String*>(value);

	MOG_Project *proj = MOG_ControllerProject::GetProject();
	if (proj != NULL)
	{
		System::Windows::Forms::OpenFileDialog* ofd = new System::Windows::Forms::OpenFileDialog();

		if (File::Exists( String::Concat(proj->GetProjectToolsPath(), S"\\", oldValue) ))
		{
			ofd->InitialDirectory = DosUtils::PathGetDirectoryPath(String::Concat(proj->GetProjectToolsPath(), S"\\", oldValue));
		}
		else
		{
			ofd->InitialDirectory = proj->GetProjectToolsPath();
		}

		if (ShowToolsBrowserFuncion != NULL)
		{
			return ShowToolsBrowserFuncion(oldValue);
		}
		else if (ofd->ShowDialog() == MOGPromptResult::OK)
		{
			return ofd->FileName;
		}
	}

	return oldValue;
}


System::Drawing::Design::UITypeEditorEditStyle FilenameTypeEditor::GetEditStyle(System::ComponentModel::ITypeDescriptorContext* context)
{
	return System::Drawing::Design::UITypeEditorEditStyle::Modal;
}

Object *FilenameTypeEditor::EditValue(System::ComponentModel::ITypeDescriptorContext* context, System::IServiceProvider* provider, Object* value)
{
	//System::Drawing::Design::UITypeEditor::GetEditStyle(
	String* oldValue = __try_cast<String*>(value);

	System::Windows::Forms::OpenFileDialog* ofd = new System::Windows::Forms::OpenFileDialog();
	if (ofd->ShowDialog() == MOGPromptResult::OK)
	{
		return ofd->FileName;
	}

	return oldValue;
}



 System::Drawing::Design::UITypeEditorEditStyle PathTypeEditor::GetEditStyle(System::ComponentModel::ITypeDescriptorContext* context)
{
	return System::Drawing::Design::UITypeEditorEditStyle::Modal;
}

Object *PathTypeEditor::EditValue(System::ComponentModel::ITypeDescriptorContext* context, System::IServiceProvider* provider, Object* value)
{
	//Windows::Forms::Design::IWindowsFormsEditorService* editorService = (Windows::Forms::Design::IWindowsFormsEditorService)provider->GetService( typeof(Windows::Forms::Design::IWindowsFormsEditorService) );

	//System::Drawing::Design::UITypeEditor::GetEditStyle(
	String* oldValue = __try_cast<String*>(value);

	System::Windows::Forms::FolderBrowserDialog* fbd = new System::Windows::Forms::FolderBrowserDialog();
	if (fbd->ShowDialog() == DialogResult::OK)
		return fbd->SelectedPath;

	//context->Instance

	//editorService.DropDownControl( new DateTimePicker() );

	return oldValue;
	//return base.EditValue (context, provider, value);
}



Boolean IconTypeEditor::GetPaintValueSupported(System::ComponentModel::ITypeDescriptorContext* context)
{
	return true;
}

void IconTypeEditor::PaintValue(PaintValueEventArgs* e)
{
	// draw the image if we have one
	if (this->img != NULL)
		e->Graphics->DrawImage(this->img, 0, 0, e->Bounds.Width, e->Bounds.Height);
}


UITypeEditorEditStyle IconTypeEditor::GetEditStyle(System::ComponentModel::ITypeDescriptorContext* context)
{
	return UITypeEditorEditStyle::Modal;
}

Object* IconTypeEditor::EditValue(System::ComponentModel::ITypeDescriptorContext* context, IServiceProvider* provider, Object* value)
{
	// get the old value of the property in case we cancel the edit
	String* oldValue = __try_cast<String*>(value);

	System::Windows::Forms::OpenFileDialog* ofd = new System::Windows::Forms::OpenFileDialog();
	if (ofd->ShowDialog() == MOGPromptResult::OK)
	{
		this->img = System::Drawing::Image::FromFile(ofd->FileName);
		if (this->img != NULL)
		{
			// if we are logged in to a project, convert ofd->FileName to a relative path
			MOG_Project *proj = MOG_ControllerProject::GetProject();
			if (proj != NULL)
			{
				String *imagesPath = String::Concat(proj->GetProjectToolsPath(), "\\Images");
				String *imageFilename = DosUtils::PathGetFileName(ofd->FileName);
				
				// make sure the images path exists
				if (!Directory::Exists(imagesPath))
				{
					Directory::CreateDirectory(imagesPath);
				}

				if (File::Exists(String::Concat(imagesPath, "\\", imageFilename)))
				{
					// no need to recopy it if it exists, so just return relative path
					return String::Concat("Images\\", imageFilename);
				}
				else if (DosUtils::FileCopyFast(ofd->FileName, String::Concat(imagesPath, "\\", imageFilename), true))
				{
					// if the file was copied correctly, display notification message and return relative path
					MOG_Prompt::PromptResponse("Image Copy Notification", String::Concat(imageFilename, S" has been copied to to project Images folder"));
					return String::Concat("Images\\", imageFilename);
				}
			}

			// if there's no project or we errored out for some reason, just return the unadulterated path
			return ofd->FileName;
		}
	}

	// user cancelled the edit, so retain the original value
	return oldValue;
}
