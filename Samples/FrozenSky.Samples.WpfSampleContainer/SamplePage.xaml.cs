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
using FrozenSky.Samples.Base;
using PropertyTools.Wpf;
using FrozenSky.Multimedia.Core;
using FrozenSky.Multimedia.Drawing3D;

namespace FrozenSky.Samples.WpfSampleContainer
{
    /// <summary>
    /// Interaktionslogik für EmptyPage.xaml
    /// </summary>
    public partial class SamplePage : UserControl
    {
        private Camera3DBase m_cameraOrthogonal;
        private Camera3DBase m_cameraPerspective;

        /// <summary>
        /// Initializes a new instance of the <see cref="SamplePage"/> class.
        /// </summary>
        public SamplePage()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        /// <summary>
        /// Default load event.. build the demo scene.
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Query for the sample name
            string sampleName = this.SampleName;
            if(string.IsNullOrEmpty(sampleName))
            {
                return;
            }
                
            // Create all cameras and apply default one
            m_cameraOrthogonal = new OrthographicCamera3D();
            m_cameraPerspective = new PerspectiveCamera3D();
            m_ctrl3DView2.Camera = m_cameraPerspective;

            // Apply the sample
            SampleFactory.Current.ApplySample(
                m_ctrl3DView2.RenderLoop,
                sampleName);
        }

        private void OnCmdUseOrthogonalCamera_Click(object sender, RoutedEventArgs e)
        {
            Camera3DBase previousCamera = m_ctrl3DView2.Camera as Camera3DBase;
            Camera3DBase newCamera = m_cameraOrthogonal;
            newCamera.Position = previousCamera.Position;
            newCamera.RelativeTarget = previousCamera.RelativeTarget;
            newCamera.UpdateCamera();

            m_ctrl3DView2.Camera = newCamera;
        }

        private void OnCmdUsePerspectiveCamera_Click(object sender, RoutedEventArgs e)
        {
            Camera3DBase previousCamera = m_ctrl3DView2.Camera as Camera3DBase;
            Camera3DBase newCamera = m_cameraPerspective;
            newCamera.Position = previousCamera.Position;
            newCamera.RelativeTarget = previousCamera.RelativeTarget;
            newCamera.UpdateCamera();

            m_ctrl3DView2.Camera = newCamera;
        }

        /// <summary>
        /// Gets or sets the name of the sample which is to be displayed.
        /// </summary>
        /// <value>
        /// The name of the sample.
        /// </value>
        public string SampleName
        {
            get;
            set;
        }
    }
}
