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



    public class CloneSolutionManagerViewModel : BaseViewModel
    {

        private Window _window;


        private Solution _currentSolution = null;
        public Solution CurrentSolution
        {
            get
            {
                return _currentSolution;
            }
            set
            {
                _currentSolution = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentSolution"));
            }
        }



        private string _newName = null;
        public string NewName
        {
            get
            {
                return _newName;
            }
            set
            {
                _newName = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NewName"));
                this.NewUniqueName = StringFormatter.FormatString(value);
                RaiseCanExecuteChanged();
            }
        }


        private string _newUniqueName = null;
        public string NewUniqueName
        {
            get
            {
                return _newUniqueName;
            }
            set
            {
                _newUniqueName = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NewUniqueName"));
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
                RaiseCanExecuteChanged();
            }
        }

        public Guid ClonedSolutionId { get; set; }

        public SolutionManager CurrentSolutionManager { get; set; }

        public void Initialize(
            Window window,
            IOrganizationService service,
            CrmConnection crmConnection,
            SolutionManager solutionManager,
            List<Setting> settings,
            Solution solution)
        {
            this._window = window;
            this.CurrentSolutionManager = solutionManager;
            this.CurrentSolution = solution;

            RegisterCommands();

            this.NewName = string.Format("{0}_Copy", solution.DisplayName);
        }

        private Guid _cloneTaskId = Guid.NewGuid();
        private void CloneSolution()
        {
            SetDialog("Cloning...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;
                var clonedSolutionId = Guid.Empty;
                try
                {
                    //TODO: add version and description
                    clonedSolutionId = CurrentSolutionManager.CloneSolution(CurrentSolution.Id, NewName, NewUniqueName);
                   
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
                        this.ClonedSolutionId = clonedSolutionId;
                        _window.Close();
                    }
                    UnsetDialog();
                });
            }, "Cloning solution...", _cloneTaskId);
        }

    
        protected override void RegisterCommands()
        {
            Commands.Add("CloneSolutionCommand", CloneSolutionCommand);
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
                            CloneSolution();
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return !string.IsNullOrEmpty( NewName) && !string.IsNullOrEmpty(NewUniqueName);
                    });
                }
                return _cloneSolutionCommand;
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
