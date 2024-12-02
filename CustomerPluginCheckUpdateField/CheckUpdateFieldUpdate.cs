using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.ServiceModel;
using Xrm;

namespace CustomerPluginCheckUpdateField
{
    public class CheckUpdateFieldUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)
                  serviceProvider.GetService(typeof(IPluginExecutionContext));
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                IOrganizationServiceFactory serviceFactory =
                  (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);


                try
                {

                    ////Check UpdateRecord<1



                    var incident = service.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(true)) as new_LunaTechSupport;


                    if (context.MessageName.ToLower() == "create")
                    {
                        if (incident.new_Confirmation == true)
                        {
                            incident.new_UpdateCheck = true;
                            service.Update(incident);

                        }

                    }

                    if (context.MessageName.ToLower() == "update")
                    {
                        if (incident.new_Confirmation == true)
                        {
                            if (!entity.TryGetAttributeValue("new_UpdateCheck".ToLower(), out bool? _))
                            {
                                incident.new_UpdateCheck = true;
                                service.Update(incident);
                            }
                        }
                    }
                    //else if (context.MessageName.ToLower() == "update")
                    //{

                    //}






                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
