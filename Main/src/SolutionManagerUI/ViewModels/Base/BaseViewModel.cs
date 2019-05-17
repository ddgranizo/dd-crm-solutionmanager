﻿using SolutionManagerUI.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SolutionManagerUI.ViewModels.Base
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }


        readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();
        public Dictionary<string, ICommand> Commands
        {
            get { return _commands; }
        }


        protected virtual void RaiseError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        protected virtual void SubscribeEvents() { }

        protected virtual void SetDefaultValues() { }

        protected virtual void RegisterCommands() { }


        public void UpdateListToCollection<T>(List<T> listItems, ObservableCollection<T> collection)
        {
            collection.Clear();
            if (listItems != null)
            {
                foreach (var item in listItems)
                {
                    collection.Add(item);
                }
            }
        }

        protected void RaiseCanExecuteChanged(object param = null)
        {
            foreach (var command in Commands)
            {
                RaiseCanExecuteChanged(command.Value, param);
            }
        }

        protected void RaiseCanExecuteChanged(ICommand command, object param = null)
        {
            System.Windows.Application app = System.Windows.Application.Current;
            if (app != null)
            {
                if (!app.Dispatcher.CheckAccess())
                {
                    app.Dispatcher.Invoke(new System.Action(() => this.RaiseCanExecuteChanged(command, param)));
                }
                else
                {
                    ((RelayCommand)command).RaiseCanExecuteChanged(param);
                }
            }
        }

    }
}
