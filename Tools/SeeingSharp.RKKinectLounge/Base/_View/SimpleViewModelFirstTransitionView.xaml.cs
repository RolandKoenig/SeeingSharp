using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SeeingSharp.Util;
using System.Windows.Markup;

namespace SeeingSharp.RKKinectLounge.Base
{
    /// <summary>
    /// Interaktionslogik für SimpleViewModelFirstTransitionView.xaml
    /// </summary>
    public partial class SimpleViewModelFirstTransitionView : UserControl
    {
        public static readonly DependencyProperty DisplayedViewModelProperty =
            DependencyProperty.Register("DisplayedViewModel", typeof(ViewModelBase), typeof(SimpleViewModelFirstTransitionView), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleViewModelFirstTransitionView"/> class.
        /// </summary>
        public SimpleViewModelFirstTransitionView()
        {
            InitializeComponent();

            this.SizeChanged += OnSizeChanged;
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
        /// Wird immer dann aufgerufen, wenn der tatsächliche Wert einer Abhängigkeitseigenschaft eines <see cref="T:System.Windows.FrameworkElement" /> aktualisiert wurde. Die spezifische Abhängigkeitseigenschaft, die sich geändert hat, wird im Argumenteparameter gemeldet. Überschreibt <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)" />.
        /// </summary>
        /// <param name="e">Die Ereignisdaten, in denen die geänderte Eigenschaft sowie die alten und neuen Werte beschrieben werden.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if(e.Property == DisplayedViewModelProperty)
            {
                ViewModelBase nextViewModel = this.DisplayedViewModel;

                // Don't do anything further if the selected image is already displayed
                if (nextViewModel == null) { return; }
                if (nextViewModel == GetCurrentlyDisplayedViewModel()) { return; }

                // Show the next element on the view
                // The real content is loaded using ResourceDictionary (=> See ViewModelFirst pattern)
                ContentControl nextContentControl = new ContentControl();
                nextContentControl.Width = m_mainGrid.ActualWidth;
                nextContentControl.Height = m_mainGrid.ActualHeight;
                IAddChild addChildInterface = m_mainGrid as IAddChild;
                addChildInterface.AddChild(nextContentControl);

                // Now bind the ViewModel to the ContentControl
                nextContentControl.Content = nextViewModel;
            }
        }

        /// <summary>
        /// Called when the size of this control has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
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

        public ViewModelBase DisplayedViewModel
        {
            get { return (ViewModelBase)GetValue(DisplayedViewModelProperty); }
            set { SetValue(DisplayedViewModelProperty, value); }
        }
    }
}
