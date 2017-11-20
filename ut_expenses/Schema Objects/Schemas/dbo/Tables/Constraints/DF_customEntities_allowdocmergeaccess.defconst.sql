ALTER TABLE [dbo].[customEntities]
    ADD CONSTRAINT [DF_customEntities_allowdocmergeaccess] DEFAULT ((0)) FOR [allowdocmergeaccess];

