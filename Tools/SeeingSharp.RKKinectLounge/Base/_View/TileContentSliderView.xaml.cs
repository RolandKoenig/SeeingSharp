using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SeeingSharp.RKKinectLounge.Base
{
    /// <summary>
    /// Interaktionslogik für ThumbnailView.xaml
    /// </summary>
    public partial class TileContentSliderView : UserControl
    {
        private const int CHANGE_INTERVAL_MIN_MS = 5000;
        private const int CHANGE_INTERVAL_MAX_MS = 10000;

        // Using a DependencyProperty as the backing store for ImageCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElementCollectionProperty =
            DependencyProperty.Register("ElementCollection", typeof(ObservableCollection<object>), typeof(TileContentSliderView), new PropertyMetadata(null));

        private DispatcherTimer m_timer;
        private Random m_random;
        private int m_lastTakenIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileContentSliderView"/> class.
        /// </summary>
        public TileContentSliderView()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
            this.SizeChanged += OnSizeChanged;

            m_random = Constants.UI_RANDOMIZER;
        }

        /// <summary>
        /// Triggers a new image change.
        /// </summary>
        private void PerformViewModelChange()
        {
            ObservableCollection<object> viewModelCollection = this.ElementCollection;
            if (viewModelCollection == null) { return; }
            if (viewModelCollection.Count == 0) { return; }

            // Choose next image to be shown (based on random)
            object nextViewModel = viewModelCollection[0];
            if(viewModelCollection.Count > 1)
            {
                int actTakenIndex = m_random.Next(0, viewModelCollection.Count);
                while(actTakenIndex == m_lastTakenIndex)
                {
                    actTakenIndex = m_random.Next(0, viewModelCollection.Count);
                }
                nextViewModel = viewModelCollection[actTakenIndex];
                m_lastTakenIndex = actTakenIndex;
            }

            // Don't do anything further if the selected image is already displayed
            if (nextViewModel == GetCurrentlyDisplayedViewModel()) { return; }

            // Show the next element on the view
            // The real content is loaded using ResourceDictionary (=> See ViewModelFirst pattern)
            ContentControl nextContentControl = new ContentControl();
            nextContentControl.Width =m_mainGrid.ActualWidth;
            nextContentControl.Height = m_mainGrid.ActualHeight;
            IAddChild addChildInterface = m_mainGrid as IAddChild;
            addChildInterface.AddChild(nextContentControl);

            // Now bind the ViewModel to the ContentControl
            nextContentControl.Content = nextViewModel;
        }

        /// <summary>
        /// Gets the currently displayed ViewModel.
        /// </summary>
        private ViewModelBase GetCurrentlyDisplayedViewModel()
        {
            var firstImage = m_mainGrid.Children.OfType<FrameworkElement>().FirstOrDefault();
            if (firstImage == null) { return null; }

            return firstImage.DataContext as ViewModelBase;
        }

        /// <summary>
        /// Called when this control is loading.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!SeeingSharpApplication.IsInitialized) { return; }

            if(m_timer == null)
            {
                m_timer = new DispatcherTimer(DispatcherPriority.Normal);
                m_timer.Interval = TimeSpan.FromMilliseconds(
                    (double)m_random.Next(CHANGE_INTERVAL_MIN_MS, CHANGE_INTERVAL_MAX_MS));
                m_timer.Tick += OnTimer_Tick;
                m_timer.Start();
            }

            // Perform initial image change
            this.PerformViewModelChange();
        }

        /// <summary>
        /// Called when this control is unloading.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if(m_timer != null)
            {
                m_timer.Tick -= OnTimer_Tick;
                m_timer.Stop();
                m_timer = null;
            }
        }

        /// <summary>
        /// Called when the timer ticks.
        /// This event triggers changing of the displayed image.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnTimer_Tick(object sender, EventArgs e)
        {
            // Refresh timer settings first (change interval)
            if (m_timer != null)
            {
                m_timer.Stop();
                m_timer.Interval = TimeSpan.FromMilliseconds(
                    (double)m_random.Next(CHANGE_INTERVAL_MIN_MS, CHANGE_INTERVAL_MAX_MS));
                m_timer.Start();
            }

            // Trigger image change
            PerformViewModelChange();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var uiElements = m_mainGrid.Children.OfType<UIElement>().ToArray();
            var lastElement = uiElements.LastOrDefault() as FrameworkElement;
            if (lastElement != null)
            {
                lastElement.Width = m_mainGrid.ActualWidth;
                lastElement.Height = m_mainGrid.ActualHeight;
            }
        }

        /// <summary>
        /// Called when a fade-in animation was finished.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            // Clear all old ui elements
            var uiElements = m_mainGrid.Children.OfType<UIElement>().ToArray();
            for (int loop = 0; loop < uiElements.Length - 1; loop++)
            {
                m_mainGrid.Children.Remove(uiElements[loop]);
            }
        }

        /// <summary>
        /// Gets or sets the image collection.
        /// </summary>
        public ObservableCollection<object> ElementCollection
        {
            get { return (ObservableCollection<object>)GetValue(ElementCollectionProperty); }
            set { SetValue(ElementCollectionProperty, value); }
        }
    }
}
