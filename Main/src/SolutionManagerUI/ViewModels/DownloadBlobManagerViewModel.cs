using DD.Crm.SolutionManager;
using DD.Crm.SolutionManager.Models;
using Microsoft.Xrm.Sdk;
using SolutionManagerUI.Commands;
using SolutionManagerUI.Models;
using SolutionManagerUI.Providers;
using SolutionManagerUI.Services;
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

    public class DownloadBlobManagerViewModel : BaseViewModel
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


        private readonly ObservableCollection<string> _blobsCollection = new ObservableCollection<string>();
        public ObservableCollection<string> BlobsCollection
        {
            get
            {
                return _blobsCollection;
            }
        }


        private List<string> _blobs = null;
        public List<string> Blobs
        {
            get
            {
                return _blobs;
            }
            set
            {
                _blobs = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Blobs"));
                UpdateListToCollection(value, BlobsCollection);
            }
        }


        private string _selectedBlob = null;
        public string SelectedBlob
        {
            get
            {
                return _selectedBlob;
            }
            set
            {
                _selectedBlob = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedBlob"));
                RaiseCanExecuteChanged();
            }
        }
        public string OutputPath { get; set; }
        public BlobStorageService BlobService { get; set; }

        public List<Setting> Settings { get; set; }
        public void Initialize(
            Window window,
            IOrganizationService service,
            CrmConnection crmConnection,
            SolutionManager solutionManager,
            List<Setting> settings,
            BlobStorageService blobService)
        {
            this._window = window;
            this.Settings = settings;
            this.OutputPath = null;
            RegisterCommands();
            this.BlobService = blobService;
            this.Blobs = this.BlobService.GetBlobsInContainer();
        }

        private Guid _downloadTaskId = Guid.NewGuid();
        private void DownloadSolution()
        {
            MessageBox.Show("Select forlder for donwload the solution from de Blobstorage");
            string defaultPath = GetDefaultZipPath();
            var path = FileDialogManager.SelectPath(defaultPath);
            path = StringFormatter.GetPathWithLastSlash(path);
            path = string.Format("{0}{1}", path, SelectedBlob);
            SetDialog("Downloading...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;
                try
                {
                    BlobService.Download(SelectedBlob, path);
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
                        this.OutputPath = path;
                        _window.Close();
                    }
                    UnsetDialog();
                });
            }, string.Empty, _downloadTaskId);
        }

        private string GetDefaultZipPath()
        {
            return BlobService.GetDefaultPathForDownload();
        }

        protected override void RegisterCommands()
        {
            Commands.Add("DownloadSolutionCommand", DownloadSolutionCommand);
        }


        private ICommand _downloadSolutionCommand = null;
        public ICommand DownloadSolutionCommand
        {
            get
            {
                if (_downloadSolutionCommand == null)
                {
                    _downloadSolutionCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            DownloadSolution();
                        }
                        catch (Exception ex)
                        {
                            RaiseError(ex.Message);
                        }
                    }, (param) =>
                    {
                        return !string.IsNullOrEmpty(SelectedBlob);
                    });
                }
                return _downloadSolutionCommand;
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
