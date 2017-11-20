CREATE PROCEDURE [dbo].[saveInvoiceFrequencyType] 
(
@ID int, 
@subAccountId int, 
@invoiceFrequencyTypeDesc nvarchar(50), 
@frequencyInMonths smallint,
@employeeId int,
@delegateID int
)
AS
DECLARE @count INT;
DECLARE @retVal INT;
DECLARE @recordTitle nvarchar(2000);

IF @ID = -1
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_invoicefrequencytype WHERE invoiceFrequencyTypeDesc = @invoiceFrequencyTypeDesc AND subAccountId = @subAccountId);
	
	IF @count = 0
	BEGIN
		INSERT INTO codes_invoicefrequencytype (subAccountId, invoiceFrequencyTypeDesc, frequencyInMonths, archived, createdOn, createdBy)
		VALUES (@subAccountId, @invoiceFrequencyTypeDesc, @frequencyInMonths, 0, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Invoice Frequency ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @invoiceFrequencyTypeDesc + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 130, @retVal, @recordTitle, @subAccountId;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_invoicefrequencytype WHERE invoiceFrequencyTypeId <> @ID AND subAccountId = @subAccountId AND invoiceFrequencyTypeDesc = @invoiceFrequencyTypeDesc);
	
	IF @count = 0
	BEGIN
		DECLARE @oldinvoiceFrequencyTypeDesc nvarchar(50);
		DECLARE @oldfrequencyInMonths smallint;
				
		select @oldinvoiceFrequencyTypeDesc = invoiceFrequencyTypeDesc, @oldfrequencyInMonths = frequencyInMonths from codes_invoicefrequencytype where invoiceFrequencyTypeId = @ID;
		
		UPDATE codes_invoicefrequencytype SET invoiceFrequencyTypeDesc = @invoiceFrequencyTypeDesc, frequencyInMonths = @frequencyInMonths, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE invoiceFrequencyTypeId = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'Invoice Frequency ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @invoiceFrequencyTypeDesc + ')';

		if @oldinvoiceFrequencyTypeDesc <> @invoiceFrequencyTypeDesc
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 130, @ID, '9DE9FA1E-4290-4F79-A77E-34D7D0B8C8DD', @oldinvoiceFrequencyTypeDesc, @invoiceFrequencyTypeDesc, @recordtitle, @subAccountId;

		if @oldfrequencyInMonths <> @frequencyInMonths
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 130, @ID, 'E650C8E1-7FA4-4414-AA7A-56EC33670424', @oldfrequencyInMonths, @frequencyInMonths, @recordtitle, @subAccountId;
	END
END

RETURN @retVal;
