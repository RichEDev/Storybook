CREATE TABLE [dbo].[addressDistances](
	[AddressDistanceID] [int] IDENTITY(1,1) NOT NULL,
	[AddressIDA] [int] NOT NULL,
	[AddressIDB] [int] NOT NULL,
	[CustomDistance] [decimal](18, 2) NOT NULL,
	[PostcodeAnywhereFastestDistance] [decimal](18, 2) NULL,
	[PostcodeAnywhereShortestDistance] [decimal](18, 2) NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedOn] [datetime] NULL,
	[ModifiedBy] [int] NULL
	CONSTRAINT [PK_addressDistances] PRIMARY KEY CLUSTERED 
	(
		[AddressDistanceID] ASC
	)
)
GO

ALTER TABLE [dbo].[addressDistances] ADD  CONSTRAINT [FK_addressDistances_addresses_a] FOREIGN KEY([AddressIDA])
REFERENCES [dbo].[addresses] ([AddressID])
GO

ALTER TABLE [dbo].[addressDistances] CHECK CONSTRAINT [FK_addressDistances_addresses_a]
GO

ALTER TABLE [dbo].[addressDistances] ADD  CONSTRAINT [FK_addressDistances_addresses_b] FOREIGN KEY([AddressIDB])
REFERENCES [dbo].[addresses] ([AddressID])
GO

ALTER TABLE [dbo].[addressDistances] CHECK CONSTRAINT [FK_addressDistances_addresses_b]
GO