CREATE TABLE [dbo].[Receipts] 
(
	[ReceiptId]			[int]			IDENTITY(1,1) NOT NULL,
	[FileExtension]		[nvarchar](6)	NOT NULL,
	[CreationMethod]	[tinyint]		NOT NULL,	
	[ClaimId]			[int]			NULL,
	[UserId]			[int]			NULL,
	[Deleted]			[bit]			NOT NULL,
	[ExpediteUsername]	[nvarchar](50)	NULL,
	[EnvelopeId]		[int]			NULL,	
	[CreatedOn]			[datetime]		NULL,
	[CreatedBy]			[int]			NULL,
	[ModifiedOn]		[datetime]		NULL,
	[ModifiedBy]		[int]			NULL,
	CONSTRAINT [PK_Receipts] PRIMARY KEY CLUSTERED ([ReceiptId] ASC),
	CONSTRAINT [FK_Receipts_claims] FOREIGN KEY ([ClaimId]) REFERENCES [claims_base] ([claimid]),
	CONSTRAINT [FK_Receipts_Users] FOREIGN KEY ([UserId]) REFERENCES [employees] ([employeeid])
)
GO
CREATE INDEX Index_Receipts_Deleted ON [dbo].[Receipts] (Deleted)
GO
CREATE INDEX Index_Receipts_ClaimId ON [dbo].[Receipts] (ClaimId)
GO
CREATE INDEX Index_Receipts_UserId ON [dbo].[Receipts] (UserId)
GO