using DevExpress.Mvvm.DataModel;

namespace Keycipher.ManagementApp.DataModel
{
    public interface IKeycipherLicensingUnitOfWork : IUnitOfWork
    {
        IRepository<Account, long> Accounts { get; }
        IRepository<Product, long> Products { get; }
    }
}