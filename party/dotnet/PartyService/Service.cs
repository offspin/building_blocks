using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace PartyService
{
    [ServiceContract]
    [XmlSerializerFormat]
    public class Service
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
            database = (IDatabase) Activator.CreateInstance(Type.GetType(dbClassName));
            database.setConnectionString(dbConnectionString);

        }

        [WebGet(UriTemplate = "party/{idStr}")]
        [OperationContract]
        [ServiceKnownType(typeof(Person))]
        [ServiceKnownType(typeof(Business))]
        public Party GetParty(string idStr)
        {
            try
            {
                int id;

                if (!int.TryParse(idStr, out id))
                {
                    throw (new PartyServiceException(
                        string.Format("'{0}' is not a valid party identifier", idStr),
                        HttpStatusCode.BadRequest));
                }

                DataRow r = database.GetParty(id);

                if (r == null)
                {
                    throw (new PartyServiceException(string.Format("Party {0} not found", id), HttpStatusCode.NotFound));
                }

                string type = Convert.ToString(r["type"]);
                switch (type)
                {
                    case "P":
                        string firstName = Convert.ToString(r["first_name"]);
                        string lastName = Convert.ToString(r["last_name"]);
                        DateTime dateOfBirth = Convert.ToDateTime(r["date_of_birth"]);
                        return new Person(id, firstName, lastName, dateOfBirth);
                    case "B":
                        string name = Convert.ToString(r["name"]);
                        return new Business(id, name);
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                return (Party)ErrorObject(new Party(), ex);
            }
     
        }

        [WebGet(UriTemplate = "party/byname/{name}")]
        [OperationContract]
        public PartyResults GetPartiesByName(string name)
        {
            string baseUrl = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri.AbsoluteUri;

            PartyResults pl = new PartyResults();

            DataTable t = database.GetPartyByName(name);
            if (t == null) { return pl; }

            foreach (DataRow r in t.Rows)
            {
                int id = Convert.ToInt32(r["id"]);
                string type = Convert.ToString(r["type"]);
                string partyName = Convert.ToString(r["name"]);
                string link = string.Format("{0}party/{1}", baseUrl, id);

                PartySummary ps = new PartySummary(id, type, partyName, link);
                pl.PartyList.Add(ps);
            }

            return pl;
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "person")]
        public Person CreatePerson(Person person)
        {
            try
            {
                int newId = database.CreatePerson(person.FirstName, person.LastName, person.DateOfBirth);
                return (Person)GetParty(Convert.ToString(newId));
            }
            catch (Exception ex)
            {
                return (Person)ErrorObject(person, ex);
            }
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "person/{idStr}")]
        public Person UpdatePerson(string idStr, Person person)
        {
            try
            {
                int id = Convert.ToInt32(idStr);
                if (database.GetParty(id) == null)
                {
                    throw (new PartyServiceException(string.Format("Person {0} not found", id), HttpStatusCode.NotFound));
                }
                database.UpdatePerson(id, person.FirstName, person.LastName, person.DateOfBirth);
                return (Person)GetParty(idStr);
            }
            catch (Exception ex)
            {
                return (Person)ErrorObject(person, ex);
            }
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "business")]
        public Business CreateBusiness(Business business)
        {
            try
            {
                int newId = database.CreateBusiness(business.Name);
                return (Business)GetParty(Convert.ToString(newId));
            }
            catch (Exception ex)
            {
                return (Business)ErrorObject(business, ex);
            }
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "business/{idStr}")]
        public Business UpdateBusiness(string idStr, Business business)
        {
            try
            {
                int id;

                if (!int.TryParse(idStr, out id))
                {
                    throw (new PartyServiceException(
                        string.Format("'{0}' is not a valid party identifier", idStr),
                        HttpStatusCode.BadRequest));
                }

                if (database.GetParty(id) == null)
                {
                    throw (new Exception(string.Format("Business {0} not found", id)));
                }
                database.UpdateBusiness(id, business.Name);
                return (Business)GetParty(idStr);
            }
            catch (Exception ex)
            {
                return (Business)ErrorObject(business, ex);
            }

        }

        [OperationContract]
        [WebInvoke(UriTemplate = "party/{idStr}/delete")]
        public Acknowledgement DeleteParty(string idStr)
        {
            int id = Convert.ToInt32(idStr);
            database.DeleteParty(id);
            return new Acknowledgement(
                string.Format("Party {0} deleted", id));
        }

        [WebGet(UriTemplate = "contact/{idStr}")]
        [OperationContract]
        [ServiceKnownType(typeof(Address))]
        [ServiceKnownType(typeof(Email))]
        [ServiceKnownType(typeof(Telephone))]
        public Contact GetContact(string idStr)
        {
            try
            {
                int id = Convert.ToInt32(idStr);
                DataRow r = database.GetContact(id);
                if (r == null)
                {
                    throw (new Exception(string.Format("Contact {0} not found", id)));
                }

                string type = Convert.ToString(r["type"]);
                string subType = Convert.ToString(r["sub_type"]);
                string street = Convert.ToString(r["street"]);
                string town = Convert.ToString(r["town"]);
                string county = Convert.ToString(r["county"]);
                string postCode = Convert.ToString(r["post_code"]);
                string emailAddress = Convert.ToString(r["email_address"]);
                string telephoneNumber = Convert.ToString(r["telephone_number"]);

                switch (type)
                {
                    case "A":
                        return new Address(id, street, town, county, postCode);
                    case "E":
                        return new Email(id, subType, emailAddress);
                    case "T":
                        return new Telephone(id, subType, telephoneNumber);
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Contact c = new Contact();
                c.Error = new Error(ex.Message);
                return c;
            }
        }

        private PartyServiceObject ErrorObject(PartyServiceObject pso, Exception ex, HttpStatusCode statusCode)
        {
            pso.Error = new Error(ex.Message);
            WebOperationContext.Current.OutgoingResponse.StatusCode = statusCode;
            return pso;
        }

        private PartyServiceObject ErrorObject(PartyServiceObject pso, Exception ex)
        {
            if (ex is PartyServiceException)
            {
                return ErrorObject(pso, ex,
                    ((PartyServiceException)ex).StatusCode);
            }
            return ErrorObject(pso, ex, HttpStatusCode.InternalServerError);
        }
      
    }
}
