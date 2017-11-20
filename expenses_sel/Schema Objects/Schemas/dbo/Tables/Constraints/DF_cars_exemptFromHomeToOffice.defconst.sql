ALTER TABLE [dbo].[cars]
    ADD CONSTRAINT [DF_cars_exemptFromHomeToOffice] DEFAULT ((0)) FOR [exemptFromHomeToOffice];

