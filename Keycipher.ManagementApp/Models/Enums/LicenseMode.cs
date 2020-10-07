using System.ComponentModel.DataAnnotations;

namespace Keycipher.ManagementApp.Models.Enums
{
    public enum LicenseMode
    {
        [Display(Name = "Lifetime", Description = "", Order = 1)]
        Lifetime, [Display(Name = "Subscription", Description = "", Order = 2)]
        Subscription
    }
}