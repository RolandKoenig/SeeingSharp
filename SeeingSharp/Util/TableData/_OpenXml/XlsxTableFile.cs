#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SeeingSharp.Util.TableData
{
    public class XlsxTableFile : ITableFile
    {
        private const string ATTRIBUTE_SHEET_NAME = "name";
        private const string ATTRIBUTE_SHEET_ID = "sheetId";
        private const string FILENAME_SHEET = "sheet{0}.xml";

        // Common members
        #region
        private SpreadsheetDocument m_spreadsheetDocument;
        private XlsxImporterConfig m_importerConfig;
        #endregion

        // Cached data
        #region
        private List<SheetInformation> m_sheetInfos;
        private List<string> m_sharedStrings;
        #endregion

        internal XlsxTableFile(ResourceLink fileSource, XlsxImporterConfig importerConfig)
        {
            m_spreadsheetDocument = SpreadsheetDocument.Open(
                fileSource.OpenInputStream(),
                false);
            m_importerConfig = importerConfig;

            // Preload all needed information
            this.LoadSheetInformation();
            this.LoadSharedStrings();
            this.LoadHeaderRows();
        }

        /// <summary>
        /// Gets a collection containing all tables names defined
        /// </summary>
        public IEnumerable<string> GetTableNames()
        {
            for(int loop=0 ; loop<m_sheetInfos.Count; loop++)
            {
                yield return m_sheetInfos[loop].Name;
            }
        }

        /// <summary>
        /// Reads the header row.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        public ITableHeaderRow ReadHeaderRow(string tableName)
        {
            for(int loop=0 ; loop<m_sheetInfos.Count; loop++)
            {
                SheetInformation actSheetInfo = m_sheetInfos[loop];
                if (actSheetInfo.Name == tableName) { return actSheetInfo.HeaderRow; }
            }

            throw new SeeingSharpException(string.Format("Table {0} not found in document!", tableName));
        }

        /// <summary>
        /// Opens a reader which reads all rows within the given table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        public ITableRowReader OpenReader(string tableName)
        {
            for (int loop = 0; loop < m_sheetInfos.Count; loop++)
            {
                SheetInformation actSheetInfo = m_sheetInfos[loop];
                if (actSheetInfo.Name == tableName) 
                {
                    OpenXmlReader reader = OpenXmlReader.Create(actSheetInfo.WorksheetPart);
                    return new XlsxTableRowReader(this, actSheetInfo.HeaderRow, reader, ReadRowValues(reader, actSheetInfo));
                }
            }

            throw new SeeingSharpException(string.Format("Table {0} not found in document!", tableName));
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public void Dispose()
        {
            CommonTools.SafeDispose(ref m_spreadsheetDocument);
        }

        /// <summary>
        /// Starts reading from the given Sheet.
        /// </summary>
        /// <param name="sheetInfo"></param>
        private IEnumerable<List<string>> ReadRowValues(OpenXmlReader reader, SheetInformation sheetInfo)
        {
            int initialCount = 10;
            if (sheetInfo.HeaderRow != null) { initialCount = sheetInfo.HeaderRow.FieldCount; }

            List<string> currentRow = null;
            int actRowIndex = -1;
            while (reader.Read())
            {
                if ((reader.ElementType == typeof(Row)) &&
                   (reader.IsStartElement))
                {
                    if (currentRow != null) { yield return currentRow; }
                    currentRow = new List<string>(initialCount);
                    actRowIndex++;
                }
                else if ((reader.ElementType == typeof(Cell)) &&
                        (reader.IsStartElement))
                {
                    Cell actCell = (Cell)reader.LoadCurrentElement();
                    string actCellValue = ReadCellValue(actCell);
                    currentRow.Add(actCellValue);
                }
            }

            if (currentRow != null) { yield return currentRow; }
        }

        /// <summary>
        /// Reads the value within the given cell.
        /// </summary>
        /// <param name="actCell">The cell to read into memory.</param>
        private string ReadCellValue(Cell actCell)
        {
            string actCellValue = actCell.CellValue.InnerText;
            if (actCell.DataType != null)
            {
                switch (actCell.DataType.Value)
                {
                    case CellValues.SharedString:
                        int sharedStringID = Int32.Parse(actCellValue);
                        if (m_sharedStrings.Count <= sharedStringID) { throw new SeeingSharpException(string.Format("SharedString ID {0} not found in document!", sharedStringID)); }
                        actCellValue = m_sharedStrings[sharedStringID];
                        break;

                    default:
                        break;

                }
            }
            return actCellValue;
        }

        /// <summary>
        /// Loads all needed information about the sheets within the xlsx document.
        /// </summary>
        private void LoadSheetInformation()
        {
            // Look for all worksheet references
            m_sheetInfos = new List<SheetInformation>(10);
            WorkbookPart wbPart = m_spreadsheetDocument.WorkbookPart;
            using (OpenXmlReader reader = OpenXmlReader.Create(wbPart))
            {
                while (reader.Read())
                {
                    if ((reader.ElementType == typeof(Sheet)) &&
                        (reader.IsStartElement))
                    {
                        SheetInformation actSheetInfo = new SheetInformation();
                        for (int loop = 0; loop < reader.Attributes.Count; loop++)
                        {
                            OpenXmlAttribute actAttrib = reader.Attributes[loop];
                            if (actAttrib.LocalName == ATTRIBUTE_SHEET_NAME)
                            {
                                actSheetInfo.Name = actAttrib.Value;
                            }
                            else if (actAttrib.LocalName == ATTRIBUTE_SHEET_ID)
                            {
                                actSheetInfo.ID = Int32.Parse(actAttrib.Value);
                            }
                        }
                        m_sheetInfos.Add(actSheetInfo);
                    }
                }
            }

            // Look for all WorksheetParts
            for(int loop=0 ; loop<m_sheetInfos.Count; loop++)
            {
                SheetInformation actSheetInfo = m_sheetInfos[loop];

                // Define the file name which we search 
                string sheetFileName = string.Format(FILENAME_SHEET, actSheetInfo.ID);

                // Search for the correct WorksheetPart
                foreach (WorksheetPart actWorksheetPart in wbPart.WorksheetParts)
                {
                    if (actWorksheetPart.Uri.OriginalString.EndsWith(sheetFileName))
                    {
                        actSheetInfo.WorksheetPart = actWorksheetPart;
                        m_sheetInfos[loop] = actSheetInfo;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Loads all shared string contained in the workbook.
        /// </summary>
        private void LoadSharedStrings()
        {
            m_sharedStrings = new List<string>(1024);

            WorkbookPart wbPart = m_spreadsheetDocument.WorkbookPart;
            SharedStringTablePart sharedStringsPart = wbPart.SharedStringTablePart;
            using(OpenXmlReader reader = OpenXmlReader.Create(sharedStringsPart))
            {
                while(reader.Read())
                {
                    if((reader.ElementType == typeof(Text)) &&
                       (reader.IsStartElement))
                    {
                        m_sharedStrings.Add(reader.GetText());
                    }
                }
            }
        }

        /// <summary>
        /// Loads all header rows.
        /// </summary>
        private void LoadHeaderRows()
        {
            for (int loop = 0; loop < m_sheetInfos.Count; loop++)
            {
                SheetInformation actSheetInfo = m_sheetInfos[loop];
                using (OpenXmlReader reader = OpenXmlReader.Create(actSheetInfo.WorksheetPart))
                {
                    int actRowIndex = -1;
                    foreach(List<string> actRowData in ReadRowValues(reader, actSheetInfo))
                    {
                        actRowIndex++;

                        if (actRowIndex == m_importerConfig.HeaderRowIndex)
                        {
                            actSheetInfo.HeaderRow = new XlsxTableHeaderRow(this, actRowData);
                            m_sheetInfos[loop] = actSheetInfo;
                            break;
                        }
                    }
                    if (actSheetInfo.HeaderRow == null) { throw new SeeingSharpException("Header row not found!"); }
                }
            }
        }

        public XlsxImporterConfig ImporterConfig
        {
            get { return m_importerConfig; }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        private struct SheetInformation
        {
            public int ID;
            public string Name;
            public XlsxTableHeaderRow HeaderRow;
            public WorksheetPart WorksheetPart;
        }
    }
}
