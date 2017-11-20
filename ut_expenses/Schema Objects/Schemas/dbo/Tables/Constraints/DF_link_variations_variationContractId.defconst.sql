ALTER TABLE [dbo].[link_variations]
    ADD CONSTRAINT [DF_link_variations_variationContractId] DEFAULT ((0)) FOR [variationContractId];

