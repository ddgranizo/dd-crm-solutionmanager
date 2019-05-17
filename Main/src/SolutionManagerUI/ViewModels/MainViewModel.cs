using SolutionManagerUI.Commands;
using SolutionManagerUI.Models;
using SolutionManagerUI.Utilities;
using SolutionManagerUI.ViewModels.Base;
using SolutionManagerUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SolutionManagerUI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {



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
            }
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
        }


        protected override void RegisterCommands()
        {
            Commands.Add("NewCommand", NewCommand);
            Commands.Add("OpenConnectionsCommand", OpenConnectionsCommand);
            Commands.Add("OpenSettingsCommand", OpenSettingsCommand);
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
    }
}
