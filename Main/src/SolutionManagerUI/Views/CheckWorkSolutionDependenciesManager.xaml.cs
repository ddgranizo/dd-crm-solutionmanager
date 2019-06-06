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
    public partial class CheckWorkSolutionDependenciesManager : Window
    {
        private CheckWorkSolutionManagerViewModel _viewModel;
        public CheckWorkSolutionDependenciesManager(
            IOrganizationService service,
            CrmConnection currentCrmConnection,
            SolutionManager solutionManager,
            List<Setting> settings,
            List<CrmConnection> connections,
            WorkSolution solution)
        {
            InitializeComponent();
            this._viewModel = LayoutRoot.Resources["viewModel"] as CheckWorkSolutionManagerViewModel;
            _viewModel.Initialize(this, service, currentCrmConnection, solutionManager, settings, connections, solution);

        }

        public CheckWorkSolutionManagerViewModel GetViewModel()
        {
            return _viewModel;
        }

    }
}
