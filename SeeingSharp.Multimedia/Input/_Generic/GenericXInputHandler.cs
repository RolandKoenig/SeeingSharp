using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Infrastructure;

// Namespace mappings
using XI = SharpDX.XInput;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.Multimedia.Input.GenericXInputHandler),
    contractType: typeof(SeeingSharp.Multimedia.Input.ISeeingSharpInputHandler))]

namespace SeeingSharp.Multimedia.Input
{
    internal class GenericXInputHandler : ISeeingSharpInputHandler
    {
        /// <summary>
        /// Gets a list containing all supported view types.
        /// Null means that all types are supported.
        /// </summary>
        public Type[] GetSupportedViewTypes()
        {
            return null;
        }

        /// <summary>
        /// Gets a list containing all supported camera types.
        /// Null means that all types are supported.
        /// </summary>
        public Type[] GetSupportedCameraTypes()
        {
            return null;
        }

        public SeeingSharpInputMode[] GetSupportedInputModes()
        {
            return null;
        }

        public void Start(object viewObject, object cameraObject)
        {
        }

        public void UpdateMovement()
        {
        }

        public void Stop()
        {
        }
    }
}