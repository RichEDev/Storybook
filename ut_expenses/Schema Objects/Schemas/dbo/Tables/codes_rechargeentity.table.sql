CREATE TABLE [dbo].[codes_rechargeentity] (
    [entityId]     INT           IDENTITY (1, 1) NOT NULL,
    [subAccountId] INT           NULL,
    [Name]         VARCHAR (50)  NULL,
    [Code]         VARCHAR (20)  NULL,
    [Shared]       SMALLINT      NULL,
    [staffRep]     INT           NULL,
    [deputyRep]    INT           NULL,
    [accountMgr]   INT           NULL,
    [serviceMgr]   INT           NULL,
    [Sector]       VARCHAR (50)  NULL,
    [serviceLine]  VARCHAR (20)  NULL,
    [Notes]        VARCHAR (250) NULL,
    [Closed]       SMALLINT      NULL,
    [dateClosed]   DATETIME      NULL,
    [dateCeased]   DATETIME      NULL
);

