CREATE TABLE [dbo].[OrphanedReceipts] 
(
 [ReceiptId]   [int]   IDENTITY(1,1) NOT NULL,
 [FileExtension]  [nvarchar](6) NOT NULL,
 [CreationMethod] [tinyint]  NOT NULL,
 CONSTRAINT [PK_OrphanedReceipts] PRIMARY KEY CLUSTERED ([ReceiptId] ASC)
)