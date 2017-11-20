
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[savePurchaseOrderHistory]
	@purchaseOrderID INT,
	@comment NVARCHAR(MAX),
	@createdByString NVARCHAR(150),
	@createdOn DATETIME,
	@createdBy int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO purchaseOrderHistory (purchaseorderid, comment, createdbystring, createdon, createdby) VALUES (@purchaseOrderID, @comment, @createdByString, @createdOn, @createdBy)
END

