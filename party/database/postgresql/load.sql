\c party
\copy party from '../data/party.txt'
\copy person from '../data/person.txt'
\copy business from '../data/business.txt'
\copy contact from '../data/contact.txt'
\copy address from '../data/address.txt'
\copy telephone from '../data/telephone.txt'
\copy email from '../data/email.txt'
\copy party_contact from '../data/party_contact.txt'

select setval('party_sequence', 1 + (select max(id) from party));
select setval('contact_sequence', 1 + (select max(id) from contact));

