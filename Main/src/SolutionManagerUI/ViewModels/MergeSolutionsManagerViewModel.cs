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

    public enum OperationType
    {
        CreateSolution,
        ExistingSolution
    }

    public class MergeSolutionsManagerViewModel : BaseViewModel
    {

        private const string AggregatedSolutionNameKey = "AGGREGATED_SOLUTION_NAME";
        private const string AggregatedSolutionDefaultName = "AGGR_{0}_{1}";
        private const string DefaultPublisherSettingKey = "DEFAULT_PUBLISHER_ID";
        private const string PublisherLogicalName = "publisher";

        private bool _isAggregatedMode = false;
        public bool IsAggregatedMode
        {
            get
            {
                return _isAggregatedMode;
            }
            set
            {
                _isAggregatedMode = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsAggregatedMode"));
            }
        }


        private AggregatedSolution _currentAggregatedSolution = null;
        public AggregatedSolution CurrentAggregatedSolution
        {
            get
            {
                return _currentAggregatedSolution;
            }
            set
            {
                _currentAggregatedSolution = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentAggregatedSolution"));
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
                RaiseCanExecuteChanged();
            }
        }




        private string _uniqueName = null;
        public string UniqueName
        {
            get
            {
                return _uniqueName;
            }
            set
            {
                _uniqueName = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("UniqueName"));
            }
        }



        private string _newDisplayName = null;
        public string NewDisplayName
        {
            get
            {
                return _newDisplayName;
            }
            set
            {
                _newDisplayName = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NewDisplayName"));
                UniqueName = RemoveDiacritics(value);
                RaiseCanExecuteChanged();
            }
        }


        private EntityReference _selectedPublisher = null;
        public EntityReference SelectedPublisher
        {
            get
            {
                return _selectedPublisher;
            }
            set
            {
                _selectedPublisher = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedPublisher"));
                RaiseCanExecuteChanged();
            }
        }

        private readonly ObservableCollection<EntityReference> _publishersCollection = new ObservableCollection<EntityReference>();
        public ObservableCollection<EntityReference> PublishersCollection
        {
            get
            {
                return _publishersCollection;
            }
        }


        private List<EntityReference> _publishers = null;
        public List<EntityReference> Publishers
        {
            get
            {
                return _publishers;
            }
            set
            {
                _publishers = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Publishers"));
                if (value != null)
                {
                    UpdateListToCollection(value, PublishersCollection);
                }
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


        private Solution _mergedSolution = null;
        public Solution MergedSolution
        {
            get
            {
                return _mergedSolution;
            }
            set
            {
                _mergedSolution = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("MergedSolution"));
                RaiseCanExecuteChanged();
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
                RaiseCanExecuteChanged();
            }
        }

        private readonly ObservableCollection<Solution> _filteredSourceSolutionsCollection = new ObservableCollection<Solution>();
        public ObservableCollection<Solution> FilteredSourceSolutionsCollection
        {
            get
            {
                return _filteredSourceSolutionsCollection;
            }
        }


        private List<Solution> _sourceSolutions = null;
        public List<Solution> SourceSolutions
        {
            get
            {
                return _sourceSolutions;
            }
            set
            {
                _sourceSolutions = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SourceSolutions"));
                if (_solutionComponents != null)
                {
                    UpdateListToCollection(_sourceSolutions, FilteredSourceSolutionsCollection);
                }
                RaiseCanExecuteChanged();
            }
        }


        public bool IsVisibleCreateSolutionSection { get { return CurrentOperationType == OperationType.CreateSolution; } }
        public bool IsVisibleExistingSolutionSection { get { return CurrentOperationType == OperationType.ExistingSolution; } }


        public IOrganizationService Service { get; set; }


        private OperationType _currentOperationType = OperationType.CreateSolution;
        public OperationType CurrentOperationType
        {
            get
            {
                return _currentOperationType;
            }
            set
            {
                _currentOperationType = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentOperationType"));
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsVisibleCreateSolutionSection"));
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsVisibleExistingSolutionSection"));
            }
        }


        private readonly ObservableCollection<MergedInSolutionComponent> _solutionComponentsCollection = new ObservableCollection<MergedInSolutionComponent>();
        public ObservableCollection<MergedInSolutionComponent> SolutionComponentsCollection
        {
            get
            {
                return _solutionComponentsCollection;
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
                if (_solutionComponents != null)
                {
                    UpdateListToCollection(_solutionComponents, SolutionComponentsCollection);
                }
            }
        }



        public List<WorkSolution> WorkSolutions { get; set; }

        public CrmConnection CurrentCrmConnection { get; set; }
        public SolutionManager CurrentSolutionManager { get; set; }
        private Window _window;

        private void FilterAndSetSolutionCollection()
        {
            if (SourceSolutions != null)
            {
                var filteredSolutions = SourceSolutions;
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
                UpdateListToCollection(filteredSolutions, FilteredSourceSolutionsCollection);
            }
        }


        public Guid DefaultPublisherId { get; set; }



        public void Initialize(
            Window window,
            IOrganizationService service,
            CrmConnection crmConnection,
            SolutionManager solutionManager,
            List<Setting> settings,
            List<Solution> selectedSolutions,
            List<Solution> allSolutions,
            List<MergedInSolutionComponent> solutionComponents,
            AggregatedSolution aggregatedSolution)
        {
            this._window = window;
            this.SolutionComponents = solutionComponents;
            this.Service = service;
            this.SourceSolutions = allSolutions;
            this.CurrentCrmConnection = crmConnection;
            this.CurrentSolutionManager = solutionManager;
            this.MergedSolution = null;

            if (selectedSolutions != null && selectedSolutions.Count > 0)
            {
                this.Publishers = selectedSolutions
                .GroupBy(k => k.Publisher.Id)
                .Select(group => group.First())
                .Select(k => { return k.Publisher; }).ToList();
                if (this.Publishers.Count > 0)
                {
                    this.SelectedPublisher = this.Publishers[0];
                }
            }
            IsAggregatedMode = false;
            if (aggregatedSolution != null)
            {
                IsAggregatedMode = true;
                PrepareAggregatedSolution(settings, aggregatedSolution);
            }

            RegisterCommands();
        }

        private void PrepareAggregatedSolution(List<Setting> settings, AggregatedSolution aggregatedSolution)
        {
            var aggregatedSolutionName = SettingsManager.GetSetting<string>(settings, AggregatedSolutionNameKey, AggregatedSolutionDefaultName);
            var defaultPublisherIdStr = SettingsManager.GetSetting<string>(settings, DefaultPublisherSettingKey, string.Empty);
            if (string.IsNullOrEmpty(defaultPublisherIdStr))
            {
                throw new Exception($"Cannot find {DefaultPublisherSettingKey} as a Setting");
            }
            if (!Guid.TryParse(defaultPublisherIdStr, out Guid defaultPublisherId))
            {
                throw new Exception($"Found setting {DefaultPublisherSettingKey} is not a valid Guid");
            }
            var publisherName = GetPublisherName(defaultPublisherId);

            SelectedPublisher = new EntityReference()
            {
                Id = defaultPublisherId,
                Name = publisherName,
                LogicalName = PublisherLogicalName,
            };
            ReloadSolutionComponentFromAggregatedSolution(aggregatedSolution);


            var uniqueName = RemoveDiacritics(aggregatedSolution.Name);
            this.NewDisplayName = string.Format(aggregatedSolutionName, aggregatedSolution.Type.ToString(), uniqueName);
            this.UniqueName = NewDisplayName;

        }


        private string GetPublisherName(Guid publisherId)
        {
            var publisher = Service.Retrieve(PublisherLogicalName, publisherId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
            return publisher.GetAttributeValue<string>("friendlyname");
        }

        private Guid _reloadComponentsTaskId = Guid.NewGuid();
        private void ReloadSolutionComponentFromAggregatedSolution(AggregatedSolution aggregatedSolution)
        {
            SetDialog("Retrieving components...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;

                List<MergedInSolutionComponent> mergedSolutionComponents = new List<MergedInSolutionComponent>();
                var workSolutions = new List<WorkSolution>();
                try
                {
                    workSolutions = CurrentSolutionManager.GetWorkSolutions(aggregatedSolution);
                    mergedSolutionComponents = CurrentSolutionManager.GetMergedSolutionComponents(workSolutions, true);
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
                        WorkSolutions = workSolutions;
                        SolutionComponents = mergedSolutionComponents;
                    }
                    UnsetDialog();
                });
            }, "Retrieving components...", _reloadComponentsTaskId);
        }

        protected override void RegisterCommands()
        {
            Commands.Add("DoMergeCommand", DoMergeCommand);
            Commands.Add("OpenSolutionInBrowserCommand", OpenSolutionInBrowserCommand);
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

                            Solution targetSolution = null;

                            if (IsAggregatedMode || IsVisibleCreateSolutionSection)
                            {
                                targetSolution = CreateSolution();
                            }
                            else
                            {
                                targetSolution = SelectedSolution;
                            }

                            MergeSolutionResult(targetSolution);


                        }
                        catch (Exception ex)
                        {

                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return
                            IsAggregatedMode && (
                                 !IsDialogOpen
                                 && SolutionComponents.Count > 0
                            )
                            || (IsVisibleExistingSolutionSection
                            && SelectedSolution != null)
                            ||
                            (IsVisibleCreateSolutionSection
                            && !string.IsNullOrEmpty(NewDisplayName)
                            && SelectedPublisher != null);
                    });
                }
                return _doMergeCommand;
            }
        }

        private Solution CreateSolution()
        {
            Solution targetSolution;
            var description = new StringBuilder();
            description
                .AppendLine(
                    string.Format("## Solution generanted automatically by {0} at {1} ##",
                    CurrentCrmConnection.Username,
                    DateTime.Now.ToString()));
            if (WorkSolutions!=null)
            {
                foreach (var item in WorkSolutions)
                {
                    description.AppendLine($"-{item.Jira}");
                }
            }
            var solution = CurrentSolutionManager.CreateSolution(NewDisplayName, UniqueName, SelectedPublisher, description.ToString());
            targetSolution = solution;
            return targetSolution;
        }

        private Guid _mergeTaskId = Guid.NewGuid();
        private void MergeSolutionResult(Solution targetSolution)
        {
            SetDialog("Merging...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;

                try
                {
                    var currentComponents = CurrentSolutionManager.GetSolutionComponents(targetSolution.Id, false);
                    if (currentComponents.Count > 0)
                    {
                        throw new Exception("Target solution already has components");
                    }
                    CurrentSolutionManager.CreateMergedSolution(targetSolution, SolutionComponents);
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
                        MergedSolution = targetSolution;
                        _window.Close();
                    }
                    UnsetDialog();
                });
            }, "Merging solution components...", _mergeTaskId);


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


        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

                if (unicodeCategory == UnicodeCategory.DecimalDigitNumber
                    || unicodeCategory == UnicodeCategory.LowercaseLetter
                    || unicodeCategory == UnicodeCategory.UppercaseLetter
                    || c == '_')
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
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
