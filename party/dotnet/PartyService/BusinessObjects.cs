using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace PartyService
{

    public class Party
    {
        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public string Type { get; set; }
        
        public Party() { }
        public Party(int id, string type)
        {
            Id = id; 
            Type = type;
        }
    }

    public class Person : Party
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FullName { get; set; }

        public Person() { }
        public Person(int id, string firstName, string lastName, DateTime dateOfBirth, string fullName)
            : base(id, "P")
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            FullName = fullName;
        }


    }

    public class Business : Party
    {
        public string Name { get; set; }
        public string RegNumber { get; set; }

        public Business() { }
        public Business(int id, string name, string regNumber)
            : base(id, "B")
        {
            Name = name;
            RegNumber = regNumber;
        }
    }

    public class Contact
    {
        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public string Type { get; set; }

        public Contact() { }
        public Contact(int id, string type)
        {
            Id = id;
            Type = type;
        }
    }

    public class Address : Contact
    {
        public string Street { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
        public string FullAddress { get; set; }

        public Address() { }
        public Address(int id, string street, string town, string county, string postCode, string fullAddress)
            : base(id, "A")
        {
            Street = street;
            Town = town;
            if (county != string.Empty) { County = county; }
            if (postCode != string.Empty) { PostCode = postCode; }
            if (FullAddress != string.Empty) { FullAddress = fullAddress; }
        }
    }

    public class Email : Contact
    {
        public string SubType { get; set; }
        public string Address { get; set; }

        public Email() { }
        public Email(int id, string type, string address)
            : base(id, "E")
        {
            SubType = type;
            Address = address;
        }

    }

    public class Telephone : Contact
    {
        public string SubType { get; set; }
        public string Number { get; set; }

        public Telephone() { }
        public Telephone(int id, string type, string number)
            : base(id, "T")
        {
            SubType = type;
            Number = number;
        }
    }

    public class PartyContact
    {
        [XmlAttribute]
        public int PartyId { get; set; }
        [XmlAttribute]
        public int ContactId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }


        public PartyContact() { }
        public PartyContact(int partyId, int contactId, DateTime validFrom, DateTime validUntil)
        {
            PartyId = partyId;
            ContactId = contactId;
            ValidFrom = validFrom;
            ValidUntil = validUntil;
        }
    }

    public class PartySummary
    {
        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public string Type { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }

        public PartySummary() { }
        public PartySummary(int id, string type, string name, string link)
        {
            Id = id;
            Type = type;
            Name = name;
            Link = link;
        }
    }


    public class ContactSummary
    {
        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public string Type { get; set; }
        public string Detail { get; set; }
        public string Link { get; set; }

        public ContactSummary() { }
        public ContactSummary(int id, string type, string detail, string link)
        {
            Id = id;
            Type = type;
            Detail = detail;
            Link = link;
        }

    }

    public class PartyResults
    {
        public PartyResults() { this.PartyList = new List<PartySummary>(); }

        [XmlElement("PartySummary")]
        public List<PartySummary> PartyList { get; set; }
    }

    public class ContactResults
    {
        public ContactResults() { this.ContactList = new List<ContactSummary>(); }

        [XmlElement("ContactSummary")]
        public List<ContactSummary> ContactList { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string PasswordHash { get; set; }

        public User() { }

        public User(string name, string fullName, string passwordHash)
        {
            Name = name;
            FullName = fullName;
            PasswordHash = passwordHash;
        }
    }

    public class SystemConfig
    {
        public string Name { get; set; }
        public int? IntValue { get; set; }
        public DateTime? TimestampValue { get; set; }
        public string StringValue { get; set; }

        public SystemConfig() { }

        public SystemConfig(string name, int intValue, DateTime timestampValue, string stringValue)
        {
            Name = name;
            IntValue = intValue;
            TimestampValue = timestampValue;
            StringValue = stringValue;
        }
    }

    public class Acknowledgement
    {
        public string Message { get; set; }

        public Acknowledgement() { }
        public Acknowledgement(string message)
        {
            Message = message;
        }
    }

   
    [DataContract(Namespace="")]
    public class Error
    {
        [DataMember(EmitDefaultValue=false)]
        public string Name { get; set; }
        [DataMember]
        public string Message { get; set; }

        public Error() { }
        public Error(string name, string message)
        {
            Name = name; Message = message;
        }
        public Error(string message)
        {
            Message = message;
        }
    }

 

}