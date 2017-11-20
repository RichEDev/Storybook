CREATE TABLE [dbo].[emailTemplateBodyFields] (
    [emailtemplateid]  INT              NOT NULL,
    [fieldid]          UNIQUEIDENTIFIER NOT NULL,
    [emailfieldtype]   TINYINT          NOT NULL,
    [emailBodyFieldID] INT              IDENTITY (1, 1) NOT NULL,
	[joinViaId]		   INT				NULL
);

