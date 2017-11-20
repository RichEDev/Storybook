CREATE TABLE [dbo].[CustomMenu](
	[CustomMenuId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[ParentMenuId] [int] NULL,
	[MenuIcon] [nvarchar](200) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedBy] [int] NULL,
	[OrderBy] [int] NULL,
	[SystemMenu] [bit] NOT NULL,
 CONSTRAINT [PK_Custommenu_1] PRIMARY KEY CLUSTERED 
(
	[CustomMenuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CustomMenu] ADD  CONSTRAINT [DF_CustomMenu_SystemMenu]  DEFAULT ((0)) FOR [SystemMenu]
GO

ALTER TABLE [dbo].[CustomMenu]  WITH CHECK ADD  CONSTRAINT [FK_CustomMenu_CustomMenu] FOREIGN KEY([ParentMenuId])
REFERENCES [dbo].[CustomMenu] ([CustomMenuId])
GO

ALTER TABLE [dbo].[CustomMenu] CHECK CONSTRAINT [FK_CustomMenu_CustomMenu]
GO

ALTER TABLE [dbo].[CustomMenu]  WITH CHECK ADD  CONSTRAINT [FK_CustomMenu_employees] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[employees] ([employeeid])
GO

ALTER TABLE [dbo].[CustomMenu] CHECK CONSTRAINT [FK_CustomMenu_employees]
GO

ALTER TABLE [dbo].[CustomMenu]  WITH CHECK ADD  CONSTRAINT [FK_CustomMenu_employees1] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[employees] ([employeeid])
GO

ALTER TABLE [dbo].[CustomMenu] CHECK CONSTRAINT [FK_CustomMenu_employees1]
GO