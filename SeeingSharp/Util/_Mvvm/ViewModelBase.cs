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

using SeeingSharp.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using System.Collections;

namespace SeeingSharp.Util
{
    public abstract class ViewModelBase : PropertyChangedBase, INotifyDataErrorInfo
#if DESKTOP
        ,IDataErrorInfo
#endif
    {
        #region Members for error reporting
        private string m_commonDataError;
        private Dictionary<string, string> m_specificDataErrors;
        private bool m_suppressChangeReports;
        private bool m_raisePropertyChangedOnError;
        #endregion

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        public ViewModelBase()
        {
#if DESKTOP
            m_raisePropertyChangedOnError = true;
#else
            m_raisePropertyChangedOnError = false;
#endif
        }

        /// <summary>
        /// Is there an error on the given property?
        /// </summary>
        /// <param name="getPropertyExpression">An expression that points to the target property.</param>
        public bool HasPropertyError<T>(Expression<Func<T>> getPropertyExpression)
        {
            return HasPropertyError(base.GetMemberName(getPropertyExpression));
        }

        /// <summary>
        /// Is there an error on the given property?
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public bool HasPropertyError(string propertyName)
        {
            if ((m_specificDataErrors != null) && m_specificDataErrors.ContainsKey(propertyName)) { return true; }
            return false;
        }

        /// <summary>
        /// Is there any error on one of the properties of this object?
        /// </summary>
        public bool HasPropertyError()
        {
            if ((m_specificDataErrors != null) && (m_specificDataErrors.Count > 0)) { return true; }
            return false;
        }

        /// <summary>
        /// Gets total count of property errors.
        /// </summary>
        public int GetPropertyErrorCount()
        {
            int result = 0;
            if ((m_specificDataErrors != null) && (m_specificDataErrors.Count > 0)) { result += m_specificDataErrors.Count; }
            return result;
        }

        /// <summary>
        /// Formats the given string value.
        /// </summary>
        /// <param name="value">The value to format.</param>
        public string FormatString(string value)
        {
            if (string.IsNullOrEmpty(value)) { return string.Empty; }
            else { return value; }
        }

        /// <summary>
        /// Clears all errors.
        /// </summary>
        public void ClearErrors()
        {
            if(m_specificDataErrors == null) { return; }

            Dictionary<string, string> prevDataErrors = m_specificDataErrors;
            m_specificDataErrors = null;

            if (!m_suppressChangeReports)
            {
                foreach (string actErrorProperty in prevDataErrors.Keys)
                {
                    if (m_raisePropertyChangedOnError)
                    {
                        this.RaisePropertyChanged(actErrorProperty);
                    }

                    this.ErrorsChanged.Raise(this, new DataErrorsChangedEventArgs(actErrorProperty));
                }
                this.RaisePropertyChanged(nameof(Error));
            }
        }

        /// <summary>
        /// Validates this DataObject.
        /// </summary>
        public bool Validate()
        {
            return this.Validate(null);
        }

        /// <summary>
        /// Validates this DataObject.
        /// </summary>
        /// <param name="dataCore">Corresponding DataCore (may be null).</param>
        public bool Validate(object dataCore)
        {
            Dictionary<string, object> changedProperties = new Dictionary<string, object>();

            bool suppressChanges = this.SuppressChangeReports;
            try
            {
                this.SuppressChangeReports = true;

                m_commonDataError = string.Empty;
                if (m_specificDataErrors != null) { foreach (string actKey in m_specificDataErrors.Keys) { changedProperties[actKey] = null; } }
                m_specificDataErrors = null;

                //Perform validation
                try
                {
                    //Perform standard attribute based validation
                    foreach (DataObjectValidationError actError in DataObjectValidator.Validate(this))
                    {
                        this.SetDataError(actError.PropertyInternalName, actError.ErrorMessage);
                    }

                    //Perform custom validation defined by super class
                    PerformValidation(dataCore);
                }
                finally
                {
                    //Override common data error if not set already
                    if ((m_specificDataErrors != null) &&
                        (string.IsNullOrEmpty(m_commonDataError)))
                    {
                        StringBuilder commonErrorBuilder = new StringBuilder();
                        commonErrorBuilder.Append("<b>" + Translatables.VALIDATION_ERRORS + "</b> ");
                        int actCount = 0;
                        List<string> propertiesWithErrors = new List<string>();
                        if (m_specificDataErrors != null)
                        {
                            foreach (KeyValuePair<string, string> actPair in m_specificDataErrors)
                            {
                                if (!propertiesWithErrors.Contains(actPair.Key)) { propertiesWithErrors.Add(actPair.Key); }

                                //if (actCount > 0) { commonErrorBuilder.Append(", "); }
                                commonErrorBuilder.Append("\n");
                                commonErrorBuilder.Append("<u>" + GetMemberDisplayName(actPair.Key) + "</u>");
                                commonErrorBuilder.Append(": " + actPair.Value);
                                actCount++;
                            }
                        }

                        //Notify UI of found data errors
                        this.Error = commonErrorBuilder.ToString();
                        foreach (string actPropertyWithError in propertiesWithErrors)
                        {
                            changedProperties[actPropertyWithError] = null;
                        }
                    }
                }
            }
            finally
            {
                this.SuppressChangeReports = suppressChanges;
            }

            // Raise changed error information
            RaisePropertyChanged(() => Error);
            foreach (string actChangedProperty in changedProperties.Keys)
            {
                if (m_raisePropertyChangedOnError)
                {
                    RaisePropertyChanged(actChangedProperty);
                }
                this.ErrorsChanged.Raise(this, new DataErrorsChangedEventArgs(actChangedProperty));
            }

            return !this.ContainsErrors;
        }

        /// <summary>
        /// Performs validation (use this[ColumnName] to get/set data errors).
        /// </summary>
        /// <param name="dataCore">Corresponding DataCore (may be null).</param>
        protected virtual void PerformValidation(object dataCore)
        {
        }

        /// <summary>
        /// Gets the error string of the given property name.
        /// </summary>
        /// <param name="getPropertyFunc">A function that points to the property.</param>
        public string GetPropertyError<T>(Expression<Func<T>> getPropertyFunc)
        {
            return GetPropertyError(GetMemberName(getPropertyFunc));
        }

        /// <summary>
        /// Gets the error string of the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public string GetPropertyError(string propertyName)
        {
            if ((m_specificDataErrors != null) && m_specificDataErrors.ContainsKey(propertyName))
            {
                return m_specificDataErrors[propertyName];
            }
            return string.Empty;
        }

        /// <summary>
        /// Sets the given error string for the given property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="error">The error to set.</param>
        protected void SetDataError(string propertyName, string error)
        {
            string givenValue = FormatString(error);
            if (!string.IsNullOrEmpty(givenValue))
            {
                //Add mode
                if (m_specificDataErrors == null) { m_specificDataErrors = new Dictionary<string, string>(); }
                m_specificDataErrors[propertyName] = givenValue;

                if (m_raisePropertyChangedOnError)
                {
                    RaisePropertyChanged(propertyName);
                }
                ErrorsChanged.Raise(this, new DataErrorsChangedEventArgs(propertyName));
                RaisePropertyChanged(() => ContainsErrors);
            }
            else
            {
                //Remove mode
                if (m_specificDataErrors != null)
                {
                    if (m_specificDataErrors.ContainsKey(propertyName))
                    {
                        m_specificDataErrors.Remove(propertyName);
                        if (m_specificDataErrors.Count == 0) { m_specificDataErrors = null; }
                        if (m_raisePropertyChangedOnError)
                        {
                            RaisePropertyChanged(propertyName);
                        }
                        ErrorsChanged.Raise(this, new DataErrorsChangedEventArgs(propertyName));
                        RaisePropertyChanged(() => ContainsErrors);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the given error string for the given property.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="getPropertyExpression">An expression describing the property.</param>
        /// <param name="error">The error to set.</param>
        protected void SetDataError<T>(Expression<Func<T>> getPropertyExpression, string error)
        {
            SetDataError(GetMemberName(getPropertyExpression), error);
        }

        public IEnumerable GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Suppress all change reports (calls to PropertyChanged event)?
        /// </summary>
        public bool SuppressChangeReports
        {
            get
            {
                return m_suppressChangeReports;
            }
            set
            {
                if (m_suppressChangeReports != value)
                {
                    m_suppressChangeReports = value;
                    RaisePropertyChanged(() => SuppressChangeReports);
                }
            }
        }

        /// <summary>
        /// Does this model contain any error?
        /// </summary>
        public bool ContainsErrors
        {
            get
            {
                if (!string.IsNullOrEmpty(m_commonDataError)) { return true; }
                if (m_specificDataErrors != null) { return true; }
                return false;
            }
        }

        /// <summary>
        /// Gets a common data error text for this model.
        /// </summary>
        public string Error
        {
            get
            {
                return m_commonDataError;
            }
            protected set
            {
                string givenValue = FormatString(value);
                if (m_commonDataError != givenValue)
                {
                    m_commonDataError = givenValue;
                    RaisePropertyChanged(() => Error);
                    RaisePropertyChanged(() => ContainsErrors);
                }
            }
        }

        /// <summary>
        /// Gets a common data error without html tags.
        /// </summary>
        public string ErrorWithoutHtml
        {
            get
            {
                string result = this.Error;
                if (result == null) { return string.Empty; }
                else { return result.Replace("<b>", "").Replace("</b>", "").Replace("<u>", "").Replace("</u>", ""); }
            }
        }

#if DESKTOP
        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified column name.
        /// This part of the interface IDataErrorInfo is implemented explicit because
        /// it may be a misunderstandig to access an error by an indexer.
        /// </summary>
        [Browsable(false)]
        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if (m_specificDataErrors == null) { return string.Empty; }
                else if (m_specificDataErrors.ContainsKey(columnName)) { return m_specificDataErrors[columnName]; }
                else { return string.Empty; }
            }
        }
#endif

        /// <summary>
        /// Gets the Messenger of the UI thread.
        /// </summary>
        public SeeingSharpMessenger Messenger
        {
            get { return SeeingSharpApplication.Current.UIMessenger; }
        }

        public bool HasErrors
        {
            get { return this.HasPropertyError(); }
        }

        public bool RaisePropertyChangedOnError
        {
            get { return m_raisePropertyChangedOnError; }
            set { m_raisePropertyChangedOnError = value; }
        }
    }
}