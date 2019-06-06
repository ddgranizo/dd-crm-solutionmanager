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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SolutionManagerUI.ViewModels
{



    public class CheckWorkSolutionManagerViewModel : BaseViewModel
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
                RaiseCanExecuteChanged();
            }
        }


        private CrmConnection _destinationCrmConnection = null;
        public CrmConnection DestinationCrmConnection
        {
            get
            {
                return _destinationCrmConnection;
            }
            set
            {
                _destinationCrmConnection = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("DestinationCrmConnection"));
                DestinationService = null;
                RaiseCanExecuteChanged();
                //if (value != null)
                //{
                //    SetDestinationOrganizationService(value);
                //}
            }
        }



        private IOrganizationService _destinationService = null;
        public IOrganizationService DestinationService
        {
            get
            {
                return _destinationService;
            }
            set
            {
                _destinationService = value;
                DestinationSolutionManager = null;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("DestinationService"));
                if (value != null)
                {
                    DestinationSolutionManager = new SolutionManager(value);
                }
            }
        }





        private readonly ObservableCollection<CrmConnection> _connectionsCollection = new ObservableCollection<CrmConnection>();
        public ObservableCollection<CrmConnection> ConnectionsCollection
        {
            get
            {
                return _connectionsCollection;
            }
        }

        private List<CrmConnection> _connections = null;
        public List<CrmConnection> Connections
        {
            get
            {
                return _connections;
            }
            set
            {
                _connections = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Connections"));
                UpdateListToCollection(value, ConnectionsCollection);
            }
        }

        private SolutionManager _destinationSolutionManager = null;
        public SolutionManager DestinationSolutionManager
        {
            get
            {
                return _destinationSolutionManager;
            }
            set
            {
                _destinationSolutionManager = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("DestinationSolutionManager"));
                RaiseCanExecuteChanged();
            }
        }


        public SolutionManager CurrentSolutionManager { get; set; }



        private WorkSolution _workSolution = null;
        public WorkSolution WorkSolution
        {
            get
            {
                return _workSolution;
            }
            set
            {
                _workSolution = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("WorkSolution"));
                RaiseCanExecuteChanged();
            }
        }


        public void Initialize(
            Window window,
            IOrganizationService service,
            CrmConnection crmConnection,
            SolutionManager solutionManager,
            List<Setting> settings,
            List<CrmConnection> connections,
            WorkSolution solution)
        {
            this._window = window;
            this.CurrentSolutionManager = solutionManager;
            this.Connections = connections;
            this.WorkSolution = solution;
            RegisterCommands();

        }




        private Guid _checkDependenciesTaskId = Guid.NewGuid();
        private void CheckDependencies()
        {
            SetDialog("Checking dependencies...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;
                //var clonedSolutionId = Guid.Empty;
                try
                {
                    SetDialog("Connecting to destination environment...");
                    var stringConnection = DestinationCrmConnection.GetStringConnetion(); var solution = CurrentSolutionManager.GetSolution(WorkSolution.SolutionId);
                    DestinationService = CrmDataProvider.GetService(stringConnection);
                    DestinationSolutionManager = new SolutionManager(DestinationService); var path = Path.GetTempPath();

                    SetDialog("Downloading solution...");

                    var filePath = $@"{path}{solution.UniqueName}.zip";
                    CurrentSolutionManager.ExportSolution(solution.UniqueName, filePath, true);
                    SetDialog("Importing solution in destionation environment...");
                    DestinationSolutionManager.ImportSolution(filePath, false, false, false);
                    SetDialog("Removing solution from destination environment...");
                    DestinationSolutionManager.RemoveSolution(solution.UniqueName);
                    CurrentSolutionManager.SetDependenciesOKForWorkSolution(WorkSolution.Id);

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
                        CurrentSolutionManager.SetDependenciesKOForWorkSolution(WorkSolution.Id, errorMessage);
                        MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        
                        //this.ClonedSolutionId = clonedSolutionId;
                        _window.Close();
                    }
                    UnsetDialog();
                });
            }, "Cloning solution...", _checkDependenciesTaskId);
        }


        protected override void RegisterCommands()
        {
            Commands.Add("RetrieveDependenciesCommand", RetrieveDependenciesCommand);
        }


        private ICommand _retrieveDependenciesCommand = null;
        public ICommand RetrieveDependenciesCommand
        {
            get
            {
                if (_retrieveDependenciesCommand == null)
                {
                    _retrieveDependenciesCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CheckDependencies();
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return DestinationCrmConnection != null;
                    });
                }
                return _retrieveDependenciesCommand;
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
