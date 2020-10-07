using System;
using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.DataModel.EF6;

namespace Keycipher.ManagementApp.DataModel
{
    public class KeycipherLicensingUnitOfWork : DbUnitOfWork<KeycipherLicensingDBEntities>,
        IKeycipherLicensingUnitOfWork
    {
        public KeycipherLicensingUnitOfWork(Func<KeycipherLicensingDBEntities> contextFactory)
            : base(contextFactory)
        {
        }

        IRepository<Account, long> IKeycipherLicensingUnitOfWork.Accounts
        {
            get
            {
                return GetRepository(x => x.Set<Account>(), x => x.Id);
            }
        }

        IRepository<Product, long> IKeycipherLicensingUnitOfWork.Products
        {
            get
            {
                return GetRepository(x => x.Set<Product>(), x => x.Id);
            }
        }
    }
}