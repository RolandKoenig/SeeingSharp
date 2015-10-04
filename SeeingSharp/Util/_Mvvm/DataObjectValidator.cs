#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Util
{
    public static class DataObjectValidator
    {
        /// <summary>
        /// Validates the given dataobject.
        /// </summary>
        /// <param name="dataObject">The object to be validated.</param>
        public static List<DataObjectValidationError> Validate(PropertyChangedBase dataObject)
        {
            if (dataObject == null) { throw new ArgumentNullException("dataObject"); }

            ValidationContext validationContext = new ValidationContext(dataObject);
            List<DataObjectValidationError> errors = new List<DataObjectValidationError>(4);
            List<ValidationResult> results = new List<ValidationResult>(4);
            if (!Validator.TryValidateObject(dataObject, validationContext, results, true))
            {
                foreach (ValidationResult actValidationResult in results)
                {
                    // Get the error message
                    string errorMessage = actValidationResult.ErrorMessage;

                    // Translate the property name within the message
                    string actPropertyName = actValidationResult.MemberNames.FirstOrDefault();
                    if ((!string.IsNullOrEmpty(actPropertyName)) &&
                        (errorMessage.Contains(actPropertyName)))
                    {
                        errorMessage = errorMessage.Replace(actPropertyName, dataObject.GetMemberDisplayName(actPropertyName));
                    }
                    if (string.IsNullOrEmpty(errorMessage)) { errorMessage = "Invalid value!"; }

                    // Register the detected data error
                    errors.Add(new DataObjectValidationError(actPropertyName, dataObject.GetMemberDisplayName(actPropertyName), errorMessage));
                }
            }

            return errors;
        }
    }
}
