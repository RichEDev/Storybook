ALTER TABLE [dbo].[template_columns]
    ADD CONSTRAINT [DF_template_columns_runtime] DEFAULT (0) FOR [runtime];

