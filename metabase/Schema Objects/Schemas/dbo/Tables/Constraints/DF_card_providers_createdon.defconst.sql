ALTER TABLE [dbo].[card_providers]
    ADD CONSTRAINT [DF_card_providers_createdon] DEFAULT (getdate()) FOR [createdon];

