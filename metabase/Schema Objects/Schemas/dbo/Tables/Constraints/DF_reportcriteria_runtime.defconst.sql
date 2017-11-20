ALTER TABLE [dbo].[reportcriteria]
    ADD CONSTRAINT [DF_reportcriteria_runtime] DEFAULT ((0)) FOR [runtime];

