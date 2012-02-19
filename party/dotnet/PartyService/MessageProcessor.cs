using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace PartyService
{
    public class MessageProcessor : IDispatchMessageInspector
    {

        public void BeforeSendReply(ref Message message, object correlationState)
        {
            if (message.Properties.ContainsKey("httpResponse"))
            {
                HttpResponseMessageProperty response = (HttpResponseMessageProperty)message.Properties["httpResponse"];
                if (response.Headers["Cache-Control"] == null)
                {
                    response.Headers.Add("Cache-Control: no-cache");
                }
            }
        }

        public object AfterReceiveRequest(ref Message message, IClientChannel channel, InstanceContext instanceContext)
        {
            if (message.Properties.ContainsKey("httpRequest") && message.Properties.ContainsKey("Via"))
            {
                Uri uri = (Uri)message.Properties["Via"];
                HttpRequestMessageProperty request = (HttpRequestMessageProperty)message.Properties["httpRequest"];
                string diag = string.Format("{0}:{1}", request.Method, uri.AbsoluteUri);
                if (request.Method == "POST")
                {
                    diag += string.Format("\nPost Data:{0}\n", message.ToString());
                }
                Console.Out.WriteLine(diag);
            }

            return null;
        }
    }

    public class ProcessorBehaviour : IEndpointBehavior
    {
        public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime)
        { }

        public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new MessageProcessor());
        }

        public void AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters)
        { }

        public void Validate(ServiceEndpoint serviceEnpoint)
        { }
    }
}
