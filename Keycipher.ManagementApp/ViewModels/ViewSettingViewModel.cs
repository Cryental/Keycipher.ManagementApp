using System.Windows.Controls;
using DevExpress.Mvvm.POCO;

namespace Keycipher.ManagementApp.ViewModels
{
    public class ViewSettingsViewModel
    {
        private readonly CollectionViewKind defaultCollectionViewKind;

        protected ViewSettingsViewModel(CollectionViewKind defaultCollectionViewKind)
        {
            this.defaultCollectionViewKind = defaultCollectionViewKind;
            ResetView();
        }

        public virtual CollectionViewKind ViewKind { get; set; }
        public virtual Orientation Orientation { get; set; }
        public virtual bool IsDataPaneVisible { get; set; }

        public static ViewSettingsViewModel Create(CollectionViewKind viewKind, object parentViewModel)
        {
            return ViewModelSource.Create(() => new ViewSettingsViewModel(viewKind))
                .SetParentViewModel(parentViewModel);
        }

        public void ResetView()
        {
            ViewKind = defaultCollectionViewKind;
            Orientation = Orientation.Horizontal;
            IsDataPaneVisible = true;
        }

        public bool CanShowList()
        {
            return !Equals(ViewKind, CollectionViewKind.ListView);
        }

        public bool CanShowCard()
        {
            return !Equals(ViewKind, CollectionViewKind.CardView);
        }

        public bool CanShowMasterDetail()
        {
            return !Equals(ViewKind, CollectionViewKind.MasterDetailView);
        }

        public void ShowList()
        {
            ViewKind = CollectionViewKind.ListView;
        }

        public void ShowCard()
        {
            ViewKind = CollectionViewKind.CardView;
        }

        public void ShowMasterDetail()
        {
            ViewKind = CollectionViewKind.MasterDetailView;
        }

        public void DataPaneRight()
        {
            Orientation = Orientation.Horizontal;
            IsDataPaneVisible = true;
        }

        public bool CanDataPaneRight()
        {
            return Orientation != Orientation.Horizontal || !IsDataPaneVisible;
        }

        public void DataPaneBottom()
        {
            Orientation = Orientation.Vertical;
            IsDataPaneVisible = true;
        }

        public bool CanDataPaneBottom()
        {
            return Orientation != Orientation.Vertical || !IsDataPaneVisible;
        }

        public void DataPaneOff()
        {
            IsDataPaneVisible = false;
        }

        public bool CanDataPaneOff()
        {
            return IsDataPaneVisible;
        }
    }
}