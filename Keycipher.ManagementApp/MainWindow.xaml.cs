using System.ComponentModel;
using DevExpress.Xpf.Core;

namespace Keycipher.ManagementApp
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            ApplicationThemeHelper.SaveApplicationThemeName();
        }
    }
}