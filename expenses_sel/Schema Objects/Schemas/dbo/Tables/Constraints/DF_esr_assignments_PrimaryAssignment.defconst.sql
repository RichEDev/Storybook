ALTER TABLE [dbo].[esr_assignments]
    ADD CONSTRAINT [DF_esr_assignments_PrimaryAssignment] DEFAULT ((0)) FOR [PrimaryAssignment];

