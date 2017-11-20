ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_staff] DEFAULT ((0)) FOR [staff];

