ALTER TABLE [dbo].[template_columns]
    ADD CONSTRAINT [DF_template_columns_funcmax] DEFAULT (0) FOR [funcmax];

