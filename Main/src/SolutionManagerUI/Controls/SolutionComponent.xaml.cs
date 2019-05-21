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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SolutionManagerUI.Controls
{
    /// <summary>
    /// Interaction logic for SolutionComponent.xaml
    /// </summary>
    public partial class SolutionComponent : UserControl
    {


        public bool ShowInTree
        {
            get
            {
                return (bool)GetValue(ShowInTreeDataProperty);
            }
            set
            {
                SetValue(ShowInTreeDataProperty, value);
            }
        }

        public static readonly DependencyProperty ShowInTreeDataProperty =
                     DependencyProperty.Register(
                         "ShowInTree",
                         typeof(bool),
                         typeof(SolutionComponent), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnValueChanged))
                         {
                             BindsTwoWayByDefault = true,
                         });


        public MergedInSolutionComponent Component
        {
            get
            {
                return (MergedInSolutionComponent)GetValue(ComponentDataProperty);
            }
            set
            {
                SetValue(ComponentDataProperty, value);
            }
        }

        public static readonly DependencyProperty ComponentDataProperty =
                     DependencyProperty.Register(
                         "Component",
                         typeof(MergedInSolutionComponent),
                         typeof(SolutionComponent), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnValueChanged))
                         {
                             BindsTwoWayByDefault = false,
                         });


        private SolutionComponentViewModel _viewModel;
        public SolutionComponent()
        {
            InitializeComponent();
            this._viewModel = LayoutRoot.Resources["viewModel"] as SolutionComponentViewModel;
            _viewModel.Initialize(this);
        }


        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SolutionComponent v = d as SolutionComponent;
            if (e.Property.Name == "Component")
            {
                v.SetSolutionComponent((MergedInSolutionComponent)e.NewValue);
            }
            else if (e.Property.Name == "ShowInTree")
            {
                v.SetShowInTree((bool)e.NewValue);
            }
        }

        private void SetSolutionComponent(MergedInSolutionComponent component)
        {
            _viewModel.Component = component;
        }

        private void SetShowInTree(bool showInTree)
        {
            _viewModel.ShowInTree = showInTree;
        }
    }
}
