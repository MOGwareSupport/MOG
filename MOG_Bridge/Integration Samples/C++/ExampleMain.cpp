int glestMain(){

	MainWindow *mainWindow= NULL;
	Program *program= NULL;
	ExceptionHandler exceptionHandler;
	exceptionHandler.install();

	try{
		Config &config = Config::getInstance();

		showCursor(config.getBool("Windowed"));
		
		program= new Program();
		mainWindow= new MainWindow(program);
		program->init(mainWindow);

#if MOG_INTEGRATION
		MOG_Integration::SetEventHandler(HandleMogEvent, NULL);
		MOG_Integration::ConnectToMOG();
#endif

		while(Window::handleEvent()){
#if MOG_INTEGRATION
			MOG_Integration::ProcessTick();
#endif
			program->loop();
		}
	}
	catch(const exception &e){
		restoreVideoMode();
		exceptionMessage(e);
	}	

	delete mainWindow;

	return 0;
}



#if MOG_INTEGRATION
void HandleMogEvent(void* parameter, char* filename)
{
	strlwr(filename);
						
	char* backslash = 0;
	while(backslash = strchr(filename, '\\'))
	{
		*backslash = '/';
	}

	if (strstr(filename, ".bmp") ||	strstr(filename, ".tga"))
	{
		// Reload the identified texture
		Renderer &renderer= Renderer::getInstance();
		for (int i = 0; i < rsCount; i++)
		{
			TextureManager* pManager = renderer.getTextureManager((ResourceScope)i);
			if (pManager)
			{
				Texture2D* pTexture = (Texture2D*)pManager->getTexture(filename);
				if (pTexture)
				{
					pTexture->end();
					pTexture->load(filename);
					pTexture->init();
					break;
				}
			}
		}
	}
	else if (strstr(filename, ".g3d"))
	{
		Renderer &renderer= Renderer::getInstance();
		for (int i = 0; i < rsCount; i++)
		{
			ModelManager* pManager = renderer.getModelManager((ResourceScope)i);
			if (pManager)
			{
				Model* pModel = pManager->getModel(filename);
				if (pModel)
				{
					pModel->unload();
					pModel->load(filename);
					break;
				}
			}
		}
	}
	else if (strstr(filename, ".wav") || strstr(filename, ".ogg"))
	{
		//closeSound();
		//openSound("bzflag");
	}
}
#endif