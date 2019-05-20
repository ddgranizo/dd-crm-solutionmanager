using DD.Crm.SolutionManager.Models;
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
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private MainViewModel _viewModel;
        public Main()
        {
            InitializeComponent();
            this._viewModel = LayoutRoot.Resources["viewModel"] as MainViewModel;
            _viewModel.Initialize(this);
        }

        private void SolutionsList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = SolutionsListScrollViewer;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void SolutionComponentsList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = SolutionComponentsListScrollViewer;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void SolutionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var addedItem in e.AddedItems)
            {
                Solution item = addedItem as Solution;
                var already = _viewModel.SelectedSolutions.IndexOf(item) > -1;
                if (!already)
                {
                    _viewModel.SelectedSolutions.Add(item);
                }
            }

            foreach (var removedItem in e.RemovedItems)
            {
                Solution item = removedItem as Solution;
                var index = _viewModel.SelectedSolutions.IndexOf(item);
                if (index > -1)
                {
                    _viewModel.SelectedSolutions.Remove(item);
                }
            }
            _viewModel.RaisePropertyChanged();
        }
    }
}
