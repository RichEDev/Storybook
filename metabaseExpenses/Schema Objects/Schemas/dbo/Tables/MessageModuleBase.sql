
CREATE TABLE [dbo].[MessageModuleBase](
	[ModuleId] [int] NOT NULL,
	[MessageId] [int] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[MessageModuleBase]  WITH CHECK ADD  CONSTRAINT [FK_MessageModuleBase_LogonMessages] FOREIGN KEY([MessageId])
REFERENCES [dbo].[LogonMessages] ([MessageId])
GO

ALTER TABLE [dbo].[MessageModuleBase] CHECK CONSTRAINT [FK_MessageModuleBase_LogonMessages]
GO

ALTER TABLE [dbo].[MessageModuleBase]  WITH CHECK ADD  CONSTRAINT [FK_MessageModuleBase_moduleBase] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[moduleBase] ([moduleID])
GO

ALTER TABLE [dbo].[MessageModuleBase] CHECK CONSTRAINT [FK_MessageModuleBase_moduleBase]
GO
