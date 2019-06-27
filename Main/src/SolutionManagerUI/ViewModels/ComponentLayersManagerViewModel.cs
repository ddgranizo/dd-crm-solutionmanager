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


    


    public class ComponentLayersManagerViewModel : BaseViewModel
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


        public MergedInSolutionComponent CurrentSolutionComponent { get; set; }


        public void Initialize(
            Window window,
            IOrganizationService service,
            CrmConnection crmConnection,
            SolutionManager solutionManager,
            List<Setting> settings,
            MergedInSolutionComponent component)
        {
            this._window = window;
            this.CurrentSolutionManager = solutionManager;
            this.CurrentSolutionComponent = component;
            RegisterCommands();
        }

        private Guid _getLayersTaskId = Guid.NewGuid();
        private void GetComponentLayers()
        {
            SetDialog("Retreiving layers...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;
                try
                {
                    
                   
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
            }, string.Empty, _getLayersTaskId);
        }
       


        protected override void RegisterCommands()
        {
            Commands.Add("CancelCommand", CancelCommand);
        }


        private ICommand _cancelCommand = null;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            this._window.Close();
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
                return _cancelCommand;
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
