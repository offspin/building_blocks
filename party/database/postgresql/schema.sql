drop database if exists party;

create database party;

\c party

create sequence party_sequence;

create table party
(
    id integer not null default nextval('party_sequence'),
    type char(1) not null,
    constraint pk_party primary key(id),
    constraint ck_party_type check(type in ('B','P'))
);

create table person
(
    party_id integer not null,
    first_name varchar(30) not null,
    last_name varchar(30) not null,
    date_of_birth date not null,
    full_name varchar(70) null,
    constraint pk_person primary key(party_id),
    constraint ak_person unique(first_name, last_name, date_of_birth),
    constraint fk_person_party 
      foreign key(party_id) references party(id)
);

create index person_first_name_fti on person 
   using gin(to_tsvector('english', first_name));

create index person_last_name_fti on person 
   using gin(to_tsvector('english', last_name));

create index person_full_name_fti on person 
   using gin(to_tsvector('english', full_name));

create table business
(
    party_id integer not null,
    name varchar(60) not null,
    constraint pk_business primary key(party_id),
    constraint ak_business unique(name),
    constraint fk_business_party 
       foreign key(party_id) references party(id)
);

create index business_name_fti on business
   using gin(to_tsvector('english', name));


create sequence contact_sequence;

create table contact
(
    id integer not null default nextval('contact_sequence'),
    type char(1) not null,
    constraint pk_contact primary key(id),
    constraint ck_contact_type check(type in ('A','E','T'))
);

create table address
(
    contact_id integer not null,
    street varchar(50) not null,
    town varchar(50) not null,
    county varchar(30) null,
    post_code varchar(20) null,
    full_address varchar(160) null,
    constraint pk_address primary key(contact_id),
    constraint fk_address_contact
       foreign key(contact_id) references contact(id)
);

create index address_street_fti on address
   using gin(to_tsvector('english', street));

create index address_town_fti on address
   using gin(to_tsvector('english', town));

create index address_county_fti on address
   using gin(to_tsvector('english', county));

create index address_post_code_fti on address
   using gin(to_tsvector('english', post_code));

create index address_full_address_fti on address
   using gin(to_tsvector('english', full_address));

create table email
(
    contact_id integer not null,
    address varchar(100) not null,
    type char(1) not null,
    constraint pk_email primary key(contact_id),
    constraint ak_email unique(address, type),
    constraint ck_email_type check(type in ('B','H')),
    constraint fk_email_contact
       foreign key(contact_id) references contact(id)
);

create table telephone
(
    contact_id integer not null,
    number varchar(50) not null,
    type char(1) not null,
    constraint pk_telephone primary key(contact_id),
    constraint ak_telephone unique(number, type),
    constraint ck_telephone_type check(type in ('B','H','M')),
    constraint fk_telephone_contact 
       foreign key(contact_id) references contact(id)
);

create table party_contact
(
    party_id integer not null,
    contact_id integer not null,
    valid_from date not null,
    valid_until date not null default '3000-01-01',
    constraint pk_party_contact primary key(party_id, contact_id, valid_from),
    constraint ck_party_contact_valid_order
       check(valid_from <= valid_until),
    constraint fk_party_contact_party
       foreign key(party_id) references party(id),
    constraint fk_party_contact_contact
       foreign key(contact_id) references contact(id)
);

create table user_of_system
(
    name varchar(20) not null,
    full_name varchar(50) not null,
    password_hash varchar(500) not null,
    constraint pk_user_of_system primary key(name)
);

create table system_config
(
    name varchar(50) not null,
    int_value integer null,
    timestamp_value timestamp null,
    string_value varchar(500) null,
    constraint pk_system_config primary key(name)
);

insert into system_config(name, string_value) values ('REALM', 'Party Service');
insert into system_config(name, string_value) values ('OPAQUE', 'partypizza');

insert into user_of_system(name, full_name, password_hash)
select 'admin', 'Administrator',
       md5('admin:' || string_value || ':admin')
from   system_config
where  name = 'REALM';
