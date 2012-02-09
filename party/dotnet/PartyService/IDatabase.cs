using System;
using System.Collections.Generic;
using System.Data;

namespace PartyService
{
    public interface IDatabase
    {
        void setConnectionString(string connectionString);
        Party GetParty(int id);
        PartyResults GetPartyByName(string name, string baseUrl);
        int CreatePerson(string firstName, string lastName, DateTime dateOfBirth);
        void UpdatePerson(int id, string firstname, string lastname, DateTime dateOfBirth);
        int CreateBusiness(string name, string regNumber);
        void UpdateBusiness(int id, string name, string regNumber);
        void DeleteParty(int id);
        Contact GetContact(int id);
        ContactResults GetContactByPartyId(int partyId, string baseUrl);
        int CreateAddress(string street, string town, string county, string postCode);
        void UpdateAddress(int id, string street, string town, string county, string postCode);
        int CreateEmail(string type, string address);
        void UpdateEmail(int id, string type, string address);
        int CreateTelephone(string type, string number);
        void UpdateTelephone(int id, string type, string number);
        void DeleteContact(int id);
        PartyContact GetPartyContact(int partyId, int contactId);
        void CreatePartyContact(int partyId, int contactId, DateTime validFrom, DateTime validUntil);
        void UpdatePartyContact(int partyId, int contactId, DateTime validFrom, DateTime validUntil);
        void DeletePartyContact(int partyId, int contactId);
        SystemConfig GetSystemConfig(string name);
        User GetUser(string name);
        void CreateUser(string name, string fullName, string passwordHash);
        void UpdateUser(string name, string fullName, string passwordHash);
        void DeleteUser(string name);

    }
}
