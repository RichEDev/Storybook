ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_primaryitem] DEFAULT (1) FOR [primaryitem];

