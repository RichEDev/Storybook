ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_plitres] DEFAULT ((0)) FOR [plitres];

