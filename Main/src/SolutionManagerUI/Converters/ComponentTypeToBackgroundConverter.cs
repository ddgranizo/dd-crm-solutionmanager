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
    public class ComponentTypeToBackgroundConverter : IValueConverter
    {
        public ComponentTypeToBackgroundConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolutionComponentType type = (SolutionComponentType)value;
            string color = SolutionComponentBase.GetTypeColor(type);
            var brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(color));
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
