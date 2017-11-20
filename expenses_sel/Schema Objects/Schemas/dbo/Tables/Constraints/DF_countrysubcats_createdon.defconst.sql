ALTER TABLE [dbo].[countrysubcats]
    ADD CONSTRAINT [DF_countrysubcats_createdon] DEFAULT (getdate()) FOR [createdon];

