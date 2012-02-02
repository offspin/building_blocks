if object_id('dbo.GetParty') is not null
    drop procedure dbo.GetParty
go

print 'Procedure : GetParty'
go

create procedure dbo.GetParty
    @Id int
as
begin

    select p.id,
           p.type,
           b.name,
           pr.first_name,
           pr.last_name,
           pr.date_of_birth
    from   dbo.party as p
     left join dbo.person as pr
      on p.id = pr.party_id
     left join dbo.business as b
      on p.id = b.party_id
    where  p.id = @Id

end
go

grant execute on dbo.GetParty to public
go

if object_id('dbo.GetPartyByName') is not null
    drop procedure dbo.GetPartyByName
go

print 'Procedure : GetPartyByName'
go

create procedure dbo.GetPartyByName
    @name varchar(500)
as
begin

    set nocount on

    declare @search varchar(1000)
    set @search = 
       '"' 
       + replace(ltrim(rtrim(@name)), '"', '')
       + '*"'

    select p.party_id as id,
           p.full_name as name
    from   person as p
    where  contains(*, @search)
    union
    select b.party_id as id,
           b.name
    from   business as b
    where  contains(*, @search)
    order by 2, 1


end
go

grant execute on dbo.GetPartyByName to public
go

if object_id('dbo.CreatePerson') is not null
    drop procedure dbo.CreatePerson
go

print 'Procedure : CreatePerson'
go

create procedure dbo.CreatePerson
    @Id int output,
    @FirstName varchar(30),
    @LastName varchar(30),
    @DateOfBirth datetime
as
begin

    set nocount on

    declare @InTrans int
    declare @Error varchar(2000)

    set @InTrans = @@trancount

    begin try

        if @InTrans = 0 begin transaction

        insert into dbo.party(type) values ('P') 

        set @Id = scope_identity()

        insert into dbo.person
        (party_id, first_name, last_name, date_of_birth)
        values (@Id, @FirstName, @LastName, @DateOfBirth)

        if @InTrans = 0 commit

    end try

    begin catch

        set @Error = error_message()
        if @InTrans = 0 rollback
        set @Id = null
        raiserror(@Error, 16, 1)

    end catch

end
go

grant execute on dbo.CreatePerson to public
go

if object_id('dbo.UpdatePerson') is not null
    drop procedure dbo.UpdatePerson
go

print 'Procedure : UpdatePerson'
go

create procedure dbo.UpdatePerson
    @Id int,
    @FirstName varchar(30),
    @LastName varchar(30),
    @DateOfBirth datetime
as
begin

    update dbo.person
    set    first_name = @FirstName,
           last_name = @LastName,
           date_of_birth = @DateOfBirth
    where  party_id = @Id

end
go

grant execute on dbo.UpdatePerson to public
go

if object_id('dbo.CreateBusiness') is not null
    drop procedure dbo.CreateBusiness
go

print 'Procedure : CreateBusiness'
go

create procedure dbo.CreateBusiness
    @Id int output,
    @Name varchar(60)
as
begin

    set nocount on

    declare @InTrans int
    declare @Error varchar(2000)

    set @InTrans = @@trancount

    begin try

        if @InTrans = 0 begin transaction

        insert into dbo.party(type) values ('P') 

        set @Id = scope_identity()

        insert into dbo.business
        (party_id, name)
        values (@Id, @Name)

        if @InTrans = 0 commit

    end try

    begin catch

        set @Error = error_message()
        if @InTrans = 0 rollback
        set @Id = null
        raiserror(@Error, 16, 1)

    end catch

end
go

grant execute on dbo.CreateBusiness to public
go

if object_id('dbo.UpdateBusiness') is not null
    drop procedure dbo.UpdateBusiness
go

print 'Procedure : UpdateBusiness'
go

create procedure dbo.UpdateBusiness
    @Id int,
    @Name varchar(60)
as
begin

    update dbo.business
    set    name = @Name
    where  party_id = @Id

end
go

grant execute on dbo.UpdateBusiness to public
go

if object_id('dbo.DeleteParty') is not null
    drop procedure dbo.DeleteParty
go

print 'Procedure : DeleteParty'
go

create procedure dbo.DeleteParty
    @Id int
as
begin

    set nocount on

    declare @Error varchar(2000)
    declare @InTrans int
    set @InTrans = @@trancount

    begin try

        if @InTrans = 0 begin transaction

        delete from dbo.party_contact
        where  party_id = @Id

        delete from dbo.person
        where  party_id = @Id

        delete from dbo.business 
        where  party_id = @Id

        delete from dbo.party
        where  id = @Id

        if @InTrans = 0 commit

    end try
    begin catch

        set @Error = error_message()
        if @InTrans = 0 rollback
        raiserror(@Error, 16, 1)

    end catch

end
go

grant execute on dbo.DeleteParty to public
go

if object_id('dbo.GetContact') is not null
    drop procedure dbo.GetContact
go

print 'Procedure : GetContact'
go

create procedure dbo.GetContact
    @Id int
as
begin

    select c.id,
           c.type,
           coalesce(t.type, e.type, c.type) as sub_type,
           a.street,
           a.town,
           a.county,
           a.post_code,
           t.number as telephone_number,
           e.address as email_address
    from   dbo.contact as c
     left join dbo.address as a
       on c.id = a.contact_id
     left join dbo.email as e
      on c.id = e.contact_id
     left join dbo.telephone as t
      on c.id = t.contact_id
    where  c.id = @Id

end
go

grant execute on dbo.GetContact to public
go

if object_id('dbo.GetContactByPartyId') is not null
    drop procedure dbo.GetContactByPartyId
go

print 'Procedure : GetContactByPartyId'
go

create procedure dbo.GetContactByPartyId
    @PartyId int
as
begin

    select c.id,
           c.type,
           c.type as sub_type,
           a.full_address as detail,
           pc.valid_from,
           pc.valid_until
    from   dbo.contact as c
     inner join dbo.address as a
      on c.id = a.contact_id
     inner join dbo.party_contact as pc
      on c.id = pc.contact_id	 
      where pc.party_id = @PartyId
     union
     select c.id,
            c.type,
            t.type as sub_type,
            t.number as detail,
            pc.valid_from,
            pc.valid_until
     from   dbo.contact as c
      inner join dbo.telephone as t
       on c.id = t.contact_id
      inner join dbo.party_contact as pc
      on c.id = pc.contact_id
     where  pc.party_id = @PartyId
     union
     select c.id,
            c.type,
            e.type as sub_type,
            e.address as detail,
            pc.valid_from,
            pc.valid_until
     from   dbo.contact as c
      inner join dbo.email as e
       on c.id = e.contact_id
      inner join dbo.party_contact as pc
       on c.id = pc.contact_id
     where  pc.party_id = @PartyId
     order by 2,3,1

end
go

grant execute on dbo.GetContactByPartyId to public
go

if object_id('dbo.CreateAddress') is not null
    drop procedure dbo.CreateAddress
go

print 'Procedure : CreateAddress'
go

create procedure dbo.CreateAddress
    @Id int output,
    @Street varchar(50),
    @Town varchar(50),
    @County varchar(30),
    @PostCode varchar(20)
as
begin

    set nocount on

    declare @InTrans int
    declare @Error varchar(2000)

    begin try

        if @InTrans = 0 begin transaction

        insert into dbo.contact(type) values ('A')

        set @Id = scope_identity()

        insert into dbo.address
        (contact_id, street, town, county, post_code)
        values(@Id, @Street, @Town, @County, @PostCode)

        if @InTrans = 0 commit

    end try
    begin catch

        set @Error = error_message()
        if @InTrans = 0 rollback
        set @Id = null
        raiserror(@Error, 16, 1)

    end catch

end
go

grant execute on dbo.CreateAddress to public
go

if object_id('dbo.UpdateAddress') is not null
    drop procedure dbo.UpdateAddress
go

create procedure dbo.UpdateAddress
    @Id int,
    @Street varchar(50),
    @Town varchar(50),
    @County varchar(30),
    @PostCode varchar(20)
as
begin

    update dbo.address
    set    street = @Street,
           town = @Town,
           county = @County,
           post_code = @PostCode
    where  contact_id = @Id

end
go

grant execute on dbo.UpdateAddress to public
go

if object_id('dbo.CreateTelephone') is not null
    drop procedure dbo.CreateTelephone
go

print 'Procedure : CreateTelephone'
go

create procedure dbo.CreateTelephone
    @Id int output,
    @Type char(1),
    @Number varchar(50)
as
begin

    set nocount on

    declare @Error varchar(2000)
    declare @InTrans int
    set @InTrans = @@trancount

    begin try

        if @InTrans = 0 begin transaction

        insert into dbo.contact(type) values ('T')

        set @Id = scope_identity()

        insert into dbo.telephone(contact_id, type, number) 
        values(@Id, @Type, @Number)

    end try
    begin catch

        set @Error = error_message()
        if @InTrans = 0 rollback
        set @Id = null
        raiserror(@Error, 16, 1)

    end catch

end
go

grant execute on dbo.CreateTelephone to public
go

if object_id('dbo.UpdateTelephone') is not null
    drop procedure dbo.UpdateTelephone
go

print 'Procedure : UpdateTelephone'
go

create procedure dbo.UpdateTelephone
    @Id int,
    @Type char(1),
    @Number varchar(50)
as
begin

    update dbo.telephone
    set    type = @Type,
           number = @Number
    where  contact_id = @Id

end
go

grant execute on dbo.UpdateTelephone to public
go

if object_id('dbo.CreateEmail') is not null
    drop procedure dbo.CreateEmail
go

print 'Procedure : CreateEmail'
go

create procedure dbo.CreateEmail
    @Id int output,
    @Type char(1),
    @Address varchar(100)
as
begin

    set nocount on 
    declare @Error varchar(2000)
    declare @InTrans int

    set @InTrans = @@trancount

    begin try

        if @InTrans = 0 begin transaction

        insert into dbo.contact(type) values ('E')

        set @Id = scope_identity()

        insert into dbo.email
        (contact_id, type, address)
        values
        (@Id, @Type, @Address)

        if @InTrans = 0 commit

    end try
    begin catch

        set @Error = error_message()
        if @InTrans = 0 rollback
        set @Id = null
        raiserror(@Error, 16, 1)

    end catch

end
go

grant execute on dbo.CreateEmail to public
go

if object_id('dbo.DeleteContact') is not null
    drop procedure dbo.DeleteContact
go

print 'Procedure : DeleteContact'
go

create procedure dbo.DeleteContact
    @Id int
as
begin

    set nocount on

    declare @Error varchar(2000)
    declare @InTrans int

    set @InTrans = @@trancount

    begin try

        if exists
         (select * from dbo.party_contact
          where  contact_id = @Id)
            raiserror('Contact is in use', 16, 1)

        if @InTrans = 0 begin transaction

        delete from dbo.address
        where  contact_id = @Id

        delete from dbo.email
        where  contact_id = @Id

        delete from dbo.telephone
        where  contact_id = @Id

        delete from dbo.contact
        where  id = @Id

        if @InTrans = 0 commit

    end try
    begin catch

        set @Error = error_message()
        if @InTrans = 0 rollback
        raiserror(@Error, 16, 1)

    end catch

end
go

grant execute on dbo.DeleteContact to public
go

if object_id('dbo.GetPartyContact') is not null
    drop procedure dbo.GetPartyContact
go

print 'Procedure : GetPartyContact'
go

create procedure dbo.GetPartyContact
    @PartyId int,
    @ContactId int
as
begin

    select party_id, contact_id, valid_from, valid_until
    from   dbo.party_contact
    where  party_id = @PartyId
    and    contact_id = @ContactId

end
go

grant execute on dbo.GetPartyContact to public
go

if object_id('dbo.CreatePartyContact') is not null
    drop procedure dbo.CreatePartyContact
go

print 'Procedure : CreatePartyContact'
go

create procedure dbo.CreatePartyContact
    @PartyId int,
    @ContactId int,
    @ValidFrom datetime,
    @ValidUntil datetime
as
begin

    insert into dbo.party_contact
    (party_id, contact_id, valid_from, valid_until)
    values
    (@PartyId, @ContactId, @ValidFrom, @ValidUntil)

end
go

grant execute on dbo.CreatePartyContact to public
go

if object_id('dbo.UpdatePartyContact') is not null
    drop procedure dbo.UpdatePartyContact
go

print 'Procedure : UpdatePartyContact'
go

create procedure dbo.UpdatePartyContact
    @PartyId int,
    @ContactId int,
    @ValidFrom datetime,
    @ValidUntil datetime
as
begin

    update dbo.party_contact
    set    valid_from = @ValidFrom,
           valid_until = @ValidUntil
    where  party_id = @PartyId
    and    contact_id = @ContactId

end
go

grant execute on dbo.UpdatePartyContact to public
go

if object_id('dbo.DeletePartyContact') is not null
    drop procedure dbo.DeletePartyContact
go

print 'Procedure : DeletePartyContact'
go

create procedure dbo.DeletePartyContact 
    @PartyId int,
    @ContactId int
as
begin

    delete from dbo.party_contact
    where  party_id = @PartyId
    and    contact_id = @ContactId

end
go

grant execute on dbo.DeletePartyContact to public
go

if object_id('dbo.GetUser') is not null
    drop procedure dbo.GetUser
go

print 'Procedure : GetUser'
go

create procedure dbo.GetUser
    @Name varchar(20)
as
begin

    select name, full_name, password_hash
    from   dbo.user_of_system
    where  name = @name

end
go

grant execute on dbo.GetUser to public
go

if object_id('dbo.GetSystemConfig') is not null
    drop procedure dbo.GetSystemConfig
go

print 'Procedure : GetSystemConfig'
go

create procedure dbo.GetSystemConfig
    @Name varchar(50)
as
begin

    select name, int_value, timestamp_value, string_value
    from   dbo.system_config
    where  name = @Name

end
go

grant execute on dbo.GetSystemConfig to public
go

if object_id('dbo.CreateUser') is not null
    drop procedure dbo.CreateUser
go

print 'Procedure : CreateUser'
go

create procedure dbo.CreateUser
    @Name varchar(20),
    @FullName varchar(50),
    @PasswordHash varchar(500)
as
begin

    insert into dbo.user_of_system
    (name, full_name, password_hash)
    values
    (@Name, @FullName, @PasswordHash)

end
go

grant execute on dbo.CreateUser to public
go

if object_id('dbo.UpdateUser') is not null
    drop procedure dbo.UpdateUser
go

print 'Procedure : UpdateUser'
go

create procedure dbo.UpdateUser
    @Name varchar(20),
    @FullName varchar(50),
    @PasswordHash varchar(500)
as
begin

    update dbo.user_of_system
    set    full_name = @FullName,
           password_hash = @PasswordHash
    where  name = @Name

end
go

grant execute on dbo.UpdateUser to public
go

if object_id('dbo.DeleteUser') is not null
    drop procedure dbo.DeleteUser
go

print 'Procedure : DeleteUser'
go

create procedure dbo.DeleteUser
    @Name varchar(20)
as
begin

    delete from dbo.user_of_system
    where  name = @Name

end
go

grant execute on dbo.DeleteUser to public
go




