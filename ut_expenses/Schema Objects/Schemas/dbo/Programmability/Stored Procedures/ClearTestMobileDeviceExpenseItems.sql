
-- =============================================
-- Create date: 03/10/2012
-- Description: Clears down the mobile device test data for receipts
-- =============================================
CREATE PROCEDURE [dbo].[ClearTestMobileDeviceExpenseItems] 
AS
BEGIN
 SET NOCOUNT ON;

 delete from [mobileExpenseItems] where subcatid in (select subcatid from subcats where subcat = 'MobileItemHasReceiptTrueTest Subcat');
 delete  from subcats where subcat = 'MobileItemHasReceiptTrueTest Subcat';
END