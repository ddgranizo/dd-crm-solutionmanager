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
    /// Interaction logic for ConnectionManager.xaml
    /// </summary>
    public partial class ConnectionManager : Window
    {
        private ConnectionManagerViewModel _viewModel;
        public ConnectionManager(List<CrmConnection> connections)
        {
            InitializeComponent();
            this._viewModel = LayoutRoot.Resources["viewModel"] as ConnectionManagerViewModel;
            _viewModel.Initialize(this, connections);
        }

        private void TxtNewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.NewPassword = txtNewPassword.Password;
        }

        private void TxtEditPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = txtEditPassword.Password;
        }
    }
}
