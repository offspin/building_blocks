using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace PartyService
{

    public class PartyServiceObject
    {
        public Error Error { get; set; }
    }

    public class Party : PartyServiceObject
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

        public Person() { }
        public Person(int id, string firstName, string lastName, DateTime dateOfBirth)
            : base(id, "P")
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }


    }

    public class Business : Party
    {
        public string Name { get; set; }

        public Business() { }
        public Business(int id, string name)
            : base(id, "B")
        {
            Name = name;
        }
    }

    public class Contact : PartyServiceObject
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

        public Address() { }
        public Address(int id, string street, string town, string county, string postCode)
            : base(id, "A")
        {
            Street = street;
            Town = town;
            if (county != String.Empty) { County = county; }
            if (postCode != String.Empty) { PostCode = postCode; }
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

    public class PartyResults : PartyServiceObject
    {
        public PartyResults() { this.PartyList = new List<PartySummary>(); }

        [XmlElement("PartySummary")]
        public List<PartySummary> PartyList { get; set; }
    }

    public class ContactResults : PartyServiceObject
    {
        public ContactResults() { this.ContactList = new List<ContactSummary>(); }

        [XmlElement("ContactSummary")]
        public List<ContactSummary> ContactList { get; set; }
    }

    public class Acknowledgement : PartyServiceObject
    {
        public string Message { get; set; }

        public Acknowledgement() { }
        public Acknowledgement(string message)
        {
            Message = message;
        }
    }

    public class Error
    {
        public string Name { get; set; }
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

    public class PartyServiceException : Exception
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }

        public PartyServiceException() { }

        public PartyServiceException(string message)
            : base(message)
        {
            StatusCode = System.Net.HttpStatusCode.InternalServerError;
        }

        public PartyServiceException(string message, System.Net.HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

}