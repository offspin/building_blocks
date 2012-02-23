using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Xml.Serialization;

namespace PartyClient
{
    public partial class Login : PartyClientForm
    {
        public Login()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                formBusyIndicator.IsBusy = true;

                string userName = userNameTextBox.Text.Trim();
                string password = passwordTextBox.Password.Trim();
                app.Credentials = new NetworkCredential(userName, password);
                this.webClient = app.ServiceGet("ping", PingCompleted);
                
            }
            catch (Exception ex)
            {
                formBusyIndicator.IsBusy = false;
                app.ShowError(ex.Message);
            }
        }

        private void PingCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                CheckForErrors(e);

                Stream s = e.Result;

                XmlSerializer xs = new XmlSerializer(typeof(PartyService.Acknowledgement));
                PartyService.Acknowledgement ack = (PartyService.Acknowledgement)xs.Deserialize(s);

                PartySearchForm partySearch = new PartySearchForm();

                SetContent(partySearch);
            }
            catch (Exception ex)
            {
                app.ShowError(ex.Message);
            }
            finally
            {
                formBusyIndicator.IsBusy = false;
            }

        }

     
    }
}
