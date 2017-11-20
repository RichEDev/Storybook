ALTER TABLE [dbo].[mileage_categories]
    ADD CONSTRAINT [DF_mileage_categories_calcmilestotal] DEFAULT ((1)) FOR [calcmilestotal];

