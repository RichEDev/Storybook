ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_norooms] DEFAULT ((0)) FOR [norooms];

