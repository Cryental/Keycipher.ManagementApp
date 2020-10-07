using DevExpress.Mvvm.POCO;

namespace Keycipher.ManagementApp.ViewModels.Licenses
{
    public class EditSingleValueViewModel
    {
        public EditSingleValueViewModel(string inputName)
        {
            InputName = inputName;
        }

        public string InputName { get; }

        public string Input { get; set; }

        public static EditSingleValueViewModel Create(string inputName)
        {
            return ViewModelSource.Create(() => new EditSingleValueViewModel(inputName));
        }
    }
}