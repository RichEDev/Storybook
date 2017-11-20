CREATE TABLE [dbo].[groups] (
    [groupid]								INT             IDENTITY (1, 1)  NOT NULL,
    [groupname]								NVARCHAR (50)   NOT NULL,
    [description]							NVARCHAR (4000) NULL,
    [CreatedOn]								DATETIME        NULL,
    [CreatedBy]								INT             NULL,
	[MailClaimantWhenEnvelopeReceived]		BIT NULL DEFAULT(NULL),
	[MailClaimantWhenEnvelopeNotReceived]	BIT NULL DEFAULT(NULL),
	[ModifiedOn]							DATETIME        NULL,
    [ModifiedBy]							INT             NULL,
    [oneClickAuthorisation]					BIT             NOT NULL	
);

