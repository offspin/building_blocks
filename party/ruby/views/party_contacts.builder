xml.instruct!

xml.contacts do
    contacts.each do |c|
        xml.contact('id' => c['id'], 'type' => c['type']) do
            xml.link url("/contact/#{c['id']}")
            xml.detail c['detail']
        end
    end
end

