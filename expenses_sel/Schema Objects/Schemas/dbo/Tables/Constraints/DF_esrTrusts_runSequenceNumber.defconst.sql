ALTER TABLE [dbo].[esrTrusts]
    ADD CONSTRAINT [DF_esrTrusts_runSequenceNumber] DEFAULT ((0)) FOR [runSequenceNumber];

