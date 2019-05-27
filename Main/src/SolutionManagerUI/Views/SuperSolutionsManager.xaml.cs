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
    public partial class SuperSolutionsManager : Window
    {
        private SuperSolutionsManagerViewModel _viewModel;
        public SuperSolutionsManager(
            IOrganizationService service,
            CrmConnection currentCrmConnection,
            SolutionManager solutionManager,
            List<Setting> settings,
            List<MergedInSolutionComponent> solutionComponents,
            AggregatedSolution aggregatedSolution)
        {
            InitializeComponent();
            this._viewModel = LayoutRoot.Resources["viewModel"] as SuperSolutionsManagerViewModel;
            _viewModel.Initialize(this, service, currentCrmConnection, solutionManager, settings, solutionComponents, aggregatedSolution);

        }


        public SuperSolutionsManagerViewModel GetViewModel()
        {
            return _viewModel;
        }

        private void SuperSolutionComponentsList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = SuperSolutionComponentsListScrollViewer;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ComponentsList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = ComponentsListScrollViewer;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
