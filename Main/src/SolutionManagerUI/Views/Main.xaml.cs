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

        private void WorkSolutionsList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = WorkSolutionsListScrollViewer;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void SolutionComponentsList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = SolutionComponentsListScrollViewer;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void WorkSolutionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedMultiple(e, _viewModel.SelectedSolutions);
            _viewModel.RaisePropertyChanged();
        }

        private void SolutionComponentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SolutionComponentsList.UpdateLayout();
            if (e.AddedItems.Count == 1)
            {
                MergedInSolutionComponent item = e.AddedItems[0] as MergedInSolutionComponent;
                var listViewItem =
                    SolutionComponentsList.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
                listViewItem.Focus();
            }

        }

        private void AggregatedSolutionsList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = AggregatedSolutionsListScrollViewer;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void AggregatedSolutionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedMultiple(e, _viewModel.SelectedAggregatedSolutions);
            _viewModel.RaisePropertyChanged();
            _viewModel.ReloadWorkSolutions();
        }

        private void UpdateSelectedMultiple<T>(SelectionChangedEventArgs e, List<T> target)
        {
            foreach (var addedItem in e.AddedItems)
            {
                T item = (T)addedItem;
                var already = target.IndexOf(item) > -1;
                if (!already)
                {
                    target.Add(item);
                }
            }
            foreach (var removedItem in e.RemovedItems)
            {
                T item = (T)removedItem;
                var index = target.IndexOf(item);
                if (index > -1)
                {
                    target.Remove(item);
                }
            }
            _viewModel.RaisePropertyChanged();
        }

    }
}
