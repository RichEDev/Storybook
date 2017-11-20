ALTER TABLE [dbo].[link_variations]
    ADD CONSTRAINT [DF_link_variations_Closed] DEFAULT ((0)) FOR [Closed];

