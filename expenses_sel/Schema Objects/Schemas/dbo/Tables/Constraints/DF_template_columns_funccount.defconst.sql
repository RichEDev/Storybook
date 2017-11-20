ALTER TABLE [dbo].[template_columns]
    ADD CONSTRAINT [DF_template_columns_funccount] DEFAULT (0) FOR [funccount];

