using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.RKKinectLounge.Modules.Multimedia
{
    public class VideoViewModel : ViewModelBase
    {
        private Uri m_videoUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoViewModel"/> class.
        /// </summary>
        public VideoViewModel()
        {
            
        }

        public Uri VideoUri
        {
            get { return m_videoUri; }
            set
            {
                if(m_videoUri != value)
                {
                    m_videoUri = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
