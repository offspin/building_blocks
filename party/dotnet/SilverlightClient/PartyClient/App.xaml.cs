using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Interop;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace PartyClient
{
    public partial class App : Application
    {

        public string PartyServiceUrl { get; set; }
        public NetworkCredential Credentials { get; set; }
        public UIElement LastContent { get; set; }

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;
            WebRequest.RegisterPrefix("http://", System.Net.Browser.WebRequestCreator.ClientHttp);

            InitializeComponent();
        }

        public void ShowError(string message)
        {
            MessagePopUp mpu = new MessagePopUp(message, MessagePopUp.MessagePopUpType.OK);
            mpu.Show();
        }

        public WebClient ServiceGet(string api, OpenReadCompletedEventHandler eventHandler)
        {
            WebClient wc = new WebClient();
            Uri uri = new Uri(string.Format("{0}/{1}", PartyServiceUrl, api));
            wc.UseDefaultCredentials = false;
            wc.Credentials = this.Credentials;
            wc.OpenReadCompleted += eventHandler;
            wc.OpenReadAsync(uri);

            return wc;

        }

        public WebClient ServicePost<T>(string api, UploadStringCompletedEventHandler eventHandler, T postObject)
        {

            string postData="";

            if (postObject != null)
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                using (MemoryStream ms = new MemoryStream())
                {
                    xs.Serialize(ms, postObject);
                    ms.Seek(0, SeekOrigin.Begin);
                    byte[] b = new byte[ms.Length];
                    ms.Read(b, 0, (int)ms.Length);
                    postData = System.Text.Encoding.UTF8.GetString(b, 0, (int)ms.Length);
                }
            }

            WebClient wc = new WebClient();
            Uri uri = new Uri(string.Format("{0}/{1}", PartyServiceUrl, api));
            wc.UseDefaultCredentials = false;
            wc.Credentials = this.Credentials;
            wc.UploadStringCompleted += eventHandler;
            wc.Headers[HttpRequestHeader.ContentType] = "application/xml";
            wc.UploadStringAsync(uri, postData);

            return wc;
        }

        public WebClient ServiceDelete(string action, UploadStringCompletedEventHandler eventHandler)
        {
            string postData = " ";

            WebClient wc = new WebClient();
            Uri uri = new Uri(string.Format("{0}/{1}", PartyServiceUrl, action));
            wc.UseDefaultCredentials = false;
            wc.Credentials = this.Credentials;
            wc.UploadStringCompleted += eventHandler;
            wc.UploadStringAsync(uri, postData);

            return wc;
           
        }             

        public string GetBusinessObjectType(Stream s)
        {
            try
            {
                XDocument xd = XDocument.Load(s);
                return Convert.ToString(xd.Root.Name);
            }
            catch
            {
                return null;
            }
        }

        public string GetBusinessObjectType(String s)
        {
            try
            {
                XDocument xd = XDocument.Parse(s);
                return Convert.ToString(xd.Root.Name);
            }
            catch
            {
                return null;
            }
        }

        public T LoadBusinessObject<T>(string s)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(s);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            return LoadBusinessObject<T>(ms);
        }

        public T LoadBusinessObject<T>(Stream s)
        {

            s.Seek(0, SeekOrigin.Begin);
            XmlSerializer xs = new XmlSerializer(typeof(T));
            try
            {
                return (T)xs.Deserialize(s);
            }
            catch
            {
                try
                {
                    XmlSerializer es = new XmlSerializer(typeof(PartyService.Error));
                    s.Seek(0, SeekOrigin.Begin);
                    PartyService.Error error = (PartyService.Error)es.Deserialize(s);
                    throw (new Exception(error.Message));
                }
                catch
                {
                    throw;
                }
            }

        }


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.RootVisual = new Login();
            if (e.InitParams.ContainsKey("PartyServiceUrl"))
            {
                this.PartyServiceUrl = e.InitParams["PartyServiceUrl"];
            }
            else
            {
                this.PartyServiceUrl =
                    string.Format("http://{0}:{1}/service",
                    this.Host.Source.Host, 8000);
            }

        }

        private void Application_Exit(object sender, EventArgs e)
        { }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }

    }
}
