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
#if DESKTOP
using System;
using System.Reflection;
using System.Windows.Markup;

namespace SeeingSharp.Infrastructure
{
    public class SingletonExtension : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonExtension" /> class.
        /// </summary>
        public SingletonExtension()
        {
            this.Name = string.Empty;
            this.Type = typeof(object);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Try to get an existing singleton
            if (SeeingSharpApplication.IsInitialized)
            {
                if ((!string.IsNullOrEmpty(this.Name)) &&
                    (SeeingSharpApplication.Current.Singletons.ContainsSingleton(this.Name)))
                {
                    return SeeingSharpApplication.Current.Singletons[this.Name];
                }
                else if ((this.Type != null) &&
                        (SeeingSharpApplication.Current.Singletons.ContainsSingleton(this.Type)))
                {
                    return SeeingSharpApplication.Current.Singletons[this.Type];
                }
            }

            // Try to create an object of the requested type
            if (this.Type != null)
            {
                // Try to create testdata
                MethodInfo testDataMethod = this.Type.GetMethod("CreateTestDataForDesigner");
                if((testDataMethod != null) &&
                   (testDataMethod.GetParameters().Length == 0) &&
                   (testDataMethod.ReturnType == this.Type))
                {
                    return testDataMethod.Invoke(null, null);
                }

                // Try to create a default instance of the object
                ConstructorInfo standardConstructor = this.Type.GetConstructor(new Type[0]);
                if (standardConstructor != null)
                {
                    return Activator.CreateInstance(this.Type);
                }
            }

            //Try to create an object of the target property type
            IProvideValueTarget targetProperty = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (targetProperty != null)
            {
                PropertyInfo propertyInfo = targetProperty.TargetProperty as PropertyInfo;
                if (propertyInfo != null)
                {
                    ConstructorInfo standardConstructor = propertyInfo.PropertyType.GetConstructor(new Type[0]);
                    if (standardConstructor != null)
                    {
                        return Activator.CreateInstance(propertyInfo.PropertyType);
                    }
                }
            }

            //Unable to get or create the object, so return null here
            return null;
        }

        /// <summary>
        /// Gets or sets the name of the singleton.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the singleton.
        /// </summary>
        public Type Type
        {
            get;
            set;
        }
    }
}
#endif