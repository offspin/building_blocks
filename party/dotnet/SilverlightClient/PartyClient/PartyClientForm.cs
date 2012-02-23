using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PartyClient
{
    public partial class PartyClientForm : UserControl
    {
        protected App app
        {
            get { return (App)Application.Current; }
        }

        protected WebClient webClient;
        protected bool isDirty;

        protected void CheckBusyAskCancel()
        {
            if (this.webClient != null && this.webClient.IsBusy)
            {
                MessagePopUp mpu = new MessagePopUp(
                    "Operation in progress: do you want to cancel it?",
                    MessagePopUp.MessagePopUpType.YesNo);
                mpu.Closed += CancelOperationPopUpHandler;
            }
        }

        protected void CancelOperationPopUpHandler(object sender, EventArgs e)
        {
            MessagePopUp mpu = (MessagePopUp)sender;

            switch (mpu.Result)
            {
                case MessagePopUp.MessagePopUpResult.Yes:
                    if (this.webClient != null && this.webClient.IsBusy)
                    {
                        this.webClient.CancelAsync();
                    }
                    break;
                case MessagePopUp.MessagePopUpResult.No:
                default:
                    // do nothing
                    break;
            }
        }

        protected void Cancel()
        {
            if (this.isDirty)
            {
                MessagePopUp mpu = new MessagePopUp(
                    "Changes have been made: do you want to save?",
                    MessagePopUp.MessagePopUpType.YesNoCancel);
                mpu.Closed += CancelPopUpHandler;
                mpu.Show();
            }
            else
            {
                RevertContent();
            }
        }

        protected void CancelPopUpHandler(object sender, EventArgs e)
        {
            MessagePopUp mpu = (MessagePopUp)sender;

            switch (mpu.Result)
            {
                case MessagePopUp.MessagePopUpResult.Yes:
                    Save();
                    break;
                case MessagePopUp.MessagePopUpResult.No:
                    RevertContent();
                    break;
                case MessagePopUp.MessagePopUpResult.Cancel:
                default:
                    // do nothing
                    break;
            }
        }

        protected void CheckForErrors(OpenReadCompletedEventArgs e)
        {
            ProcessError(e.Cancelled ? new Exception("Operation cancelled") : e.Error);
        }

        protected void CheckForErrors(UploadStringCompletedEventArgs e)
        {
            ProcessError(e.Cancelled ? new Exception("Operation cancelled") : e.Error);
        }


        private void ProcessError(Exception ex)
        {
            if (ex != null)
            {
                throw (ex.InnerException == null) ?
                              ex : ex.InnerException;
            }
        }

        public void SetContent(UIElement content)
        {
            app.LastContent = this.Content;
            this.Content = content;
        }

        public void RevertContent()
        {
            if (app.LastContent != null)
            {
                this.Content = app.LastContent;
            }
        }

        protected virtual void Save()
        {
            throw (new Exception("Save not implemented"));
        }

        protected virtual void Delete()
        {
            throw (new Exception("Delete not implemented"));
        }


    }
}
