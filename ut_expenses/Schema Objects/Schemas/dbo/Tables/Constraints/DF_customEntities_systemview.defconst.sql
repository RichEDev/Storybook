ALTER TABLE [dbo].[customEntities]
    ADD CONSTRAINT [DF_customEntities_systemview] DEFAULT ((0)) FOR [systemview];

