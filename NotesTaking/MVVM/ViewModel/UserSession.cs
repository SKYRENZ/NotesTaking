using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesTaking.MVVM.ViewModel
{
    public static class UserSession
    {
        public static string LoggedInUsername { get; set; }
    }
    public class NoteViewModel : INotifyPropertyChanged
    {
        private string _noteTitle;
        private string _noteContent;

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
