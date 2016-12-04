using SeeingSharp.Infrastructure;
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
using SeeingSharp.Checking;

namespace SeeingSharp.View
{
    /// <summary>
    /// Interaction logic for SeeingSharpWpfErrorDialog.xaml
    /// </summary>
    public partial class SeeingSharpWpfErrorDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeeingSharpWpfErrorDialog"/> class.
        /// </summary>
        public SeeingSharpWpfErrorDialog()
        {
            InitializeComponent();
        }

        public static void ShowDialog(ExceptionInfo exInfo)
        {
            exInfo.EnsureNotNull(nameof(exInfo));

            SeeingSharpWpfErrorDialog errorDlg = new SeeingSharpWpfErrorDialog();
            errorDlg.DataSource = exInfo;
            errorDlg.ShowDialog();
        }

        public static void ShowDialog(Window owner, ExceptionInfo exInfo)
        {
            exInfo.EnsureNotNull(nameof(owner));
            exInfo.EnsureNotNull(nameof(exInfo));

            SeeingSharpWpfErrorDialog errorDlg = new SeeingSharpWpfErrorDialog();
            errorDlg.DataSource = exInfo;
            errorDlg.Owner = owner;
            errorDlg.ShowDialog();
        }

        private void OnCmdClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChkShowDetails_Checked(object sender, RoutedEventArgs e)
        {
            if(this.Height < 350) { this.Height = 350; }
        }

        public ExceptionInfo DataSource
        {
            get { return this.DataContext as ExceptionInfo; }
            set { this.DataContext = value; }
        }
    }
}
