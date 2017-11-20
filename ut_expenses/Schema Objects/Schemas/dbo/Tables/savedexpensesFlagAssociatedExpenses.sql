CREATE TABLE [dbo].[savedexpensesFlagAssociatedExpenses](
	[associatedExpenseId] [int] NOT NULL,
	[flaggedItemId] [int] NOT NULL,
 CONSTRAINT [PK_savedexpensesFlagAssociatedExpenses] PRIMARY KEY CLUSTERED 
(
	[associatedExpenseId] ASC,
	[flaggedItemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

