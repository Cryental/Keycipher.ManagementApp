using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.POCO;
using Keycipher.ManagementApp.Common;
using Keycipher.ManagementApp.DataModel;

namespace Keycipher.ManagementApp.ViewModels.Accounts
{
    public partial class AccountViewModel : SingleObjectViewModel<Account, long, IKeycipherLicensingUnitOfWork>
    {
        protected AccountViewModel(IUnitOfWorkFactory<IKeycipherLicensingUnitOfWork> unitOfWorkFactory = null)
            : base(unitOfWorkFactory ?? UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Accounts, x => x.Owner)
        {
        }

        public static AccountViewModel Create(IUnitOfWorkFactory<IKeycipherLicensingUnitOfWork> unitOfWorkFactory =
            null)
        {
            return ViewModelSource.Create(() => new AccountViewModel(unitOfWorkFactory));
        }
    }

    public partial class AccountViewModel
    {
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
            return Entity.Owner;
        }
    }
}