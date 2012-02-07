xml.instruct!

xml.PartyResults do
    parties.each do |p|
        xml.PartySummary('Id' => p['id'], 'Type' => p['type']) do
            xml.Name p['name']
            xml.Link url("/party/#{p['id']}")
        end
    end
end

