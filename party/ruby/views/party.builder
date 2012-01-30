xml.instruct!

if party['type'] == 'P'
    xml.person do 
        xml.party_id party['id']
        xml.first_name party['first_name']
        xml.last_name party['last_name']
        xml.date_of_birth party['date_of_birth']
    end
end

if party['type'] == 'B'
    xml.business do
        xml.party_id party['id']
        xml.name party['name']
    end
end

