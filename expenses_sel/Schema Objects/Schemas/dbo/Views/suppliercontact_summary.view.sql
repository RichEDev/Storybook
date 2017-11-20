CREATE VIEW [dbo].[suppliercontact_summary]
AS 
SELECT supplier_contacts.*,
p.address_title as pAddressTitle,
p.addr_line1 as pAddrLine1,
p.addr_line2 as pAddrLine2,
p.town as pTown,
p.county as pCounty,
p.postcode As pPostCode,
pgc.country as pCountry,
p.switchboard as pSwitchboard,
p.fax as pFax,
b.address_title as bAddressTitle,
b.addr_line1 as bAddrLine1,
b.addr_line2 as bAddrLine2,
b.town as bTown,
b.county as bCounty,
b.postcode As bPostCode,
bgc.country as bCountry,
b.switchboard as bSwitchboard,
b.fax as bFax
FROM supplier_contacts
LEFT JOIN supplier_addresses as p ON supplier_contacts.home_addressid = p.addressid
LEFT JOIN countries as pc on p.countryid = pc.countryid
LEFT JOIN global_countries as pgc ON pc.globalcountryid = pgc.globalcountryid
LEFT JOIN supplier_addresses as b ON supplier_contacts.business_addressid = b.addressid
LEFT JOIN countries as bc on b.countryid = bc.countryid
LEFT JOIN global_countries as bgc ON bc.globalcountryid = bgc.globalcountryid
