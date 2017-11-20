ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_norooms] DEFAULT (0) FOR [norooms];

