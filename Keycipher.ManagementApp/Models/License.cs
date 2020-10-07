using System.ComponentModel;
using DevExpress.Mvvm;
using Keycipher.ManagementApp.Models.Enums;
using Keycipher.Models;

namespace Keycipher.ManagementApp.Models
{
    public class License : BindableBase, IDocumentContent
    {
        public License(Licenses.Object license)
        {
            UpdateLicense(license);
        }

        public Licenses.Object APILicense { get; private set; }

        public string Code
        {
            get => GetProperty(() => Code);
            set => SetProperty(() => Code, value);
        }

        public LicenseMode LicenseMode
        {
            get => GetProperty(() => LicenseMode);
            set => SetProperty(() => LicenseMode, value);
        }

        public HardwareMode HardwareMode
        {
            get => GetProperty(() => HardwareMode);
            set => SetProperty(() => HardwareMode, value);
        }

        public ActivationStatus ActivationStatus
        {
            get => GetProperty(() => ActivationStatus);
            set => SetProperty(() => ActivationStatus, value);
        }

        public ExpirationStatus ExpirationStatus
        {
            get => GetProperty(() => ExpirationStatus);
            set => SetProperty(() => ExpirationStatus, value);
        }

        public BanStatus BanStatus
        {
            get => GetProperty(() => BanStatus);
            set => SetProperty(() => BanStatus, value);
        }

        public string StartDate
        {
            get => GetProperty(() => StartDate);
            set => SetProperty(() => StartDate, value);
        }

        public string EndDate
        {
            get => GetProperty(() => EndDate);
            set => SetProperty(() => EndDate, value);
        }

        public string HardwareId
        {
            get => GetProperty(() => HardwareId);
            set => SetProperty(() => HardwareId, value);
        }

        public string CustomData
        {
            get => GetProperty(() => CustomData);
            set => SetProperty(() => CustomData, value);
        }

        public string Comment
        {
            get => GetProperty(() => Comment);
            set => SetProperty(() => Comment, value);
        }


        public string ActivatedDate
        {
            get => GetProperty(() => ActivatedDate);
            set => SetProperty(() => ActivatedDate, value);
        }

        public string LastLoggedDate
        {
            get => GetProperty(() => LastLoggedDate);
            set => SetProperty(() => LastLoggedDate, value);
        }

        public Geolocation ActivationGeoLocation
        {
            get => GetProperty(() => ActivationGeoLocation);
            set => SetProperty(() => ActivationGeoLocation, value);
        }

        public Geolocation LastLoggedGeoLocation
        {
            get => GetProperty(() => LastLoggedGeoLocation);
            set => SetProperty(() => LastLoggedGeoLocation, value);
        }

        public void UpdateLicense(Licenses.Object updatedLicense)
        {
            APILicense = updatedLicense;
            Code = updatedLicense.License;
            LicenseMode = updatedLicense.IsLifetime ? LicenseMode.Lifetime : LicenseMode.Subscription;
            HardwareMode = updatedLicense.IsHardwareLocked ? HardwareMode.Locked : HardwareMode.Free;
            ActivationStatus = updatedLicense.IsActivated ? ActivationStatus.Active : ActivationStatus.Inactive;
            ExpirationStatus = updatedLicense.IsExpired ? ExpirationStatus.Expired : ExpirationStatus.Valid;
            BanStatus = updatedLicense.IsBanned ? BanStatus.Banned : BanStatus.Allowed;
            if (updatedLicense.Subscription != null)
            {
                StartDate = updatedLicense.Subscription.Starts;
                EndDate = updatedLicense.Subscription.Expires;
                ActivatedDate = updatedLicense.Subscription.ActivatedDate;
                LastLoggedDate = updatedLicense.Subscription.LastLoggedOnDate;
                ActivationGeoLocation = updatedLicense.Subscription.ActivatedIP;
                LastLoggedGeoLocation = updatedLicense.Subscription.LastLoggedOnIP;
            }

            HardwareId = updatedLicense.Extra.LockedTo;
            CustomData = updatedLicense.Extra.CustomData;
            Comment = updatedLicense.Extra.Comment;
        }


        public void Close()
        {
            DocumentOwner.Close(this, false);
        }

        #region IDocumentContent

        public IDocumentOwner DocumentOwner { get; set; }

        public void OnClose(CancelEventArgs e)
        {
        }

        public void OnDestroy()
        {
        }

        public object Title => $"{Code}-{Comment}";

        #endregion
    }
}