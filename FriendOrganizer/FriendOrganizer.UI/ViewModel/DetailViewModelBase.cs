using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        private bool _hasChanges;
        protected readonly IEventAggregator EventAggregator;
        private int _id;
        private string _title;

        public DetailViewModelBase(IEventAggregator eventAggregator) {
            EventAggregator = eventAggregator;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute); 
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public string Title {
            get => _title;
            protected set {
                _title = value;
                OnPropertyChanged();
            }
        }

        public bool HasChanges {
            get => _hasChanges;
            set {
                if (_hasChanges == value) return;
                _hasChanges = value;
                OnPropertyChanged();
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
        public int Id {
            get => _id;
            protected set => _id = value;
        }

        protected abstract void OnDeleteExecute();

        protected abstract bool OnSaveCanExecute();

        protected abstract void OnSaveExecute();

        public abstract Task LoadAsync(int? id);

        protected virtual void RaiseDetailDeletedEvent(int modelId) {
            EventAggregator.GetEvent<AfterDetailDeletedEvent>().Publish(
                new AfterDetailDeletedEventArgs {
                    Id = modelId,
                    ViewModelName = this.GetType().Name
                });
        }

        protected virtual void RaiseDetailSavedEvent(int modelId, string displayMember)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(
                new AfterDetailSavedEventArgs()
                {
                    Id = modelId,
                    DisplayMember = displayMember,
                    ViewModelName = this.GetType().Name
                });
        }
    }

}
