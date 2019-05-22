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

    public class MergeManagerViewModel : BaseViewModel
    {


        public string UniqueName
        {
            get
            {
                return RemoveDiacritics(NewDisplayName);
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
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("UniqueName"));
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

        public void Initialize(
            Window window,
            IOrganizationService service,
            CrmConnection crmConnection,
            SolutionManager solutionManager,
            List<Solution> selectedSolutions,
            List<Solution> allSolutions,
            List<MergedInSolutionComponent> solutionComponents)
        {
            this._window = window;
            this.SolutionComponents = solutionComponents;
            this.Service = service;
            this.SourceSolutions = allSolutions;
            this.CurrentCrmConnection = crmConnection;
            this.CurrentSolutionManager = solutionManager;

            this.MergedSolution = null;

            this.Publishers = selectedSolutions
                .GroupBy(k => k.Publisher.Id)
                .Select(group => group.First())
                .Select(k => { return k.Publisher; }).ToList();
            if (this.Publishers.Count > 0)
            {
                this.SelectedPublisher = this.Publishers[0];
            }

            RegisterCommands();
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
                            if (IsVisibleExistingSolutionSection)
                            {
                                CurrentSolutionManager.CreateMergedSolution(SelectedSolution, SolutionComponents);
                                MergedSolution = SelectedSolution;
                            }
                            else
                            {
                                var description = new StringBuilder();
                                description
                                    .AppendLine(
                                        string.Format("## Solution generanted automatically by {0} at {1} ##",
                                        CurrentCrmConnection.Username,
                                        DateTime.Now.ToString()));
                                var solution = CurrentSolutionManager.CreateSolution(NewDisplayName, UniqueName, SelectedPublisher, description.ToString());
                                CurrentSolutionManager.CreateMergedSolution(solution, SolutionComponents);
                                MergedSolution = solution;
                            }
                            MessageBox.Show("The merged has been completed", "Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                            _window.Close();
                        }
                        catch (Exception ex)
                        {
                            //TODO: roll back
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return (IsVisibleExistingSolutionSection
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
    }
}
