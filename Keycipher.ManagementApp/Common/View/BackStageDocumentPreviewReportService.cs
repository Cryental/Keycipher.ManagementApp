using System;
using System.Windows;
using DevExpress.Xpf.Ribbon;
using DevExpress.XtraReports;
using Keycipher.ManagementApp.Common.ViewModel;
using Keycipher.ManagementApp.Controls;

namespace Keycipher.ManagementApp.Common.View
{
    public class BackstageDocumentPreviewReportService : ReportServiceBase
    {
        public static readonly DependencyProperty BackstageViewIsOpenProperty =
            DependencyProperty.Register("BackstageViewIsOpen", typeof(bool),
                typeof(BackstageDocumentPreviewReportService), new PropertyMetadata(false, (d, e) =>
                    ((BackstageDocumentPreviewReportService) d).OnBackstageViewIsOpenChanged()));

        public static readonly DependencyProperty BackstageItemProperty =
            DependencyProperty.Register("BackstageItem", typeof(BackstageItemBase),
                typeof(BackstageDocumentPreviewReportService), new PropertyMetadata(null, (d, e) =>
                    ((BackstageDocumentPreviewReportService) d).OnBackstageItemChanged((BackstageItemBase) e.OldValue,
                        (BackstageItemBase) e.NewValue)));

        public static readonly DependencyProperty BackstageDocumentPreviewProperty =
            DependencyProperty.Register("BackstageDocumentPreview", typeof(CustomBackstageDocumentPreview),
                typeof(BackstageDocumentPreviewReportService), new PropertyMetadata(null, (d, e) =>
                    ((BackstageDocumentPreviewReportService) d).OnBackstageDocumentPreviewChanged(e)));

        private bool isDocumentFirstLoading = true;

        public bool BackstageViewIsOpen
        {
            get => (bool) GetValue(BackstageViewIsOpenProperty);
            set => SetValue(BackstageViewIsOpenProperty, value);
        }

        public BackstageItemBase BackstageItem
        {
            get => (BackstageItemBase) GetValue(BackstageItemProperty);
            set => SetValue(BackstageItemProperty, value);
        }

        public CustomBackstageDocumentPreview BackstageDocumentPreview
        {
            get => (CustomBackstageDocumentPreview) GetValue(BackstageDocumentPreviewProperty);
            set => SetValue(BackstageDocumentPreviewProperty, value);
        }

        protected override void ShowReport(IReportInfo reportInfo)
        {
            base.ShowReport(reportInfo);
            Dispatcher.BeginInvoke((Action) OpenBackstageView);
        }

        private void OpenBackstageView()
        {
            ((BackstageTabItem) BackstageItem).Backstage.SelectedTab = (BackstageTabItem) BackstageItem;
            BackstageItem.Backstage.IsOpen = true;
        }

        protected override void UpdateReportCore(IReportInfo actualReportInfo)
        {
            base.UpdateReportCore(actualReportInfo);
            BackstageItem.IsEnabled = actualReportInfo != null;
        }

        private void OnBackstageViewIsOpenChanged()
        {
            IsVisible = BackstageViewIsOpen;
        }

        private void OnBackstageItemChanged(BackstageItemBase oldItem, BackstageItemBase newItem)
        {
            if (newItem != null)
            {
                newItem.IsEnabled = oldItem != null && oldItem.IsEnabled;
            }
        }

        private void OnBackstageDocumentPreviewChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((CustomBackstageDocumentPreview) e.OldValue).Loaded -= BackstageDocumentPreview_Loaded;
            }

            if (e.NewValue != null)
            {
                ((CustomBackstageDocumentPreview) e.NewValue).Loaded += BackstageDocumentPreview_Loaded;
            }
        }

        private void BackstageDocumentPreview_Loaded(object sender, RoutedEventArgs e)
        {
            Action updateReportAction = () => CreateReport();
            var document = (CustomBackstageDocumentPreview) sender;
            if (document.IsVisible && isDocumentFirstLoading)
            {
                Dispatcher.BeginInvoke(updateReportAction);
                isDocumentFirstLoading = false;
            }
        }

        protected override void SetDocumentSource(IReport report)
        {
            BackstageDocumentPreview.DocumentSource = report;
        }

        protected override void SetCustomSettingsViewModel(object customSettingsViewModel)
        {
            BackstageDocumentPreview.CustomSettings = customSettingsViewModel;
        }
    }
}