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
    public class ComponentBehaviorIncludeMetadataToVisibilityConverter : IValueConverter
    {
        public ComponentBehaviorIncludeMetadataToVisibilityConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value!=null && value is RootComponentBehaviorType)
            {
                var typed = (RootComponentBehaviorType)value;
                if (typed == RootComponentBehaviorType.DoNotIncludeSubcomponents)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
