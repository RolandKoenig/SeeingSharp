using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FrozenSky.RKKinectLounge.Base
{
    [XmlType]
    public class FolderConfiguration
    {
        public static readonly FolderConfiguration Default = new FolderConfiguration();

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderConfiguration"/> class.
        /// </summary>
        public FolderConfiguration()
        {
            this.ViewType = string.Empty;
        }

        /// <summary>
        /// Gets or sets the type of the view.
        /// </summary>
        [XmlAttribute]
        public string ViewType
        {
            get;
            set;
        }
    }
}
