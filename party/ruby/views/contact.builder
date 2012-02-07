xml.instruct!

if contact['type'] == 'A'
    xml.Address('Id' => contact['id'], 'Type' => contact['type']) do 
        xml.Street contact['street']
        xml.Town contact['town']
        if contact['county']
            xml.County contact['county']
        end
        if contact['post_code']
            xml.PostCode contact['post_code']
        end
    end
end

if contact['type'] == 'E'
    xml.Email('Id' => contact['id'], 'Type' => contact['type']) do
        xml.SubType contact['sub_type']
        xml.Address contact['email_address']
    end
end

if contact['type'] == 'T'
    xml.Telephone('Id' => contact['id'], 'Type' => contact['type']) do
        xml.SubType contact['sub_type']
        xml.Number contact['telephone_number']
    end
end


