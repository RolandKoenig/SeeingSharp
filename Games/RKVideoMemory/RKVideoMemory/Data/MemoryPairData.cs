using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory.Data
{
    public class MemoryPairData 
    {
        private List<string> m_childImageFiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPairData"/> class.
        /// </summary>
        /// <param name="title">The title of this pari.</param>
        internal MemoryPairData(string title)
        {
            this.Title = title;

            m_childImageFiles = new List<string>();
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Title)) { return this.Title; }
            return base.ToString();
        }

        /// <summary>
        /// Determines whether this MemoryPairData is valid for the game.
        /// </summary>
        internal bool IsValidPair()
        {
            return
                (!string.IsNullOrEmpty(this.TitleFile)) &&
                (m_childImageFiles.Count > 0);
        }

        /// <summary>
        /// Processes the given image file.
        /// </summary>
        /// <param name="filePath">The path to the image file.</param>
        public void ProcessImageFile(string filePath)
        {
            if (Path.GetFileNameWithoutExtension(filePath).Equals(Constants.TITLE_IMAGE_NAME, StringComparison.OrdinalIgnoreCase))
            {
                // We are loading the title image
                this.TitleFile = filePath;
            }
            else
            {
                // We are loading a image for the image sequence
                m_childImageFiles.Add(filePath);
            }
        }

        public string Title
        {
            get;
            private set;
        }

        public string TitleFile
        {
            get;
            private set;
        }

        public List<string> ChildImageFilePaths
        {
            get { return m_childImageFiles; }
        }
    }
}
