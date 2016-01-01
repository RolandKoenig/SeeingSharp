#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
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
using SeeingSharp;
using SeeingSharp.Multimedia.Drawing2D;
using SeeingSharp.Util;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKRocket
{
    /// <summary>
    /// A static class which defines all resources used by RKRocket.
    /// </summary>
    internal static class GraphicsResources
    {
        #region Brushes
        public static readonly BrushResource Brush_Background = new LinearGradientBrushResource(
            new Vector2(0f, 0f),
            new Vector2(0f, 1f),
            new GradientStop[]
            {
                new GradientStop(new Color4(117,198,218), 0f),
                new GradientStop(Color4.LightBlue, 1)
            });
        public static readonly BrushResource Brush_Fragment = new SolidBrushResource(Color4.Gray.ChangeAlphaTo(0.3f));
        #endregion

        #region Bitmaps
        public static readonly StandardBitmapResource Bitmap_StarGray = new StandardBitmapResource(
            new AssemblyResourceUriBuilder(
                "RKRocket", true,
                "Assets/Bitmaps/StarGray_128x128.png"));
        public static readonly StandardBitmapResource Bitmap_Player = new StandardBitmapResource(
            new AssemblyResourceUriBuilder(
                "RKRocket", true,
                "Assets/Bitmaps/Rocket_205x329.png"));
        public static readonly StandardBitmapResource Bitmap_Projectile = new StandardBitmapResource(
            new AssemblyResourceUriBuilder(
                "RKRocket", true,
                "Assets/Bitmaps/BulletYellow_128x128.png"));

        // All possible block colors within an array of bitmaps
        public static readonly StandardBitmapResource[] Bitmap_Blocks = new StandardBitmapResource[]
        {
            new StandardBitmapResource(
                new AssemblyResourceUriBuilder(
                    "RKRocket", true,
                    "Assets/Bitmaps/BlockBlue_128x44.png")),
            new StandardBitmapResource(
                new AssemblyResourceUriBuilder(
                    "RKRocket", true,
                    "Assets/Bitmaps/BlockYellow_128x44.png")),
            new StandardBitmapResource(
                new AssemblyResourceUriBuilder(
                    "RKRocket", true,
                    "Assets/Bitmaps/BlockRed_128x44.png")),
        };
        #endregion

    }
}
