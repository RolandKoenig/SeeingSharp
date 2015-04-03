using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Util;

namespace RKVideoMemory.Data
{
    public class LevelData
    {
        private List<MemoryPairData> m_memoryPairs;

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelData"/> class.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
        private LevelData(string directoryName)
        {
            m_memoryPairs = new List<MemoryPairData>();
            foreach(string actSubdirectory in Directory.GetDirectories(directoryName))
            {
                MemoryPairData actMemoryPair = new MemoryPairData(
                    Path.GetFileName(actSubdirectory));

                // Select all file names within the directory (order by filename)
                IEnumerable<string> fileNames =
                    from actFile in Directory.GetFiles(actSubdirectory)
                    orderby Path.GetFileName(actFile)
                    select actFile;
                foreach(string actFilePath in fileNames)
                {
                    string actFileExtension = Path.GetExtension(actFilePath);

                    // Handle image files
                    if(Constants.SUPPORTED_IMAGE_FORMATS.ContainsString(actFileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        actMemoryPair.ProcessImageFile(actFilePath);
                    }
                }

                // Add the pair to the new object
                if(actMemoryPair.IsValidPair())
                {
                    m_memoryPairs.Add(actMemoryPair);
                }
            }
        }

        /// <summary>
        /// Loads the level from the given directory.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
        public static async Task<LevelData> FromDirectory(string directoryName)
        {
            LevelData result = null;
            await Task.Factory.StartNew(() => result = new LevelData(directoryName));
            return result;
        }

        /// <summary>
        /// Gets a collection containing all found MemoryPairs.
        /// </summary>
        public List<MemoryPairData> MemoryPairs
        {
            get { return m_memoryPairs; }
        }
    }
}
