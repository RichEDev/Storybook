CREATE TABLE [dbo].[organisationAddresses] (
	[OrganisationID]	INT NOT NULL,
	[AddressID]			INT NOT NULL,
	CONSTRAINT [PK_organisationAddress] PRIMARY KEY CLUSTERED ([OrganisationID] ASC, [AddressID] ASC)
);