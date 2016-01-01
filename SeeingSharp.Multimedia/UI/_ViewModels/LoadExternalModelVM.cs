#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.UI
{
    public class LoadExternalModelVM : ViewModelBase
    {
        #region Members for model preview
        private Scene m_scene;
        private Camera3DBase m_camera;
        #endregion

        #region Members for model data
        private string m_filePath;
        private ImportOptions m_importOptions;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadExternalModelVM"/> class.
        /// </summary>
        public LoadExternalModelVM()
        {
            m_scene = new Scene();
            m_camera = new PerspectiveCamera3D();

            m_importOptions = new ImportOptions();

            this.Commmand_ReloadModel = new DelegateCommand(OnCommandReloadModel_Execute);
        }

        private void OnCommandReloadModel_Execute()
        {

        }

        public string FilePath
        {
            get { return m_filePath; }
            set
            {
                if(m_filePath != value)
                {
                    m_filePath = value;
                    RaisePropertyChanged(nameof(FilePath));
                }
            }
        }

        public Scene Scene
        {
            get { return m_scene; }
        }

        public Camera3DBase Camera
        {
            get { return m_camera; }
        }

        public ImportOptions ImportOptions
        {
            get { return m_importOptions; }
        }

        public DelegateCommand Commmand_ReloadModel
        {
            get;
            private set;
        }
    }
}
