
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TK_DomainModel.Model.SendServer;
using TK_DomainModel.Model.SendSupport;
using Tk_Infrastucture_Customer.Model.Note;
using Tk_Infrastucture_Customer.Model.SendSupport;
using Xrm;
using Task = System.Threading.Tasks.Task;
using Timer = System.Timers.Timer;



namespace LunaService
{

    public class BaseTimer
    {
        //public static string idaccount = "3755613e-8e12-ef11-837a-000c2914a0a5";///نیرو فراب
        //public static string idaccount = "d0e25583-5d88-ef11-a790-000c29d20076";//آردینه
        //public static string idaccount = "11180acc-dcf0-ee11-8f3b-000c29104c01";//بهینه چوب
        public static string idaccount = "8ff185e7-d38c-ed11-91c8-000c29d20076";//فرزانه آرمان

        public static Timer _timerticketlistupdate;
        public static Timer _timerpipeAdded;
        public static Timer _timerpipeUpdate;
        public string senderget;
        public string apiget;
        //اسم نخ دریافت تیکت جدید
        public static string pipeAdded = "PipeModelSupportSendAdded";
        //اسم نخ دریافت اپدیت تیکت از سمت مشتری
        public static string pipeUpdate = "PipeModelSupportSendUpdate";


        //تعریف متد خارجی غیر از زبان سی شارپ
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public BaseTimer()
        {
            try
            {
                Console.WriteLine("TimerElapsedticketlistupdate");
                //_timerticketlistupdate = new Timer(900000);
                _timerticketlistupdate = new Timer(600000);
                _timerticketlistupdate.Elapsed += TimerElapsedticketlistupdate;

                Console.WriteLine("TimerElapsedpipeAdded");
                //_timerpipeAdded = new Timer(900000);
                _timerpipeAdded = new Timer(600000);
                _timerpipeAdded.Elapsed += TimerElapsedpipeAdded;


                // دریافت نوت از مشتری
                /* _timerpipeUpdate = new Timer(900000)*/
                _timerpipeUpdate = new Timer(600000);
                _timerpipeUpdate.Elapsed += TimerElapsedpipeUpdate;

            }
            catch
            {


            }

        }
        /// <summary>
        /// تایمر اپدیت تیکت و ارسال به مشتری
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        public static bool isrunnigTimerElapsedticketlistupdat = false;
        public static async void TimerElapsedticketlistupdate(object sender, ElapsedEventArgs e)
        {
            if (isrunnigTimerElapsedticketlistupdat) { return; }
            isrunnigTimerElapsedticketlistupdat = true;
            try
            {
                _timerticketlistupdate.Stop();
                Console.WriteLine("isrunnigTimerElapsedticketlistupdat = true");
                Uri oUri = new Uri("https://lunagroup.lunagroup.ir:444/XRMServices/2011/Organization.svc");
                ClientCredentials clientCredentials = new ClientCredentials();
                clientCredentials.UserName.UserName = @"lunagroup\support";
                clientCredentials.UserName.Password = @"LunaTech2012";
                OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(oUri, null, clientCredentials, null);
                _serviceProxy.EnableProxyTypes();
                XrmServiceContext dbxrm = new XrmServiceContext(_serviceProxy);

                List<ModelReciveId> lunset = new List<ModelReciveId>();
                ModelReciveId change;
                //var ticketlistupdate = Program.dbxrm.IncidentSet.Where(x => x.new_CheckUpdate == true && x.new_IdFactory == "1EA44246-A895-ED11-91CF-000C29D20076" && x.TicketNumber != null).ToList();
                //"CAS-01324-K8S0C8"
                var ticketlistupdate = dbxrm.IncidentSet.Where(x => x.new_CheckUpdate == true && x.new_IdFactory == idaccount && x.TicketNumber != null).ToList();
                //var ticketlistupdate = Program.dbxrm.IncidentSet.Where(x => x.new_CheckUpdate == true && x.new_IdFactory == idaccount && x.TicketNumber == "CAS-01324-K8S0C8").ToList();
                if (ticketlistupdate.Count > 0)
                {

                    List<ModelSendUpdateAzLuna> listupdate = new List<ModelSendUpdateAzLuna>();
                    foreach (var item in ticketlistupdate)
                    {
                        List<ModelNote> listnot = new List<ModelNote>();

                        var statusint = 100000002;
                        if (item.StatusCode != null)
                        {
                            if (item.StatusCode.Value == 1) { statusint = 100000007; }
                            else if (item.StatusCode.Value == 100000000) { statusint = 100000006; }
                            else if (item.StatusCode.Value == 4) { statusint = 100000005; }
                            else if (item.StatusCode.Value == 3) { statusint = 100000008; }
                            else if (item.StatusCode.Value == 5) { statusint = 100000003; }
                            else if (item.StatusCode.Value == 6) { statusint = 100000004; }

                        }

                        var not = dbxrm.AnnotationSet.Where(x => x.ObjectId.Id == item.Id).ToList();
                        if (not != null)
                        {
                            if (not.Count > 0)
                            {
                                foreach (var no in not)
                                {
                                    ModelNote modelnote = new ModelNote()
                                    {
                                        name = no.ModifiedBy.Name,
                                        subject = no.Subject,
                                        notetext = no.NoteText,

                                    };
                                    listnot.Add(modelnote);

                                }
                            }
                        }


                        var usersystem = dbxrm.SystemUserSet.Where(x => x.Id == item.OwningUser.Id).FirstOrDefault();
                        if (item.new_estimateddeliverydate != null && item.new_EstimateTheRequiredTime != null)
                        {
                            ModelSendUpdateAzLuna modelSendUpdateAzLuna = new ModelSendUpdateAzLuna()
                            {

                                statuscode = statusint,
                                estimatetherequiredtime = (int)item.new_EstimateTheRequiredTime,
                                estimateddeliverydate = (DateTime)item.new_estimateddeliverydate,
                                numberticket = item.TicketNumber,
                                nots = listnot,
                                billableterms = item.new_BillableTerms,
                                statecode = item.StateCode?.ToString(),
                                emailUser = usersystem.InternalEMailAddress,
                                nameUser = usersystem.FullName,


                            };
                            listupdate.Add(modelSendUpdateAzLuna);
                        }
                        else
                        {
                            ModelSendUpdateAzLuna modelSendUpdateAzLuna = new ModelSendUpdateAzLuna()
                            {

                                statuscode = statusint,
                                numberticket = item.TicketNumber,
                                nots = listnot,
                                billableterms = item.new_BillableTerms,
                                statecode = item.StateCode?.ToString(),
                                emailUser = usersystem.InternalEMailAddress,
                                nameUser = usersystem.FullName,
                            };
                            listupdate.Add(modelSendUpdateAzLuna);
                        }



                        change = new ModelReciveId()
                        {
                            id = item.Id,
                            numberticket = item.TicketNumber,
                            state = item.StateCode?.ToString(),
                        };
                        lunset.Add(change);


                    }

                    while (!await updatePipeServerAsync("PipeModelReciveRecordUpdate", listupdate))
                    {
                        await updatePipeServerAsync("PipeModelReciveRecordUpdate", listupdate);
                    }

                    if (lunset.Count > 0)
                    {
                        foreach (var index in lunset)
                        {
                            if (index.state == "Active")
                            {
                                var x = dbxrm.IncidentSet.FirstOrDefault(y => y.Id == index.id);
                                //x.new_CheckUpdate = false;
                                _serviceProxy.Update(new Incident { Id = x.Id, new_CheckUpdate = false });
                                //Program.dbxrm.UpdateObject(x);

                            }
                            dbxrm.SaveChanges();

                        }


                    }

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            finally
            {
                Console.WriteLine("   isrunnigTimerElapsedticketlistupdat = false");
                isrunnigTimerElapsedticketlistupdat = false;
                _timerticketlistupdate.Start();
            }


        }


        /// <summary>
        /// تایمر دریافت تیکت جدید از مشتری
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static bool isrunnigTimerElapsedpipeAdded = false;
        public static async void TimerElapsedpipeAdded(object sender, ElapsedEventArgs e)
        {
            if (isrunnigTimerElapsedpipeAdded) { return; }
            isrunnigTimerElapsedpipeAdded = true;
            try
            {
                _timerpipeAdded.Stop();
                Console.WriteLine("isrunnigTimerElapsedpipeAdded = true");

                if (IsConnectedToInternet())
                {
                    ///اینجا در برگشت مشخص میشود باید در لونا ایجاد شود یا اپدیت شود

                    (object, string) result = await GetDataFromPippipadd(pipeAdded);
                    if (result.Item2 == "addretrunid")
                    {
                        ///متد ایجاد
                        Dictionary<Guid, string> listid = ProcessReceivedDataAdded(result.Item1);

                        if (listid != null)
                            SendNumberTicketToCustomer(listid);

                    }

                    else if (result.Item2 == "")
                    {
                        ///
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            finally
            {
                Console.WriteLine("   isrunnigTimerElapsedpipeAdded = false");
                isrunnigTimerElapsedpipeAdded = false;
                _timerpipeAdded.Start();
            }


        }

        /// <summary>
        /// تایمر دریافت نوت اپدیت از مشتری 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static bool isrunnigTimerElapsedpipeUpdate = false;
        public async void TimerElapsedpipeUpdate(object sender, ElapsedEventArgs e)
        {

            if (isrunnigTimerElapsedpipeUpdate) { return; }
            isrunnigTimerElapsedpipeUpdate = true;
            _timerpipeUpdate.Stop();
            try
            {

                if (IsConnectedToInternet())
                {
                    ///اینجا در برگشت مشخص میشود باید در لونا ایجاد شود یا اپدیت شود
                    (object, string) result = await GetDataFromPipesapipupdate(pipeUpdate);

                    if (result.Item2 == "updatenot")
                    {

                        //متد اپدیت
                        await ReciveCustomerUpdateAndSetToLuna(result.Item1);


                    }
                    else if (result.Item2 == "")
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            finally
            {
                isrunnigTimerElapsedpipeUpdate = false;
                _timerpipeUpdate.Start();
            }


        }

        //چک کردن اتصال به اینترنت
        private static bool IsConnectedToInternet()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }


        /// <summary>
        /// ثبت نوت های امده از مشتری در تیکت سمت لونا
        /// </summary>
        /// <param name="result"></param>
        public static bool Isrunnig_ReciveCustomerUpdateAndSetToLuna = false;
        private static async Task ReciveCustomerUpdateAndSetToLuna(object result)
        {
            if (Isrunnig_ReciveCustomerUpdateAndSetToLuna) { return; }
            Isrunnig_ReciveCustomerUpdateAndSetToLuna = true;
            try
            {

                Uri oUri = new Uri("https://lunagroup.lunagroup.ir:444/XRMServices/2011/Organization.svc");
                ClientCredentials clientCredentials = new ClientCredentials();
                clientCredentials.UserName.UserName = @"lunagroup\support";
                clientCredentials.UserName.Password = @"LunaTech2012";
                OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(oUri, null, clientCredentials, null);
                _serviceProxy.EnableProxyTypes();
                XrmServiceContext dbxrm = new XrmServiceContext(_serviceProxy);

                List<ModelNote> listnote = new List<ModelNote>();
                ModelNote note;
                List<ModelSupportSendUpdate> convertobject = ((IEnumerable)result).Cast<ModelSupportSendUpdate>().ToList();


                if (convertobject.Count > 0)
                {

                    foreach (var item in convertobject)
                    {

                        //var searchid = dbxrm.IncidentSet.FirstOrDefault(x => x.TicketNumber == item.coderahgiry && x.new_IdFactory == "1EA44246-A895-ED11-91CF-000C29D20076");
                        //var searchid = dbxrm.IncidentSet.FirstOrDefault(x => x.TicketNumber == item.coderahgiry && x.new_IdFactory == idaccount);
                        var searchid = dbxrm.IncidentSet.FirstOrDefault(x => x.TicketNumber == item.coderahgiry && x.new_IdFactory.Equals(idaccount));
                        if (searchid != null)
                        {
                            if (!string.IsNullOrEmpty(searchid.new_IdTicketCustomer) && !string.IsNullOrEmpty(searchid.new_FactoryCustomer))
                            {
                                searchid.new_IdTicketCustomer = item.getId;
                                searchid.new_FactoryCustomer = item.entityname;
                                dbxrm.UpdateObject(searchid, true);
                            }


                            foreach (var no in item.notes)
                            {
                                var query = dbxrm.AnnotationSet.FirstOrDefault(x => x.NoteText == no.notetext && x.ObjectId == searchid.ToEntityReference());
                                if (query == null)
                                {
                                    Annotation annotation = new Annotation()
                                    {

                                        ObjectId = searchid.ToEntityReference(),
                                        NoteText = no.notetext,
                                        Subject = $":کاربر {no.name} گفت \n{no.subject}",
                                    };
                                    _serviceProxy.Create(annotation);

                                }

                            }


                        }

                    }
                    dbxrm.SaveChanges();

                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Isrunnig_ReciveCustomerUpdateAndSetToLuna = false;
            }
        }

        /// <summary>
        ///بعد از ایجاد تیکت در این متد برای برگشت کدهریگیری های تولید شده به مشتری اجرا خواهد شد
        /// </summary>
        /// <param name="result"></param>
        private static async void SendNumberTicketToCustomer(Dictionary<Guid, string> result)
        {
            try
            {
                _timerticketlistupdate.Stop();
                Uri oUri = new Uri("https://lunagroup.lunagroup.ir:444/XRMServices/2011/Organization.svc");
                ClientCredentials clientCredentials = new ClientCredentials();
                clientCredentials.UserName.UserName = @"lunagroup\support";
                clientCredentials.UserName.Password = @"LunaTech2012";
                OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(oUri, null, clientCredentials, null);
                _serviceProxy.EnableProxyTypes();
                XrmServiceContext dbxrm = new XrmServiceContext(_serviceProxy);

                if (result != null)
                {
                    List<ModelReciveId> lunset = new List<ModelReciveId>();
                    ModelReciveId change;

                    List<ModelReciveId> listmodel = new List<ModelReciveId>();

                    foreach (var item in result)
                    {
                        var mdoel = new ModelReciveId()
                        {
                            id = item.Key,
                            numberticket = item.Value,

                        };
                        listmodel.Add(mdoel);
                        change = new ModelReciveId()
                        {
                            numberticket = item.Value,
                        };
                        lunset.Add(change);
                    }

                    while (!await CreatePipeServerAsync("PipeModelReciveIDSendToCustomer", listmodel))
                    {
                        await CreatePipeServerAsync("PipeModelReciveIDSendToCustomer", listmodel);
                    }

                    if (lunset.Count > 0)
                    {
                        foreach (var item in lunset)
                        {
                            var updateentity = dbxrm.IncidentSet.FirstOrDefault(x => x.TicketNumber == item.numberticket);

                            _serviceProxy.Update(new Incident { Id = updateentity.Id, new_CheckUpdate = false });
                            dbxrm.SaveChanges();
                        }


                    }



                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _timerticketlistupdate.Start();
            }
        }

        /// <summary>
        /// ایجاد نخ ویندوز
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="dataToSend"></param>
        /// <returns></returns>
        public static NamedPipeServerStream pipeServerClinett;
        public static async Task<bool> CreatePipeServerAsync(string pipeName, object dataToSend)
        {
            try
            {
                //using (pipeServerClinet = new NamedPipeServerStream(pipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                using (pipeServerClinett = new NamedPipeServerStream(pipeName, PipeDirection.Out))
                {

                    Console.WriteLine("Waiting for client connection...");
                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(700)); // زمانبندی 30 ثانیه
                    await pipeServerClinett.WaitForConnectionAsync(cts.Token);
                    //await Task.Factory.FromAsync(pipeServerClinet.BeginWaitForConnection, pipeServerClinet.EndWaitForConnection, null);
                    Console.WriteLine("Client connected.");

                    BinaryFormatter formatterAdded = new BinaryFormatter();
                    {
                        formatterAdded.Serialize(pipeServerClinett, dataToSend);

                        return true;
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n");
                pipeServerClinett.Disconnect();
                pipeServerClinett.Dispose();
                return false;
            }



        }

        /// <summary>
        /// ایجاد خط لوله و ارسال دیتا داخلش برای دریافت از سمت مشتری
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="dataToSend"></param>
        /// <returns></returns>
        public static NamedPipeServerStream pipeServerClinet;
        public static async Task<bool> updatePipeServerAsync(string pipeName, object dataToSend)
        {

            try
            {

                //using (pipeServerClinet = new NamedPipeServerStream(pipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                using (pipeServerClinet = new NamedPipeServerStream(pipeName, PipeDirection.Out))
                {
                    Console.WriteLine("Waiting for client connection...");
                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(700)); // زمانبندی 30 ثانیه
                    await pipeServerClinet.WaitForConnectionAsync(cts.Token);
                    //await Task.Factory.FromAsync(pipeServerClinet.BeginWaitForConnection, pipeServerClinet.EndWaitForConnection, null);
                    Console.WriteLine("Client connected.");

                    BinaryFormatter formatterAdded = new BinaryFormatter();
                    {
                        formatterAdded.Serialize(pipeServerClinet, dataToSend);

                        return true;
                    }


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n");
                pipeServerClinet.Disconnect();
                pipeServerClinet.Dispose();
                return false;
            }

        }

        //تکمیل
        //دریافت از ویندوز سرویس مشتری
        // دو متد  چک کردن خط لوله و در صورت پر بودن دریافت دیتا(Object)
        /// <summary>
        /// چک کردن خط لوله و در صورت بودن دریافت ابجکت ایجاد در داخل لوله
        /// </summary>
        /// <param name="pipeName1"></param>
        /// <param name="pipeName2"></param>
        /// <returns></returns>
        public static NamedPipeClientStream pipeClient;
        public static async Task<(object, string)> GetDataFromPippipadd(string pipadd)
        {
            try
            {
                using (pipeClient = new NamedPipeClientStream(".", pipadd, PipeDirection.In))
                {

                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(600)); // زمانبندی 30 ثانیه
                    await pipeClient.ConnectAsync(cts.Token);
                    if (pipeClient.IsConnected == true)
                    {

                        var objectFromPipe1 = ReadDataFromPipe(pipeClient);
                        // بعد از خواندن داده، لوله باید بسته شود
                        pipeClient.Dispose();
                        // بعد از خواندن داده، لوله باید بسته شود

                        return (objectFromPipe1, "addretrunid");
                    }
                    else
                    {
                        pipeClient.Dispose();
                    }


                }
                return (null, "");
            }
            catch
            {
                pipeClient?.Dispose();
                return (null, "");
            }




        }


        /// <summary>
        /// چک کردن خط لوله و در صورت بودن دریافت ابجکت اپدیت در داخل لوله
        /// </summary>
        /// <param name="pipeName1"></param>
        /// <param name="pipeName2"></param>
        /// <returns></returns>
        public static NamedPipeClientStream pipeClient2;
        public async Task<(object, string)> GetDataFromPipesapipupdate(string pipupdate)
        {

            try
            {
                using (pipeClient2 = new NamedPipeClientStream(".", pipupdate, PipeDirection.In))
                {

                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(600)); // زمانبندی 10 ثانیه
                    await pipeClient2?.ConnectAsync(cts.Token);
                    if (pipeClient2?.IsConnected == true)
                    {
                        var objectFromPipe2 = ReadDataFromPipe(pipeClient2);
                        // بعد از خواندن داده، لوله باید بسته شود
                        pipeClient2.Dispose();
                        return (objectFromPipe2, "updatenot");
                    }
                    else
                    {
                        pipeClient2.Dispose();
                    }

                }


                return (null, "");
            }
            catch
            {
                pipeClient2.Dispose();
                return (null, "");
            }



        }


        /// <summary>
        ///در صورت پر بودن خط لوله این متد برای دریافت دیتای خط لوله اجرا میشود ابجکت ریترن یراس متد GetDataFormPipes
        /// </summary>
        /// <param name="pipeClient"></param>
        /// <returns>object</returns>
        private static object ReadDataFromPipe(NamedPipeClientStream pipeClient)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            if (pipeClient != null && pipeClient.IsConnected && pipeClient.CanRead)
            {

                return formatter.Deserialize(pipeClient);

            }
            {
                // انجام عملیات مناسب در صورتی که داده برای خواندن موجود نباشد
                return null;
            }
        }


        /// <summary>
        /// ایجاد تیکت جدید و برگشت ایدی تیکت
        /// </summary>
        /// <param name="objectFromPipe1"></param>
        /// <returns></returns>
        private static Dictionary<Guid, string> ProcessReceivedDataAdded(object objectFromPipe1)
        {
            try
            {
                Uri oUri = new Uri("https://lunagroup.lunagroup.ir:444/XRMServices/2011/Organization.svc");

                ClientCredentials clientCredentials = new ClientCredentials();
                clientCredentials.UserName.UserName = @"lunagroup\support";
                clientCredentials.UserName.Password = @"LunaTech2012";
                OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(oUri, null, clientCredentials, null);
                _serviceProxy.EnableProxyTypes();
                XrmServiceContext dbxrm = new XrmServiceContext(_serviceProxy);

                Guid guidid;
                Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();

                //دریافت ایدی فیلد سابجکت مقدار   "خدمات"2
                Guid subjectbase = new Guid("2B92B9A2-05A2-ED11-91CF-000C29D20076");
                List<ModelSupportSendAdd> convertobject = ((IEnumerable)objectFromPipe1).Cast<ModelSupportSendAdd>().ToList();

                //دریافت نام شرکت account
                Guid accountid = new Guid(idaccount);//ایدی باید برای هر شرکت مخصوص باشد و دستی تغییر کند
                var customerid = dbxrm.AccountSet.FirstOrDefault(x => x.Id == accountid);

                //تکرار برای ایجاد تیکت
                foreach (var item in convertobject)
                {
                    var serachcontact = dbxrm.ContactSet.FirstOrDefault(x => x.MobilePhone == item.mobilephone);

                    if (serachcontact == null)
                    {
                        // ایجاد مخاطب در صورتی که با این شماره موبایل صبت تیکت در لونا موجود نباشد
                        Contact contact = new Contact();
                        contact.MobilePhone = item.mobilephone;
                        contact.new_Khatab = new OptionSetValue(100000000);
                        contact.LastName = item.ownername;
                        contact.ParentCustomerId = customerid.ToEntityReference();
                        _serviceProxy.Create(contact);


                    }
                    else if (serachcontact != null && serachcontact.ParentCustomerId != customerid.ToEntityReference())
                    {
                        _serviceProxy.Update(new Contact { Id = serachcontact.Id, ParentCustomerId = customerid.ToEntityReference() });

                    }


                    //اینجا حتما مخاطبی در لونا یافت میشود
                    var findcontact = dbxrm.ContactSet.FirstOrDefault(x => x.MobilePhone == item.mobilephone);

                    Guid subjectid = new Guid("2B92B9A2-05A2-ED11-91CF-000C29D20076");
                    var subject = dbxrm.SubjectSet.FirstOrDefault(x => x.Id == subjectid).ToEntityReference();



                    ///اکانت همیشه موجود است و ایدی منحصر به فرد
                    if (findcontact != null && subject != null)
                    {

                        Incident incident = new Incident()
                        {

                            Title = item.title,
                            SubjectId = subject,
                            CaseOriginCode = new OptionSetValue(8),
                            CaseTypeCode = new OptionSetValue(item.type),
                            CustomerId = customerid.ToEntityReference(),
                            PrimaryContactId = findcontact.ToEntityReference(),
                            new_CheckUpdate = false,
                            FollowupBy = DateTime.Now,
                            new_IdFactory = idaccount,//این ایتم ایدی برای هر سازمان مختلف است و باید دستی تغییر کند 
                            Description = item.description,

                        };
                        guidid = Program._serviceProxy.Create(incident);


                        var ticket = dbxrm.IncidentSet.FirstOrDefault(z => z.Id == guidid).TicketNumber;
                        dictionary.Add(item.supportid, ticket);


                    }

                    Program.dbxrm.SaveChanges();

                }


                return dictionary;
            }
            catch
            {
                return null;

            }

        }


        //finish
        public void Start()
        {
            _timerticketlistupdate.Start();
            _timerpipeAdded.Start();
            _timerpipeUpdate.Start();
        }

        public void Stop()
        {
            _timerticketlistupdate.Stop();
            _timerpipeAdded.Stop();
            _timerpipeUpdate.Stop();
        }



    }


}
