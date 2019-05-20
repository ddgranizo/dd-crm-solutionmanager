using DD.Crm.SolutionManager.Models;
using SolutionManagerUI.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SolutionManagerUI.ViewModels
{
    public class SolutionComponentViewModel : BaseViewModel
    {


        private MergedInSolutionComponent _component = null;
        public MergedInSolutionComponent Component
        {
            get
            {
                return _component;
            }
            set
            {
                _component = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Component"));
            }
        }

        public SolutionComponentViewModel()
        {
        }
        private UserControl _window;

        public void Initialize(UserControl w)
        {
            _window = w;
        }
    }
}
