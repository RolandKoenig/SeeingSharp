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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeeingSharp.Multimedia.UI
{
    public partial class LoadExternalModelFormsDlg : Form
    {
        public LoadExternalModelFormsDlg()
        {
            InitializeComponent();

            m_dataSource.DataSource = new LoadExternalModelVM();
            m_dlgOpenFile.Filter = GraphicsCore.Current.ImportersAndExporters.GetOpenFileDialogFilter();
        }

        /// <summary>
        /// Updates the state of the dialog.
        /// </summary>
        private void UpdateDialogState()
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.UpdateDialogState();
        }

        private void OnCmdFilePath_Click(object sender, EventArgs e)
        {
            if(m_dlgOpenFile.ShowDialog(this) == DialogResult.OK)
            {

            }

            this.UpdateDialogState();
        }

        private void OnCmdOK_Click(object sender, EventArgs e)
        {

        }

        private void OnCmdCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
