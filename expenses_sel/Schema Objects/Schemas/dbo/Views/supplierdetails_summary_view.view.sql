CREATE VIEW [dbo].[supplierdetails_summary_view]
AS
SELECT 
supplier_details.supplierid,
supplier_details.suppliername,
supplier_details.categoryid,
supplier_details.statusid,
supplier_details.annual_turnover,
supplier_details.supplier_currency,
supplier_details.numberofemployees,
supplier_details.weburl,
supplier_details.financial_ye,
supplier_details.financialStatusLastChecked,
supplier_details.supplierEmail as Email,
supplier_addresses.[Switchboard],
supplier_addresses.[Fax],
supplier_addresses.[Addr_line1],
supplier_addresses.[Addr_Line2],
supplier_addresses.town,
supplier_addresses.county,
supplier_addresses.postcode,
emp_1.firstname + ' ' + emp_1.surname AS [SupplierCreatedBy],
emp_2.firstname + ' ' + emp_2.surname AS [SupplierModifiedBy],
supplier_details.ModifiedOn,
supplier_details.CreatedOn,
financial_status.description as Financial_Status,
emp_3.firstname + ' ' + emp_3.surname as InternalContact,
(select count(*) from contract_details where contract_details.supplierid = supplier_details.supplierid) AS ContractCount,
(select country from global_countries
inner join countries on countries.countryid = supplier_addresses.countryid
where global_countries.globalcountryid = countries.globalcountryid) as Country
FROM supplier_details
INNER JOIN supplier_addresses ON supplier_details.primary_addressid = supplier_addresses.addressid
LEFT JOIN employees AS emp_1 ON emp_1.[employeeId] = supplier_details.[CreatedBy]
LEFT JOIN employees AS emp_2 ON emp_2.[employeeId] = supplier_details.[ModifiedBy]
LEFT JOIN employees AS emp_3 ON emp_3.[employeeId] = supplier_details.[internalStaffContactId]
LEFT JOIN financial_status ON financial_status.statusid = supplier_details.[financial_statusid]
