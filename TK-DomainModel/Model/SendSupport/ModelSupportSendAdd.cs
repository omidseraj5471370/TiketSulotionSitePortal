using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Tk_Infrastucture_Customer.Model.SendSupport
{
    [Serializable]
    public class ModelSupportSendAdd
    {
        public ModelSupportSendAdd()
        {
            createdon = DateTime.Now;
            email = null;
            coderahgiri = null;

        }

        public Guid supportid { get; set; }
        public DateTime createdon { get; set; }
        public string title { get; set; }
        public string mobilephone { get; set; }
        public string email { get; set; }
        public int type { get; set; }
        public string description { get; set; }
        public int statusCode { get; set; }
        public int stateCode { get; set; }
        public string coderahgiri { get; set; }
        public string ownername{ get; set; }

    }
}
