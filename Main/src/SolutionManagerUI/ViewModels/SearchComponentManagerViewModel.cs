using DD.Crm.SolutionManager;
using DD.Crm.SolutionManager.Models;
using Microsoft.Xrm.Sdk;
using SolutionManagerUI.Commands;
using SolutionManagerUI.Models;
using SolutionManagerUI.Providers;
using SolutionManagerUI.Utilities;
using SolutionManagerUI.Utilities.Threads;
using SolutionManagerUI.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using static DD.Crm.SolutionManager.Models.SolutionComponentBase;

namespace SolutionManagerUI.ViewModels
{

    public class SearchComponentManagerViewModel : BaseViewModel
    {

        private Window _window;



        private string _messageDialog = null;
        public string MessageDialog
        {
            get
            {
                return _messageDialog;
            }
            set
            {
                _messageDialog = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("MessageDialog"));
            }
        }

        private bool _isDialogOpen = false;
        public bool IsDialogOpen
        {
            get
            {
                return _isDialogOpen;
            }
            set
            {
                _isDialogOpen = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsDialogOpen"));
            }
        }



        private ComponentTypeData _selectedType = null;
        public ComponentTypeData SelectedType
        {
            get
            {
                return _selectedType;
            }
            set
            {
                _selectedType = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedType"));
            }
        }

        private string _value = null;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Value"));
            }
        }


        private List<ComponentTypeData> _types = null;
        public List<ComponentTypeData> Types
        {
            get
            {
                return _types;
            }
            set
            {
                _types = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Types"));
                UpdateListToCollection(value, TypesCollection);
            }
        }


        private readonly ObservableCollection<ComponentTypeData> _typesCollection = new ObservableCollection<ComponentTypeData>();
        public ObservableCollection<ComponentTypeData> TypesCollection
        {
            get
            {
                return _typesCollection;
            }
        }



        public Guid ClonedSolutionId { get; set; }

        public SolutionManager CurrentSolutionManager { get; set; }
        public List<SolutionComponentBase> SearchedComponents { get; set; }
        public void Initialize(
            Window window,
            IOrganizationService service,
            CrmConnection crmConnection,
            SolutionManager solutionManager,
            List<Setting> settings)
        {
            this._window = window;
            this.CurrentSolutionManager = solutionManager;
            SearchedComponents = new List<SolutionComponentBase>();
            RegisterCommands();

            this.Types =
                Enum
                .GetValues(typeof(SolutionComponentType))
                .Cast<SolutionComponentType>()
                .Select(k => new ComponentTypeData() { Type = k })
                .ToList();

        }

        private Guid _searchTaskId = Guid.NewGuid();
        private void SearchComponent()
        {
            SetDialog("Searching...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;
                var components = new List<SolutionComponentBase>();
                try
                {
                    components = CurrentSolutionManager.SearchComponent(SelectedType.Type, Value);
                }
                catch (Exception ex)
                {
                    isError = true;
                    errorMessage = ex.Message;
                }
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    if (isError)
                    {
                        MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        SearchedComponents = components;
                        _window.Close();
                    }
                    UnsetDialog();
                });
            }, string.Empty, _searchTaskId);
        }


        protected override void RegisterCommands()
        {
            Commands.Add("SearchCommand", SearchCommand);
        }


        private ICommand _searchCommand = null;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            SearchComponent();
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return true;
                    });
                }
                return _searchCommand;
            }
        }


        private void SetDialog(string message)
        {
            IsDialogOpen = true;
            this.MessageDialog = message;
        }

        private void UpdateDialogMessage(string message, int unsetInMs = 0)
        {
            this.MessageDialog = message;
            if (unsetInMs > 0)
            {
                var timer = new System.Timers.Timer(unsetInMs);
                timer.Elapsed += (sender, e) => { UnsetDialog(); timer.Stop(); };
                timer.Start();
            }
        }

        private void UnsetDialog()
        {
            IsDialogOpen = false;
        }

    }


}
