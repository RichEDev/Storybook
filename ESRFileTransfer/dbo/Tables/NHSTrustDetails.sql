CREATE TABLE [dbo].[NHSTrustDetails] (
    [trustID]     INT            IDENTITY (1, 1) NOT NULL,
    [AccountID]   INT            NOT NULL,
    [expTrustID]  INT            NOT NULL,
    [TrustName]   NVARCHAR (150) NOT NULL,
    [TrustVPD]    NVARCHAR (3)   NOT NULL,
    [FTPAddress]  NVARCHAR (100) NULL,
    [FTPUsername] NVARCHAR (100) NULL,
    [FTPPassword] NVARCHAR (100) NULL,
    [archived]    BIT            CONSTRAINT [DF_NHSTrustDetails_archived] DEFAULT ((0)) NOT NULL,
    [CreatedOn]   DATETIME       NULL,
    [ModifiedOn]  DATETIME       NULL,
    CONSTRAINT [PK_NHSTrustDetails] PRIMARY KEY CLUSTERED ([trustID] ASC)
);

