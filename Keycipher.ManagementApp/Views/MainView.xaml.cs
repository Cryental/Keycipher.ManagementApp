using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Keycipher.ManagementApp.Views
{
    /// <summary>
    ///     Interaction logic for View1.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }


        public class ObjectsEqualityConverter : IValueConverter
        {
            public bool Inverse { get; set; }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null)
                {
                    return value;
                }

                var result = Equals(value.ToString(), parameter);
                return Inverse ? !result : result;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (bool) value ? parameter : null;
            }
        }
    }
}