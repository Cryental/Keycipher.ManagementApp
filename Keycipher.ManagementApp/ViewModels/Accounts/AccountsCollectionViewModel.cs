using System;
using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.ViewModel;
using Keycipher.ManagementApp.Common;
using Keycipher.ManagementApp.DataModel;

namespace Keycipher.ManagementApp.ViewModels.Accounts
{
    public partial class AccountsCollectionViewModel : CollectionViewModel<Account, long, IKeycipherLicensingUnitOfWork>
    {
        protected AccountsCollectionViewModel(IUnitOfWorkFactory<IKeycipherLicensingUnitOfWork> unitOfWorkFactory =
                null,
            UnitOfWorkPolicy unitOfWorkPolicy = UnitOfWorkPolicy.Individual)
            : base(unitOfWorkFactory ?? UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Accounts,
                unitOfWorkPolicy: unitOfWorkPolicy)
        {
        }

        public static AccountsCollectionViewModel Create(
            IUnitOfWorkFactory<IKeycipherLicensingUnitOfWork> unitOfWorkFactory = null,
            UnitOfWorkPolicy unitOfWorkPolicy = UnitOfWorkPolicy.Individual)
        {
            return ViewModelSource.Create(() => new AccountsCollectionViewModel(unitOfWorkFactory, unitOfWorkPolicy));
        }
    }

    public partial class AccountsCollectionViewModel
    {
        private AccountPanelViewModel accountPanelViewModel;
        private ViewSettingsViewModel viewSettings;

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

        public AccountPanelViewModel AccountPanelViewModel
        {
            get
            {
                if (accountPanelViewModel == null)
                {
                    accountPanelViewModel = AccountPanelViewModel.Create();
                }

                return accountPanelViewModel;
            }
        }


        public virtual Account TableViewSelectedEntity { get; set; }
        public virtual Account CardViewSelectedEntity { get; set; }


        public bool CanEditAccount()
        {
            return SelectedEntity != null;
        }


        protected override void OnSelectedEntityChanged()
        {
            base.OnSelectedEntityChanged();
            if (SelectedEntity != null)
            {
                AccountPanelViewModel.Entity = SelectedEntity;
            }

            if (SelectedEntity != null)
            {
                TableViewSelectedEntity = SelectedEntity;
            }

            CardViewSelectedEntity = SelectedEntity;
        }

        public override void UpdateSelectedEntity()
        {
            base.UpdateSelectedEntity();
            AccountPanelViewModel.RaisePropertyChanged(x => x.Entity);
        }

        protected override void OnEntitiesAssigned(Func<Account> getSelectedEntityCallback)
        {
            base.OnEntitiesAssigned(getSelectedEntityCallback);
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