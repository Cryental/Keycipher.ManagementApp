using DevExpress.Mvvm.POCO;
using Keycipher.ManagementApp.Models.Enums;

namespace Keycipher.ManagementApp.ViewModels.Licenses
{
    public class NewLicenseViewModel
    {
        public NewLicenseViewModel()
        {
            LicensesCount = 1;
            ExpireHour = 999;
        }

        public virtual int LicensesCount { get; set; }

        public virtual int ExpireHour { get; set; }

        public virtual HardwareMode HardwareMode { get; set; }

        public virtual LicenseMode LicenseMode { get; set; }

        public virtual string CustomData { get; set; }

        public virtual string Comment { get; set; }

        public static NewLicenseViewModel Create()
        {
            return ViewModelSource.Create(() => new NewLicenseViewModel());
        }

        protected void OnLicenseModeChanged(LicenseMode oldValue)
        {
            if (oldValue != LicenseMode.Lifetime)
            {
                ExpireHour = 999;
            }
        }
    }
}