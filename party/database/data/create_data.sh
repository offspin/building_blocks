#/bin/bash
cut -f1 person_raw.txt | nl | cut -f1 | sed 's/^  *//;s/$/	P/' > party.txt
cut -f1 business_raw.txt | nl -v 5001 | cut -f1 | sed 's/^  *//;s/$/	B/' >> party.txt
cut -f1,2,3 person_raw.txt | nl | sed 's/^  *//' > person.txt
cut -f1,2 business_raw.txt | nl -v 5001 | sed 's/^  *//;s/	\([^	]*\)$/ \1/' > business.txt
cut -f1 person_raw.txt | nl | cut -f1 | sed 's/^  *//;s/$/	A/' > contact.txt
cut -f1 business_raw.txt | nl -v 5001 | cut -f1 | sed 's/^  *//;s/$/	A/' >> contact.txt
cut -f4,5,6,7 person_raw.txt | nl | sed 's/^  *//' > address.txt
cut -f4,5,6,7 business_raw.txt | nl -v 5001 | sed 's/^  *//' >> address.txt
cut -f1 person_raw.txt | nl  -v 10001 | cut -f1 | sed 's/^  *//;s/$/	T/' >> contact.txt
cut -f1 person_raw.txt | nl  -v 15001 | cut -f1 | sed 's/^  *//;s/$/	T/' >> contact.txt
cut -f1 person_raw.txt | nl  -v 20001 | cut -f1 | sed 's/^  *//;s/$/	T/' >> contact.txt
cut -f1 person_raw.txt | nl  -v 25001 | cut -f1 | sed 's/^  *//;s/$/	E/' >> contact.txt
cut -f1 person_raw.txt | nl  -v 30001 | cut -f1 | sed 's/^  *//;s/$/	E/' >> contact.txt
cut -f1 business_raw.txt | nl  -v 35001 | cut -f1 | sed 's/^  *//;s/$/	T/' >> contact.txt
cut -f1 business_raw.txt | nl  -v 40001 | cut -f1 | sed 's/^  *//;s/$/	E/' >> contact.txt
cut -f8 person_raw.txt | nl  -v 10001 | sed 's/^  *//;s/$/	H/' > telephone.txt
cut -f9 person_raw.txt | nl  -v 15001 | sed 's/^  *//;s/$/	B/' >> telephone.txt
cut -f10 person_raw.txt | nl  -v 20001 | sed 's/^  *//;s/$/	M/' >> telephone.txt
cut -f11 person_raw.txt | nl  -v 25001 | sed 's/^  *//;s/$/	H/' > email.txt
cut -f12 person_raw.txt | nl  -v 30001 | sed 's/^  *//;s/$/	B/' >> email.txt
cut -f3 business_raw.txt | nl -v 35001 | sed 's/^  *//;s/$/	B/' >> telephone.txt
cut -f8 business_raw.txt | nl -v 40001 | sed 's/^  *//;s/$/	B/' >> email.txt

