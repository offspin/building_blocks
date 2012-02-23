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
using System.IO;
using System.Xml.Serialization;


namespace PartyClient
{
    public partial class PartySearchForm : PartyClientForm
    {
        public PartySearchForm()
        {
            InitializeComponent();
        }


        private void nameSearchButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                CheckBusyAskCancel();

                formBusyIndicator.IsBusy = true;
                string search = nameTextBox.Text.Trim();
                string api = string.Format("party/byname/{0}", search);

                this.webClient = app.ServiceGet(api, NameSearchCompleted);

            }
            catch (Exception ex)
            {
                formBusyIndicator.IsBusy = false;
                app.ShowError(ex.Message);
            }

        }

        void NameSearchCompleted(object sender, OpenReadCompletedEventArgs e)
        {

            try
            {
                if (e.Cancelled)
                {
                    throw (new Exception("Request Cancelled"));
                }

                if (e.Error != null)
                {
                    throw (e.Error.InnerException == null) ?
                        e.Error : e.Error.InnerException;
                }

                PartyService.PartyResults pr =
                    (PartyService.PartyResults)((App)App.Current).LoadBusinessObject<PartyService.PartyResults>(e.Result);

                partyDataGrid.ItemsSource = pr.PartyList;
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

        private void LoadParty(PartyService.PartySummary partySummary)
        {
            try
            {
                formBusyIndicator.IsBusy = true;

                CheckBusyAskCancel();

                this.webClient = app.ServiceGet(
                    string.Format("party/{0}", partySummary.Id), PartyLoadCompleted);

            }
            catch (Exception ex)
            {
                formBusyIndicator.IsBusy = false;
                MessageBox.Show(ex.Message, "Loading", MessageBoxButton.OK);
            }
        }

        void PartyLoadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    throw (new Exception("Request Cancelled"));
                }

                if (e.Error != null)
                {
                    throw (e.Error.InnerException == null) ?
                        e.Error : e.Error.InnerException;
                }


                string returnTypeName = app.GetBusinessObjectType(e.Result);

                switch (returnTypeName)
                {
                    case "Business":
                        PartyService.Business bus = app.LoadBusinessObject<PartyService.Business>(e.Result);
                        BusinessForm businessForm = new BusinessForm();
                        businessForm.DataContext = bus;
                        SetContent(businessForm);
                        break;
                    case "Person":
                        PartyService.Person per = app.LoadBusinessObject<PartyService.Person>(e.Result);
                        PersonForm personForm = new PersonForm();
                        personForm.DataContext = per;
                        SetContent(personForm);
                        break;
                    case "Error":
                        PartyService.Error err = app.LoadBusinessObject<PartyService.Error>(e.Result);
                        throw (new Exception(err.Message));
                    default:
                        throw (new Exception("Unrecognised object type in server response"));
                }


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

        private void editPartyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                formBusyIndicator.IsBusy = true;
                Button cellButton = (Button)e.OriginalSource;
                PartyService.PartySummary partySummary = (PartyService.PartySummary)cellButton.Tag;
                LoadParty(partySummary);
            }
            catch (Exception ex)
            {
                formBusyIndicator.IsBusy = false;
                app.ShowError(ex.Message);
            }
        }

        private void newPersonButton_Click(object sender, RoutedEventArgs e)
        {
            NewPerson();
        }

        private void NewPerson()
        {

            try
            {
                formBusyIndicator.IsBusy = true;
                PartyService.Person person = new PartyService.Person();
                PersonForm personForm = new PersonForm();
                personForm.DataContext = person;
                SetContent(personForm);
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

        private void newBusinessButton_Click(object sender, RoutedEventArgs e)
        {
            NewBusiness();
        }

        private void NewBusiness()
        {
            try
            {
                formBusyIndicator.IsBusy = true;
                PartyService.Business business = new PartyService.Business();
                BusinessForm businessForm = new BusinessForm();
                businessForm.DataContext = business;
                SetContent(businessForm);
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

        private void deletePartyButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedParty();
        }

        private void DeleteSelectedParty()
        {
            try
            {
                if (partyDataGrid.SelectedItems.Count == 0)
                {
                    throw (new Exception("Please select an item to delete"));
                }

                PartyService.PartySummary partySummary =
                    (PartyService.PartySummary)partyDataGrid.SelectedItem;

                MessagePopUp confirmDelete = new MessagePopUp(
                    string.Format("Are you sure you want to delete [{0}]?", partySummary.Name),
                    MessagePopUp.MessagePopUpType.YesNo);

                confirmDelete.Closed += ConfirmDeleteHandler;

                confirmDelete.Show();
            }
            catch (Exception ex)
            {
                formBusyIndicator.IsBusy = false;
                app.ShowError(ex.Message);
            }
              
        }

        private void ConfirmDeleteHandler(object sender, EventArgs e)
        {
            try
            {
                CheckBusyAskCancel();

                formBusyIndicator.IsBusy = true;

                if (partyDataGrid.SelectedItems.Count == 0)
                {
                    throw (new Exception("Please select an item to delete"));
                }

                PartyService.PartySummary partySummary =
                    (PartyService.PartySummary)partyDataGrid.SelectedItem;

                string action =
                    string.Format("party/{0}/delete", partySummary.Id);

                this.webClient = app.ServiceDelete(action, DeleteCompleted);
                
            }
            catch (Exception ex)
            {
                formBusyIndicator.IsBusy = false;
                app.ShowError(ex.Message);
            }
 
        }

        private void DeleteCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                CheckForErrors(e);

                string returnTypeName = app.GetBusinessObjectType(e.Result);

                switch (returnTypeName)
                {
                    case "Acknowledgement":
                        PartyService.Acknowledgement ack = app.LoadBusinessObject<PartyService.Acknowledgement>(e.Result);
                        MessagePopUp ackPopUp = new MessagePopUp(ack.Message, MessagePopUp.MessagePopUpType.OK);
                        ackPopUp.Show();
                        break;
                    case "Error":
                        PartyService.Error err = app.LoadBusinessObject<PartyService.Error>(e.Result);
                        throw (new Exception(err.Message));
                    default:
                        throw (new Exception("Unrecognised object type in server response"));
                }


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
