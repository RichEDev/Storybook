ALTER TABLE [dbo].[template_columns]
    ADD CONSTRAINT [DF_template_columns_funcmin] DEFAULT (0) FOR [funcmin];

