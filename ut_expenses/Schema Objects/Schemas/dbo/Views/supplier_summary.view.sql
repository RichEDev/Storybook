CREATE VIEW [dbo].[supplier_summary]
AS 
	SELECT [supplierId],[subAccountId],
	dbo.AttachmentCount(1,[supplier_details].[supplierId]) AS [SupplierAttachmentCount]
	FROM [supplier_details]

