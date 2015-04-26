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
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace SeeingSharp.Util
{
    /// <summary>
    /// Base class of all messages sent and received through ApplicationMessenger class.
    /// </summary>
    public class SeeingSharpMessage
    {
        /// <summary>
        /// Gets a list containing all target threads for message routing.
        /// An empty list means that no routing logic applies.
        /// </summary>
        public string[] GetAsyncRoutingTargetThreads()
        {
            return GetAsyncRoutingTargetThreadsOfMessageType(this.GetType());
        }

#if DESKTOP
        /// <summary>
        /// Gets a documentation string for the given message type.
        /// </summary>
        /// <param name="messageType">The message type to get a documentation for.</param>
        public static string GetDocumentationString(Type messageType)
        {
            return GetDocumentationString(messageType, int.MaxValue);
        }

        /// <summary>
        /// Gets a documentation string for the given message type.
        /// </summary>
        /// <param name="messageType">The message type to get a documentation for.</param>
        /// <param name="maxColumnCount">Maximum count of columns for the generated text.</param>
        public static string GetDocumentationString(Type messageType, int maxColumnCount)
        {
            // Prepare variables
            string description = string.Empty;
            string[] propertyDescriptions = null;
            string[] usageExamples = null;

            // Query for description attribute
            MessageDescriptionAttribute actDescriptionAttribute = messageType.GetCustomAttribute<MessageDescriptionAttribute>();
            if (actDescriptionAttribute != null)
            {
                description = actDescriptionAttribute.Description;
            }

            // Query for property descriptions
            PropertyInfo[] properties = messageType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            propertyDescriptions = new string[properties.Length];
            for (int loop = 0; loop < properties.Length; loop++)
            {
                propertyDescriptions[loop] = GetMessagePropertyDescription(properties[loop]);
            }

            // Query for usage examples
            MessageUsageExampleAttribute[] usageExampleAttribs = messageType.GetCustomAttributes<MessageUsageExampleAttribute>().ToArray();
            usageExamples = new string[usageExampleAttribs.Length];
            for (int loop = 0; loop < usageExampleAttribs.Length; loop++)
            {
                usageExamples[loop] = usageExampleAttribs[loop].ExampleString;
            }

            // Build documentation string
            StringBuilder resultBuilder = new StringBuilder(1024);
            resultBuilder.Append(WrapText(description, maxColumnCount));
            if (resultBuilder.Length > 0)
            {
                resultBuilder.AppendLine();
                resultBuilder.AppendLine();
            }
            if ((propertyDescriptions != null) && (propertyDescriptions.Length > 0))
            {
                resultBuilder.AppendLine("Message properties:");
                foreach (string actProperty in propertyDescriptions.OrderBy((actString) => actString))
                {
                    resultBuilder.AppendLine(WrapText(actProperty, maxColumnCount, " - "));
                }
            }
            if ((usageExamples != null) && (usageExamples.Length > 0))
            {
                if (resultBuilder.Length > 0) { resultBuilder.AppendLine(); }
                resultBuilder.AppendLine("Possible usage scenarios:");
                foreach (string actUsageScenario in usageExamples.OrderBy((actString) => actString))
                {
                    resultBuilder.AppendLine(WrapText(actUsageScenario, maxColumnCount, " - "));
                }
            }

            // Remove last enter.
            if (resultBuilder.Length > 5)
            {
                resultBuilder.Remove(resultBuilder.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            }

            return resultBuilder.ToString();
        }

        /// <summary>
        /// Generates the documentation for the given message property.
        /// </summary>
        /// <param name="propertyInfo">The property information object.</param>
        public static string GetMessagePropertyDescription(PropertyInfo propertyInfo)
        {
            StringBuilder actPropertyStringBuilder = new StringBuilder(256);

            // Append header (In/Out)
            actPropertyStringBuilder.Append("(");
            if ((propertyInfo.GetMethod != null) && (propertyInfo.GetMethod.IsPublic)) { actPropertyStringBuilder.Append("In"); }
            if ((propertyInfo.SetMethod != null) && (propertyInfo.SetMethod.IsPublic))
            {
                if (actPropertyStringBuilder.Length > 1) { actPropertyStringBuilder.Append('/'); }
                actPropertyStringBuilder.Append("Out");
            }
            actPropertyStringBuilder.Append(")");

            // Append property name
            actPropertyStringBuilder.Append(" " + propertyInfo.Name);

            // Append description
            MessagePropertyDescriptionAttribute actPropertyDescAttrib = propertyInfo.GetCustomAttribute<MessagePropertyDescriptionAttribute>();
            if (actPropertyDescAttrib != null)
            {
                actPropertyStringBuilder.Append(": " + actPropertyDescAttrib.Description);
            }

            return actPropertyStringBuilder.ToString();
        }
#endif

        /// <summary>
        /// Gets a list containing all target threads for message routing.
        /// An empty list means that no routing logic applies.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        public static string[] GetAsyncRoutingTargetThreadsOfMessageType(Type messageType)
        {
            MessageAsyncRoutingTargetsAttribute routingAttrib = messageType.GetTypeInfo().GetCustomAttribute(
                typeof(MessageAsyncRoutingTargetsAttribute)) as MessageAsyncRoutingTargetsAttribute;
            if (routingAttrib != null) { return routingAttrib.AsyncTargetThreads; }
            else { return new string[0]; }
        }

        /// <summary>
        /// Gets a list containing all possible source threads of this message.
        /// An empty list means that every thread can fire this message
        /// </summary>
        public string[] GetPossibleSourceThreads()
        {
            return GetPossibleSourceThreadsOfMessageType(this.GetType());
        }

        /// <summary>
        /// Gets a list containing all possible source threads of this message.
        /// An empty list means that every thread can fire this message
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        public static string[] GetPossibleSourceThreadsOfMessageType(Type messageType)
        {
            MessagePossibleSourceAttribute routingAttrib = messageType.GetTypeInfo().GetCustomAttribute(
                typeof(MessagePossibleSourceAttribute)) as MessagePossibleSourceAttribute;
            if (routingAttrib != null) { return routingAttrib.PossibleSourceThreads; }
            else { return new string[0]; }
        }

        /// <summary>
        /// Helper method which wraps the given text for display within documentation string.
        /// </summary>
        /// <param name="text">The text to be wrapped.</param>
        /// <param name="maxCharactersPerLine">The maximum count of characters in one line.</param>
        /// <param name="lineHeader">The line header to append on the beginning.</param>
        private static string WrapText(string text, int maxCharactersPerLine, string lineHeader = "")
        {
            string[] originalLines = text.Split(
                new string[] { " " },
                StringSplitOptions.RemoveEmptyEntries);

            // Prepare line
            StringBuilder actualLine = new StringBuilder();
            string lineHeaderNextLines = "";
            if (!string.IsNullOrEmpty(lineHeader))
            {
                lineHeaderNextLines = new string(' ', lineHeader.Length);
                actualLine.Append(lineHeader);
            }

            // Generate the list containing all wrapped lines
            List<string> wrappedLines = new List<string>();
            int totalCharacterCount = 0;
            foreach (var item in originalLines)
            {
                if (actualLine.Length == 0)
                {
                    actualLine.Append(lineHeaderNextLines);
                }

                actualLine.Append(item.Replace("\r", "").Replace("\n", "") + " ");
                if (actualLine.Length > maxCharactersPerLine)
        {
                    wrappedLines.Add(actualLine.ToString());
                    totalCharacterCount += actualLine.Length;
                    actualLine.Clear();
                }
        }

            if (actualLine.Length > 0)
        {
                wrappedLines.Add(actualLine.ToString());
                totalCharacterCount += actualLine.Length;
        }

            // Build result string
            StringBuilder concatenated = new StringBuilder(totalCharacterCount);
            for (int loop = 0; loop < wrappedLines.Count; loop++)
        {
                if (loop >= wrappedLines.Count - 1) { concatenated.Append(wrappedLines[loop]); }
                else { concatenated.AppendLine(wrappedLines[loop]); }
            }

            return concatenated.ToString();
        }
    }
}
