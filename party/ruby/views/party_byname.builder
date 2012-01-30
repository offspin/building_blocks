xml.instruct!
xml.parties do
    parties.each do |p|
        xml.party('id' => p['id'], 'type' => p['type']) do
            xml.link url("/party/#{p['id']}")
            xml.name p['name']
        end
    end
end

