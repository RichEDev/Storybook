ALTER TABLE [dbo].[returnedexpenses]
    ADD CONSTRAINT [DF_returnedexpenses_corrected] DEFAULT (0) FOR [corrected];

