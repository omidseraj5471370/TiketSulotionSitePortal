using Microsoft.Xrm.Sdk.Client;
using System;
using System.ServiceModel.Description;
using Topshelf;
using Xrm;

namespace CustomerService
{
    public class Program
    {
        //نیروفراب
        //static Uri oUri = new Uri("https://niroofarab.niroofarabgroup.com:444/XRMServices/2011/Organization.svc");
        //آردینه
        //static Uri oUri = new Uri("https://crm.ardineh.com/XRMServices/2011/Organization.svc");
        //بهینه چوب
        //static Uri oUri = new Uri("http://dynamics/BehineChoob/XRMServices/2011/Organization.svc");
        //فرانه آرمان
        static Uri oUri = new Uri("http://crm2/FarTest/XRMServices/2011/Organization.svc");
        public static OrganizationServiceProxy _serviceProxy;

        //public static OrganizationServiceProxy ServiceProxy { get => _serviceProxy; }

        public static XrmServiceContext dbxrm;

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {

                    ClientCredentials clientCredentials = new ClientCredentials();
                    //نیروفراب
                    //clientCredentials.UserName.UserName = @"farab\it";
                    //clientCredentials.UserName.Password = @"Admin#@!123";
                    //آردینه
                    //clientCredentials.UserName.UserName = @"ardineh\crmadmin";
                    //clientCredentials.UserName.Password = @"arqwe123!@#";
                    //بهینه چوب
                    //clientCredentials.UserName.UserName = @"behinchoob\administrator";
                    //clientCredentials.UserName.Password = @"Behine2024";
                    //فرزانه آرمان
                    clientCredentials.UserName.UserName = @"fartest\administrator";
                    clientCredentials.UserName.Password = @"FTit@Factory";

                    _serviceProxy = new OrganizationServiceProxy(oUri, null, clientCredentials, null);
                    _serviceProxy.EnableProxyTypes();
                    dbxrm = new XrmServiceContext(_serviceProxy);


                    var Exitcode = HostFactory.Run(x =>
                    {
                        x.Service<BaseTimer>(y =>
                        {
                            y.ConstructUsing(timeRrun => new BaseTimer());
                            y.WhenStarted(timeRrun => timeRrun.Start());
                            y.WhenStopped(timerRun => timerRun.Stop());

                        });
                        x.RunAsLocalSystem();
                        x.SetServiceName("LunaTicketCustomer");
                        x.SetDisplayName("LunaTicketCustomer");
                        x.SetDescription("تیکت از سمت مشتری LunaGroup Copyright © 2024 ");
                    });
                    int Exitcodevalue = (int)Convert.ChangeType(Exitcode, Exitcode.GetTypeCode());
                    Environment.ExitCode = Exitcodevalue;
                    Console.ReadLine();
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
            }


        }

    }
}
