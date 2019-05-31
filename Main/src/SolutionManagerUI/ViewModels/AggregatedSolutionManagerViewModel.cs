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

namespace SolutionManagerUI.ViewModels
{


    

    public class AggregatedSolutionManagerViewModel : BaseViewModel
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


        private string _name = null;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Name"));
            }
        }

        private AggregatedSolutionTypeData _currentType = null;
        public AggregatedSolutionTypeData CurrentType
        {
            get
            {
                return _currentType;
            }
            set
            {
                _currentType = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentType"));
            }
        }


        private List<AggregatedSolutionTypeData> _types = null;
        public List<AggregatedSolutionTypeData> Types
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



        private readonly ObservableCollection<AggregatedSolutionTypeData> _typesCollection = new ObservableCollection<AggregatedSolutionTypeData>();
        public ObservableCollection<AggregatedSolutionTypeData> TypesCollection
        {
            get
            {
                return _typesCollection;
            }
        }



        public SolutionManager CurrentSolutionManager { get; set; }
        public string Path { get; set; }

        public void Initialize(
            Window window,
            IOrganizationService service,
            CrmConnection crmConnection,
            SolutionManager solutionManager,
            List<Setting> settings)
        {
            this._window = window;
            this.CurrentSolutionManager = solutionManager;

            RegisterCommands();

            this.Types = Enum
                .GetValues(typeof(AggregatedSolution.AggregatedSolutionType))
                .Cast<AggregatedSolution.AggregatedSolutionType>()
                .Select(k => new AggregatedSolutionTypeData() { Type = k })
                .ToList();
        }

        private Guid _createTaskId = Guid.NewGuid();
        private void CreateSolution()
        {
            SetDialog("Creating...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;
                try
                {
                    CurrentSolutionManager.CreateAggregatedSolution(this.Name, this.CurrentType.Type);
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
                        _window.Close();
                    }
                    UnsetDialog();
                });
            }, string.Empty, _createTaskId);
        }


        protected override void RegisterCommands()
        {
            Commands.Add("CreateAggregatedCommand", CreateAggregatedCommand);
        }


        private ICommand _createAggregatedCommand = null;
        public ICommand CreateAggregatedCommand
        {
            get
            {
                if (_createAggregatedCommand == null)
                {
                    _createAggregatedCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CreateSolution();
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
                return _createAggregatedCommand;
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
