ALTER TABLE [dbo].[customEntities]
    ADD CONSTRAINT [DF_customEntities_audienceTableID] DEFAULT (newid()) FOR [audienceTableID];

