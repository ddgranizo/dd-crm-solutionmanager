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
    /// Interaction logic for MergeManager.xaml
    /// </summary>
    public partial class MergeManager : Window
    {
        private MergeManagerViewModel _viewModel;
        public MergeManager(
            IOrganizationService service,
            CrmConnection currentCrmConnection,
            SolutionManager solutionManager,
            List<Solution> selectedSolutions,
            List<Solution> allSolutions,
            List<MergedInSolutionComponent> solutionComponents)
        {
            InitializeComponent();
            this._viewModel = LayoutRoot.Resources["viewModel"] as MergeManagerViewModel;
            _viewModel.Initialize(this, service, currentCrmConnection, solutionManager, selectedSolutions, allSolutions, solutionComponents);

        }


        public MergeManagerViewModel GetViewModel()
        {
            return _viewModel;
        }

        private void SolutionComponentsList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = SolutionComponentsListScrollViewer;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        

        private void SolutionList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = SolutionListScrollViewer;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void SolutionList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ICommand c = _viewModel.OpenSolutionInBrowserCommand;
            if (c.CanExecute(null))
            {
                c.Execute(null);
            }
        }


    }
}
