require './pgdb'


describe 'PostgreSQL Database' do

    db = nil
    person_party_id = 0
    business_party_id = 0
    address_contact_id = 0
    home_telephone_contact_id = 0
    business_telephone_contact_id = 0
    mobile_telephone_contact_id = 0
    home_email_contact_id = 0
    business_email_contact_id = 0

    before :each do
	db = connect_db
    end

    it 'should be connected' do
	db.status.should == PGconn::CONNECTION_OK
    end

    it 'should create a person' do
	person_party_id = create_person(db, 'First', 'Last', '2000-03-05')
	person_party_id.should be > 0
    end

    it 'should find the person created by id' do
	res = get_party(db, person_party_id)
	res['first_name'].should == 'First'
	res['last_name'].should == 'Last'
	res['date_of_birth'].should == '2000-03-05'
    end

    it 'should find the person created by name' do
	found = false
	res = get_party_byname(db, 'first last')
	res.each do |r|
	   if r['id'].to_i == person_party_id
	       found = true
	       break
	   end
	end
	found.should == true
    end

    it 'should update the person created' do
	update_person(db, person_party_id, 'Firster', 'Laster', '2000-04-06')
	res = get_party(db, person_party_id)
	res['first_name'].should == 'Firster'
	res['last_name'].should == 'Laster'
	res['date_of_birth'].should == '2000-04-06'
    end

    it 'should create an address' do
	address_contact_id = create_address(db, '48 Pleasant Crescent', 'Royston Vasey', 'Hertfordshire', 'SG8 1AB')
	address_contact_id.should be > 0
    end

    it 'should create a home telephone number' do
	home_telephone_contact_id = create_telephone(db, '01234 567890', 'H')
	home_telephone_contact_id.should be > 0
    end

    it 'should create a business telephone number' do
	business_telephone_contact_id = create_telephone(db, '01234 234567', 'B')
	business_telephone_contact_id.should be > 0
    end

    it 'should create a mobile telephone number' do
	mobile_telephone_contact_id = create_telephone(db, '07734 654321', 'M')
	mobile_telephone_contact_id.should be > 0
    end

    it 'should create a home email' do
	home_email_contact_id = create_email(db, 'me@home.co.uk', 'H')
	home_email_contact_id.should be > 0
    end

    it 'should create a business email' do
	business_email_contact_id = create_email(db, 'him@business.co.uk', 'B')
	business_email_contact_id.should be > 0
    end

    it 'should update an address' do
	update_address(db, address_contact_id, '49 Down The Line', 'Royston', 'Herts', 'SG8 6QW')
	res = get_contact(db, address_contact_id)
	res['street'].should == '49 Down The Line'
	res['town'].should == 'Royston'
	res['county'].should == 'Herts'
	res['post_code'].should == 'SG8 6QW'
    end

    it 'should link the address created to the person created' do
	create_party_contact(db, person_party_id, address_contact_id, '2000-05-06', '2005-06-05')
	res = get_contact_bypartyid(db, person_party_id)

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
	update_party_contact(db, person_party_id, address_contact_id, '2008-01-02', '2009-02-01')
	res = get_contact_bypartyid(db, person_party_id)

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
	    delete_contact(db, address_contact_id)
	end.should raise_exception
    end

    it 'should delete the link between address and person' do
	delete_party_contact(db, person_party_id, address_contact_id)
	res = get_contact_bypartyid(db, person_party_id)

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
	delete_address(db, address_contact_id)
	res = get_contact(db, address_contact_id)
	res.should be nil
    end
	
    it 'should delete the person created' do
	delete_party(db, person_party_id)
	res = get_party(db, person_party_id)
	res.should be nil
    end

    it 'should create a business' do
	business_party_id = create_business(db, 'The Business')
	business_party_id.should be > 0
    end

    it 'should find the business created by id' do
	res = get_party(db, business_party_id)
	res['name'].should == 'The Business'
    end

    it 'should find the business created by name' do
	found = false
	res = get_party_byname(db, 'the business')
	res.each do |r|
	    if r['id'].to_i == business_party_id
		found = true
		break
	    end
	end
	found.should == true
    end

    it 'should update the business created ' do
	update_business(db, business_party_id, 'The Businesser')
	res = get_party(db, business_party_id)
	res['name'].should == 'The Businesser'
    end

    it 'should delete the business created ' do
	delete_party(db, business_party_id)
	res = get_party(db, business_party_id)
	res.should be nil
    end


end
