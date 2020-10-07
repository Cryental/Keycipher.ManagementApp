using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Core;

namespace Keycipher.ManagementApp
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs z)
        {
            if (!File.Exists("Data\\KeycipherLicensing.db"))
            {
                var file = new FileInfo("Data\\KeycipherLicensing.db");
                file.Directory.Create(); // If the directory already exists, this method does nothing.
                File.WriteAllBytes(file.FullName, ManagementApp.Properties.Resources.KeycipherLicensing);
            }

            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                var msg = new StringBuilder();
                msg.AppendLine(e.Exception.GetType().FullName);
                msg.AppendLine(e.Exception.Message);
                var st = new StackTrace();
                msg.AppendLine(st.ToString());
                msg.AppendLine();
                File.AppendAllText("errorlog.txt", msg.ToString());
                ApplicationThemeHelper.UpdateApplicationThemeName();
            };

            base.OnStartup(z);
        }
    }
}