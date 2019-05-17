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
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SolutionManagerUI.ViewModels
{
    public class ConnectionManagerViewModel : BaseViewModel
    {


        private Window _window;






        private readonly ObservableCollection<CrmConnection> _connectionsCollection = new ObservableCollection<CrmConnection>();
        public ObservableCollection<CrmConnection> ConnectionsCollection { get { return _connectionsCollection; } }


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



        private CrmConnection _selectedCrmConnection = null;
        public CrmConnection SelectedCrmConnection
        {
            get
            {
                return _selectedCrmConnection;
            }
            set
            {
                _selectedCrmConnection = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedCrmConnection"));
                RaiseCanExecuteChanged();

                if (_selectedCrmConnection != null)
                {
                    IsEditCommandPanelVisible = true;
                    IsNewCommandPanelVisible = false;
                    Username = _selectedCrmConnection.Username;
                    Name = _selectedCrmConnection.Name;
                    Url = _selectedCrmConnection.Endpoint;
                }

            }
        }


        private int _errorTasks = 0;
        public int ErrorTasks
        {
            get
            {
                return _errorTasks;
            }
            set
            {
                _errorTasks = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ErrorTasks"));
            }
        }


        private int _activeTasks = 0;
        public int ActiveTasks
        {
            get
            {
                return _activeTasks;
            }
            set
            {
                _activeTasks = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ActiveTasks"));
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

        private string _Username = null;
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Username"));
                RaiseCanExecuteChanged();
            }
        }

        private string _Url = null;
        public string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                _Url = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Url"));
                RaiseCanExecuteChanged();
            }
        }


        private string _Password = null;
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Password"));
                RaiseCanExecuteChanged();
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
                RaiseCanExecuteChanged();
            }
        }

        private string _newUsername = null;
        public string NewUsername
        {
            get
            {
                return _newUsername;
            }
            set
            {
                _newUsername = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NewUsername"));
                RaiseCanExecuteChanged();
            }
        }

        private string _newUrl = null;
        public string NewUrl
        {
            get
            {
                return _newUrl;
            }
            set
            {
                _newUrl = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NewUrl"));
                RaiseCanExecuteChanged();
            }
        }


        private string _newPassword = null;
        public string NewPassword
        {
            get
            {
                return _newPassword;
            }
            set
            {
                _newPassword = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NewPassword"));
                RaiseCanExecuteChanged();
            }
        }



        private bool _isNewCommandPanelVisible = false;
        public bool IsNewCommandPanelVisible
        {
            get
            {
                return _isNewCommandPanelVisible;
            }
            set
            {
                _isNewCommandPanelVisible = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsNewCommandPanelVisible"));
            }
        }


        private bool _isEditCommandPanelVisible = false;
        public bool IsEditCommandPanelVisible
        {
            get
            {
                return _isEditCommandPanelVisible;
            }
            set
            {
                _isEditCommandPanelVisible = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsEditCommandPanelVisible"));
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

        public ConnectionManagerViewModel()
        {
            ThreadManager.Instance.OnStartedTask += Instance_OnStartedTask;
            ThreadManager.Instance.OnScheduledTask += Instance_OnScheduledTask;
            ThreadManager.Instance.OnCompletedTask += Instance_OnCompletedTask;
            ThreadManager.Instance.OnErrorTask += Instance_OnErrorTask;
        }

        public void Initialize(Window window, List<CrmConnection> connections)
        {
            this._window = window;
            this.Connections = connections;

            RegisterCommands();
        }

        protected override void RegisterCommands()
        {
            Commands.Add("ConfirmNewConnectionCommand", ConfirmNewConnectionCommand);
            Commands.Add("RequestNewConnectionCommand", RequestNewConnectionCommand);
            Commands.Add("CancelRequestCommand", CancelRequestCommand);
            Commands.Add("RemoveConnectionCommand", RemoveConnectionCommand);
            Commands.Add("ConfirmEditConnectionCommand", ConfirmEditConnectionCommand);
            Commands.Add("TestConnectionCommand", TestConnectionCommand);

        }


        private Guid _testConnectionId = Guid.NewGuid();


        private bool _isTestingConnection = false;
        public bool IsTestingConnection
        {
            get
            {
                return _isTestingConnection;
            }
            set
            {
                _isTestingConnection = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsTestingConnection"));
                RaiseCanExecuteChanged();
            }
        }

        private ICommand _testConnectionCommand = null;
        public ICommand TestConnectionCommand
        {
            get
            {
                if (_testConnectionCommand == null)
                {
                    _testConnectionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var stringConnection
                                = string.Format(@"ServiceUri={0}; Username={1}; Password={2}; authtype=Office365; RequireNewInstance=True;",
                                    SelectedCrmConnection.Endpoint,
                                    SelectedCrmConnection.Username,
                                    Crypto.Decrypt(SelectedCrmConnection.Password));

                            IsTestingConnection = true;
                            SetDialog("Testing connection...");
                            ThreadManager.Instance.ScheduleTask(() =>
                            {
                                string okMessage = "Connection OK!";
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
                                    IsTestingConnection = false;
                                    string message = service != null
                                        ? okMessage
                                        : koMessage;
                                    UpdateDialogMessage(message, 2000);
                                    
                                });
                            }, "Validating connection...", _testConnectionId);
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return SelectedCrmConnection != null
                            && !IsTestingConnection;
                    });
                }
                return _testConnectionCommand;
            }
        }

        private ICommand _removeConnectionCommand = null;
        public ICommand RemoveConnectionCommand
        {
            get
            {
                if (_removeConnectionCommand == null)
                {
                    _removeConnectionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            if (SelectedCrmConnection != null)
                            {
                                var response = MessageBox.Show("Confirm the delete? Cannot be undone", "Confirm", MessageBoxButton.OKCancel);
                                if (response == MessageBoxResult.OK)
                                {
                                    var _connections = Connections;
                                    var index = _connections.IndexOf(SelectedCrmConnection);
                                    _connections.RemoveAt(index);
                                    ReloadConnections();

                                    SelectedCrmConnection = null;
                                    IsEditCommandPanelVisible = false;
                                }
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return SelectedCrmConnection != null
                            && !IsTestingConnection;
                    });
                }
                return _removeConnectionCommand;
            }
        }



        private ICommand _requestNewConnectionCommand = null;
        public ICommand RequestNewConnectionCommand
        {
            get
            {
                if (_requestNewConnectionCommand == null)
                {
                    _requestNewConnectionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            IsNewCommandPanelVisible = true;
                            IsEditCommandPanelVisible = false;
                            SelectedCrmConnection = null;

                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return !IsTestingConnection;
                    });
                }
                return _requestNewConnectionCommand;
            }
        }

        private ICommand _cancelRequestCommand = null;
        public ICommand CancelRequestCommand
        {
            get
            {
                if (_cancelRequestCommand == null)
                {
                    _cancelRequestCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            SelectedCrmConnection = null;
                            IsNewCommandPanelVisible = false;
                            IsEditCommandPanelVisible = false;

                            NewName = null;
                            NewUsername = null;
                            NewUrl = null;
                            NewPassword = null;
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return  !IsTestingConnection;
                    });
                }
                return _cancelRequestCommand;
            }
        }


        private ICommand _confirmEditConnectionCommand = null;
        public ICommand ConfirmEditConnectionCommand
        {
            get
            {
                if (_confirmEditConnectionCommand == null)
                {
                    _confirmEditConnectionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {


                            SelectedCrmConnection.Endpoint = Url;
                            SelectedCrmConnection.Name = Name;
                            SelectedCrmConnection.Username = Username;
                            if (!string.IsNullOrEmpty(Password))
                            {
                                SelectedCrmConnection.Password = Crypto.Encrypt(Password);
                            }
                            SelectedCrmConnection = null;
                            IsEditCommandPanelVisible = false;
                            ReloadConnections();

                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return !string.IsNullOrEmpty(Name)
                            && !string.IsNullOrEmpty(Username)
                            && !string.IsNullOrEmpty(Url)
                            && !IsTestingConnection;
                    });
                }
                return _confirmEditConnectionCommand;
            }
        }


        private ICommand _confirmNewConnectionCommand = null;
        public ICommand ConfirmNewConnectionCommand
        {
            get
            {
                if (_confirmNewConnectionCommand == null)
                {
                    _confirmNewConnectionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var connections = Connections;
                            connections.Add(new CrmConnection()
                            {
                                Endpoint = NewUrl,
                                Password = Crypto.Encrypt(NewPassword),
                                Name = NewName,
                                Username = NewUsername
                            });
                            Connections = connections;
                            IsNewCommandPanelVisible = false;
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return !string.IsNullOrEmpty(NewName)
                            && !string.IsNullOrEmpty(NewUsername)
                            && !string.IsNullOrEmpty(NewPassword)
                            && !string.IsNullOrEmpty(NewUrl)
                            && !IsTestingConnection;
                    });
                }
                return _confirmNewConnectionCommand;
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


        private void ReloadConnections()
        {
            var connections = Connections;
            Connections = _connections;
        }


        private void Instance_OnErrorTask(object sender, Events.ThreadManagerScheduledTaskEventArgs args)
        {
            UpdateActiveTasks();
        }

        private void Instance_OnCompletedTask(object sender, Events.ThreadManagerScheduledTaskEventArgs args)
        {
            UpdateActiveTasks();
        }

        private void Instance_OnScheduledTask(object sender, Events.ThreadManagerScheduledTaskEventArgs args)
        {
            UpdateActiveTasks();
        }
        private void Instance_OnStartedTask(object sender, Events.ThreadManagerScheduledTaskEventArgs args)
        {
            UpdateActiveTasks();

        }

        private void UpdateActiveTasks()
        {
            ActiveTasks = ThreadManager.Instance.ActiveTasks.Count;
            ErrorTasks = ThreadManager.Instance.CompletedTasks.Where(k => k.IsError).ToList().Count;
            RaiseCanExecuteChanged();
        }
    }
}
