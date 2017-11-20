ALTER TABLE [dbo].[codes_inflatormetrics]
    ADD CONSTRAINT [DF_codes_inflatormetrics_requiresExtraPct] DEFAULT ((0)) FOR [requiresExtraPct];

