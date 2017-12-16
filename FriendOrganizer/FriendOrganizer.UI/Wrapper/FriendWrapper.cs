using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using FriendOrganizer.Model;
using FriendOrganizer.UI.ViewModel;

namespace FriendOrganizer.UI.Wrapper
{
    public class FriendWrapper:NotifyDataErrorBase
    {

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
                ValdiateProperty(nameof(FirstName));
            }
        }

        private void ValdiateProperty(string propertyName) {
            ClearErrors(propertyName);
            switch (propertyName) {
                case nameof(FirstName):
                    if (string.Equals(FirstName, "Robot", StringComparison.CurrentCultureIgnoreCase))
                        AddError(propertyName, "Robots are not valid friends.");
                    break;
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
    }

    public class NotifyDataErrorBase:ViewModelBase,INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();

        public bool HasErrors => _errorsByPropertyName.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName)
                ? _errorsByPropertyName[propertyName]
                : null;
        }

        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>Simplify the way to add an error to the 'errors Dictionary'."/>.</summary>
        protected void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
                _errorsByPropertyName[propertyName] = new List<string>();

            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>Simplify the way to remove errors from the 'errors Dictionary'."/>.</summary>
        protected void ClearErrors(string propertyName)
        {
            _errorsByPropertyName.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }
}
