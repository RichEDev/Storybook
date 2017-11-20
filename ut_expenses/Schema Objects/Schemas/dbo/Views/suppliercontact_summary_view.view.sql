CREATE VIEW [dbo].[suppliercontact_summary_view]
AS 
SELECT supplier_addresses.*
FROM supplier_addresses
INNER JOIN supplier_details ON supplier_details.primary_addressid = supplier_addresses.addressid
