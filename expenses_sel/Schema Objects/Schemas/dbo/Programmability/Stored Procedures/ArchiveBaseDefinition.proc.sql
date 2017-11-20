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
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '309174d1-72c9-4e08-9406-eacc7c97b9bf', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE dbo.codes_contractstatus SET archived = 0 where statusId = @ID				
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '309174d1-72c9-4e08-9406-eacc7c97b9bf', 1, 0, @recordTitle, @subAccountID;
				END
		END
	
	if @element = 109 -- Contract Category
		BEGIN
			SELECT @recordTitle = categoryDescription, @archived = archived FROM codes_contractcategory where categoryId = @ID
			
			IF @archived = 0
				BEGIN
					UPDATE dbo.codes_contractcategory SET archived = 1 where categoryId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '49ace124-585c-45bb-b02d-f43e9b0c876f', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE dbo.codes_contractcategory SET archived = 0 where categoryId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '49ace124-585c-45bb-b02d-f43e9b0c876f', 1, 0, @recordTitle, @subAccountID;
				END
		END
	
	if @element = 110 -- Contract Type
		BEGIN
			SELECT @recordTitle = contractTypeDescription, @archived = archived FROM codes_contracttype where contractTypeId = @ID
		
			IF @archived = 0
				BEGIN
					UPDATE dbo.codes_contracttype SET archived = 1 where contractTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '3E18CD92-2C6A-4815-8614-8D1742DDB98A', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE dbo.codes_contracttype SET archived = 0 where contractTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '3E18CD92-2C6A-4815-8614-8D1742DDB98A', 1, 0, @recordTitle, @subAccountID;
				END
		END
	if @element = 130 -- Invoice Frequency Type
		BEGIN
			SELECT @recordTitle = invoiceFrequencyTypeDesc, @archived = archived FROM codes_invoicefrequencytype where invoiceFrequencyTypeId = @ID
		
			IF @archived = 0
				BEGIN
					UPDATE dbo.codes_invoicefrequencytype SET archived = 1 where invoiceFrequencyTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '9DE9FA1E-4290-4F79-A77E-34D7D0B8C8DD', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE dbo.codes_invoicefrequencytype SET archived = 0 where invoiceFrequencyTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '9DE9FA1E-4290-4F79-A77E-34D7D0B8C8DD', 1, 0, @recordTitle, @subAccountID;
				END
		END
	
	if @element = 131 -- Invoice Status
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM invoiceStatusType where invoiceStatusTypeId = @ID
		
			IF @archived = 0
				BEGIN
					UPDATE dbo.invoiceStatusType SET archived = 1 where invoiceStatusTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '1634E57F-FF62-4870-8BF1-B6317F33D92F', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE dbo.invoiceStatusType SET archived = 0 where invoiceStatusTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '1634E57F-FF62-4870-8BF1-B6317F33D92F', 1, 0, @recordTitle, @subAccountID;
				END
		END
	if @element = 115 -- Licence Renewal Types
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM licenceRenewalTypes where renewalType = @ID
			
			IF @archived = 0
				BEGIN
					UPDATE dbo.licenceRenewalTypes SET archived = 1 where renewalType = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '6F291BA0-D13E-43DB-BBEA-3AFBCEDA0570', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE dbo.licenceRenewalTypes SET archived = 0 where renewalType = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '6F291BA0-D13E-43DB-BBEA-3AFBCEDA0570', 1, 0, @recordTitle, @subAccountID;
				END
		END
	
	if @element = 112 -- Inflator Metrics
		BEGIN
			SELECT @recordTitle = [name], @archived = archived FROM codes_inflatormetrics where metricId = @ID
			
			IF @archived = 0
				BEGIN
					UPDATE codes_inflatormetrics SET archived = 1 where metricId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'BCDF9634-741D-4F07-80E7-4B3F6CAA2CC7', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE codes_inflatormetrics SET archived = 0 where metricId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'BCDF9634-741D-4F07-80E7-4B3F6CAA2CC7', 1, 0, @recordTitle, @subAccountID;
				END
		END
	if @element = 113 -- Term Types
		BEGIN
			SELECT @recordTitle = termTypeDescription, @archived = archived FROM codes_termtype where termTypeId = @ID
			
			IF @archived = 0
				BEGIN
					UPDATE codes_termtype SET archived = 1 where termTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '707067D6-6632-43DF-BE2A-3CB78A272A33', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE codes_termtype SET archived = 0 where termTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '707067D6-6632-43DF-BE2A-3CB78A272A33', 1, 0, @recordTitle, @subAccountID;
				END
		END
	if @element = 129 -- Financial Status
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM financial_status where statusId = @ID
				
			IF @archived = 0
				BEGIN
					UPDATE financial_status SET archived = 1 where statusId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'b26fe20b-1c3a-41f3-a648-31632ac1164d', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE financial_status SET archived = 0 where statusId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'b26fe20b-1c3a-41f3-a648-31632ac1164d', 1, 0, @recordTitle, @subAccountID;
				END
		END
		
	if @element = 120 -- Task Type
		BEGIN
			SELECT @recordTitle = typeDescription, @archived = archived FROM codes_tasktypes where typeId = @ID
				
			IF @archived = 0
				BEGIN
					UPDATE codes_tasktypes SET archived = 1 where typeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '4BCC7AA0-83D3-4AB9-9A61-6B72AE841E3A', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE codes_tasktypes SET archived = 0 where typeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '4BCC7AA0-83D3-4AB9-9A61-6B72AE841E3A', 1, 0, @recordTitle, @subAccountID;
				END
		END
		
	if @element = 118 -- Units
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM codes_units where unitId = @ID
				
			IF @archived = 0
				BEGIN
					UPDATE codes_units SET archived = 1 where unitId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'C9DB0368-196A-4091-AA67-3230A810DBA1', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE codes_units SET archived = 0 where unitId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'C9DB0368-196A-4091-AA67-3230A810DBA1', 1, 0, @recordTitle, @subAccountID;
				END
		END
	
	if @element = 60 -- Product Categories
		BEGIN
			SELECT @recordTitle = [Description], @archived = archived FROM productCategories where categoryId = @ID
				
			IF @archived = 0
				BEGIN
					UPDATE productCategories SET archived = 1 where categoryId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '7F52AC64-0666-4077-B1F8-1917120306E2', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE productCategories SET archived = 0 where categoryId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '7F52AC64-0666-4077-B1F8-1917120306E2', 1, 0, @recordTitle, @subAccountID;
				END
		END
		
	if @element = 55 -- Supplier Status
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM supplier_status where statusid = @ID
				
			IF @archived = 0
				BEGIN
					UPDATE supplier_status SET archived = 1 where statusid = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'D66C488C-CF20-44C2-AD94-0EA060553DDB', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE supplier_status SET archived = 0 where statusid = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, 'D66C488C-CF20-44C2-AD94-0EA060553DDB', 1, 0, @recordTitle, @subAccountID;
				END
			
		END
		
	if @element = 53 -- Supplier Categories
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM supplier_categories where categoryid = @ID
					
			IF @archived = 0
				BEGIN
					UPDATE supplier_categories SET archived = 1 where categoryid = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '0F1D0D1A-CDDA-468C-93FF-AB549217D6F6', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE supplier_categories SET archived = 0 where categoryid = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '0F1D0D1A-CDDA-468C-93FF-AB549217D6F6', 1, 0, @recordTitle, @subAccountID;
				END
		END
	
	if @element = 136 -- Product Licence Types
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM codes_licencetypes where licenceTypeId = @ID
					
			IF @archived = 0
				BEGIN
					UPDATE codes_licencetypes SET archived = 1 where licenceTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '72B9B98C-88F6-44B6-81F5-F0E131D0EF9C', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE codes_licencetypes SET archived = 0 where licenceTypeId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '72B9B98C-88F6-44B6-81F5-F0E131D0EF9C', 1, 0, @recordTitle, @subAccountID;
				END
		END
	
	if @element = 114 -- Sales Tax
		BEGIN
			SELECT @recordTitle = [description], @archived = archived FROM codes_salesTax where salesTaxId = @ID
					
			IF @archived = 0
				BEGIN
					UPDATE codes_salesTax SET archived = 1 where salesTaxId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '86EE822F-032A-4D31-A34C-BEB89DACF5CF', 0, 1, @recordTitle, @subAccountID;
				END
			ELSE
				BEGIN
					UPDATE codes_salesTax SET archived = 0 where salesTaxId = @ID
					exec addUpdateEntryToAuditLog @employeeID, @delegateID, @element, @ID, '86EE822F-032A-4D31-A34C-BEB89DACF5CF', 1, 0, @recordTitle, @subAccountID;
				END
		END			
