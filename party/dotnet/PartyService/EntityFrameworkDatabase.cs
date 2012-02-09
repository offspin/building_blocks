using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Objects;
using System.Data.EntityClient;
using System.Transactions;

namespace PartyService
{
    public class EntityFrameworkDatabase : IDatabase
    {
        private string connectionString;

        public void setConnectionString(string connectionString)
        {
            
            EntityConnectionStringBuilder ecsb = new EntityConnectionStringBuilder();
            ecsb.Metadata = "res://*/Party.csdl|res://*/Party.ssdl|res://*/Party.msl";
            ecsb.Provider = "System.Data.SqlClient";
            if (!connectionString.ToLower().Contains("multipleactiveresultsets=true"))
            {
                connectionString += ";multipleactiveresultsets=true;";
            }
            ecsb.ProviderConnectionString = connectionString;
            this.connectionString = ecsb.ToString();
        }

        public Party GetParty(int id)
        {
            using (PartyEntities context = new PartyEntities(this.connectionString))
            {
                var parties =
                    from party in context.Parties
                    where (party.Id == id)
                    select party;

                if (parties.Count() == 0) { return null; }

                return parties.First();
            }

        }


        public PartyResults GetPartyByName(string name, string baseUrl)
        {
            using (PartyEntities context = new PartyEntities(this.connectionString))
            {
                PartyResults partyResults = new PartyResults();

                var nameQuery = context.ExecuteStoreQuery<PartySummary>(
                    "execute dbo.GetPartyByName {0}",
                    name);

                foreach (PartySummary partySummary in nameQuery)
                {
                    partySummary.Link =
                        string.Format("{0}party/{1}", baseUrl, partySummary.Id);

                    partyResults.PartyList.Add(partySummary);
                }

                return partyResults;
            }

        }

        public int CreatePerson(string firstName, string lastName, DateTime dateOfBirth)
        {
            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    Person person = new Person();
                    person.FirstName = firstName;
                    person.LastName = lastName;
                    person.DateOfBirth = dateOfBirth;
                    person.Type = "P";

                    context.Parties.AddObject(person);
                    context.SaveChanges();

                    return person.Id;
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }

        }

        public void UpdatePerson(int id, string firstName, string lastName, DateTime dateOfBirth)
        {
            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    Person person = (Person)GetParty(id);

                    if (person != null)
                    {
                        person.FirstName = firstName;
                        person.LastName = lastName;
                        person.DateOfBirth = dateOfBirth;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }

        public int CreateBusiness(string name, string regNumber)
        {
            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    Business business = new Business();
                    business.Name = name;
                    business.RegNumber = regNumber;
                    business.Type = "B";

                    context.Parties.AddObject(business);
                    context.SaveChanges();

                    return business.Id;
                }

            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }

        }

        public void UpdateBusiness(int id, string name, string regNumber)
        {
            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    Business business = (Business)GetParty(id);

                    if (business != null)
                    {
                        business.Name = name;
                        business.RegNumber = regNumber;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }

        public void DeleteParty(int id)
        {
            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    using (TransactionScope txnScope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        Party party =
                            (from p in context.Parties
                             where (p.Id == id)
                             select p).First();

                        if (party != null)
                        {
                            var partyContacts =
                                from partyContact in context.PartyContacts
                                where (partyContact.PartyId == id)
                                select partyContact;

                            foreach (PartyContact toRemove in partyContacts)
                            {
                                context.DeleteObject(toRemove);
                                context.SaveChanges();
                            }

                            context.DeleteObject(party);
                            
                            context.SaveChanges();
                            context.AcceptAllChanges();
                            txnScope.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }

        public Contact GetContact(int id)
        {
            using (PartyEntities context = new PartyEntities(this.connectionString))
            {
                var contacts =
                    from contact in context.Contacts
                    where (contact.Id == id)
                    select contact;

                if (contacts.Count() == 0) { return null; }

                return contacts.First();
            }
        }

        public ContactResults GetContactByPartyId(int partyId, string baseUrl)
        {
            using (PartyEntities context = new PartyEntities(this.connectionString))
            {
                ContactResults contactResults = new ContactResults();

                var contactQuery = context.ExecuteStoreQuery<ContactSummary>(
                    "execute dbo.GetContactByPartyId {0}", partyId);

                foreach (ContactSummary contactSummary in contactQuery)
                {
                    contactSummary.Link =
                        string.Format("{0}contact/{1}", baseUrl, contactSummary.Id);
                    contactResults.ContactList.Add(contactSummary);
                }

                return contactResults;
            }
        }

        public int CreateAddress(string street, string town, string county, string postCode)
        {

            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    Address address = new Address();
                    address.Street = street;
                    address.Town = town;
                    address.County = county;
                    address.PostCode = postCode;
                    address.Type = "A";

                    context.Contacts.AddObject(address);
                    context.SaveChanges();
                    return address.Id;
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }

        public void UpdateAddress(int id, string street, string town, string county, string postCode)
        {
            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    Address address = (Address)GetContact(id);

                    if (address != null)
                    {
                        address.Street = street;
                        address.Town = town;
                        address.County = county;
                        address.PostCode = postCode;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }

        public int CreateEmail(string type, string address)
        {

            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    Email email = new Email();
                    email.Address = address;
                    email.SubType = type;
                    email.Type = "E";

                    context.Contacts.AddObject(email);
                    context.SaveChanges();
                    return email.Id;
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }

        public void UpdateEmail(int id, string type, string address)
        {
            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    Email email = (Email)GetContact(id);

                    if (email != null)
                    {
                        email.Address = address;
                        email.SubType = type;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }

        public int CreateTelephone(string type, string number)
        {
            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    Telephone telephone = new Telephone();
                    telephone.Number = number;
                    telephone.SubType = type;
                    telephone.Type = "T";

                    context.Contacts.AddObject(telephone);
                    context.SaveChanges();
                    return telephone.Id;
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }

        public void UpdateTelephone(int id, string type, string number)
        {
            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    Telephone telephone = (Telephone)GetContact(id);

                    if (telephone != null)
                    {
                        telephone.Number = number;
                        telephone.SubType = type;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }

        }

        public void DeleteContact(int id)
        {

            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {

                    Contact contact =
                        (from ct in context.Contacts
                         where (ct.Id == id)
                         select ct).First();

                    if (contact != null)
                    {
                        context.DeleteObject(contact);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }

        public PartyContact GetPartyContact(int partyId, int contactId)
        {

            using (PartyEntities context = new PartyEntities(this.connectionString))
            {
                var partyContacts =
                    from partyContact in context.PartyContacts
                    where (partyContact.PartyId == partyId && partyContact.ContactId == contactId)
                    select partyContact;

                if (partyContacts.Count() == 0) { return null; }

                return partyContacts.First();

            }

        }

        public void CreatePartyContact(int partyId, int contactId, DateTime validFrom, DateTime validUntil)
        {

            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    PartyContact partyContact = new PartyContact();
                    partyContact.PartyId = partyId;
                    partyContact.ContactId = contactId;
                    partyContact.ValidFrom = validFrom;
                    partyContact.ValidUntil = validUntil;

                    context.PartyContacts.AddObject(partyContact);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }

        public void UpdatePartyContact(int partyId, int contactId, DateTime validFrom, DateTime validUntil)
        {

            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    PartyContact partyContact = GetPartyContact(partyId, contactId);

                    if (partyContact != null)
                    {
                        partyContact.ValidFrom = validFrom;
                        partyContact.ValidUntil = validUntil;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }

        public void DeletePartyContact(int partyId, int contactId)
        {
            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {

                    PartyContact partyContact =
                       (from pc in context.PartyContacts
                        where (pc.PartyId == partyId && pc.ContactId == contactId)
                        select pc).First();

                    if (partyContact != null)
                    {
                        context.DeleteObject(partyContact);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }

        public SystemConfig GetSystemConfig(string name)
        {
            using (PartyEntities context = new PartyEntities(this.connectionString))
            {
                var systemConfigs =
                    from systemConfig in context.SystemConfigs
                    where (systemConfig.Name == name)
                    select systemConfig;

                if (systemConfigs.Count() == 0) { return null; }
                return systemConfigs.First();
            }
        }

        public User GetUser(string name)
        {
            using (PartyEntities context = new PartyEntities(this.connectionString))
            {
                var users =
                    from user in context.Users
                    where (user.Name == name)
                    select user;

                if (users.Count() == 0) { return null; }
                return users.First();
            }
        }


        public void CreateUser(string name, string fullName, string passwordHash)
        {
            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    User user = new User();
                    user.Name = name;
                    user.FullName = fullName;
                    user.PasswordHash = passwordHash;

                    context.Users.AddObject(user);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }
        
        public void UpdateUser(string name, string fullName, string passwordHash) 
        {
            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {
                    User user = GetUser(name);

                    if (user != null)
                    {
                        user.FullName = fullName;
                        user.PasswordHash = passwordHash;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }
        
        public void DeleteUser(string name)
        {
            try
            {
                using (PartyEntities context = new PartyEntities(this.connectionString))
                {

                    User user =
                        (from u in context.Users
                         where (u.Name == name)
                         select u).First();

                    if (user != null)
                    {
                        context.DeleteObject(user);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is UpdateException) { throw ex.InnerException; }
                throw;
            }
        }

    }

    class PartyEntities : ObjectContext
    {
        private ObjectSet<Party> parties;
        private ObjectSet<User> users;
        private ObjectSet<Contact> contacts;
        private ObjectSet<PartyContact> partyContacts;

        private ObjectSet<SystemConfig> systemConfigs;

        public ObjectSet<Party> Parties
        {
            get
            {
                if (parties == null)
                {
                    parties =
                        base.CreateObjectSet<Party>();
                }
                return parties;
            }
        }

        public ObjectSet<Contact> Contacts
        {
            get
            {
                if (contacts == null)
                {
                    contacts =
                        base.CreateObjectSet<Contact>();
                }
                return contacts;
            }
        }

        public ObjectSet<PartyContact> PartyContacts
        {
            get
            {
                if (partyContacts == null)
                {
                    partyContacts =
                        base.CreateObjectSet<PartyContact>();
                }
                return partyContacts;
            }
        }

        public ObjectSet<User> Users
        {
            get
            {
                if (users == null)
                {
                    users =
                     base.CreateObjectSet<User>();
                }
                return users;
            }
        }

        public ObjectSet<SystemConfig> SystemConfigs
        {
            get
            {
                if (systemConfigs == null)
                {
                    systemConfigs = base.CreateObjectSet<SystemConfig>();
                }
                return systemConfigs;
            }
        }

        public PartyEntities()
            : base("name=PartyEntities", "PartyContainer")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
        }

        public PartyEntities(string connectionString) :
            base(connectionString, "PartyContainer")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
        }

    }
}
