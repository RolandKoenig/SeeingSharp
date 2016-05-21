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
using SeeingSharp.Multimedia.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeeingSharpModelViewer.View
{
    public partial class ImportDialog : Form
    {
        private ImportOptions m_currentImportOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportDialog"/> class.
        /// </summary>
        public ImportDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Update current ImportOptions based on selected path.
        /// </summary>
        /// <param name="path">The path of the model file.</param>
        private void SetImportOptionsFor(string path)
        {
            // Create new ImportOptions object based on selected path
            ImportOptions newImportOptions = null;
            if(!string.IsNullOrEmpty(path))
            {
                string extension = Path.GetExtension(path);
                if(!string.IsNullOrEmpty(extension))
                {
                    newImportOptions = GraphicsCore.Current.ImportersAndExporters.CreateImportOptionsByFileType(extension);
                }
            }

            // Replace currently selected ImportOptions object if necessary
            if(m_currentImportOptions == null) { m_currentImportOptions = newImportOptions; }
            else if(newImportOptions == null) { m_currentImportOptions = null; }
            else if(m_currentImportOptions.GetType() != newImportOptions.GetType())
            {
                m_currentImportOptions = newImportOptions;
            }

            // Update PropertyGrid
            m_gridProperties.SelectedObject = m_currentImportOptions;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            m_dlgOpenModelFile.Filter = GraphicsCore.Current.ImportersAndExporters.GetOpenFileDialogFilter();
        }

        private void OnCmdOK_Click(object sender, EventArgs e)
        {
            if((!string.IsNullOrEmpty(m_txtModelPath.Text)) &&
               (m_currentImportOptions != null))
            {
                this.FileName = m_txtModelPath.Text;
                this.ImportOptions = m_currentImportOptions;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void OnCmdCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OnCmdModelPath_Click(object sender, EventArgs e)
        {
            if(m_dlgOpenModelFile.ShowDialog(this) == DialogResult.OK)
            {
                m_txtModelPath.Text = m_dlgOpenModelFile.FileName;
            }
        }

        private void OnTxtModelPath_TextChanged(object sender, EventArgs e)
        {
            SetImportOptionsFor(m_txtModelPath.Text);
        }

        /// <summary>
        /// The selected model path.
        /// </summary>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// The configured ImportOptions for the selected model path.
        /// </summary>
        public ImportOptions ImportOptions
        {
            get;
            set;
        }
    }
}
