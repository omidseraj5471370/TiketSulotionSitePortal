using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tk_Infrastucture_Customer.Model.Note;


namespace TK_DomainModel.Model.SendServer
{
    [Serializable]
    public class ModelSendUpdateAzLuna
    {
        public ModelSendUpdateAzLuna()
        {
            estimatetherequiredtime = 0;
            estimateddeliverydate = new DateTime(1, 1, 1);
        }

        //status code
        public int statuscode { get; set; }

        //پیش بینی زمان مورد نیاز
        public int? estimatetherequiredtime { get; set; } 

        //تاریخ حدودی تحویل
        public DateTime? estimateddeliverydate { get; set; }

        //ticketnumber
        public string numberticket { get; set; }

        //زمان حل پرونده
        public int? billableterms { get; set; }

        //ایدی شرکت
        public string idfactory { get; set; }

        //list not
        public List<ModelNote> nots { get; set; }


        //وضعیت فرم
        public string statecode { get; set; }

        //نام یوزر مالک پرونده
        public string nameUser { get; set; }

        //ایمیل یوزر مالک پرونده
        public string emailUser { get; set; }
    }
}
