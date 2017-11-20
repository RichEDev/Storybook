ALTER TABLE [dbo].[custom_fields]
    ADD CONSTRAINT [DF__custom_fi__field__7A15877D] DEFAULT (newid()) FOR [fieldid];

