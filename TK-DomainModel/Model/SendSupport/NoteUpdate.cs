using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TK_DomainModel.Model.SendSupport
{
    [Serializable]
    public class NoteUpdate
    {
        public string coderahgiri { get; set; }
        public Guid notid { get; set; }
        public string not { get; set; }
        public DateTime notdate { get; set; }
    }
}
