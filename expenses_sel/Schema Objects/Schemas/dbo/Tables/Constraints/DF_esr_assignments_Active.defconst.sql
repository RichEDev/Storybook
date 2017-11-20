ALTER TABLE [dbo].[esr_assignments]
    ADD CONSTRAINT [DF_esr_assignments_Active] DEFAULT ((1)) FOR [Active];

