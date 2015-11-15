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
using SeeingSharp.Util;
using SeeingSharp.Util.TableData;
using Xunit;

namespace SeeingSharp.Tests
{
    public class TableDataTests
    {
        public const string TEST_CATEGORY = "SeeingSharp TableData";

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void Load_DummyData_Csv()
        {
            ResourceLink tableSource = new AssemblyResourceLink(
                typeof(TableDataTests), "Resources.TableData.DummyData.csv");

            CsvTableImporter tableImporter = new CsvTableImporter();
            Load_DummyData_GenericPart(tableSource, tableImporter, tableImporter.CreateDefaultConfig(tableSource), "CSV");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void Load_DummyData_Txt()
        {
            ResourceLink tableSource = new AssemblyResourceLink(
                typeof(TableDataTests), "Resources.TableData.DummyData.txt");

            CsvTableImporter tableImporter = new CsvTableImporter();
            Load_DummyData_GenericPart(tableSource, tableImporter, tableImporter.CreateDefaultConfig(tableSource), "CSV");
        }

        /// <summary>
        /// Internal helper method for all "LoadSimple" tests.
        /// </summary>
        /// <param name="tableSource">The source of the table file.</param>
        /// <param name="tableImporter">The importer to be used.</param>
        private static void Load_DummyData_GenericPart(
            ResourceLink tableSource, 
            ITableImporter tableImporter, 
            TableImporterConfig importConfig,
            string firstTableName)
        {
            string[] tableNames = null;
            ITableHeaderRow headerRow = null;
            List<ITableRow> loadedRows = new List<ITableRow>();
            using (ITableFile tableFile = tableImporter.OpenTableFile(tableSource, importConfig))
            {
                tableNames = tableFile.GetTableNames().ToArray();
                if (tableNames.Contains(firstTableName))
                {
                    headerRow = tableFile.ReadHeaderRow(firstTableName);
                    using (ITableRowReader rowReader = tableFile.OpenReader(firstTableName))
                    {
                        loadedRows.AddRange(rowReader.ReadAllRows());
                    }
                }
            }

            // Do all checking
            Assert.True(tableNames.Length > 0);
            Assert.True(tableNames[0] == firstTableName);
            Assert.NotNull(headerRow);
            Assert.True(headerRow.FieldCount == 7);
            Assert.True(headerRow.GetFieldName(1) == "TestCol_2");
            Assert.True(headerRow.GetFieldName(3) == "TestCol_4");
            Assert.True(loadedRows.Count == 20);
            Assert.True(loadedRows[4].ReadField<int>("TestCol_4") == 2202304);
        }
    }
}
