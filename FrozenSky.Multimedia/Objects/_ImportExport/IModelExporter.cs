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

using FrozenSky.Util;

namespace FrozenSky.Multimedia.Objects
{
    public interface IModelExporter
    {
        /// <summary>
        /// Exports the model(s) defined in the given model container to the given model file.
        /// </summary>
        /// <param name="modelContainer">The model(s) to export.</param>
        /// <param name="targetFile">The path to the target file.</param>
        /// <param name="exportOptions">Some configuration for the exporter.</param>
        void ExportModelAsync(ImportedModelContainer modelContainer, ResourceLink targetFile, ExportOptions exportOptions);
    }
}
