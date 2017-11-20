ALTER TABLE [dbo].[floats]
    ADD CONSTRAINT [DF_floats_disputed] DEFAULT (0) FOR [disputed];

