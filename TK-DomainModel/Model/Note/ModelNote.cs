using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tk_Infrastucture_Customer.Model.Note
{
    [Serializable]
    public class ModelNote
    {
        public Guid? Id { get; set; }
        
        public Guid? objectid{ get; set; }
        
        public string subject{ get; set; }
        public string name { get; set; }
        public string notetext { get; set; }
        public DateTime? createon { get; set; }
    }
}
