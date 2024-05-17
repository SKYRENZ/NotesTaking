using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesTaking.MVVM.Model
{
    public class Note
    {

        public int NotesID { get; set; }
        public string NoteTitle { get; set; }
        public string NoteContent { get; set; }
    }
}
