using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;


namespace PartyClient
{
    public partial class BusinessForm : PartyClientForm
    {
        public BusinessForm()
        {
            InitializeComponent();
        }

        private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.isDirty = true;
        }

        private void regNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.isDirty = true;
        }
     
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        protected override void Save()
        {
            try
            {
                base.CheckBusyAskCancel();
                formBusyIndicator.IsBusy = true;

                PartyService.Business business = (PartyService.Business)DataContext;
                business.Name = nameTextBox.Text.Trim();
                business.RegNumber = regNumberTextBox.Text.Trim();

                string action = "business";
                if (business.Id > 0)
                {
                    action += string.Format("/{0}", business.Id);
                }

                this.webClient = 
                    app.ServicePost<PartyService.Business>(action, SaveEventHandler, business);

            }
            catch (Exception ex)
            {
                formBusyIndicator.IsBusy = false;
                app.ShowError(ex.Message);
            }
        }


        private void SaveEventHandler(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                CheckForErrors(e);

                this.DataContext = app.LoadBusinessObject<PartyService.Business>(e.Result);
                this.isDirty = false;
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
