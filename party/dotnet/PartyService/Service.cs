using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;


namespace PartyService
{

    public class Service : IService, IStaticItemService
    {
        private IDatabase database;

        public Service()
        {
            string dbClassName = Properties.Settings.Default.DatabaseClass;
            string dbConnectionString = Properties.Settings.Default.DatabaseConnectionString;

            if (dbClassName == String.Empty || dbConnectionString == string.Empty)
            {
                throw (new Exception("Service Configuration Error"));
            }
            database = (IDatabase)Activator.CreateInstance(Type.GetType(dbClassName));
            database.setConnectionString(dbConnectionString);

        }

        public Party GetParty(string idStr)
        {

            try
            {
                int id;

                if (!int.TryParse(idStr, out id))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid party identifier", idStr)),
                        HttpStatusCode.BadRequest));
                }

                Party p = database.GetParty(id);

                if (p == null)
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("Party {0} not found", id)),
                        HttpStatusCode.NotFound));
                }

                return p;
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }

        }

        public PartyResults GetPartiesByName(string name)
        {
            try
            {
                string baseUrl = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri.AbsoluteUri;

                PartyResults partyResults = database.GetPartyByName(name, baseUrl);

                return partyResults;

            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }
        }

        public Person CreatePerson(Person person)
        {
            try
            {
                int newId = database.CreatePerson(person.FirstName, person.LastName, person.DateOfBirth);
                return (Person)GetParty(Convert.ToString(newId));
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }
        }

        public Person UpdatePerson(string idStr, Person person)
        {
            try
            {
                int id;

                if (!int.TryParse(idStr, out id))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid party identifier", idStr)),
                        HttpStatusCode.BadRequest));
                }

                if (database.GetParty(id) == null)
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("Person {0} not found", id)),
                        HttpStatusCode.NotFound));
                }

                database.UpdatePerson(id, person.FirstName, person.LastName, person.DateOfBirth);

                return (Person)GetParty(idStr);

            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }
        }

        public Business CreateBusiness(Business business)
        {
            try
            {
                int newId = database.CreateBusiness(business.Name, business.RegNumber);
                return (Business)GetParty(Convert.ToString(newId));
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }
        }

        public Business UpdateBusiness(string idStr, Business business)
        {
            try
            {
                int id;

                if (!int.TryParse(idStr, out id))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid party identifier", idStr)),
                        HttpStatusCode.BadRequest));
                }

                if (database.GetParty(id) == null)
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("Business {0} not found", id)),
                        HttpStatusCode.NotFound));
                }

                database.UpdateBusiness(id, business.Name, business.RegNumber);

                return (Business)GetParty(idStr);
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }

        }

        public Acknowledgement DeleteParty(string idStr)
        {

            try
            {
                int id;

                if (!int.TryParse(idStr, out id))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid party identifier", idStr)),
                        HttpStatusCode.BadRequest));
                }

                if (database.GetParty(id) == null)
                {
                    throw (new Exception(string.Format("Party {0} not found", id)));
                }

                database.DeleteParty(id);

                return new Acknowledgement(
                    string.Format("Party {0} deleted", id));
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }

        }

        public Contact GetContact(string idStr)
        {
            try
            {
                int id;

                if (!int.TryParse(idStr, out id))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid contact identifier", idStr)),
                        HttpStatusCode.BadRequest));
                }

                Contact c = database.GetContact(id);

                if (c == null)
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("Contact {0} not found", id)),
                        HttpStatusCode.NotFound));
                }

                return c;

            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }
        }

        public Address CreateAddress(Address address)
        {
            try
            {
                int newId = database.CreateAddress(address.Street, address.Town, address.County, address.PostCode);
                return (Address)GetContact(Convert.ToString(newId));
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }
        }

        public Address UpdateAddress(string idStr, Address address)
        {
            try
            {
                int id;

                if (!int.TryParse(idStr, out id))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid contact identifier", idStr)),
                        HttpStatusCode.BadRequest));
                }

                if (database.GetContact(id) == null)
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("Address {0} not found", id)),
                        HttpStatusCode.NotFound));
                }

                database.UpdateAddress(id, address.Street, address.Town, address.County, address.PostCode);

                return (Address)GetContact(idStr);
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }
        }

        public Email CreateEmail(Email email)
        {
            try
            {
                int newId = database.CreateEmail(email.SubType, email.Address);
                return (Email)GetContact(Convert.ToString(newId));
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }
        }

        public Email UpdateEmail(string idStr, Email email)
        {
            try
            {
                int id;

                if (!int.TryParse(idStr, out id))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid contact identifier", idStr)),
                        HttpStatusCode.BadRequest));
                }

                if (database.GetContact(id) == null)
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("Email {0} not found", id)),
                        HttpStatusCode.NotFound));
                }

                database.UpdateEmail(id, email.SubType, email.Address);

                return (Email)GetContact(idStr);
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }
        }

        public Telephone CreateTelephone(Telephone telephone)
        {
            try
            {
                int newId = database.CreateTelephone(telephone.SubType, telephone.Number);
                return (Telephone)GetContact(Convert.ToString(newId));
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }
        }

        public Telephone UpdateTelephone(string idStr, Telephone telephone)
        {
            try
            {
                int id;

                if (!int.TryParse(idStr, out id))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid contact identifier", idStr)),
                        HttpStatusCode.BadRequest));
                }

                if (database.GetContact(id) == null)
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("Telephone {0} not found", id)),
                        HttpStatusCode.NotFound));
                }

                database.UpdateTelephone(id, telephone.SubType, telephone.Number);

                return (Telephone)GetContact(idStr);
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }
        }

        public Acknowledgement DeleteContact(string idStr)
        {

            try
            {
                int id;

                if (!int.TryParse(idStr, out id))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid contact identifier", idStr)),
                        HttpStatusCode.BadRequest));
                }

                if (database.GetContact(id) == null)
                {
                    throw (new Exception(string.Format("Contact {0} not found", id)));
                }

                database.DeleteContact(id);

                return new Acknowledgement(
                    string.Format("Contact {0} deleted", id));
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }

        }

        public PartyContact GetPartyContact(string partyIdStr, string contactIdStr)
        {

            try
            {

                int partyId, contactId;

                if (!int.TryParse(partyIdStr, out partyId))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid party identifier", partyIdStr)),
                        HttpStatusCode.BadRequest));
                }

                if (!int.TryParse(contactIdStr, out contactId))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid contact identifier", contactIdStr)),
                        HttpStatusCode.BadRequest));
                }

                PartyContact partyContact = database.GetPartyContact(partyId, contactId);

                if (partyContact == null)
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("Party {0} is not linked to contact {1}", partyId, contactId)),
                        HttpStatusCode.NotFound));
                }

                return partyContact;

            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }

        }

        public PartyContact UpdatePartyContact(string partyIdStr, string contactIdStr, PartyContact partyContact)
        {

            try
            {

                int partyId, contactId;

                if (!int.TryParse(partyIdStr, out partyId))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid party identifier", partyIdStr)),
                        HttpStatusCode.BadRequest));
                }

                if (!int.TryParse(contactIdStr, out contactId))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid contact identifier", contactIdStr)),
                        HttpStatusCode.BadRequest));
                }

                PartyContact pc = database.GetPartyContact(partyId, contactId);

                if (pc == null)
                {
                    database.CreatePartyContact(partyId, contactId, partyContact.ValidFrom, partyContact.ValidUntil);
                }
                else
                {
                    database.UpdatePartyContact(partyId, contactId, partyContact.ValidFrom, partyContact.ValidUntil);
                }
                return GetPartyContact(partyIdStr, contactIdStr);
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }

        }

        public Acknowledgement DeletePartyContact(string partyIdStr, string contactIdStr)
        {

            try
            {

                int partyId, contactId;

                if (!int.TryParse(partyIdStr, out partyId))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid party identifier", partyIdStr)),
                        HttpStatusCode.BadRequest));
                }

                if (!int.TryParse(contactIdStr, out contactId))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid contact identifier", contactIdStr)),
                        HttpStatusCode.BadRequest));
                }

                PartyContact pc = database.GetPartyContact(partyId, contactId);

                if (pc == null)
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("Party {0} is not linked to contact {1}", partyId, contactId)),
                        HttpStatusCode.NotFound));
                }

                database.DeletePartyContact(partyId, contactId);

                return new Acknowledgement(
                    string.Format("Contact {0} removed from party {1}", contactId, partyId));
            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }

        }

        public ContactResults GetContactsByParty(string idStr)
        {
            try
            {

                int id;
                string baseUrl = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri.AbsoluteUri;

                if (!int.TryParse(idStr, out id))
                {
                    throw (new WebFaultException<Error>(
                        new Error(string.Format("'{0}' is not a valid party identifier", idStr)),
                        HttpStatusCode.BadRequest));
                }

                ContactResults contactResults = database.GetContactByPartyId(id, baseUrl);

                return contactResults;

            }
            catch (Exception ex)
            {
                throw MakeWebFaultException(ex);
            }
        }

        public Stream GetStaticFile(string fileName, string extension)
        {

            fileName.Trim(); extension.Trim();
            fileName.Replace("..", ""); extension.Replace("..", "");

            string contentType = "application/octet-stream";

            switch (extension.ToLower())
            {
                case "xml":
                    contentType = "application/xml";
                    break;
                case "html":
                    contentType = "text/html";
                    break;
                case "txt":
                    contentType = "text/plain";
                    break;
                case "gif":
                    contentType = "image/gif";
                    break;
                case "jpg":
                    contentType = "image/jpg";
                    break;
            }

  
            string staticFileName = string.Format("./public/{0}.{1}", fileName, extension);

            try
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = contentType;
                return File.OpenRead(staticFileName);
            }
            catch
            {
                throw (new WebFaultException<Error>(
                       new Error(string.Format("File access error for '{0}.{1}'", fileName, extension)),
                       HttpStatusCode.NotFound));
            }
          
        }

         private WebFaultException<Error> MakeWebFaultException(Exception ex)
        {
            return (ex is WebFaultException<Error>) ?
                (WebFaultException<Error>)ex :
                new WebFaultException<Error>(
                    new Error(ex.Message), HttpStatusCode.InternalServerError);
        }

    }
}
