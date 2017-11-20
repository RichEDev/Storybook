CREATE NONCLUSTERED INDEX [IX_Addresses_CityLookup] ON [dbo].[addresses] 
(
 [CityLookup] ASC
)
INCLUDE ( [AddressID],
[Postcode],
[AddressName],
[Line1],
[Line2],
[Line3],
[City],
[County],
[Country],
[CreationMethod],
[LookupDate],
[SubAccountID],
[Longitude],
[Latitude],
[CreatedOn],
[CreatedBy],
[ModifiedOn],
[ModifiedBy],
[GlobalIdentifier],
[Archived]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]