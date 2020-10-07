using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Keycipher.ManagementApp.ViewModels;

namespace Keycipher.ManagementApp.Converter
{
    public class ModuleDescriptionToBoolConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is KeycipherModuleDescription;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}