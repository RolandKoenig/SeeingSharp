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

using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Objects
{
    public class ImportedResourceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedResourceInfo"/> class.
        /// </summary>
        /// <param name="resourceKey">The key which is used by added the resource to a scene.</param>
        /// <param name="resourceFactory">The resource factory.</param>
        public ImportedResourceInfo(NamedOrGenericKey resourceKey, Func<Resource> resourceFactory)
        {
            this.ResourceKey = resourceKey;
            this.ResourceFactory = resourceFactory;
        }

        public override string ToString()
        {
            return this.ResourceKey.ToString();
        }

        public NamedOrGenericKey ResourceKey
        {
            get;
            private set;
        }

        public Func<Resource> ResourceFactory
        {
            get;
            private set;
        }
    }
}
