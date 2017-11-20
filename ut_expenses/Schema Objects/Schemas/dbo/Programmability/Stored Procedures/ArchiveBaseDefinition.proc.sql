CREATE PROCEDURE [dbo].[ArchiveBaseDefinition] 
	-- Add the parameters for the stored procedure here
	@ID int,
	@element int,
	@subAccountID int,
	@employeeID INT,
	@delegateID INT

AS
	DECLARE @recordTitle nvarchar(2000);
	DECLARE @archived bit;

	if @element = 111 --Contract Status
		BEGIN
			SELECT @recordTitle = statusDescription, @archived = archived FROM codes_contractstatus WHERE statusID = @ID
			
			IF @archived = 0
				BEGIN
					UPDATE dbo.codes_contractstatus SET archived = 1 where statusId = @ID				
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'CAFC02A3-D62C-4AB1-9FB4-D994A7496314', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE dbo.codes_contractstatus SET archived = 0 where statusId = @ID				
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'CAFC02A3-D62C-4AB1-9FB4-D994A7496314', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
	
	if @element = 109 -- Contract Category
		BEGIN
			SELECT @recordTitle = categoryDescription, @archived = archived FROM codes_contractcategory where categoryId = @ID
			
			IF @archived = 0
				BEGIN
					UPDATE dbo.codes_contractcategory SET archived = 1 where categoryId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '75AF06A6-62C0-4CDA-BC65-14C922B57C57', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE dbo.codes_contractcategory SET archived = 0 where categoryId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '75AF06A6-62C0-4CDA-BC65-14C922B57C57', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
	
	if @element = 110 -- Contract Type
		BEGIN
			SELECT @recordTitle = contractTypeDescription, @archived = archived FROM codes_contracttype where contractTypeId = @ID
		
			IF @archived = 0
				BEGIN
					UPDATE dbo.codes_contracttype SET archived = 1 where contractTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'C4ACB4C1-544F-4D9D-AB26-65DD41EEE871', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE dbo.codes_contracttype SET archived = 0 where contractTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'C4ACB4C1-544F-4D9D-AB26-65DD41EEE871', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
	if @element = 130 -- Invoice Frequency Type
		BEGIN
			SELECT @recordTitle = invoiceFrequencyTypeDesc, @archived = archived FROM codes_invoicefrequencytype where invoiceFrequencyTypeId = @ID
		
			IF @archived = 0
				BEGIN
					UPDATE dbo.codes_invoicefrequencytype SET archived = 1 where invoiceFrequencyTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'E56CF6D5-3CC0-45DC-B7C5-9907FB87F896', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE dbo.codes_invoicefrequencytype SET archived = 0 where invoiceFrequencyTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'E56CF6D5-3CC0-45DC-B7C5-9907FB87F896', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
	
	if @element = 131 -- Invoice Status
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM invoiceStatusType where invoiceStatusTypeId = @ID
		
			IF @archived = 0
				BEGIN
					UPDATE dbo.invoiceStatusType SET archived = 1 where invoiceStatusTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '80FC4B9B-BA43-4360-9674-6B78BEB1791D', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE dbo.invoiceStatusType SET archived = 0 where invoiceStatusTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '80FC4B9B-BA43-4360-9674-6B78BEB1791D', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
	if @element = 115 -- Licence Renewal Types
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM licenceRenewalTypes where renewalType = @ID
			
			IF @archived = 0
				BEGIN
					UPDATE dbo.licenceRenewalTypes SET archived = 1 where renewalType = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '76ECA1E9-4003-4980-A926-47ACB15953A5', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE dbo.licenceRenewalTypes SET archived = 0 where renewalType = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '76ECA1E9-4003-4980-A926-47ACB15953A5', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
	
	if @element = 112 -- Inflator Metrics
		BEGIN
			SELECT @recordTitle = [name], @archived = archived FROM codes_inflatormetrics where metricId = @ID
			
			IF @archived = 0
				BEGIN
					UPDATE codes_inflatormetrics SET archived = 1 where metricId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '55E1F977-FFEF-496E-A4E8-449BA5AE5DEE', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE codes_inflatormetrics SET archived = 0 where metricId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '55E1F977-FFEF-496E-A4E8-449BA5AE5DEE', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
	if @element = 113 -- Term Types
		BEGIN
			SELECT @recordTitle = termTypeDescription, @archived = archived FROM codes_termtype where termTypeId = @ID
			
			IF @archived = 0
				BEGIN
					UPDATE codes_termtype SET archived = 1 where termTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '0A01D13A-F5CE-4EDC-B723-9A4BF91818B1', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE codes_termtype SET archived = 0 where termTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '0A01D13A-F5CE-4EDC-B723-9A4BF91818B1', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
	if @element = 129 -- Financial Status
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM financial_status where statusId = @ID
				
			IF @archived = 0
				BEGIN
					UPDATE financial_status SET archived = 1 where statusId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'E265BD16-1C80-436D-87DD-12041C103CDE', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE financial_status SET archived = 0 where statusId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'E265BD16-1C80-436D-87DD-12041C103CDE', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
		
	if @element = 120 -- Task Type
		BEGIN
			SELECT @recordTitle = typeDescription, @archived = archived FROM codes_tasktypes where typeId = @ID
				
			IF @archived = 0
				BEGIN
					UPDATE codes_tasktypes SET archived = 1 where typeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'F6B35038-767B-4E62-83C4-FEFB7C561B4A', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE codes_tasktypes SET archived = 0 where typeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'F6B35038-767B-4E62-83C4-FEFB7C561B4A', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
		
	if @element = 118 -- Units
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM codes_units where unitId = @ID
				
			IF @archived = 0
				BEGIN
					UPDATE codes_units SET archived = 1 where unitId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '32C6C315-7C8B-4025-B85A-F5BE2DFDAB60', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE codes_units SET archived = 0 where unitId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '32C6C315-7C8B-4025-B85A-F5BE2DFDAB60', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
	
	if @element = 60 -- Product Categories
		BEGIN
			SELECT @recordTitle = [Description], @archived = archived FROM productCategories where categoryId = @ID
				
			IF @archived = 0
				BEGIN
					UPDATE productCategories SET archived = 1 where categoryId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '13C54C1B-C9E0-4141-A360-41D400119F4E', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE productCategories SET archived = 0 where categoryId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '13C54C1B-C9E0-4141-A360-41D400119F4E', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
		
	if @element = 55 -- Supplier Status
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM supplier_status where statusid = @ID
				
			IF @archived = 0
				BEGIN
					UPDATE supplier_status SET archived = 1 where statusid = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '23D684AE-B237-46C1-840F-048E24E34AFD', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE supplier_status SET archived = 0 where statusid = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '23D684AE-B237-46C1-840F-048E24E34AFD', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
			
		END
		
	if @element = 53 -- Supplier Categories
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM supplier_categories where categoryid = @ID
					
			IF @archived = 0
				BEGIN
					UPDATE supplier_categories SET archived = 1 where categoryid = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '814A2E6C-261D-486A-9D48-6A4B2CBD1FE5', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE supplier_categories SET archived = 0 where categoryid = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '814A2E6C-261D-486A-9D48-6A4B2CBD1FE5', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
	
	if @element = 136 -- Product Licence Types
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM codes_licencetypes where licenceTypeId = @ID
					
			IF @archived = 0
				BEGIN
					UPDATE codes_licencetypes SET archived = 1 where licenceTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '82F9F9BE-F4C5-4BAE-928E-6183A68B71BC', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE codes_licencetypes SET archived = 0 where licenceTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '82F9F9BE-F4C5-4BAE-928E-6183A68B71BC', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END
	
	if @element = 114 -- Sales Tax
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM codes_salesTax where salesTaxId = @ID
					
			IF @archived = 0
				BEGIN
					UPDATE codes_salesTax SET archived = 1 where salesTaxId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '0FC66303-F125-4F35-9F51-7BB42A838B4E', 'Live', 'Archived', @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE codes_salesTax SET archived = 0 where salesTaxId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '0FC66303-F125-4F35-9F51-7BB42A838B4E', 'Archived', 'Live', @recordTitle, @subAccountID;
				END
		END