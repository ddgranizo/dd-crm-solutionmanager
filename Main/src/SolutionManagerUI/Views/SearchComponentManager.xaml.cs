using DD.Crm.SolutionManager;
using DD.Crm.SolutionManager.Models;
using Microsoft.Xrm.Sdk;
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
    /// Interaction logic for MergeSolutionsManager.xaml
    /// </summary>
    public partial class SearchComponentManager : Window
    {
        private SearchComponentManagerViewModel _viewModel;
        public SearchComponentManager(
            IOrganizationService service,
            CrmConnection currentCrmConnection,
            SolutionManager solutionManager,
            List<Setting> settings)
        {
            InitializeComponent();
            this._viewModel = LayoutRoot.Resources["viewModel"] as SearchComponentManagerViewModel;
            _viewModel.Initialize(this, service, currentCrmConnection, solutionManager, settings);
        }

        public SearchComponentManagerViewModel GetViewModel()
        {
            return _viewModel;
        }

    }
}
