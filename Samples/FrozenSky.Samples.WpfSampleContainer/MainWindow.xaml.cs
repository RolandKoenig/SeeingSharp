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

namespace WpfSampleContainer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Build the menu bar
            foreach (SampleDescription actSample in SampleFactory.Current.GetSampleInfos())
            {
                SampleDescription actSampleInner = actSample;

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
        /// <param name="sampleDesc">The description of the sample to be switched to</param>
        private void SwitchSampleTo(SampleDescription sampleDesc)
        {
            SamplePage samplePage = new SamplePage();
            samplePage.Sample = sampleDesc;

            // Attach the sample page to the screen
            if (!this.CurrentSampleContainer.Children.Contains(samplePage))
            {
                this.CurrentSampleContainer.Children.Clear();
                this.CurrentSampleContainer.Children.Add(samplePage);
            }
        }
    }
}
