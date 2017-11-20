ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_relabel] DEFAULT ((0)) FOR [relabel];

