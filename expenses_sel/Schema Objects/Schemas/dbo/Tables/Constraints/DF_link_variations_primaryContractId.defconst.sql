ALTER TABLE [dbo].[link_variations]
    ADD CONSTRAINT [DF_link_variations_primaryContractId] DEFAULT ((0)) FOR [primaryContractId];

