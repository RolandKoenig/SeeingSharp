#region License information
/*
    This sourcecode is extracted from System.Numerics.dll because
    System.Numerics.Vectors.dll is not available on the android platform.

    Hopefully it will come in the near future, so this a simple workaround
*/
#endregion

using System;

namespace System.Numerics
{
	internal static class Vector
	{
		public static bool IsHardwareAccelerated
		{
			get
			{
				return false;
			}
		}
	}
}
