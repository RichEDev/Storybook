ALTER TABLE [dbo].[global_faqs]
    ADD CONSTRAINT [FK_faqs_global_faqcategories] FOREIGN KEY ([faqcategoryid]) REFERENCES [dbo].[global_faqcategories] ([faqcategoryid]) ON DELETE NO ACTION ON UPDATE NO ACTION NOT FOR REPLICATION;

