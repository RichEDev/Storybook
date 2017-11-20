ALTER TABLE [dbo].[faqs]
    ADD CONSTRAINT [FK_faqs_faqcategories] FOREIGN KEY ([faqcategoryid]) REFERENCES [dbo].[faqcategories] ([faqcategoryid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

