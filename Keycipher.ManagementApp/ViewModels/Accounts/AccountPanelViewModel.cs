using DevExpress.Mvvm.POCO;
using Keycipher.ManagementApp.DataModel;

namespace Keycipher.ManagementApp.ViewModels.Accounts
{
    public class AccountPanelViewModel
    {
        protected AccountPanelViewModel()
        {
        }

        public virtual Account Entity { get; set; }

        public static AccountPanelViewModel Create()
        {
            return ViewModelSource.Create(() => new AccountPanelViewModel());
        }
    }
}