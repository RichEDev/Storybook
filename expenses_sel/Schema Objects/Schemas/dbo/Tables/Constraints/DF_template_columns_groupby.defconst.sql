ALTER TABLE [dbo].[template_columns]
    ADD CONSTRAINT [DF_template_columns_groupby] DEFAULT (0) FOR [groupby];

