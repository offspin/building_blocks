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
    public partial class PersonForm : PartyClientForm
    {
        public PersonForm()
        {
            InitializeComponent();
        }

        protected override void Save()
        {
            try
            {
                base.CheckBusyAskCancel();
                formBusyIndicator.IsBusy = true;

                PartyService.Person person = (PartyService.Person)DataContext;
                person.FirstName = firstNameTextBox.Text.Trim();
                person.LastName = lastNameTextBox.Text.Trim();
                person.DateOfBirth = (DateTime)dateOfBirthPicker.SelectedDate;
                this.webClient = app.ServicePost<PartyService.Person>(
                   string.Format("person/{0}", person.Id), SaveEventHandler, person);

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

                this.DataContext = app.LoadBusinessObject<PartyService.Person>(e.Result);
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
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void firstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.isDirty = true;
        }

        private void lastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.isDirty = true;
        }

        private void dateOfBirthPicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            this.isDirty = true;
        }

        private void dateOfBirthPicker_TextInputStart(object sender, TextCompositionEventArgs e)
        {
            this.isDirty = true;
        }

      
    }
}
