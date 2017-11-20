CREATE NONCLUSTERED INDEX [IX_Addresses_Archived_Obsolete_Country_GlobalIdentifier_AddressNameLookup] ON [dbo].[addresses]
(
	[Archived] ASC,
	[Obsolete] ASC,
	[Country] ASC,
	[GlobalIdentifier] ASC,
	[AddressNameLookup] ASC
)
INCLUDE ( 	[AddressID],
	[Postcode],
	[Line1],
	[Line2],
	[Line3],
	[City],
	[County],
	[CreationMethod],
	[LookupDate],
	[SubAccountID],
	[Longitude],
	[Latitude],
	[CreatedOn],
	[CreatedBy],
	[ModifiedOn],
	[ModifiedBy],
	[AccountWideFavourite],
	[Line1Lookup],
	[Line2Lookup],
	[CityLookup],
	[PostcodeLookup],
	[AddressName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


