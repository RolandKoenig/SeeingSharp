using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeeingSharpModelViewer
{
    public partial class MainWindow : SeeingSharpForm
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (SeeingSharpApplication.IsInitialized)
            {
                this.Text = $"{SeeingSharpApplication.Current.ProductName} - {SeeingSharpApplication.Current.ProductVersion}";

                m_dlgOpenFile.Filter =
                    GraphicsCore.Current.ImportersAndExporters.GetOpenFileDialogFilter();
            }
        }

        private void OnCmdOpen_Click(object sender, EventArgs e)
        {
            if(m_dlgOpenFile.ShowDialog(this) == DialogResult.OK)
            {

            }
        }
    }
}
