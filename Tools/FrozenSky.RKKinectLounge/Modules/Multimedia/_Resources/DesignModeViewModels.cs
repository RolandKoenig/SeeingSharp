using FrozenSky.RKKinectLounge.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FrozenSky.RKKinectLounge.Modules.Multimedia
{
    public class DesignModeViewModels
    {
        public VideoViewModel VideoVM
        {
            get { return new VideoViewModel(); }
        }
         
        public FolderViewModel MultimediaSingleVM
        {
            get
            {
                FolderViewModel result = FolderViewModel.CreateDummy();
                MultimediaFolderViewModelExtension resultExt =
                    result.GetAndEnsureExtension<MultimediaFolderViewModelExtension>();
                resultExt.FullViewsLoaded.Add(new ImageViewModel()
                    {
                         ImageSoure = new BitmapImage(new Uri("/Assets/Images/Video.jpg", UriKind.Relative))
                    });
                return result;
            }
        }
    }
}
