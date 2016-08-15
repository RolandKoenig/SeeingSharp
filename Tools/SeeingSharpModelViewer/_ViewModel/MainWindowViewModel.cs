#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# Tools. More info at 
     - https://github.com/RolandKoenig/SeeingSharp/tree/master/Tools (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
	This program is distributed under the terms of the Microsoft Public License (Ms-PL)-
	More info at https://msdn.microsoft.com/en-us/library/ff647676.aspx
*/
#endregion
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Infrastructure;

namespace SeeingSharpModelViewer
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region main references
        private Scene m_scene;
        private Camera3DBase m_camera;
        #endregion main references

        #region Child viewmodels
        private LoadedFileViewModel m_loadedFileVM;
        private MiscGraphicsObjectsViewModel m_miscObjectsVM;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            if (!SeeingSharpApplication.IsInitialized) { return; }

            m_scene = new Scene();
            m_scene.DiscardAutomaticUnload = true;

            m_camera = new PerspectiveCamera3D();

            m_loadedFileVM = new LoadedFileViewModel(m_scene);
            m_miscObjectsVM = new MiscGraphicsObjectsViewModel(m_scene);

            this.CommandOpen = new DelegateCommand(OnCommandOpen_Execute);
            this.CommandClose = new DelegateCommand(OnCommandClose_Execute, OnCommandClose_CanExecute);
        }

        /// <summary>
        /// Creates the test data for the designer.
        /// </summary>
        public static MainWindowViewModel CreateTestDataForDesigner()
        {
            return new MainWindowViewModel();
        }

        public Task InitializeAsync()
        {
            return m_miscObjectsVM.InitializeAsync();
        }

        private async void OnCommandOpen_Execute()
        {
            ICommonDialogsViewService dialogService = base.TryGetViewService<ICommonDialogsViewService>();
            dialogService.EnsureNotNull(nameof(dialogService));

            ResourceLink fileToOpen = dialogService.PickFileByDialog(
                GraphicsCore.Current.ImportersAndExporters.GetOpenFileDialogFilter());
            if(fileToOpen == null) { return; }

            await m_loadedFileVM.ImportNewFileAsync(fileToOpen);
        }

        private async void OnCommandClose_Execute()
        {
            await m_loadedFileVM.CloseAsync();
        }

        private bool OnCommandClose_CanExecute()
        {
            return m_loadedFileVM.CurrentFile != null;   
        }

        public Scene Scene
        {
            get { return m_scene; }
        }

        public Camera3DBase Camera
        {
            get { return m_camera; }
        }

        public LoadedFileViewModel LoadedFile
        {
            get { return m_loadedFileVM; }
        }

        public MiscGraphicsObjectsViewModel MiscObjects
        {
            get { return m_miscObjectsVM; }
        }

        public DelegateCommand CommandOpen
        {
            get;
            private set;
        }

        public DelegateCommand CommandClose
        {
            get;
            private set;
        }
    }
}
