ALTER TABLE [dbo].[previouspasswords]
    ADD CONSTRAINT [DF_previouspasswords_order] DEFAULT (0) FOR [order];

