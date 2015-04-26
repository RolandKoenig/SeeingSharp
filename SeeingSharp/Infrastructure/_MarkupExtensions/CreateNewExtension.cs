#if DESKTOP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace SeeingSharp.Infrastructure
{
    public class CreateNewExtension : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateNewExtension"/> class.
        /// </summary>
        public CreateNewExtension()
        {

        }

        /// <summary>
        /// Gibt bei der Implementierung in einer abgeleiteten Klasse ein Objekt zurück, das als Wert der Zieleigenschaft für diese Markuperweiterung bereitgestellt wird.
        /// </summary>
        /// <param name="serviceProvider">Ein Dienstanbieter-Hilfsobjekt, das Dienste für die Markuperweiterung bereitstellen kann.</param>
        /// <returns>
        /// Der Objektwert, der für die Eigenschaft festgelegt werden soll, für die die Erweiterung angewendet wird.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                if (this.Type == null) { return null; }

                return Activator.CreateInstance(this.Type);
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the type of the object to be created.
        /// </summary>
        public Type Type
        {
            get;
            set;
        }
    }
}
#endif