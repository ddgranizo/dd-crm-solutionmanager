using DD.Crm.SolutionManager;
using DD.Crm.SolutionManager.Models;
using Microsoft.Xrm.Sdk;
using SolutionManagerUI.Commands;
using SolutionManagerUI.Models;
using SolutionManagerUI.Providers;
using SolutionManagerUI.Utilities;
using SolutionManagerUI.Utilities.Threads;
using SolutionManagerUI.ViewModels.Base;
using SolutionManagerUI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SolutionManagerUI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {


        private bool _showMerged = false;
        public bool ShowMerged
        {
            get
            {
                return _showMerged;
            }
            set
            {
                _showMerged = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ShowMerged"));
                FilterAndSetSolutionComponentsCollection();
            }
        }

        private MergedInSolutionComponent _selectedSolutionComponent = null;
        public MergedInSolutionComponent SelectedSolutionComponent
        {
            get
            {
                return _selectedSolutionComponent;
            }
            set
            {
                _selectedSolutionComponent = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedSolutionComponent"));
                RaiseCanExecuteChanged();
            }
        }

        private readonly ObservableCollection<MergedInSolutionComponent> _filteredSolutionComponentsCollection = new ObservableCollection<MergedInSolutionComponent>();
        public ObservableCollection<MergedInSolutionComponent> FilteredSolutionComponentsCollection
        {
            get
            {
                return _filteredSolutionComponentsCollection;
            }
        }


        private List<MergedInSolutionComponent> _solutionComponents = null;
        public List<MergedInSolutionComponent> SolutionComponents
        {
            get
            {
                return _solutionComponents;
            }
            set
            {
                _solutionComponents = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SolutionComponents"));
                if (value != null)
                {
                    FilterAndSetSolutionComponentsCollection();
                }
            }
        }

        private void FilterAndSetSolutionComponentsCollection()
        {
            if (SolutionComponents != null)
            {
                var filteredSolutionComponents = SolutionComponents;
                if (!string.IsNullOrEmpty(SolutionComponentFilter))
                {
                    filteredSolutionComponents =
                        filteredSolutionComponents
                        .Where(k => k.DisplayName.ToLowerInvariant().IndexOf(SolutionComponentFilter.ToLowerInvariant()) > -1)
                        .ToList();
                }

                if (ShowMerged)
                {
                    filteredSolutionComponents =
                        filteredSolutionComponents
                            .Where(k => k.IsIn)
                                .ToList();
                }

                UpdateListToCollection(filteredSolutionComponents, FilteredSolutionComponentsCollection);
            }
        }


        private string _solutionComponentFilter = null;
        public string SolutionComponentFilter
        {
            get
            {
                return _solutionComponentFilter;
            }
            set
            {
                _solutionComponentFilter = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SolutionComponentFilter"));
                FilterAndSetSolutionComponentsCollection();
            }
        }





        private Solution _selectedSolution = null;
        public Solution SelectedSolution
        {
            get
            {
                return _selectedSolution;
            }
            set
            {
                _selectedSolution = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedSolution"));
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedSolutionsString"));
                RaiseCanExecuteChanged();
            }
        }



        private readonly List<Solution> _selectedSolutions = new List<Solution>();
        public List<Solution> SelectedSolutions
        {
            get { return _selectedSolutions; }
        }


        public string SelectedSolutionsString
        {
            get
            {
                return string.Join(", ", SelectedSolutions.Select(k => { return k.DisplayName; }));
            }
        }



        private readonly ObservableCollection<Solution> _filteredSolutionsCollection = new ObservableCollection<Solution>();
        public ObservableCollection<Solution> FilteredSolutionsCollection
        {
            get
            {
                return _filteredSolutionsCollection;
            }
        }


        private List<Solution> _solutions = null;
        public List<Solution> Solutions
        {
            get
            {
                return _solutions;
            }
            set
            {
                _solutions = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Solutions"));
                if (value != null)
                {
                    FilterAndSetSolutionCollection();
                }
            }
        }

        private void FilterAndSetSolutionCollection()
        {
            if (Solutions != null)
            {
                var filteredSolutions = Solutions;
                if (!string.IsNullOrEmpty(SolutionFilter))
                {

                    filteredSolutions =
                        filteredSolutions
                        .Where(k =>
                            {
                                bool found = false;
                                foreach (var item in SolutionFilter.Split(';'))
                                {
                                    found = found ? found : k.DisplayName.ToLowerInvariant().IndexOf(item.ToLowerInvariant()) > -1;
                                }
                                return found;
                            })
                        .ToList();
                }
                UpdateListToCollection(filteredSolutions, FilteredSolutionsCollection);
            }
        }


        private string _solutionFilter = null;
        public string SolutionFilter
        {
            get
            {
                return _solutionFilter;
            }
            set
            {
                _solutionFilter = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SolutionFilter"));
                FilterAndSetSolutionCollection();
            }
        }


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


        private bool _isRetrievingService = false;
        public bool IsRetrievingService
        {
            get
            {
                return _isRetrievingService;
            }
            set
            {
                _isRetrievingService = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsRetrievingService"));
            }
        }



        private SolutionManager _currentSolutionManager = null;
        public SolutionManager CurrentSolutionManager
        {
            get
            {
                return _currentSolutionManager;
            }
            set
            {
                _currentSolutionManager = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentSolutionManager"));
                RaiseCanExecuteChanged();
                if (CurrentSolutionManager != null)
                {
                    ICommand reloadSolutionsCommand = ReloadSolutionsCommand;
                    if (reloadSolutionsCommand.CanExecute(null))
                    {
                        reloadSolutionsCommand.Execute(null);
                    }
                }
            }
        }


        private Guid _getServiceTaskId = Guid.NewGuid();


        private IOrganizationService _service = null;
        public IOrganizationService Service
        {
            get
            {
                return _service;
            }
            set
            {
                _service = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Service"));
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsServiceConnected"));

                CurrentSolutionManager = null;
                if (_service != null)
                {
                    CurrentSolutionManager = new SolutionManager(_service);
                }
                RaiseCanExecuteChanged();
            }
        }

        public bool IsServiceConnected
        {
            get
            {
                return this.Service != null;
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
                if (_connections != null)
                {
                    UpdateListToCollection(value, ConnectionsCollection);
                }
            }
        }


        private List<Setting> _settings = null;
        public List<Setting> Settings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Settings"));
            }
        }


        private CrmConnection _currentCrmConnection = null;
        public CrmConnection CurrentCrmConnection
        {
            get
            {
                return _currentCrmConnection;
            }
            set
            {
                _currentCrmConnection = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentCrmConnection"));
                Service = null;
                if (value != null)
                {
                    SetNewOrganizationService();
                }
                RaiseCanExecuteChanged();
            }
        }

        private void SetNewOrganizationService()
        {
            var stringConnection = CurrentCrmConnection.GetStringConnetion();

            IsRetrievingService = true;
            SetDialog("Connecting...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                string okMessage = "Connected";
                string koMessage = "Cannot connect to the environment with this configuration";

                IOrganizationService service = null;
                try
                {
                    service = CrmDataProvider.GetService(stringConnection);
                }
                catch (Exception)
                {
                    //Avoid raising exception
                    UpdateDialogMessage(koMessage, 2000);
                }

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    IsRetrievingService = false;
                    string message = service != null
                        ? okMessage
                        : koMessage;
                    UpdateDialogMessage(message, 2000);
                    this.Service = service;
                });
            }, "Validating connection...", _getServiceTaskId);
        }


        private Guid _retrieveSolutionsTaskId = Guid.NewGuid();
        private void ReloadSolutions()
        {
            SetDialog("Retrieving solutions...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var solutions = new List<Solution>();
                try
                {
                    solutions = CurrentSolutionManager.GetSolutions();
                }
                catch (Exception ex)
                {
                    UpdateDialogMessage(ex.Message, 2000);
                }
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    this.Solutions = solutions;
                    UnsetDialog();
                });
            }, "Retrieving solutions...", _retrieveSolutionsTaskId);
        }


        private Guid _retrieveSolutionComponentsTaskId = Guid.NewGuid();
        private void ReloadSolutionComponents()
        {
            SetDialog("Retrieving solution components...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var solutionComponents = new List<MergedInSolutionComponent>();
                try
                {
                    solutionComponents = CurrentSolutionManager.GetMergedSolutionComponents(SelectedSolutions, true);
                }
                catch (Exception ex)
                {
                    UpdateDialogMessage(ex.Message, 2000);
                }
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    this.SolutionComponents =
                        solutionComponents
                            .OrderBy(k => k.GetOrderWeight())
                            .ToList();
                    UnsetDialog();
                });
            }, "Retrieving solution components...", _retrieveSolutionComponentsTaskId);
        }

        public MainViewModel()
        {
            AppDataManager.CreateAppDataPathIfNotExists();
            Connections = AppDataManager.LoadConnections();
            Settings = AppDataManager.LoadSettings();
        }

        public void Initialize(Window window)
        {
            RegisterCommands();
        }


        public void RaisePropertyChanged()
        {
            RaiseCanExecuteChanged();
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedSolutionsString"));
        }

        protected override void RegisterCommands()
        {
            Commands.Add("NewCommand", NewCommand);
            Commands.Add("OpenConnectionsCommand", OpenConnectionsCommand);
            Commands.Add("OpenSettingsCommand", OpenSettingsCommand);
            Commands.Add("DisconnectCommand", DisconnectCommand);
            Commands.Add("ReloadSolutionsCommand", ReloadSolutionsCommand);
            Commands.Add("ReloadSolutionComponentsCommand", ReloadSolutionComponentsCommand);
            Commands.Add("OpenSolutionInBrowserCommand", OpenSolutionInBrowserCommand);
            Commands.Add("FindReasonWhyComponentIsNotInCommand", FindReasonWhyComponentIsNotInCommand);

        }




        private ICommand _findReasonWhyComponentIsNotInCommand = null;
        public ICommand FindReasonWhyComponentIsNotInCommand
        {
            get
            {
                if (_findReasonWhyComponentIsNotInCommand == null)
                {
                    _findReasonWhyComponentIsNotInCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            SelectedSolutionComponent = SelectedSolutionComponent.RemovedByComponent;
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null
                                    && SelectedSolutionComponent != null
                                    && !SelectedSolutionComponent.IsIn;

                    });
                }
                return _findReasonWhyComponentIsNotInCommand;
            }
        }


        private ICommand _openSolutionInBrowserCommand = null;
        public ICommand OpenSolutionInBrowserCommand
        {
            get
            {
                if (_openSolutionInBrowserCommand == null)
                {
                    _openSolutionInBrowserCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var url = GetSolutionUrl(SelectedSolution);
                            Process.Start("chrome.exe", url);
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return SelectedSolution != null;
                    });
                }
                return _openSolutionInBrowserCommand;
            }
        }


        private ICommand _reloadSolutionComponentsCommand = null;
        public ICommand ReloadSolutionComponentsCommand
        {
            get
            {
                if (_reloadSolutionComponentsCommand == null)
                {
                    _reloadSolutionComponentsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            ReloadSolutionComponents();
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null;
                    });
                }
                return _reloadSolutionComponentsCommand;
            }
        }

        private ICommand _reloadSolutionsCommand = null;
        public ICommand ReloadSolutionsCommand
        {
            get
            {
                if (_reloadSolutionsCommand == null)
                {
                    _reloadSolutionsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            ReloadSolutions();
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null;
                    });
                }
                return _reloadSolutionsCommand;
            }
        }


        private ICommand _openSettingsCommand = null;
        public ICommand OpenSettingsCommand
        {
            get
            {
                if (_openSettingsCommand == null)
                {
                    _openSettingsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            SettingManager manager = new SettingManager(Settings);
                            manager.ShowDialog();
                            AppDataManager.SaveSettings(Settings);
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
                return _openSettingsCommand;
            }
        }


        private ICommand _openConnectionsCommand = null;
        public ICommand OpenConnectionsCommand
        {
            get
            {
                if (_openConnectionsCommand == null)
                {
                    _openConnectionsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            ConnectionManager connection = new ConnectionManager(Connections);
                            connection.ShowDialog();
                            AppDataManager.SaveConnections(Connections);
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
                return _openConnectionsCommand;
            }
        }


        private ICommand _disconnectCommand = null;
        public ICommand DisconnectCommand
        {
            get
            {
                if (_disconnectCommand == null)
                {
                    _disconnectCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CurrentCrmConnection = null;
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return Service != null;
                    });
                }
                return _disconnectCommand;
            }
        }


        private ICommand _newCommand = null;
        public ICommand NewCommand
        {
            get
            {
                if (_newCommand == null)
                {
                    _newCommand = new RelayCommand((object param) =>
                    {
                        try
                        {


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
                return _newCommand;
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

        private string GetSolutionUrl(Solution solution)
        {
            var composedUrl = CurrentCrmConnection.Endpoint;
            if (composedUrl.Last() != '/')
            {
                composedUrl = $"{composedUrl}/";
            }
            return $"{composedUrl}tools/solution/edit.aspx?id={solution.Id}";
        }
    }
}
