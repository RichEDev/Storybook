ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_allowance] DEFAULT (0) FOR [allowance];

