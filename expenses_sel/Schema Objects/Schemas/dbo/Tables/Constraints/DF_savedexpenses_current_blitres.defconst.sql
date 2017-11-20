ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_blitres] DEFAULT ((0)) FOR [blitres];

