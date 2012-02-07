xml.instruct!

xml.PartyContact('PartyId' => party_contact['party_id'], 'ContactId' => party_contact['contact_id']) do
    xml.ValidFrom party_contact['valid_from']
    xml.ValidUntil party_contact['valid_until']
end
