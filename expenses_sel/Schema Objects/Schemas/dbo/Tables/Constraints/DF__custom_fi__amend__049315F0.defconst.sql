ALTER TABLE [dbo].[custom_fields]
    ADD CONSTRAINT [DF__custom_fi__amend__049315F0] DEFAULT (getdate()) FOR [amendedon];

