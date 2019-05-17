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
    public class SettingManagerViewModel : BaseViewModel
    {


        private Window _window;






        private readonly ObservableCollection<Setting> _settingsCollection = new ObservableCollection<Setting>();
        public ObservableCollection<Setting> SettingsCollection { get { return _settingsCollection; } }


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
                UpdateListToCollection(value, SettingsCollection);
            }
        }



        private Setting _selectedSetting = null;
        public Setting SelectedSetting
        {
            get
            {
                return _selectedSetting;
            }
            set
            {
                _selectedSetting = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedSetting"));
                RaiseCanExecuteChanged();

                if (_selectedSetting != null)
                {
                    Key = _selectedSetting.Key;
                    Value = _selectedSetting.Value;
                    IsEditCommandPanelVisible = true;
                }
            }
        }


        private string _value = null;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Value"));
                RaiseCanExecuteChanged();
            }
        }

        private string _newValue = null;
        public string NewValue
        {
            get
            {
                return _newValue;
            }
            set
            {
                _newValue = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NewValue"));
                RaiseCanExecuteChanged();
            }
        }


        private string _key = null;
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Key"));
                RaiseCanExecuteChanged();
            }
        }

        private string _newKey = null;
        public string NewKey
        {
            get
            {
                return _newKey;
            }
            set
            {
                _newKey = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NewKey"));
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




        public void Initialize(Window window, List<Setting> settings)
        {
            this._window = window;
            this.Settings = settings;
            RegisterCommands();
        }

        protected override void RegisterCommands()
        {
            Commands.Add("ConfirmNewSettingCommand", ConfirmNewSettingCommand);
            Commands.Add("RequestNewSettingCommand", RequestNewSettingCommand);
            Commands.Add("CancelRequestCommand", CancelRequestCommand);
            Commands.Add("RemoveSettingCommand", RemoveSettingCommand);
            Commands.Add("ConfirmEditSettingCommand", ConfirmEditSettingCommand);

        }


        private ICommand _removeSettingCommand = null;
        public ICommand RemoveSettingCommand
        {
            get
            {
                if (_removeSettingCommand == null)
                {
                    _removeSettingCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            if (SelectedSetting != null)
                            {
                                var response = MessageBox.Show("Confirm the delete? Cannot be undone", "Confirm", MessageBoxButton.OKCancel);
                                if (response == MessageBoxResult.OK)
                                {
                                    var _settings = Settings;
                                    var index = _settings.IndexOf(SelectedSetting);
                                    _settings.RemoveAt(index);
                                    ReloadSettings();
                                    SelectedSetting = null;
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
                        return SelectedSetting != null;
                           
                    });
                }
                return _removeSettingCommand;
            }
        }



        private ICommand _requestNewSettingCommand = null;
        public ICommand RequestNewSettingCommand
        {
            get
            {
                if (_requestNewSettingCommand == null)
                {
                    _requestNewSettingCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            IsNewCommandPanelVisible = true;
                            IsEditCommandPanelVisible = false;
                            SelectedSetting = null;

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
                return _requestNewSettingCommand;
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
                            SelectedSetting = null;
                            IsNewCommandPanelVisible = false;
                            IsEditCommandPanelVisible = false;
                            NewKey = null;
                            NewValue = null;
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


        private ICommand _confirmEditSettingCommand = null;
        public ICommand ConfirmEditSettingCommand
        {
            get
            {
                if (_confirmEditSettingCommand == null)
                {
                    _confirmEditSettingCommand = new RelayCommand((object param) =>
                    {
                        try
                        {

                            SelectedSetting.Key = Key;
                            SelectedSetting.Value = Value;
                            SelectedSetting = null;
                            IsEditCommandPanelVisible = false;
                            ReloadSettings();

                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return !string.IsNullOrEmpty(Key)
                            && !string.IsNullOrEmpty(Value);
                    });
                }
                return _confirmEditSettingCommand;
            }
        }


        private ICommand _confirmNewSettingCommand = null;
        public ICommand ConfirmNewSettingCommand
        {
            get
            {
                if (_confirmNewSettingCommand == null)
                {
                    _confirmNewSettingCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var settings = Settings;
                            settings.Add(new Setting()
                            {
                                Key = NewKey,
                                Value = NewValue,
                            });
                            Settings = settings;
                            IsNewCommandPanelVisible = false;
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return !string.IsNullOrEmpty(NewKey)
                            && !string.IsNullOrEmpty(NewValue);
                    });
                }
                return _confirmNewSettingCommand;
            }
        }

        private void ReloadSettings()
        {
            var _settings = Settings;
            Settings = _settings;
        }

    }
}
