require 'rubygems'
require 'uri'
require 'builder'
require 'rexml/document'
require 'sinatra/base'

module Party

    class Service < Sinatra::Base

        get '/' do

            redirect url('/index.html')

        end

        get '/party/byname/:name' do

            res = THE_DB.get_party_byname(params[:name])

            builder :party_byname, :locals => { :parties => res }

        end


        get '/party/:id' do

            res = THE_DB.get_party(params[:id])
            
            not_found if res == nil

            party_type = res['type']

            builder :party, :locals => { :party => res }

        end    

        get '/party/:id/contacts' do

             res = THE_DB.get_contact_bypartyid(params[:id])

             builder :party_contacts, :locals => { :contacts => res }

        end

        post '/party/:party_id/contact/:contact_id' do

            xml = REXML::Document.new(params.to_s)

            party_id = params[:party_id].to_i
            contact_id = params[:contact_id].to_i
            valid_from = xml.get_text('/party_contact/valid_from')
            valid_until = xml.get_text('/party_contact/valid_until')

            if THE_DB.get_party_contact(party_id, contact_id)
                THE_DB.update_party_contact(party_id, contact_id, valid_from, valid_until)
            else
                THE_DB.create_party_contact(party_id, contact_id, valid_from, valid_until)
            end

            builder :acknowledge, :locals => { :message => "Contact #{contact_id} added to party #{party_id}" }
            
        end

        post '/party/:party_id/contact/:contact_id/delete' do

            party_id = params[:party_id]
            contact_id = params[:contact_id]

            not_found unless THE_DB.get_party_contact(party_id, contact_id)

            THE_DB.delete_party_contact(party_id, contact_id)

            builder :acknowledge, :locals => { :message => "Contact #{contact_id} removed from party #{party_id}" }

        end

        post '/person' do

            xml = REXML::Document.new(params.to_s)

            party_id = xml.get_text('/person/party_id')
            first_name = xml.get_text('/person/first_name')
            last_name = xml.get_text('/person/last_name')
            date_of_birth = xml.get_text('/person/date_of_birth')

            if party_id 
                pty = THE_DB.get_party(party_id)
                not_found unless pty
                raise "Party #{party_id} is not a person" unless pty['type'] == 'P'
                THE_DB.update_person(party_id, first_name, last_name, date_of_birth)
            else
                party_id = THE_DB.create_person(first_name, last_name, date_of_birth)
            end

            if party_id
                redirect url("/party/#{party_id}")
            end

        end

        post '/business' do

            xml = REXML::Document.new(params.to_s)

            party_id = xml.get_text('/business/party_id')
            name = xml.get_text('/business/name')

            if party_id
                pty = THE_DB.get_party(party_id)
                not_found unless pty
                raise "Party #{party_id} is not a business" unless pty['type'] == 'B'
                THE_DB.update_business(party_id, name)
            else
                party_id = THE_DB.create_business(name)
            end

            if party_id
                redirect url("/party/#{party_id}")
            end

        end

        post '/party/:id/delete' do
            
            not_found unless THE_DB.get_party(params[:id])

            THE_DB.delete_party(params[:id])

            builder :acknowledge, :locals => { :message => "Party #{params[:id]} deleted" }

        end


        not_found do
           
           status 404

           builder :error, :locals => { :name => nil, :message => 'Resource not found' }

        end

        error do

           the_error = env['sinatra.error']

           name = nil
           message = 'Unknown error'

           if the_error.respond_to? :name
               name = the_error.name
           end
          
           if the_error.respond_to? :message
               message = the_error.message
           else
               messsage = the_error
           end

           builder :error, :locals => { :name => name, :message => message }

        end

        get '/contact/:id' do 

            res = THE_DB.get_contact(params[:id])

            not_found if res == nil

            builder :contact, :locals => { :contact => res }

        end

        post '/address' do

            xml = REXML::Document.new(params.to_s)

            contact_id = xml.get_text('/address/contact_id')
            street = xml.get_text('/address/street')
            town = xml.get_text('/address/town')
            county = xml.get_text('/address/county')
            post_code = xml.get_text('/address/post_code')

            if contact_id
                ct = THE_DB.get_contact(contact_id)
                not_found unless ct
                raise "Contact #{contact_id} is not an address" unless ct['type'] == 'A'
                THE_DB.update_address(contact_id, street, town, county, post_code)
            else
                contact_id = THE_DB.create_address(street, town, county, post_code)
            end

            if contact_id
                redirect url("/contact/#{contact_id}")
            end

        end

        post '/telephone' do

            xml = REXML::Document.new(params.to_s)

            contact_id = xml.get_text('/telephone/contact_id')
            number = xml.get_text('/telephone/number')
            type = xml.get_text('/telephone/type')

            if contact_id
                ct = THE_DB.get_contact(contact_id)
                not_found unless ct
                raise "Contact #{contact_id} is not a telephone" unless ct['type'] == 'T'
                THE_DB.update_telephone(contact_id, number, type)
            else
                contact_id = THE_DB.create_telephone(number, type)
            end

            if contact_id
                redirect url("/contact/#{contact_id}")
            end

        end

        post '/email' do

            xml = REXML::Document.new(params.to_s)

            contact_id = xml.get_text('/email/contact_id')
            address = xml.get_text('/email/address')
            type = xml.get_text('/email/type')

            if contact_id
                ct = THE_DB.get_contact(contact_id)
                not_found unless ct
                raise "Contact #{contact_id} is not an email" unless ct['type'] == 'E'
                THE_DB.update_email(contact_id, address, type)
            else
                contact_id = THE_DB.create_email(address, type)
            end

            if contact_id
                redirect url("/contact/#{contact_id}")
            end

        end

        post '/contact/:id/delete' do
            
            not_found unless THE_DB.get_contact(params[:id])

            THE_DB.delete_contact(params[:id])

            builder :acknowledge, :locals => { :message => "Contact #{params[:id]} deleted" }

        end


        run! if app_file == $0

    end

end

