require './pgdb'


describe Party::PostgresDatabase do

    person_party_id = 0
    business_party_id = 0
    address_contact_id = 0
    home_telephone_contact_id = 0
    business_telephone_contact_id = 0
    mobile_telephone_contact_id = 0
    home_email_contact_id = 0
    business_email_contact_id = 0

    before :each do
        @db = Party::PostgresDatabase.new
    end

    it 'should be connected' do
	@db.connection.status.should == PGconn::CONNECTION_OK
    end

    it 'should create a person' do
	person_party_id = @db.create_person('First', 'Last', '2000-03-05')
	person_party_id.should be > 0
    end

    it 'should find the person created by id' do
	res = @db.get_party(person_party_id)
	res['first_name'].should == 'First'
	res['last_name'].should == 'Last'
	res['date_of_birth'].should == '2000-03-05'
    end

    it 'should find the person created by name' do
	found = false
	res = @db.get_party_byname('first last')
	res.each do |r|
	   if r['id'].to_i == person_party_id
	       found = true
	       break
	   end
	end
	found.should == true
    end

    it 'should update the person created' do
	@db.update_person(person_party_id, 'Firster', 'Laster', '2000-04-06')
	res = @db.get_party(person_party_id)
	res['first_name'].should == 'Firster'
	res['last_name'].should == 'Laster'
	res['date_of_birth'].should == '2000-04-06'
    end

    it 'should create an address' do
	address_contact_id = @db.create_address('48 Pleasant Crescent', 'Royston Vasey', 'Hertfordshire', 'SG8 1AB')
	address_contact_id.should be > 0
    end

    it 'should create a home telephone number' do
	home_telephone_contact_id = @db.create_telephone('01234 567890', 'H')
	home_telephone_contact_id.should be > 0
    end

    it 'should create a business telephone number' do
	business_telephone_contact_id = @db.create_telephone('01234 234567', 'B')
	business_telephone_contact_id.should be > 0
    end

    it 'should create a mobile telephone number' do
	mobile_telephone_contact_id = @db.create_telephone('07734 654321', 'M')
	mobile_telephone_contact_id.should be > 0
    end

    it 'should create a home email' do
	home_email_contact_id = @db.create_email('me@home.co.uk', 'H')
	home_email_contact_id.should be > 0
    end

    it 'should create a business email' do
	business_email_contact_id = @db.create_email('him@business.co.uk', 'B')
	business_email_contact_id.should be > 0
    end

    it 'should update an address' do
	@db.update_address(address_contact_id, '49 Down The Line', 'Royston', 'Herts', 'SG8 6QW')
	res = @db.get_contact(address_contact_id)
	res['street'].should == '49 Down The Line'
	res['town'].should == 'Royston'
	res['county'].should == 'Herts'
	res['post_code'].should == 'SG8 6QW'
    end

    it 'should link the address created to the person created' do
	@db.create_party_contact(person_party_id, address_contact_id, '2000-05-06', '2005-06-05')
        pc = @db.get_party_contact(person_party_id, address_contact_id)
        pc.should_not be nil
    end

    it 'should find the linked address in the person\'s contacts' do

	res = @db.get_contact_bypartyid(person_party_id)

	found = false
	res.each do |r|
	    if r['id'].to_i == address_contact_id
		found = true
		break
	    end
	end

	found.should == true
    end

    it 'should update the link valid dates' do
	@db.update_party_contact(person_party_id, address_contact_id, '2008-01-02', '2009-02-01')
	res = @db.get_contact_bypartyid(person_party_id)

	ok = false
	res.each do |r|
	    if r['id'].to_i == address_contact_id
		if r['valid_from'] == '2008-01-02' && r['valid_until'] == '2009-02-01'
		    ok = true
		    break
		end
	    end
	end

	ok.should == true
    end


    it 'should not be able to delete the linked address' do
	lambda do
	    @db.delete_contact(address_contact_id)
	end.should raise_exception
    end

    it 'should delete the link between address and person' do
	@db.delete_party_contact(person_party_id, address_contact_id)
	res = @db.get_contact_bypartyid(person_party_id)

	gone = true
	res.each do |r|
	    if r['id'].to_i == address_contact_id
		gone = false
		break
	    end
	end

	gone.should == true
    end

    it 'should delete the unlinked address' do
	@db.delete_address(address_contact_id)
	res = @db.get_contact(address_contact_id)
	res.should be nil
    end
	
    it 'should delete the person created' do
	@db.delete_party(person_party_id)
	res = @db.get_party(person_party_id)
	res.should be nil
    end

    it 'should create a business' do
	business_party_id = @db.create_business('The Business')
	business_party_id.should be > 0
    end

    it 'should find the business created by id' do
	res = @db.get_party(business_party_id)
	res['name'].should == 'The Business'
    end

    it 'should find the business created by name' do
	found = false
	res = @db.get_party_byname('the business')
	res.each do |r|
	    if r['id'].to_i == business_party_id
		found = true
		break
	    end
	end
	found.should == true
    end

    it 'should update the business created ' do
	@db.update_business(business_party_id, 'The Businesser')
	res = @db.get_party(business_party_id)
	res['name'].should == 'The Businesser'
    end

    it 'should delete the business created ' do
	@db.delete_party(business_party_id)
	res = @db.get_party(business_party_id)
	res.should be nil
    end

    it 'should delete remaining contacts' do

        @db.delete_contact(home_telephone_contact_id)
        res = @db.get_contact(home_telephone_contact_id)
        res.should be nil

        @db.delete_contact(business_telephone_contact_id)
        res = @db.get_contact(business_telephone_contact_id)
        res.should be nil

        @db.delete_contact(mobile_telephone_contact_id)
        res = @db.get_contact(mobile_telephone_contact_id)
        res.should be nil

        @db.delete_contact(home_email_contact_id)
        res = @db.get_contact(home_email_contact_id)
        res.should be nil

        @db.delete_contact(business_email_contact_id)
        res = @db.get_contact(business_email_contact_id)
        res.should be nil

    end


end
