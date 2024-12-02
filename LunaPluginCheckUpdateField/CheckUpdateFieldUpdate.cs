using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IdentityModel.Metadata;
using System.ServiceModel;
using Xrm;


namespace LunaPluginCheckUpdateField
{
    public class CheckUpdateFieldUpdate:IPlugin
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
             

                    var incident = service.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(true)) as Incident;

                   
                        if (!entity.TryGetAttributeValue("new_checkupdate".ToLower(), out bool? _))
                        {
                  
                            Incident support = new Incident();
                            support.Id = incident.Id;
                            support.new_CheckUpdate = true;
                            service.Update(support);
                        }



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
