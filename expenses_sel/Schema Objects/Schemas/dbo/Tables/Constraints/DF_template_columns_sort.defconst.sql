ALTER TABLE [dbo].[template_columns]
    ADD CONSTRAINT [DF_template_columns_sort] DEFAULT (0) FOR [sort];

