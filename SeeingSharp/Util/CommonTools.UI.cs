#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
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

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DESKTOP
using WinForms = System.Windows.Forms;
#endif

namespace SeeingSharp.Util
{
    public static partial class CommonTools
    {
#if DESKTOP
        public static void CopyContentsToClipboard(this WinForms.DataGridView dataGrid)
        {
            // Prepare copy string builder
            StringBuilder csvBuilder = new StringBuilder(dataGrid.RowCount * 100 + 100);

            // Build header string
            bool isFirst = true;
            foreach(WinForms.DataGridViewColumn actColumn in dataGrid.Columns)
            {
                if (!isFirst) { csvBuilder.Append(';'); }
                isFirst = false;

                csvBuilder.Append(actColumn.HeaderText);
            }
            csvBuilder.Append(Environment.NewLine);

            // Build strings for all lines
            for(int actRowID =0; actRowID<dataGrid.Rows.Count; actRowID++)
            {
                for(int actColumnID = 0; actColumnID < dataGrid.Columns.Count; actColumnID++)
                {
                    if (actColumnID > 0) { csvBuilder.Append(';'); }

                    object actValue = dataGrid[actColumnID, actRowID].Value;
                    csvBuilder.Append(actValue != null ? actValue.ToString() : string.Empty);
                }
                csvBuilder.Append(Environment.NewLine);
            }

            // Add text to the clipboard
            //  Here we do specialized conversion for excel compatibility
            //  see http://stackoverflow.com/questions/329918/how-to-paste-csv-data-to-windows-clipboard-with-c-sharp
            System.Windows.DataObject dataObject = new System.Windows.DataObject();

            // Convert the CSV text to a UTF-8 byte stream before adding it to the container object.
            var bytes = System.Text.Encoding.UTF8.GetBytes(csvBuilder.ToString());
            var stream = new System.IO.MemoryStream(bytes);
            dataObject.SetData(System.Windows.DataFormats.CommaSeparatedValue, stream);

            // Copy the container object to the clipboard.
            System.Windows.Clipboard.SetDataObject(dataObject, true);
        }

        public static void ShowChildForm(this WinForms.Form parentForm, WinForms.Form childForm)
        {
            //childForm.Opacity = 0.5;
            //childForm.LostFocus += (sender, eArgs) =>
            //{
            //    childForm.Opacity = 0.5;
            //};
            //childForm.GotFocus += (sender, eArgs) =>
            //{
            //    childForm.Opacity = 1.0;
            //};

            childForm.Show(parentForm);
        }
#endif
    }
}
