CREATE TABLE [dbo].[favourites] (
	[FavouriteID]	INT IDENTITY(1,1) NOT NULL,
	[EmployeeID]	INT NULL,
	[AddressID]		INT NULL,
	CONSTRAINT [PK_favourite] PRIMARY KEY CLUSTERED ([FavouriteID] ASC)
);