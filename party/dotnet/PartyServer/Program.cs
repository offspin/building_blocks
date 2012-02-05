using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;

namespace PartyServer
{
    class Program
    {
        static void Main(string[] args)
        {

            WebServiceHost wsh = new WebServiceHost(typeof(PartyService.Service), new Uri("http://localhost:8000"));

            ServiceEndpoint ep = wsh.AddServiceEndpoint(typeof(PartyService.Service), new WebHttpBinding(), "");

            wsh.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;

            wsh.Open();
            Console.WriteLine("Service is running - press enter to quit");
            Console.ReadLine();
            wsh.Close();
            Console.WriteLine("Service stopped");

        }
    }
}
