xml.instruct!

xml.ContactResults do
    contacts.each do |c|
        xml.ContactSummary('Id' => c['id'], 'Type' => c['type']) do
            xml.Detail c['detail']
            xml.Link url("/contact/#{c['id']}")
        end
    end
end

