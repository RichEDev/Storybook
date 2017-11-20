ALTER TABLE [dbo].[custom_entity_form_sections]
    ADD CONSTRAINT [IX_custom_entity_form_sections] UNIQUE NONCLUSTERED ([formid] ASC, [header_caption] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF) ON [PRIMARY];

