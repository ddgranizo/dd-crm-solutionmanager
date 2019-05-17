using SolutionManagerUI.Models;
using SolutionManagerUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SolutionManagerUI.Views
{
    /// <summary>
    /// Interaction logic for SettingManager.xaml
    /// </summary>
    public partial class SettingManager : Window
    {
        private SettingManagerViewModel _viewModel;
        public SettingManager(List<Setting> settings)
        {
            InitializeComponent();
            this._viewModel = LayoutRoot.Resources["viewModel"] as SettingManagerViewModel;
            _viewModel.Initialize(this, settings);
        }
    }
}
