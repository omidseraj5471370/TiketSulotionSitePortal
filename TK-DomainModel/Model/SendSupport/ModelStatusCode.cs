using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tk_Infrastucture_Customer.Model.SendSupport
{
    public class ModelStatusCode
    {
        //Active
        //جدید
        public int jadid { get; private set; } = 100000000;

        //تایید شده
        public int taidshode { get; private set; } = 100000001;

        //ارسال شده
        public int ersalshode { get; private set; } = 100000002;

        //درحال بررسی
        public int darhalebaresi { get; private set; } = 100000005;

        //نیاز به تائید زمان پیشنهادی دارد
        public int niyazbetaidzaman { get; private set; } = 100000006;

        //درحال انجام
        public int darhalanjam { get; private set; } = 100000007;

        ///Inactive
        /////تکمیل شده
        public int takmilshode { get; private set; } = 100000003;

        //لغو شده
        public int laghvshod { get; private set; } = 100000004;
    }
}
