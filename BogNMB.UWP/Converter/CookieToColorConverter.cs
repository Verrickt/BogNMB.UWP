using BogNMB.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BogNMB.UWP.Converter
{
    public class CookieToColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            if (value is ThreadViewModel tv)
            {
                if (tv.IsAdmin) return Colors.Red;
            } else if(value is PostViewModel pv)
            {
                if (pv.IsAdmin) return Colors.Red;
            }
            return (Color)Application.Current.Resources["SystemBaseMediumColor"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
