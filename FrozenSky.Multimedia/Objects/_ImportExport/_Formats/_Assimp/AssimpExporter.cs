using FrozenSky.Infrastructure;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(FrozenSky.Multimedia.Objects.AssimpExporter),
    contractType: typeof(FrozenSky.Multimedia.Objects.IModelExporter))]

namespace FrozenSky.Multimedia.Objects
{
    [SupportedFileFormat("3ds", "3D Studio Max format")]
    [SupportedFileFormat("obj", "Wavefront Technologies format")]
    [SupportedFileFormat("x", "DirectX format")]
    [SupportedFileFormat("dae", "Collada")]
    public class AssimpExporter : IModelExporter
    {
        /// <summary>
        /// Exports the model(s) defined in the given model container to the given model file.
        /// </summary>
        /// <param name="modelContainer">The model(s) to export.</param>
        /// <param name="targetFile">The path to the target file.</param>
        /// <param name="exportOptions">Some configuration for the exporter.</param>
        public void ExportModelAsync(ImportedModelContainer modelContainer, ResourceSource targetFile, ExportOptions exportOptions)
        {
            
        }
    }
}
