CREATE TABLE [dbo].[supplier_addresses] (
    [addressid]       INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [address_title]   NVARCHAR (250) NULL,
    [addr_line1]      NVARCHAR (150) NULL,
    [addr_line2]      NVARCHAR (150) NULL,
    [town]            NVARCHAR (150) NULL,
    [county]          NVARCHAR (150) NULL,
    [postcode]        NVARCHAR (10)  NULL,
    [countryid]       INT            NULL,
    [switchboard]     NVARCHAR (30)  NULL,
    [fax]             NVARCHAR (30)  NULL,
    [private_address] BIT            NOT NULL,
    [createdon]       DATETIME       NULL,
    [createdby]       INT            NULL,
    [modifiedon]      DATETIME       NULL,
    [modifiedby]      INT            NULL,
    [supplierid]      INT            NULL
);

