#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# Tools. More info at
     - https://github.com/RolandKoenig/SeeingSharp/tree/master/Tools (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)

	This program is distributed under the terms of the Microsoft Public License (Ms-PL)-
	More info at https://msdn.microsoft.com/en-us/library/ff647676.aspx
*/
#endregion License information (SeeingSharp and all based games/applications)

using Microsoft.Win32;
using SeeingSharp.Util;
using System.Windows;

namespace SeeingSharpModelViewer
{
    public class CommonDialogsViewService : ICommonDialogsViewService
    {
        private FrameworkElement m_host;

        public void SetViewServiceHost(FrameworkElement frameworkElement)
        {
            m_host = frameworkElement;
        }

        public ResourceLink PickFileByDialog(string fileFilter)
        {
            // Prepare the dialog
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = fileFilter;
            dlgOpenFile.CheckFileExists = true;
            dlgOpenFile.CheckPathExists = true;
            dlgOpenFile.Multiselect = false;
            dlgOpenFile.RestoreDirectory = true;
            dlgOpenFile.ValidateNames = true;

            // Show the dialog and return the result
            if (true == dlgOpenFile.ShowDialog(Window.GetWindow(m_host)))
            {
                return dlgOpenFile.FileName;
            }
            else
            {
                return null;
            }
        }
    }
}