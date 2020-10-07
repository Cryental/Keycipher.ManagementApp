using System;
using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.DataModel.EF6;

namespace Keycipher.ManagementApp.DataModel
{
    public static class UnitOfWorkSource
    {
        public static IUnitOfWorkFactory<IKeycipherLicensingUnitOfWork> GetUnitOfWorkFactory()
        {
            Func<KeycipherLicensingDBEntities> contextFactory = () => new KeycipherLicensingDBEntities();
            return new DbUnitOfWorkFactory<IKeycipherLicensingUnitOfWork>(() =>
                new KeycipherLicensingUnitOfWork(contextFactory));
        }
    }
}