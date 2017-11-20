CREATE TABLE [dbo].[addressPostcodeDistances] (
    [postcodeLookupID]    INT             IDENTITY (1, 1) NOT NULL,
    [fromAddressPostcode] NVARCHAR (15)   NOT NULL,
    [toAddressPostcode]   NVARCHAR (15)   NOT NULL,
    [lookupDate]          DATETIME        NOT NULL,
    [distance]            DECIMAL (18, 2) NOT NULL
);

