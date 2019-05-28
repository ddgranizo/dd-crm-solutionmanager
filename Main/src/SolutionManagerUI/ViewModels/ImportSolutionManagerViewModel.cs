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

    public class ImportSolutionManagerViewModel : BaseViewModel
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


        private bool _overwriteUnmanagedCustomizations = true;
        public bool OverwriteUnmanagedCustomizations
        {
            get
            {
                return _overwriteUnmanagedCustomizations;
            }
            set
            {
                _overwriteUnmanagedCustomizations = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("OverwriteUnmanagedCustomizations"));
            }
        }


        private bool _migrateAsHold = false;
        public bool MigrateAsHold
        {
            get
            {
                return _migrateAsHold;
            }
            set
            {
                _migrateAsHold = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("MigrateAsHold"));
            }
        }


        private bool _publishWorkflows = true;
        public bool PublishWorkflows
        {
            get
            {
                return _publishWorkflows;
            }
            set
            {
                _publishWorkflows = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("PublishWorkflows"));
            }
        }


        private bool _importAsync = true;
        public bool ImportAsync
        {
            get
            {
                return _importAsync;
            }
            set
            {
                _importAsync = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ImportAsync"));
            }
        }

        public Guid ClonedSolutionId { get; set; }

        public SolutionManager CurrentSolutionManager { get; set; }
        public string Path { get; set; }

        public void Initialize(
            Window window,
            IOrganizationService service,
            CrmConnection crmConnection,
            SolutionManager solutionManager,
            List<Setting> settings,
            string path)
        {
            this._window = window;
            this.CurrentSolutionManager = solutionManager;

            RegisterCommands();

            this.Path = path;
            
        }

        private Guid _importTaskId = Guid.NewGuid();
        private void ImportSolution()
        {
            SetDialog("Importing async...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;
                var clonedSolutionId = Guid.Empty;
                try
                {
                    if (ImportAsync)
                    {
                        CurrentSolutionManager
                            .ImportSolutionAsnyc(Path, OverwriteUnmanagedCustomizations, MigrateAsHold, PublishWorkflows);
                    }
                    else
                    {
                        CurrentSolutionManager
                            .ImportSolution(Path, OverwriteUnmanagedCustomizations, MigrateAsHold, PublishWorkflows);
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
            }, string.Empty, _importTaskId);
        }


        protected override void RegisterCommands()
        {
            Commands.Add("ImportSolutionCommand", ImportSolutionCommand);
        }


        private ICommand _importSolutionCommand = null;
        public ICommand ImportSolutionCommand
        {
            get
            {
                if (_importSolutionCommand == null)
                {
                    _importSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            ImportSolution();
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
                return _importSolutionCommand;
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
