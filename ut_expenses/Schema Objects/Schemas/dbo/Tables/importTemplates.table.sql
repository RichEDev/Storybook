CREATE TABLE [dbo].[importTemplates] (
    [templateID]      INT            IDENTITY (1, 1)  NOT NULL,
    [templateName]    NVARCHAR (150) NOT NULL,
    [applicationType] TINYINT        NOT NULL,
    [isAutomated]     BIT            NOT NULL,
    [NHSTrustID]      INT            NULL,
    [createdOn]       DATETIME       NULL,
    [createdBy]       INT            NULL,
    [modifiedOn]      DATETIME       NULL,
    [modifiedBy]      INT            NULL,
	SignOffOwnerFieldId	uniqueidentifier,
	LineManagerFieldId	uniqueidentifier
);
