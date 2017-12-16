using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FriendOrganizer.Model;
using FriendOrganizer.UI.ViewModel;

namespace FriendOrganizer.UI.Wrapper
{
    public class FriendWrapper:ViewModelBase,INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();

        public FriendWrapper(Friend model) {
            Model = model;
        }

        public Friend Model { get; }

        public int Id => Model.Id;

        public string FirstName {
            get => Model.FirstName;
            set {
                Model.FirstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName {
            get => Model.LastName;
            set {
                Model.LastName = value;
                OnPropertyChanged();
            }
        }

        public string Email {
            get => Model.Email;
            set {
                Model.Email = value;
                OnPropertyChanged();
            } 
        }

        public bool HasErrors => _errorsByPropertyName.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName) {
            return _errorsByPropertyName.ContainsKey(propertyName)
                ? _errorsByPropertyName[propertyName]
                : null;
        }
        
        private void OnErrorsChanged(string propertyName) {
            ErrorsChanged?.Invoke(this,new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>Simplify the way to add an error to the 'errors Dictionary'."/>.</summary>
        private void AddError(string propertyName, string error) {
            if(!_errorsByPropertyName.ContainsKey(propertyName))
                _errorsByPropertyName[propertyName] = new List<string>();

            if (!_errorsByPropertyName[propertyName].Contains(error)) {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>Simplify the way to remove errors from the 'errors Dictionary'."/>.</summary>
        private void ClearErrors(string propertyName) {
            _errorsByPropertyName.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }
}
