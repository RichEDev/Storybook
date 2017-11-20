ALTER TABLE [dbo].[custom_entity_form_fields]
    ADD CONSTRAINT [PK_custom_entity_form_fields] PRIMARY KEY CLUSTERED ([formid] ASC, [attributeid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

