using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util.TableData
{
    public class XlsxImporterConfig : TableImporterConfig
    {
        public XlsxImporterConfig()
        {
            this.HeaderRowIndex = 0;
            this.FirstValueRowIndex = 1;
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
    }
}
