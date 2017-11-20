ALTER TABLE [dbo].[costcodes]
    ADD CONSTRAINT [DF_costcodes_archived] DEFAULT (0) FOR [archived];

