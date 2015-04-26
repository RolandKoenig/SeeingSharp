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

namespace SeeingSharp.RKKinectLounge.Base
{
    /// <summary>
    /// Interaktionslogik für KinectWelcomeView.xaml
    /// </summary>
    public partial class KinectWelcomeView : UserControl
    {
        public static readonly DependencyProperty MainViewVisibilityProperty =
            DependencyProperty.Register("MainViewVisibility", typeof(Visibility), typeof(KinectWelcomeView), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectWelcomeView"/> class.
        /// </summary>
        public KinectWelcomeView()
        {
            InitializeComponent();
        }

        public Visibility MainViewVisibility
        {
            get { return (Visibility)GetValue(MainViewVisibilityProperty); }
            set { SetValue(MainViewVisibilityProperty, value); }
        }
    }
}
