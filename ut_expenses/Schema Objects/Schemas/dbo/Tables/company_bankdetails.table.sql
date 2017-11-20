CREATE TABLE [dbo].[company_bankdetails] (
    [bankreference] NVARCHAR (50) NULL,
    [accountnumber] NVARCHAR (50) NULL,
    [accounttype]   NVARCHAR (50) NULL,
    [sortcode]      NVARCHAR (50) NULL,
    [id]            INT           IDENTITY (1, 1)  NOT NULL
);

