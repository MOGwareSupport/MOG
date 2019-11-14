
#ifndef __MOG_UITYPECONVERTERS_H__
#define __MOG_UITYPECONVERTERS_H__

#using <system.drawing.dll>

using namespace System::Drawing::Design;
using namespace System::Windows::Forms::Design;
using namespace System::ComponentModel;


namespace MOG
{
namespace UITYPESEDITORS
{

public __gc class MOGToolTypeEditor : public System::Drawing::Design::UITypeEditor
{
public:
		virtual UITypeEditorEditStyle GetEditStyle(System::ComponentModel::ITypeDescriptorContext* context);
		virtual Object *EditValue(System::ComponentModel::ITypeDescriptorContext* context, System::IServiceProvider* provider, Object* value);
		__delegate String* ShowToolsBrowser(String* path);
		static ShowToolsBrowser* ShowToolsBrowserFuncion;
};

public __gc class FilenameTypeEditor : public System::Drawing::Design::UITypeEditor
{
public:
		virtual System::Drawing::Design::UITypeEditorEditStyle GetEditStyle(System::ComponentModel::ITypeDescriptorContext* context);
		virtual Object *EditValue(System::ComponentModel::ITypeDescriptorContext* context, System::IServiceProvider* provider, Object* value);
};


public __gc class PathTypeEditor : public System::Drawing::Design::UITypeEditor
{
public:
		virtual System::Drawing::Design::UITypeEditorEditStyle GetEditStyle(System::ComponentModel::ITypeDescriptorContext* context);
		virtual Object *EditValue(System::ComponentModel::ITypeDescriptorContext* context, System::IServiceProvider* provider, Object* value);
};


public __gc class IconTypeEditor : public System::Drawing::Design::UITypeEditor
{
private:
	System::Drawing::Image* img;

public:
		virtual Boolean GetPaintValueSupported(System::ComponentModel::ITypeDescriptorContext* context);
		virtual void PaintValue(PaintValueEventArgs* e);
		virtual UITypeEditorEditStyle GetEditStyle(System::ComponentModel::ITypeDescriptorContext* context);
		virtual Object* EditValue(System::ComponentModel::ITypeDescriptorContext* context, IServiceProvider* provider, Object* value);
};

public __gc class MOG_PropertyDescriptor : public PropertyDescriptor
{
private:
	PropertyDescriptor *basePropertyDescriptor;
	bool mInherited;
	bool mReadOnly;

public:

	MOG_PropertyDescriptor(PropertyDescriptor *basePropertyDescriptor, bool bInherited, bool bReadOnly)
		: PropertyDescriptor( basePropertyDescriptor )
	{
		this->basePropertyDescriptor = basePropertyDescriptor;
		this->mInherited = bInherited;
		this->mReadOnly = bReadOnly;
	}

	__property bool get_IsReadOnly()
	{
		return mReadOnly;
	}

	__property String *get_DisplayName()
	{
		String *displayName = basePropertyDescriptor->DisplayName;
		String *newDisplayName;

		if( displayName->IndexOf( S"_InheritedBoolean" ) > -1 )
		{
			newDisplayName = displayName->Replace(S"_InheritedBoolean", S"");
		}
		else
		{
			newDisplayName = displayName;
		}

		return newDisplayName;
	}

//		#region Abstract methods and properties we must provide implementation for
	bool CanResetValue(Object *component)
	{
		return basePropertyDescriptor->CanResetValue(component);
	}

	Type *get_ComponentType()
	{
		return basePropertyDescriptor->ComponentType;
	}

	Object *GetValue(Object *component)
	{
		return basePropertyDescriptor->GetValue(component);
	}

	__property String *get_Name()
	{
		return basePropertyDescriptor->Name;
	}

	__property Type *get_PropertyType()
	{
		return basePropertyDescriptor->PropertyType;
	}

	void ResetValue(Object *component)
	{
		basePropertyDescriptor->ResetValue(component);
	}

	bool ShouldSerializeValue(Object *component)
	{
		if (mInherited)
		{
			return false;
		}
		else
		{
			return basePropertyDescriptor->ShouldSerializeValue(component);
		}
	}

	void SetValue(Object *component, Object *value)
	{
		basePropertyDescriptor->SetValue(component, value);
	}
//		#endregion Abstract methods and properties we must provide implementation for
};

} // end ns
}

#endif

