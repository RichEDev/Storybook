ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_vatlimitwithout] DEFAULT (0) FOR [vatlimitwithout];

