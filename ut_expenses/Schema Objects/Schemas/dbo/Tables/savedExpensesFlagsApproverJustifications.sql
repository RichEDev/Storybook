-- Columns

CREATE TABLE [dbo].[savedExpensesFlagsApproverJustifications](
	[historyID] [int] IDENTITY(1,1) NOT NULL,
	[flaggedItemId] [int] NOT NULL,
	[stage] [tinyint] NOT NULL,
	[approverId] [int] NOT NULL,
	[justification] [nvarchar](max) NOT NULL,
	[datestamp] [datetime] NOT NULL,
	[delegateID] [int] NULL,
 CONSTRAINT [IX_savedExpensesFlagsApproverJustifications] UNIQUE NONCLUSTERED 
(
	[flaggedItemId] ASC,
	[stage] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[savedExpensesFlagsApproverJustifications]  WITH CHECK ADD  CONSTRAINT [FK_savedExpensesFlagsApproverJustifications_savedexpensesFlags] FOREIGN KEY([flaggedItemId])
REFERENCES [dbo].[savedexpensesFlags] ([flaggedItemId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[savedExpensesFlagsApproverJustifications] CHECK CONSTRAINT [FK_savedExpensesFlagsApproverJustifications_savedexpensesFlags]
GO

ALTER TABLE [dbo].[savedExpensesFlagsApproverJustifications] ADD  CONSTRAINT [DF_savedExpensesFlagsApproverJustifications_datestamp]  DEFAULT (getdate()) FOR [datestamp]
GO