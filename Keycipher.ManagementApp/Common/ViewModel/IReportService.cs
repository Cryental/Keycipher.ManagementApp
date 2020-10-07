using System;
using DevExpress.XtraReports;

namespace Keycipher.ManagementApp.Common.ViewModel
{
    public interface IReportInfo
    {
        object ParametersViewModel { get; }
        IReport CreateReport();
    }

    public class ParameterlessReportInfo : IReportInfo
    {
        private readonly IReport report;

        public ParameterlessReportInfo(IReport report)
        {
            this.report = report;
        }

        object IReportInfo.ParametersViewModel => null;

        IReport IReportInfo.CreateReport()
        {
            return report;
        }
    }

    public class ReportInfo<TParametersViewModel> : IReportInfo
    {
        private readonly TParametersViewModel parametersViewModel;
        private readonly Func<TParametersViewModel, IReport> reportFactory;

        public ReportInfo(TParametersViewModel parametersViewModel, Func<TParametersViewModel, IReport> reportFactory)
        {
            this.parametersViewModel = parametersViewModel;
            this.reportFactory = reportFactory;
        }

        object IReportInfo.ParametersViewModel => parametersViewModel;

        IReport IReportInfo.CreateReport()
        {
            return reportFactory(parametersViewModel);
        }
    }

    public interface IReportService
    {
        void SetDefaultReport(IReportInfo reportInfo);
        void ShowReport(IReportInfo reportInfo);
    }
}