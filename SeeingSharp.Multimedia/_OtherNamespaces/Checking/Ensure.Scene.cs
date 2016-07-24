using SeeingSharp.Multimedia.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Checking
{
    public static partial class EnsureMultimedia
    {
        [Conditional("DEBUG")]
        public static void EnsureObjectOfScene(
            this SceneObject sceneObject, Scene scene, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            if(sceneObject.Scene != scene)
            {
                throw new SeeingSharpCheckException(string.Format(
                    "The object {0} within method {1} is not part of the expected Scene!",
                    checkedVariableName, callerMethod));
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureObjectOfScene(
            this IEnumerable<SceneObject> sceneObjects, Scene scene, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            int actIndex = 0;
            foreach(SceneObject actObject in sceneObjects)
            {
                actObject.EnsureObjectOfScene(scene, $"{checkedVariableName}[{actIndex}]", callerMethod);
                actIndex++;
            }
        }
    }
}
