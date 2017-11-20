CREATE TABLE [dbo].[supplier_contacts] (
    [contactid]          INT            IDENTITY (1, 1)  NOT NULL,
    [supplierid]         INT            NULL,
    [contactname]        NVARCHAR (200) NOT NULL,
    [position]           NVARCHAR (200) NULL,
    [email]              NVARCHAR (300) NULL,
    [mobile]             NVARCHAR (50)  NULL,
    [business_addressid] INT            NULL,
    [home_addressid]     INT            NULL,
    [comments]           NVARCHAR (MAX) NULL,
    [main_contact]       BIT            NOT NULL,
    [createdon]          DATETIME       NULL,
    [createdby]          INT            NULL,
    [modifiedon]         DATETIME       NULL,
    [modifiedby]         INT            NULL
);

