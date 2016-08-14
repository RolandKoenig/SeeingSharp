#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# Tools. More info at 
     - https://github.com/RolandKoenig/SeeingSharp/tree/master/Tools (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
	This program is distributed under the terms of the Microsoft Public License (Ms-PL)-
	More info at https://msdn.microsoft.com/en-us/library/ff647676.aspx
*/
#endregion
using SeeingSharp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharpModelViewer.ModelViewerBootstrapper),
    contractType: typeof(SeeingSharp.Infrastructure.IBootstrapperItem))]

namespace SeeingSharpModelViewer
{
    public class ModelViewerBootstrapper : IBootstrapperItem
    {

        public Task Execute()
        {
            return Task.Delay(100);
        }

        public string Description
        {
            get { return Localizables.BOOTSTRAPPER_NAME; }
        }

        public int OrderID
        {
            get { return 0; }
        }
    }
}
