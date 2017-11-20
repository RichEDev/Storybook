ALTER TABLE [dbo].[customEntityAttributes]
    ADD CONSTRAINT [DF_customEntityAttributes_is_audit_identity] DEFAULT 0 FOR [is_audit_identity];

