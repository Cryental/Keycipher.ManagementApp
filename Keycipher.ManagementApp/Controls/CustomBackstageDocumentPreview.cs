using System.Windows;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports;

namespace Keycipher.ManagementApp.Controls
{
    public class CustomBackstageDocumentPreview : BackstageDocumentPreview
    {
        public static readonly DependencyProperty DocumentSourceProperty =
            DependencyProperty.Register("DocumentSource", typeof(IReport), typeof(CustomBackstageDocumentPreview),
                new PropertyMetadata(null));

        public IReport DocumentSource
        {
            get => (IReport) GetValue(DocumentSourceProperty);
            set => SetValue(DocumentSourceProperty, value);
        }
    }
}