require 'rubygems'
require 'sinatra/base'
require 'rspec'
require 'rack/test'

Sinatra::Base.set :environment, :test
Sinatra::Base.set :raise_errors, false

require './pgdb'
require './service'

describe 'Party Service' do
    include Rack::Test::Methods

    def app
        Party::Service.new
    end

    THE_DB = Party::PostgresDatabase.new

    person_party_id = 0
    business_party_id = 0
    address_contact_id = 0
    telephone_contact_id = 0
    email_contact_id = 0

    it 'should create and return a person' do

        post_xml = <<-EOX
            <person>
             <first_name>First</first_name>
             <last_name>Last</last_name>
             <date_of_birth>1985-04-03</date_of_birth>
            </person>
        EOX

        post '/person', post_xml

        last_response.should be_redirect
        follow_redirect!
        last_response.should be_ok

        rx = REXML::Document.new(last_response.body)

        person_party_id = rx.get_text('/person/party_id')

        person_party_id.should be > 0

    end

    it 'should find the person created' do

        get "/party/#{person_party_id}"
        
        last_response.should be_ok

        rx = REXML::Document.new(last_response.body)

        all_there = true
        all_there = false if rx.get_text('/person/party_id') != person_party_id
        all_there = false if rx.get_text('/person/first_name') != 'First'
        all_there = false if rx.get_text('/person/last_name') != 'Last'
        all_there = false if rx.get_text('/person/date_of_birth') != '1985-04-03'

        all_there.should == true

    end

    it 'should find the person created by name' do

        get "/party/byname/first" 

        last_response.should be_ok

        rx = REXML::Document.new(last_response.body)

        el = REXML::XPath.match(rx, "/parties/party[@id=#{person_party_id}]")

        el.should_not be_nil

    end

    it 'should not be able to create an identical person' do

        post_xml = <<-EOX
            <person>
             <first_name>First</first_name>
             <last_name>Last</last_name>
             <date_of_birth>1985-04-03</date_of_birth>
            </person>
        EOX

        post '/person', post_xml

        last_response.status.should == 500

    end

    it 'should update the person created' do

        post_xml = <<-EOX
            <person>
             <party_id>#{person_party_id}</party_id>
             <first_name>Firster</first_name>
             <last_name>Laster</last_name>
             <date_of_birth>1984-03-02</date_of_birth>
            </person>
        EOX

        post '/person', post_xml

        last_response.should be_redirect
        follow_redirect!
        last_response.should be_ok

        rx = REXML::Document.new(last_response.body)

        all_there = true
        all_there = false if rx.get_text('/person/party_id') != person_party_id
        all_there = false if rx.get_text('/person/first_name') != 'Firster'
        all_there = false if rx.get_text('/person/last_name') != 'Laster'
        all_there = false if rx.get_text('/person/date_of_birth') != '1984-03-02'

        all_there.should == true


    end

    it 'should create and return an address' do

        post_xml = <<-EOX
            <address>
             <street>48 Pleasant Crescent</street>
             <town>Oldham</town>
             <county>Lancashire</county>
             <post_code>OL8 1XD</post_code>
            </address>
        EOX

        post '/address', post_xml

        last_response.should be_redirect
        follow_redirect!
        last_response.should be_ok

        rx = REXML::Document.new(last_response.body)

        address_contact_id = rx.get_text('/address/contact_id')

        address_contact_id.should be > 0

    end

    it 'should find the address created' do

        get "/contact/#{address_contact_id}"
        
        last_response.should be_ok

        rx = REXML::Document.new(last_response.body)

        all_there = true
        all_there = false if rx.get_text('/address/contact_id') != address_contact_id
        all_there = false if rx.get_text('/address/street') != '48 Pleasant Crescent'
        all_there = false if rx.get_text('/address/town') != 'Oldham'
        all_there = false if rx.get_text('/address/county') != 'Lancashire'
        all_there = false if rx.get_text('/address/post_code') != 'OL8 1XD'

        all_there.should == true

    end

    it 'should not be able to create an identical address' do

        post_xml = <<-EOX
            <address>
             <street>48 Pleasant Crescent</street>
             <town>Oldham</town>
             <county>Lancashire</county>
             <post_code>OL8 1XD</post_code>
            </address>
        EOX

        post '/address', post_xml
        last_response.status.should == 500

    end

    it 'should create and return a telephone' do

        post_xml = <<-EOX
            <telephone>
             <type>H</type>
             <number>01234 567890</number>
            </telephone>
        EOX

        post '/telephone', post_xml

        last_response.should be_redirect
        follow_redirect!
        last_response.should be_ok

        rx = REXML::Document.new(last_response.body)

        telephone_contact_id = rx.get_text('/telephone/contact_id')

        telephone_contact_id.should be > 0

    end

    it 'should find the telephone created' do

        get "/contact/#{telephone_contact_id}" 

        last_response.should be_ok

        rx = REXML::Document.new(last_response.body)

        all_there = true
        all_there = false if rx.get_text('/telephone/contact_id') != telephone_contact_id
        all_there = false if rx.get_text('/telephone/type') != 'H'
        all_there = false if rx.get_text('/telephone/number') != '01234 567890'

        all_there.should == true

    end

    it 'should not be able to create an identical telephone' do

        post_xml = <<-EOX
            <telephone>
             <type>H</type>
             <number>01234 567890</number>
            </telephone>
        EOX

        post '/telephone', post_xml
        last_response.status.should == 500

    end

    it 'should create and return an email' do

        post_xml = <<-EOX
            <email>
             <type>B</type>
             <address>name@example.com</address>
            </email>
        EOX

        post '/email', post_xml

        last_response.should be_redirect
        follow_redirect!
        last_response.should be_ok

        rx = REXML::Document.new(last_response.body)

        email_contact_id = rx.get_text('/email/contact_id')

        email_contact_id.should be > 0

    end

    it 'should find the email created' do

        get "/contact/#{email_contact_id}"

        last_response.should be_ok

        rx = REXML::Document.new(last_response.body)

        all_there = true
        all_there = false if rx.get_text('/email/contact_id') != email_contact_id
        all_there = false if rx.get_text('/email/type') != 'B'
        all_there = false if rx.get_text('/email/address') != 'name@example.com'

        all_there.should == true

    end

    it 'should not be able to create an identical email' do

        post_xml = <<-EOX
            <email>
             <type>B</type>
             <address>name@example.com</address>
            </email>
        EOX

        post '/email', post_xml
        last_response.status.should == 500

    end

    it 'should link the contacts to the person' do

        post_xml = <<-EOX
            <party_contact>
              <valid_from>1998-08-05</valid_from>
              <valid_until>2014-05-08</valid_until>
            </party_contact>
        EOX

        post "/party/#{person_party_id}/contact/#{address_contact_id}", post_xml
        last_response.should be_ok

        post "/party/#{person_party_id}/contact/#{telephone_contact_id}", post_xml
        last_response.should be_ok

        post "/party/#{person_party_id}/contact/#{email_contact_id}", post_xml
        last_response.should be_ok


    end

    it 'should find the contacts for the person' do

        get "/party/#{person_party_id}/contacts"

        last_response.should be_ok

        rx = REXML::Document.new(last_response.body)

        all_there = true
        all_there = false unless REXML::XPath.match(rx, "/contacts/contact[@id=#{address_contact_id}]")
        all_there = false unless REXML::XPath.match(rx, "/contacts/contact[@id=#{telephone_contact_id}]")
        all_there = false unless REXML::XPath.match(rx, "/contacts/contact[@id=#{email_contact_id}]")

        all_there.should == true

    end

    it 'should not be able to delete the contacts' do

        post "/contact/#{address_contact_id}/delete"
        last_response.status.should == 500

        post "/contact/#{telephone_contact_id}/delete"
        last_response.status.should == 500

        post "/contact/#{email_contact_id}/delete"
        last_response.status.should == 500

    end

    it 'should remove a contact from the person' do

        post "/party/#{person_party_id}/contact/#{address_contact_id}/delete"

        last_response.should be_ok

    end

    it 'should delete the person created, cascading contact references' do

        post "/party/#{person_party_id}/delete"

        last_response.should be_ok

        get "/party/#{person_party_id}"

        last_response.should be_not_found

    end

    it 'should delete the address created' do
         
        post "/contact/#{address_contact_id}/delete"

        last_response.should be_ok

    end

    it 'should delete the telephone created' do

        post "/contact/#{telephone_contact_id}/delete"

        last_response.should be_ok

    end

    it 'should delete the email created' do

        post "/contact/#{email_contact_id}/delete"

        last_response.should be_ok

    end



end
