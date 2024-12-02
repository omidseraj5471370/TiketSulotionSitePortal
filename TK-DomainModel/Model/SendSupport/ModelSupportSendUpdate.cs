using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Tk_Infrastucture_Customer.Model.Note;

namespace TK_DomainModel.Model.SendSupport
{
    [Serializable]
    public class ModelSupportSendUpdate
    {

        public string coderahgiry { get; set; }
        public Guid supportid { get; set; }
       public List<ModelNote> notes { get; set; }

       public string entityname  { get; set; }
       public string getId { get; set; }

        public string idfactory { get; set; }

    }
}
