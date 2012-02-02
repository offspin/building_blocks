if db_id('party') is not null
begin

    declare @kill varchar(50)
    declare cod cursor for
        select 'kill ' + cast(spid as varchar(10))
        from   master..sysprocesses
        where  dbid = db_id('party')
        for read only
    
    open cod

    fetch next from cod into @kill

    while @@fetch_status = 0
    begin

        exec(@kill)
        fetch next from cod into @kill

    end
    
    close cod deallocate cod

    drop database party

end
go


create database party
go

use party
go

create fulltext catalog partyFTC
  with accent_sensitivity = off
  as default
go

create table dbo.party
(
    id integer identity,
    type char(1) not null,
    constraint pk_party primary key(id),
    constraint ck_party_type check(type in ('B','P'))
)
go



create table dbo.person
(
    party_id integer not null,
    first_name varchar(30) not null,
    last_name varchar(30) not null,
    date_of_birth date not null,
    full_name 
      as first_name + ' ' + last_name,
    constraint pk_person primary key(party_id),
    constraint ak_person unique(first_name, last_name, date_of_birth),
    constraint fk_person_party 
      foreign key(party_id) references party(id)
)
go


create fulltext index on dbo.person
  (first_name, last_name, full_name)
   key index pk_person
go

create table dbo.business
(
    party_id integer not null,
    name varchar(60) not null,
    constraint pk_business primary key(party_id),
    constraint ak_business unique(name),
    constraint fk_business_party 
       foreign key(party_id) references party(id)
)
go

create fulltext index on dbo.business
    (name)
    key index pk_business
go

create table dbo.contact
(
    id integer identity,
    type char(1) not null,
    constraint pk_contact primary key(id),
    constraint ck_contact_type check(type in ('A','E','T'))
)
go



create table dbo.address
(
    contact_id integer not null,
    street varchar(50) not null,
    town varchar(50) not null,
    county varchar(30) null,
    post_code varchar(20) null,
    full_address 
      as street + ' ' + town 
        + coalesce(' ' + county, '')
        + coalesce(' ' + post_code, ''),
    constraint pk_address primary key(contact_id),
    constraint ak_address unique(street, town, county, post_code),
    constraint fk_address_contact
       foreign key(contact_id) references contact(id)
)
go

create fulltext index on dbo.address
   (street, town, county, post_code, full_address)
    key index pk_address
go

create table dbo.email
(
    contact_id integer not null,
    address varchar(100) not null,
    type char(1) not null,
    constraint pk_email primary key(contact_id),
    constraint ak_email unique(address, type),
    constraint ck_email_type check(type in ('B','H')),
    constraint fk_email_contact
       foreign key(contact_id) references contact(id)
)
go

create table dbo.telephone
(
    contact_id integer not null,
    number varchar(50) not null,
    type char(1) not null,
    constraint pk_telephone primary key(contact_id),
    constraint ak_telephone unique(number, type),
    constraint ck_telephone_type check(type in ('B','H','M')),
    constraint fk_telephone_contact 
       foreign key(contact_id) references contact(id)
)
go



create table dbo.party_contact
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
)
go



create table dbo.user_of_system
(
    name varchar(20) not null,
    full_name varchar(50) not null,
    password_hash varchar(500) not null,
    constraint pk_user_of_system primary key(name)
)
go



create table dbo.system_config
(
    name varchar(50) not null,
    int_value integer null,
    timestamp_value timestamp null,
    string_value varchar(500) null,
    constraint pk_system_config primary key(name)
)
go

insert into dbo.system_config(name, string_value) values ('REALM', 'Party Service')
go

insert into dbo.system_config(name, string_value) values ('OPAQUE', 'partypizza')
go

insert into dbo.user_of_system(name, full_name, password_hash)
select 'admin', 'Administrator', 
       lower(convert(varchar(500), hashbytes('md5', 'admin:' + string_value + ':admin'), 2))
from   dbo.system_config
where  name = 'REALM'
go

