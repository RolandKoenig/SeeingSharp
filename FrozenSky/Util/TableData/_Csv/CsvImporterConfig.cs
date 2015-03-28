using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util.TableData
{
    public class CsvImporterConfig : TableImporterConfig
    {
        public CsvImporterConfig()
        {
            this.HeaderRowIndex = 0;
            this.FirstValueRowIndex = 1;
            this.Encoding = null;
            this.SeparationChar = ';';
        }

        public int HeaderRowIndex
        {
            get;
            set;
        }

        public int FirstValueRowIndex
        {
            get;
            set;
        }

        public Encoding Encoding
        {
            get;
            set;
        }

        public char SeparationChar
        {
            get;
            set;
        }
    }
}
