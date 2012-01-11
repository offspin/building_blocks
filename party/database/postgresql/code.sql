----------------------------------------------------------------------
-- Code for PostgreSQL party database.
----------------------------------------------------------------------

\c party

----------------------------------------------------------------------
-- Trigger to maintain full name
----------------------------------------------------------------------
drop trigger if exists person_full_name_tr on person;

create or replace function person_full_name_tf() 
   returns trigger
as 
$$
begin

    NEW.full_name = 
       NEW.first_name || ' ' || NEW.last_name;

    return NEW;

end;
$$
language plpgsql;

create trigger person_full_name_tr 
    before insert or update on person
for each row execute procedure person_full_name_tf();


----------------------------------------------------------------------
-- Trigger to maintain full address
----------------------------------------------------------------------
drop trigger if exists address_full_address_tr on address;

create or replace function address_full_address_tf() 
   returns trigger
as 
$$
begin

    NEW.full_address = 
       NEW.street || ' ' || NEW.town 
    || coalesce(' ' || NEW.county, '')
    || coalesce(' ' || NEW.post_code, '');

    return NEW;

end;
$$
language plpgsql;

create trigger address_full_address_tr 
    before insert or update on address
for each row execute procedure address_full_address_tf();


----------------------------------------------------------------------
-- Search party by name
----------------------------------------------------------------------
drop function if exists search_party(varchar);

create or replace function search_party
    (party_name varchar(150))
    returns table
    (
	id integer,
	name varchar(150)
    )
as
$$

declare 
    name_tsq tsquery;

begin

    name_tsq := plainto_tsquery('english', party_name);

    return query
        select p.party_id,
	       p.full_name
	from   person as p
	where  (
	    ( to_tsvector('english', p.first_name) @@ name_tsq )
	    or
	    ( to_tsvector('english', p.last_name) @@ name_tsq )
	    or
	    ( to_tsvector('english', p.full_name) @@ name_tsq )
	)
	union
	select b.party_id, b.name
	from   business as b
	where  to_tsvector('english', b.name) @@ name_tsq 
	order by 2,1 ;
end;
$$
language plpgsql;

----------------------------------------------------------------------
-- Get party by id
----------------------------------------------------------------------
drop function if exists get_party_byid(integer);

create or replace function get_party_byid
    (p_id integer)
    returns table
    (
	id integer,
	type char(1),
	name varchar(150),
	first_name varchar(30),
	last_name varchar(30),
	date_of_birth date
    )
as
$$
begin

    return query
        select p.id,
	       p.type,
	       b.name,
	       pr.first_name,
	       pr.last_name,
	       pr.date_of_birth
	from   party as p
	 left join person as pr
	  on p.id = pr.party_id
	 left join business as b
	  on p.id = b.party_id
	where p.id = p_id;

end;
$$
language plpgsql;

----------------------------------------------------------------------
-- Get contacts for party id
----------------------------------------------------------------------
drop function if exists get_contact_bypartyid(integer);

create or replace function get_contact_bypartyid
    (p_party_id integer)
    returns table
    (
	id integer,
	type char(1),
	sub_type char(1),
	detail varchar(200),
	valid_from date,
	valid_until date
    )
as
$$
begin

    return query
        select c.id,
	       c.type,
	       c.type,
	       a.full_address,
	       pc.valid_from,
	       pc.valid_until
	from   contact as c
	 inner join address as a
	  on c.id = a.contact_id
	 inner join party_contact as pc
	  on c.id = pc.contact_id
	where  pc.party_id = p_party_id
	union
	select c.id,
	       c.type,
	       t.type,
	       t.number,
	       pc.valid_from,
	       pc.valid_until
	from   contact as c
	 inner join telephone as t
	  on c.id = t.contact_id
	 inner join party_contact as pc
	  on c.id = pc.contact_id
	where  pc.party_id = p_party_id
	union
	select c.id,
	       c.type,
	       e.type,
	       e.address,
	       pc.valid_from,
	       pc.valid_until
	from   contact as c
	 inner join email as e
	  on c.id = e.contact_id
	 inner join party_contact as pc
	  on c.id = pc.contact_id
	where  pc.party_id = p_party_id
	order by 2,3,1;

end;
$$ language plpgsql;

----------------------------------------------------------------------
-- Return a single contact
----------------------------------------------------------------------
drop function if exists get_contact_byid(integer);

create or replace function get_contact_byid
    (p_id integer)
    returns table
    (
	id integer,
	type char(1),
	sub_type char(1),
	street varchar(50),
	town varchar(50),
	county varchar(50),
	post_code varchar(20),
	telephone_number varchar(50),
	email_address varchar(100)
    )
as
$$
begin

    return query
        select c.id,
	       c.type,
	       coalesce(t.type, e.type, 'A'), 
	       a.street,
	       a.town,
	       a.county,
	       a.post_code,
	       t.number,
	       e.address
	from   contact as c
	 left join address as a
	  on c.id = a.contact_id
	 left join email as e
	  on c.id = e.contact_id
	 left join telephone as t
	  on c.id = t.contact_id
	where c.id = p_id;

end;
$$ language plpgsql;

	       

