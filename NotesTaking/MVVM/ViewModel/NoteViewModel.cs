using System.Collections.ObjectModel;
using System.ComponentModel;
using NotesTaking.MVVM.Model;

namespace NotesTaking.MVVM.ViewModel
{
    public class NoteViewModel : INotifyPropertyChanged
    {
        private string _noteTitle;
        private string _noteContent;

        public ObservableCollection<Note> Notes { get; set; }

        public string NoteTitle
        {
            get => _noteTitle;
            set
            {
                _noteTitle = value;
                OnPropertyChanged(nameof(NoteTitle));
            }
        }

        public string NoteContent
        {
            get => _noteContent;
            set
            {
                _noteContent = value;
                OnPropertyChanged(nameof(NoteContent));
            }
        }

        public NoteViewModel()
        {
            Notes = new ObservableCollection<Note>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
