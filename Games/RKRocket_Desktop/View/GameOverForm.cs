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
using RKRocket.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RKRocket.View
{
    public partial class GameOverForm : Form
    {
        public GameOverForm()
        {
            InitializeComponent();
        }

        private void OnCmdCancel_Click(object sender, EventArgs e)
        {
            this.ViewModel.CommandCancel.Execute();

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OnCmdOK_Click(object sender, EventArgs e)
        {
            this.ViewModel.CommandPostScore.Execute();

            if (!this.ViewModel.ContainsErrors)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        public GameOverViewModel ViewModel
        {
            get { return m_dataSource.DataSource as GameOverViewModel; }
            set
            {
                if(value == null) { m_dataSource.DataSource = typeof(GameOverViewModel); }
                else { m_dataSource.DataSource = value; }
            }
        }
    }
}
