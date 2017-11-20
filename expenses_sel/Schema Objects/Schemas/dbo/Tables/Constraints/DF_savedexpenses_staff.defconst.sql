ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_staff] DEFAULT (0) FOR [staff];

