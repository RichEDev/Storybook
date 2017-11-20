ALTER TABLE [dbo].[custom_entities]
    ADD CONSTRAINT [DF_custom_entities_allowdocmergeaccess] DEFAULT ((0)) FOR [allowdocmergeaccess];

