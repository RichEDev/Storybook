ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_printout] DEFAULT ((0)) FOR [printout];

