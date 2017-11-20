ALTER TABLE [dbo].[global_faqs]
    ADD CONSTRAINT [DF_faqs_datecreated] DEFAULT (getdate()) FOR [datecreated];

