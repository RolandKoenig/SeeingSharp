#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
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
using FrozenSky.Multimedia.Core;
using FrozenSky.Util;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;
using MF = SharpDX.MediaFoundation;

namespace FrozenSky.Multimedia.DrawingVideo
{
    /// <summary>
    /// This VideoWriter is responsible for writing mp4 files to hard disc.
    /// For details see tutorial at: https://msdn.microsoft.com/en-us/library/windows/desktop/ff819477(v=vs.85).aspx
    /// </summary>
    public class Mp4VideoWriter : MediaFoundationVideoWriter
    {
        private static readonly Guid VIDEO_ENCODING_FORMAT = MF.VideoFormatGuids.H264;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mp4VideoWriter"/> class.
        /// </summary>
        /// <param name="targetFile">The target file to write to.</param>
        public Mp4VideoWriter(ResourceLink targetFile)
            : base(targetFile)
        {

        }

        /// <summary>
        /// Creates a media target.
        /// </summary>
        /// <param name="sinkWriter">The previously created SinkWriter.</param>
        /// <param name="videoPixelSize">The pixel size of the video.</param>
        /// <param name="streamIndex">The stream index for the new target.</param>
        protected override void CreateMediaTarget(MF.SinkWriter sinkWriter, Size2 videoPixelSize, out int streamIndex)
        {
            using (MF.MediaType mediaTypeOut = new MF.MediaType())
            {
                mediaTypeOut.Set<Guid>(MF.MediaTypeAttributeKeys.MajorType, MF.MediaTypeGuids.Video);
                mediaTypeOut.Set<Guid>(MF.MediaTypeAttributeKeys.Subtype, VIDEO_ENCODING_FORMAT);
                mediaTypeOut.Set<int>(MF.MediaTypeAttributeKeys.AvgBitrate, base.Bitrate * 1000);
                mediaTypeOut.Set<int>(MF.MediaTypeAttributeKeys.InterlaceMode, (int)MF.VideoInterlaceMode.Progressive);
                mediaTypeOut.Set<long>(MF.MediaTypeAttributeKeys.FrameSize, MFHelper.GetMFEncodedIntsByValues(videoPixelSize.Width, videoPixelSize.Height));
                mediaTypeOut.Set<long>(MF.MediaTypeAttributeKeys.FrameRate, MFHelper.GetMFEncodedIntsByValues(base.Framerate, 1));
                sinkWriter.AddStream(mediaTypeOut, out streamIndex);
            }
        }

        /// <summary>
        /// Internal use: FlipY during rendering?
        /// </summary>
        protected override bool FlipY
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a dummy video file name, for example Dummy.mp4.
        /// This is needed to pass a dummy name to MediaFoundation.
        /// </summary>
        protected override string DummyFileName
        {
            get { return "Dummy.mp4"; }
        }
    }
}
