ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_pmiles] DEFAULT ((0)) FOR [pmiles];

