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
    public class AggregatedSolutionStatusToBackgroundConverter : IValueConverter
    {
        public AggregatedSolutionStatusToBackgroundConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AggregatedSolution.AggregatedSolutionStatus status = (AggregatedSolution.AggregatedSolutionStatus)value;
            string color = "#FFFFFF";
            if (status == AggregatedSolution.AggregatedSolutionStatus.Development)
            {
                color = "#FFFF84";
            }
            else if(status == AggregatedSolution.AggregatedSolutionStatus.ClosedDevelopment)
            {
                color ="#D1FFB3";
            }
            else if (status == AggregatedSolution.AggregatedSolutionStatus.StagingAndIntegration)
            {
                color = "#fa9200";
            }
            else if (status == AggregatedSolution.AggregatedSolutionStatus.Preproduction)
            {
                color = "#32cd32";
            }
            else if (status == AggregatedSolution.AggregatedSolutionStatus.Production)
            {
                color = "#00bfff";
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
