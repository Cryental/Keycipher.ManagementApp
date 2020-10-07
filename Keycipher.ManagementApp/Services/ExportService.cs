using System.Linq;
using System.Windows;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Grid;

namespace Keycipher.ManagementApp.Services
{
    public interface IExportService
    {
        void ExportTo(ExportType fileType, string fileName);
        void ShowPreview();
    }

    public class ExportService : ServiceBase, IExportService
    {
        public static readonly DependencyProperty ViewProperty =
            DependencyProperty.Register("View", typeof(TableView), typeof(ExportService), new PropertyMetadata(null));

        public TableView View
        {
            get => (TableView) GetValue(ViewProperty);
            set => SetValue(ViewProperty, value);
        }

        public void ExportTo(ExportType fileType, string fileName)
        {
            if (View == null)
            {
                return;
            }

            switch (fileType)
            {
                case ExportType.XLSX:
                    View.ExportToXlsx(fileName);
                    break;
                case ExportType.PDF:
                    View.ExportToPdf(fileName);
                    break;
            }
        }

        public void ShowPreview()
        {
            if (View == null)
            {
                return;
            }

            var rootWindow =
                LayoutTreeHelper.GetVisualParents(AssociatedObject).FirstOrDefault(x => x is Window) as Window;
            if (rootWindow != null)
            {
                View.ShowPrintPreviewDialog(rootWindow);
            }
        }
    }

    public enum ExportType
    {
        XLSX, PDF
    }
}