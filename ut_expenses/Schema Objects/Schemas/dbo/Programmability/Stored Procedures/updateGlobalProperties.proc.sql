-- =============================================
-- Author:		Paul Lancashire
-- Create date: 30 April 2009
-- Description:	Update the Other table to set Global Properties
-- =============================================
CREATE PROCEDURE [dbo].[updateGlobalProperties] 
	-- Add the parameters for the stored procedure here
	@modifiedon datetime,
    @modifiedby int,
    @productFieldType tinyint,
    @supplierFieldType tinyint,
    @purchaseOrderNumberName nvarchar(150),
    @supplierName nvarchar(150),
    @dateApprovedName nvarchar(150),
    @totalName nvarchar(150),
    @orderRecurrenceName nvarchar(150),
    @orderEndDateName nvarchar(150),
    @commentsName nvarchar(150),
    @productName nvarchar(150),
    @countryName nvarchar(150),
    @currencyName nvarchar(150),
    @exchangeRateName nvarchar(150),
    @allowRecurring bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--update [other] set modifiedOn = @modifiedon, modifiedby = @modifiedby, productFieldType = @productFieldType, supplierFieldType = @supplierFieldType, purchaseOrderNumberName = @purchaseOrderNumberName, supplierName = @supplierName, dateApprovedName = @dateApprovedName, totalName = @totalName, orderRecurrenceName = @orderRecurrenceName, orderEndDateName = @orderEndDateName, commentsName = @commentsName, productName = @productName, countryName = @countryName, currencyName = @currencyName, exchangeRateName = @exchangeRateName, allowRecurring = @allowRecurring
END
