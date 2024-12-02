using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TK_DomainModel.Model.SendSupport
{
    [Serializable]
    public class ModelReciveId
    {
        public Guid id { get; set; }
        public string numberticket { get; set; }
        public string state{ get; set; }
    }
}
