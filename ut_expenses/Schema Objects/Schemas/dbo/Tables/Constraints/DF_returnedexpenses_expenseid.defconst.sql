ALTER TABLE [dbo].[returnedexpenses]
    ADD CONSTRAINT [DF_returnedexpenses_expenseid] DEFAULT (0) FOR [expenseid];

