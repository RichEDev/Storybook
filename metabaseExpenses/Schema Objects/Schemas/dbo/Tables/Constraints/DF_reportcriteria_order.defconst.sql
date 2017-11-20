ALTER TABLE [dbo].[reportcriteria]
    ADD CONSTRAINT [DF_reportcriteria_order] DEFAULT ((1)) FOR [order];

