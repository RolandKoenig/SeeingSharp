#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Infrastructure
{
    /// <summary>
    /// This interface is used by the error-reporting framework.
    /// It queries for all information provided by an exception which will be presentet to
    /// the user / developer.
    /// </summary>
    public interface IExceptionAnalyzer
    {
        /// <summary>
        /// Reads all exception information from the given exception object.
        /// </summary>
        /// <param name="ex">The exception to be analyzed.</param>
        IEnumerable<ExceptionProperty> ReadExceptionInfo(Exception ex);

        /// <summary>
        /// Gets all inner exceptions provided by the given exception object.
        /// </summary>
        /// <param name="ex">The exception to be analyzed.</param>
        IEnumerable<Exception> GetInnerExceptions(Exception ex);
    }
}
