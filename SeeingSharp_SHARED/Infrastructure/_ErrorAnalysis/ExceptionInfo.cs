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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Infrastructure
{
    public class ExceptionInfo
    {
        private List<ExceptionInfoNode> m_childNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionInfo"/> class.
        /// </summary>
        public ExceptionInfo()
        {
            m_childNodes = new List<ExceptionInfoNode>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionInfo"/> class.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public ExceptionInfo(Exception ex)
            : this()
        {
            this.MainMessage = Translatables.ERROR_UNHANDLED_EX;
            this.Description = ex.Message;

            // Read all available analyzers
            List<IExceptionAnalyzer> exceptionAnalyzer = 
                SeeingSharpApplication.Current.TypeQuery.GetAndInstanciateByContract<IExceptionAnalyzer>();

            // Analyze the given exception 
            ExceptionInfoNode newNode = new ExceptionInfoNode(ex);
            m_childNodes.Add(newNode);
            AnalyzeException(ex, newNode, exceptionAnalyzer);
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
            foreach(IExceptionAnalyzer actAnalyzer in exceptionAnalyzers)
            {
                // Read all properties of the current exception
                foreach(ExceptionProperty actProperty in actAnalyzer.ReadExceptionInfo(ex))
                {
                    if (actProperty == null) { continue; }
                    if (string.IsNullOrEmpty(actProperty.Name)) { continue; }

                    ExceptionInfoNode propertyNode = new ExceptionInfoNode(actProperty);
                    targetNode.ChildNodes.Add(propertyNode);
                }

                // Read all inner exception information
                foreach (Exception actInnerException in actAnalyzer.GetInnerExceptions(ex))
                {
                    if (actInnerException == null) { continue; }

                    ExceptionInfoNode actInfoNode = new ExceptionInfoNode(actInnerException);
                    AnalyzeException(actInnerException, actInfoNode, exceptionAnalyzers);
                    targetNode.ChildNodes.Add(actInfoNode);
                }
            }

            // Sort all generated nodes
            targetNode.ChildNodes.Sort();
        }

        /// <summary>
        /// Gets a collection containing all subnodes.
        /// </summary>
        public List<ExceptionInfoNode> ChildNodes
        {
            get { return m_childNodes; }
        }

        /// <summary>
        /// Gets or sets the main message.
        /// </summary>
        public string MainMessage
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }
    }
}
