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
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharpModelViewer
{
    public class Loaded3DModelViewModel : ViewModelBase
    {
        private ImportOptions m_importOptions;

        /// <summary>
        /// Creates the test data for the designer.
        /// </summary>
        public static Loaded3DModelViewModel CreateTestDataForDesigner()
        {
            return null;
        }


        public ImportOptions ImportOptions
        {
            get { return m_importOptions; }
            private set
            {
                if(m_importOptions != value)
                {
                    m_importOptions = value;
                    RaisePropertyChanged(nameof(ImportOptions));
                }
            }
        }
    }
}
