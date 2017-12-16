using System.Runtime.CompilerServices;

namespace FriendOrganizer.UI.Wrapper {
    public class ModelWrapper<T> : NotifyDataErrorBase
    {
        public ModelWrapper(T model) {
            Model = model;
        }

        public T Model { get; }

        protected virtual TValue GetValue<TValue>([CallerMemberName] string propertyName = null) {
            return (TValue) typeof(T).GetProperty(propertyName).GetValue(Model);
        }

        protected virtual void SetValue<TValue>(TValue value, [CallerMemberName] string propertyName = null) {
            typeof(T).GetProperty(propertyName).SetValue(Model, value);
            OnPropertyChanged(propertyName);
        }
    }
}