
using Microsoft.Xrm.Sdk.Client;
using System;
using System.ServiceModel.Description;
using Topshelf;
using Xrm;


namespace LunaService
{
    public class Program
    {
        static Uri oUri = new Uri("https://lunagroup.lunagroup.ir:444/XRMServices/2011/Organization.svc");

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
                    clientCredentials.UserName.UserName = @"lunagroup\support";
                    clientCredentials.UserName.Password = @"LunaTech2012";

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
                        x.SetServiceName("LunaTicketServer");
                        x.SetDisplayName("LunaTicketServer");
                        x.SetDescription("ارسال تیکت به سرور لوناLunaGroup Copyright © 2024 ");
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
