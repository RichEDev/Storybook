ALTER TABLE [dbo].[faqs]
    ADD CONSTRAINT [DF_faqs_datecreated] DEFAULT (getdate()) FOR [datecreated];

