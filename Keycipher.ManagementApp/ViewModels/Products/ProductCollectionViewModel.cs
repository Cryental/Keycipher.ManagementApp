using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.ViewModel;
using Keycipher.ManagementApp.Common;
using Keycipher.ManagementApp.DataModel;

namespace Keycipher.ManagementApp.ViewModels.Products
{
    public partial class ProductsCollectionViewModel : CollectionViewModel<Product, long, IKeycipherLicensingUnitOfWork>
    {
        protected ProductsCollectionViewModel(IUnitOfWorkFactory<IKeycipherLicensingUnitOfWork> unitOfWorkFactory =
                null,
            UnitOfWorkPolicy unitOfWorkPolicy = UnitOfWorkPolicy.Individual)
            : base(unitOfWorkFactory ?? UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Products,
                unitOfWorkPolicy: unitOfWorkPolicy)
        {
        }

        public static ProductsCollectionViewModel Create(
            IUnitOfWorkFactory<IKeycipherLicensingUnitOfWork> unitOfWorkFactory = null,
            UnitOfWorkPolicy unitOfWorkPolicy = UnitOfWorkPolicy.Individual)
        {
            return ViewModelSource.Create(() => new ProductsCollectionViewModel(unitOfWorkFactory, unitOfWorkPolicy));
        }
    }

    public partial class ProductsCollectionViewModel
    {
        private ViewSettingsViewModel viewSettings;

        public virtual Product TableViewSelectedEntity { get; set; }
        public virtual Product CardViewSelectedEntity { get; set; }

        public ViewSettingsViewModel ViewSettings
        {
            get
            {
                if (viewSettings == null)
                {
                    viewSettings = ViewSettingsViewModel.Create(CollectionViewKind.ListView, this);
                }

                return viewSettings;
            }
        }

        public bool CanEditProduct()
        {
            return SelectedEntity != null;
        }

        public virtual void OnTableViewSelectedEntityChanged()
        {
            if (viewSettings.ViewKind == CollectionViewKind.ListView)
            {
                SelectedEntity = TableViewSelectedEntity;
            }
        }

        public virtual void OnCardViewSelectedEntityChanged()
        {
            if (viewSettings.ViewKind == CollectionViewKind.CardView)
            {
                SelectedEntity = CardViewSelectedEntity;
            }
        }
    }
}