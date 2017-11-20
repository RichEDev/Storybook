ALTER TABLE [dbo].[template_columns]
    ADD CONSTRAINT [DF_template_columns_hidden] DEFAULT (0) FOR [hidden];

