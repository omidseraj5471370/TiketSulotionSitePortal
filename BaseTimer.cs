using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TK_DomainModel.Model.SendServer;
using TK_DomainModel.Model.SendSupport;
using Tk_Infrastucture_Customer.Model.Note;
using Tk_Infrastucture_Customer.Model.SendSupport;
using Xrm;
using static System.Net.Mime.MediaTypeNames;
using Task = System.Threading.Tasks.Task;
using Timer = System.Timers.Timer;


namespace CustomerService
{
    public class BaseTimer
    {
        //ایدی مشتری دریافتی از سرور لونا فرم اکانت مخصوص شرکت یا سازمان
        //public static string idaccount = "3755613e-8e12-ef11-837a-000c2914a0a5";//نیروفراب
        //public static string idaccount = "d0e25583-5d88-ef11-a790-000c29d20076";//آردینه
        //public static string idaccount = "11180acc-dcf0-ee11-8f3b-000c29104c01";//بیهینه چوب
        public static string idaccount = "8ff185e7-d38c-ed11-91c8-000c29d20076";//فرزانه آرمان

        //timer8*9
        public static Timer _timerSendAddedToServiceLuna;
        public static Timer _timerSendUpdateToServiceLuna;
        public static Timer _timerpipereciveid;
        public static Timer _timerpipeUpdate;
        public string senderget;
        public string apiget;

        /// <summary>
        /// نام خط لوله ها از سمت سرویس لونا
        /// </summary>
        public static string pipereciveid = "PipeModelReciveIDSendToCustomer";
        public static string pipeUpdate = "PipeModelReciveRecordUpdate";

        //نیروفراب
        //public static string username = @"farab\it";
        //public static string password = @"Admin#@!123";
        //public static string uri = "https://niroofarab.niroofarabgroup.com:444/XRMServices/2011/Organization.svc";
        ////آردینه
        //public static string username = @"ardineh\crmadmin";
        //public static string password = @"arqwe123!@#";
        //public static string uri = "https://crm.ardineh.com/XRMServices/2011/Organization.svc";
        //بهینه چوب
        //public static string username = @"behinchoob\administrator";
        //public static string password = @"Behine2024";
        //public static string uri = "http://dynamics/BehineChoob/XRMServices/2011/Organization.svc";
        //فرزانه آرمان
        public static string username = @"fartest\administrator";
        public static string password = @"FTit@Factory";
        public static string uri = "http://crm2/FarTest/XRMServices/2011/Organization.svc";


        //تعریف متد خارجی غیر از زبان سی شارپ
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        /// <summary>
        /// کانترکستور
        /// </summary>
        public BaseTimer()
        {
            try
            {

                //_timerpipeUpdate = new Timer(900000) { AutoReset = false };
                _timerpipeUpdate = new Timer(600000) { AutoReset = false };
                _timerpipeUpdate.Elapsed += TimerElapsedpipeUpdate;

                // _timerSendAddedToServiceLuna = new Timer(900000) { AutoReset = false };
                _timerSendAddedToServiceLuna = new Timer(600000) { AutoReset = false };
                _timerSendAddedToServiceLuna.Elapsed += TimerElapsedSendAddedToServiceLuna;

                // تکمیل ارسال نوت از مشتری به لونا
                //_timerSendAddedToServiceLuna = new Timer(900000) { AutoReset = false };
                _timerSendUpdateToServiceLuna = new Timer(600000) { AutoReset = false };
                _timerSendUpdateToServiceLuna.Elapsed += TimerElapsedSendUpdateToServiceLuna;


                //_timerpipereciveid = new Timer(900000) { AutoReset = false };
                _timerpipereciveid = new Timer(600000) { AutoReset = false };
                _timerpipereciveid.Elapsed += TimerElapsedpipereciveid;

            }
            catch
            {

            }

        }

        /// <summary>
        /// تایمر ارسال تیکت جدید به لونا
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static bool IsRunnig_TimerElapsedSendAddedToServiceLuna = false;
        public static async void TimerElapsedSendAddedToServiceLuna(object sender, ElapsedEventArgs e)
        {
            if (IsRunnig_TimerElapsedSendAddedToServiceLuna) { return; }
            IsRunnig_TimerElapsedSendAddedToServiceLuna = true;
            try
            {
                _timerSendAddedToServiceLuna.Stop();
                Uri oUri = new Uri(uri);
                ClientCredentials clientCredentials = new ClientCredentials();
                clientCredentials.UserName.UserName = username;
                clientCredentials.UserName.Password = password;
                OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(oUri, null, clientCredentials, null);
                _serviceProxy.EnableProxyTypes();
                XrmServiceContext dbxrm = new XrmServiceContext(_serviceProxy);

                /// رکورد هایی که کد رهگیری ندارند و باید در سرور لونا ایجاد شوند
                var model_create = dbxrm.new_LunaTechSupportSet.Where(x => x.new_UpdateCheck == true && x.new_TrackingCode == null).ToList();


                /// شرط خالی نبودن برای رفتن به عملیات ایجاد رکورد
                if (model_create.Count > 0)
                {
                    SendAddedToServiceLuna(model_create);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            finally
            {
                IsRunnig_TimerElapsedSendAddedToServiceLuna = false;
                _timerSendAddedToServiceLuna.Start();
            }


        }


        /// <summary>
        /// تایمر ارسال نوت ایجاد شده در تیکت به لونا
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static bool IsRunnig_TimerElapsedSendUpdateToServiceLuna = false;
        public static async void TimerElapsedSendUpdateToServiceLuna(object sender, ElapsedEventArgs e)
        {
            if (IsRunnig_TimerElapsedSendUpdateToServiceLuna) { return; }
            IsRunnig_TimerElapsedSendUpdateToServiceLuna = true;
            try
            {
                _timerSendUpdateToServiceLuna.Stop();
                Uri oUri = new Uri(uri);
                ClientCredentials clientCredentials = new ClientCredentials();
                clientCredentials.UserName.UserName = username;
                clientCredentials.UserName.Password = password;
                OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(oUri, null, clientCredentials, null);
                _serviceProxy.EnableProxyTypes();
                XrmServiceContext dbxrm = new XrmServiceContext(_serviceProxy);
                //چک کردن رکورد هایی که دارای کد رهگیری هستند یعنی قبلا ثبت شدند و فقط باید اپدیت انها چک شود
                var model_update = dbxrm.new_LunaTechSupportSet.Where(x => x.new_UpdateCheck == true && x.new_TrackingCode != null && x.new_Checkopen != false).ToList();


                if (model_update != null)
                {
                    if (model_update.Count > 0)
                    {
                        ///ارسال آپدیت ها از سمت مشتری
                      await  SendUpdateToServiceLuna(model_update);
                    }     ///شرط خالی نبودن برای رفتن به عملیات اپدیت رکوردها

                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            finally
            {
                IsRunnig_TimerElapsedSendUpdateToServiceLuna = false;
                _timerSendUpdateToServiceLuna.Start();
            }



        }

        /// <summary>
        /// تایمر دریافت شماره تیکت ایجاد شده در مشتری از لونا
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static bool IsRunnig_TimerElapsedpipereciveid = false;
        public async void TimerElapsedpipereciveid(object sender, ElapsedEventArgs e)
        {
            if (IsRunnig_TimerElapsedpipereciveid) { return; }
            IsRunnig_TimerElapsedpipereciveid = true;
            try
            {

                _timerpipereciveid.Stop();
                Uri oUri = new Uri(uri);
                ClientCredentials clientCredentials = new ClientCredentials();
                clientCredentials.UserName.UserName = username;
                clientCredentials.UserName.Password = password;
                OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(oUri, null, clientCredentials, null);
                _serviceProxy.EnableProxyTypes();
                XrmServiceContext dbxrm = new XrmServiceContext(_serviceProxy);

                if (IsConnectedToInternet())
                {
                    (object, string) result = await GetDataFromPipespipereciveid(pipereciveid);
                    if (result.Item2 == "ReciveId")
                    {
                        ///متد ایجاد
                        ///

                        if (result.Item2 != null)
                        {
                            var request = CreateNumberTicket(result.Item1);
                        }

                        //if (request == true)
                        //{
                        //    //عملیات بعد از اپدیت کد رهیگری تیکت
                        //}

                    }

                    else if (result.Item2 == "")
                    {
                        /////
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            finally { IsRunnig_TimerElapsedpipereciveid = false; _timerpipereciveid.Start(); }


        }
        public static bool IsRunnig_TimerElapsedpipeUpdate = false;
        /// <summary>
        /// تایمر دریافت تیکت اپدیت 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void TimerElapsedpipeUpdate(object sender, ElapsedEventArgs e)
        {
            if (IsRunnig_TimerElapsedpipeUpdate) { return; }
            IsRunnig_TimerElapsedpipeUpdate = true;
            try
            
            {
                _timerpipeUpdate.Stop();
                Uri oUri = new Uri(uri);
                ClientCredentials clientCredentials = new ClientCredentials();
                clientCredentials.UserName.UserName = username;
                clientCredentials.UserName.Password = password;
                OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(oUri, null, clientCredentials, null);
                _serviceProxy.EnableProxyTypes();
                XrmServiceContext dbxrm = new XrmServiceContext(_serviceProxy);
                if (IsConnectedToInternet())
                {
                    (object, string) result = await GetDataFromPipespipeUpdate(pipeUpdate);

                    if (result.Item2 == "updateserver")
                    {
                        //متد اپدیت
                        if (result.Item2 != null)
                        {
                            await UpdateTicket(result.Item1);
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
                IsRunnig_TimerElapsedpipeUpdate = false;
                _timerpipeUpdate.Start();
            }

        }


        //چک کردن اتصال به اینترنت
        private static bool IsConnectedToInternet()
        {
            try
            {
                int description;
                return InternetGetConnectedState(out description, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;

            }

        }

        //Finish
        /// <summary>
        /// رکورد اپدیت شده
        /// </summary>
        /// <param name="result"></param>
        public static bool IsRuunig_SendUpdateToServiceLuna = false;
        private static async Task SendUpdateToServiceLuna(List<Xrm.new_LunaTechSupport> result)
        {

            if (IsRuunig_SendUpdateToServiceLuna) { return; }
            IsRuunig_SendUpdateToServiceLuna = true;
            try
            {

                if (result != null)
                {
                    List<ModelChangeFieldUpdate> lunset = new List<ModelChangeFieldUpdate>();
                    ModelChangeFieldUpdate change;

                    List<ModelSupportSendUpdate> listmodel = new List<ModelSupportSendUpdate>();
                    List<ModelNote> listnote;
                    foreach (var item in result)
                    {
                        listnote = new List<ModelNote>();
                        ////باز بودن تیکت 
                        if (((int?)item.statecode) != 100000004 && ((int?)item.statecode) != 100000003)
                        {
                            var annotaionnote = Program.dbxrm.AnnotationSet.Where(x => x.ObjectId.Id == item.Id).ToList();

                            if (annotaionnote.Count > 0)
                            {
                                
                                foreach (var no in annotaionnote)
                                {
                                   
                                    ModelNote note = new ModelNote
                                    {
                                        subject = no.Subject,
                                        notetext = no.NoteText,
                                        name = no.ModifiedBy.Name,
                                    };

                                    listnote.Add(note);

                                }
                            }


                            ModelSupportSendUpdate mdoel = new ModelSupportSendUpdate()
                            {
                                coderahgiry = item.new_TrackingCode,
                                supportid = item.Id,
                                //idfactory = "1EA44246-A895-ED11-91CF-000C29D20076",
                                idfactory = idaccount,
                                notes = listnote,
                                entityname = item.LogicalName.ToString()??null,
                                getId = item.Id.ToString()??null,


                            };

                            listmodel.Add(mdoel);
                            change = new ModelChangeFieldUpdate()
                            {
                                id = item.Id,
                            };
                            lunset.Add(change);


                        }

                    }
                    //while (!await UpdatePipeServerAsync("PipeModelSupportSendUpdate", listmodel))
                    //{
                    //    await UpdatePipeServerAsync("PipeModelSupportSendUpdate", listmodel);
                    //}

                    var resultCall = await CallUpdatePipeServerAsync("PipeModelSupportSendUpdate", listmodel);

                    if (resultCall)
                    {
                        if (lunset.Count > 0)
                        {
                            foreach (var i in lunset)
                            {
                                Program._serviceProxy.Update(new new_LunaTechSupport { Id = i.id, new_UpdateCheck = false });

                            }
                            Program.dbxrm.SaveChanges();

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
                IsRuunig_SendUpdateToServiceLuna = false;

            }


        }

        public static async Task<bool> CallUpdatePipeServerAsync(string pipeName, object dataToSend)
        {
            while (!await UpdatePipeServerAsync(pipeName, dataToSend))
            {
                Console.WriteLine("Retrying...");
                await Task.Delay(1000); // تاخیر 1 ثانیهای قبل از تلاش مجدد
            }

            return true;
        }
        public static async Task<bool> CallAddedPipeServerAsync(string pipeName, object dataToSend)
        {
            while (!await CreatePipeServerAsync(pipeName, dataToSend))
            {
                Console.WriteLine("Retrying...");
                await Task.Delay(1000); // تاخیر 1 ثانیهای قبل از تلاش مجدد
            }

            return true;
        }


        //finish
        /// <summary>
        /// ارسال شده اپدیت تیکت از لونا داخل این متد برای اپدیت سمت مشتری
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>

        public static bool Isrunnig_UpdateTicket = false;
        private static async Task UpdateTicket(object item)
        {
            if (Isrunnig_UpdateTicket) { return; }
            Isrunnig_UpdateTicket = true;

            try
            {

                List<ModelSendUpdateAzLuna> convertobject = ((IEnumerable)item).Cast<ModelSendUpdateAzLuna>().ToList();

                if (convertobject.Count > 0)
                {
                    foreach (var s in convertobject)
                    {

                        var support = Program.dbxrm.new_LunaTechSupportSet.FirstOrDefault(z => z.new_TrackingCode
                        == s.numberticket);
                        if (support != null)
                        {

                            if (support.new_Checkopen != false)
                            {

                                new_LunaTechSupport model;

                                DateTime? estimateddeliverydate;

                                if (s.statecode != "Active")
                                {
                                    int statudeint = 100000003;
                                    if (s.statuscode == 5 || s.statuscode == 1000) { statudeint = 100000003; }
                                    else { statudeint = 100000004; }
                                    model = new new_LunaTechSupport()
                                    {
                                        Id = support.Id,
                                        statecode = new_LunaTechSupportState.Inactive,
                                        new_TimeSpent = s.billableterms,
                                        statuscode = new OptionSetValue(statudeint),
                                        new_EstimatedDeliveryDate = s.estimateddeliverydate,
                                        new_UpdateCheck = false,
                                        new_PredictingTheTime = s.estimatetherequiredtime,
                                        new_Checkopen = false,
                                        new_SupportUserEmail = s.emailUser,
                                        new_SupportUser = s.nameUser,

                                    };
                                    Program._serviceProxy.Update(model);
                                }

                                else
                                {

                                    model = new new_LunaTechSupport()
                                    {
                                        Id = support.Id,
                                        statecode = new_LunaTechSupportState.Active,
                                        statuscode = new OptionSetValue(s.statuscode),
                                        new_EstimatedDeliveryDate = s.estimateddeliverydate,
                                        new_UpdateCheck = false,
                                        new_PredictingTheTime = s.estimatetherequiredtime,
                                        new_SupportUserEmail = s.emailUser,
                                        new_SupportUser = s.nameUser,
                                    };
                                    Program._serviceProxy.Update(model);
                                }

                                Program.dbxrm.SaveChanges();
                                if (support != null)
                                {

                                    foreach (var no in s.nots)
                                    {
                                        var result = Program.dbxrm.AnnotationSet.FirstOrDefault(x => x.NoteText == no.notetext && x.ObjectId == support.ToEntityReference());
                                        if (result == null)
                                        {
                                            Annotation annotation = new Annotation()
                                            {

                                                ObjectId = support.ToEntityReference(),
                                                NoteText = no.notetext,
                                                Subject = $": کاربر پشتیبانی {no.name} گفت \n{no.subject}",
                                            };
                                            Program._serviceProxy.Create(annotation);
                                            Program.dbxrm.SaveChanges();
                                        }

                                    }

                                }

                                new_LunaTechSupport entitylast = (new new_LunaTechSupport { Id = support.Id });
                                Program._serviceProxy.Update(entitylast);
                                Program.dbxrm.SaveChanges();
                            }
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
                Isrunnig_UpdateTicket = false;
            }

        }


        //finish
        /// <summary>
        /// دریافت شماره تیکت از لونا و تغییر وضعیت تیکت مشتری به ارسال شده
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool CreateNumberTicket(object item)
        {
            try
            {
                List<ModelReciveId> convertobject = ((IEnumerable)item).Cast<ModelReciveId>().ToList();

                if (convertobject.Count > 0)
                {
                    foreach (var y in convertobject)
                    {
                        new_LunaTechSupport lunasupport = Program.dbxrm.new_LunaTechSupportSet.FirstOrDefault(x =>
                        x.Id == y.id);

                        if (lunasupport != null && lunasupport.new_TrackingCode == null)
                        {
                            Program._serviceProxy.Update(new new_LunaTechSupport
                            {
                                Id = lunasupport.Id,
                                new_TrackingCode = y.numberticket,
                                new_UpdateCheck = false,
                                statuscode = new OptionSetValue(100000002),
                            });

                        }

                    }
                    Program.dbxrm.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }


        //finish
        /// <summary>
        /// رکورد ایجاد شده
        /// </summary>
        /// <param name="result"></param>
        private static async void SendAddedToServiceLuna(List<Xrm.new_LunaTechSupport> result)
        {
            try
            {

                if (result != null)
                {

                    List<ModelChangeFieldUpdate> lunset = new List<ModelChangeFieldUpdate>();
                    ModelChangeFieldUpdate change;

                    List<ModelSupportSendAdd> listmodel = new List<ModelSupportSendAdd>();

                    foreach (var item in result)
                    {
                        //var onwer = Program.dbxrm.new_LunaTechSupportSet.FirstOrDefault(x =>
                        //x.new_LunaTechSupportId == item.new_LunaTechSupportId);

                        var mdoel = new ModelSupportSendAdd()
                        {
                            supportid = item.Id,

                            title = item.new_name,

                            mobilephone = item.new_MobilePhone,

                            ownername = item.OwnerId.Name,

                            email = item.new_Email,

                            type = item.new_Type.Value,

                            description = item.new_Descriptions,

                            statusCode = item.statuscode.Value,

                            coderahgiri = item.new_TrackingCode,


                            //ownername = onwer.new_name,
                        };
                        listmodel.Add(mdoel);
                        change = new ModelChangeFieldUpdate()
                        {
                            id = item.Id,
                        };
                        lunset.Add(change);


                    }


                    //while (!await CreatePipeServerAsync("PipeModelSupportSendAdded", listmodel))
                    //{
                    //    await CreatePipeServerAsync("PipeModelSupportSendAdded", listmodel);
                    //}

                    var resultCall = await CallAddedPipeServerAsync("PipeModelSupportSendAdded", listmodel);

                    if (resultCall)
                    {
                        if (lunset.Count > 0)
                        {
                            foreach (var item in lunset)
                            {
                                Program._serviceProxy.Update(new new_LunaTechSupport { Id = item.id, new_UpdateCheck = false });
                                Program.dbxrm.SaveChanges();
                            }

                        }
                    }




                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //تکمیل
        /// <summary>
        /// تیکت آپدیت شده و ارسال داخل لوله  
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="dataToSend"></param>
        /// <returns></returns>
        public static NamedPipeServerStream pipeServerClinet;

        public static async Task<bool> UpdatePipeServerAsync(string pipeName, object dataToSend)
        {

            try
            {

                using (pipeServerClinet = new NamedPipeServerStream(pipeName, PipeDirection.Out, maxNumberOfServerInstances: 1))
                {

                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(700)); // زمانبندی 30 ثانیه
                    Console.WriteLine("Waiting for client connection...");
                    await pipeServerClinet.WaitForConnectionAsync(cts.Token);

                    //await Task.Factory.FromAsync(pipeServerClinet.BeginWaitForConnection, pipeServerClinet.EndWaitForConnection, null);

                    BinaryFormatter formatterAdded = new BinaryFormatter();
                    {

                        formatterAdded.Serialize(pipeServerClinet, dataToSend);

                        return true;
                    }

                }


            }
            catch (Exception ex)
            {
                pipeServerClinet.Disconnect();
                pipeServerClinet.Dispose();
                Console.WriteLine(ex.Message + "erq" + "\n");
                return false;
            }




        }
        //finish

        //ایجاد برای ویندوز سرویس مشتری
        /// <summary>
        /// ایجاد یه خط لوله با دیتای مشخص
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="dataToSend"></param>
        /// <returns></returns>
        public static NamedPipeServerStream pipeServerClinet1;
        public static async Task<bool> CreatePipeServerAsync(string pipeName, object dataToSend)
        {

            try
            {
                using (pipeServerClinet1 = new NamedPipeServerStream(pipeName, PipeDirection.Out))
                {

                    Console.WriteLine("Waiting for client connection...");
                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(700)); // زمانبندی 30 ثانیه
                    await pipeServerClinet1.WaitForConnectionAsync(cts.Token);
                    //await Task.Factory.FromAsync(pipeServerClinet.BeginWaitForConnection, pipeServerClinet.EndWaitForConnection, null);
                    Console.WriteLine("Client connected.");

                    BinaryFormatter formatterAdded = new BinaryFormatter();
                    {
                        formatterAdded.Serialize(pipeServerClinet1, dataToSend);

                        return true;
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "er" + "\n");
                pipeServerClinet1.Disconnect();
                pipeServerClinet1.Dispose();
                return false;
            }



        }


        //finish
        //دریافت از ویندوز سرویس دوم
        // دو متد  چک کردن خط لوله و در صورت پر بودن دریافت دیتا(Object)
        /// <summary>
        /// چک کردن خط لوله و در صورت بودن دریافت ابجکت موجود در داخل لوله
        /// </summary>
        /// <param name="pipeName1"></param>
        /// <param name="pipeName2"></param>
        /// <returns></returns>
        //دریافت دیتای داخل لوله از سرویس لونا

        public static NamedPipeServerStream pipeClientpipeName1;
        public static async Task<(object, string)> GetDataFromPipespipereciveid(string pipeName1)
        {
            try
            {
                using (var pipeClientpipeName1 = new NamedPipeClientStream(".", pipeName1, PipeDirection.In))
                {
                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(600)); // زمانبندی 30 ثانیه
                    await pipeClientpipeName1.ConnectAsync(cts.Token);
                    if (pipeClientpipeName1.IsConnected == true)
                    {
                        //اگه خط لوله پر باشه این متد دیتای داخل خط لوله را دریافت میکند
                        var objectFromPipe1 = ReadDataFromPipe(pipeClientpipeName1);

                        pipeClientpipeName1.Dispose();  // بعد از خواندن داده، لوله باید بسته شود
                        return (objectFromPipe1, "ReciveId");
                    }
                    else
                    {
                        pipeClientpipeName1.Dispose();
                    }

                }


                return (null, "");
            }
            catch (Exception ex)
            {
                pipeClientpipeName1.Dispose();
                return (null, "");
            }


        }

        public static NamedPipeClientStream pipeClientpipeName;
        public static async Task<(object, string)> GetDataFromPipespipeUpdate(string pipeName2)
        {
            try
            {
                using (var pipeClientpipeName = new NamedPipeClientStream(".", pipeName2, PipeDirection.In))
                //using (var pipeClient2 = new NamedPipeClientStream(".", pipeName2))
                {
                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(600));
                    await pipeClientpipeName.ConnectAsync(cts.Token);
                    if (pipeClientpipeName.IsConnected == true)
                    {

                        var objectFromPipe2 = ReadDataFromPipe(pipeClientpipeName);
                        pipeClientpipeName.Dispose();  // بعد از خواندن داده، لوله باید بسته شود
                        return (objectFromPipe2, "updateserver");
                    }
                    else
                    {
                        pipeClientpipeName.Dispose();
                    }

                }

                return (null, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                pipeClientpipeName.Dispose();
                return (null, "");
            }


        }
        //finish

        /// <summary>
        ///در صورت پر بودن خط لوله این متد برای دریافت دیتای خط لوله اجرا میشود ابجکت ریترن یراس متد GetDataFormPipes
        /// </summary>
        /// <param name="pipeClient"></param>
        /// <returns>object</returns>
        private static object ReadDataFromPipe(NamedPipeClientStream pipeClient)
        {
            try
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
            catch
            {
                return null;
            }

        }

        //finish
        //متدهای پایه کلاس تایمر
        public void Start()
        {
            _timerSendAddedToServiceLuna.Start();
            _timerpipeUpdate.Start();
            _timerSendUpdateToServiceLuna.Start();
            _timerpipereciveid.Start();
            ;
        }

        public void Stop()
        {
            _timerSendAddedToServiceLuna.Stop();
            _timerpipeUpdate.Stop();
            _timerSendUpdateToServiceLuna.Stop();
            _timerpipereciveid.Stop();
        }
    }







}
