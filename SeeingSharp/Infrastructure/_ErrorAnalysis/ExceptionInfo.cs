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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Infrastructure
{
    public class ExceptionInfo : ExceptionInfoNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionInfo"/> class.
        /// </summary>
        public ExceptionInfo()
            : base(typeof(ExceptionInfo))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionInfo"/> class.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public ExceptionInfo(Exception ex)
            : base(ex.GetType())
        {
            this.MainMessage = ex.Message;

            // Read all available analyzers
            List<IExceptionAnalyzer> exceptionAnalyzer = 
                SeeingSharpApplication.Current.TypeQuery.GetAndInstanciateByContract<IExceptionAnalyzer>();

            // Analyze the given exception 
            AnalyzeException(ex, this, exceptionAnalyzer);
        }

        /// <summary>
        /// Analyzes the given exception.
        /// </summary>
        /// <param name="ex">The exception to be analyzed.</param>
        /// <param name="targetNode">The target node where to put all data to.</param>
        /// <param name="exceptionAnalyzers">All loaded analyzer objects.</param>
        private void AnalyzeException(Exception ex, ExceptionInfoNode targetNode, IEnumerable<IExceptionAnalyzer> exceptionAnalyzers)
        {
            // Query over all exception data
            Dictionary<Exception, ExceptionInfoNode> innerExceptions = new Dictionary<Exception,ExceptionInfoNode>();
            Dictionary<string, ExceptionProperty> properties = new Dictionary<string,ExceptionProperty>();
            foreach(IExceptionAnalyzer actAnalyzer in exceptionAnalyzers)
            {
                // Read all properties of the current exception
                foreach(ExceptionProperty actProperty in actAnalyzer.ReadExceptionInfo(ex))
                {
                    if (actProperty == null) { continue; }

                    if (string.IsNullOrEmpty(actProperty.Name)) { continue; }
                    properties[actProperty.Name] = actProperty;
                }

                // Read all inner exception information
                foreach (Exception actInnerException in actAnalyzer.GetInnerExceptions(ex))
                {
                    if (actInnerException == null) { continue; }

                    ExceptionInfoNode actInfoNode = new ExceptionInfoNode(actInnerException.GetType());
                    AnalyzeException(actInnerException, actInfoNode, exceptionAnalyzers);
                    innerExceptions[actInnerException] = actInfoNode;
                }
            }

            // Fill data of current node with all found data
            targetNode.Properties.AddRange(properties.Values);
            targetNode.InnerExceptionNodes.AddRange(innerExceptions.Values);
        }

        /// <summary>
        /// Gets or sets the main message.
        /// </summary>
        public string MainMessage
        {
            get;
            set;
        }
    }
}
