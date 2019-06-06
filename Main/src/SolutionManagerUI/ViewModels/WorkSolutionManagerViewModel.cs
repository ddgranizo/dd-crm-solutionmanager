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


    public enum WorkOperationType
    {
        CreateWorkSolution,
        AddExistingWorkSolution
    }


    public class WorkSolutionManagerViewModel : BaseViewModel
    {

        private Window _window;


        private string _filterWork = null;
        public string FilterWork
        {
            get
            {
                return _filterWork;
            }
            set
            {
                _filterWork = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("FilterWork"));
                FilterWorkSolutions();
            }
        }


        public bool IsCreateingMode { get { return CurrentOperationType == WorkOperationType.CreateWorkSolution; } }
        public bool IsAddingExistingMode { get { return CurrentOperationType == WorkOperationType.AddExistingWorkSolution; } }

        private WorkOperationType _currentOperationType = 0;
        public WorkOperationType CurrentOperationType
        {
            get
            {
                return _currentOperationType;
            }
            set
            {
                _currentOperationType = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentOperationType"));
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsCreateingMode"));
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsAddingExistingMode"));
                RaiseCanExecuteChanged();
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
                RaiseCanExecuteChanged();
            }
        }


        private string _jira = null;
        public string Jira
        {
            get
            {
                return _jira;
            }
            set
            {
                _jira = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Jira"));
                RaiseCanExecuteChanged();
            }
        }



        public SolutionManager CurrentSolutionManager { get; set; }
        public string Path { get; set; }


        public WorkSolution CurrentWorkSolution { get; set; }


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
                FilterWorkSolutions();
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



        public void FilterWorkSolutions()
        {
            var filteredSolutions = WorkSolutions;
            if (!string.IsNullOrEmpty(FilterWork))
            {
                filteredSolutions = WorkSolutions
                        .Where(k => k.Name.ToLowerInvariant().IndexOf(FilterWork.ToLowerInvariant()) > -1)
                        .ToList();
            }
            UpdateListToCollection(filteredSolutions, FilteredWorkSolutionsCollection);
        }


        public AggregatedSolution AggregatedSolution { get; set; }

        public void Initialize(
            Window window,
            IOrganizationService service,
            CrmConnection crmConnection,
            SolutionManager solutionManager,
            List<Setting> settings,
            AggregatedSolution aggregated)
        {
            this._window = window;
            this.CurrentSolutionManager = solutionManager;
            this.AggregatedSolution = aggregated;
            this.WorkSolutions = this.CurrentSolutionManager.GetAllWorkSolutions();
            RegisterCommands();
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
                    if (IsCreateingMode)
                    {
                        var workSolutionId = CurrentSolutionManager.CreateWorkSolution(this.Name, this.Jira);
                        CurrentSolutionManager.AssignWorkSolutionToAggregatedSolution(AggregatedSolution.Id, workSolutionId);
                    }
                    else
                    {
                        if (CurrentWorkSolution != null)
                        {
                            CurrentSolutionManager.AssignWorkSolutionToAggregatedSolution(AggregatedSolution.Id, CurrentWorkSolution.Id);
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
                        _window.Close();
                    }
                    UnsetDialog();
                });
            }, string.Empty, _createTaskId);
        }



        private Guid _assignTaskId = Guid.NewGuid();
        private void AssignSolution()
        {
            SetDialog("Assigning...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;
                try
                {
                    CurrentSolutionManager
                        .AssignWorkSolutionToAggregatedSolution
                            (AggregatedSolution.Id, SelectedWorkSolution.Id);
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
            }, string.Empty, _assignTaskId);
        }



        protected override void RegisterCommands()
        {
            Commands.Add("CreateWorkSolutionCommand", CreateWorkSolutionCommand);
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

                            if (IsCreateingMode)
                            {
                                CreateSolution();
                            }
                            else if (IsAddingExistingMode)
                            {
                                AssignSolution();
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return IsCreateingMode && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Jira)
                                    || IsAddingExistingMode && SelectedWorkSolution != null;
                    });
                }
                return _createWorkSolutionCommand;
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
