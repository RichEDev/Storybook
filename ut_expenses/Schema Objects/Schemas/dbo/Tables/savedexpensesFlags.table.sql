CREATE TABLE [dbo].[savedexpensesFlags]
(
[flaggedItemId] [int] NOT NULL IDENTITY(1, 1),
[expenseid] [int] NOT NULL,
[flagID] [int] NULL,
[flagType] [tinyint] NOT NULL,
[flagDescription] [nvarchar] (max) COLLATE Latin1_General_CI_AS NOT NULL,
[flagText] [nvarchar] (max) COLLATE Latin1_General_CI_AS NULL,
[duplicateExpenseID] [int] NULL,
[flagColour] [tinyint] NOT NULL CONSTRAINT [DF_savedexpensesFlags_flagColour] DEFAULT ((2)),
[claimantJustification] [nvarchar] (max) COLLATE Latin1_General_CI_AS NULL,
[exceededLimit] [decimal] (18, 2) NULL,
[stepNumber] [tinyint] NULL,
[claimantJustificationDelegateID] [int] NULL

)


GO
ALTER TABLE [dbo].[savedexpensesFlags] ADD CONSTRAINT [FK_savedexpensesFlags_savedexpenses] FOREIGN KEY ([expenseid]) REFERENCES [dbo].[savedexpenses] ([expenseid]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[savedexpensesFlags] ADD CONSTRAINT [FK_savedexpensesFlags_savedexpenses1] FOREIGN KEY ([duplicateExpenseID]) REFERENCES [dbo].[savedexpenses] ([expenseid])
GO
-- Constraints and Indexes

ALTER TABLE [dbo].[savedexpensesFlags] ADD CONSTRAINT [CK_savedexpensesFlags] CHECK (([flagColour]=(1) OR [flagColour]=(2) OR [flagColour]=(3)))
GO
ALTER TABLE [dbo].[savedexpensesFlags] ADD CONSTRAINT [PK_savedexpensesFlags_1] PRIMARY KEY CLUSTERED  ([flaggedItemId])
GO
-- Extended Properties

EXEC sp_addextendedproperty N'MS_Description', N'1=amber;2=red', 'SCHEMA', N'dbo', 'TABLE', N'savedexpensesFlags', 'COLUMN', N'flagColour'