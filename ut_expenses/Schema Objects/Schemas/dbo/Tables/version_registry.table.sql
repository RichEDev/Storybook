CREATE TABLE [dbo].[version_registry] (
    [RegistryId]        INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [ContractId]        INT           NOT NULL,
    [ProductId]         INT           NOT NULL,
    [Version]           NVARCHAR (30) NULL,
    [Version Order]     INT           NULL,
    [Quantity]          INT           NULL,
    [FirstObtainedDate] DATETIME      NULL,
    CONSTRAINT [PK_Version_Registry] PRIMARY KEY CLUSTERED ([RegistryId] ASC)
);



