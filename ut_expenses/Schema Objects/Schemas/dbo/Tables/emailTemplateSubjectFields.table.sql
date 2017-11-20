CREATE TABLE [dbo].[emailTemplateSubjectFields] (
    [emailtemplateid]     INT              NOT NULL,
    [fieldid]             UNIQUEIDENTIFIER NOT NULL,
    [emailfieldtype]      TINYINT          NOT NULL,
    [emailSubjectFieldID] INT              IDENTITY (1, 1) NOT NULL,
	[joinViaId]           INT              NULL
);

