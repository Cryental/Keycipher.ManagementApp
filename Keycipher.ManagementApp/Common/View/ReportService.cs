using System.ComponentModel;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports;
using Keycipher.ManagementApp.Common.ViewModel;

namespace Keycipher.ManagementApp.Common.View
{
    public abstract class ReportServiceBase : ServiceBase, IReportService
    {
        private IReportInfo actualReportInfo;
        private IReportInfo defaultReportInfo;
        private bool isVisible;
        private IReportInfo reportInfo;

        protected bool IsVisible
        {
            get => isVisible;
            set
            {
                isVisible = value;
                UpdateReport();
                if (!isVisible)
                {
                    reportInfo = null;
                }
            }
        }

        private object ActualParametersViewModel =>
            actualReportInfo == null ? null : actualReportInfo.ParametersViewModel;

        protected virtual void SetDefaultReport(IReportInfo reportInfo)
        {
            defaultReportInfo = reportInfo;
            UpdateReport();
        }

        protected virtual void ShowReport(IReportInfo reportInfo)
        {
            this.reportInfo = reportInfo;
            UpdateReport();
        }

        private void UpdateReport()
        {
            UpdateReportCore(IsVisible ? reportInfo ?? defaultReportInfo : null);
        }

        protected virtual void UpdateReportCore(IReportInfo actualReportInfo)
        {
            UnsubscribeFromParametersViewModel();
            this.actualReportInfo = actualReportInfo;
            SubscribeToParametersViewModel();
            if (this.actualReportInfo == null)
            {
                DestroyReport();
            }
            else
            {
                CreateReport();
            }
        }

        private void OnParametersViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CreateReport();
        }

        protected void CreateReport()
        {
            var report = actualReportInfo.CreateReport();
            if (report == null)
            {
                return;
            }

            SetCustomSettingsViewModel(actualReportInfo.ParametersViewModel);
            SetDocumentSource(report);
            report.PrintingSystemBase.ClearContent();
            report.CreateDocument(true);
        }

        private void DestroyReport()
        {
            SetCustomSettingsViewModel(null);
        }

        protected abstract void SetDocumentSource(IReport report);
        protected abstract void SetCustomSettingsViewModel(object customSettingsViewModel);

        private void SubscribeToParametersViewModel()
        {
            var parametersViewModel = ActualParametersViewModel as INotifyPropertyChanged;
            if (parametersViewModel != null)
            {
                parametersViewModel.PropertyChanged += OnParametersViewModelPropertyChanged;
            }
        }

        private void UnsubscribeFromParametersViewModel()
        {
            var parametersViewModel = ActualParametersViewModel as INotifyPropertyChanged;
            if (parametersViewModel != null)
            {
                parametersViewModel.PropertyChanged -= OnParametersViewModelPropertyChanged;
            }
        }

        #region IReportService

        void IReportService.SetDefaultReport(IReportInfo reportInfo)
        {
            SetDefaultReport(reportInfo);
        }

        void IReportService.ShowReport(IReportInfo reportInfo)
        {
            ShowReport(reportInfo);
        }

        #endregion
    }

    public class DocumentViewerReportService : ReportServiceBase
    {
        private DocumentPreviewControl DocumentViewer => (DocumentPreviewControl) AssociatedObject;

        public ZoomMode ZoomMode { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            IsVisible = true;
            ZoomMode = ZoomMode.FitToWidth;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            IsVisible = false;
        }

        protected override void SetCustomSettingsViewModel(object customSettingsViewModel)
        {
        }

        protected override void SetDocumentSource(IReport report)
        {
            DocumentViewer.DocumentSource = report;
            DocumentViewer.ZoomMode = ZoomMode;
        }
    }
}