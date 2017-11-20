ALTER TABLE [dbo].[floats]
    ADD CONSTRAINT [DF_floats_approved] DEFAULT (0) FOR [approved];

