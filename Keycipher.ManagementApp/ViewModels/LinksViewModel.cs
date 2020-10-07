using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core;

namespace Keycipher.ManagementApp.ViewModels
{
    [POCOViewModel]
    public class LinksViewModel
    {
        public static LinksViewModel Create()
        {
            return ViewModelSource.Create(() => new LinksViewModel());
        }

        public void GettingStarted()
        {
            DocumentPresenter.OpenLink("https://cryental.dev/services/licensing/");
        }

        public void RequestProducts()
        {
            DocumentPresenter.OpenLink("https://cryental.dev/services/licensing/create");
        }

        public void ContactMe()
        {
            DocumentPresenter.OpenLink("https://cryental.dev");
        }

        public void About()
        {
            // to do later
        }
    }
}