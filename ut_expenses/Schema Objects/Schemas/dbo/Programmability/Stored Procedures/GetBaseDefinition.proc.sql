CREATE PROCEDURE [dbo].[GetBaseDefinition] 
	@element int,
	@subAccountID int
AS
BEGIN
	if @element = 111 -- Contract Status
	BEGIN
		SELECT statusId, statusDescription, createdOn, createdBy, modifiedOn, modifiedBy, archived, isArchive from dbo.codes_contractstatus where subAccountId = @subAccountID
	END
	
	if @element = 109 -- Contract Category
	BEGIN
		SELECT categoryId, categoryDescription, createdOn, createdBy, modifiedOn, modifiedBy, archived from dbo.codes_contractcategory where subAccountId = @subAccountID
	END
	
	if @element = 110 -- Contract Type
	BEGIN
		SELECT contractTypeId, contractTypeDescription, createdOn, createdBy, modifiedOn, modifiedBy, archived from dbo.codes_contracttype where subAccountId = @subAccountID
	END
	
	if @element = 130 -- Invoice Frequency Types
	BEGIN
		SELECT [invoiceFrequencyTypeId], [invoiceFrequencyTypeDesc], createdOn, createdBy, modifiedOn, modifiedBy, [archived], [frequencyInMonths]  from dbo.codes_invoicefrequencytype where subAccountId = @subAccountID
	END
	
	if @element = 131 -- Invoice Status
	BEGIN
		SELECT [invoiceStatusTypeID], [description], createdOn, createdBy, modifiedOn, modifiedBy, archived, isArchive from dbo.invoiceStatusType where subAccountId = @subAccountID
	END
	
	if @element = 115 -- Licence Renewal Types
	BEGIN
		SELECT [renewalType], [description], createdOn, createdBy, modifiedOn, modifiedBy, archived FROM dbo.licenceRenewalTypes where subAccountId = @subAccountID
	END
	
	if @element = 112 -- Inflator Metrics
	BEGIN
		SELECT metricId, name, createdOn, createdBy, modifiedOn, modifiedBy, archived, percentage, requiresExtraPct from dbo.codes_inflatormetrics where subAccountId = @subAccountID
	END
	
	if @element = 113 -- Term types
	BEGIN
		SELECT termTypeId, termTypeDescription, createdOn, createdBy, modifiedOn, modifiedBy, archived from dbo.codes_termtype where subAccountId = @subAccountID
	END
	
	if @element = 129 -- Financial Status
	BEGIN
		SELECT statusid, description, createdOn, createdBy, modifiedOn, modifiedBy, archived FROM dbo.financial_status  where subAccountId = @subAccountID
	END
	
	if @element = 120 -- Task Type
	BEGIN
		SELECT [typeId], [typeDescription], createdOn, createdBy, modifiedOn, modifiedBy, archived FROM dbo.codes_tasktypes  where subAccountId = @subAccountID
	END
	
	if @element = 118 -- Units
	BEGIN
		select [unitId], [description], createdOn, createdBy, modifiedOn, modifiedBy, archived from dbo.codes_units where subAccountId = @subAccountID
	END
	
	if @element = 60 -- Product Categories
	BEGIN
		SELECT [CategoryId],[Description], createdOn, createdBy, modifiedOn,modifiedBy, archived FROM dbo.productCategories where subAccountId = @subAccountID	
	END
	
	if @element = 55 -- Supplier Status
	BEGIN
		SELECT statusid, [description], createdOn, createdBy, modifiedOn, modifiedBy, archived, sequence, deny_contract_add FROM dbo.supplier_status where subAccountId = @subAccountID
	END  
	
	if @element = 53 -- Supplier Categories
	BEGIN
		SELECT categoryid, [description], createdOn, createdBy, modifiedOn, modifiedby, archived FROM dbo.supplier_categories where subAccountId = @subAccountID
	END
	
	if @element = 136 -- Product Licence Types
	BEGIN
		SELECT licenceTypeId, [description], createdOn, createdBy, modifiedOn, modifiedBy, archived from dbo.codes_licenceTypes where subAccountId = @subAccountID
	END
	
	if @element = 114 -- Sales Tax
	BEGIN
		SELECT salesTaxID, [description], createdOn, createdBy, modifiedOn, modifiedBy, archived, salesTax FROM dbo.codes_salesTax where subAccountId = @subAccountID
	END
END
