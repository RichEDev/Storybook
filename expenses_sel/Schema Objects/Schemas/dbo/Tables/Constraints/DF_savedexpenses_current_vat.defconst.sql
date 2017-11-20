ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_vat] DEFAULT ((0)) FOR [vat];

