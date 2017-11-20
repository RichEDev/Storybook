ALTER TABLE [dbo].[countrysubcats]
    ADD CONSTRAINT [DF_countrysubcats_vat] DEFAULT (0) FOR [vat];

