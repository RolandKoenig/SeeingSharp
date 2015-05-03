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

using SeeingSharp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Bootstrapper.TranslationBootstrapper),
    contractType: typeof(SeeingSharp.Infrastructure.IBootstrapperItem))]

namespace SeeingSharp.Bootstrapper
{
    internal class TranslationBootstrapper : IBootstrapperItem
    {
        /// <summary>
        /// Executes the background action behind this item.
        /// </summary>
        /// <returns></returns>
        public async Task Execute()
        {
            // Load all translation data
            await SeeingSharpApplication.Current.Translator.QueryTranslationsAsync(
                SeeingSharpApplication.Current.AppAssemblies).ConfigureAwait(false);

            // Translate all translatable classes
            SeeingSharpApplication.Current.Translator.TranslateAllTranslatableClasses();
        }

        /// <summary>
        /// Gets a simple order id. The list of bootstrapper items will be sorted by this ID.
        /// </summary>
        public int OrderID
        {
            get { return SeeingSharpConstants.BOOTSTRAP_ORDERID_TRANSLATOR; }
        }

        /// <summary>
        /// Gets a short description of this item for the UI (e. g. for splash screens).
        /// </summary>
        public string Description
        {
            get { return Translatables.BOOTSTRAP_TRANSLATION; }
        }
    }
}
