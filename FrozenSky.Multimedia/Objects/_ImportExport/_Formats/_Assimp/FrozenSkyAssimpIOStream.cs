//#region License information (FrozenSky and all based games/applications)
///*
//    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
//    Copyright (C) 2014 Roland König (RolandK)

//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/.
//*/
//#endregion

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FrozenSky.Multimedia.Objects
//{
//    internal class FrozenSkyAssimpIOStream : Assimp.IOStream
//    {
//        private Stream m_stream;

//        public FrozenSkyAssimpIOStream(Stream stream, string filePath, Assimp.FileIOMode fileMode)
//            : base(filePath, fileMode)
//        {
//            m_stream = stream;
//        }

//        public override void Flush()
//        {
//            m_stream.Flush();
//        }

//        public override long GetFileSize()
//        {
//            return m_stream.Length;
//        }

//        public override long GetPosition()
//        {
//            return m_stream.Position;
//        }

//        public override long Read(byte[] dataRead, long count)
//        {
//            throw new NotImplementedException();
//        }

//        public override Assimp.ReturnCode Seek(long offset, Assimp.Origin seekOrigin)
//        {
//            throw new NotImplementedException();
//        }

//        public override long Write(byte[] dataToWrite, long count)
//        {
//            throw new NotImplementedException();
//        }

//        public override bool IsValid
//        {
//            get { return true; }
//        }
//    }
//}
