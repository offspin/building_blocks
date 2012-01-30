xml.instruct!

if contact['type'] == 'A'
    xml.address do 
        xml.contact_id contact['id']
        xml.street contact['street']
        xml.town contact['town']
        if contact['county']
            xml.county contact['county']
        end
        if contact['post_code']
            xml.post_code contact['post_code']
        end
    end
end

if contact['type'] == 'E'
    xml.email do
        xml.contact_id contact['id']
        xml.type contact['sub_type']
        xml.address contact['email_address']
    end
end

if contact['type'] == 'T'
    xml.telephone do 
        xml.contact_id contact['id']
        xml.type contact['sub_type']
        xml.number contact['telephone_number']
    end
end


