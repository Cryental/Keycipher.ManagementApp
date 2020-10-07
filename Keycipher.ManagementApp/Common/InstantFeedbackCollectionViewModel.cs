using System;
using System.Linq;
using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.ViewModel;

namespace Keycipher.ManagementApp.Common
{
    public class
        InstantFeedbackCollectionViewModel<TEntity, TPrimaryKey, TUnitOfWork> : InstantFeedbackCollectionViewModelBase<
            TEntity, TEntity, TPrimaryKey, TUnitOfWork>
        where TEntity : class, new()
        where TUnitOfWork : IUnitOfWork
    {
        protected InstantFeedbackCollectionViewModel(IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory,
            Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc,
            Func<IRepositoryQuery<TEntity>, IQueryable<TEntity>> projection = null,
            Func<bool> canCreateNewEntity = null)
            : base(unitOfWorkFactory, getRepositoryFunc, projection, canCreateNewEntity)
        {
        }

        public static InstantFeedbackCollectionViewModel<TEntity, TPrimaryKey, TUnitOfWork>
            CreateInstantFeedbackCollectionViewModel(IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory,
                Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc,
                Func<IRepositoryQuery<TEntity>, IQueryable<TEntity>> projection = null,
                Func<bool> canCreateNewEntity = null)
        {
            return ViewModelSource.Create(() =>
                new InstantFeedbackCollectionViewModel<TEntity, TPrimaryKey, TUnitOfWork>(unitOfWorkFactory,
                    getRepositoryFunc, projection, canCreateNewEntity));
        }
    }
}