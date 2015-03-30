using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Util;
using FrozenSky.Util.TableData;
using Xunit;

namespace FrozenSky.Tests
{
    public class TableDataTests
    {
        public const string TEST_CATEGORY = "FrozenSky TableData";

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void Load_DummyData_Csv()
        {
            ResourceSource tableSource = new AssemblyResourceLink(
                typeof(TableDataTests), "Resources.TableData.DummyData.csv");

            CsvTableImporter tableImporter = new CsvTableImporter();
            Load_DummyData_GenericPart(tableSource, tableImporter, tableImporter.CreateDefaultConfig(tableSource), "CSV");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void Load_DummyData_Txt()
        {
            ResourceSource tableSource = new AssemblyResourceLink(
                typeof(TableDataTests), "Resources.TableData.DummyData.txt");

            CsvTableImporter tableImporter = new CsvTableImporter();
            Load_DummyData_GenericPart(tableSource, tableImporter, tableImporter.CreateDefaultConfig(tableSource), "CSV");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void Load_DummyData_Xlsx()
        {
            ResourceSource tableSource = new AssemblyResourceLink(
                typeof(TableDataTests), "Resources.TableData.DummyData.xlsx");

            XlsxTableImporter tableImporter = new XlsxTableImporter();
            Load_DummyData_GenericPart(tableSource, tableImporter, tableImporter.CreateDefaultConfig(tableSource), "Table_01");
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void Load_DataWithFormulasAndLinks_Xlsx()
        {
            // Define data source
            ResourceSource tableSource = new AssemblyResourceLink(
                typeof(TableDataTests), "Resources.TableData.Excel_WithFormulasAndLinks.xlsx");

            // Import the excel file
            XlsxTableImporter tableImporter = new XlsxTableImporter();
            List<Tuple<string, string>> xlsxData = new List<Tuple<string, string>>();
            using(ITableFile tableFile = tableImporter.OpenTableFile(tableSource, tableImporter.CreateDefaultConfig(tableSource)))
            {
                using(ITableRowReader rowReader = tableFile.OpenReader("Table_01"))
                {
                    rowReader.ReadAllRows()
                        .ForEachInEnumeration((actRow) =>
                        {
                            xlsxData.Add(new Tuple<string, string>(
                                actRow.ReadFieldAsString(0),
                                actRow.ReadFieldAsString(1)));
                        });
                }
            }

            // Check all data that we've read from the excel sheet
            Assert.True(xlsxData.Count == 20);
            foreach(var actDataRow in xlsxData)
            {
                Assert.True(actDataRow.Item1 == actDataRow.Item2);
            }
            Assert.True(xlsxData[3].Item1 == "87");
        }

        /// <summary>
        /// Internal helper method for all "LoadSimple" tests.
        /// </summary>
        /// <param name="tableSource">The source of the table file.</param>
        /// <param name="tableImporter">The importer to be used.</param>
        private static void Load_DummyData_GenericPart(
            ResourceSource tableSource, 
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
