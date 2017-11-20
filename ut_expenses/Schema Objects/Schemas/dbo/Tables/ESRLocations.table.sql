create table dbo.ESRLocations
(
	ESRLocationId bigint not null,
	LocationCode nvarchar(60) null,
	Description nvarchar(240) null,
	InactiveDate datetime null,
	AddressLine1 nvarchar(240) null,
	AddressLine2 nvarchar(240) null,
	AddressLine3 nvarchar(240) null,
	Town nvarchar(30) null,
	County nvarchar(70) null,
	Postcode nvarchar(30) null,
	Country nvarchar(60) null,
	Telephone nvarchar(60) null,
	Fax nvarchar(60) null,
	PayslipDeliveryPoint nvarchar(1) null,
	SiteCode nvarchar(2) null,
	WelshLocationTranslation nvarchar(60) null,
	WelshAddress1 nvarchar(60) null,
	WelshAddress2 nvarchar(60) null,
	WelshAddress3 nvarchar(60) null,
	WelshTownTranslation nvarchar(60) null,
	ESRLastUpdate datetime not null
)