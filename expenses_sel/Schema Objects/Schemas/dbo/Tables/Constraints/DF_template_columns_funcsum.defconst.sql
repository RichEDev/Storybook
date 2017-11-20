ALTER TABLE [dbo].[template_columns]
    ADD CONSTRAINT [DF_template_columns_funcsum] DEFAULT (0) FOR [funcsum];

