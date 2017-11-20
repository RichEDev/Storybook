ALTER TABLE [dbo].[floats]
    ADD CONSTRAINT [DF_floats_settled] DEFAULT (0) FOR [settled];

