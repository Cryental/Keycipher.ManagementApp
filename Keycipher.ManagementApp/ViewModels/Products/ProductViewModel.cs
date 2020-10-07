using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.ViewModel;
using Keycipher.ManagementApp.Common;
using Keycipher.ManagementApp.DataModel;

namespace Keycipher.ManagementApp.ViewModels.Products
{
    public partial class ProductViewModel : SingleObjectViewModel<Product, long, IKeycipherLicensingUnitOfWork>
    {
        protected ProductViewModel(IUnitOfWorkFactory<IKeycipherLicensingUnitOfWork> unitOfWorkFactory = null)
            : base(unitOfWorkFactory ?? UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Products, x => x.Name)
        {
        }

        public IEntitiesViewModel<Account> LookUpAccounts
        {
            get
            {
                return GetLookUpEntitiesViewModel(
                    (ProductViewModel x) => x.LookUpAccounts,
                    x => x.Accounts);
            }
        }

        public static ProductViewModel Create(IUnitOfWorkFactory<IKeycipherLicensingUnitOfWork> unitOfWorkFactory =
            null)
        {
            return ViewModelSource.Create(() => new ProductViewModel(unitOfWorkFactory));
        }
    }

    public partial class ProductViewModel
    {
        private LinksViewModel linksViewModel;

        public LinksViewModel LinksViewModel
        {
            get
            {
                if (linksViewModel == null)
                {
                    linksViewModel = LinksViewModel.Create();
                }

                return linksViewModel;
            }
        }

        protected IDocumentManagerService DocumentManagerService => this.GetRequiredService<IDocumentManagerService>();

        protected override bool TryClose()
        {
            var closed = base.TryClose();
            if (closed)
            {
                DocumentManagerService.Documents.First(x => x.Content == this).DestroyOnClose = true;
            }

            return closed;
        }

        protected override string GetTitle()
        {
            return Entity.Name;
        }
    }
}