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



    public class CreateSolutionManagerViewModel : BaseViewModel
    {
        private const string DefaultPublisherSettingKey = "DEFAULT_PUBLISHER_ID";
        private const string PublisherLogicalName = "publisher";


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
                UniqueName = StringFormatter.FormatString(value);
                RaiseCanExecuteChanged();
            }
        }



        private Solution _createdSolution = null;
        public Solution CreatedSolution
        {
            get
            {
                return _createdSolution;
            }
            set
            {
                _createdSolution = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CreatedSolution"));
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
                UpdateListToCollection(value, PublishersCollection);
            }
        }


        public IOrganizationService Service { get; set; }


        public CrmConnection CurrentCrmConnection { get; set; }
        public SolutionManager CurrentSolutionManager { get; set; }
        private Window _window;

        public Guid DefaultPublisherId { get; set; }


        public void Initialize(
            Window window,
            IOrganizationService service,
            CrmConnection crmConnection,
            SolutionManager solutionManager,
            List<Setting> settings,
            List<EntityReference> publishers)
        {
            this._window = window;
            this.Service = service;
            this.CurrentCrmConnection = crmConnection;
            this.CurrentSolutionManager = solutionManager;
            this.CreatedSolution = null;
            this.Publishers = publishers;
            SetDefaultPublisher(settings);
            RegisterCommands();
        }

        private void SetDefaultPublisher(List<Setting> settings)
        {
            var defaultPublisherIdStr = SettingsManager.GetSetting<string>(settings, DefaultPublisherSettingKey, string.Empty);
            if (!string.IsNullOrEmpty(defaultPublisherIdStr))
            {
                if (Guid.TryParse(defaultPublisherIdStr, out Guid defaultPublisherId))
                {
                    var defaultPublisher = Publishers.FirstOrDefault(k => k.Id == defaultPublisherId);
                    if (defaultPublisher != null)
                    {
                        SelectedPublisher = defaultPublisher;
                    }
                }
            }
        }


        protected override void RegisterCommands()
        {
            Commands.Add("CreateCommand", CreateCommand);
        }



        private ICommand _createCommand = null;
        public ICommand CreateCommand
        {
            get
            {
                if (_createCommand == null)
                {
                    _createCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            var created = CreateSolution();
                            this.CreatedSolution = created;
                            this._window.Close();
                        }
                        catch (Exception ex)
                        {

                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return !string.IsNullOrEmpty(NewDisplayName)
                            && SelectedPublisher != null;
                    });
                }
                return _createCommand;
            }
        }

        private Solution CreateSolution()
        {
            Solution targetSolution;
            var description = new StringBuilder();
            description
                .AppendLine(
                    string.Format("## Solution created by {0} at {1} ##",
                    CurrentCrmConnection.Username,
                    DateTime.Now.ToString()));
            var solution = CurrentSolutionManager.CreateSolution(NewDisplayName, UniqueName, SelectedPublisher, description.ToString());
            targetSolution = solution;
            return targetSolution;
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
