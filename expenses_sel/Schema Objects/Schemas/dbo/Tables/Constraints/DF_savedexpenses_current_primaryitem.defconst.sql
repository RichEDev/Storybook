ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_primaryitem] DEFAULT ((1)) FOR [primaryitem];

