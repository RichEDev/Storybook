CREATE TABLE [dbo].[companydetails] (
    [companyname]        NVARCHAR (50) NULL,
    [address1]           NVARCHAR (50) NULL,
    [address2]           NVARCHAR (50) NULL,
    [city]               NVARCHAR (50) NULL,
    [county]             NVARCHAR (50) NULL,
    [postcode]           NVARCHAR (50) NULL,
    [telno]              NVARCHAR (50) NULL,
    [faxno]              NVARCHAR (50) NULL,
    [email]              NVARCHAR (50) NULL,
    [financialyearstart] NVARCHAR (5)  NULL,
    [financialyearend]   NVARCHAR (5)  NULL,
    [companynumber]      NVARCHAR (50) NULL,
    [id]                 INT           IDENTITY (1, 1)  NOT NULL
);

