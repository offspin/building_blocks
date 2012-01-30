require 'rubygems'
require 'bundler/setup'
require 'sinatra/base'
require 'digest/md5'
require './pgdb'
require './service'

THE_DB = Party::PostgresDatabase.new

realm_res = THE_DB.get_system_config('REALM')
realm = realm_res['string_value']

opaque_res = THE_DB.get_system_config('OPAQUE')
opaque = opaque_res['string_value']

the_service = Rack::Auth::Digest::MD5.new(Party::Service, realm, opaque) do |name|
    res = THE_DB.get_user(name)
    pwh = res['password_hash']
end
the_service.passwords_hashed = true

map '/service' do 
    run the_service
end


