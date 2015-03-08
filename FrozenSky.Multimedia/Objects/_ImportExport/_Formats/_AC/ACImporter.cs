#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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

using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(FrozenSky.Multimedia.Objects.ACImporter),
    contractType: typeof(FrozenSky.Multimedia.Objects.IModelImporter))]

namespace FrozenSky.Multimedia.Objects
{
    [SupportedFileFormat("ac", "AC3D native format")]
    public class ACImporter : IModelImporter
    {
        /// <summary>
        /// Imports a model from the given file.
        /// </summary>
        /// <param name="sourceFile">The source file to be loaded.</param>
        /// <param name="importOptions">Some configuration for the importer.</param>
        public ImportedModelContainer ImportModel(ResourceSource sourceFile, ImportOptions importOptions)
        {
            ImportedModelContainer result = new ImportedModelContainer(importOptions);

            // Get needed resource key
            NamedOrGenericKey resGeometry = GraphicsCore.GetNextGenericResourceKey();

            // Load and fill result object
            ObjectType objType = ACFileLoader.ImportObjectType(sourceFile);
            result.ImportedResources.Add(new ImportedResourceInfo(
                resGeometry,
                () => new GeometryResource(objType)));
            result.Objects.Add(new GenericObject(resGeometry));

            return result;
        }


        /// <summary>
        /// Creates a default import options object for this importer.
        /// </summary>
        public ImportOptions CreateDefaultImportOptions()
        {
            return new ImportOptions();
        }
    }
}
