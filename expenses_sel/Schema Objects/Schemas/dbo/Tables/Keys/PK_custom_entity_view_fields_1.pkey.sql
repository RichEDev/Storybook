ALTER TABLE [dbo].[custom_entity_view_fields]
    ADD CONSTRAINT [PK_custom_entity_view_fields_1] PRIMARY KEY CLUSTERED ([viewid] ASC, [fieldid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

