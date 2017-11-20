CREATE  VIEW [dbo].[invoice_details_view]
AS 
	SELECT 
	contract_details.[subAccountId],
	invoices.*, 
	codes_salestax.[Description], 
	codes_salestax.[SalesTax] AS [SalesTaxPercentage],
	invoiceStatusType.[description] AS [InvoiceStatusDescription],
	invoiceStatusType.[Archived] AS [InvoiceStatusIsArchive]
	FROM invoices
	INNER JOIN contract_details ON contract_details.[ContractId] = [invoices].[ContractId]
	LEFT OUTER JOIN codes_salestax ON codes_salestax.[SalesTaxId] = invoices.[SalesTaxRate]
	LEFT OUTER JOIN invoiceStatusType ON invoiceStatusType.[invoiceStatusTypeId] = invoices.[InvoiceStatus]
