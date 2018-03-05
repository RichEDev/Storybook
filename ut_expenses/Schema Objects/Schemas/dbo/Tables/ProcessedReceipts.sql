CREATE TABLE [dbo].[ProcessedReceipts]
(
	ProcessedReceiptId INT NOT NULL IDENTITY(1,1)
	,WalletReceiptId INT NOT NULL
	,[Date] DATETIME NULL
	,Total DECIMAL(18, 2) NULL
	,Merchant NVARCHAR(100) NULL
)