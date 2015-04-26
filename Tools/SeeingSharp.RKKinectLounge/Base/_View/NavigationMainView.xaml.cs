using SeeingSharp.Infrastructure;
using SeeingSharp.RKKinectLounge.Base;
using SeeingSharp.Util;
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
using System.Windows.Interactivity;
using System.Threading;

namespace SeeingSharp.RKKinectLounge.Base
{
    /// <summary>
    /// Interaktionslogik für BrowserMainView.xaml
    /// </summary>
    public partial class NavigationMainView : UserControl
    {
        private List<CancellationTokenSource> m_prevCancellationTokenSources;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationMainView"/> class.
        /// </summary>
        public NavigationMainView()
        {
            InitializeComponent();

            m_prevCancellationTokenSources = new List<CancellationTokenSource>();
        }

        /// <summary>
        /// Wird immer dann aufgerufen, wenn der tatsächliche Wert einer Abhängigkeitseigenschaft eines <see cref="T:System.Windows.FrameworkElement" /> aktualisiert wurde. Die spezifische Abhängigkeitseigenschaft, die sich geändert hat, wird im Argumenteparameter gemeldet. Überschreibt <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)" />.
        /// </summary>
        /// <param name="e">Die Ereignisdaten, in denen die geänderte Eigenschaft sowie die alten und neuen Werte beschrieben werden.</param>
        protected override async void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            // Check for general app initialization (= simple DesignTime check)
            if (!SeeingSharpApplication.IsInitialized) { return; }

            if(e.Property == NavigationMainView.DataContextProperty)
            {
                // Create the new UIElement to be displayed
                FrameworkElement newElement = null;
                NavigateableViewModelBase newViewModel = e.NewValue as NavigateableViewModelBase;
                if (newViewModel != null)
                {
                    newElement = ViewFactory.CreateFullView(newViewModel);
                    newElement.DataContext = newViewModel;
                }
                if (newElement == null) { newElement = new Control(); }

                // Remember all previous child contents
                List<FrameworkElement> previousElements = GetPreviousChildElements();

                // Apply new element on the ui
                m_contentGrid.Children.Clear();
                m_contentGrid.Children.Add(newElement);

                // Trigger cancellation of previous loading operations
                // (e. g. cancels asynchronous image loading of previous control)
                foreach(CancellationTokenSource actPrevCancelTokenSource in m_prevCancellationTokenSources)
                {
                    actPrevCancelTokenSource.Cancel();
                }
                m_prevCancellationTokenSources.Clear();

                // Unload all previous child contents (and wait for finishing)
                // (this part sets all references to null, deregisters events, etc.)
                foreach(FrameworkElement actPreviousChild in previousElements)
                {
                    NavigateableViewModelBase actPreviousVM = actPreviousChild.DataContext as NavigateableViewModelBase;
                    if(actPreviousVM != null)
                    {
                        await actPreviousVM.UnloadAsync();
                    }
                }

                // Trigger loading of the detail contents of the current control
                // (triggers loading subfolder previous, etc.)
                if (newViewModel != null)
                {
                    CancellationTokenSource newCancelTokenSource = new CancellationTokenSource();
                    m_prevCancellationTokenSources.Add(newCancelTokenSource);

                    await newViewModel.LoadDetailContentAsync(newCancelTokenSource.Token);
                }
            }
        }

        /// <summary>
        /// Gets the previous child elements.
        /// </summary>
        private List<FrameworkElement> GetPreviousChildElements()
        {
            List<FrameworkElement> previousElements = new List<FrameworkElement>();
            foreach (var actPreviousChild in m_contentGrid.Children)
            {
                FrameworkElement actPreviousChildElement = actPreviousChild as FrameworkElement;
                if (actPreviousChildElement == null) { continue; }

                previousElements.Add(actPreviousChildElement);
            }
            return previousElements;
        }
    }
}
