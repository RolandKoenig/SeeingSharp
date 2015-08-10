using SeeingSharp.Infrastructure;
using SeeingSharp.RKKinectLounge.Base;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

// Define assembly attributes for the type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(SeeingSharp.RKKinectLounge.Modules.Multimedia.MultimediaFolderViewModelExtension),
    contractType: typeof(SeeingSharp.RKKinectLounge.Base.INavigateableViewModelExtension))]

namespace SeeingSharp.RKKinectLounge.Modules.Multimedia
{
    public class MultimediaFolderViewModelExtension : INavigateableViewModelExtension
    {
        private List<ViewModelBase> m_fullViewsPreview;
        private ObservableCollection<ViewModelBase> m_fullViewsLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultimediaFolderViewModelExtension"/> class.
        /// </summary>
        public MultimediaFolderViewModelExtension()
        {
            m_fullViewsPreview = new List<ViewModelBase>();
            m_fullViewsLoaded = new ObservableCollection<ViewModelBase>();
        }

        /// <summary>
        /// Does this object extend the given object?
        /// Return false to ensure that no further logic is executed here.
        /// </summary>
        /// <param name="ownerViewModel">The owner view model.</param>
        public bool ExtendsViewModelType(NavigateableViewModelBase ownerViewModel)
        {
            return ownerViewModel is FolderViewModel;
        }

        /// <summary>
        /// Loads the preview content asynchronous.
        /// </summary>
        /// <param name="ownerViewModel">The parent view model.</param>
        /// <param name="cancelToken">The cancel token.</param>
        public async Task LoadPreviewContentAsync(NavigateableViewModelBase ownerViewModel, CancellationToken cancelToken)
        {
            FolderViewModel ownerFolder = ownerViewModel as FolderViewModel;
            if (ownerFolder == null) { return; }

            // Load all multimedia content
            await LoadPreviewContentImagesAsync(ownerFolder, cancelToken);
            LoadPreviewContentVideos(ownerFolder);
        }

        /// <summary>
        /// Loads all folder contents that belong to this extension.
        /// </summary>
        /// <param name="ownerViewModel">The ViewModel for which to load detail content.</param>
        /// <param name="cancelToken">The cancel token.</param>
        public async Task LoadDetailContentAsync(NavigateableViewModelBase ownerViewModel, CancellationToken cancelToken)
        {
            while(m_fullViewsPreview.Count > 0)
            {
                if (cancelToken.IsCancellationRequested) { return; }

                ViewModelBase actPreviewVM = m_fullViewsPreview[0];
                m_fullViewsPreview.RemoveAt(0);

                // Handle images
                ImageViewModel actImage = actPreviewVM as ImageViewModel;
                if (actImage != null)
                {
                    await actImage.PreloadAsync(1024, 768);
                }

                // Delay some time before adding the ViewModel
                await Task.Delay(300);

                m_fullViewsLoaded.Add(actPreviewVM);
            }
        }

        /// <summary>
        /// Unloads all contents which are loaded currently.
        /// </summary>
        public void Unload()
        {
            m_fullViewsPreview.Clear();
            m_fullViewsLoaded.Clear();
        }

        /// <summary>
        /// Performs preview loading on images.
        /// </summary>
        /// <param name="ownerViewModel">The owner view model.</param>
        /// <param name="cancelToken">The cancel token.</param>
        private async Task LoadPreviewContentImagesAsync(FolderViewModel ownerViewModel, CancellationToken cancelToken)
        {
            // Load all details 
            Dictionary<string, object> loadedFiles = new Dictionary<string, object>();

            // Load all thumbnails
            int loadedThumbnailCount = 0;
            foreach (string actFileName in Directory.GetFiles(ownerViewModel.BasePath, Constants.BROWSING_SEARCH_PATTERN_THUMBNAIL))
            {
                // Do only load supported formats
                if (Array.IndexOf(Constants.SUPPORTED_IMAGE_FORMATS, Path.GetExtension(actFileName)) < 0)
                {
                    continue;
                }

                // Load the bitmap
                ImageViewModel actThumbnailVM = new ImageViewModel();
                actThumbnailVM.FilePath = actFileName;
                ownerViewModel.ThumbnailViewModels.Add(actThumbnailVM);
                loadedThumbnailCount++;

                // Register currently loaded file
                loadedFiles[actFileName] = null;
            }

            // Handle all remaining files
            foreach (string actFileName in 
                Directory.GetFiles(ownerViewModel.BasePath)
                    .OrderBy((actPath) => actPath))
            {
                if (loadedFiles.ContainsKey(actFileName)) { continue; }

                // Handle images
                if (Array.IndexOf(Constants.SUPPORTED_IMAGE_FORMATS, Path.GetExtension(actFileName)) >= 0)
                {
                    // Load the bitmap
                    m_fullViewsPreview.Add(new ImageViewModel() { FilePath = actFileName });

                    continue;
                }
            }

            // Take some images from full-size collection if there are no explicit thumbnails
            if ((loadedThumbnailCount == 0) &&
                (m_fullViewsPreview.Count > 0))
            {
                ImageViewModel[] fullImages = m_fullViewsPreview.OfType<ImageViewModel>().ToArray();
                if (fullImages.Length > 0)
                {
                    List<int> takenRandomValues = new List<int>();
                    for (int loop = 0; loop < 5 && loop < fullImages.Length; loop++)
                    {
                        int nextRandom = Constants.UI_RANDOMIZER.Next(0, fullImages.Length);
                        while(takenRandomValues.Contains(nextRandom))
                        {
                            nextRandom = Constants.UI_RANDOMIZER.Next(0, fullImages.Length);
                        }

                        ownerViewModel.ThumbnailViewModels.Add(
                            fullImages[nextRandom]);
                    }
                }
            }

            // Preload thumbnail images
            foreach (ImageViewModel actThumbnailVM in ownerViewModel.ThumbnailViewModels.OfType<ImageViewModel>())
            {
                await actThumbnailVM.PreloadAsync(Constants.THUMBNAIL_WDITH, Constants.THUMBNAIL_HEIGHT);

                // Cancel here if cancellation is requested
                if (cancelToken.IsCancellationRequested) { return; }
            }
        }

        /// <summary>
        /// Performs preview loading on videos.
        /// </summary>
        /// <param name="ownerViewModel">The owner view model.</param>
        private void LoadPreviewContentVideos(FolderViewModel ownerViewModel)
        {
            // Load all thumbnails
            foreach (string actFileName in Directory.GetFiles(ownerViewModel.BasePath))
            {
                // Do only load supported formats
                if (Array.IndexOf(Constants.SUPPORTED_VIDEO_FORMATS, Path.GetExtension(actFileName)) < 0)
                {
                    continue;
                }

                // Load the bitmap
                VideoViewModel videoVM = new VideoViewModel();
                videoVM.VideoUri = new Uri(actFileName, UriKind.Absolute);

                // Register currently loaded file
                m_fullViewsPreview.Add(videoVM);
            }
        }

        /// <summary>
        /// Gets a collection containing all ViewModels for the SliderView control.
        /// </summary>
        public ObservableCollection<ViewModelBase> FullViewsLoaded
        {
            get { return m_fullViewsLoaded; }
        }

        /// <summary>
        /// Gets the short name of this ViewModel extension.
        /// </summary>
        public string ShortName
        {
            get { return Constants.VIEW_MODEL_EXT_MULTIMEDIA; }
        }

        public int CountFullImages
        {
            get { return m_fullViewsPreview.Count + m_fullViewsLoaded.Count; }
        }
    }
}
