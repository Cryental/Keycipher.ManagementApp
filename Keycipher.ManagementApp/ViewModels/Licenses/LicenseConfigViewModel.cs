using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using Keycipher.ManagementApp.DataModel;
using Keycipher.ManagementApp.Models.Enums;
using Keycipher.ManagementApp.Services;
using License = Keycipher.ManagementApp.Models.License;

namespace Keycipher.ManagementApp.ViewModels.Licenses
{
    public class LicenseConfigViewModel
    {
        public LicenseConfigViewModel(Product product)
        {
            LicenseManager = new Licensing(new Configuration
            {
                ProductID = product.AccessKey,
                AccessKey = product.Account.APIKey,
                PrivateKey = product.PrivateKey,
                PublicKey = product.PublicKey
            });
            Licenses = new ObservableCollection<License>(LicenseManager.GetAllLicenses().Select(p => new License(p))
                .ToList());
            SelectedLicenses = new ObservableCollection<License>();
        }

        [ServiceProperty(Key = "NewLicenseService")]
        public virtual IDialogService NewLicenseService => null;

        [ServiceProperty(Key = "MessageBoxService")]
        public virtual IMessageBoxService MessageBoxService => null;

        [ServiceProperty(Key = "EditDialogService")]
        public virtual IDialogService EditDialogService => null;

        [ServiceProperty(Key = "SaveFileDialogService")]
        public virtual ISaveFileDialogService SaveFileDialogService => null;

        [ServiceProperty(Key = "ExportService")]
        public virtual IExportService ExportService => null;

        [ServiceProperty(Key = "SplashScreenManagerService")]
        public virtual ISplashScreenManagerService SplashScreenManagerService => null;

        [ServiceProperty(Key = "DocumentManagerService")]
        public virtual IDocumentManagerService DocumentManagerService => null;


        public Licensing LicenseManager { get; }


        public ObservableCollection<License> Licenses { get; set; }

        public ObservableCollection<License> SelectedLicenses { get; set; }

        public static LicenseConfigViewModel Create(Product product)
        {
            return ViewModelSource.Create(() => new LicenseConfigViewModel(product));
        }

        public void CreateDocument(object arg)
        {
            var doc = DocumentManagerService.FindDocument(arg);
            if (doc == null)
            {
                doc = DocumentManagerService.CreateDocument("LicenseDetailsView", arg);
                doc.Id = DocumentManagerService.Documents.Count();
            }

            doc.Show();
        }

        #region RibbonCommands

        public void Copy()
        {
            var licenses = "";
            SplashScreenManagerService.Show();
            foreach (var license in SelectedLicenses)
            {
                licenses += license.Code + Environment.NewLine;
            }

            Clipboard.SetText(licenses);
            SplashScreenManagerService.Close();
        }

        public void Generate()
        {
            var dialog = NewLicenseViewModel.Create();
            var OkCommand = new UICommand
            {
                Id = MessageBoxResult.OK,
                Caption = "Generate",
                IsCancel = false,
                IsDefault = true,
                Command = new DelegateCommand<CancelEventArgs>(GenerateLicense, CanGenerate, true)
            };

            void GenerateLicense(CancelEventArgs ar)
            {
                SplashScreenManagerService.Show();

                var generatedLicenses = LicenseManager.GenerateLicense(dialog.LicensesCount,
                    dialog.HardwareMode == HardwareMode.Locked, dialog.ExpireHour,
                    dialog.LicenseMode == LicenseMode.Lifetime, dialog.CustomData, dialog.Comment);
                generatedLicenses.ForEach(l => Licenses.Add(new License(l)));

                SplashScreenManagerService.Close();
            }

            bool CanGenerate(CancelEventArgs ar)
            {
                if (dialog.LicensesCount == 0)
                {
                    return false;
                }

                if (dialog.LicenseMode == LicenseMode.Subscription && dialog.ExpireHour == 0)
                {
                    return false;
                }

                return true;
            }

            var CancelCommand = new UICommand
            {
                Id = MessageBoxResult.Cancel, Caption = "Cancel", IsCancel = true, IsDefault = false
            };
            NewLicenseService.ShowDialog(
                new List<UICommand> {OkCommand, CancelCommand},
                "Generate Licenses Dialog",
                dialog);
        }

        public void Ban()
        {
            var result =
                MessageBoxService.Show("Ban Selected Licenses", "UnBan Confirmation", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                SplashScreenManagerService.Show();
                foreach (var license in SelectedLicenses)
                {
                    if (license.BanStatus == BanStatus.Allowed)
                    {
                        license.APILicense.Ban();
                        license.UpdateLicense(license.APILicense.RefreshData());
                    }
                }

                SplashScreenManagerService.Close();
            }
        }

        public void Unban()
        {
            var result = MessageBoxService.Show("UnBan Selected Licenses", "UnBan Confirmation",
                MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                SplashScreenManagerService.Show();
                foreach (var license in SelectedLicenses)
                {
                    if (license.BanStatus == BanStatus.Banned)
                    {
                        license.APILicense.Unban();
                        license.UpdateLicense(license.APILicense.RefreshData());
                    }
                }

                SplashScreenManagerService.Close();
            }
        }

        public void Delete()
        {
            var result = MessageBoxService.Show("Deleted Selected Licenses", "Delete Confirmation",
                MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                SplashScreenManagerService.Show();
                var count = SelectedLicenses.Count;
                for (var index = 0; index < count; index++)
                {
                    LicenseManager.DeleteLicense(SelectedLicenses[0].APILicense.License);
                    Licenses.Remove(SelectedLicenses[0]);
                }

                SplashScreenManagerService.Close();
            }
        }

        public void ExtendHours()
        {
            var dialog = EditSingleValueViewModel.Create("Extra Hours Count");
            var OkCommand = new UICommand
            {
                Id = MessageBoxResult.OK,
                Caption = "Extend",
                IsCancel = false,
                IsDefault = true,
                Command = new DelegateCommand<CancelEventArgs>(ExtendHours, CanExtendHours, true)
            };

            void ExtendHours(CancelEventArgs ar)
            {
                SplashScreenManagerService.Show();
                foreach (var license in SelectedLicenses)
                {
                    license.APILicense.ExtendHours(int.Parse(dialog.Input));
                    license.UpdateLicense(license.APILicense.RefreshData());
                }

                SplashScreenManagerService.Close();
            }

            bool CanExtendHours(CancelEventArgs ar)
            {
                if (string.IsNullOrEmpty(dialog.Input))
                {
                    return false;
                }

                if (int.TryParse(dialog.Input, out var hours))
                {
                    return hours > 0 && hours < 99999;
                }

                return false;
            }

            var CancelCommand = new UICommand
            {
                Id = MessageBoxResult.Cancel, Caption = "Cancel", IsCancel = true, IsDefault = false
            };

            EditDialogService.ShowDialog(
                new List<UICommand> {OkCommand, CancelCommand},
                "Extend Hours Dialog",
                dialog);
        }

        public void EditHours()
        {
            var dialog = EditSingleValueViewModel.Create("Expire Hours");
            var OkCommand = new UICommand
            {
                Id = MessageBoxResult.OK,
                Caption = "Modify",
                IsCancel = false,
                IsDefault = true,
                Command = new DelegateCommand<CancelEventArgs>(EditHours, CanEditHours, true)
            };

            void EditHours(CancelEventArgs ar)
            {
                SplashScreenManagerService.Show();
                foreach (var license in SelectedLicenses)
                {
                    license.APILicense.ModifyHours(int.Parse(dialog.Input));
                    license.UpdateLicense(license.APILicense.RefreshData());
                }

                SplashScreenManagerService.Close();
            }

            bool CanEditHours(CancelEventArgs ar)
            {
                if (string.IsNullOrEmpty(dialog.Input))
                {
                    return false;
                }

                if (int.TryParse(dialog.Input, out var hours))
                {
                    return hours > 0 && hours < 99999;
                }

                return false;
            }

            var CancelCommand = new UICommand
            {
                Id = MessageBoxResult.Cancel, Caption = "Cancel", IsCancel = true, IsDefault = false
            };

            EditDialogService.ShowDialog(
                new List<UICommand> {OkCommand, CancelCommand},
                "Edit Hours Dialog",
                dialog);
        }

        public void EditComment()
        {
            var dialog = EditSingleValueViewModel.Create("Modified Comment");
            var OkCommand = new UICommand
            {
                Id = MessageBoxResult.OK,
                Caption = "Modify",
                IsCancel = false,
                IsDefault = true,
                Command = new DelegateCommand<CancelEventArgs>(ModifyComment)
            };

            void ModifyComment(CancelEventArgs ar)
            {
                SplashScreenManagerService.Show();
                foreach (var license in SelectedLicenses)
                {
                    license.APILicense.ModifyComment(dialog.Input);
                    license.UpdateLicense(license.APILicense.RefreshData());
                }

                SplashScreenManagerService.Close();
            }


            var CancelCommand = new UICommand
            {
                Id = MessageBoxResult.Cancel, Caption = "Cancel", IsCancel = true, IsDefault = false
            };

            EditDialogService.ShowDialog(
                new List<UICommand> {OkCommand, CancelCommand},
                "Edit Comment Dialog",
                dialog);
        }

        public void EditCustomData()
        {
            var dialog = EditSingleValueViewModel.Create("Modified Custom Data");
            var OkCommand = new UICommand
            {
                Id = MessageBoxResult.OK,
                Caption = "Modify",
                IsCancel = false,
                IsDefault = true,
                Command = new DelegateCommand<CancelEventArgs>(ModifyComment)
            };

            void ModifyComment(CancelEventArgs ar)
            {
                SplashScreenManagerService.Show();
                foreach (var license in SelectedLicenses)
                {
                    license.APILicense.ModifyCustomData(dialog.Input);
                    license.UpdateLicense(license.APILicense.RefreshData());
                }

                SplashScreenManagerService.Close();
            }


            var CancelCommand = new UICommand
            {
                Id = MessageBoxResult.Cancel, Caption = "Cancel", IsCancel = true, IsDefault = false
            };

            EditDialogService.ShowDialog(
                new List<UICommand> {OkCommand, CancelCommand},
                "Edit Custom Data Dialog Dialog",
                dialog);
        }

        public void LockHardware()
        {
            var result = MessageBoxService.Show("Lock Hardware for Selected Licenses", "Lock Confirmation",
                MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                SplashScreenManagerService.Show();
                foreach (var license in SelectedLicenses)
                {
                    if (license.HardwareMode == HardwareMode.Free)
                    {
                        license.APILicense.SetHardwareLockStatus(true);
                        license.UpdateLicense(license.APILicense.RefreshData());
                    }
                }

                SplashScreenManagerService.Close();
            }
        }

        public void UnlockHardware()
        {
            var result = MessageBoxService.Show("Unlock Hardware for Selected Licenses", "Unlock Confirmation",
                MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                SplashScreenManagerService.Show();
                foreach (var license in SelectedLicenses)
                {
                    if (license.HardwareMode == HardwareMode.Locked)
                    {
                        license.APILicense.SetHardwareLockStatus(false);
                        license.UpdateLicense(license.APILicense.RefreshData());
                    }
                }

                SplashScreenManagerService.Close();
            }
        }

        public void ReleaseHardware()
        {
            var result = MessageBoxService.Show("Release Hardware for Selected Licenses", "Release Confirmation",
                MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                SplashScreenManagerService.Show();
                foreach (var license in SelectedLicenses)
                {
                    if (!string.IsNullOrEmpty(license.HardwareId))
                    {
                        license.APILicense.ReleaseHardwareID();
                        license.UpdateLicense(license.APILicense.RefreshData());
                    }
                }

                SplashScreenManagerService.Close();
            }
        }

        public void ExportGrid(ExportType fileType)
        {
            switch (fileType)
            {
                case ExportType.XLSX:
                    SaveFileDialogService.DefaultExt = "xlsx";
                    SaveFileDialogService.Filter = "Excel 2007+|*.xlsx";
                    break;
                case ExportType.PDF:
                    SaveFileDialogService.DefaultExt = "pdf";
                    SaveFileDialogService.Filter = "PDF|*.pdf";
                    break;
            }

            if (SaveFileDialogService.ShowDialog())
            {
                var fileName = SaveFileDialogService.GetFullFileName();
                ExportService.ExportTo(fileType, fileName);
                if (MessageBoxService.ShowMessage("Would you like to open the file?", "Export finished",
                    MessageButton.YesNo) == MessageResult.Yes)
                {
                    Process.Start(fileName);
                }
            }
        }

        public void ShowPreview()
        {
            ExportService.ShowPreview();
        }

        #endregion
    }
}