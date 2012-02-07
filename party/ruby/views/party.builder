xml.instruct!

if party['type'] == 'P'
    xml.Person('Id' => party['id'], 'Type' => party['type']) do 
        xml.FirstName party['first_name']
        xml.LastName party['last_name']
        xml.DateOfBirth party['date_of_birth']
    end
end

if party['type'] == 'B'
    xml.Business('Id' => party['id'], 'Type' => party['type']) do
        xml.Name party['name']
        xml.RegNumber party['reg_number']
    end
end

