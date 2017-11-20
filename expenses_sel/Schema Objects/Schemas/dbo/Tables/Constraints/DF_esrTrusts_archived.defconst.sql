ALTER TABLE [dbo].[esrTrusts]
    ADD CONSTRAINT [DF_esrTrusts_archived] DEFAULT ((0)) FOR [archived];

