using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace PartyService
{
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IService
    {

        [OperationContract]
        [WebGet(UriTemplate = "ping")]
        Acknowledgement Ping();

        [OperationContract]
        [WebGet(UriTemplate = "party/{idStr}")]
        [ServiceKnownType(typeof(Person))]
        [ServiceKnownType(typeof(Business))]
        Party GetParty(string idStr);

        [WebGet(UriTemplate = "party/byname/{name}")]
        [OperationContract]
        PartyResults GetPartiesByName(string name);

        [OperationContract]
        [WebInvoke(UriTemplate = "person")]
        Person CreatePerson(Person person);

        [OperationContract]
        [WebInvoke(UriTemplate = "person/{idStr}")]
        Person UpdatePerson(string idStr, Person person);

        [OperationContract]
        [WebInvoke(UriTemplate = "business")]
        Business CreateBusiness(Business business);

        [OperationContract]
        [WebInvoke(UriTemplate = "business/{idStr}")]
        Business UpdateBusiness(string idStr, Business business);

        [OperationContract]
        [WebInvoke(UriTemplate = "party/{idStr}/delete")]
        Acknowledgement DeleteParty(string idStr);

        [OperationContract]
        [WebGet(UriTemplate = "contact/{idStr}")]
        [ServiceKnownType(typeof(Address))]
        [ServiceKnownType(typeof(Email))]
        [ServiceKnownType(typeof(Telephone))]
        Contact GetContact(string idStr);

        [OperationContract]
        [WebInvoke(UriTemplate = "address")]
        Address CreateAddress(Address address);

        [WebInvoke(UriTemplate = "address/{idStr}")]
        [OperationContract]
        Address UpdateAddress(string idStr, Address address);

        [OperationContract]
        [WebInvoke(UriTemplate = "email")]
        Email CreateEmail(Email email);

        [OperationContract]
        [WebInvoke(UriTemplate = "email/{idStr}")]
        Email UpdateEmail(string idStr, Email email);

        [WebInvoke(UriTemplate = "telephone")]
        [OperationContract]
        Telephone CreateTelephone(Telephone telephone);

        [OperationContract]
        [WebInvoke(UriTemplate = "telephone/{idStr}")]
        Telephone UpdateTelephone(string idStr, Telephone telephone);

        [OperationContract]
        [WebInvoke(UriTemplate = "contact/{idStr}/delete")]
        Acknowledgement DeleteContact(string idStr);

        [OperationContract]
        [WebGet(UriTemplate = "party/{partyIdStr}/contact/{contactIdStr}")]
        PartyContact GetPartyContact(string partyIdStr, string contactIdStr);

        [OperationContract]
        [WebInvoke(UriTemplate = "party/{partyIdStr}/contact/{contactIdStr}")]
        PartyContact UpdatePartyContact(string partyIdStr, string contactIdStr, PartyContact partyContact);

        [OperationContract]
        [WebInvoke(UriTemplate = "party/{partyIdStr}/contact/{contactIdStr}/delete")]
        Acknowledgement DeletePartyContact(string partyIdStr, string contactIdStr);

        [OperationContract]
        [WebGet(UriTemplate = "party/{idStr}/contacts")]
        ContactResults GetContactsByParty(string idStr);

    }
}
