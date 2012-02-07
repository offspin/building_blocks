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

        get '/party/:party_id/contact/:contact_id' do

            party_id = params[:party_id]
            contact_id = params[:contact_id]

            res = THE_DB.get_party_contact(party_id, contact_id)

            builder :party_contact, :locals => { :party_contact => res }

        end

        post '/party/:party_id/contact/:contact_id' do

            xml = REXML::Document.new(request.body.read)

            party_id = params[:party_id].to_i
            contact_id = params[:contact_id].to_i
            valid_from = xml.get_text('/PartyContact/ValidFrom')
            valid_until = xml.get_text('/PartyContact/ValidUntil')

            if THE_DB.get_party_contact(party_id, contact_id)
                THE_DB.update_party_contact(party_id, contact_id, valid_from, valid_until)
            else
                THE_DB.create_party_contact(party_id, contact_id, valid_from, valid_until)
            end

            redirect url("/party/#{party_id}/contact/#{contact_id}")

        end

        post '/party/:party_id/contact/:contact_id/delete' do

            party_id = params[:party_id]
            contact_id = params[:contact_id]

            not_found unless THE_DB.get_party_contact(party_id, contact_id)

            THE_DB.delete_party_contact(party_id, contact_id)

            builder :acknowledge, :locals => { :message => "Contact #{contact_id} removed from party #{party_id}" }

        end

        post '/person' do
            
            xml = REXML::Document.new(request.body.read)

            first_name = xml.get_text('/Person/FirstName')
            last_name = xml.get_text('/Person/LastName')
            date_of_birth = xml.get_text('/Person/DateOfBirth')

            party_id = THE_DB.create_person(first_name, last_name, date_of_birth)

            if party_id
                redirect url("/party/#{party_id}")
            end

        end

        post '/person/:id' do

            party_id = params[:id]

            xml = REXML::Document.new(request.body.read)

            first_name = xml.get_text('/Person/FirstName')
            last_name = xml.get_text('/Person/LastName')
            date_of_birth = xml.get_text('/Person/DateOfBirth')

            pty = THE_DB.get_party(party_id)
            not_found unless pty
            raise "Party #{party_id} is not a person" unless pty['type'] == 'P'
            THE_DB.update_person(party_id, first_name, last_name, date_of_birth)

            redirect url("/party/#{party_id}")

        end

        post '/business' do

            xml = REXML::Document.new(request.body.read)

            name = xml.get_text('/Business/Name')

            party_id = THE_DB.create_business(name)

            if party_id
                redirect url("/party/#{party_id}")
            end

        end

        post '/business/:id' do

            party_id = params[:id]

            xml = REXML::Document.new(request.body.read)

            name = xml.get_text('/Business/Name')

            pty = THE_DB.get_party(party_id)
            not_found unless pty
            raise "Party #{party_id} is not a business" unless pty['type'] == 'B'
            THE_DB.update_business(party_id, name)

            redirect url("/party/#{party_id}")

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

            contact_id = xml.get_text('/Address/@Id')
            street = xml.get_text('/Address/Street')
            town = xml.get_text('/Address/Town')
            county = xml.get_text('/Address/County')
            post_code = xml.get_text('/Address/PostCode')

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

            xml = REXML::Document.new(request.body.read)

            number = xml.get_text('/Telephone/Number')
            type = xml.get_text('/Telephone/SubType')

            contact_id = THE_DB.create_telephone(number, type)

            if contact_id
                redirect url("/contact/#{contact_id}")
            end

        end

        post '/telephone/:id' do

            contact_id = params[:id]
            xml = REXML::Document.new(request.body.read)

            number = xml.get_text('/Telephone/Number')
            type = xml.get_text('/Telephone/SubType')

            ct = THE_DB.get_contact(contact_id)
            not_found unless ct
            raise "Contact #{contact_id} is not a telephone" unless ct['type'] == 'T'
            THE_DB.update_telephone(contact_id, number, type)

            redirect url("/contact/#{contact_id}")

        end

        post '/email' do

            xml = REXML::Document.new(request.body.read)

            address = xml.get_text('/Email/Address')
            type = xml.get_text('/Email/SubType')

            contact_id = THE_DB.create_email(address, type)

            if contact_id
                redirect url("/contact/#{contact_id}")
            end

        end

        post '/email/:id' do

            xml = REXML::Document.new(request.body.read)

            contact_id = params[:id]

            address = xml.get_text('/Email/Address')
            type = xml.get_text('/Email/SubType')

            ct = THE_DB.get_contact(contact_id)
            not_found unless ct
            raise "Contact #{contact_id} is not an email" unless ct['type'] == 'E'
            THE_DB.update_email(contact_id, address, type)
            redirect url("/contact/#{contact_id}")

        end


        post '/contact/:id/delete' do
            
            not_found unless THE_DB.get_contact(params[:id])

            THE_DB.delete_contact(params[:id])

            builder :acknowledge, :locals => { :message => "Contact #{params[:id]} deleted" }

        end


        run! if app_file == $0

    end

end

