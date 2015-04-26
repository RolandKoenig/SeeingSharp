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

using SeeingSharp.Util;

namespace SeeingSharp.Multimedia.Objects
{
    public interface IModelImporter
    {
        /// <summary>
        /// Imports a model from the given file.
        /// </summary>
        /// <param name="importOptions">Some configuration for the importer.</param>
        /// <param name="sourceFile">The source file to be loaded.</param>
        ImportedModelContainer ImportModel(ResourceLink sourceFile, ImportOptions importOptions);

        /// <summary>
        /// Creates a default import options object for this importer.
        /// </summary>
        ImportOptions CreateDefaultImportOptions();
    }
}
