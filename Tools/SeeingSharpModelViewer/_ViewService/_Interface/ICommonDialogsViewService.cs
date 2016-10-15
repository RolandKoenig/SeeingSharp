#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# Tools. More info at
     - https://github.com/RolandKoenig/SeeingSharp/tree/master/Tools (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)

	This program is distributed under the terms of the Microsoft Public License (Ms-PL)-
	More info at https://msdn.microsoft.com/en-us/library/ff647676.aspx
*/
#endregion License information (SeeingSharp and all based games/applications)

using SeeingSharp.Util;
using SeeingSharp.View;

namespace SeeingSharpModelViewer
{
    public interface ICommonDialogsViewService : IViewService
    {
        ResourceLink PickFileByDialog(string fileFilter);
    }
}