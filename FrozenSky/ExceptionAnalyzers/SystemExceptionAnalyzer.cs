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
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(FrozenSky.ExceptionAnalyzers.SystemExceptionAnalyzer),
    contractType: typeof(FrozenSky.Infrastructure.IExceptionAnalyzer))]

namespace FrozenSky.ExceptionAnalyzers
{
    public class SystemExceptionAnalyzer : IExceptionAnalyzer
    {
        /// <summary>
        /// Reads all exception information from the given exception object.
        /// </summary>
        /// <param name="ex">The exception to be analyzed.</param>
        public IEnumerable<ExceptionProperty> ReadExceptionInfo(Exception ex)
        {
            // Write common properties
            yield return new ExceptionProperty("Type", ex.GetType().FullName);
            yield return new ExceptionProperty("HResult", ex.HResult.ToString());
            yield return new ExceptionProperty("HelpLink", ex.HelpLink);
            yield return new ExceptionProperty("Message", ex.Message);
            yield return new ExceptionProperty("Source", ex.Source);
            yield return new ExceptionProperty("StackTrace", ex.StackTrace);

            // Write infos about the source method
#if DESKTOP
            if (ex.TargetSite != null)
            {
                yield return new ExceptionProperty("TargetSite.Name", ex.TargetSite.Name);
                if (ex.TargetSite.DeclaringType != null)
                {
                    yield return new ExceptionProperty("TargetSite.DeclaringType", ex.TargetSite.DeclaringType.FullName);
                }
            }
#endif

            // Write Infos about a type load exception
            ReflectionTypeLoadException typeLoadException = ex as ReflectionTypeLoadException;
            if((typeLoadException != null) &&
               (typeLoadException.Types != null))
            {
                StringBuilder stringBuilder = new StringBuilder(1024);
                foreach(Type actType in typeLoadException.Types)
                {
                    stringBuilder.AppendLine(actType.FullName);
                }
                yield return new ExceptionProperty("Types.FullName", stringBuilder.ToString());
            }
        }

        /// <summary>
        /// Gets all inner exceptions provided by the given exception object.
        /// </summary>
        /// <param name="ex">The exception to be analyzed.</param>
        public IEnumerable<Exception> GetInnerExceptions(Exception ex)
        {
            // Return default inenr exception
            yield return ex.InnerException;

            // Query over all inner exceptions of an aggregate exception
            AggregateException aggregateException = ex as AggregateException;
            if(aggregateException != null)
            {
                foreach(Exception actInnerException in aggregateException.InnerExceptions)
                {
                    yield return actInnerException;
                }
            }

            // Query over all loader exceptions on a ReflectionTypeLoadException
            ReflectionTypeLoadException typeLoadException = ex as ReflectionTypeLoadException;
            if((typeLoadException != null) &&
               (typeLoadException.LoaderExceptions != null))
            {
                foreach(Exception actInner in typeLoadException.LoaderExceptions)
                {
                    yield return actInner;
                }
            }
        }
    }
}
