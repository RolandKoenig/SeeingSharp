using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrozenSky.Samples.VirtualTestRoom
{
    public partial class MainWindow : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            TestingRoomViewModel vm = new TestingRoomViewModel();
            m_ctrlRenderer.Scene = vm.Scene;
            m_ctrlRenderer.Camera = vm.Camera;
        }
    }
}
