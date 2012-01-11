\c party

delete from party_contact
where  ( (party_id between 51 and 5000) or (party_id between 5051 and 10000) );

delete from address
where not exists
 (select * from party_contact 
  where party_contact.contact_id = address.contact_id);

delete from email
where not exists
 (select * from party_contact 
  where party_contact.contact_id = email.contact_id);

delete from telephone
where not exists
 (select * from party_contact 
  where party_contact.contact_id = telephone.contact_id);

delete from contact
where not exists
 (select * from party_contact
  where  party_contact.contact_id = contact.id);

delete from person
where not exists
 (select * from party_contact
  where  party_contact.party_id = person.party_id);

delete from business
where not exists
 (select * from party_contact
  where  party_contact.party_id = business.party_id);

delete from party
where not exists
 (select * from party_contact
  where  party_contact.party_id = party.id);


