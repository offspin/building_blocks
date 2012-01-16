require 'rubygems'
require 'pg'

def connect_db

    if (ENV['DATABASE_URL'])
	dbu = URI.parse(ENV['DATABASE_URL'])
	db = PGconn.new(:host => dbu.host, :user => dbu.user, :password => dbu.password, :dbname => dbu.path[1..-1])
    else
	db = PGconn.new(:dbname => 'party')
    end

end

def get_party(db, id)

    sql = <<-EOS
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
	where  p.id = $1;
    EOS

    res = db.exec(sql, [ id ])

    pty = res.first

end

def get_party_byname(db, name)

    sql = <<-EOS
       select p.party_id as id,
              p.full_name as name
       from   person as p
       where (
        ( to_tsvector('english', p.first_name) @@ plainto_tsquery('english', $1 ) )
	or
        ( to_tsvector('english', p.last_name) @@ plainto_tsquery('english', $1 ) )
        or
        ( to_tsvector('english', p.full_name) @@ plainto_tsquery('english', $1 ) )
      )
      union
      select b.party_id, b.name
      from   business as b
      where  to_tsvector('english', b.name) @@ plainto_tsquery('english', $1)
      order by 2, 1 ;
    EOS

    res = db.exec(sql, [name])

    ptys = []

    res.each do |r|
	ptys << r
    end
end

def create_person(db, first_name, last_name, date_of_birth)

    party_sql = <<-EOS
        insert into party(type) values ('P');
    EOS

    person_sql = <<-EOS
	insert into person
	(party_id, first_name, last_name, 
	 date_of_birth, full_name)
	select lastval(), $1, $2, $3, $4
	   returning party_id;
    EOS

    party_id = nil

    db.transaction do
        db.exec party_sql
	res = db.exec(person_sql, [first_name, last_name, date_of_birth, [first_name, last_name].join(' ')]) 
        party_id = res.first['party_id'] if res.first
    end 

    return party_id.to_i

end

def update_person(db, party_id, first_name, last_name, date_of_birth)

    sql = <<-EOS
        update person
	set    first_name = $2,
	       last_name = $3,
	       date_of_birth = $4,
	       full_name = $5
	where  party_id = $1;
    EOS

    db.exec(sql, [party_id, first_name, last_name, date_of_birth, [first_name, last_name].join(' ')])

end

def create_business(db, name)

    party_sql = <<-EOS
        insert into party(type) values('P');
    EOS

    business_sql = <<-EOS
        insert into business
	(party_id, name)
	select lastval(), $1
	   returning party_id;
    EOS

    party_id = nil

    db.transaction do
	db.exec party_sql
	res = db.exec(business_sql, [name])
	party_id = res.first['party_id'] if res.first
    end
    
    return party_id.to_i

end

def update_business(db, party_id, name)

    sql = <<-EOS
        update business
	set    name = $2
	where  party_id = $1;
    EOS

    db.exec(sql, [party_id, name])

end

def delete_party(db, party_id)

    party_contact_sql = <<-EOS
        delete from party_contact
	where  party_id = $1;
    EOS

    person_sql = <<-EOS
        delete from person
	where  party_id = $1;
    EOS

    business_sql = <<-EOS
        delete from business
	where  party_id = $1;
    EOS

    party_sql = <<-EOS
        delete from party
	where  id = $1;
    EOS

    db.transaction do

	db.exec(party_contact_sql, [party_id])
	db.exec(person_sql, [party_id])
	db.exec(business_sql, [party_id])
	db.exec(party_sql, [party_id])

    end

end

def get_contact(db, id)

    sql = <<-EOS
        select c.id,
	       c.type,
	       coalesce(t.type, e.type, c.type) as sub_type,
	       a.street,
	       a.town,
	       a.county,
	       a.post_code,
	       t.number as telephone_number,
	       e.address as email_address
	from   contact as c
	 left join address as a
	  on c.id = a.contact_id
	 left join email as e
	  on c.id = e.contact_id
	 left join telephone as t
	  on c.id = t.contact_id
	where  c.id = $1;
    EOS

    res = db.exec(sql, [id]);

    cont = res.first

end

def get_contact_bypartyid(db, party_id)
    sql = <<-EOS
        select c.id,
	       c.type,
	       c.type as sub_type,
	       a.full_address as detail,
	       pc.valid_from,
	       pc.valid_until
	from   contact as c
	 inner join address as a
	  on c.id = a.contact_id
	 inner join party_contact as pc
	  on c.id = pc.contact_id	 
	  where pc.party_id = $1
	 union
	 select c.id,
	        c.type,
		t.type as sub_type,
		t.number as detail,
		pc.valid_from,
		pc.valid_until
	 from   contact as c
	  inner join telephone as t
	   on c.id = t.contact_id
	  inner join party_contact as pc
	  on c.id = pc.contact_id
	 where  pc.party_id = $1
	 union
	 select c.id,
	        c.type,
		e.type as sub_type,
		e.address as detail,
		pc.valid_from,
		pc.valid_until
	 from   contact as c
	  inner join email as e
	   on c.id = e.contact_id
	  inner join party_contact as pc
	   on c.id = pc.contact_id
	 where  pc.party_id = $1
	 order by 2,3,1;
    EOS

    res = db.exec(sql, [party_id])

    conts = []

    res.each do |c|
	conts << c
    end

end

def create_address(db, street, town, county, post_code)

    contact_sql = <<-EOS
        insert into contact(type) values ('A')
    EOS

    address_sql = <<-EOS
        insert into address
	(contact_id, street, town, county, post_code)
	select lastval(), $1, $2, $3, $4
	   returning contact_id;
    EOS

    contact_id = nil

    db.transaction do
	db.exec contact_sql
	res = db.exec(address_sql, [street, town, county, post_code])
	contact_id = res.first['contact_id'] if res.first
    end
    
    return contact_id.to_i

end

def update_address(db, contact_id, street, town, county, post_code)

    sql = <<-EOS
        update address
	set    street = $2,
	       town = $3,
	       county = $4,
	       post_code = $5
	where  contact_id = $1;
    EOS

    db.exec(sql, [contact_id, street, town, county, post_code])

end

def delete_address(db, contact_id)

    address_sql = <<-EOS
        delete from address
	where  contact_id = $1;
    EOS

    contact_sql = <<-EOS
        delete from contact
	where  id = $1;
    EOS

    db.transaction do
	db.exec(address_sql, [contact_id])
	db.exec(contact_sql, [contact_id])
    end

end

def create_telephone(db, number, type)

    contact_sql = <<-EOS
        insert into contact(type) values ('T')
    EOS

    telephone_sql = <<-EOS
        insert into telephone
	(contact_id, number, type)
	select lastval(), $1, $2
	   returning contact_id;
    EOS

    contact_id = nil

    db.transaction do
	db.exec contact_sql
	res = db.exec(telephone_sql, [number, type])
	contact_id = res.first['contact_id'] if res.first
    end
    
    return contact_id.to_i

end

def create_email(db, address, type)

    contact_sql = <<-EOS
        insert into contact(type) values ('E')
    EOS

    email_sql = <<-EOS
        insert into email
	(contact_id, address, type)
	select lastval(), $1, $2
	   returning contact_id;
    EOS

    contact_id = nil

    db.transaction do
	db.exec contact_sql
	res = db.exec(email_sql, [address, type])
	contact_id = res.first['contact_id'] if res.first
    end
    
    return contact_id.to_i

end

def delete_contact(db, contact_id)

    address_sql = <<-EOS
        delete from address
        where  contact_id = $1;
    EOS

    email_sql = <<-EOS
        delete from email
        where  contact_id = $1;
    EOS

    telephone_sql = <<-EOS
        delete from telephone
        where  contact_id = $1;
    EOS

    contact_sql = <<-EOS
        delete from contact
        where  id = $1;
    EOS

    db.transaction do
        db.exec(address_sql, [contact_id])
        db.exec(email_sql, [contact_id])
        db.exec(telephone_sql, [contact_id])
        db.exec(contact_sql, [contact_id])
    end

end

def get_party_contact(db, party_id, contact_id)

    sql = <<-EOS
        select party_id, contact_id, valid_from, valid_until
        from   party_contact
        where  party_id = $1 and contact_id = $2;
    EOS

    res = db.exec(sql, [party_id, contact_id])

    pc = res.first

end

def create_party_contact(db, party_id, contact_id, valid_from, valid_until)

    sql = <<-EOS
        insert into party_contact
	(party_id, contact_id, valid_from, valid_until)
	values ($1, $2, $3, $4);
    EOS

    db.exec(sql, [party_id, contact_id, valid_from, valid_until])

end

def update_party_contact(db, party_id, contact_id, valid_from, valid_until)

    sql = <<-EOS
        update party_contact
	set    valid_from = $3,
	       valid_until = $4
	where  party_id = $1
	and    contact_id = $2;
    EOS

    db.exec(sql, [party_id, contact_id, valid_from, valid_until])

end


def delete_party_contact(db, party_id, contact_id)

    sql = <<-EOS
        delete from party_contact
	where  party_id = $1
	and    contact_id = $2;
    EOS

    db.exec(sql, [party_id, contact_id])

end

def get_system_config(db, name)

    sql = <<-EOS
        select name, int_value, timestamp_value, string_value
	from   system_config
	where  name = $1;
    EOS

    res = db.exec(sql, [name])

    sc = res.first

end

def get_user(db, name)

    sql = <<-EOS
        select name, full_name, password_hash
	from   user_of_system
	where  name = $1;
    EOS

    res = db.exec(sql, [name])

    ph = res.first

end

def create_user(db, name, full_name, password_hash)

    sql = <<-EOS
        insert into user_of_system
	(name, full_name, password_hash)
	values($1, $2, $3);
    EOS

    db.exec(sql, [name, full_name, password_hash])

end

def update_user(db, name, full_name, password_hash)

    sql = <<-EOS
        update user_of_system
	set    full_name = $2,
	       password_hash = $3
	where  name = $1;
    EOS

    db.exec(sql, [name, full_name, password_hash])

end


def delete_user(db, name)
    
    sql = <<-EOS
        delete from user_of_system
	where  name = $1; 
    EOS

    db.exec(sql, [name])

end


