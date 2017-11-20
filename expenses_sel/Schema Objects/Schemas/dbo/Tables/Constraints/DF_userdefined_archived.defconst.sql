ALTER TABLE [dbo].[userdefined]
    ADD CONSTRAINT [DF_userdefined_archived] DEFAULT ((0)) FOR [archived];

