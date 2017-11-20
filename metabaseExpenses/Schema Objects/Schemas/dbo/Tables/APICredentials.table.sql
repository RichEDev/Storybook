CREATE TABLE [dbo].[APICredentials] (
    [CredentialID] INT           IDENTITY (1, 1) NOT NULL,
    [Username]     NVARCHAR (50) COLLATE Latin1_General_CI_AS NOT NULL,
    [Password]     NVARCHAR (50) COLLATE Latin1_General_CI_AS NULL,
    [Active]       BIT           NOT NULL,
    [ProviderID]   INT           NOT NULL,
    [CreatedOn]    DATE          NULL,
    [ModifiedOn]   DATE          NULL
);

