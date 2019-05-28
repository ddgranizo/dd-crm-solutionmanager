﻿using DD.Crm.SolutionManager;
using DD.Crm.SolutionManager.Models;
using DD.Crm.SolutionManager.Models.Data;
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



    public class SuperSolutionsManagerViewModel : BaseViewModel
    {

        private const string FormatSuperSolutionSettingKeyName = "COMPONENT_TYPE_{0}_SUPERSOLUTION";
        private const string FormatWebResourceSuperSolutionSettingKeyName = "COMPONENT_TYPE_{0}_{1}_SUPERSOLUTION";
        private const int WebResourceType = 61;

        private Solution _selectedSuperSolution = null;
        public Solution SelectedSuperSolution
        {
            get
            {
                return _selectedSuperSolution;
            }
            set
            {
                _selectedSuperSolution = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedSuperSolution"));
                RaiseCanExecuteChanged();
                if (value != null)
                {
                    this.SolutionComponentsForSupersolutions = GetSolutionComponentsForSolution(value);
                }
            }
        }



        private readonly ObservableCollection<Solution> _superSolutionsCollection = new ObservableCollection<Solution>();
        public ObservableCollection<Solution> SuperSolutionsCollection
        {
            get
            {
                return _superSolutionsCollection;
            }
        }



        private List<Solution> _superSolutions = new List<Solution>();
        public List<Solution> SuperSolutions
        {
            get
            {
                return _superSolutions;
            }
            set
            {
                _superSolutions = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SuperSolutions"));
                UpdateListToCollection(value, SuperSolutionsCollection);
                RaiseCanExecuteChanged();
            }
        }


        private bool _isAggregatedMode = false;
        public bool IsAggregatedMode
        {
            get
            {
                return _isAggregatedMode;
            }
            set
            {
                _isAggregatedMode = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsAggregatedMode"));
            }
        }


        private AggregatedSolution _currentAggregatedSolution = null;
        public AggregatedSolution CurrentAggregatedSolution
        {
            get
            {
                return _currentAggregatedSolution;
            }
            set
            {
                _currentAggregatedSolution = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentAggregatedSolution"));
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






        public IOrganizationService Service { get; set; }




        private readonly ObservableCollection<MergedInSolutionComponent> _solutionComponentsCollection = new ObservableCollection<MergedInSolutionComponent>();
        public ObservableCollection<MergedInSolutionComponent> SolutionComponentsCollection
        {
            get
            {
                return _solutionComponentsCollection;
            }
        }


        private List<MergedInSolutionComponent> _solutionComponents = null;
        public List<MergedInSolutionComponent> SolutionComponents
        {
            get
            {
                return _solutionComponents;
            }
            set
            {
                _solutionComponents = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SolutionComponents"));
                UpdateListToCollection(_solutionComponents, SolutionComponentsCollection);
            }
        }


        private readonly ObservableCollection<MergedInSolutionComponent> _solutionComponentsForSupersolutionsCollection = new ObservableCollection<MergedInSolutionComponent>();
        public ObservableCollection<MergedInSolutionComponent> SolutionComponentsForSupersolutionsCollection
        {
            get
            {
                return _solutionComponentsForSupersolutionsCollection;
            }
        }


        private List<MergedInSolutionComponent> _solutionComponentsForSupersolutions = null;
        public List<MergedInSolutionComponent> SolutionComponentsForSupersolutions
        {
            get
            {
                return _solutionComponentsForSupersolutions;
            }
            set
            {
                _solutionComponentsForSupersolutions = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SolutionComponentsForSupersolutions"));
                UpdateListToCollection(value, SolutionComponentsForSupersolutionsCollection);
            }
        }

        public Dictionary<string, string> ComponentTypesSolutionMapping { get; set; }

        public CrmConnection CurrentCrmConnection { get; set; }
        public SolutionManager CurrentSolutionManager { get; set; }
        private Window _window;



        public void Initialize(
            Window window,
            IOrganizationService service,
            CrmConnection crmConnection,
            SolutionManager solutionManager,
            List<Setting> settings,
            List<MergedInSolutionComponent> solutionComponents,
            AggregatedSolution aggregatedSolution)
        {
            this._window = window;
            this.SolutionComponents = solutionComponents;
            this.Service = service;
            this.CurrentCrmConnection = crmConnection;
            this.CurrentSolutionManager = solutionManager;
            SetSuperSolutions(service, settings);

            IsAggregatedMode = false;
            if (aggregatedSolution != null)
            {
                IsAggregatedMode = true;
                //PrepareAggregatedSolution(settings, aggregatedSolution);
            }

            RegisterCommands();
        }



        private Guid _mergeTaskId = Guid.NewGuid();
        private void MergeWithSuperSolutions()
        {
            SetDialog("Merging...");
            ThreadManager.Instance.ScheduleTask(() =>
            {
                var isError = false;
                var errorMessage = string.Empty;
                var mappingsWithBackpus = new Dictionary<string, string>();
                try
                {
                    List<Solution> affectedSuperSolutions = GetAffectedSuperSolutions();
                    UpdateDialogMessage($"Cloning affected solutions... ({affectedSuperSolutions.Count})");
                    mappingsWithBackpus = CloneAffectedSolutions(affectedSuperSolutions);
                    CleanAndMergeSourceSolutions(affectedSuperSolutions, mappingsWithBackpus);
                    RemovedClonedBackupSolutions(affectedSuperSolutions, mappingsWithBackpus);

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
            }, "Merging solution components...", _mergeTaskId);


        }

        private void CleanAndMergeSourceSolutions(List<Solution> affectedSuperSolutions, Dictionary<string, string> mappings)
        {
            foreach (var item in affectedSuperSolutions)
            {
                UpdateDialogMessage($"Cleaning source solution {item.UniqueName}...");
                CurrentSolutionManager.CleanSolution(item.Id);
                UpdateDialogMessage($"Merging new result in source solution {item.UniqueName}...");

                var sourceKey = item.UniqueName;
                var backupKey = mappings[sourceKey];

                var backupSolution = CurrentSolutionManager.GetSolution(backupKey);
                var componentsInBackupSolution = CurrentSolutionManager.GetSolutionComponents(backupSolution.Id, false);
                UpdateDialogMessage($"Calculating new components for {item.UniqueName}...");

                var componentsForThisSolution = GetSolutionComponentsForSolution(item);

                var newSuperSolutionComponents =
                    CurrentSolutionManager
                        .GetMergedSolutionComponents(new List<Guid>() { item.Id }, componentsForThisSolution, false);
                UpdateDialogMessage($"Adding components to supersolution {item.UniqueName}...");
                CurrentSolutionManager.CreateMergedSolution(item.Id, newSuperSolutionComponents);
                UpdateDialogMessage($"Increasing revision version of {item.UniqueName}...");
                CurrentSolutionManager.IncreaseSolutionRevisionVersion(item.Id);
            }
        }


        private void RemovedClonedBackupSolutions(List<Solution> affectedSuperSolutions, Dictionary<string, string> mappings)
        {
            UpdateDialogMessage($"Removing backup solutions...");
            foreach (var item in affectedSuperSolutions)
            {
                var sourceKey = item.UniqueName;
                var backupKey = mappings[sourceKey];

                CurrentSolutionManager.RemoveSolution(backupKey);
            }

        }
        private Dictionary<string, string> CloneAffectedSolutions(List<Solution> affectedSuperSolutions)
        {
            var mappingsWithBackpus = new Dictionary<string, string>();
            try
            {
                foreach (var item in affectedSuperSolutions)
                {
                    mappingsWithBackpus.Add(item.UniqueName, string.Format("{0}_backup", item.UniqueName));
                    CurrentSolutionManager
                        .CloneSolution(
                            item.Id,
                            mappingsWithBackpus[item.UniqueName],
                            mappingsWithBackpus[item.UniqueName],
                            item.Version,
                            item.Description);
                }
            }
            catch (Exception)
            {
                UpdateDialogMessage($"Rolling back cloning solutions...");
                foreach (var keyValue in mappingsWithBackpus)
                {
                    var originalSolution = keyValue.Key;
                    var clonedVersion = keyValue.Value;
                    CurrentSolutionManager.RemoveSolution(clonedVersion);
                }
                throw;
            }

            return mappingsWithBackpus;

        }

        private List<Solution> GetAffectedSuperSolutions()
        {
            var affectedSuperSolutions = new List<Solution>();
            foreach (var superSolution in SuperSolutions)
            {
                var components = GetSolutionComponentsForSolution(superSolution);
                if (components.Count > 0)
                {
                    affectedSuperSolutions.Add(superSolution);
                }
            }

            return affectedSuperSolutions;
        }

        protected override void RegisterCommands()
        {
            Commands.Add("FindUnassignedComponentsCommand", FindUnassignedComponentsCommand);
            Commands.Add("DoMergeWithSupersolutionsCommand", DoMergeWithSupersolutionsCommand);
        }


        private ICommand _doMergeWithSupersolutionsCommand = null;
        public ICommand DoMergeWithSupersolutionsCommand
        {
            get
            {
                if (_doMergeWithSupersolutionsCommand == null)
                {
                    _doMergeWithSupersolutionsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            MergeWithSuperSolutions();
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
                return _doMergeWithSupersolutionsCommand;
            }
        }

        private ICommand _findUnassignedComponentsCommand = null;
        public ICommand FindUnassignedComponentsCommand
        {
            get
            {
                if (_findUnassignedComponentsCommand == null)
                {
                    _findUnassignedComponentsCommand = new RelayCommand((object param) =>
                    {
                        try
                        {
                            this.SolutionComponentsForSupersolutions = GetUnasiggnedSolutionComponents();
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
                return _findUnassignedComponentsCommand;
            }
        }


        private List<MergedInSolutionComponent> GetUnasiggnedSolutionComponents()
        {
            //TODO: it doesn't work well with the change of webresources. Fixit!
            if (SelectedSuperSolution != null)
            {
                var typesNotAllowed = ComponentTypesSolutionMapping
                        .Select(k => k.Key)
                        .ToList();
                return SolutionComponents
                        .Where(k => typesNotAllowed.IndexOf(((int)k.Type).ToString()) < 0)
                        .ToList();
            }
            return new List<MergedInSolutionComponent>();
        }


        private List<MergedInSolutionComponent> GetSolutionComponentsForSolution(Solution solution)
        {
            var output = new List<MergedInSolutionComponent>(); ;
            if (solution != null)
            {
                var typesAllowed = ComponentTypesSolutionMapping
                        .Where(k => k.Value == solution.UniqueName)
                        .Select(k => k.Key)
                        .ToList();
                foreach (var type in typesAllowed)
                {
                    if (type.IndexOf("_")>-1)
                    {
                        output.AddRange(SolutionComponents
                            .Where(k => k.Type == SolutionComponentBase.SolutionComponentType.WebResource)
                            .Where(k=> ((WebResourceData)k.ObjectDefinition).WebResourceType.ToString() == type.Split('_')[1]));
                    }
                    else
                    {
                        output.AddRange(SolutionComponents.Where(k => ((int)k.Type).ToString() == type));
                    }
                }
                //return SolutionComponents
                //        .Where(k => typesAllowed.IndexOf(((int)k.Type).ToString()) > -1)
                //        .ToList();
            }
            return output;
        }


        private void SetSuperSolutions(IOrganizationService service, List<Setting> settings)
        {
            //TODO: set array of valid types = [1, 2, 3, 4, 5, 20, 21, etc...]
            List<string> solutionUniqueNames = new List<string>();
            List<Solution> solutions = new List<Solution>();
            this.ComponentTypesSolutionMapping = new Dictionary<string, string>();

            for (int i = 1; i < 999; i++)
            {
                if (i != WebResourceType)
                {
                    var settingForThisComponent = settings.FirstOrDefault(k => k.Key == string.Format(FormatSuperSolutionSettingKeyName, i));
                    if (settingForThisComponent != null)
                    {
                        var value = settingForThisComponent.Value;
                        if (!this.ComponentTypesSolutionMapping.ContainsKey(i.ToString()))
                        {
                            this.ComponentTypesSolutionMapping.Add(i.ToString(), value);
                        }

                        if (solutionUniqueNames.IndexOf(value) < 0)
                        {
                            solutionUniqueNames.Add(settingForThisComponent.Value);
                        }
                    }
                }
                else
                {
                    for (int j = 1; j < 12; j++) //Different types of webresources
                    {
                        var settingForThisComponent = settings.FirstOrDefault(k => k.Key == string.Format(FormatWebResourceSuperSolutionSettingKeyName, i, j));
                        if (settingForThisComponent != null)
                        {
                            var value = settingForThisComponent.Value;
                            if (!this.ComponentTypesSolutionMapping.ContainsKey($"{i}_{j}"))
                            {
                                this.ComponentTypesSolutionMapping.Add($"{i}_{j}", value);
                            }

                            if (solutionUniqueNames.IndexOf(value) < 0)
                            {
                                solutionUniqueNames.Add(settingForThisComponent.Value);
                            }
                        }
                    }
                }
            }

            foreach (var solutionUniqueName in solutionUniqueNames)
            {
                solutions.Add(CurrentSolutionManager.GetSolution(solutionUniqueName));
            }

            this.SuperSolutions = solutions;
            if (this.SuperSolutions.Count > 0)
            {
                SelectedSuperSolution = this.SuperSolutions[0];
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
