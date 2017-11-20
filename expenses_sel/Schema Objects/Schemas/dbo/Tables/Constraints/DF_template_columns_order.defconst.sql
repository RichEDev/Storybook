ALTER TABLE [dbo].[template_columns]
    ADD CONSTRAINT [DF_template_columns_order] DEFAULT (1) FOR [order];

