using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tk_Infrastucture_Customer.Model.SendSupport
{
    public class ModelType
    {
        //سوال
        public int soal { get; private set; } = 100000002;

        //درخواست تغییرات
        public int darkhasttaghirat { get; private set; } = 100000000;

        //مشکل نرم افزار
        public int moshkeldarnarmafzar { get; private set; } = 100000001;

    }
}
