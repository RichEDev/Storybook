ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_reimbursable] DEFAULT (1) FOR [reimbursable];

