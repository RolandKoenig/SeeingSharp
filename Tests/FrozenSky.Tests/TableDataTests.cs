using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Util;
using FrozenSky.Util.TableData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FrozenSky.Tests
{
    [TestClass]
    public class TableDataTests
    {
        public const string TEST_CATEGORY = "FrozenSky TableData";

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void Load_DummyData_Csv()
        {
            ResourceSource tableSource = new AssemblyResourceLink(
                typeof(TableDataTests), "Resources.TableData.DummyData.csv");

            CsvTableImporter tableImporter = new CsvTableImporter();
            Load_DummyData_GenericPart(tableSource, tableImporter, tableImporter.CreateDefaultConfig(tableSource), "CSV");
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void Load_DummyData_Txt()
        {
            ResourceSource tableSource = new AssemblyResourceLink(
                typeof(TableDataTests), "Resources.TableData.DummyData.txt");

            CsvTableImporter tableImporter = new CsvTableImporter();
            Load_DummyData_GenericPart(tableSource, tableImporter, tableImporter.CreateDefaultConfig(tableSource), "CSV");
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void Load_DummyData_Xslx()
        {
            ResourceSource tableSource = new AssemblyResourceLink(
                typeof(TableDataTests), "Resources.TableData.DummyData.xlsx");

            XlsxTableImporter tableImporter = new XlsxTableImporter();
            Load_DummyData_GenericPart(tableSource, tableImporter, tableImporter.CreateDefaultConfig(tableSource), "Table_01");
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
            Assert.IsTrue(tableNames.Length > 0);
            Assert.IsTrue(tableNames[0] == firstTableName);
            Assert.IsNotNull(headerRow);
            Assert.IsTrue(headerRow.FieldCount == 7);
            Assert.IsTrue(headerRow.GetFieldName(1) == "TestCol_2");
            Assert.IsTrue(headerRow.GetFieldName(3) == "TestCol_4");
            Assert.IsTrue(loadedRows.Count == 20);
            Assert.IsTrue(loadedRows[4].ReadField<int>("TestCol_4") == 2202304);
        }
    }
}
