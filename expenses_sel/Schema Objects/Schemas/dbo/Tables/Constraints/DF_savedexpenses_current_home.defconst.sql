ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_home] DEFAULT ((0)) FOR [home];

