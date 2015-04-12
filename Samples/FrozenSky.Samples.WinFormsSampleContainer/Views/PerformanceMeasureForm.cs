#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2015 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion

using FrozenSky.Multimedia.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrozenSky.Util;

namespace FrozenSky.Samples.WinFormsSampleContainer.Views
{
    public partial class PerformanceMeasureForm : Form
    {
        private PerformanceAnalyzer m_performanceAnalyzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMeasureForm"/> class.
        /// </summary>
        public PerformanceMeasureForm()
        {
            InitializeComponent();

            m_colDuration.DefaultCellStyle.Format = "N3";
            m_colDuration.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if(!this.DesignMode)
            {
                m_performanceAnalyzer = GraphicsCore.Current.PerformanceCalculator;
            }
        }

        private void OnRefreshTimerTick(object sender, EventArgs e)
        {
            m_dataSource.DataSource = new List<DurationPerformanceResult>(m_performanceAnalyzer.UIDurationKpisCurrents
                .OrderBy((actResult) => actResult.CalculatorName));
        }

        private void OnCmdCopyClick(object sender, EventArgs e)
        {
            m_dataGrid.CopyContentsToClipboard();
        }
    }
}
