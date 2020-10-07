using System;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.ViewModel;
using Keycipher.ManagementApp.Common.ViewModel;
using Keycipher.ManagementApp.DataModel;

namespace Keycipher.ManagementApp.ViewModels
{
    public class MainViewModel : DocumentsViewModel<KeycipherModuleDescription, IKeycipherLicensingUnitOfWork>
    {
        private readonly Workspace defaultWorkspace = new Workspace();


        private LinksViewModel linksViewModel;

        protected MainViewModel() : base(UnitOfWorkSource.GetUnitOfWorkFactory())
        {
        }

        private IDocumentManagerService SignleObjectDocumentManagerService =>
            this.GetService<IDocumentManagerService>("SignleObjectDocumentManagerService");

        private IMainWindowService MainWindowService => this.GetRequiredService<IMainWindowService>();
        private IWorkspaceService WorkspaceService => this.GetRequiredService<IWorkspaceService>();

        private IDispatcherService FinishModuleChangingDispatcherService =>
            this.GetRequiredService<IDispatcherService>("FinishModuleChangingDispatcherService");

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

        public static MainViewModel Create()
        {
            return ViewModelSource.Create(() => new MainViewModel());
        }


        protected override KeycipherModuleDescription[] CreateModules()
        {
            return new[]
            {
                new KeycipherModuleDescription("Accounts", "AccountsCollectionView", "Tables"),
                new KeycipherModuleDescription("Products", "ProductsCollectionView", "Tables"),
                new KeycipherModuleDescription("Licenses", "LicensesCollectionView", "Tables")
            };
        }

        public override void SaveLogicalLayout()
        {
        }

        public override bool RestoreLogicalLayout()
        {
            return false;
        }

        protected override void OnActiveModuleChanged(KeycipherModuleDescription oldModule)
        {
            base.OnActiveModuleChanged(oldModule);

            var title = Convert.ToString(DocumentManagerService.ActiveDocument.Title);

            MainWindowService.Title = title + " - Keycipher Licensing";

            FinishModuleChangingDispatcherService.BeginInvoke(() =>
            {
                UpdateWorkspace(oldModule, ActiveModule);
            });
        }

        private void UpdateWorkspace(KeycipherModuleDescription oldModule, KeycipherModuleDescription newModule)
        {
            var oldWorkspace = WorkspaceService.SaveWorkspace();
            var newWorkspace = new Workspace();
            try
            {
                if (oldModule != null)
                {
                    oldModule.Workspace = oldWorkspace;
                }

                if (newModule != null)
                {
                    newWorkspace = newModule.Workspace ?? defaultWorkspace;
                }
            }
            finally
            {
                WorkspaceService.RestoreWorkspace(newWorkspace);
            }
        }
    }


    public class KeycipherModuleDescription : ModuleDescription<KeycipherModuleDescription>
    {
        public KeycipherModuleDescription(string title, string documentType, string group) : base(title, documentType,
            group)
        {
        }

        public Workspace Workspace { get; set; }
    }
}