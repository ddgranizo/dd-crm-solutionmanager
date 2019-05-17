using SolutionManagerUI.Commands;
using SolutionManagerUI.Models;
using SolutionManagerUI.Utilities.Threads;
using SolutionManagerUI.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                            IsNewCommandPanelVisible = false;
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
                return _cancelRequestCommand;
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
                            connections.Add(new CrmConnection() { Endpoint = NewUrl, Password = NewPassword, Name = NewName, Username = NewUsername });
                            Connections = connections;
                            IsNewCommandPanelVisible = false;
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
                return _confirmNewConnectionCommand;
            }
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
