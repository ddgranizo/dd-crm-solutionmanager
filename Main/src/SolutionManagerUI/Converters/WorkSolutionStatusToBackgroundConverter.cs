using DD.Crm.SolutionManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SolutionManagerUI.Converters
{
    public class WorkSolutionStatusToBackgroundConverter : IValueConverter
    {
        public WorkSolutionStatusToBackgroundConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            WorkSolution.WorkSolutionStatus status = (WorkSolution.WorkSolutionStatus)value;
            string color = "#FFFFFF";
            if (status == WorkSolution.WorkSolutionStatus.Development)
            {
                color = "#FFFF84";
            }
            else if(status == WorkSolution.WorkSolutionStatus.ReadyToInt)
            {
                color ="#D1FFB3";
            }
            var brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(color));
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
