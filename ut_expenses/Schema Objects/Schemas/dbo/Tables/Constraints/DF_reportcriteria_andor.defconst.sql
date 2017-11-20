ALTER TABLE [dbo].[reportcriteria]
    ADD CONSTRAINT [DF_reportcriteria_andor] DEFAULT (0) FOR [andor];

