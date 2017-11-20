ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_allowanceamount] DEFAULT (0) FOR [allowanceamount];

