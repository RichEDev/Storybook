CREATE TABLE [dbo].[AddressLocatorDistances](
	[AddressLocatorDistanceId] [int] IDENTITY(1,1) NOT NULL,
	[OriginLocator] [nvarchar](15) NOT NULL,
	[DestinationLocator] [nvarchar](15) NOT NULL,
	[LookupDate] [datetime] DEFAULT (getdate()) NOT NULL,
	[FastestDistanceWithPostcodeCentres] [decimal](18, 2) NULL,
	[ShortestDistanceWithPostcodeCentres] [decimal](18, 2) NULL,
	[FastestDistanceWithRoads] [decimal](18, 2) NULL,
	[ShortestDistanceWithRoads] [decimal](18, 2) NULL,
	CONSTRAINT [PK_addressLocatorDistances] PRIMARY KEY CLUSTERED ([AddressLocatorDistanceId] ASC)
);