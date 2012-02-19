using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace PartyServer
{
    class Program
    {
        static void Main(string[] args)
        {

            WebServiceHost webServiceHost = new WebServiceHost(typeof(PartyService.Service));

            WebHttpBinding serviceBinding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
            serviceBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            ServiceEndpoint ep = webServiceHost.AddServiceEndpoint(
                typeof(PartyService.IService), 
                serviceBinding, 
                "http://localhost:8000/service");
            ep.Behaviors.Add(new PartyService.ProcessorBehaviour());
           
  
            WebHttpBinding staticBinding = new WebHttpBinding(WebHttpSecurityMode.None);
            ServiceEndpoint sep = webServiceHost.AddServiceEndpoint(
                typeof(PartyService.IStaticItemService), 
                new WebHttpBinding(), 
                "http://localhost:8000");

            webServiceHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            webServiceHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new PartyService.Validator();
            webServiceHost.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;

            webServiceHost.Open();
            Console.WriteLine("Service is running - press enter to quit");
            Console.ReadLine();
            webServiceHost.Close();
            Console.WriteLine("Service stopped");

        }
    }
}
