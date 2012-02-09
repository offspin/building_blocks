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

            WebServiceHost wsh = new WebServiceHost(typeof(PartyService.Service), new Uri("http://localhost:8000"));
            
            
            WebHttpBinding whb = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
            whb.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            wsh.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            wsh.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new PartyService.Validator();
            wsh.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
   
            ServiceEndpoint ep = wsh.AddServiceEndpoint(typeof(PartyService.Service), whb, "");
         
            

            wsh.Open();
            Console.WriteLine("Service is running - press enter to quit");
            Console.ReadLine();
            wsh.Close();
            Console.WriteLine("Service stopped");

        }
    }
}
