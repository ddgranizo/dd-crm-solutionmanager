using DD.Crm.SolutionManager;
using DD.Crm.SolutionManager.Models;
using DD.Crm.SolutionManager.Models.Data;
using Microsoft.Xrm.Sdk;
using SolutionManagerUI.Commands;
using SolutionManagerUI.Models;
using SolutionManagerUI.Providers;
using SolutionManagerUI.Services;
using SolutionManagerUI.Utilities;
using SolutionManagerUI.Utilities.Threads;
using SolutionManagerUI.ViewModels.Base;
using SolutionManagerUI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SolutionManagerUI.ViewModels
{

    public delegate void OnRequetedSelectionListHandler(object sender, EventArgs e);





    public class MainViewModel : BaseViewModel
    {
        public const string DefaultSolutionUniqueName = "Default";
        public const string ActiveSolutionUniqueName = "Active";
        public const string DefaultExportPathSettingKey = "SOLUTION_OUTPUT_DIRECTORY";
        

        public event OnRequetedSelectionListHandler OnRequetedSelectAllWorkSolutions;
        public event OnRequetedSelectionListHandler OnRequetedUnselectAllWorkSolutions;


        private bool _isAggregatedWorkSolutionMode = false;
        public bool IsAggregatedWorkSolutionMode
        {
            get
            {
                return _isAggregatedWorkSolutionMode;
            }
            set
            {
                _isAggregatedWorkSolutionMode = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsAggregatedWorkSolutionMode"));
                RaiseCanExecuteChanged();
            }
        }

        private bool _isSolutionMode = false;
        public bool IsSolutionMode
        {
            get
            {
                return _isSolutionMode;
            }
            set
            {
                _isSolutionMode = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsSolutionMode"));
                RaiseCanExecuteChanged();
            }
        }


        public bool IsLoadingSolutionsPanel { get { return IsRetrievingSolutions || IsRetrievingWorkSolutions; } }
        private bool _isRetrievingSolutions = false;
        public bool IsRetrievingSolutions
        {
            get
            {
                return _isRetrievingSolutions;
            }
            set
            {
                _isRetrievingSolutions = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsRetrievingSolutions"));
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsLoadingSolutionsPanel"));
                RaiseCanExecuteChanged();
            }
        }


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
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsLoadingSolutionsPanel"));
                RaiseCanExecuteChanged();
            }
        }



        private bool _isRetrievingSolutionComponents = false;
        public bool IsRetrievingSolutionComponents
        {
            get
            {
                return _isRetrievingSolutionComponents;
            }
            set
            {
                _isRetrievingSolutionComponents = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsRetrievingSolutionComponents"));
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
                RaiseCanExecuteChanged();
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

                UpdateListToCollection(value, FilteredWorkSolutionsCollection);


                RaiseCanExecuteChanged();
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
                if (IsAggregatedWorkSolutionMode)
                {
                    return string.Join(", ", SelectedAggregatedSolutions.Select(k => { return k.Name; }));
                }
                return "Solutions";

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
                RaiseCanExecuteChanged();
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

                UpdateListToCollection(value, FilteredAgregatedSolutionsCollection);

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

                FilterAndSetSolutionComponentsCollection();

            }
        }

        private void FilterAndSetSolutionComponentsCollection()
        {
            FilteredSolutionComponentsCollection.Clear();

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


        private void SetSolutionSubset(List<Solution> solutions)
        {
            SolutionFilter = null;
            UpdateListToCollection(solutions, FilteredSolutionsCollection);
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

                FilterAndSetSolutionCollection();

            }
        }

        private void FilterAndSetSolutionCollection()
        {
            FilteredSolutionsCollection.Clear();
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
                    SetCrmTemplate(CurrentCrmConnection);
                    ICommand c = OnLoadCommand;
                    if (c.CanExecute(null))
                    {
                        c.Execute(null);
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
                    CurrentSolutionManager.ExpandDefinition = GetDefaultExpandMode();
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
                UpdateListToCollection(value, ConnectionsCollection);
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
                SetEmptyTemplate();
                EmptyContext(true);
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentCrmConnection"));
                Service = null;
                if (value != null)
                {
                    
                    SetNewOrganizationService();
                }
                RaiseCanExecuteChanged();
            }
        }


        private const string ExpandModeSettingKeyName = "EXPAND_MODE";
        private bool GetDefaultExpandMode()
        {
            var settingValue = SettingsManager.GetSetting<string>(this.Settings, ExpandModeSettingKeyName, "true");
            if (!bool.TryParse(settingValue, out bool value))
            {
                return true;
            }
            return value;
        }

        public BlobStorageService BlobService { get; set; }

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
                    UpdateDialogMessage(message, 200);
                    this.Service = service;
                });
            }, "Validating connection...", _getServiceTaskId);
        }


        private Guid _retrieveSolutionsTaskId = Guid.NewGuid();
        private void ReloadSolutions()
        {
            ReloadSolutions(Guid.Empty);
        }

        private void ReloadSolutions(Guid selectSolutionId)
        {
            IsRetrievingSolutions = true;
            SolutionComponents = new List<MergedInSolutionComponent>();
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var solutions = new List<Solution>();
                var isError = false;
                var errorMessage = string.Empty;
                try
                {
                    solutions = CurrentSolutionManager.GetSolutions();
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
                        this.Solutions = solutions;
                        if (selectSolutionId != Guid.Empty)
                        {
                            SelectSolutionAsync(selectSolutionId);
                        }
                    }
                    IsRetrievingSolutions = false;
                });
            }, "Retrieving solutions...", _retrieveSolutionsTaskId);
        }


        private Guid _selectSolutionAsyncTaskId = Guid.NewGuid();
        private void SelectSolutionAsync(Guid selectSolutionId)
        {
            ThreadManager.Instance.ScheduleTask(() =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    SelectedSolution = Solutions.FirstOrDefault(k => k.Id == selectSolutionId);
                });
            }, "Retrieving solutions...", _selectSolutionAsyncTaskId, 500);

        }

        private Guid _retrieveSolutionComponentsTaskId = Guid.NewGuid();
        private void ReloadSolutionComponents()
        {
            IsRetrievingSolutionComponents = true;
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var solutionComponents = new List<MergedInSolutionComponent>();
                var isError = false;
                var errorMessage = string.Empty;
                try
                {
                    if (IsAggregatedWorkSolutionMode)
                    {
                        solutionComponents =
                            CurrentSolutionManager
                            .GetMergedSolutionComponents(SelectedWorkSolutions, true);
                    }
                    else
                    {
                        solutionComponents =
                            CurrentSolutionManager
                            .GetMergedSolutionComponents(SelectedSolutions, true);
                    }

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
                        this.SolutionComponents =
                            solutionComponents
                                .OrderBy(k => k.GetOrderWeight())
                                .ToList();
                    }
                    IsRetrievingSolutionComponents = false;
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
            SetEmptyTemplate();

            EmptyContext(true);

            RegisterCommands();
            IsSolutionMode = true;
            OnLoadCommand = InitialReloadCommand;
            BlobService = new BlobStorageService(this.Settings);
        }

        private void EmptyContext(bool restartSolutionManager)
        {
            SelectedWorkSolution = null;
            WorkSolutions = null;
            SelectedAggregatedSolution = null;
            AgregatedSolutions = null;
            SelectedSolutionComponent = null;
            SolutionComponents = null;
            SelectedSolution = null;
            Solutions = null;
            SolutionFilter = null;
            if (restartSolutionManager)
            {
                CurrentSolutionManager = null;
            }
        }

        private static void SetCrmTemplate(CrmConnection connection)
        {
            var theme = connection.ThemeColor;
            if (!string.IsNullOrEmpty(theme))
            {
                try
                {
                    System.Windows.Application.Current.Resources.MergedDictionaries
                    .Add(new ResourceDictionary()
                    { Source = new Uri($"pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.{theme}.xaml") });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Theme '{theme}' does not exists. Use theme names as 'Red', 'LightBlue' etc");
                }
                
            }
        }

        private static void SetEmptyTemplate()
        {
            System.Windows.Application.Current.Resources.MergedDictionaries.Clear();
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml") });
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml") });
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
                    }
                    else
                    {
                        this.WorkSolutions = workSolutions;
                        ICommand c = MarkUnmarkAllWorkSolutionsCommand;
                        if (c.CanExecute(null))
                        {
                            c.Execute(null);
                        }
                    }
                    IsRetrievingWorkSolutions = false;
                });
            }, "Retrieving work solutions...", _retrieveWorkSolutionTaskId);



        }




        private Guid _findEmptySolutionsTaskId = Guid.NewGuid();
        public void FindEmptySolutions()
        {
            SetDialog("Finding empty solutions...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var emptySolutions = new List<Solution>();
                var isError = false;
                var errorMessage = string.Empty;
                try
                {
                    emptySolutions = CurrentSolutionManager.FindEmptySolutions();
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
                        SetSolutionSubset(emptySolutions);
                    }
                    UnsetDialog();
                });
            }, "Finding empty solutions...", _retrieveWorkSolutionTaskId);
        }


        private Guid _exportingSolutionTaskId = Guid.NewGuid();
        private void ExportSolution(Solution solution, bool managed)
        {
            string defaultPath = GetDefaultZipPath();
            var path = FileDialogManager.SelectPath(defaultPath);
            path = StringFormatter.GetPathWithLastSlash(path);
            var fileName = StringFormatter.GetSolutionFileName(solution.UniqueName, solution.Version, managed);
            var fullPath = string.Format("{0}{1}", path, fileName);
            SetDialog($"Exporting solution managed={managed}...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;
                try
                {
                    CurrentSolutionManager.ExportSolution(solution.UniqueName, fullPath, managed);
                    if (BlobService.IsEnabledBlobStorage())
                    {
                        SetDialog($"Uploading to BlobStorage '{solution.UniqueName}' managed={managed}...");
                        BlobService.Upload(solution.UniqueName, fullPath);
                    }
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

                    }
                    UnsetDialog();
                });
            }, string.Empty, _exportingSolutionTaskId);
        }


        private Guid _removeSolutionTaskId = Guid.NewGuid();
        private void RemoveSolution(Solution solution)
        {
            var response =
                MessageBox.Show($"Are you sure you want to remove the solution '{solution.DisplayName} ({solution.UniqueName})'? This operation cannot be undone!", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (response == MessageBoxResult.Yes)
            {
                SetDialog($"Removing solution...");
                ThreadManager.Instance.ScheduleTask(() =>
                {
                    var isError = false;
                    var errorMessage = string.Empty;
                    try
                    {
                        CurrentSolutionManager.RemoveSolution(solution.Id);
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
                            ReloadSolutions();
                        }
                        UnsetDialog();
                    });
                }, string.Empty, _removeSolutionTaskId);
            }



        }
        private Guid _cleanSolutionTaskId = Guid.NewGuid();
        private void CleanSolution(Solution solution)
        {
            var response =
                MessageBox.Show($"Are you sure you want to clean the solution '{solution.DisplayName} ({solution.UniqueName})'? This operation cannot be undone!", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (response == MessageBoxResult.Yes)
            {
                SetDialog($"Cleaning solution...");
                ThreadManager.Instance.ScheduleTask(() =>
                {
                    var isError = false;
                    var errorMessage = string.Empty;
                    try
                    {
                        CurrentSolutionManager.CleanSolution(solution.Id);
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
                            ReloadSolutions();
                        }
                        UnsetDialog();
                    });
                }, string.Empty, _cleanSolutionTaskId);
            }
        }

        private Guid _checkSolutionComponentsWhichAreOnlyInSolutionTaskId = Guid.NewGuid();
        private void CheckSolutionComponentsWichAreOnlyInSolution(Solution solution)
        {
            SetDialog($"Calculating components which are only in this solution...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;
                var componentsWichAreOnlyInThisSolution = new List<MergedInSolutionComponent>();
                try
                {
                    var solutionComponents =
                        CurrentSolutionManager
                            .GetMergedSolutionComponents(new List<Solution>() { solution }, true);

                    foreach (var component in solutionComponents)
                    {
                        var areInSolutions =
                            CurrentSolutionManager
                                .GetSolutionWhereComponentIs(component.ObjectId)
                                .Where(k => k.UniqueName != DefaultSolutionUniqueName)
                                .Where(k => k.UniqueName != ActiveSolutionUniqueName)
                                .ToList();
                        if (areInSolutions.Count == 1)
                        {
                            componentsWichAreOnlyInThisSolution.Add(component);
                        }
                    }
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
                        SolutionComponents = componentsWichAreOnlyInThisSolution;
                    }
                    UnsetDialog();
                });
            }, string.Empty, _checkSolutionComponentsWhichAreOnlyInSolutionTaskId);
        }


        private Guid _publishAllTaskId = Guid.NewGuid();
        private void PublishAll()
        {
            SetDialog($"Publishing all customizations...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;
                try
                {
                    CurrentSolutionManager.PublishAll();
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

                    }
                    UnsetDialog();
                });
            }, string.Empty, _publishAllTaskId);
        }



        private Guid _retrieveAllWorkSolutionsTaskId = Guid.NewGuid();
        public void ReloadAllWorkSolutions()
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
                    workSolutions = CurrentSolutionManager.GetAllWorkSolutions();
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
                        this.WorkSolutions = workSolutions;
                        ICommand c = MarkUnmarkAllWorkSolutionsCommand;
                        if (c.CanExecute(null))
                        {
                            c.Execute(null);
                        }
                    }
                    IsRetrievingWorkSolutions = false;
                });
            }, string.Empty, _retrieveAllWorkSolutionsTaskId);



        }


        protected override void RegisterCommands()
        {
            Commands.Add("NewCommand", NewCommand);
            Commands.Add("OpenConnectionsCommand", OpenConnectionsCommand);
            Commands.Add("OpenSettingsCommand", OpenSettingsCommand);
            Commands.Add("DisconnectCommand", DisconnectCommand);
            Commands.Add("ReloadSolutionsCommand", ReloadSolutionsCommand);
            Commands.Add("ReloadSolutionComponentsCommand", ReloadSolutionComponentsCommand);
            Commands.Add("FindReasonWhyComponentIsNotInCommand", FindReasonWhyComponentIsNotInCommand);


            Commands.Add("ReloadAggregatedSolutionsCommand", ReloadAggregatedSolutionsCommand);
            Commands.Add("OpenAggregatedSolutionInBrowserCommand", OpenAggregatedSolutionInBrowserCommand);
            Commands.Add("ReloadWorkSolutionsCommand", ReloadWorkSolutionsCommand);

            Commands.Add("OpenWorkSolutionInBrowserCommand", OpenWorkSolutionInBrowserCommand);
            Commands.Add("OpenSolutionInBrowserCommand", OpenSolutionInBrowserCommand);

            Commands.Add("MarkUnmarkAllWorkSolutionsCommand", MarkUnmarkAllWorkSolutionsCommand);


            Commands.Add("SetAggregatedWorkSolutionsModeCommand", SetAggregatedWorkSolutionsModeCommand);
            Commands.Add("SetSolutionsModeCommand", SetSolutionsModeCommand);

            Commands.Add("InitialReloadCommand", InitialReloadCommand);

            Commands.Add("DoMergeCommand", DoMergeCommand);

            Commands.Add("MergeAggregatedSolutionsCommand", MergeAggregatedSolutionsCommand);
            Commands.Add("CheckAggregatedSolutionsCommand", CheckAggregatedSolutionsCommand);

            Commands.Add("FindLayersWhereComponentIs", FindLayersWhereComponentIs);

            Commands.Add("DoMergeInSupersolutionsCommand", DoMergeInSupersolutionsCommand);

            Commands.Add("CloneSolutionCommand", CloneSolutionCommand);


            Commands.Add("FindEmptySolutionsCommand", FindEmptySolutionsCommand);
            Commands.Add("DeleteSolutionCommand", DeleteSolutionCommand);

            Commands.Add("ExportManagedSolutionCommand", ExportManagedSolutionCommand);
            Commands.Add("ExportUnmanagedSolutionCommand", ExportUnmanagedSolutionCommand);
            Commands.Add("ImportSolutionCommand", ImportSolutionFromFileCommand);
            Commands.Add("ImportSolutionFromBlobCommand", ImportSolutionFromBlobCommand);
            

            Commands.Add("CleanSolutionCommand", CleanSolutionCommand);

            Commands.Add("CheckSolutionComponentsWichAreOnlyInSolutionCommand", CheckSolutionComponentsWichAreOnlyInSolutionCommand);
            Commands.Add("PublishAllCommand", PublishAllCommand);
            Commands.Add("OpenEnvironmentInBrowserCommand", OpenEnvironmentInBrowserCommand);

            Commands.Add("SearchComponentCommand", SearchComponentCommand);

            Commands.Add("CreateAggregatedSolutionCommand", CreateAggregatedSolutionCommand);

            Commands.Add("CreateWorkSolutionCommand", CreateWorkSolutionCommand);

            Commands.Add("RemoveWorkSolutionCommand", RemoveWorkSolutionCommand);
            Commands.Add("RemoveAggregatedSolutionCommand", RemoveAggregatedSolutionCommand);

            Commands.Add("SetReadyWorkSolutionCommand", SetReadyWorkSolutionCommand);
            Commands.Add("SetNotReadyWorkSolutionCommand", SetNotReadyWorkSolutionCommand);


            Commands.Add("SetMergedWithSupersolutionFlagInAggregatedSolutionCommand", SetMergedWithSupersolutionFlagInAggregatedSolutionCommand);
            Commands.Add("UnsetMergedWithSupersolutionFlagInAggregatedSolutionCommand", UnsetMergedWithSupersolutionFlagInAggregatedSolutionCommand);
            Commands.Add("SetStatusDevelopmentAggregatedSolutionCommand", SetStatusDevelopmentAggregatedSolutionCommand);
            Commands.Add("SetStatusClosedDevelopmentAggregatedSolutionCommand", SetStatusClosedDevelopmentAggregatedSolutionCommand);
            Commands.Add("SetStatusStagingAndIntegrationAggregatedSolutionCommand", SetStatusStagingAndIntegrationAggregatedSolutionCommand);
            Commands.Add("SetStatusPreproductionAggregatedSolutionCommand", SetStatusPreproductionAggregatedSolutionCommand);
            Commands.Add("SetStatuProductionAggregatedSolutionCommand", SetStatuProductionAggregatedSolutionCommand);


            Commands.Add("CreateSolutionCommand", CreateSolutionCommand);

            Commands.Add("SetAllWorkSolutionsCommand", SetAllWorkSolutionsCommand);
        }



        private ICommand _checkDependenciesCommand = null;
        public ICommand CheckDependenciesCommand
        {
            get
            {
                if (_checkDependenciesCommand == null)
                {
                    _checkDependenciesCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CheckWorkSolutionDependenciesManager man = new CheckWorkSolutionDependenciesManager(
                                this.Service,
                                this.CurrentCrmConnection,
                                this.CurrentSolutionManager,
                                this.Settings,
                                this.Connections,
                                this.SelectedWorkSolution);
                            man.ShowDialog();
                            
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null && SelectedWorkSolution != null;
                    });
                }
                return _checkDependenciesCommand;
            }
        }


        private ICommand _setAllWorkSolutionsCommand = null;
        public ICommand SetAllWorkSolutionsCommand
        {
            get
            {
                if (_setAllWorkSolutionsCommand == null)
                {
                    _setAllWorkSolutionsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            ReloadAllWorkSolutions();
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
                return _setAllWorkSolutionsCommand;
            }
        }


        private ICommand _createSolutionCommand = null;
        public ICommand CreateSolutionCommand
        {
            get
            {
                if (_createSolutionCommand == null)
                {
                    _createSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var publishers = CurrentSolutionManager.GetPublishers();
                            CreateSolutionManager man = new CreateSolutionManager(
                                this.Service,
                                this.CurrentCrmConnection,
                                this.CurrentSolutionManager,
                                this.Settings,
                                publishers);
                            man.ShowDialog();
                            var createdSolution = man.GetViewModel().CreatedSolution;
                            if (createdSolution != null)
                            {
                                SelectSolutionAsync(createdSolution.Id);
                            }
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
                return _createSolutionCommand;
            }
        }



        private ICommand _setStatuProductionAggregatedSolutionCommand = null;
        public ICommand SetStatuProductionAggregatedSolutionCommand
        {
            get
            {
                if (_setStatuProductionAggregatedSolutionCommand == null)
                {
                    _setStatuProductionAggregatedSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CurrentSolutionManager.SetStatusAggregatedSolution(SelectedAggregatedSolution.Id, AggregatedSolution.AggregatedSolutionStatus.Production);
                            ICommand c = ReloadAggregatedSolutionsCommand;
                            if (c.CanExecute(null))
                            {
                                c.Execute(null);
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null && SelectedAggregatedSolution != null;

                    });
                }
                return _setStatuProductionAggregatedSolutionCommand;
            }
        }


        private ICommand _setStatusPreproductionAggregatedSolutionCommand = null;
        public ICommand SetStatusPreproductionAggregatedSolutionCommand
        {
            get
            {
                if (_setStatusPreproductionAggregatedSolutionCommand == null)
                {
                    _setStatusPreproductionAggregatedSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CurrentSolutionManager.SetStatusAggregatedSolution(SelectedAggregatedSolution.Id, AggregatedSolution.AggregatedSolutionStatus.Preproduction);
                            ICommand c = ReloadAggregatedSolutionsCommand;
                            if (c.CanExecute(null))
                            {
                                c.Execute(null);
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null && SelectedAggregatedSolution != null;

                    });
                }
                return _setStatusPreproductionAggregatedSolutionCommand;
            }
        }



        private ICommand _setStatusStagingAndIntegrationAggregatedSolutionCommand = null;
        public ICommand SetStatusStagingAndIntegrationAggregatedSolutionCommand
        {
            get
            {
                if (_setStatusStagingAndIntegrationAggregatedSolutionCommand == null)
                {
                    _setStatusStagingAndIntegrationAggregatedSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CurrentSolutionManager.SetStatusAggregatedSolution(SelectedAggregatedSolution.Id, AggregatedSolution.AggregatedSolutionStatus.StagingAndIntegration);
                            ICommand c = ReloadAggregatedSolutionsCommand;
                            if (c.CanExecute(null))
                            {
                                c.Execute(null);
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null && SelectedAggregatedSolution != null;

                    });
                }
                return _setStatusStagingAndIntegrationAggregatedSolutionCommand;
            }
        }


        private ICommand _setStatusClosedDevelopmentAggregatedSolutionCommand = null;
        public ICommand SetStatusClosedDevelopmentAggregatedSolutionCommand
        {
            get
            {
                if (_setStatusClosedDevelopmentAggregatedSolutionCommand == null)
                {
                    _setStatusClosedDevelopmentAggregatedSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CurrentSolutionManager.SetStatusAggregatedSolution(SelectedAggregatedSolution.Id, AggregatedSolution.AggregatedSolutionStatus.ClosedDevelopment);
                            ICommand c = ReloadAggregatedSolutionsCommand;
                            if (c.CanExecute(null))
                            {
                                c.Execute(null);
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null && SelectedAggregatedSolution != null;

                    });
                }
                return _setStatusClosedDevelopmentAggregatedSolutionCommand;
            }
        }

        private ICommand _setStatusDevelopmentAggregatedSolutionCommand = null;
        public ICommand SetStatusDevelopmentAggregatedSolutionCommand
        {
            get
            {
                if (_setStatusDevelopmentAggregatedSolutionCommand == null)
                {
                    _setStatusDevelopmentAggregatedSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CurrentSolutionManager.SetStatusAggregatedSolution(SelectedAggregatedSolution.Id, AggregatedSolution.AggregatedSolutionStatus.Development);
                            ICommand c = ReloadAggregatedSolutionsCommand;
                            if (c.CanExecute(null))
                            {
                                c.Execute(null);
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null && SelectedAggregatedSolution != null;

                    });
                }
                return _setStatusDevelopmentAggregatedSolutionCommand;
            }

        }


        private ICommand _unsetMergedWithSupersolutionFlagInAggregatedSolutionCommand = null;
        public ICommand UnsetMergedWithSupersolutionFlagInAggregatedSolutionCommand
        {
            get
            {
                if (_unsetMergedWithSupersolutionFlagInAggregatedSolutionCommand == null)
                {
                    _unsetMergedWithSupersolutionFlagInAggregatedSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CurrentSolutionManager.UnsetMergedWithSupersolutionFlagInAggregatedSolution(SelectedAggregatedSolution.Id);
                            ICommand c = ReloadAggregatedSolutionsCommand;
                            if (c.CanExecute(null))
                            {
                                c.Execute(null);
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null && SelectedAggregatedSolution != null;

                    });
                }
                return _unsetMergedWithSupersolutionFlagInAggregatedSolutionCommand;
            }

        }

        private ICommand _setMergedWithSupersolutionFlagInAggregatedSolutionCommand = null;
        public ICommand SetMergedWithSupersolutionFlagInAggregatedSolutionCommand
        {
            get
            {
                if (_setMergedWithSupersolutionFlagInAggregatedSolutionCommand == null)
                {
                    _setMergedWithSupersolutionFlagInAggregatedSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CurrentSolutionManager.SetMergedWithSupersolutionFlagInAggregatedSolution(SelectedAggregatedSolution.Id);
                            ICommand c = ReloadAggregatedSolutionsCommand;
                            if (c.CanExecute(null))
                            {
                                c.Execute(null);
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null && SelectedAggregatedSolution != null;

                    });
                }
                return _setMergedWithSupersolutionFlagInAggregatedSolutionCommand;
            }

        }

        private ICommand _setNotReadyWorkSolutionCommand = null;
        public ICommand SetNotReadyWorkSolutionCommand
        {
            get
            {
                if (_setNotReadyWorkSolutionCommand == null)
                {
                    _setNotReadyWorkSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CurrentSolutionManager.SetNotReadyWorkSolution(SelectedWorkSolution.Id);
                            ICommand c = ReloadWorkSolutionsCommand;
                            if (c.CanExecute(null))
                            {
                                c.Execute(null);
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null && SelectedWorkSolution != null;

                    });
                }
                return _setNotReadyWorkSolutionCommand;
            }

        }

        private ICommand _setReadyWorkSolutionCommand = null;
        public ICommand SetReadyWorkSolutionCommand
        {
            get
            {
                if (_setReadyWorkSolutionCommand == null)
                {
                    _setReadyWorkSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CurrentSolutionManager.SetReadyWorkSolution(SelectedWorkSolution.Id);
                            ICommand c = ReloadWorkSolutionsCommand;
                            if (c.CanExecute(null))
                            {
                                c.Execute(null);
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null && SelectedWorkSolution != null;

                    });
                }
                return _setReadyWorkSolutionCommand;
            }

        }


        private ICommand _removeAggregatedSolutionCommand = null;
        public ICommand RemoveAggregatedSolutionCommand
        {
            get
            {
                if (_removeAggregatedSolutionCommand == null)
                {
                    _removeAggregatedSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var response = MessageBox.Show($"Are you sure you want to remove the aggregated solution '{SelectedAggregatedSolution.Name}? This operation cannot be undone!", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (response == MessageBoxResult.Yes)
                            {
                                CurrentSolutionManager.RemoveAggregatedSolution(SelectedAggregatedSolution.Id);
                                ICommand c = ReloadAggregatedSolutionsCommand;
                                if (c.CanExecute(null))
                                {
                                    c.Execute(null);
                                }
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null && SelectedAggregatedSolution != null;

                    });
                }
                return _removeAggregatedSolutionCommand;
            }


        }
        private ICommand _removeWorkSolutionCommand = null;
        public ICommand RemoveWorkSolutionCommand
        {
            get
            {
                if (_removeWorkSolutionCommand == null)
                {
                    _removeWorkSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var response =   MessageBox.Show($"Are you sure you want to remove the work solution '{SelectedWorkSolution.Name}? This operation cannot be undone!", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (response == MessageBoxResult.Yes)
                            {
                                CurrentSolutionManager.RemoveWorkSolution(SelectedWorkSolution.Id);
                                ICommand c = ReloadWorkSolutionsCommand;
                                if (c.CanExecute(null))
                                {
                                    c.Execute(null);
                                }
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null && SelectedWorkSolution != null;

                    });
                }
                return _removeWorkSolutionCommand;
            }
        }


        private ICommand _createWorkSolutionCommand = null;
        public ICommand CreateWorkSolutionCommand
        {
            get
            {
                if (_createWorkSolutionCommand == null)
                {
                    _createWorkSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            WorkSolutionManager man = new WorkSolutionManager(
                                Service,
                                CurrentCrmConnection,
                                CurrentSolutionManager,
                                Settings,
                                SelectedAggregatedSolution);
                            man.ShowDialog();

                            ICommand c = ReloadWorkSolutionsCommand;
                            if (c.CanExecute(null))
                            {
                                c.Execute(null);
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null && SelectedAggregatedSolution != null;

                    });
                }
                return _createWorkSolutionCommand;
            }
        }


        private ICommand _createAggregatedSolutionCommand = null;
        public ICommand CreateAggregatedSolutionCommand
        {
            get
            {
                if (_createAggregatedSolutionCommand == null)
                {
                    _createAggregatedSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            AggregatedSolutionManager man = new AggregatedSolutionManager(
                                Service,
                                CurrentCrmConnection,
                                CurrentSolutionManager,
                                Settings);
                            man.ShowDialog();

                            ICommand c = ReloadAggregatedSolutionsCommand;
                            if (c.CanExecute(null))
                            {
                                c.Execute(null);
                            }
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
                return _createAggregatedSolutionCommand;
            }
        }

        private ICommand _searchComponentCommand = null;
        public ICommand SearchComponentCommand
        {
            get
            {
                if (_searchComponentCommand == null)
                {
                    _searchComponentCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            SearchComponentManager man = new SearchComponentManager(
                                Service,
                                CurrentCrmConnection,
                                CurrentSolutionManager,
                                Settings);
                            man.ShowDialog();
                            this.SolutionComponents =
                                man.GetViewModel()
                                .SearchedComponents
                                .Select(k => new MergedInSolutionComponent(k))
                                .ToList();
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
                return _searchComponentCommand;
            }
        }


        private ICommand _openEnvironmentInBrowserCommand = null;
        public ICommand OpenEnvironmentInBrowserCommand
        {
            get
            {
                if (_openEnvironmentInBrowserCommand == null)
                {
                    _openEnvironmentInBrowserCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var url = CurrentCrmConnection.Endpoint;
                            Process.Start("chrome.exe", url);
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
                return _openEnvironmentInBrowserCommand;
            }
        }



        private ICommand _publishAllCommand = null;
        public ICommand PublishAllCommand
        {
            get
            {
                if (_publishAllCommand == null)
                {
                    _publishAllCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            PublishAll();
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
                return _publishAllCommand;
            }
        }




        private ICommand _checkSolutionComponentsWichAreOnlyInSolutionCommand = null;
        public ICommand CheckSolutionComponentsWichAreOnlyInSolutionCommand
        {
            get
            {
                if (_checkSolutionComponentsWichAreOnlyInSolutionCommand == null)
                {
                    _checkSolutionComponentsWichAreOnlyInSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CheckSolutionComponentsWichAreOnlyInSolution(SelectedSolution);
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
                return _checkSolutionComponentsWichAreOnlyInSolutionCommand;
            }
        }


        private ICommand _cleanSolutionCommand = null;
        public ICommand CleanSolutionCommand
        {
            get
            {
                if (_cleanSolutionCommand == null)
                {
                    _cleanSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CleanSolution(SelectedSolution);
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
                return _cleanSolutionCommand;
            }
        }


        private ICommand _importSolutionFromBlobCommand = null;
        public ICommand ImportSolutionFromBlobCommand
        {
            get
            {
                if (_importSolutionFromBlobCommand == null)
                {
                    _importSolutionFromBlobCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            DownloadBlobManager man = new DownloadBlobManager(
                                this.Service,
                                this.CurrentCrmConnection,
                                this.CurrentSolutionManager,
                                this.Settings,
                                this.BlobService);
                            man.ShowDialog();
                            var path = man.GetViewModel().OutputPath;
                            if (!string.IsNullOrEmpty(path))
                            {
                                ImportSolutionManager import =
                                    new ImportSolutionManager(
                                        this.Service,
                                        this.CurrentCrmConnection,
                                        this.CurrentSolutionManager,
                                        this.Settings,
                                        path);
                                import.ShowDialog();
                            }
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
                return _importSolutionFromBlobCommand;
            }
        }

        private ICommand _importSolutionFromFileCommand = null;
        public ICommand ImportSolutionFromFileCommand
        {
            get
            {
                if (_importSolutionFromFileCommand == null)
                {
                    _importSolutionFromFileCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            string defaultPath = GetDefaultZipPath();
                            var file = FileDialogManager.SelectFile(defaultPath);
                            if (!string.IsNullOrEmpty(file))
                            {
                                ImportSolutionManager import =
                                    new ImportSolutionManager(
                                        this.Service,
                                        this.CurrentCrmConnection,
                                        this.CurrentSolutionManager,
                                        this.Settings,
                                        file);
                                import.ShowDialog();
                            }
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
                return _importSolutionFromFileCommand;
            }
        }

        private string GetDefaultZipPath()
        {
            return SettingsManager.GetSetting<string>(this.Settings, DefaultExportPathSettingKey, null);
        }

        private ICommand _exportUnmanagedSolutionCommand = null;
        public ICommand ExportUnmanagedSolutionCommand
        {
            get
            {
                if (_exportUnmanagedSolutionCommand == null)
                {
                    _exportUnmanagedSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            ExportSolution(SelectedSolution, false);
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
                return _exportUnmanagedSolutionCommand;
            }
        }


        private ICommand _exportManagedSolutionCommand = null;
        public ICommand ExportManagedSolutionCommand
        {
            get
            {
                if (_exportManagedSolutionCommand == null)
                {
                    _exportManagedSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            ExportSolution(SelectedSolution, true);
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
                return _exportManagedSolutionCommand;
            }
        }

        private ICommand _deleteSolutionCommand = null;
        public ICommand DeleteSolutionCommand
        {
            get
            {
                if (_deleteSolutionCommand == null)
                {
                    _deleteSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            RemoveSolution(SelectedSolution);
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
                return _deleteSolutionCommand;
            }
        }


        private ICommand _findEmptySolutionsCommand = null;
        public ICommand FindEmptySolutionsCommand
        {
            get
            {
                if (_findEmptySolutionsCommand == null)
                {
                    _findEmptySolutionsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            FindEmptySolutions();
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
                return _findEmptySolutionsCommand;
            }
        }



        private ICommand _cloneSolutionCommand = null;
        public ICommand CloneSolutionCommand
        {
            get
            {
                if (_cloneSolutionCommand == null)
                {
                    _cloneSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var settings = AppDataManager.LoadSettings();
                            CloneSolutionManager cloneManager = new CloneSolutionManager(
                                Service,
                                CurrentCrmConnection,
                                CurrentSolutionManager,
                                settings,
                                SelectedSolution);

                            cloneManager.ShowDialog();

                            SolutionFilter = null;
                            var clonedSolutionId = cloneManager.GetViewModel().ClonedSolutionId;
                            ReloadSolutions(clonedSolutionId);

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
                return _cloneSolutionCommand;
            }
        }

        private ICommand _doMergeInSupersolutionsCommand = null;
        public ICommand DoMergeInSupersolutionsCommand
        {
            get
            {
                if (_doMergeInSupersolutionsCommand == null)
                {
                    _doMergeInSupersolutionsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var settings = AppDataManager.LoadSettings();

                            SuperSolutionsManager super = new SuperSolutionsManager(
                                Service,
                                CurrentCrmConnection,
                                CurrentSolutionManager,
                                settings,
                                SolutionComponents
                                    .Where(k => k.IsIn)
                                    .ToList(),
                                null);
                            super.ShowDialog();
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return SolutionComponents != null && SolutionComponents.Count > 0;
                    });
                }
                return _doMergeInSupersolutionsCommand;
            }
        }


        private ICommand _findLayersWhereComponentIs = null;
        public ICommand FindLayersWhereComponentIs
        {
            get
            {
                if (_findLayersWhereComponentIs == null)
                {
                    _findLayersWhereComponentIs = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var solutions = CurrentSolutionManager.GetSolutionWhereComponentIs(SelectedSolutionComponent.ObjectId);
                            SetSolutionSubset(solutions);
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return CurrentSolutionManager != null
                                    && SelectedSolutionComponent != null;

                    });
                }
                return _findLayersWhereComponentIs;
            }
        }


        private ICommand _checkAggregatedSolutionsCommand = null;
        public ICommand CheckAggregatedSolutionsCommand
        {
            get
            {
                if (_checkAggregatedSolutionsCommand == null)
                {
                    _checkAggregatedSolutionsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            CurrentSolutionManager.CheckAggregatedSolution(SelectedAggregatedSolution);

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
                return _checkAggregatedSolutionsCommand;
            }
        }


        private ICommand _mergeAggregatedSolutionsCommand = null;
        public ICommand MergeAggregatedSolutionsCommand
        {
            get
            {
                if (_mergeAggregatedSolutionsCommand == null)
                {
                    _mergeAggregatedSolutionsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            //CurrentSolutionManager.CheckAggregatedSolution(SelectedAggregatedSolution);
                            var settings = AppDataManager.LoadSettings();
                            MergeSolutionsManager mergeManager =
                                new MergeSolutionsManager(
                                    Service,
                                    CurrentCrmConnection,
                                    CurrentSolutionManager,
                                    settings,
                                    null,
                                    null,
                                    null,
                                    SelectedAggregatedSolution);
                            mergeManager.ShowDialog();

                            if (IsSolutionMode)
                            {
                                SolutionFilter = null;
                                var solutionCreated = mergeManager.GetViewModel().MergedSolution;
                                ReloadSolutions(solutionCreated.Id);
                            }

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
                return _mergeAggregatedSolutionsCommand;
            }
        }



        private ICommand _doMergeCommand = null;
        public ICommand DoMergeCommand
        {
            get
            {
                if (_doMergeCommand == null)
                {
                    _doMergeCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var settings = AppDataManager.LoadSettings();

                            var solutionItemsForMerge =
                                SolutionComponents
                                    .Where(k => k.IsIn)
                                    .OrderBy(k => k.GetOrderWeight())
                                    .ToList();
                            MergeSolutionsManager mergeManager =
                                new MergeSolutionsManager(
                                    Service,
                                    CurrentCrmConnection,
                                    CurrentSolutionManager,
                                    settings,
                                    SelectedSolutions,
                                    Solutions,
                                    solutionItemsForMerge,
                                    null);
                            mergeManager.ShowDialog();
                            SolutionFilter = null;
                            var solutionCreated = mergeManager.GetViewModel().MergedSolution;
                            ReloadSolutions(solutionCreated.Id);
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return FilteredSolutionComponentsCollection.Count > 0;
                    });
                }
                return _doMergeCommand;
            }
        }

        private ICommand _setSolutionsModeCommand = null;
        public ICommand SetSolutionsModeCommand
        {
            get
            {
                if (_setSolutionsModeCommand == null)
                {
                    _setSolutionsModeCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            IsAggregatedWorkSolutionMode = false;
                            IsSolutionMode = true;
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
                return _setSolutionsModeCommand;
            }
        }


        private ICommand _setAggregatedWorkSolutionsModeCommand = null;
        public ICommand SetAggregatedWorkSolutionsModeCommand
        {
            get
            {
                if (_setAggregatedWorkSolutionsModeCommand == null)
                {
                    _setAggregatedWorkSolutionsModeCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            IsAggregatedWorkSolutionMode = true;
                            IsSolutionMode = false;

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
                return _setAggregatedWorkSolutionsModeCommand;
            }
        }



        private ICommand _markUnmarkAllWorkSolutionsCommand = null;
        public ICommand MarkUnmarkAllWorkSolutionsCommand
        {
            get
            {
                if (_markUnmarkAllWorkSolutionsCommand == null)
                {
                    _markUnmarkAllWorkSolutionsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            if (SelectedWorkSolutions.Count == WorkSolutions.Count)
                            {
                                OnRequetedUnselectAllWorkSolutions?.Invoke(this, new EventArgs());
                            }
                            else
                            {
                                OnRequetedSelectAllWorkSolutions?.Invoke(this, new EventArgs());
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return WorkSolutions != null && WorkSolutions.Count > 0;
                    });
                }
                return _markUnmarkAllWorkSolutionsCommand;
            }
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

        private ICommand _initialReloadCommand = null;
        public ICommand InitialReloadCommand
        {
            get
            {
                if (_initialReloadCommand == null)
                {
                    _initialReloadCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            ICommand c = null;
                            if (IsSolutionMode)
                            {
                                c = ReloadSolutionsCommand;
                            }
                            if (IsAggregatedWorkSolutionMode)
                            {
                                c = ReloadAggregatedSolutionsCommand;
                            }
                            if (c.CanExecute(null))
                            {
                                c.Execute(null);
                            }
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
                return _initialReloadCommand;
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
                            if (SelectedSolutionComponent.ReasonWhyIsNot == MergedInSolutionComponent.ReasonWhyIsNotType.OverComponent)
                            {
                                SelectedSolutionComponent = SelectedSolutionComponent.RemovedByComponent;
                            }
                            else if (SelectedSolutionComponent.ReasonWhyIsNot == MergedInSolutionComponent.ReasonWhyIsNotType.ParentIsNot)
                            {
                                var objectTypeCode = ((BaseEntity)SelectedSolutionComponent.ObjectDefinition).ObjectTypeCode;
                                MessageBox.Show($"The parent entity '{objectTypeCode}' of this item is not in the solution or is including all subcomponents", "Reason why is not in", MessageBoxButton.OK, MessageBoxImage.Information);
                            }

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

        private ICommand _openWorkSolutionInBrowserCommand = null;
        public ICommand OpenWorkSolutionInBrowserCommand
        {
            get
            {
                if (_openWorkSolutionInBrowserCommand == null)
                {
                    _openWorkSolutionInBrowserCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var url = GetWorkSolutionUrl(SelectedWorkSolution);
                            Process.Start("chrome.exe", url);
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return SelectedWorkSolution != null;
                    });
                }
                return _openWorkSolutionInBrowserCommand;
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

                        return (IsAggregatedWorkSolutionMode
                                && SelectedWorkSolutions != null
                                && SelectedWorkSolutions.Count > 0
                                ||
                                IsSolutionMode
                                && SelectedSolutions != null
                                && SelectedSolutions.Count > 0);
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
                            if (CurrentSolutionManager != null)
                            {
                                CurrentSolutionManager.ExpandDefinition = GetDefaultExpandMode();
                            }
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
                            //CurrentCrmConnection = null;
                            //UpdateListToCollection(Connections, ConnectionsCollection);
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
                            
                            EmptyContext(true);
                            CurrentCrmConnection = null;
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

        private string GetWorkSolutionUrl(WorkSolution solution)
        {
            var composedUrl = CurrentCrmConnection.Endpoint;
            if (composedUrl.Last() != '/')
            {
                composedUrl = $"{composedUrl}/";
            }
            return $"{composedUrl}main.aspx?etc=10554&pagetype=entityrecord&extraqs=id%3d{solution.Id}";
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
