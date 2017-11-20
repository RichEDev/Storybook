ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_blitres] DEFAULT (0) FOR [blitres];

