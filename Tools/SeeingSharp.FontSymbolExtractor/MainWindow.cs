using SeeingSharp.Multimedia.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeeingSharp.FontSymbolExtractor
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            m_cmbFont.Items.Clear();
            m_cmbFont.Items.AddRange(GraphicsCore.Current.GetFontFamilyNames(CultureInfo.CurrentCulture.Name).ToArray());
        }

        private void OnCmdConvert_Click(object sender, EventArgs e)
        {

        }
    }
}
