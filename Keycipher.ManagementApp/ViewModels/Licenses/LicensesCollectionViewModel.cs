using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.ViewModel;
using Keycipher.ManagementApp.DataModel;
using Keycipher.ManagementApp.Services;

namespace Keycipher.ManagementApp.ViewModels.Licenses
{
    [POCOViewModel]
    public class LicensesCollectionViewModel
    {
        protected LicensesCollectionViewModel()
        {
            UpdateAccounts();
            NavigationPaneVisibility = NavigationPaneVisibility.Normal;
        }

        [ServiceProperty(Key = "NewLicenseService")]
        public virtual IDialogService NewLicenseService => null;

        [ServiceProperty(Key = "MessageBoxService")]
        public virtual IMessageBoxService MessageBoxService => null;

        [ServiceProperty(Key = "EditDialogService")]
        public virtual IDialogService EditDialogService => null;

        [ServiceProperty(Key = "SaveFileDialogService")]
        public virtual ISaveFileDialogService SaveFileDialogService => null;

        [ServiceProperty(Key = "ExportService")]
        public virtual IExportService ExportService => null;

        [ServiceProperty(Key = "SplashScreenManagerService")]
        public virtual ISplashScreenManagerService SplashScreenManagerService => null;

        [ServiceProperty(Key = "DocumentManagerService")]
        public virtual IDocumentManagerService DocumentManagerService => null;


        public virtual NavigationPaneVisibility NavigationPaneVisibility { get; set; }

        public virtual List<Account> Accounts { get; set; }
        public virtual Account SelectedAccount { get; set; }
        public virtual Product SelectedProduct { get; set; }

        public virtual LicenseConfigViewModel LicenseConfigVM { get; set; }


        public void RetrieveLicenses()
        {
            SplashScreenManagerService.Show();
            LicenseConfigVM = LicenseConfigViewModel.Create(SelectedProduct);
            LicenseConfigVM.SetParentViewModel(this);
            SplashScreenManagerService.Close();
        }

        public bool CanRetrieveLicenses()
        {
            return SelectedProduct != null;
        }

        public void UpdateAccounts()
        {
            var db = new KeycipherLicensingDBEntities();
            Accounts = db.Accounts.Include("Products").Cast<Account>().ToList();
            SelectedAccount = null;
            SelectedProduct = null;
            LicenseConfigVM = null;
        }


        public static LicensesCollectionViewModel Create()
        {
            return ViewModelSource.Create(() => new LicensesCollectionViewModel());
        }
    }
}