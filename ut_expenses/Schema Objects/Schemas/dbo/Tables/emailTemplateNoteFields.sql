CREATE TABLE [dbo].[emailTemplateNoteFields](
	[emailtemplateid] [int] NOT NULL,
	[fieldid] [uniqueidentifier] NOT NULL,
	[emailfieldtype] [tinyint] NOT NULL,
	[emailNoteFieldID] [int] IDENTITY(1,1) NOT NULL,
	[joinViaId] [int] NULL,
 CONSTRAINT [PK_emailTemplateNoteFields_1] PRIMARY KEY CLUSTERED 
(
	[emailNoteFieldID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] 
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[emailTemplateNoteFields]  WITH CHECK ADD  CONSTRAINT [FK_emailTemplateNoteFields_emailTemplates] FOREIGN KEY([emailtemplateid])
REFERENCES [dbo].[emailTemplates] ([emailtemplateid])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[emailTemplateNoteFields] CHECK CONSTRAINT [FK_emailTemplateNoteFields_emailTemplates]
GO

ALTER TABLE [dbo].[emailTemplateNoteFields] ADD  DEFAULT ((0)) FOR [joinViaId]
GO


