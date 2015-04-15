using FrozenSky.Samples.Base;
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
using System.Windows.Shapes;

namespace FrozenSky.Samples.WpfSampleContainer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, SamplePage> m_loadedSamples;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            m_loadedSamples = new Dictionary<string, SamplePage>();

            // Build the menu bar
            foreach (string actSample in SampleManager.Current.GetSampleNames())
            {
                string actSampleInner = actSample;

                MenuItem actItem = new MenuItem();
                actItem.Header = actSample;
                actItem.Click += (sender, eArgs) =>
                {
                    SwitchSampleTo(actSampleInner);
                };
                this.MainMenuBar.Items.Add(actItem);
            }
        }

        /// <summary>
        /// Switches the currently displayed sample to the given one.
        /// </summary>
        /// <param name="sampleName">Name of the sample.</param>
        private void SwitchSampleTo(string sampleName)
        {
            // Get/Create the requested sample page
            SamplePage samplePage = null;
            if(!m_loadedSamples.TryGetValue(sampleName, out samplePage))
            {
                samplePage = new SamplePage();
                samplePage.SampleName = sampleName;

                m_loadedSamples.Add(sampleName, samplePage);
            }

            // Attach the sample page to the screen
            if (!this.CurrentSampleContainer.Children.Contains(samplePage))
            {
                this.CurrentSampleContainer.Children.Clear();
                this.CurrentSampleContainer.Children.Add(samplePage);
            }
        }
    }
}
