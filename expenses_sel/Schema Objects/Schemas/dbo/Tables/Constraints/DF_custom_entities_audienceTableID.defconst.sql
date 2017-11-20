ALTER TABLE [dbo].[custom_entities]
    ADD CONSTRAINT [DF_custom_entities_audienceTableID] DEFAULT (newid()) FOR [audienceTableID];

