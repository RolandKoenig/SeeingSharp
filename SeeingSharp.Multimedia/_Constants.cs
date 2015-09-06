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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia
{
    internal static class Constants
    {
        public const float MAX_FOCUS_DISTANCE = 15f;
        public const float MIN_FOCUS_DISTANCE = 3f;

        public const int ASYNC_LOADER_MAX_PRALLEL_TASK_COUNT = 2;

        #region View constants
        public const int MIN_VIEW_WIDTH = 32;
        public const int MIN_VIEW_HEIGHT = 32;
        #endregion

        #region Constants for time duration measuring
        public const string PERF_GLOBAL_PER_FRAME = "Graphics.Global.OneFrame";
        public const string PERF_GLOBAL_WAIT_TIME = "Graphics.Global.WaitTime";
        public const string PERF_GLOBAL_UPDATE_AND_PREPARE = "Graphics.Global.UpdateAndPrepare";
        public const string PERF_GLOBAL_UPDATE_SCENE = "Graphics.Global.Update (Scene: {0})";
        public const string PERF_GLOBAL_RENDER_AND_UPDATE_BESIDE = "Graphics.Global.RenderAndUpdateBeside";
        public const string PERF_GLOBAL_RENDER_DEVICE = "Graphics.Global.Render (Device: {0})";
        public const string PERF_GLOBAL_UPDATE_BESIDE = "Graphics.Global.UpdateBeside (Scene: {0})";
        public const string PERF_RENDERLOOP_PRESENT = "Graphics.RenderLoop.Present (Scene: {0}, View: {1})";
        public const string PERF_RENDERLOOP_RENDER = "Graphics.RenderLoop.Render (Scene: {0}, View: {1})";
        public const string PERF_RENDERLOOP_RENDER_2D = "Graphics.RenderLoop.Render.2D (Scene: {0}, View: {1})";
        #endregion

        #region Constants for occurences per timeunit measuring
        public static string PERF_GLOBAL_FRAMES_FINISHED = "Graphics.Global.FramesRendered";
        #endregion

        #region Constants for animation system
        public static readonly TimeSpan UPDATE_STATE_MAX_TIME = new TimeSpan(0, 0, 0, 0, Int32.MaxValue);
        #endregion
    }
}
