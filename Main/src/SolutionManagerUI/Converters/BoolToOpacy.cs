using DD.Crm.SolutionManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using static DD.Crm.SolutionManager.Models.SolutionComponentBase;

namespace SolutionManagerUI.Converters
{
    public class BoolToOpacy : IValueConverter
    {
        public BoolToOpacy()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isIn = (bool)value;
            if (!isIn)
            {
                return 0.3;
            }
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
