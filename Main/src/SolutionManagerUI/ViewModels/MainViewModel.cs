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



        private bool _isRetrievingWorkSolutions = false;
        public bool IsRetrievingWorkSolutions
        {
            get
            {
                return _isRetrievingWorkSolutions;
            }
            set
            {
                _isRetrievingWorkSolutions = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsRetrievingWorkSolutions"));
                RaiseCanExecuteChanged();
            }
        }


        private readonly List<WorkSolution> _selectedWorkSolutions = new List<WorkSolution>();
        public List<WorkSolution> SelectedWorkSolutions
        {
            get { return _selectedWorkSolutions; }
        }
        public string SelectedWorkSolutionsString
        {
            get
            {
                return string.Join(", ", SelectedWorkSolutions.Select(k => { return k.Name; }));
            }
        }


        private WorkSolution _selectedWorkSolution = null;
        public WorkSolution SelectedWorkSolution
        {
            get
            {
                return _selectedWorkSolution;
            }
            set
            {
                _selectedWorkSolution = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedWorkSolution"));
            }
        }

        private readonly ObservableCollection<WorkSolution> _filteredWorkSolutionsCollection = new ObservableCollection<WorkSolution>();
        public ObservableCollection<WorkSolution> FilteredWorkSolutionsCollection
        {
            get
            {
                return _filteredWorkSolutionsCollection;
            }
        }


        private List<WorkSolution> _workSolutions = null;
        public List<WorkSolution> WorkSolutions
        {
            get
            {
                return _workSolutions;
            }
            set
            {
                _workSolutions = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("WorkSolutions"));
                if (AgregatedSolutions != null)
                {
                    UpdateListToCollection(value, FilteredWorkSolutionsCollection);
                }
            }
        }



        private readonly List<AggregatedSolution> _selectedAggregatedSolutions = new List<AggregatedSolution>();
        public List<AggregatedSolution> SelectedAggregatedSolutions
        {
            get { return _selectedAggregatedSolutions; }
        }
        public string SelectedAggregatedSolutionsString
        {
            get
            {
                return string.Join(", ", SelectedAggregatedSolutions.Select(k => { return k.Name; }));
            }
        }


        private AggregatedSolution _selectedAggregatedSolution = null;
        public AggregatedSolution SelectedAggregatedSolution
        {
            get
            {
                return _selectedAggregatedSolution;
            }
            set
            {
                _selectedAggregatedSolution = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedAggregatedSolution"));
            }
        }

        private readonly ObservableCollection<AggregatedSolution> _filteredAgregatedSolutionsCollection = new ObservableCollection<AggregatedSolution>();
        public ObservableCollection<AggregatedSolution> FilteredAgregatedSolutionsCollection
        {
            get
            {
                return _filteredAgregatedSolutionsCollection;
            }
        }


        private List<AggregatedSolution> _agregatedSolutions = null;
        public List<AggregatedSolution> AgregatedSolutions
        {
            get
            {
                return _agregatedSolutions;
            }
            set
            {
                _agregatedSolutions = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("AgregatedSolutions"));
                if (AgregatedSolutions != null)
                {
                    UpdateListToCollection(value, FilteredAgregatedSolutionsCollection);
                }
            }
        }



        private bool _showInTree = false;
        public bool ShowInTree
        {
            get
            {
                return _showInTree;
            }
            set
            {
                _showInTree = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ShowInTree"));
                FilterAndSetSolutionComponentsCollection();
            }
        }

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

                if (ShowMerged)
                {
                    filteredSolutionComponents =
                        filteredSolutionComponents
                            .Where(k => k.IsIn)
                                .ToList();
                }

                if (ShowInTree)
                {
                    filteredSolutionComponents =
                        filteredSolutionComponents
                            .Where(k => !k.IsChild)
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


        public ICommand OnLoadCommand { get; set; }


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
                if (CurrentSolutionManager != null && OnLoadCommand != null)
                {
                    ICommand reloadSolutionsCommand = OnLoadCommand;
                    if (OnLoadCommand.CanExecute(null))
                    {
                        OnLoadCommand.Execute(null);
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
            OnLoadCommand = ReloadAggregatedSolutionsCommand;
        }


        public void RaisePropertyChanged()
        {
            RaiseCanExecuteChanged();
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedSolutionsString"));
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedAggregatedSolutionsString"));

        }


        private Guid _retrieveWorkSolutionTaskId = Guid.NewGuid();
        public void ReloadWorkSolutions()
        {
            IsRetrievingWorkSolutions = true;
            SolutionComponents = null;
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var workSolutions = new List<WorkSolution>();
                var isError = false;
                var errorMessage = string.Empty;
                try
                {
                    workSolutions = CurrentSolutionManager.GetWorkSolutions(SelectedAggregatedSolutions);
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
                    }else
                    {
                        this.WorkSolutions = workSolutions;
                    }
                    IsRetrievingWorkSolutions = false;
                });
            }, "Retrieving work solutions...", _retrieveWorkSolutionTaskId);

            
            
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


            Commands.Add("ReloadAggregatedSolutionsCommand", ReloadAggregatedSolutionsCommand);
            Commands.Add("OpenAggregatedSolutionInBrowserCommand", OpenAggregatedSolutionInBrowserCommand);
        }


        private ICommand _reloadWorkSolutionsCommand = null;
        public ICommand ReloadWorkSolutionsCommand
        {
            get
            {
                if (_reloadWorkSolutionsCommand == null)
                {
                    _reloadWorkSolutionsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            ReloadWorkSolutions();
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return SelectedAggregatedSolutions.Count > 0;
                    });
                }
                return _reloadWorkSolutionsCommand;
            }
        }


        private ICommand _openAggregatedSolutionInBrowserCommand = null;
        public ICommand OpenAggregatedSolutionInBrowserCommand
        {
            get
            {
                if (_openAggregatedSolutionInBrowserCommand == null)
                {
                    _openAggregatedSolutionInBrowserCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var url = GetAggregatedUrl(SelectedAggregatedSolution);
                            Process.Start("chrome.exe", url);
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return SelectedAggregatedSolution != null;
                    });
                }
                return _openAggregatedSolutionInBrowserCommand;
            }
        }

        private ICommand _reloadAggregatedSolutionsCommand = null;
        public ICommand ReloadAggregatedSolutionsCommand
        {
            get
            {
                if (_reloadAggregatedSolutionsCommand == null)
                {
                    _reloadAggregatedSolutionsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            this.AgregatedSolutions = CurrentSolutionManager.GetAggregatedSolutions();
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
                return _reloadAggregatedSolutionsCommand;
            }
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


        //main.aspx?etc=10555&extraqs=&histKey=67155678&id=%7bB25EC1CB-CF7B-E911-A97C-000D3A23443B%7d&newWindow=true&pagetype=entityrecord#82260615

        private string GetAggregatedUrl(AggregatedSolution solution)
        {
            var composedUrl = CurrentCrmConnection.Endpoint;
            if (composedUrl.Last() != '/')
            {
                composedUrl = $"{composedUrl}/";
            }
            return $"{composedUrl}main.aspx?etc=10555&pagetype=entityrecord&extraqs=id%3d{solution.Id}";
        }
    }
}
