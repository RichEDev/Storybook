ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_others] DEFAULT (0) FOR [others];

